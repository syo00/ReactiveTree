using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    /// <summary>TreeElement value and its array indexes.</summary>
    public struct GrandChild<TKey, TValue> : IEquatable<GrandChild<TKey, TValue>>
    {
        /// <remarks>Do not use default constructor.</remarks>
        public GrandChild(IEnumerable<int?> indexes, TreeElement<TKey, TValue> value)
            : this()
        {
            Contract.Requires<ArgumentNullException>(indexes != null);
            Contract.Requires<ArgumentNullException>(value != null);

            this.Indexes = indexes;
            this.Value = value;
        }

        /// <summary>Array indexes. Where not an array, Indexes element is null.</summary>
        public IEnumerable<int?> Indexes { get; private set; }

        public TreeElement<TKey, TValue> Value { get; private set; }

        public bool Equals(GrandChild<TKey, TValue> other)
        {
            return Object.Equals(this.Value, other.Value)
                && this.Indexes.SequenceEqual(other.Indexes);
        }

        public override bool Equals(object obj)
        {
            if (obj is GrandChild<TKey, TValue>)
            {
                var c = (GrandChild<TKey, TValue>)obj;
                return this.Equals(c);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var valueHashCode
                = this.Value == null ? 0 : this.Value.GetHashCode();
            var directoryGrandChildrenHashCodes
                = this.Indexes
                .Select(d => d.GetHashCode())
                .Aggregate((x, y) => x ^ y);
            return valueHashCode ^ directoryGrandChildrenHashCodes;
        }

        public static bool operator ==(GrandChild<TKey, TValue> x, GrandChild<TKey, TValue> y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(GrandChild<TKey, TValue> x, GrandChild<TKey, TValue> y)
        {
            return !(x == y);
        }
    }
}
