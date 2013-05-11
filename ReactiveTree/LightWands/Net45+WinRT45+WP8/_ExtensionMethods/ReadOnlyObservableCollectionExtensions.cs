using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public static class ReadOnlyObservableCollectionExtensions
    {
        /// <summary>一対多の射影により新たな ReadOnlyObservableCollection を作成します。</summary>
        public static ReadOnlyObservableCollection<TResult> SelectNew<Tfrom, TResult>(this ReadOnlyObservableCollection<Tfrom> source, Func<Tfrom, TResult> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            return INotifyCollectionChangedExtensions.SelectNew<Tfrom, TResult>(source, selector).ToReadOnly();
        }
    }
}
