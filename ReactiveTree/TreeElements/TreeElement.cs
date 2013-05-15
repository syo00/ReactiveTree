using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.TreeElements
{
    /// <summary>Indicates tree structure that supports arrays.</summary>
    public class TreeElement<TKey, TValue> : IEquatable<TreeElement<TKey, TValue>>
    {
        private readonly IDictionary<TKey, TreeElement<TKey, TValue>> nodeChildren;
        private readonly IDictionary<TKey, IList<TreeElement<TKey, TValue>>> arrayNodeChildren;
        private TValue leafValue;
        private ISubject<ChangedChildren<TKey, TValue>> childrenChanged;

        /// <summary>Creates a leaf instance.</summary>
        public TreeElement(TValue initValue)
        {
            this.Type = ElementType.Leaf;
            this.leafValue = initValue;
        }

        /// <summary>Creates a node instance.</summary>
        public TreeElement(params KeyValuePair<TKey, TreeElement<TKey, TValue>>[] nodes)
            : this(false, nodes)
        {
            Contract.Requires<ArgumentNullException>(nodes != null);
        }

        /// <summary>Creates a node instance.</summary>
        public TreeElement(IEnumerable<KeyValuePair<TKey, TreeElement<TKey, TValue>>> nodes)
            : this(false, nodes)
        {
            Contract.Requires<ArgumentNullException>(nodes != null);
        }

        /// <summary>Creates a node instance.</summary>
        public TreeElement(bool convertMultipleValuesToArray, IEnumerable<KeyValuePair<TKey, TreeElement<TKey, TValue>>> nodes)
        {
            Contract.Requires<ArgumentNullException>(nodes != null);
            Contract.Requires(nodes.All(p => p.Key != null), "Nodes contain null keys.");
            Contract.Requires(nodes.All(p => p.Value != null), "Nodes contain null values.");

            this.Type = ElementType.Node;
            var nodesGroup = nodes.GroupBy(p => p.Key, p => p.Value).ToArray();
            if (convertMultipleValuesToArray)
            {
                this.nodeChildren = nodesGroup.Where(g => g.Count() == 1).ToDictionary(g => g.Key, g => g.Single());
                this.arrayNodeChildren = nodesGroup.Where(g => g.Count() >= 2).ToDictionary(g => g.Key, g => (IList<TreeElement<TKey, TValue>>)g.ToList());
            }
            else
            {
                try
                {
                    this.nodeChildren = nodesGroup.ToDictionary(g => g.Key, g => g.Single());
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException();
                }
                this.arrayNodeChildren = new Dictionary<TKey, IList<TreeElement<TKey, TValue>>>();
            }
            this.childrenChanged = new Subject<ChangedChildren<TKey, TValue>>();
        }

        /// <summary>Creates a node instance.</summary>
        public TreeElement(bool convertMultipleValuesToArray, params KeyValuePair<TKey, TreeElement<TKey, TValue>>[] nodes)
            : this(convertMultipleValuesToArray, nodes.AsEnumerable())
        {
            
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Type != ElementType.Empty);
        }

        /// <summary>Tries to get one node by a key. When not contains the key or is array, returns null.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the element is not a node.</exception>
        public TreeElement<TKey, TValue> GetSingleChildOrDefault(TKey key)
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            return this.nodeChildren.ValueOrDefault(key);
        }

        /// <summary>Tries to get an array by a key. When not contains the key or is not array, returns null.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the element is not a node.</exception>
        public IEnumerable<TreeElement<TKey, TValue>> GetArrayChildOrDefault(TKey key)
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            var value = this.arrayNodeChildren.ValueOrDefault(key);
            return value == null ? null : value.ToArray();
        }

        private IList<TreeElement<TKey, TValue>> GetArrayChildReferenceOrDefault(TKey key)
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            var value = this.arrayNodeChildren.ValueOrDefault(key);
            return value == null ? null : value;
        }

        /// <summary>Gets all nodes by a key.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the element is not a node.</exception>
        public IEnumerable<Child<TKey, TValue>> GetAllChildren()
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
            Contract.Ensures(Contract.Result<IEnumerable<Child<TKey, TValue>>>() != null);
            
            var l = this.nodeChildren.Select(p => new Child<TKey, TValue>(p.Key, p.Value));
            var r = this.arrayNodeChildren.Select(p => new Child<TKey, TValue>(p.Key, p.Value.ToArray()));
            return l.Concat(r).ToArray();
        }

        /// <summary>Gets the value of the leaf.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the element is not a leaf.</exception>
        public TValue LeafValue
        {
            get
            {
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Leaf);

                return this.leafValue;
            }
        }
        
        /// <summary>Gets or sets a value to the node.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the element is not a node or is an array.</exception>
        /// <exception cref="System.KeyNotFoundException">Thrown when the node does not have the specified key.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when tries to set null.</exception>
        public TreeElement<TKey, TValue> this[TKey key]
        {
            get
            {
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
                Contract.Ensures(Contract.Result<TreeElement<TKey, TValue>>() != null);

                var child = this.nodeChildren.ValueOrDefault(key);
                if (child != null) return child;
                if (this.arrayNodeChildren.ContainsKey(key)) throw new KeyNotFoundException();
                throw new KeyNotFoundException();
            }
            set
            {
                Contract.Requires<ArgumentNullException>(key != null);
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
                if (this.arrayNodeChildren.Any(c => CompareKeys(c.Key, key))) throw new InvalidOperationException();

                var oldValue = this.nodeChildren.ValueOrDefault(key);
                nodeChildren[key] = value;
                this.childrenChanged.OnNext(new ChangedChildren<TKey, TValue>(key, oldValue, value));
            }
        }

        public void SetSingleChild(TKey key, TreeElement<TKey, TValue> newChild)
        {
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(newChild != null);

            IEnumerable<TreeElement<TKey, TValue>> oldArray = null;
            TreeElement<TKey, TValue> oldSingleChild = GetSingleChildOrDefault(key);
            if (oldSingleChild == null)
            {
                oldArray = GetArrayChildReferenceOrDefault(key);
            }
            
            this.arrayNodeChildren.Remove(key);
            this.nodeChildren[key] = newChild;

            if (oldArray != null)
            {
                this.childrenChanged.OnNext(new ChangedChildren<TKey, TValue>(key, oldArray, newChild));
            }
            else if (oldSingleChild != null)
            {
                this.childrenChanged.OnNext(new ChangedChildren<TKey, TValue>(key, oldSingleChild, newChild));
            }
            else
            {
                this.childrenChanged.OnNext(new ChangedChildren<TKey, TValue>(key, (TreeElement<TKey, TValue>)null, newChild));
            }
        }

        public void ModifyArrayChild(TKey key, Action<IList<TreeElement<TKey, TValue>>> arrayModifier)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(arrayModifier != null);
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            ModifyArrayChild(key, _ => null, arrayModifier);
        }

        public void ModifyArrayChild(TKey key, Func<TreeElement<TKey, TValue>, IEnumerable<TreeElement<TKey, TValue>>> arrayCreator)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(arrayCreator != null);
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            ModifyArrayChild(key, arrayCreator, _ => { });
        }

        /// <summary>
        /// Change or creats an array.
        /// </summary>
        /// <param name="arrayModifier">
        /// <para>How to change an array when already the array existed.</para>
        /// <para>First parameter is not null.</para>
        /// </param>
        /// <param name="arrayCreator">
        /// <para>How to set an array when no array existed.</para>
        /// <para>First parameter is null when no array existed.</para>
        /// <para>Return null to abort setting an array.</para>
        /// </param>
        public void ModifyArrayChild(TKey key, Func<TreeElement<TKey, TValue>, IEnumerable<TreeElement<TKey, TValue>>> arrayCreator, Action<IList<TreeElement<TKey, TValue>>> arrayModifier)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(arrayModifier != null);
            Contract.Requires<ArgumentNullException>(arrayCreator != null);
            Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);

            var oldArray = this.GetArrayChildReferenceOrDefault(key);
            if (oldArray == null)
            {
                var oldSingleChild = GetSingleChildOrDefault(key);
                var newValues = arrayCreator(oldSingleChild);
                if (newValues == null) return;
                this.nodeChildren.Remove(key);
                this.arrayNodeChildren[key] = newValues.ToList();
                this.childrenChanged.OnNext(new ChangedChildren<TKey, TValue>(key, oldSingleChild, newValues.ToArray()));
            }
            else
            {
                var oldValues = oldArray.ToArray();
                arrayModifier(oldArray);
                var newValues = oldArray.ToArray();
                this.nodeChildren.Remove(key);
                this.childrenChanged.OnNext(new ChangedChildren<TKey, TValue>(key, oldValues, newValues));
            }
        }

        /// <summary>Gets the element's type.</summary>
        public ElementType Type
        {
            get;
            private set;
        }

        public IObservable<ChangedChildren<TKey, TValue>> ChildrenChanged
        {
            get
            {
                Contract.Requires<InvalidOperationException>(this.Type == ElementType.Node);
                Contract.Ensures(Contract.Result<IObservable<ChangedChildren<TKey, TValue>>>() != null);

                return this.childrenChanged
                    .Where(cc =>
                        {
                            if (cc.AreOldValuesArray != cc.AreNewValuesArray) return true;
                            if (cc.OldValues == null)
                            {
                                if (cc.NewValues == null) return false;
                                else return true;
                            }
                            if (cc.NewValues == null) return true;
                            return !cc.OldValues.SequenceEqual(cc.NewValues);
                        });
            }
        }

        private IEqualityComparer<TKey> GetKeyEqualityComparer()
        {
            return EqualityComparer<TKey>.Default;
        }

        private bool CompareKeys(TKey x, TKey y)
        {
            return GetKeyEqualityComparer().Equals(x, y);
        }

        public bool Equals(TreeElement<TKey, TValue> other)
        {
            if (this.Type == ElementType.Leaf && other.Type == ElementType.Leaf)
            {
                return Object.Equals(this.LeafValue, other.LeafValue);
            }
            else
            {
                return Object.ReferenceEquals(this, other);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.Equals((TreeElement<TKey, TValue>)obj);
        }

        public override int GetHashCode()
        {
            if (this.Type == ElementType.Leaf)
            {
                return this.LeafValue == null ? 0 : this.LeafValue.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override string ToString()
        {
            if (this.Type == ElementType.Leaf) return "TreeElement (Leaf: " + (this.LeafValue == null ? "null" : this.LeafValue.ToString()) + ")";
            if (this.Type == ElementType.Node) return "TreeElement (Node)";
            return "TreeElement (Empty)";
        }
    }
}
