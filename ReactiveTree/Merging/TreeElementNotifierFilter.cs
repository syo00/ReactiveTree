using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;
using Kirinji.ReactiveTree.TreeStructures;

namespace Kirinji.ReactiveTree.Merging
{
    /// <summary>Make IDirectoryValueChanged not to subscribe changes in specified directory,</summary>
    public class TreeElementNotifierFilter<TKey, TValue> : Disposable, IDirectoryValueChanged<TKey, TValue>
    {
        readonly IDirectoryValueChanged<TKey, TValue> inner;
        readonly Func<IEnumerable<KeyArray<NodeKeyOrArrayIndex<TKey>>>, bool> filter;

        /// <param name="filter">Parameter gives directory. Return false to ignore subscription.</param>
        public TreeElementNotifierFilter(IDirectoryValueChanged<TKey, TValue> inner, Func<IEnumerable<KeyArray<NodeKeyOrArrayIndex<TKey>>>, bool> filter)
        {
            Contract.Requires<ArgumentNullException>(inner != null);
            Contract.Requires<ArgumentNullException>(filter != null);

            this.inner = inner;
            this.filter = filter;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.inner != null);
            Contract.Invariant(this.filter != null);
        }

        public IObservable<IEnumerable<ElementDirectory<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<NodeKeyOrArrayIndex<TKey>>> directories)
        {
            if (filter(directories)) return this.inner.ValuesChanged(directories);
            return Observable.Empty<IEnumerable<ElementDirectory<TKey, TValue>>>();
        }

        public IEnumerable<TreeStructures.ElementDirectory<TKey, TValue>> GetValues(IEnumerable<TreeStructures.KeyArray<TreeStructures.NodeKeyOrArrayIndex<TKey>>> directories)
        {
            return this.inner.GetValues(directories);
        }
    }
}
