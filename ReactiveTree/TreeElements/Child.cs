using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public struct Child<TKey, TValue>
    {
        public Child(TKey key, TreeElement<TKey, TValue> singleChild)
            : this()
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(singleChild != null);

            this.p_Key = key;
            this.p_Values = new[] { singleChild };
            this.IsArray = false;
        }

        public Child(TKey key, IEnumerable<TreeElement<TKey, TValue>> arrayChildren)
            : this()
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires(arrayChildren != null && arrayChildren.All(x => x != null));

            this.p_Key = key;
            this.p_Values = arrayChildren.ToArray();
            this.IsArray = true;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.p_Key != null);
            Contract.Invariant(this.p_Values != null);
            Contract.Invariant(this.p_Values.All(v => v != null));
        }

        TKey p_Key;
        public TKey Key
        {
            get
            {
                Contract.Ensures(Contract.Result<TKey>() != null);

                return p_Key;
            }
        }

        IEnumerable<TreeElement<TKey, TValue>> p_Values;
        public IEnumerable<TreeElement<TKey, TValue>> Values 
        { 
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<TreeElement<TKey, TValue>>>() != null);
                Contract.Ensures(Contract.Result<IEnumerable<TreeElement<TKey, TValue>>>().All(v => v != null));

                return p_Values;
            }
        }

        public bool IsArray { get; private set; }

        public ChildWithoutKey<TKey, TValue> ToChildWithoutKey()
        {
            if (this.IsArray)
            {
                return new ChildWithoutKey<TKey, TValue>(this.Values);
            }
            else
            {
                return new ChildWithoutKey<TKey, TValue>(this.Values.Single());
            }
        }
    }
}
