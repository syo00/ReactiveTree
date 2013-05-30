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
    public class TreeElementNotifierFilter<TKey, TValue> : Disposable, ISimpleReactiveDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>>
    {
        readonly ISimpleReactiveDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>> inner;
        readonly Func<KeyArray<KeyOrIndex<TKey>>, bool> filter;

        /// <param name="filter">Parameter gives directory. Return false to ignore subscription.</param>
        public TreeElementNotifierFilter(ISimpleReactiveDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>> inner, Func<KeyArray<KeyOrIndex<TKey>>, bool> filter)
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

        public IReadOnlyDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>> Values(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            return this.inner.Values(directories);
        }

        public IObservable<IReadOnlyDictionary<KeyArray<KeyOrIndex<TKey>>, TreeElement<TKey, TValue>>> ValuesChanged(IEnumerable<KeyArray<KeyOrIndex<TKey>>> directories)
        {
            return inner.ValuesChanged(directories.Where(dir => filter(dir)));
        }
    }
}
