using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.ReactiveTree.TreeElements;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.Merging
{
    internal class TreeElementNotifier<TKey, TValue> : Disposable, IDirectoryValueChanged<TKey, TValue>, IStopSubscription
    {
        /// <summary>Pairs of directory and its subject. Keys indicate its directory.</summary>
        private IDictionary<IEnumerable<TKey>, TreeElementChangesObserver<TKey, TValue>> watchingJsonAndDirectory
            = new Dictionary<IEnumerable<TKey>, TreeElementChangesObserver<TKey, TValue>>(EqualityComparer.EnumerableOf<TKey>());

        public TreeElementNotifier()
            : this(new TreeElement<TKey, TValue>())
        {
            
        }

        public TreeElementNotifier(TreeElement<TKey, TValue> initElement)
        {
            Contract.Requires<ArgumentNullException>(initElement != null);
            Contract.Requires<ArgumentException>(initElement.Type == ElementType.Node, "Must be a node.");

            this.p_currentTree = initElement;
            this.IsSubscribing = true;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.CurrentTree != null);
            Contract.Invariant(this.watchingJsonAndDirectory != null);
        }

        public GrandChildrenContainer<TKey, TValue> GetValue(IEnumerable<TKey> keyDirectory)
        {
            ThrowExceptionIfDisposed();
            return this.CurrentTree.GetAllGrandChildren(keyDirectory);
        }

        public IObservable<GrandChildrenContainer<TKey, TValue>> ValueChanged(IEnumerable<TKey> keyDirectory)
        {
            // No contracts written here because interface always defines them.

            ThrowExceptionIfDisposed();
            if (this.watchingJsonAndDirectory.ContainsKey(keyDirectory) != true)
            {
                this.watchingJsonAndDirectory[keyDirectory] =
                    new TreeElementChangesObserver<TKey, TValue>(this.CurrentTree, keyDirectory);
            }
            var obs = this.watchingJsonAndDirectory[keyDirectory];
            return obs.ValueChanged;
        }

        private TreeElement<TKey, TValue> p_currentTree;
        public TreeElement<TKey, TValue> CurrentTree
        {
            get
            {
                Contract.Ensures(Contract.Result<TreeElement<TKey, TValue>>() != null);

                ThrowExceptionIfDisposed();
                return this.p_currentTree;
            }
        }

        protected override void OnDisposingManagedResources()
        {
            StopSubscription();
            base.OnDisposingManagedResources();
            this.watchingJsonAndDirectory.ForEach(p => p.Value.Dispose());
        }

        public bool StopSubscription()
        {
            if (this.IsSubscribing)
            {
                this.watchingJsonAndDirectory.ForEach(p => p.Value.StopSubscription());
                this.IsSubscribing = false;
                return true;
            }
            return false;
        }

        public bool IsSubscribing
        {
            get;
            private set;
        }
    }
}
