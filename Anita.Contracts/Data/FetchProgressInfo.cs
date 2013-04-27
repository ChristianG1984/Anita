using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SachsenCoder.Anita.Contracts.Data
{
    public class FetchProgressInfo
    {
        public SearchCelebrityAnswerData CelebritySearchResult { get; set; }
        public LinkNodeInfo LinkNodeInfo { get; set; }
        public string PicUriPath { get; set; }
        public bool IsFinished { get; set; }
    }
}
