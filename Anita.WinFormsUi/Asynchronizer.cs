using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ImageGrabber
{
    public class Asynchronizer<T>
    {
        public void Input(T data)
        {
            var thread = new Thread(() => Output(data));
            thread.Name = "Asynchronizer";
            thread.Start();
        }

        public event Action<T> Output;
    }
}
