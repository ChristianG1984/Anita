using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageGrabber
{
    sealed class CancelProvider<T>
    {
        public void Input(T data)
        {
            var cancelTokenSource = new CancellationTokenSource();
            OutputCancelSource(new CancelSource<T>(cancelTokenSource, data));
            OutputCancelTarget(new CancelTarget<T>(cancelTokenSource.Token, data));
        }

        public event Action<CancelSource<T>> OutputCancelSource;
        public event Action<CancelTarget<T>> OutputCancelTarget;
    }
}
