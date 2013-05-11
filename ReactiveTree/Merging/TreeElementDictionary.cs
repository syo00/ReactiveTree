using Kirinji.ReactiveTree.TreeElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.Merging
{
    /// <summary>Add or merge tree elements by comparing key.</summary>
    internal class TreeElementDictionary<TKey, TElementKey, TElementValue>
    {
        public class TreeElementAndNotifierPair
        {
            public TreeElementAndNotifierPair(TKey key, TreeElement<TElementKey, TElementValue> treeElement)
            {
                Contract.Requires<ArgumentNullException>(key != null);
                Contract.Requires<ArgumentNullException>(treeElement != null);

                this.Key = key;
                this.TreeElement = treeElement;
                this.Notifier = new TreeElementNotifier<TElementKey, TElementValue>(treeElement);
            }

            [ContractInvariantMethod]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.TreeElement != null);
                Contract.Invariant(this.Notifier != null);
                Contract.Invariant(this.Key != null);
            }

            public TKey Key
            {
                get;
                private set;
            }

            public TreeElement<TElementKey, TElementValue> TreeElement
            {
                get;
                private set;
            }

            public TreeElementNotifier<TElementKey, TElementValue> Notifier
            {
                get;
                private set;
            }
        }

        Func<TreeElement<TElementKey, TElementValue>, TKey> keySelector;
        Func<TKey, bool> keyFilter;
        IEqualityComparer<TKey> keyComparer;
        IDictionary<TKey, WeakReference<TreeElementAndNotifierPair>> treeElementsList;

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TKey> keySelector)
            : this(keySelector, _ => true, null)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
        }

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
            : this(keySelector, _ => true, keyComparer)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
        }

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TKey> keySelector, Func<TKey, bool> keyFilter)
            : this(keySelector, keyFilter, null)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(keyFilter != null);
        }

        public TreeElementDictionary(Func<TreeElement<TElementKey, TElementValue>, TKey> keySelector, Func<TKey, bool> keyFilter, IEqualityComparer<TKey> keyComparer)
        {
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(keyFilter != null);

            this.keySelector = keySelector;
            this.keyFilter = keyFilter;
            this.keyComparer = keyComparer;
            this.treeElementsList = new Dictionary<TKey, WeakReference<TreeElementAndNotifierPair>>(keyComparer);
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
        public TreeElementAndNotifierPair Merge(TreeElement<TElementKey, TElementValue> treeElement)
        {
            Contract.Requires<ArgumentNullException>(treeElement != null);
           
            var key = this.keySelector(treeElement);
            if (key == null) throw new ArgumentNullException("keySelector must not return null.");
            if (!this.keyFilter(key)) return null;
            var matchedWeakReference = this.treeElementsList.ValueOrDefault(key);
            TreeElementAndNotifierPair matchedPair;
            if (matchedWeakReference != null && matchedWeakReference.TryGetTarget(out matchedPair))
            {
                matchedPair.TreeElement.MergeWithArraySelector(treeElement);
                return matchedPair;
            }
            else
            {
                var notifier = new TreeElementAndNotifierPair(key, treeElement);
                this.treeElementsList[key] = new WeakReference<TreeElementAndNotifierPair>(notifier);
                return notifier;
            }
        }

        public IDictionary<TKey, WeakReference<TreeElementAndNotifierPair>> GetAllTreeElement()
        {
            return new Dictionary<TKey, WeakReference<TreeElementAndNotifierPair>>(this.treeElementsList);
        }

        public WeakReference<TreeElementAndNotifierPair> ValueOrDefault(TKey key)
        {
            Contract.Requires<ArgumentNullException>(key != null);

            return this.treeElementsList.ValueOrDefault(key);
        }
    }
}
