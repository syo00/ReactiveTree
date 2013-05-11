using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    static class INotifyCollectionChangedExtensions
    {
        // SelectNew と SelectManyNew を public にしないのは、ReadOnlyObservableCollection も ObservableCollection にしてしまうから。そのため、ラッパークラスを別のクラスに作成している

        /// <summary>
        /// INotifyCollectionChanged から ObservableCollection を一対一の射影により作成します。IEnumerable もあわせて継承しているクラスの場合、ObservableCollection にもそれらの要素を追加します。
        /// </summary>
        public static ObservableCollection<TResult> SelectNew<Tfrom, TResult>(this INotifyCollectionChanged source, Func<Tfrom, TResult> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            ObservableCollection<TResult> newOc;
            var ieSource = source as IEnumerable<Tfrom>;
            if (ieSource != null)
            {
                newOc = new ObservableCollection<TResult>(ieSource.Select(selector));
            }
            else
            {
                var notGenericIeSource = source as System.Collections.IEnumerable;
                if (notGenericIeSource != null)
                {
                    newOc = new ObservableCollection<TResult>(notGenericIeSource.Cast<Tfrom>().Select(selector));
                }
                else
                {
                    newOc = new ObservableCollection<TResult>();
                }
            }
            source.CollectionChanged += (_, e) => ApplyNotifyCollectionChangedEventArgsToObservableCollection<TResult, Tfrom>(newOc, e, selector);
            return newOc;
        }

        /// <summary>
        /// NotifyCollectionChangedEventArgs の内容を ObservableCollection に反映します。
        /// </summary>
        /// <param name="changeCollection">このコレクションの要素が変更されます。</param>
        private static void ApplyNotifyCollectionChangedEventArgsToObservableCollection<Tsource, TcollectionChanged>(ObservableCollection<Tsource> changeCollection, NotifyCollectionChangedEventArgs e, Func<TcollectionChanged, Tsource> converter)
        {
            Contract.Requires<ArgumentNullException>(e != null);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<TcollectionChanged>().ForEach(i => changeCollection.Add(converter(i)));
                    return;
                case NotifyCollectionChangedAction.Move:
                    changeCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
                    return;
                case NotifyCollectionChangedAction.Remove:
                    changeCollection.RemoveAt(e.OldStartingIndex);
                    return;
                case NotifyCollectionChangedAction.Replace:
                    changeCollection[e.NewStartingIndex] = converter((TcollectionChanged)e.NewItems[0]);
                    return;
                case NotifyCollectionChangedAction.Reset:
                    changeCollection.Clear();
                    return;
            }
        }
    }
}
