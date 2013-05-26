using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree
{
    [ContractClass(typeof(IReactiveTreeContract<,>))]
    public interface IReactiveTree<TKey, TValue>
    {
        IReadOnlyDictionary<KeyArray<TKey>, TValue> Values(IEnumerable<KeyArray<TKey>> directories);
        IObservable<IReadOnlyDictionary<KeyArray<TKey>, TValue>> ValuesChanged(IEnumerable<KeyArray<TKey>> directories);
    }

    #region IReactiveTree contract binding
    [ContractClassFor(typeof(IReactiveTree<,>))]
    abstract class IReactiveTreeContract<TKey, TValue> : IReactiveTree<TKey, TValue>
    {
        public IReadOnlyDictionary<KeyArray<TKey>, TValue> Values(IEnumerable<KeyArray<TKey>> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Requires(Contract.ForAll(directories, dir => dir != null && Contract.ForAll(dir, key => key != null)));
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<KeyArray<TKey>, TValue>>() != null);
            // Contract.Result<IReadOnlyDictionary<KeyArray<TKey>, TValue>>().Count == directories.Distinct().Count()
            // All KeyValuePair keys are returned and values may be null.

            throw new NotImplementedException();
        }

        public IObservable<IReadOnlyDictionary<KeyArray<TKey>, TValue>> ValuesChanged(IEnumerable<KeyArray<TKey>> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Requires(Contract.ForAll(directories, dir => dir != null && Contract.ForAll(dir, key => key != null)));
            Contract.Ensures(Contract.Result<IObservable<IReadOnlyDictionary<KeyArray<TKey>, TValue>>>() != null);

            throw new NotImplementedException();
        }
    }
    #endregion

    public static class ReactiveTree
    {
        public static IObservable<IReadOnlyDictionary<KeyArray<TKey>, TValue>> ValuesChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, params KeyArray<TKey>[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IObservable<IReadOnlyDictionary<KeyArray<TKey>, TValue>>>() != null);

            return source.ValuesChanged(directories.AsEnumerable());
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, KeyArray<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return source.ValuesChanged(new[] { directory }).SelectMany(dic => dic);
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return source.ValueChanged(new KeyArray<TKey>(directory));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return source.ValueChanged(directory.AsEnumerable());
        }


        public static IReadOnlyDictionary<KeyArray<TKey>, TValue> Values<TKey, TValue>(this IReactiveTree<TKey, TValue> source, params KeyArray<TKey>[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<KeyArray<TKey>, TValue>>() != null);

            return source.Values(directories.AsEnumerable());
        }

        public static KeyValuePair<KeyArray<TKey>, TValue> Value<TKey, TValue>(this IReactiveTree<TKey, TValue> source, KeyArray<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.Values(new[] { directory }).Single();
        }

        public static KeyValuePair<KeyArray<TKey>, TValue> Value<TKey, TValue>(this IReactiveTree<TKey, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.Value(new KeyArray<TKey>(directory));
        }

        public static KeyValuePair<KeyArray<TKey>, TValue> Value<TKey, TValue>(this IReactiveTree<TKey, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.Value(directory.AsEnumerable());
        }


        public static IObservable<IReadOnlyDictionary<KeyArray<TKey>, TValue>> ValuesAndChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, params KeyArray<TKey>[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IObservable<IEnumerable<KeyValuePair<KeyArray<TKey>, TValue>>>>() != null);

            return Observable.Merge(Observable.Return(source.Values(directories)), source.ValuesChanged(directories));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueAndChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, KeyArray<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return Observable.Merge(
                Observable.Return(source.Value(directory)),
                source.ValueChanged(directory));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueAndChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return Observable.Merge(
                Observable.Return(source.Value(directory)),
                source.ValueChanged(directory));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueAndChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return Observable.Merge(
                Observable.Return(source.Value(directory)),
                source.ValueChanged(directory));
        }
    }
}
