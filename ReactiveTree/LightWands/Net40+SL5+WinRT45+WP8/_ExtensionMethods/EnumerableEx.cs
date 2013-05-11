using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public static class EnumerableEx
    {
        public static IEnumerable<T> Empty<T>()
        {
            return new T[] { };
        }

        public static IEnumerable<T> Return<T>(T value)
        {
            return new T[] { value };
        }
    }
}
