using Kirinji.LightWands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    public static class TreeElement
    {
        public static void Merge<TKey, TValue>(this TreeElement<TKey, TValue> source, TreeElement<TKey, TValue> second, Func<TreeElement<TKey, TValue>, TreeElement<TKey, TValue>, bool> comparerToMerge)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(second != null);
            Contract.Requires<InvalidOperationException>(
                source.Type == ElementType.Array && second.Type == ElementType.Array
                || source.Type == ElementType.Node && second.Type == ElementType.Node);

            if (source.Type == ElementType.Array) // array
            {
                foreach (var i in second.Array.Indexes())
                {
                    if (i.Key < source.Array.Count)
                    // replace or merge
                    {
                        var oldElement = source.Array[i.Key];
                        var newElement = i.Value;
                        if (comparerToMerge != null
                            && comparerToMerge(oldElement, newElement)
                            && oldElement.Type == newElement.Type
                            && (oldElement.Type == ElementType.Array || oldElement.Type == ElementType.Node))
                        // merge
                        {
                            oldElement.Merge(newElement, comparerToMerge);
                        }
                        else
                        // replace
                        {
                            source.Array[i.Key] = newElement;
                        }
                    }
                    else
                    // add
                    {
                        source.Array.Add(i.Value);
                    }
                }
            }
            else // node
            {
                foreach (var pair in second.NodeChildren)
                {
                    if (source.NodeChildren.ContainsKey(pair.Key))
                    // replace or merge
                    {
                        var oldElement = source.NodeChildren[pair.Key];
                        var newElement = pair.Value;
                        if (comparerToMerge != null
                            && comparerToMerge(oldElement, newElement)
                            && oldElement.Type == newElement.Type
                            && (oldElement.Type == ElementType.Array || oldElement.Type == ElementType.Node))
                        // merge
                        {
                            oldElement.Merge(newElement, comparerToMerge);
                        }
                        else
                        // replace (or add)
                        {
                            source.NodeChildren[pair.Key] = newElement;
                        }
                    }
                    else
                    // add
                    {
                        source.NodeChildren.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        public static TreeElement<TKey, TValue> GetOrDefault<TKey, TValue>(this TreeElement<TKey, TValue> source, IEnumerable<KeyOrIndex<TKey>> directory)
        {
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires<ArgumentNullException>(Contract.ForAll(directory, key => key != null));

            return (TreeElement<TKey, TValue>)ReadOnlyTreeElement.GetOrDefault(source, directory);
        }

        public static TreeElement<TKey, TValue> GetOrDefault<TKey, TValue>(this TreeElement<TKey, TValue> source, params KeyOrIndex<TKey>[] directory)
        {
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires<ArgumentNullException>(Contract.ForAll(directory, key => key != null));

            return (TreeElement<TKey, TValue>)ReadOnlyTreeElement.GetOrDefault(source, directory);
        }
    }
}
