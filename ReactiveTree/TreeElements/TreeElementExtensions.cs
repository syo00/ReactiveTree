using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.TreeElements
{
    public static class TreeElementExtensions
    {
        /// <summary>Gets children.</summary>
        public static Child<TKey, TValue>? GetChild<TKey, TValue>(this TreeElement<TKey, TValue> source, TKey key)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(key != null);

            if (source.Type != ElementType.Node) return null;
            var singleChild = source.GetSingleChildOrDefault(key);
            if (singleChild != null) return new Child<TKey, TValue>(key, singleChild);
            var arrayChild = source.GetArrayChildOrDefault(key);
            if (arrayChild != null) return new Child<TKey, TValue>(key, arrayChild);
            return null;
        }

        /// <summary>
        /// Gets one child in the specified directory.
        /// </summary>
        /// <returns>When not found, returns null.</returns>
        public static TreeElement<TKey, TValue> GetSingleChildOrDefault<TKey, TValue>(this TreeElement<TKey, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires(directory.All(k => k != null));

            return source.GetSingleChildOrDefault(null, directory);
        }

        /// <summary>
        /// Gets one child in the specified directory.
        /// </summary>
        /// <param name="valueSelector">Delegate to get value in an array. Can be set null and returns null when an array.</param>
        /// <returns>When not found, returns null.</returns>
        public static TreeElement<TKey, TValue> GetSingleChildOrDefault<TKey, TValue>(this TreeElement<TKey, TValue> source,
            Func<IEnumerable<TreeElement<TKey, TValue>>, IEnumerable<TKey>, TreeElement<TKey, TValue>> valueSelector,
            IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.ForAll(directory, x => x != null);

            var currentElement = source;
            var currentDirectory = new List<TKey>();
            foreach (var key in directory)
            {
                currentDirectory.Add(key);
                if (currentElement.Type == ElementType.Leaf) return null;
                var singleChild = currentElement.GetSingleChildOrDefault(key);
                if (singleChild == null)
                {
                    if (valueSelector == null) return null;
                    var childArray = currentElement.GetArrayChildOrDefault(key);
                    if (childArray == null) return null;
                    var selectedChild = valueSelector(childArray, currentDirectory.Hide());
                    if (selectedChild == null) return null;
                    currentElement = selectedChild;
                }
                else
                {
                    currentElement = singleChild;
                }
            }
            return currentElement;
        }

        public static GrandChildrenContainer<TKey, TValue> GetAllGrandChildren<TKey, TValue>(this TreeElement<TKey, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires(directory.All(k => k != null));

            var currentElement
                = new Tuple<IEnumerable<int?>, TreeElement<TKey, TValue>>(new int?[0], source); // (indexes, tree)
            IEnumerable<Tuple<IEnumerable<int?>, TreeElement<TKey, TValue>>> currentElements
                = new[] { currentElement };
            var currentDirectory = new List<TKey>();
            foreach (var key in directory)
            {
                currentDirectory.Add(key);
                currentElements = currentElements
                    .Select(tpl => new { Indexes = tpl.Item1, Child = tpl.Item2.GetChild(key) })
                    .SelectMany(a =>
                        {
                            if (a.Child == null)
                            {
                                return new Tuple<IEnumerable<int?>, TreeElement<TKey, TValue>>[0];
                            }
                            else if (a.Child.Value.IsArray)
                            {
                                var l = new List<Tuple<IEnumerable<int?>, TreeElement<TKey, TValue>>>();
                                var index = 0;
                                foreach (var v in a.Child.Value.Values)
                                {
                                    l.Add(new Tuple<IEnumerable<int?>, TreeElement<TKey, TValue>>(a.Indexes.Concat(new int?[] { index }), v));
                                    index++;
                                }

                                return l.AsEnumerable();
                            }
                            else
                            {
                                var t = new Tuple<IEnumerable<int?>, TreeElement<TKey, TValue>>(
                                    a.Indexes.Concat(new int?[] { null }), a.Child.Value.Values.Single());
                                return new[] { t };
                            }

                        });
            }
            
            var grandChildrenElements = currentElements
                .Select(tpl => new GrandChild<TKey, TValue>(tpl.Item1, tpl.Item2))
                .ToArray();
            return new GrandChildrenContainer<TKey, TValue>(directory, grandChildrenElements.AsEnumerable());
        }

        public static LeafValueContainer<TValue> GetSingleValue<TKey, TValue>(this TreeElement<TKey, TValue> source, TKey key)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(key != null);

            return source.GetSingleValue(null, new[] { key });
        }

        public static LeafValueContainer<TValue> GetSingleValue<TKey, TValue>(this TreeElement<TKey, TValue> source, params TKey[] directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.ForAll(directory, x => x != null);

            return source.GetSingleValue(null, directory.AsEnumerable());
        }

        public static LeafValueContainer<TValue> GetSingleValue<TKey, TValue>(this TreeElement<TKey, TValue> source, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.ForAll(directory, x => x != null);

            return source.GetSingleValue(null, directory);
        }

        public static LeafValueContainer<TValue> GetSingleValue<TKey, TValue>(this TreeElement<TKey, TValue> source,
            Func<IEnumerable<TreeElement<TKey, TValue>>, IEnumerable<TKey>, TreeElement<TKey, TValue>> valueSelectorInArray,
            IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.ForAll(directory, x => x != null);

            if (!directory.Any())
            {
                if (source.Type == ElementType.Leaf)
                {
                    return new LeafValueContainer<TValue>(source.LeafValue);
                }
                else
                {
                    return new LeafValueContainer<TValue>();
                }   
            }

            var child = source.GetSingleChildOrDefault(valueSelectorInArray, directory);
            if (child != null && child.Type == ElementType.Leaf)
            {
                return new LeafValueContainer<TValue>(child.LeafValue);
            }
            else
            {
                return new LeafValueContainer<TValue>();
            }
        }

        public static void MergeWithArraySelector<TKey, TValue>(this TreeElement<TKey, TValue> mergedElement, TreeElement<TKey, TValue> mergingElement)
        {
            Contract.Requires<ArgumentNullException>(mergedElement != null);
            Contract.Requires<ArgumentNullException>(mergingElement != null);

            mergedElement.MergeWithArraySelector(mergingElement, (n, d) => n);
        }

        /// <summary>TreeElement をマージします。Array -&gt; Array のときに、どの要素にマージするかを詳細に設定出来ます。それ以外のケースでは置き換えられます。</summary>
        /// <param name="keySelector">
        /// <para>First parameter: A value in the array.</para>
        /// <para>Second parameter: Current directory.</para>
        /// <para>Returning value: A key to compare.</para>
        /// </param>
        public static void MergeWithArraySelector<TKey, TValue>(this TreeElement<TKey, TValue> mergedElement, TreeElement<TKey, TValue> mergingElement, Func<TreeElement<TKey, TValue>, IEnumerable<TKey>, object> keySelector)
        {
            Contract.Requires<ArgumentNullException>(mergedElement != null);
            Contract.Requires<ArgumentNullException>(mergingElement != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);

            Func<Child<TKey, TValue>?, Child<TKey, TValue>, IEnumerable<TKey>, ChildWithoutKey<TKey, TValue>?> f = null;
            f = (o, n, d) =>
            {
                if (o != null && o.Value.IsArray && n.IsArray)
                {
                    var newArrayList = new List<TreeElement<TKey, TValue>>();
                    var oldKeys = o.Value.Values.ToDictionary(v => keySelector(v, d), v => v);
                    foreach (var v in n.Values)
                    {
                        var key = keySelector(v, d);
                        if (oldKeys.ContainsKey(key))
                        {
                            var selectedOldValue = oldKeys[key];
                            if (v.Type == ElementType.Node)
                            {
                                selectedOldValue.MergeCore(v, f, d.Concat(new[] { o.Value.Key }));
                            } // Leaf の場合は何もしない
                            newArrayList.Add(selectedOldValue);
                            oldKeys.Remove(key);
                        }
                        else
                        {
                            newArrayList.Add(v);
                        }
                    }
                    return new ChildWithoutKey<TKey, TValue>(newArrayList.Concat(oldKeys.Values));
                }
                else
                {
                    return n.ToChildWithoutKey();
                }
            };
            mergedElement.MergeCore(mergingElement, f, new TKey[0]);
        }

        /// <summary>
        /// Merges a TreeElement.
        /// </summary>
        /// <param name="mergingElement">TreeElement to merge.</param>
        /// <param name="selector">
        /// <para>Selects a value to merge. Can skip merging.</para>
        /// <para>First parameter: A single value, array, or empty(null) value to be merged. May be null.</para>
        /// <para>Second parameter: New node or leaf value to merge. May not be null.</para>
        /// <para>Third parameter: Current directory.</para>
        /// <para>Returning value: New value. Return null to not change.</para>
        /// <para>First and second parameters are not nodes at the same time.</para>
        /// </param>
        public static void Merge<TKey, TValue>(this TreeElement<TKey, TValue> mergedElement, TreeElement<TKey, TValue> mergingElement, Func<Child<TKey, TValue>?, Child<TKey, TValue>, IEnumerable<TKey>, ChildWithoutKey<TKey, TValue>?> selector)
        {
            Contract.Requires<ArgumentNullException>(mergedElement != null);
            Contract.Requires<ArgumentNullException>(mergingElement != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            mergedElement.MergeCore(mergingElement, selector, new TKey[0]);
        }

        private static void MergeCore<TKey, TValue>(this TreeElement<TKey, TValue> mergedElement, TreeElement<TKey, TValue> mergingElement, Func<Child<TKey, TValue>?, Child<TKey, TValue>, IEnumerable<TKey>, ChildWithoutKey<TKey, TValue>?> selector, IEnumerable<TKey> fetchingDirectory)
        {
            Contract.Requires<ArgumentNullException>(mergedElement != null);
            Contract.Requires<ArgumentNullException>(mergingElement != null);
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(fetchingDirectory != null);
            Contract.Requires(fetchingDirectory.All(d => !Object.Equals(d, null)));
            Contract.Requires<ArgumentException>(mergingElement.Type == ElementType.Node);

            var mergedElementAllChildren = mergedElement.GetAllChildren();
            var mergingElementAllChildren = mergingElement.GetAllChildren();
            var currentDirectory = fetchingDirectory.ToList();
            foreach (var newChild in mergingElementAllChildren)
            {
                var newKey = newChild.Key;
                currentDirectory.Add(newKey);
                var mergedElementChild = mergedElementAllChildren.SingleOrNull(c => Object.Equals(c.Key, newKey));
                var mergingElementChild = mergingElementAllChildren.Single(c => Object.Equals(c.Key, newKey));
                if (mergedElementChild != null 
                    && !mergedElementChild.Value.IsArray 
                    && !mergingElementChild.IsArray
                    && mergedElementChild.Value.Values.Single().Type == ElementType.Node
                    && mergingElementChild.Values.Single().Type == ElementType.Node
                    )
                {
                    mergedElementChild.Value.Values.Single().MergeCore(
                        mergingElementChild.Values.Single(),
                        selector,
                        fetchingDirectory);
                }

                var selectedChildToMerge = selector(
                    mergedElementChild,
                    mergingElementChild,
                    fetchingDirectory.Concat(new[] { newKey }));
                if (selectedChildToMerge == null) return;
                if (selectedChildToMerge.Value.IsArray)
                {
                    mergedElement.ModifyArrayChild(newKey,
                        _ => selectedChildToMerge.Value.Values,
                        ary => { ary.Clear(); ary.AddRange(selectedChildToMerge.Value.Values); }
                        );
                }
                else
                {
                    mergedElement.SetSingleChild(newKey, selectedChildToMerge.Value.Values.Single());
                }
            }
        }
    }
}
