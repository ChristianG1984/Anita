using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using CsQuery;
using Jurassic;
using System.Diagnostics;
using System.IO;
using SachsenCoder.Anita.Contracts.Data;
using SachsenCoder.Anita.Contracts;

namespace SachsenCoder.Anita.Core.Leafs
{
    public class WikifeetGrabber
    {
        public void SearchCelebrity(UniqueData<CancelTarget<string>> searchText)
        {
            var cancelToken = searchText.Data.CancelToken;

            if (cancelToken.IsCancellationRequested == true) {
                return;
            }
            
            var data = string.Format(@"req=suggest&value={0}", searchText.Data.Data).ToUriString().ToUTF8Bytes();
            var webReq = (HttpWebRequest)WebRequest.Create(new Uri(_wikiBaseUri, @"perl/ajax.fpl"));
            webReq.Method = "POST";
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (cancelToken.IsCancellationRequested == true) {
                return;
            }

            Stream streamToSend = null;
            try {
                streamToSend = webReq.GetRequestStream();
            }
            catch (Exception ex) {
                OutputError(new ErrorData("SearchCelebrity failed", ex));
                return;
            }
            streamToSend.Write(data, 0, data.Length);
            streamToSend.Close();
            streamToSend.Dispose();

            HttpWebResponse webResp = null;
            try {
                webResp = (HttpWebResponse)webReq.GetResponse();
            }
            catch (Exception ex) {
                OutputError(new ErrorData("SearchCelebrity failed", ex));
                return;
            }
            var result = new List<SearchCelebrityAnswerData>();
            Stream answer = null;
            try {
                answer = webResp.GetResponseStream();
            }
            catch (Exception ex) {
                OutputError(new ErrorData("SearchCelebrity failed", ex));
                return;
            }
            var dom = new CQ(answer, new UTF8Encoding());
            answer.Close();
            var tds = dom.Select("[onclick]");

            if (tds == null) {
                SearchResult(UniqueData.Create(result.AsEnumerable(), searchText.Id));
                return;
            }

            foreach (var node in tds) {
                var attr = node.Attributes["onclick"];
                var idx = attr.IndexOf(".value='") + 8;
                var sub = attr.Substring(idx);
                idx = sub.IndexOf("';");
                var name = sub.Substring(0, idx).Replace(@"\", string.Empty);
                idx = sub.IndexOf("encodeURI('") + 11;
                sub = sub.Substring(idx);
                idx = sub.IndexOf("')");
                var uriPart = sub.Substring(0, idx).Replace(@"\", string.Empty);
                result.Add(new SearchCelebrityAnswerData(name, new Uri(_wikiBaseUri, uriPart)));
            }

            SearchResult(UniqueData.Create(result.AsEnumerable(), searchText.Id));
        }

        public void FetchCelebrityPictures(CancelTarget<FetchCelebrityPicturesData> metaInfos)
        {
            var cancelToken = metaInfos.CancelToken;
            var celebrityName = metaInfos.Data.SearchCelebrityAnswerData.FullName;
            var celebrityUri = metaInfos.Data.SearchCelebrityAnswerData.UriPath;
            var localStoreRootPath = metaInfos.Data.BaseFolder;

            var webReq = (HttpWebRequest)WebRequest.Create(celebrityUri);
            webReq.Method = "GET";
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse webResp = null;
            Stream answer = null;
            try {
                webResp = (HttpWebResponse)webReq.GetResponse();
                answer = webResp.GetResponseStream();
            }
            catch (Exception ex) {
                OutputError(new ErrorData("FetchCelebrityPictures failed", ex));
                return;
            }
            CQ dom = new CQ(answer, new UTF8Encoding());
            var jEngine = new ScriptEngine();
            answer.Close();

            processJavaScript(dom, jEngine);

            var messanger = jEngine.GetGlobalValue<Jurassic.Library.ObjectInstance>("messanger");
            var cfname = (string)messanger.GetPropertyValue("cfname");
            var gdata = (Jurassic.Library.ArrayInstance)messanger.GetPropertyValue("gdata");
            var maxCount = gdata.Length;
            var currentNumber = 0;
            foreach (Jurassic.Library.ObjectInstance element in gdata.ElementValues) {
                if (cancelToken.IsCancellationRequested == true) { return; }
                ++currentNumber;
                var pid = int.Parse(element.GetPropertyValue("pid") as string);
                var picUriPath = "http://pics.wikifeet.com/" + cfname + "-Feet-" + pid + ".jpg";

                var progressInfo = new FetchProgressInfo
                {
                    CelebritySearchResult = metaInfos.Data.SearchCelebrityAnswerData,
                    LinkNodeInfo = new LinkNodeInfo
                    {
                        CurrentNumber = currentNumber,
                        MaxImageCount = (int)maxCount
                    },
                    PicUriPath = picUriPath
                };
                OutputFetchProgressInfo(progressInfo);
                fetchImage(localStoreRootPath, celebrityName, picUriPath);
            }
            OutputFetchProgressInfo(new FetchProgressInfo
            {
                CelebritySearchResult = metaInfos.Data.SearchCelebrityAnswerData,
                IsFinished = true
            });
        }

        public event Action<UniqueData<IEnumerable<SearchCelebrityAnswerData>>> SearchResult;
        public event Action<FetchProgressInfo> OutputFetchProgressInfo;
        public event Action<ErrorData> OutputError;

        private void processJavaScript(CQ dom, ScriptEngine jEngine)
        {
            var scriptPathNodes = dom.Select("script[src]");

            foreach (var node in scriptPathNodes) {
                Uri scriptUri;
                try {
                    scriptUri = new Uri(node.Attributes["src"], UriKind.Relative);
                }
                catch (UriFormatException) { continue; }
                var webReq = (HttpWebRequest)WebRequest.Create(new Uri(_wikiBaseUri, scriptUri));
                webReq.Method = "GET";
                webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                HttpWebResponse webResp = null;
                Stream answer = null;
                try {
                    webResp = (HttpWebResponse)webReq.GetResponse();
                    answer = webResp.GetResponseStream();
                }
                catch (Exception ex) {
                    OutputError(new ErrorData("FetchCelebrityPictures failed", ex));
                    return;
                }

                var sr = new StreamReader(answer);
                var script = new StringBuilder();
                script.AppendLine("var window = new Object();");
                script.Append(sr.ReadToEnd());
                sr.Close();
                sr.Dispose();
                try
                {
                    jEngine.Execute(script.ToString());
                }
                catch (JavaScriptException) { }
            }

            scriptPathNodes = dom.Select("div#conts script")[1].Cq();
            jEngine.Execute(scriptPathNodes.Text());

            scriptPathNodes = dom.Select("div#thepics script");
            var strReader = new StringReader(scriptPathNodes.Text());
            jEngine.Execute(strReader.ReadLine());
            jEngine.Execute(strReader.ReadLine());
            strReader.Dispose();
        }

        private void fetchImage(string localStoreRootPath, string celebrityName, string picUriPath)
        {
            var targetFolder = Path.Combine(localStoreRootPath, celebrityName);
            var picUri = new Uri(picUriPath);

            if (Directory.Exists(targetFolder) == false) {
                try {
                    Directory.CreateDirectory(targetFolder);
                }
                catch (Exception ex) {
                    OutputError(new ErrorData(string.Format("fetchImage failed! Could not create Directory:\r\n\"{0}\"", targetFolder), ex));
                    return;
                }
            }

            var targetFullPath = Path.Combine(targetFolder, picUri.Segments[1]);

            if (File.Exists(targetFullPath) == true) {
                return;
            }

            var webClient = new WebClient();
            try {
                webClient.DownloadFile(picUri, targetFullPath);
            }
            catch (Exception) {
                File.Delete(targetFullPath);
            }
            finally {
                webClient.Dispose();
            }
        }

        private Uri _wikiBaseUri = new Uri("http://www.wikifeet.com", UriKind.Absolute);
    }
}
