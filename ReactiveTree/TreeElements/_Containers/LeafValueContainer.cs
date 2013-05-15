using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public struct LeafValueContainer<TValue> : IEquatable<LeafValueContainer<TValue>>
    {
        public LeafValueContainer(TValue value)
            : this()
        {
            this.Value = value;
            this.IsExist = true;
        }

        public bool IsExist { get; private set; }
        public TValue Value { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is LeafValueContainer<TValue>)
            {
                return this.Equals((LeafValueContainer<TValue>)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.IsExist.GetHashCode()
                ^ (this.Value == null ? 0 : this.Value.GetHashCode());
        }

        public override string ToString()
        {
            if (this.IsExist)
            {
                return this.Value == null ? "null" : this.Value.ToString();
            }
            else
            {
                return "(Empty)";
            }
        }

        public bool Equals(LeafValueContainer<TValue> other)
        {
            return this.IsExist == other.IsExist
                && Object.Equals(this.Value, other.Value);
        }

        public static bool operator ==(LeafValueContainer<TValue> left, LeafValueContainer<TValue> right)
        {
            if ((object)left == null)
            {
                return (object)right == null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(LeafValueContainer<TValue> left, LeafValueContainer<TValue> right)
        {
            return !(left == right);
        }
    }
}
