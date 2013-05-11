using Kirinji.ReactiveTree.TreeElements;
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
        public TreeElementContainer(TreeElement<TKey, TValue> initElement)
        {
            Contract.Requires<ArgumentNullException>(initElement != null);

            this.p_currentTree = initElement;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.p_currentTree != null);
        }

        public GrandChildrenContainer<TKey, TValue> GetValue(IEnumerable<TKey> keyDirectory)
        {
            return this.p_currentTree.GetAllGrandChildren(keyDirectory);
        }

        public IObservable<GrandChildrenContainer<TKey, TValue>> ValueChanged(IEnumerable<TKey> keyDirectory)
        {
            return Observable.Empty<GrandChildrenContainer<TKey, TValue>>();
        }

        private TreeElement<TKey, TValue> p_currentTree;
    }
}
