using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree
{
    public class KeyOrIndex<TKey> : IEquatable<KeyOrIndex<TKey>>
    {
        readonly bool isArray;
        readonly TKey nodeKey;
        readonly int arrayIndex;

        public KeyOrIndex(TKey nodeKey)
        {
            this.nodeKey = nodeKey;
        }

        public KeyOrIndex(int arrayIndex)
        {
            this.arrayIndex = arrayIndex;
            this.isArray = true;
        }

        public void Do(Action<TKey> action1, Action<int> action2)
        {
            if (isArray)
            {
                if (action2 == null) throw new NotImplementedException("action2"); // NotImplementedException is proper??
                action2(arrayIndex);
            }
            else
            {
                if (action1 == null) throw new NotImplementedException("action1");
                action1(nodeKey);
            }
        }

        public bool IsArray
        {
            get
            {
                return isArray;
            }
        }

        public bool IsNode
        {
            get
            {
                return !IsArray;
            }
        }

        public int ArrayIndex
        {
            get
            {
                Contract.Requires<InvalidOperationException>(IsArray);

                return arrayIndex;
            }
        }

        public TKey NodeKey
        {
            get
            {
                Contract.Requires<InvalidOperationException>(IsNode);

                return nodeKey;
            }
        }

        public bool Is(int index)
        {
            if (IsNode) return false;

            return Object.Equals(ArrayIndex, index);
        }

        public bool Is(TKey key)
        {
            if (IsArray) return false;

            return Object.Equals(NodeKey, key);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((KeyOrIndex<TKey>)obj);
        }

        public bool Equals(KeyOrIndex<TKey> other)
        {
            if (other == null) return false;
            if (this.IsArray == other.IsArray)
            {
                if (this.IsArray)
                {
                    return this.ArrayIndex == other.ArrayIndex;
                }
                else
                {
                    return Object.Equals(this.NodeKey, other.NodeKey);
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (this.IsArray)
            {
                return this.ArrayIndex.GetHashCode();
            }
            else
            {
                return this.NodeKey == null ? 0 : this.NodeKey.GetHashCode();
            }
        }

        public override string ToString()
        {
            if (IsArray)
            {
                return ArrayIndex.ToString();
            }
            else
            {
                return NodeKey == null ? "Null" : NodeKey.ToString();
            }
        }
    }

    public static class KeyOrIndex
    {
        public static KeyOrIndex<T> Key<T>(T key)
        {
            Contract.Ensures(Contract.Result<KeyOrIndex<T>>() != null);

            return new KeyOrIndex<T>(key);
        }

        public static KeyOrIndex<T> Index<T>(int index)
        {
            Contract.Ensures(Contract.Result<KeyOrIndex<T>>() != null);

            return new KeyOrIndex<T>(index);
        }
    }
}
