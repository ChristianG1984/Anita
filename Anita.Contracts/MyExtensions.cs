using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SachsenCoder.Anita.Contracts
{
    public static class MyExtensions
    {
        public static string ToUriString(this string str)
        {
            return Uri.EscapeUriString(str);
        }

        public static byte[] ToUTF8Bytes(this string str)
        {
            return UTF8Encoding.UTF8.GetBytes(str);
        }

        public static KeyValuePair<string, string> AsStorable(this string value, string key)
        {
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
