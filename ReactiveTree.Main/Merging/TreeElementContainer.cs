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
    public class TreeElementContainer<TKey, TValue> : IReactiveTree<KeyOrIndex<TKey>, TreeElement<TKey, TValue>>
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

        public IReadOnlyDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>> Values(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            return directories
                .Select(d => new { Key = d, Value = currentTree.GetOrDefault(d) })
                .ToDictionary(a => a.Key, a => a.Value);
        }

        public IObservable<IReadOnlyDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            return Observable.Return(Values(directories));
        }
    }
}
