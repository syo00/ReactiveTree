using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Kirinji.LightWands;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace Kirinji.ReactiveTree.TreeStructures
{
    // K means Key and V means Value: To shorten codes, uses one character type.
    public class TreeElement<K, V> : IEquatable<TreeElement<K, V>>, IReadOnlyTreeElement<K,V>
    {
        private readonly ObservableDictionary<K, TreeElement<K, V>> nodeChildren;
        private readonly ObservableCollection<TreeElement<K, V>> arrayValues;
        private readonly V leafValue;
        private readonly ElementType type;

        /// <summary>Creates a leaf.</summary>
        public TreeElement(V leafValue)
        {
            this.leafValue = leafValue;
            this.type = ElementType.Leaf;
        }

        /// <summary>Creates an array.</summary>
        public TreeElement(IEnumerable<TreeElement<K, V>> arrayValues)
        {
            Contract.Requires<ArgumentNullException>(arrayValues != null);
            Contract.Requires(Contract.ForAll(arrayValues, a => a != null));

            this.arrayValues = new ObservableCollection<TreeElement<K, V>>(arrayValues);
            this.type = ElementType.Array;
        }

        /// <summary>Creates an array.</summary>
        public TreeElement(params TreeElement<K, V>[] arrayValues)
            : this(arrayValues.AsEnumerable())
        {
            Contract.Requires<ArgumentNullException>(arrayValues != null);
            Contract.Requires(Contract.ForAll(arrayValues, a => a != null));
        }

        /// <summary>Creates a node.</summary>
        public TreeElement(IEnumerable<KeyValuePair<K, TreeElement<K, V>>> nodeChildren)
            : this(nodeChildren, false)
        {
            Contract.Requires<ArgumentNullException>(nodeChildren != null);
            Contract.Requires(Contract.ForAll(nodeChildren, p => p.Key != null && p.Value != null));
        }

        /// <summary>Creates a node.</summary>
        public TreeElement(IEnumerable<KeyValuePair<K, TreeElement<K, V>>> nodeChildren, bool convertDuplicateValuesToArray)
        {
            Contract.Requires<ArgumentNullException>(nodeChildren != null);
            Contract.Requires(Contract.ForAll(nodeChildren, p => p.Key != null && p.Value != null));

            this.nodeChildren = new ObservableDictionary<K, TreeElement<K, V>>();
            foreach (var lookup in nodeChildren.ToLookup(pair => pair.Key))
            {
                if (lookup.Count() >= 2)
                {
                    if (!convertDuplicateValuesToArray) throw new ArgumentException("Contains duplicate values.");
                    var array = new TreeElement<K, V>(lookup.Select(l => l.Value));
                    this.nodeChildren.Add(lookup.Key, array);
                }
                else
                {
                    this.nodeChildren.Add(lookup.Single());
                }
            }
            this.type = ElementType.Node;
        }

        /// <summary>Creates a node.</summary>
        public TreeElement(params KeyValuePair<K, TreeElement<K, V>>[] nodeChildren)
            : this(nodeChildren.AsEnumerable())
        {
            Contract.Requires<ArgumentNullException>(nodeChildren != null);
            Contract.Requires(Contract.ForAll(nodeChildren, p => p.Key != null && p.Value != null));
        }

        // TODO: Add Invariant methods

        public ElementType Type
        {
            get
            {
                return type;
            }
        }

        public IList<TreeElement<K, V>> Array
        {
            get
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Array, TreeElementMessages.NotArrayErrorMessage);
                Contract.Ensures(Contract.Result<IList<TreeElement<K, V>>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IList<TreeElement<K, V>>>(), e => e != null));

                return arrayValues;
            }
        }

        IEnumerable<IReadOnlyTreeElement<K, V>> IReadOnlyTreeElement<K, V>.Array
        {
            get
            {
                // contract is defined at IReadOnlyTreeElement

                return Array.Hide();
            }
        }

        private IObservable<NotifyCollectionChangedEventArgs> arrayChanged;
        public IObservable<NotifyCollectionChangedEventArgs> ArrayChanged
        {
            get
            {
                // contract is defined at IReadOnlyTreeElement

                if (arrayChanged == null)
                {
                    arrayChanged
                        = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => h.Invoke,
                        h => this.arrayValues.CollectionChanged += h,
                        h => this.arrayValues.CollectionChanged -= h)
                        .Select(h => h.EventArgs);
                }
                return arrayChanged;
            }
        }

        public V LeafValue
        {
            get
            {
                // contract is defined at IReadOnlyTreeElement

                return leafValue;
            }
        }

        public IDictionary<K, TreeElement<K, V>> NodeChildren
        {
            get
            {
                Contract.Requires<InvalidOperationException>(Type == ElementType.Node, TreeElementMessages.NotNodeErrorMessage);
                Contract.Ensures(Contract.Result<IDictionary<K, TreeElement<K, V>>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IDictionary<K, TreeElement<K, V>>>(), e => e.Key != null && e.Value != null));

                return nodeChildren;
            }
        }

        IEnumerable<KeyValuePair<K, IReadOnlyTreeElement<K, V>>> IReadOnlyTreeElement<K, V>.NodeChildren
        {
            get
            {
                // contract is defined at IReadOnlyTreeElement

                return NodeChildren.Select(pair => new KeyValuePair<K, IReadOnlyTreeElement<K, V>>(pair.Key, pair.Value));
            }
        }

        private IObservable<NotifyCollectionChangedEventArgs> nodeChildrenChanged;
        public IObservable<NotifyCollectionChangedEventArgs> NodeChildrenChanged
        {
            get
            {
                // contract is defined at IReadOnlyTreeElement

                if (nodeChildrenChanged == null)
                {
                    INotifyCollectionChanged c = this.nodeChildren.KeyValuePairs;
                    nodeChildrenChanged
                        = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => h.Invoke,
                        h => c.CollectionChanged += h,
                        h => c.CollectionChanged -= h)
                        .Select(h => h.EventArgs);
                }
                return nodeChildrenChanged.AsObservable();
            }
        }

        private IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>> grandChildrenChanged;

        /// <summary>Pushes all changed grand children. When changed, returns its element's children all array elements or node children.</summary>
        public IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>> GrandChildrenChanged
        {
            get
            {
                // contract is defined at IReadOnlyTreeElement

                if (grandChildrenChanged == null)
                {
                    IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>> thisChildrenChanged;
                    IObservable<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>> childrenGrandChildrenChanged;

                    if (Type == ElementType.Array)
                    {
                        thisChildrenChanged = ArrayChanged.Select(c => 
                            new KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>(
                            new KeyArray<KeyOrIndex<K>>(),
                            c));

                        childrenGrandChildrenChanged = ArrayChanged
                            .Select(_ => Array.Indexes())
                            .SelectMany(pairs =>
                                pairs
                                    .Select(pair =>
                                        pair.Value
                                        .GrandChildrenChanged
                                        .Select(p =>
                                            new KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>(
                                            new KeyArray<KeyOrIndex<K>>(new[] { new KeyOrIndex<K>(pair.Key) }.Concat(p.Key)),
                                            p.Value))
                                      )
                            )
                            .Merge();

                        grandChildrenChanged = Observable.Merge(thisChildrenChanged, childrenGrandChildrenChanged);
                    }
                    else if (Type == ElementType.Node)
                    {
                        thisChildrenChanged = NodeChildrenChanged.Select(c => 
                            new KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>(
                            new KeyArray<KeyOrIndex<K>>(),
                            c));

                        childrenGrandChildrenChanged = NodeChildrenChanged
                            .Select(_ => NodeChildren)
                            .SelectMany(pairs =>
                                pairs
                                    .Select(pair =>
                                        pair.Value
                                        .GrandChildrenChanged
                                        .Select(p =>
                                            new KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>(
                                            new KeyArray<KeyOrIndex<K>>(new[] { new KeyOrIndex<K>(pair.Key) }.Concat(p.Key)),
                                            p.Value))
                                      )
                            )
                            .Merge();

                        grandChildrenChanged = Observable.Merge(thisChildrenChanged, childrenGrandChildrenChanged);
                    }
                    else
                    {
                        grandChildrenChanged = Observable.Empty<KeyValuePair<KeyArray<KeyOrIndex<K>>, NotifyCollectionChangedEventArgs>>();
                    }
                }

                return grandChildrenChanged;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((TreeElement<K, V>)obj);
        }

        public bool Equals(TreeElement<K, V> other)
        {
            if (other == null) return false;

            if (Type == ElementType.Leaf && other.Type == ElementType.Leaf)
            {
                return Object.Equals(LeafValue, other.LeafValue);
            }
            else
            {
                return Object.ReferenceEquals(this, other);
            }
        }

        public override int GetHashCode()
        {
            if (Type == ElementType.Leaf) return LeafValue == null ? 0 : LeafValue.GetHashCode();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            switch(Type)
            {
                case ElementType.Default:
                    return "Default";
                case ElementType.Array:
                    return "Array";
                case ElementType.Leaf:
                    return "Leaf: " + LeafValue == null ? "Null" : LeafValue.ToString();
                case ElementType.Node:
                    return "Node";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
