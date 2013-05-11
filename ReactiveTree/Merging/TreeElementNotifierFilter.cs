using Kirinji.ReactiveTree.TreeElements;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.Merging
{
    /// <summary>Make IDirectoryValueChanged not to subscribe changes in specified directory,</summary>
    internal class TreeElementNotifierFilter<TKey, TValue> : Disposable, IDirectoryValueChanged<TKey, TValue>, IStopSubscription
    {
        readonly IDirectoryValueChanged<TKey, TValue> inner;
        readonly Func<IEnumerable<TKey>, bool> filter;
        bool skipCastingToStopSubscription;
        IStopSubscription castedInnerCache;

        /// <param name="filter">Parameter gives directory. Return false to ignore subscription.</param>
        public TreeElementNotifierFilter(IDirectoryValueChanged<TKey, TValue> inner, Func<IEnumerable<TKey>, bool> filter)
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


        public GrandChildrenContainer<TKey, TValue> GetValue(IEnumerable<TKey> keyDirectory)
        {
            return this.inner.GetValue(keyDirectory);
        }

        public IObservable<GrandChildrenContainer<TKey, TValue>> ValueChanged(IEnumerable<TKey> keyDirectory)
        {
            if (this.filter(keyDirectory)) return this.inner.ValueChanged(keyDirectory);
            return Observable.Empty<GrandChildrenContainer<TKey, TValue>>();
        }

        public bool StopSubscription()
        {
            var castedInner = AsStopSubscription();
            if (castedInner == null) return false;
            return castedInner.StopSubscription();
        }

        public bool IsSubscribing
        {
            get
            {
                var castedInner = AsStopSubscription();
                if (castedInner == null) return false;
                return castedInner.IsSubscribing;
            }
        }

        private IStopSubscription AsStopSubscription()
        {
            if (this.skipCastingToStopSubscription) return null;
            if (this.castedInnerCache == null)
            {
                this.castedInnerCache = this.inner as IStopSubscription;
                if (this.castedInnerCache == null)
                {
                    this.skipCastingToStopSubscription = true;
                    return null;
                }
            }
            return this.castedInnerCache;
        }

        protected override void OnDisposingManagedResources()
        {
            StopSubscription();
            TryDispose(this.inner);
        }
    }
}
