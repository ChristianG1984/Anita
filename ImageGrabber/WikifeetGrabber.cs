using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using HtmlAgilityPack;
using System.Diagnostics;
using System.IO;

namespace ImageGrabber
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
            var webReq = (HttpWebRequest)WebRequest.Create(new Uri(_wikiBaseUri, @"perl/ajax.pl"));
            webReq.Method = "POST";
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (cancelToken.IsCancellationRequested == true) {
                return;
            }

            var streamToSend = webReq.GetRequestStream();
            streamToSend.Write(data, 0, data.Length);
            streamToSend.Close();
            streamToSend.Dispose();

            var webResp = (HttpWebResponse)webReq.GetResponse();
            var result = new List<SearchCelebrityAnswerData>();
            var answer = webResp.GetResponseStream();
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(answer, new UTF8Encoding());
            answer.Close();
            var tds = htmlDoc.DocumentNode.SelectNodes("//@onclick");
            if (tds == null) {
                SearchResult(UniqueData.Create(result.AsEnumerable(), searchText.Id));
                return;
            }

            foreach (var node in tds) {
                var attr = node.Attributes["onclick"].Value;
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

            var webResp = (HttpWebResponse)webReq.GetResponse();
            var answer = webResp.GetResponseStream();
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(answer, new UTF8Encoding());
            answer.Close();
            
            foreach (var linkNodeInfo in getNextLinkNode(htmlDoc)) {
                if (cancelToken.IsCancellationRequested == true) { return; }
                var a = linkNodeInfo.LinkNode;
                var picUriPath = a.Attributes["href"].Value;
                var thumbUriPath = a.ChildNodes["img"].Attributes["src"].Value;
                var progressInfo = new FetchProgressInfo
                {
                    CelebritySearchResult = metaInfos.Data.SearchCelebrityAnswerData,
                    LinkNodeInfo = linkNodeInfo,
                    PicUriPath = picUriPath
                };
                OutputFetchProgressInfo(progressInfo);
                fetchImage(localStoreRootPath, celebrityName, picUriPath);
            }
        }

        public event Action<UniqueData<IEnumerable<SearchCelebrityAnswerData>>> SearchResult;
        public event Action<FetchProgressInfo> OutputFetchProgressInfo;

        private IEnumerable<LinkNodeInfo> getNextLinkNode(HtmlDocument htmlDoc)
        {
            var cid = getCid(htmlDoc);
            var picsLeft = getPicsLeft(htmlDoc);
            var lastPid = getLastPid(htmlDoc);
            var linkNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='thepics']//a");
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
                var webReq = (HttpWebRequest)WebRequest.Create(new Uri(_wikiBaseUri, @"perl/ajax.pl"));
                webReq.Method = "POST";
                webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var streamToSend = webReq.GetRequestStream();
                streamToSend.Write(data, 0, data.Length);
                streamToSend.Close();
                streamToSend.Dispose();

                var webResp = (HttpWebResponse)webReq.GetResponse();
                var result = new List<SearchCelebrityAnswerData>();
                var answer = webResp.GetResponseStream();
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

        private static void fetchImage(string localStoreRootPath, string celebrityName, string picUriPath)
        {
            var targetFolder = Path.Combine(localStoreRootPath, celebrityName);
            var picUri = new Uri(picUriPath);

            if (Directory.Exists(targetFolder) == false) {
                Directory.CreateDirectory(targetFolder);
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
