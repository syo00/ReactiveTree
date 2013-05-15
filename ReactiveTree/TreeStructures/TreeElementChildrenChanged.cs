using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    public class TreeElementChildrenChanged<TKey, TValue>
    {
        public TreeElementChildrenChanged(TreeElement<TKey, TValue> source, NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(e != null);

            this.source = source;
            this.notifyCollectionChangedEventArgs = e;
        }

        NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs;
        public NotifyCollectionChangedEventArgs NotifyCollectionChangedEventArgs
        {
            get
            {
                Contract.Ensures(Contract.Result<NotifyCollectionChangedEventArgs>() != null);

                return notifyCollectionChangedEventArgs;
            }
        }

        TreeElement<TKey, TValue> source;
        public TreeElement<TKey, TValue> Source
        {
            get
            {
                Contract.Ensures(Contract.Result<TreeElement<TKey, TValue>>() != null);

                return source;
            }
        }
    }
}
