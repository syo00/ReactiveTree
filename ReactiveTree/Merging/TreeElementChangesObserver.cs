using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;
using Kirinji.ReactiveTree.TreeElements;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.Merging
{
    /// <summary>Observes one tree element changes.</summary>
    internal class TreeElementChangesObserver<TKey, TValue> : Kirinji.LightWands.Disposable, IStopSubscription
    {
        private TreeElement<TKey, TValue> watchingTreeElement;
        private IEnumerable<TKey> watchingDirectory;
        private CompositeDisposable subscriptionDisposable = new CompositeDisposable();
        private GrandChildrenContainer<TKey, TValue> latestValue;
        private ISubject<GrandChildrenContainer<TKey, TValue>> valueChangedSubject;
        private IObservable<GrandChildrenContainer<TKey, TValue>> valueChanged;

        public TreeElementChangesObserver(TreeElement<TKey, TValue> watchingTreeElement, IEnumerable<TKey> watchingDirectory)
        {
            Contract.Requires<ArgumentNullException>(watchingTreeElement != null);
            Contract.Requires<ArgumentNullException>(watchingDirectory != null);
            Contract.ForAll(watchingDirectory, v => v != null);

            this.watchingTreeElement = watchingTreeElement;
            this.watchingDirectory = watchingDirectory;
            UpdateReferences();
            this.latestValue = GetValue();
            this.valueChangedSubject = new Subject<GrandChildrenContainer<TKey, TValue>>();
            this.valueChanged = this.valueChangedSubject.AsObservable();
            this.IsSubscribing = true;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.watchingTreeElement != null);
            Contract.Invariant(this.watchingDirectory != null);
            Contract.Invariant(this.watchingDirectory.All(v => v != null));
            Contract.Invariant(this.subscriptionDisposable != null);
            Contract.Invariant(this.latestValue != null);
            Contract.Invariant(this.valueChangedSubject != null);
            Contract.Invariant(this.valueChanged != null);
        }

        private void Update()
        {
            UpdateReferences();
            this.latestValue = GetValue();
            this.valueChangedSubject.OnNext(this.latestValue);
        }

        private void UpdateReferences()
        {
            this.subscriptionDisposable.Dispose();
            this.subscriptionDisposable = new CompositeDisposable();

            this.subscriptionDisposable.Add(this.watchingTreeElement.ChildrenChanged.Subscribe(_ => Update()));
            SplitTreeElement(this.watchingTreeElement, this.watchingDirectory)
                .Where(e => e.Value.Type == ElementType.Node)
                .ForEach(e => this.subscriptionDisposable.Add(e.Value.ChildrenChanged.Subscribe(_ => Update())));
        }

        private GrandChildrenContainer<TKey, TValue> GetValue()
        {
            var grandChildren = this.watchingTreeElement.GetAllGrandChildren(this.watchingDirectory);
            return grandChildren;
        }

        private static IEnumerable<KeyValuePair<TKey, TreeElement<TKey, TValue>>> SplitTreeElement(TreeElement<TKey, TValue> treeElement, IEnumerable<TKey> directory)
        {
            Contract.Requires<ArgumentNullException>(treeElement != null);
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<TKey, TreeElement<TKey, TValue>>>>() != null);

            var currentElement = treeElement;
            IList<KeyValuePair<TKey, TreeElement<TKey, TValue>>> r = new List<KeyValuePair<TKey, TreeElement<TKey, TValue>>>();
            foreach (var key in directory)
            {
                if (currentElement.Type != ElementType.Node) return r;
                var e = currentElement.GetSingleChildOrDefault(key);
                if (e == null) return r;
                r.Add(new KeyValuePair<TKey, TreeElement<TKey, TValue>>(key, e));
                currentElement = e;
            }
            return r;
        }

        public GrandChildrenContainer<TKey, TValue> Value
        {
            get
            {
                ThrowExceptionIfDisposed();
                return this.latestValue;
            }
        }

        public IObservable<GrandChildrenContainer<TKey, TValue>> ValueChanged
        {
            get
            {
                ThrowExceptionIfDisposed();
                return this.valueChanged;
            }
        }

        public TreeElement<TKey, TValue> TreeElement
        {
            get
            {
                ThrowExceptionIfDisposed();
                return this.watchingTreeElement;
            }
        }

        public IEnumerable<TKey> Directory
        {
            get
            {
                ThrowExceptionIfDisposed();
                return this.watchingDirectory;
            }
        }

        public bool StopSubscription()
        {
            if (this.IsSubscribing)
            {
                this.valueChangedSubject.OnCompleted();
                this.subscriptionDisposable.Dispose();
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

        protected override void OnDisposingManagedResources()
        {
            StopSubscription();
            TryDispose(this.valueChangedSubject);
            this.subscriptionDisposable.Dispose();
        }
    }
}
