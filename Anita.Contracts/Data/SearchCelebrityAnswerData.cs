using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SachsenCoder.Anita.Contracts.Data
{
    public class SearchCelebrityAnswerData
    {
        public SearchCelebrityAnswerData(string fullName, Uri uriPath) : this(fullName, fullName, uriPath) { }

        public SearchCelebrityAnswerData(string id, string fullName, Uri uriPath)
        {
            Id = id;
            FullName = fullName;
            UriPath = uriPath;
        }

        public override string ToString()
        {
            return FullName;
        }

        public string Id { private set; get; }
        public string FullName { private set; get; }
        public Uri UriPath { private set; get; }
    }
}
