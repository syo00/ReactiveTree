using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace Kirinji.ReactiveTree.TreeStructures
{
    [ContractClass(typeof(IReadOnlyTreeElementContract<,>))]
    public interface IReadOnlyTreeElement<K, V>
    {
        IEnumerable<IReadOnlyTreeElement<K, V>> Array { get; }
        IObservable<NotifyCollectionChangedEventArgs> ArrayChanged { get; }
        IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>> GrandChildrenChanged { get; }
        V LeafValue { get; }
        IEnumerable<KeyValuePair<K, IReadOnlyTreeElement<K, V>>> NodeChildren { get; }
        IObservable<NotifyCollectionChangedEventArgs> NodeChildrenChanged { get; }
        ElementType Type { get; }
    }

    #region IReadOnlyTreeElement contract binding
    [ContractClassFor(typeof(IReadOnlyTreeElement<,>))]
    abstract class IReadOnlyTreeElementContract<K, V> : IReadOnlyTreeElement<K, V>
    {
        public IEnumerable<IReadOnlyTreeElement<K, V>> Array
        {
            get
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Array, TreeElementMessages.NotArrayErrorMessage);
                Contract.Ensures(Contract.Result<IEnumerable<IReadOnlyTreeElement<K, V>>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<IReadOnlyTreeElement<K, V>>>(), e => e != null));

                throw new NotImplementedException(); 
            }
        }

        public IObservable<NotifyCollectionChangedEventArgs> ArrayChanged
        {
            get
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Array, TreeElementMessages.NotArrayErrorMessage);
                Contract.Ensures(Contract.Result<IObservable<NotifyCollectionChangedEventArgs>>() != null);

                throw new NotImplementedException();
            }
        }

        public IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>> GrandChildrenChanged
        {
            get
            {
                Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>>>() != null);

                throw new NotImplementedException();
            }
        }

        public V LeafValue
        {
            get 
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Leaf, TreeElementMessages.NotLeafErrorMessage);


                throw new NotImplementedException();
            }
        }

        public IEnumerable<KeyValuePair<K, IReadOnlyTreeElement<K, V>>> NodeChildren
        {
            get
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Node, TreeElementMessages.NotNodeErrorMessage);
                Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<K, IReadOnlyTreeElement<K, V>>>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<KeyValuePair<K, IReadOnlyTreeElement<K, V>>>>(), e => e.Key != null && e.Value != null));

                throw new NotImplementedException(); 
            }
        }

        public IObservable<NotifyCollectionChangedEventArgs> NodeChildrenChanged
        {
            get
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Node, TreeElementMessages.NotNodeErrorMessage);
                Contract.Ensures(Contract.Result<IObservable<NotifyCollectionChangedEventArgs>>() != null);

                throw new NotImplementedException(); 
            }
        }

        public ElementType Type
        {
            get { throw new NotImplementedException(); }
        }
    }
    #endregion

}
