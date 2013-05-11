using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.ReactiveTree.TreeElements;

namespace Kirinji.ReactiveTree
{
    [ContractClass(typeof(IDirectoryValueChangedContract<,>))]
    public interface IDirectoryValueChanged<TKey, TValue>
    {
        GrandChildrenContainer<TKey, TValue> GetValue(IEnumerable<TKey> keyDirectory);
        IObservable<GrandChildrenContainer<TKey, TValue>> ValueChanged(IEnumerable<TKey> keyDirectory);
    }

    #region IDirectoryValueChanged contract binding
    [ContractClassFor(typeof(IDirectoryValueChanged<,>))]
    abstract class IDirectoryValueChangedContract<TKey, TValue> : IDirectoryValueChanged<TKey, TValue>
    {
        IObservable<GrandChildrenContainer<TKey, TValue>> IDirectoryValueChanged<TKey, TValue>.ValueChanged(IEnumerable<TKey> keyDirectory)
        {
            Contract.Requires<ArgumentNullException>(keyDirectory != null);
            Contract.Ensures(Contract.Result<IObservable<GrandChildrenContainer<TKey, TValue>>>() != null);

            throw new NotImplementedException();
        }

        public GrandChildrenContainer<TKey, TValue> GetValue(IEnumerable<TKey> keyDirectory)
        {
            Contract.Requires<ArgumentNullException>(keyDirectory != null);
            Contract.Ensures(Contract.Result<GrandChildrenContainer<TKey, TValue>>() != null);

            throw new NotImplementedException();
        }
    }
    #endregion

    public static class IDirectoryValueChangedExtensions
    {
        public static IObservable<GrandChildrenContainer<TKey, TValue>> ValueChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params TKey[] keyDirectory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keyDirectory != null);
            Contract.Ensures(Contract.Result<IObservable<GrandChildrenContainer<TKey, TValue>>>() != null);

            return source.ValueChanged(keyDirectory.AsEnumerable());
        }

        public static GrandChildrenContainer<TKey, TValue> GetValue<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params TKey[] keyDirectory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keyDirectory != null);
            Contract.Ensures(Contract.Result<GrandChildrenContainer<TKey, TValue>>() != null);

            return source.GetValue(keyDirectory.AsEnumerable());
        }

        public static IObservable<GrandChildrenContainer<TKey, TValue>> GetValueAndChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, IEnumerable<TKey> keyDirectory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keyDirectory != null);
            Contract.Ensures(Contract.Result<IObservable<GrandChildrenContainer<TKey, TValue>>>() != null);

            return Observable.Merge(Observable.Return(source.GetValue(keyDirectory)), source.ValueChanged(keyDirectory));
        }

        public static IObservable<GrandChildrenContainer<TKey, TValue>> GetValueAndChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params TKey[] keyDirectory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keyDirectory != null);
            Contract.Ensures(Contract.Result<IObservable<GrandChildrenContainer<TKey, TValue>>>() != null);

            return source.GetValueAndChanged(keyDirectory.AsEnumerable());
        }
    }
}
