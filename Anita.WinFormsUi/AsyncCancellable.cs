using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SachsenCoder.Anita.Contracts.Data;
using SachsenCoder.Anita.Core.Leafs;

namespace ImageGrabber
{
    sealed class AsyncCancellable<T>
    {
        public AsyncCancellable()
        {
            var async = new Asynchronizer<CancelTarget<T>>();
            var cancelProvider = new CancelProvider<T>();

            _input += cancelProvider.Input;

            cancelProvider.OutputCancelSource += (data) => OutputCancelSource(data);
            cancelProvider.OutputCancelTarget += async.Input;

            async.Output += (data) => OutputCancelTarget(data);
        }

        public void Input(T data)
        {
            _input(data);
        }

        private event Action<T> _input;

        public event Action<CancelSource<T>> OutputCancelSource;
        public event Action<CancelTarget<T>> OutputCancelTarget;
    }
}
