using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SachsenCoder.Anita.Core.Leafs
{
    public class Synchronizer<T>
    {
        public Synchronizer()
        {
            _context = SynchronizationContext.Current;
        }

        public void Input(T data)
        {
            if (_context != null) {
                _context.Send((_) => Output(data), null);
                return;
            }
            Output(data);
        }

        public event Action<T> Output;

        private SynchronizationContext _context; 
    }
}
