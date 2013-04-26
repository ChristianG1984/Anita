using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageGrabber
{
    public class PacketHandler<TInput, TOutput>
    {
        public void InputPlain(TInput data)
        {
            lock (_lockObj) {
                _lastString = data.ToString();
            }
            OutputUnique(UniqueData.Create(data, _lastString));
        }

        public void InputUnique(UniqueData<TOutput> data)
        {
            bool equal;
            lock (_lockObj) {
                equal = data.Id == _lastString;
            }
            if (equal) {
                OutputPlain(data.Data);
            }
        }

        public event Action<TOutput> OutputPlain;
        public event Action<UniqueData<TInput>> OutputUnique;

        private string _lastString = string.Empty;
        private object _lockObj = new object();
    }
}
