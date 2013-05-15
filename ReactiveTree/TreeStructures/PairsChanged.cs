using Kirinji.LightWands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    // Maybe KeyValuePairsChanged is more proper name, but it is long.
    public class PairsChanged<TKey, TValue> : IEquatable<PairsChanged<TKey, TValue>>
    {
        private IEnumerable<KeyValuePair<TKey, TValue>> added;
        private IEnumerable<KeyValuePair<TKey, TValue>> removed;
        private IEqualityComparer<TKey> keysComparer;
        private IEqualityComparer<TValue> valuesComparer;

        public PairsChanged(
            IEnumerable<KeyValuePair<TKey, TValue>> added,
            IEnumerable<KeyValuePair<TKey, TValue>> removed)
            : this(added, removed, null, null)
        {
            Contract.Requires<ArgumentNullException>(added != null);
            Contract.Requires<ArgumentNullException>(removed != null);
        }

        public PairsChanged(
            IEnumerable<KeyValuePair<TKey, TValue>> added,
            IEnumerable<KeyValuePair<TKey, TValue>> removed,
            IEqualityComparer<TKey> keysComparer,
            IEqualityComparer<TValue> valuesComparer)
        {
            Contract.Requires<ArgumentNullException>(added != null);
            Contract.Requires<ArgumentNullException>(removed != null);

            this.added = added;
            this.removed = removed;
            this.keysComparer = keysComparer;
            this.valuesComparer = valuesComparer;
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Added
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<TKey, TValue>>>() != null);

                return added;
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Removed
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<TKey, TValue>>>() != null);

                return removed;
            }
        }

        public IEnumerable<MovedItem> Moved
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<MovedItem>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<MovedItem>>(), m => m != null));

                // Added と Remove を参照して自動的に導く
                throw new NotImplementedException();
            }
        }

        [Pure]
        public PairsChanged<TKey, TValue> Combine(PairsChanged<TKey, TValue> combined)
        {
            Contract.Requires<ArgumentNullException>(combined != null);
            Contract.Ensures(Contract.Result<PairsChanged<TKey, TValue>>() != null);

            throw new NotImplementedException();
        }


        public class MovedItem
        {
            public MovedItem()
            {
                throw new NotImplementedException();
            }

            public TKey From { get; private set; }
            public TKey To { get; private set; }
            public TValue Value { get; private set; }
        }

        public virtual bool Equals(PairsChanged<TKey, TValue> other)
        {
            throw new NotImplementedException();
        }
    }

    // abbreviation of PairsChanged<IEnumerable<NodeKeyOrArrayIndex<TKey>>, TreeElement<TKey, TValue>> and easily can be compared by keys
    public class GrandChildrenPairsChanged<TKey, TValue> : PairsChanged<KeyArray<NodeKeyOrArrayIndex<TKey>>, TreeElement<TKey, TValue>>, IEquatable<GrandChildrenPairsChanged<TKey, TValue>>
    {
        public GrandChildrenPairsChanged(
            IEnumerable<KeyValuePair<KeyArray<NodeKeyOrArrayIndex<TKey>>, TreeElement<TKey, TValue>>> added,
            IEnumerable<KeyValuePair<KeyArray<NodeKeyOrArrayIndex<TKey>>, TreeElement<TKey, TValue>>> removed)
            : base(added, removed)
        {
            Contract.Requires<ArgumentNullException>(added != null);
            Contract.Requires<ArgumentNullException>(removed != null);
        }

        public override bool Equals(PairsChanged<KeyArray<NodeKeyOrArrayIndex<TKey>>, TreeElement<TKey, TValue>> other)
        {
            return base.Equals(other);
        }

        public virtual bool Equals(GrandChildrenPairsChanged<TKey, TValue> other)
        {
            throw new NotImplementedException();
        }
    }
}
