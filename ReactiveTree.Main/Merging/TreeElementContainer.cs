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
    public class TreeElementContainer<TKey, TValue> : ISimpleReactiveDictionary<KeyArray<KeyOrIndex<TKey>>, IReadOnlyTreeElement<TKey, TValue>>
    {
        private IReadOnlyTreeElement<TKey, TValue> currentTree;

        public TreeElementContainer(IReadOnlyTreeElement<TKey, TValue> initElement)
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

        public IReadOnlyDictionary<KeyArray<KeyOrIndex<TKey>>, IReadOnlyTreeElement<TKey, TValue>> Values(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            return directories
                .Select(d => new { Key = d, Value = currentTree.GetOrDefault(d) })
                .ToDictionary(a => a.Key, a => a.Value);
        }

        public IObservable<IReadOnlyDictionary<KeyArray<KeyOrIndex<TKey>>, IReadOnlyTreeElement<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            return Observable.Return(Values(directories));
        }
    }
}
