using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageGrabber
{
    public sealed class CancelSource<T>
    {
        public CancelSource(CancellationTokenSource cancelSource, T data)
        {
            CancelTokenSource = cancelSource;
            Data = data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public CancellationTokenSource CancelTokenSource { get; private set; }
        public T Data { get; private set; }
    }
}
