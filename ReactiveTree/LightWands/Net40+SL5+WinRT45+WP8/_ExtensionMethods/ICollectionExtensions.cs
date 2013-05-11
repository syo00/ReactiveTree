using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(collection != null);

            foreach (var r in collection) source.Add(r);
        }
    }
}
