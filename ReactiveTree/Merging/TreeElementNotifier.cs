using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Kirinji.LightWands;
using Kirinji.ReactiveTree.TreeStructures;
using System.Collections.Specialized;
using System.Reactive.Disposables;

namespace Kirinji.ReactiveTree.Merging
{
    public class TreeElementNotifier<K, V> : IDirectoryValueChanged<K, V>
    {
        private bool isModifyingStraight;
        private ISubject<IEnumerable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>>> modifyingStraightSubject = new Subject<IEnumerable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>>>();
        private IObservable<IEnumerable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>>> rawValueChanged;

        private static IList<object> preventDisposingObjects = new List<object>(); // To prevent to be disposed as an weak reference element of TreeElementDictionary, add objects to this list. To change to be GCed, remove the objects from this list.

        public TreeElementNotifier()
            : this(new TreeElement<K, V>())
        {
            
        }

        public TreeElementNotifier(TreeElement<K, V> initElement)
        {
            Contract.Requires<ArgumentNullException>(initElement != null);
            Contract.Requires<ArgumentException>(initElement.Type == ElementType.Node, "Must be a node.");

            this.currentTree = initElement;
            this.rawValueChanged
                = CurrentTree
                .GrandChildrenChanged
                .Where(_ => !isModifyingStraight)
                .Select(x => new[] { x })
                .Merge(modifyingStraightSubject)
                .Publish()
                .RefCount();
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.CurrentTree != null);
        }

        private TreeElement<K, V> currentTree;
        public TreeElement<K, V> CurrentTree
        {
            get
            {
                Contract.Ensures(Contract.Result<TreeElement<K, V>>() != null);

                return this.currentTree;
            }
        }

        public void ModifyCurrentTreeStraight(Action<TreeElement<K, V>> modifyingAction)
        {
            Contract.Requires<ArgumentNullException>(modifyingAction != null);

            isModifyingStraight = true;
            var l = new List<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>>();
            var s = CurrentTree.GrandChildrenChanged.Subscribe(l.Add);
            modifyingAction(CurrentTree);
            s.Dispose();
            modifyingStraightSubject.OnNext(l);
            isModifyingStraight = false;
        }

        public IObservable<IEnumerable<ElementDirectory<K, V>>> ValuesChanged(IEnumerable<KeyArray<KeyOrIndex<K>>> directories)
        {
            preventDisposingObjects.Add(this);

            var inner = rawValueChanged
                .Select(pairsChanged =>
                {
                    var rtn = new List<ElementDirectory<K, V>>();
                    foreach (var dir in directories)
                    {
                        // If in case that dir is [1, 2, 3]:
                        // [1, 2] is matched
                        // [1, 2, 3] is matched
                        // [1, 2, 4] is not matched
                        // [1, 3] is not matched

                        var isMatched = pairsChanged.Any(ed =>
                            {
                                var dirEnumerator = dir.GetEnumerator();
                                var changedDirEnumerator = ed.Key.GetEnumerator();
                                while (true)
                                {
                                    if (changedDirEnumerator.MoveNext())
                                    {
                                        if (!dirEnumerator.MoveNext())
                                        {
                                            return false;
                                        }
                                        if (!Object.Equals(changedDirEnumerator.Current, dirEnumerator.Current))
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                            });

                        if (isMatched)
                        {
                            rtn.Add(new ElementDirectory<K, V>(new KeyArray<KeyOrIndex<K>>(dir), CurrentTree.GetOrDefault(dir)));
                        }
                    }
                    return rtn;
                })
                .Where(list => list.Count != 0);

            return Observable.Create<IEnumerable<ElementDirectory<K, V>>>(observer =>
                {
                    var subscription = inner.Subscribe(observer);
                    return new CompositeDisposable(
                        subscription,
                        System.Reactive.Disposables.Disposable.Create(() => preventDisposingObjects.Remove(this)));
                });
        }

        public IEnumerable<ElementDirectory<K, V>> GetValues(IEnumerable<KeyArray<KeyOrIndex<K>>> directories)
        {
            return directories
                .Select(d => new { Key = d, Value = CurrentTree.GetOrDefault(d) })
                .Select(a => new ElementDirectory<K, V>(a.Key, a.Value))
                .ToArray();
        }
    }
}
