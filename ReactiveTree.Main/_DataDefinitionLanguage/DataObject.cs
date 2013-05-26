using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree
{
    /// <summary>Simple implementation of <c>IDataObject</c>.</summary>
    public class DataObject : IEquatable<DataObject>, IDataObject
    {
        public DataObject(object obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null);

            this.InnerObject = obj;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.InnerObject != null);
        }

        protected object InnerObject
        {
            get;
            private set;
        }

        public virtual bool TryCast<T>(out T value)
        {
            if (InnerObject is T)
            {
                value = (T)this.InnerObject;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public override string ToString()
        {
            return this.InnerObject.ToString();
        }

        public virtual bool Equals(DataObject other)
        {
            if (other == null) return false;

            return Object.Equals(this.InnerObject, other.InnerObject);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;

            return this.Equals((SimpleJsonDataObject)obj);
        }

        public override int GetHashCode()
        {
            return this.InnerObject.GetHashCode();
        }
    }
}
