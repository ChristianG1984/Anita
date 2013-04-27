using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SachsenCoder.Anita.Contracts.Data
{
    public class UniqueData<T>
    {
        public UniqueData(T data, string id)
        {
            Data = data;
            Id = id;
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public T Data { get; private set; }
        public string Id { get; private set; }
    }

    public static class UniqueData
    {
        public static UniqueData<T> Create<T>(T value, string id)
        {
            return new UniqueData<T>(value, id);
        }
    }
}
