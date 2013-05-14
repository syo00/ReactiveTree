using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public class SeriesTreeElement<TKey, TValue> : ITreeElement<TKey, TValue>
    {
        private TreeElement<TKey, TValue> coreTreeElement;
        private readonly object id;

        public SeriesTreeElement(TreeElement<TKey, TValue> treeElement, object id)
        {
            Contract.Requires<ArgumentNullException>(treeElement != null);
            Contract.Requires<ArgumentNullException>(id != null);

            this.coreTreeElement = treeElement;
            this.id = id;
        }

        public ElementType Type
        {
            get { return coreTreeElement.Type; }
        }

        public IEnumerable<TKey> Keys
        {
            get { return coreTreeElement.Keys; }
        }

        public TreeElement<TKey, TValue> this[TKey key]
        {
            get
            {
                return coreTreeElement[key];
            }
            set
            {
                coreTreeElement.IndexerSet(key, value, id);
            }
        }

        public IEnumerable<Child<TKey, TValue>> GetAllChildren()
        {
            return coreTreeElement.GetAllChildren();
        }

        public IEnumerable<TreeElement<TKey, TValue>> GetArrayChildOrDefault(TKey key)
        {
            return coreTreeElement.GetArrayChildOrDefault(key);
        }

        public TreeElement<TKey, TValue> GetSingleChildOrDefault(TKey key)
        {
            return coreTreeElement.GetSingleChildOrDefault(key);
        }

        public TValue LeafValue
        {
            get { return coreTreeElement.LeafValue; }
        }

        public void SetSingleChild(TKey key, TreeElement<TKey, TValue> newChild)
        {
            coreTreeElement.SetSingleChild(key, newChild, id);
        }

        public void ModifyArrayChild(TKey key, Func<TreeElement<TKey, TValue>, IEnumerable<TreeElement<TKey, TValue>>> arrayCreator, Action<IList<TreeElement<TKey, TValue>>> arrayModifier)
        {
            coreTreeElement.ModifyArrayChild(key, arrayCreator, arrayModifier, id);
        }

        public void ModifyTreeAsSeries(Action<ITreeElement<TKey, TValue>> treeEditor)
        {
            treeEditor(this);
        }

        public bool Equals(ITreeElement<TKey, TValue> other)
        {
            return coreTreeElement.Equals(other);
        }
    }
}
