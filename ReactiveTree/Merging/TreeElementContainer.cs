using Kirinji.ReactiveTree.TreeStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.Merging
{
    // TreeElementNotifier の、TreeElement の変更通知をしないバージョン
    internal class TreeElementContainer<TKey, TValue> : IDirectoryValueChanged<TKey, TValue>
    {
        private TreeElement<TKey, TValue> currentTree;

        public TreeElementContainer(TreeElement<TKey, TValue> initElement)
        {
            Contract.Requires<ArgumentNullException>(initElement != null);

            this.currentTree = initElement;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(currentTree != null);
        }

        public IObservable<IEnumerable<ElementDirectory<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<NodeKeyOrArrayIndex<TKey>>> directories)
        {
            return Observable.Return(GetValues(directories));
        }

        public IEnumerable<ElementDirectory<TKey, TValue>> GetValues(IEnumerable<KeyArray<NodeKeyOrArrayIndex<TKey>>> directories)
        {
            return directories
                .Select(d => new { Key = d, Value = currentTree.GetOrDefault(d) })
                .Select(a => new ElementDirectory<TKey, TValue>(a.Key, a.Value))
                .ToArray();
        }
    }
}
