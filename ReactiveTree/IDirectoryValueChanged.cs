using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.ReactiveTree.TreeStructures;

namespace Kirinji.ReactiveTree
{
    [ContractClass(typeof(IDirectoryValueChangedContract<,>))]
    public interface IDirectoryValueChanged<TKey, TValue>
    {
        IObservable<IEnumerable<ElementDirectory<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories);
        IEnumerable<ElementDirectory<TKey, TValue>> GetValues(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories);
    }

    #region IDirectoryValueChanged contract binding
    [ContractClassFor(typeof(IDirectoryValueChanged<,>))]
    abstract class IDirectoryValueChangedContract<TKey, TValue> : IDirectoryValueChanged<TKey, TValue>
    {
        public IObservable<IEnumerable<ElementDirectory<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Requires(Contract.ForAll(directories, dir => dir != null && Contract.ForAll(dir, key => key != null)));
            Contract.Ensures(Contract.Result<IObservable<IEnumerable<ElementDirectory<TKey, TValue>>>>() != null);
            // For all IObservable values, all keys and values must not be null.

            // If keysDirectories 

            throw new NotImplementedException();
        }

        public IEnumerable<ElementDirectory<TKey, TValue>> GetValues(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Requires(Contract.ForAll(directories, dir => dir != null && Contract.ForAll(dir, key => key != null)));
            Contract.Ensures(Contract.Result<IEnumerable<ElementDirectory<TKey, TValue>>>() != null);
            // Contract.Ensures are not fully implemented.
            // All keys and values must not be null.

            throw new NotImplementedException();
        }
    }
    #endregion

    public static class IDirectoryValueChangedExtensions
    {
        public static IObservable<IEnumerable<ElementDirectory<TKey, TValue>>> ValuesChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params KeyArray<KeyOrIndex<TKey>>[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IObservable<IEnumerable<ElementDirectory<TKey, TValue>>>>() != null);

            return source.ValuesChanged(directories.AsEnumerable());
        }

        public static IObservable<ElementDirectory<TKey, TValue>> ValueChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, KeyArray<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<ElementDirectory<TKey, TValue>>>() != null);

            return source.ValuesChanged(new[] { directory }).Select(dic => dic.SingleOrDefault()).Where(x => x != null);
        }

        public static IObservable<ElementDirectory<TKey, TValue>> ValueChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, IEnumerable<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<ElementDirectory<TKey, TValue>>>() != null);

            return source.ValueChanged(new KeyArray<KeyOrIndex<TKey>>(directory));
        }

        public static IObservable<ElementDirectory<TKey, TValue>> ValueChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params KeyOrIndex<TKey>[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<ElementDirectory<TKey, TValue>>>() != null);

            return source.ValueChanged(directory.AsEnumerable());
        }


        public static IEnumerable<ElementDirectory<TKey, TValue>> GetValues<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params KeyArray<KeyOrIndex<TKey>>[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IEnumerable<ElementDirectory<TKey, TValue>>>() != null);

            return source.GetValues(directories.AsEnumerable());
        }

        public static ElementDirectory<TKey, TValue> GetValue<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, KeyArray<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.GetValues(new[] { directory }).SingleOrDefault();
        }

        public static ElementDirectory<TKey, TValue> GetValue<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, IEnumerable<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.GetValue(new KeyArray<KeyOrIndex<TKey>>(directory));
        }

        public static ElementDirectory<TKey, TValue> GetValue<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params KeyOrIndex<TKey>[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<ElementDirectory<TKey, TValue>>() != null);

            return source.GetValue(directory.AsEnumerable());
        }


        public static IObservable<IEnumerable<ElementDirectory<TKey, TValue>>> GetValuesAndChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params KeyArray<KeyOrIndex<TKey>>[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IObservable<IEnumerable<ElementDirectory<TKey, TValue>>>>() != null);

            return Observable.Merge(Observable.Return(source.GetValues(directories)), source.ValuesChanged(directories));
        }

        public static IObservable<ElementDirectory<TKey, TValue>> GetValueAndChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, KeyArray<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<ElementDirectory<TKey, TValue>>>() != null);

            return Observable.Merge(Observable.Return(source.GetValue(directory)), source.ValueChanged(directory));
        }

        public static IObservable<ElementDirectory<TKey, TValue>> GetValueAndChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, IEnumerable<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<ElementDirectory<TKey, TValue>>>() != null);
            
            return Observable.Merge(Observable.Return(source.GetValue(directory)), source.ValueChanged(directory));
        }

        public static IObservable<ElementDirectory<TKey, TValue>> GetValueAndChanged<TKey, TValue>(this IDirectoryValueChanged<TKey, TValue> source, params KeyOrIndex<TKey>[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<ElementDirectory<TKey, TValue>>>() != null);

            return Observable.Merge(Observable.Return(source.GetValue(directory)), source.ValueChanged(directory));
        }
    }
}
