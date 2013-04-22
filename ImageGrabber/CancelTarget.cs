using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageGrabber
{
    public sealed class CancelTarget<T>
    {
        public CancelTarget(CancellationToken cancelTarget, T data)
        {
            CancelToken = cancelTarget;
            Data = data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public CancellationToken CancelToken { get; private set; }
        public T Data { get; private set; }
    }
}
