using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Kirinji.ReactiveTree.TreeElements
{
    public partial interface ITreeElement<TKey, TValue> : IEquatable<ITreeElement<TKey, TValue>>
    {
        ElementType Type { get; }
        IEnumerable<TKey> Keys { get; }
        TreeElement<TKey, TValue> this[TKey key] { get; set; }
        IEnumerable<Child<TKey, TValue>> GetAllChildren();
        IEnumerable<TreeElement<TKey, TValue>> GetArrayChildOrDefault(TKey key);
        TreeElement<TKey, TValue> GetSingleChildOrDefault(TKey key);
        TValue LeafValue { get; }
        void SetSingleChild(TKey key, TreeElement<TKey, TValue> newChild);
        void ModifyArrayChild(TKey key, Func<TreeElement<TKey, TValue>, IEnumerable<TreeElement<TKey, TValue>>> arrayCreator, Action<IList<TreeElement<TKey, TValue>>> arrayModifier);
        void ModifyTreeAsSeries(Action<ITreeElement<TKey, TValue>> treeEditor);
    }

    public static class TreeElement
    {
        public static void ModifyArrayChild<TKey, TValue>(this ITreeElement<TKey, TValue> source, TKey key, Action<IList<TreeElement<TKey, TValue>>> arrayModifier)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(arrayModifier != null);

            source.ModifyArrayChild(key, _ => null, arrayModifier);
        }

        public static void ModifyArrayChild<TKey, TValue>(this ITreeElement<TKey, TValue> source, TKey key, Func<ITreeElement<TKey, TValue>, IEnumerable<TreeElement<TKey, TValue>>> arrayCreator)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(arrayCreator != null);
            Contract.Requires<InvalidOperationException>(source.Type == ElementType.Node);

            source.ModifyArrayChild(key, arrayCreator, _ => { });
        }
    }
}
