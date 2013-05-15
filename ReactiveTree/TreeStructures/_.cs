using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    public static class _
    {
        public static IEnumerable<KeyValuePair<int, T>> Indexes<T>(this IEnumerable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<int, T>>>() != null);

            int count = 0;
            foreach (var e in source)
            {
                yield return new KeyValuePair<int, T>(count, e);
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] second)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(second != null);

            return source.Concat(second.AsEnumerable());
        }
    }
}
