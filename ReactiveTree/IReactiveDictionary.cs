using Kirinji.ReactiveTree.TreeStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.ReactiveTree;
using System.Reactive.Disposables;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree
{
    [ContractClass(typeof(IReactiveDictionaryValueContract<>))]
    public interface IReactiveDictionaryValue<TKey>
    {
        TValue GetValue<TValue>(TKey directory);
    }

    // Bacause GetValue is a generic method (i.e. extension method is not useful), implemented this interface 
    [ContractClass(typeof(IReactiveDirectoryValueContract<>))]
    public interface IReactiveDirectoryValue<TKey> : IReactiveDictionaryValue<KeyArray<TKey>>
    {
        TValue GetValue<TValue>(IEnumerable<TKey> directory);
        TValue GetValue<TValue>(params TKey[] directory);
    }

    [ContractClass(typeof(IReactiveDictionaryContract<>))]
    public interface IReactiveDictionary<TKey> : IReactiveDictionaryValue<TKey>
    {
        IObservable<T> ObserveChanges<T>(Func<IReactiveDictionaryValue<TKey>, T> selector);
    }

    [ContractClass(typeof(IReactiveDirectoryContract<>))]
    public interface IReactiveDirectory<TKey> : IReactiveDirectoryValue<TKey>, IReactiveDictionary<KeyArray<TKey>>
    {
        IObservable<T> ObserveChanges<T>(Func<IReactiveDirectoryValue<TKey>, T> selector);
    }


    [ContractClassFor(typeof(IReactiveDictionaryValue<>))]
    abstract class IReactiveDictionaryValueContract<TKey> : IReactiveDictionaryValue<TKey>
    {
        public TValue GetValue<TValue>(TKey directory)
        {
            Contract.Requires<ArgumentNullException>(directory != null);

            throw new NotImplementedException();
        }
    }

    [ContractClassFor(typeof(IReactiveDirectoryValue<>))]
    abstract class IReactiveDirectoryValueContract<TKey> : IReactiveDirectoryValue<TKey>
    {
        public TValue GetValue<TValue>(IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(directory != null);

            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(directory != null);

            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(KeyArray<TKey> directory)
        {
            throw new NotImplementedException();
        }
    }

    [ContractClassFor(typeof(IReactiveDictionary<>))]
    abstract class IReactiveDictionaryContract<TKey> : IReactiveDictionary<TKey>
    {
        public IObservable<T> ObserveChanges<T>(Func<IReactiveDictionaryValue<TKey>, T> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(TKey directory)
        {
            throw new NotImplementedException();
        }
    }

    [ContractClassFor(typeof(IReactiveDirectory<>))]
    abstract class IReactiveDirectoryContract<TKey> : IReactiveDirectory<TKey>
    {
        public IObservable<T> ObserveChanges<T>(Func<IReactiveDirectoryValue<TKey>, T> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(IEnumerable<TKey> directory)
        {
            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(params TKey[] directory)
        {
            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(KeyArray<TKey> directory)
        {
            throw new NotImplementedException();
        }

        public IObservable<T> ObserveChanges<T>(Func<IReactiveDictionaryValue<KeyArray<TKey>>, T> selector)
        {
            throw new NotImplementedException();
        }
    }
}
