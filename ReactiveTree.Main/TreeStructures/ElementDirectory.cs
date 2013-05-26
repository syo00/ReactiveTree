using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    public class ElementDirectory<TKey, TValue> : IEquatable<ElementDirectory<TKey, TValue>>
    {
        public ElementDirectory(IEnumerable<KeyOrIndex<TKey>> directory, TreeElement<TKey, TValue> value)
            : this(new KeyArray<KeyOrIndex<TKey>>(directory), value)
        {
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires<ArgumentNullException>(Contract.ForAll(directory, d => d != null));
        }

        public ElementDirectory(KeyArray<KeyOrIndex<TKey>> directory, TreeElement<TKey, TValue> value)
        {
            Contract.Requires<ArgumentNullException>(directory != null);
            Contract.Requires<ArgumentNullException>(Contract.ForAll(directory, d => d != null));

            this.directory = directory;
            this.Value = value;
        }

        KeyArray<KeyOrIndex<TKey>> directory;
        public KeyArray<KeyOrIndex<TKey>> Directory
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<KeyOrIndex<TKey>>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<KeyOrIndex<TKey>>>(), d => d != null));

                return directory;
            }
        }

        public TreeElement<TKey, TValue> Value
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((ElementDirectory<TKey, TValue>)obj);
        }

        public bool Equals(ElementDirectory<TKey, TValue> other)
        {
            if (other == null) return false;

            return Object.Equals(this.Directory, other.Directory)
                && Object.Equals(this.Value, other.Value);
        }

        public override int GetHashCode()
        {
            return this.Directory.GetHashCode()
                ^ (this.Value == null ? 0 : this.Value.GetHashCode());
        }
    }
}
