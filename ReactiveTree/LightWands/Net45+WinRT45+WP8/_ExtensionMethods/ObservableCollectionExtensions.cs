using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public static class ObservableCollectionExtensions
    {
        public static int ReplaceAll<T>(this ObservableCollection<T> source, T newItem, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(newItem != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            return source.ReplaceAll(t => newItem, predicate);
        }

        public static int ReplaceAll<T>(this ObservableCollection<T> source, Func<T, T> newItem, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(newItem != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            int matchedCount = 0;
            for (int i = 0; i <= source.Count - 1; i++)
            {
                var item = source[i];
                if (predicate(item))
                {
                    source[i] = newItem(item);
                    matchedCount++;
                }
            }
            return matchedCount;
        }

        public static ReadOnlyObservableCollection<T> ToReadOnly<T>(this ObservableCollection<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            return new ReadOnlyObservableCollection<T>(source);
        }

        /// <summary>一対一の射影により新たな ObservableCollection を作成します。</summary>
        public static ObservableCollection<TResult> SelectNew<Tfrom, TResult>(this ObservableCollection<Tfrom> source, Func<Tfrom, TResult> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            return INotifyCollectionChangedExtensions.SelectNew<Tfrom, TResult>(source, selector);
        }

        public static void Update<T>(this ObservableCollection<T> source, IEnumerable<T> updateCollection)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(updateCollection != null);

            source.Update(updateCollection, EqualityComparer<T>.Default);
        }

        /// <summary>ObservableCollection の内容をコレクションと比較し、それぞれの要素数を合わせます。順序は保存されません。</summary>
        public static void Update<T>(this ObservableCollection<T> source, IEnumerable<T> updateCollection, IEqualityComparer<T> comparer)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(updateCollection != null);
            Contract.Requires<ArgumentNullException>(comparer != null);

            // updateCollection の数 - source の数
            IDictionary<T, int> 要素の増減量 = new Dictionary<T, int>();

            foreach (var s in source)
            {
                if (要素の増減量.ContainsKey(s))
                {
                    要素の増減量[s]--;
                }
                else
                {
                    要素の増減量[s] = -1;
                }
            }

            foreach (var u in updateCollection)
            {
                if (要素の増減量.ContainsKey(u))
                {
                    要素の増減量[u]++;
                }
                else
                {
                    要素の増減量[u] = 1;
                }
            }

            foreach (var i in 要素の増減量.Where(p => p.Value < 0))
            {
                source.Remove(i.Key);
            }

            foreach (var i in 要素の増減量.Where(p => p.Value > 0))
            {
                source.Add(i.Key);
            }
        }
    }
}
