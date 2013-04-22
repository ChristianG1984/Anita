using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGrabber
{
    public class ErrorData
    {
        public ErrorData(string description, Exception error)
        {
            Description = description;
            Error = error;
        }

        public string Description { get; private set; }
        public Exception Error { get; private set; }
    }
}
