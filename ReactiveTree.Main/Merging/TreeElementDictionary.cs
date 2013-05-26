using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;
using Kirinji.ReactiveTree.TreeStructures;

namespace Kirinji.ReactiveTree.Merging
{
    /// <summary>Add or merge tree elements by comparing key.</summary>
    public class TreeElementDictionary<TDictionaryKey, TElementKey, TElementValue>
    {
        Func<TreeElement<TElementKey, TElementValue>, TDictionaryKey> keySelector;
        Func<TDictionaryKey, bool> keyFilter;
        IEqualityComparer<TDictionaryKey> keyComparer;
        IDictionary<TDictionaryKey, WeakReference<TreeElementNotifier<TElementKey, TElementValue>>> treeElementsList;

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TDictionaryKey> keySelector)
            : this(keySelector, _ => true, null)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
        }

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TDictionaryKey> keySelector, IEqualityComparer<TDictionaryKey> keyComparer)
            : this(keySelector, _ => true, keyComparer)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
        }

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TDictionaryKey> keySelector, Func<TDictionaryKey, bool> keyFilter)
            : this(keySelector, keyFilter, null)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(keyFilter != null);
        }

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TDictionaryKey> keySelector, Func<TDictionaryKey, bool> keyFilter, IEqualityComparer<TDictionaryKey> keyComparer)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(keyFilter != null);

            this.keySelector = keySelector;
            this.keyFilter = keyFilter;
            this.keyComparer = keyComparer;
            this.treeElementsList = new Dictionary<TDictionaryKey, WeakReference<TreeElementNotifier<TElementKey, TElementValue>>>(keyComparer);
        }

        public static TreeElementDictionary<TreeElement<TElementKey, TElementValue>, TElementKey, TElementValue> CreateDefault()
        {
            return new TreeElementDictionary<TreeElement<TElementKey, TElementValue>, TElementKey, TElementValue>(tree => tree, tree => tree != null);
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.keySelector != null);
            Contract.Invariant(this.keyFilter != null);
        }

        /// <summary>Add or merge tree elements by comparing key. When the key is matched, tree element is merged. When not, added.</summary>
        /// <returns>When the key is matched, returns the merged tree element and its notifier. When not, returns same  tree element and its notifier.</returns>
        public TreeElementNotifier<TElementKey, TElementValue> Merge(
            TreeElement<TElementKey, TElementValue> treeElement,
            Func<TreeElement<TElementKey, TElementValue>,TreeElement<TElementKey, TElementValue>,bool> mergeComparer)
        {
            Contract.Requires<ArgumentNullException>(treeElement != null);
           
            var key = this.keySelector(treeElement);
            if (key == null) throw new ArgumentNullException("keySelector must not return null.");
            if (!this.keyFilter(key)) return null;
            var matchedWeakReference = this.treeElementsList.ValueOrDefault(key);
            TreeElementNotifier<TElementKey, TElementValue> matched;
            if (matchedWeakReference != null && matchedWeakReference.TryGetTarget(out matched))
            {
                matched.ModifyCurrentTreeStraight(tree => tree.Merge(treeElement, mergeComparer));
                return matched;
            }
            else
            {
                var notifier = new TreeElementNotifier<TElementKey, TElementValue>(treeElement);
                this.treeElementsList[key] = new WeakReference<TreeElementNotifier<TElementKey, TElementValue>>(notifier);
                return notifier;
            }
        }

        public IReadOnlyDictionary<TDictionaryKey, WeakReference<TreeElementNotifier<TElementKey, TElementValue>>> GetAllTreeElement()
        {
            return new Dictionary<TDictionaryKey, WeakReference<TreeElementNotifier<TElementKey, TElementValue>>>(this.treeElementsList);
        }

        public WeakReference<TreeElementNotifier<TElementKey, TElementValue>> ValueOrDefault(TDictionaryKey key)
        {
            Contract.Requires<ArgumentNullException>(key != null);

            return this.treeElementsList.ValueOrDefault(key);
        }
    }
}
