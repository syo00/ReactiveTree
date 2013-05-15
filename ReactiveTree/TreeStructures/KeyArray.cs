using Kirinji.LightWands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    public class KeyArray<T> : IEquatable<KeyArray<T>>, IEnumerable<T>
    {
        readonly IEnumerable<T> keyArray;
        readonly IEqualityComparer<IEnumerable<T>> equalityComparer = EqualityComparer.EnumerableOf<T>();

        public KeyArray(IEnumerable<T> keyArray)
        {
            Contract.Requires<ArgumentNullException>(keyArray != null);

            this.keyArray = keyArray.ToArray();
        }

        public KeyArray(params T[] keyArray)
            : this(keyArray.AsEnumerable())
        {
            Contract.Requires<ArgumentNullException>(keyArray != null);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return keyArray.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((KeyArray<T>)obj);
        }

        public bool Equals(KeyArray<T> other)
        {
            return equalityComparer.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return equalityComparer.GetHashCode(this);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[");
            bool isFirst = false;
            foreach (var k in keyArray)
            {
                if (isFirst) builder.Append(", ");
                builder.Append(k == null ? "null" : k.ToString());
                isFirst = true;
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
