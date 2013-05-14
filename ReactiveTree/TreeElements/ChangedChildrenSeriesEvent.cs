using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands;

namespace Kirinji.ReactiveTree.TreeElements
{
    // ModifyTreeAsSeries により生じた変更をまとめる。
    public struct ChangedChildrenSeriesEvent<TKey, TValue> : IEnumerable<ChangedChildren<TKey, TValue>>
    {
        private readonly IEnumerable<ChangedChildren<TKey, TValue>> children;
        private readonly object id;

        public ChangedChildrenSeriesEvent(IEnumerable<ChangedChildren<TKey, TValue>> children)
            : this()
        {
            Contract.Requires<ArgumentNullException>(children != null);
            Contract.Requires<ArgumentException>(children.Any());
            Contract.Requires(children.Select(cc => cc.Id).Distinct().Count() == 1); // All Id must be same

            var id = children.First().Id;
            this.id = id;

            // 同じキーに対して複数回変更があった場合、初めの値と最後の値のみを取り出して 1 つの変更とみなす
            this.children = children
                .ToLookup(cc => cc.Key, TreeElement<TKey, TValue>.GetKeyEqualityComparer())
                .Select(lookup => new
                {
                    Id = id,
                    Key = lookup.Key,
                    OldestValues = lookup.First().OldValues,
                    AreOldestValuesArray = lookup.First().AreOldValuesArray,
                    LatestValues = lookup.Last().NewValues,
                    AreLatestValuesArray = lookup.Last().AreNewValuesArray,
                })
                .Select(a =>
                    {
                        if (a.AreOldestValuesArray)
                        {
                            if (a.AreLatestValuesArray)
                            {
                                return new ChangedChildren<TKey, TValue>(a.Id, a.Key, a.OldestValues, a.LatestValues);
                            }
                            else
                            {
                                return new ChangedChildren<TKey, TValue>(a.Id, a.Key, a.OldestValues, a.LatestValues == null ? null : a.LatestValues.Single());
                            }
                        }
                        else
                        {
                            if (a.AreLatestValuesArray)
                            {
                                return new ChangedChildren<TKey, TValue>(a.Id, a.Key, a.OldestValues == null ? null : a.OldestValues.Single(), a.LatestValues);
                            }
                            else
                            {
                                return new ChangedChildren<TKey, TValue>(a.Id, a.Key, a.OldestValues == null ? null : a.OldestValues.Single(), a.LatestValues == null ? null : a.LatestValues.Single());
                            }
                        }
                    })
                .ToArray();

        }

        public object Id
        {
            get
            {
                return id;
            }
        }

        public IEnumerator<ChangedChildren<TKey, TValue>> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
