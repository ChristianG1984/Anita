using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace SachsenCoder.Anita.Contracts.Data
{
    public struct LinkNodeInfo
    {
        public int CurrentNumber { get; set; }
        public int MaxImageCount { get; set; }
        public HtmlNode LinkNode { get; set; }
        public Exception Error { get; set; }
    }
}
