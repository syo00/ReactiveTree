using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.LightWands;
using Kirinji.ReactiveTree.TreeStructures;

namespace Kirinji.ReactiveTree
{
    [ContractClass(typeof(ISimpleReactiveDictionaryContract<,>))]
    public interface ISimpleReactiveDictionary<TKey, TValue>
    {
        IReadOnlyDictionary<TKey, TValue> Values(IEnumerable<TKey> directories);
        IObservable<IReadOnlyDictionary<TKey, TValue>> ValuesChanged(IEnumerable<TKey> directories);
    }

    #region IReactiveTree contract binding
    [ContractClassFor(typeof(ISimpleReactiveDictionary<,>))]
    abstract class ISimpleReactiveDictionaryContract<TKey, TValue> : ISimpleReactiveDictionary<TKey, TValue>
    {
        public IReadOnlyDictionary<TKey, TValue> Values(IEnumerable<TKey> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Requires(Contract.ForAll(directories, dir => dir != null));
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<TKey, TValue>>() != null);
            // Contract.Result<IReadOnlyDictionary<TKey, TValue>>().Count == directories.Distinct().Count()
            // All KeyValuePair keys are returned and values may be null.

            throw new NotImplementedException();
        }

        public IObservable<IReadOnlyDictionary<TKey, TValue>> ValuesChanged(IEnumerable<TKey> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Requires(Contract.ForAll(directories, dir => dir != null));
            Contract.Ensures(Contract.Result<IObservable<IReadOnlyDictionary<TKey, TValue>>>() != null);
            //　Not always all KeyValuePair keys are returned and values may be null.

            throw new NotImplementedException();
        }
    }
    #endregion

    public static class SimpleReactiveDictionary
    {
        public static IObservable<IReadOnlyDictionary<TKey, TValue>> ValuesChanged<TKey, TValue>(this ISimpleReactiveDictionary<TKey, TValue> source, params TKey[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IObservable<IReadOnlyDictionary<TKey, TValue>>>() != null);

            return source.ValuesChanged(directories.AsEnumerable());
        }

        public static IObservable<KeyValuePair<TKey, TValue>> ValueChanged<TKey, TValue>(this ISimpleReactiveDictionary<TKey, TValue> source, TKey directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<TKey, TValue>>>() != null);

            return source.ValuesChanged(new[] { directory }).SelectMany(dic => dic);
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this ISimpleReactiveDictionary<KeyArray<TKey>, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return source.ValueChanged(new KeyArray<TKey>(directory));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this ISimpleReactiveDictionary<KeyArray<TKey>, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return source.ValueChanged(directory.AsEnumerable());
        }


        public static IReadOnlyDictionary<TKey, TValue> Values<TKey, TValue>(this ISimpleReactiveDictionary<TKey, TValue> source, params TKey[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<TKey, TValue>>() != null);

            return source.Values(directories.AsEnumerable());
        }

        public static KeyValuePair<TKey, TValue> Value<TKey, TValue>(this ISimpleReactiveDictionary<TKey, TValue> source, TKey directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.Values(new[] { directory }).Single();
        }

        public static KeyValuePair<KeyArray<TKey>, TValue> Value<TKey, TValue>(this ISimpleReactiveDictionary<KeyArray<TKey>, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.Value(new KeyArray<TKey>(directory));
        }

        public static KeyValuePair<KeyArray<TKey>, TValue> Value<TKey, TValue>(this ISimpleReactiveDictionary<KeyArray<TKey>, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);

            return source.Value(directory.AsEnumerable());
        }


        public static IObservable<IReadOnlyDictionary<TKey, TValue>> ValuesAndChanged<TKey, TValue>(this ISimpleReactiveDictionary<TKey, TValue> source, params TKey[] directories)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IObservable<IEnumerable<KeyValuePair<TKey, TValue>>>>() != null);

            return Observable.Merge(Observable.Return(source.Values(directories)), source.ValuesChanged(directories));
        }

        public static IObservable<KeyValuePair<TKey, TValue>> ValueAndChanged<TKey, TValue>(this ISimpleReactiveDictionary<TKey, TValue> source, TKey directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<TKey, TValue>>>() != null);

            return Observable.Merge(
                Observable.Return(source.Value(directory)),
                source.ValueChanged(directory));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueAndChanged<TKey, TValue>(this ISimpleReactiveDictionary<KeyArray<TKey>, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return Observable.Merge(
                Observable.Return(source.Value(directory)),
                source.ValueChanged(directory));
        }

        public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueAndChanged<TKey, TValue>(this ISimpleReactiveDictionary<KeyArray<TKey>, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

            return Observable.Merge(
                Observable.Return(source.Value(directory)),
                source.ValueChanged(directory));
        }


        //public static IObservable<IReadOnlyDictionary<TKey, TValue>> ValuesChanged<TKey, TValue>(this IReactiveTree<TKey, IDataObject> source, params TKey[] directories)
        //{
        //    Contract.Requires<ArgumentNullException>(source != null);
        //    Contract.Requires<ArgumentNullException>(directories != null);
        //    Contract.Ensures(Contract.Result<IObservable<IReadOnlyDictionary<TKey, TValue>>>() != null);

        //    return source
        //        .ValuesChanged(directories.AsEnumerable())
        //        .Select(dic =>
        //        {

        //        });
        //}

        //public static IObservable<KeyValuePair<TKey, TValue>> ValueChanged<TKey, TValue>(this IReactiveTree<TKey, TValue> source, TKey directory)
        //{
        //    Contract.Requires<ArgumentNullException>(source != null);
        //    Contract.Requires<ArgumentNullException>(directory != null);
        //    Contract.Ensures(Contract.Result<IObservable<KeyValuePair<TKey, TValue>>>() != null);

        //    return source.ValuesChanged(new[] { directory }).SelectMany(dic => dic);
        //}

        //public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this IReactiveTree<KeyArray<TKey>, TValue> source, IEnumerable<TKey> directory)
        //{
        //    Contract.Requires<ArgumentNullException>(source != null);
        //    Contract.Requires<ArgumentNullException>(directory != null);
        //    Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

        //    return source.ValueChanged(new KeyArray<TKey>(directory));
        //}

        //public static IObservable<KeyValuePair<KeyArray<TKey>, TValue>> ValueChanged<TKey, TValue>(this IReactiveTree<KeyArray<TKey>, TValue> source, params TKey[] directory)
        //{
        //    Contract.Requires<ArgumentNullException>(source != null);
        //    Contract.Requires<ArgumentNullException>(directory != null);
        //    Contract.Ensures(Contract.Result<IObservable<KeyValuePair<KeyArray<TKey>, TValue>>>() != null);

        //    return source.ValueChanged(directory.AsEnumerable());
        //}

        //private static KeyValuePair<TKey, TValue> Convert<TKey, TValue>(KeyValuePair<TKey, IReadOnlyTreeElement<TKey, IDataObject>> pair)
        //{
        //    var tree = pair.Value;
        //    if (tree == null || tree.Type != ElementType.Leaf)
        //    {
        //        return new KeyValuePair<TKey, TValue>(pair.Key, default(TValue));
        //    }
        //    var leafValue = tree.LeafValue;
        //    if (leafValue == null) return new KeyValuePair<TKey, TValue>(pair.Key, default(TValue));
        //    return new KeyValuePair<TKey, TValue>(pair.Key, leafValue.CastOrDefault<TValue>());
        //}
    }
}
