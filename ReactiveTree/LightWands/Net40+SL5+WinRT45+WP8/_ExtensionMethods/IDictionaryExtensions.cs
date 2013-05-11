using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public static class IDictionaryExtensions
    {
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            TValue value;
            if (source.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return default(TValue);
            }
        }
    }
}
