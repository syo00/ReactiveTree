using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    #region ITreeElement contract binding
    [ContractClass(typeof(ITreeElementContract<,>))]
    public partial interface ITreeElement<TKey, TValue>
    {
        
    }

    [ContractClassFor(typeof(ITreeElement<,>))]
    abstract class ITreeElementContract<TKey, TValue> : ITreeElement<TKey, TValue>
    {
        public ElementType Type
        {
            get { throw new NotImplementedException(); }
        }

        public TreeElement<TKey, TValue> this[TKey key]
        {
            get
            {
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
                Contract.Ensures(Contract.Result<TreeElement<TKey, TValue>>() != null);

                throw new NotImplementedException();
            }
            set
            {
                Contract.Requires<ArgumentNullException>(key != null);
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

                throw new NotImplementedException();
            }
        }

        public IEnumerable<Child<TKey, TValue>> GetAllChildren()
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
            Contract.Ensures(Contract.Result<IEnumerable<Child<TKey, TValue>>>() != null);

            throw new NotImplementedException();
        }

        public IEnumerable<TreeElement<TKey, TValue>> GetArrayChildOrDefault(TKey key)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            throw new NotImplementedException();
        }

        public TreeElement<TKey, TValue> GetSingleChildOrDefault(TKey key)
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
            Contract.Requires<ArgumentNullException>(key != null);

            throw new NotImplementedException();
        }

        public TValue LeafValue
        {
            get
            {
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Leaf);

                throw new NotImplementedException(); 
            }
        }

        public void SetSingleChild(TKey key, TreeElement<TKey, TValue> newChild)
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(newChild != null);

            throw new NotImplementedException();
        }

        public void ModifyArrayChild(TKey key, Func<TreeElement<TKey, TValue>, IEnumerable<TreeElement<TKey, TValue>>> arrayCreator, Action<IList<TreeElement<TKey, TValue>>> arrayModifier)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(arrayModifier != null);
            Contract.Requires<ArgumentNullException>(arrayCreator != null);
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            throw new NotImplementedException();
        }

        public void ModifyTreeAsSeries(Action<ITreeElement<TKey, TValue>> treeEditor)
        {
            Contract.Requires<ArgumentNullException>(treeEditor != null);

            throw new NotImplementedException();
        }

        public bool Equals(ITreeElement<TKey, TValue> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
                Contract.Ensures(Contract.Result<IEnumerable<TKey>>() != null);

                throw new NotImplementedException();
            }
        }
    }
    #endregion

}
