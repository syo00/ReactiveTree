using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    public class NodeKeyOrArrayIndex<TKey> : IEquatable<NodeKeyOrArrayIndex<TKey>>
    {
        readonly bool isArray;
        readonly TKey nodeKey;
        readonly int arrayIndex;

        public NodeKeyOrArrayIndex(TKey nodeKey)
        {
            this.nodeKey = nodeKey;
        }

        public NodeKeyOrArrayIndex(int arrayIndex)
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

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((NodeKeyOrArrayIndex<TKey>)obj);
        }

        public bool Equals(NodeKeyOrArrayIndex<TKey> other)
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
}
