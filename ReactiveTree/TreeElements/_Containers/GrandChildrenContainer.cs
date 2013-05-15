using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.TreeElements
{
    /// <summary>Read-only array of GrandChild. Elements are sorted.</summary>
    public class GrandChildrenContainer<TKey, TValue> : IEnumerable<GrandChild<TKey, TValue>>, IEquatable<GrandChildrenContainer<TKey, TValue>>
    {
        private readonly IEnumerable<GrandChild<TKey, TValue>> orderedGrandChildren;

        public GrandChildrenContainer(IEnumerable<TKey> directory, IEnumerable<GrandChild<TKey, TValue>> grandChildren)
        {
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires<ArgumentNullException>(grandChildren != null);
            Contract.Requires<ArgumentException>(directory.All(k => k != null));

            this.Directory = directory;
            IDictionary<IEnumerable<int?>, GrandChild<TKey, TValue>> grandChildrenDictionary
                = new Dictionary<IEnumerable<int?>, GrandChild<TKey, TValue>>(EqualityComparer.EnumerableOf<int?>());
            grandChildren.ForEach(gc => grandChildrenDictionary.Add(gc.Indexes, gc));
            this.orderedGrandChildren = SortGrandChildren(grandChildrenDictionary);
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Directory != null);
            Contract.Invariant(this.LeafValues != null);
        }

        private IEnumerable<GrandChild<TKey, TValue>> SortGrandChildren(IDictionary<IEnumerable<int?>, GrandChild<TKey, TValue>> grandChildren)
        {
            Contract.Requires<ArgumentNullException>(grandChildren != null);
            Contract.Ensures(Contract.Result<IEnumerable<GrandChild<TKey, TValue>>>() != null);

            if (!grandChildren.Any()) return grandChildren.Values.ToArray();
            var indexesCount = grandChildren.First().Key.Count();
            IOrderedEnumerable<KeyValuePair<IEnumerable<int?>, GrandChild<TKey, TValue>>> currentOrdered = null;
            foreach (var i in Enumerable.Range(0, indexesCount))
            {
                if (currentOrdered == null)
                {
                    currentOrdered = grandChildren.OrderBy(p => p.Key.ElementAt(i) ?? -1);
                }
                else
                {
                    currentOrdered = currentOrdered.ThenBy(p => p.Key.ElementAt(i) ?? -1);
                }
            }
            if (currentOrdered == null)
            {
                return grandChildren.Select(p => p.Value).ToArray();
            }
            else
            {
                return currentOrdered.Select(p => p.Value).ToArray();
            }
        }

        public IEnumerable<TKey> Directory { get; private set; }

        private IEnumerable<TValue> p_values;
        /// <summary>Gets all leaf values. This is simply a syntax sugar.</summary>
        public IEnumerable<TValue> LeafValues
        {
            get
            {
                if (p_values == null)
                {
                    this.p_values = this.Select(gc => gc.Value)
                        .Where(tree => tree.Type == ElementType.Leaf)
                        .Select(tree => tree.LeafValue);
                }
                return p_values;
            }
        }

        public bool Equals(GrandChildrenContainer<TKey, TValue> other)
        {
            if (other == null) return false;
            return this.orderedGrandChildren.SequenceEqual(other.orderedGrandChildren)
                && this.Directory.SequenceEqual(other.Directory);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            var c = (GrandChildrenContainer<TKey, TValue>)obj;
            return this.Equals(c);
        }

        public override int GetHashCode()
        {
            var orderedGrandChildrenHashCodes
                = this.orderedGrandChildren
                .Select(gc => gc.GetHashCode())
                .Aggregate((x, y) => x ^ y);
            var directoryGrandChildrenHashCodes
                = this.Directory
                .Select(d => d.GetHashCode())
                .Aggregate((x, y) => x ^ y);
            return orderedGrandChildrenHashCodes ^ directoryGrandChildrenHashCodes;
        }

        public IEnumerator<GrandChild<TKey, TValue>> GetEnumerator()
        {
            return this.orderedGrandChildren.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
