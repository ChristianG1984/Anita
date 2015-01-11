using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using CsQuery;
using HtmlAgilityPack;
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
            //CQ dom = CQ.CreateFromUrl(celebrityUri.AbsoluteUri);
            //var htmlDoc = new HtmlDocument();
            //htmlDoc.Load(answer, new UTF8Encoding());
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
                var pid = (int)element.GetPropertyValue("pid");
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

            //foreach (var linkNodeInfo in getNextLinkNode(htmlDoc, dom)) {
            //    if (cancelToken.IsCancellationRequested == true) { return; }
            //    if (linkNodeInfo.Error != null) {
            //        OutputError(new ErrorData("FetchCelebrityPictures failed (enumerate linkNodeInfos)", linkNodeInfo.Error));
            //        continue;
            //    }
            //    string picUriPath = string.Empty;
            //    string thumbUriPath = string.Empty;
            //    HtmlNode a = linkNodeInfo.LinkNode;
            //    try {
            //        picUriPath = a.Attributes["href"].Value;
            //        thumbUriPath = a.ChildNodes["img"].Attributes["src"].Value;
            //    }
            //    catch (NullReferenceException ex) {
            //        OutputError(new ErrorData(string.Format("Problem with URL! The picture \"{0}\" will be skipped!", picUriPath), ex));
            //        continue;
            //    }
            //    var progressInfo = new FetchProgressInfo
            //    {
            //        CelebritySearchResult = metaInfos.Data.SearchCelebrityAnswerData,
            //        LinkNodeInfo = linkNodeInfo,
            //        PicUriPath = picUriPath
            //    };
            //    OutputFetchProgressInfo(progressInfo);
            //    fetchImage(localStoreRootPath, celebrityName, picUriPath);
            //}
            //OutputFetchProgressInfo(new FetchProgressInfo
            //{
            //    CelebritySearchResult = metaInfos.Data.SearchCelebrityAnswerData,
            //    IsFinished = true
            //});
        }

        private void processJavaScript(CQ dom, ScriptEngine jEngine)
        {
            var scriptPathNodes = dom.Select("script[src]");

            foreach (var node in scriptPathNodes) {
                var scriptUri = new Uri(node.Attributes["src"], UriKind.Relative);
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
                jEngine.Execute(script.ToString());
            }

            scriptPathNodes = dom.Select("div#conts script").First();
            jEngine.Execute(scriptPathNodes.Text());

            scriptPathNodes = dom.Select("div#thepics script");
            var strReader = new StringReader(scriptPathNodes.Text());
            jEngine.Execute(strReader.ReadLine());
            jEngine.Execute(strReader.ReadLine());
            strReader.Dispose();
        }

        public event Action<UniqueData<IEnumerable<SearchCelebrityAnswerData>>> SearchResult;
        public event Action<FetchProgressInfo> OutputFetchProgressInfo;
        public event Action<ErrorData> OutputError;

        private IEnumerable<LinkNodeInfo> getNextLinkNode(HtmlDocument htmlDoc, CQ dom)
        {
            Exception error = null;
            var cid = getCid(htmlDoc);
            var picsLeft = getPicsLeft(htmlDoc);
            var lastPid = getLastPid(htmlDoc);
            var linkNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='thepics']//a");
            var str = dom.Render();
            
            var linkNodes_new = dom.Select("div#conts script");
            var jEngine = new ScriptEngine();
            foreach (var node in linkNodes_new) {
                jEngine.Execute(node.InnerText);
            }
            var text = jEngine.GetGlobalValue<string>("messanger.cfname");
            var maxCount = linkNodes.Count + picsLeft;
            var currentNumber = 0;

            foreach (var node in linkNodes) {
                yield return new LinkNodeInfo
                {
                    CurrentNumber = ++currentNumber,
                    MaxImageCount = maxCount,
                    LinkNode = node
                };
            }

            while (picsLeft > 0) {
                var data = string.Format(@"req=morepics&cid={0}&lastpid={1}", cid, lastPid).ToUriString().ToUTF8Bytes();
                var webReq = (HttpWebRequest)WebRequest.Create(new Uri(_wikiBaseUri, @"perl/ajax.fpl"));
                webReq.Method = "POST";
                webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                Stream streamToSend = null;
                try {
                    streamToSend = webReq.GetRequestStream();
                }
                catch (Exception ex) {
                    error = ex;
                }
                
                if (error != null) {
                    yield return new LinkNodeInfo { Error = error };
                    yield break;
                }
                streamToSend.Write(data, 0, data.Length);
                streamToSend.Close();
                streamToSend.Dispose();

                HttpWebResponse webResp = null;
                try {
                    webResp = (HttpWebResponse)webReq.GetResponse();
                }
                catch (Exception ex) {
                    error = ex;
                }

                if (error != null) {
                    yield return new LinkNodeInfo { Error = error };
                    yield break;
                }
                var result = new List<SearchCelebrityAnswerData>();
                Stream answer = null;
                try {
                    answer = webResp.GetResponseStream();
                }
                catch (Exception ex) {
                    error = ex;
                }

                if (error != null) {
                    yield return new LinkNodeInfo { Error = error };
                    yield break;
                }
                var sr = new StreamReader(answer, new UTF8Encoding());
                var answerStrings = sr.ReadToEnd().Split('|');
                sr.Close();
                sr.Dispose();

                if (int.TryParse(answerStrings[1], out picsLeft) == false) {
                    yield break;
                }

                var lastPidString = answerStrings[2].Split(',').Last();
                if (int.TryParse(lastPidString, out lastPid) == false) {
                    yield break;
                }

                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(answerStrings[0]);
                linkNodes = htmlDoc.DocumentNode.SelectNodes("//a");

                foreach (var node in linkNodes) {
                    yield return new LinkNodeInfo
                    {
                        CurrentNumber = ++currentNumber,
                        MaxImageCount = maxCount,
                        LinkNode = node
                    };
                }
            }

            yield break;
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

        private static int getCid(HtmlDocument htmlDoc)
        {
            var scriptNode = htmlDoc.DocumentNode.SelectNodes("//script[not(@*)]");
            if (scriptNode == null) {
                return -1;
            }

            int cid;
            var cidString = scriptNode[0].InnerText.Split(';')[0].Split('=')[1].Trim();
            if (int.TryParse(cidString, out cid) == false) {
                return -1;
            }
            return cid;
        }

        private static int getPicsLeft(HtmlDocument htmlDoc)
        {
            var div = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='morepics']");
            if (div == null) {
                return -1;
            }
            if (string.IsNullOrWhiteSpace(div.InnerText) == true) {
                return 0;
            }

            int picsLeft;
            var picsLeftString = div.ChildNodes[HtmlNode.HtmlNodeTypeNameText].InnerText.Split(' ')[0];
            if (int.TryParse(picsLeftString, out picsLeft) == false) {
                return -1;
            }
            return picsLeft;
        }

        private static int getLastPid(HtmlDocument htmlDoc)
        {
            var button = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='morepics']//@onclick");
            if (button == null) {
                return -1;
            }

            int lastPid = 0;
            var lastPidString = button.Attributes["onclick"].Value;
            var start = lastPidString.IndexOf('(') + 1;
            var end = lastPidString.LastIndexOf(')');
            lastPidString = lastPidString.Substring(start, end - start);
            if (int.TryParse(lastPidString, out lastPid) == false) {
                return -1;
            }
            return lastPid;
        }

        private Uri _wikiBaseUri = new Uri("http://www.wikifeet.com", UriKind.Absolute);
    }
}
