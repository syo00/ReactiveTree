using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public struct ChildWithoutKey<TKey, TValue>
    {
        public ChildWithoutKey(TreeElement<TKey, TValue> singleChild)
            : this()
        {
            Contract.Requires<ArgumentNullException>(singleChild != null);
            this.Values = new[] { singleChild };
            this.IsArray = false;
        }

        public ChildWithoutKey(IEnumerable<TreeElement<TKey, TValue>> arrayChildren)
            : this()
        {
            Contract.Requires<ArgumentNullException>(arrayChildren != null);
            Contract.Requires(arrayChildren.All(v => v != null));
            this.Values = arrayChildren.ToArray();
            this.IsArray = true;
        }

        public IEnumerable<TreeElement<TKey, TValue>> Values { get; private set; }
        public bool IsArray { get; private set; }
    }
}
