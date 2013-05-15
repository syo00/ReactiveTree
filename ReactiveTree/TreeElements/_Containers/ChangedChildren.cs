﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public struct ChangedChildren<TKey, TValue>
    {
        public ChangedChildren(TKey key, TreeElement<TKey, TValue> oldValue, TreeElement<TKey, TValue> newValue)
            : this(
            key, 
            oldValue == null ? null : new[] { oldValue },
            newValue == null ? null : new[] { newValue },
            false, false)
        {
            Contract.Requires<ArgumentNullException>(key != null);
        }

        public ChangedChildren(TKey key, IEnumerable<TreeElement<TKey, TValue>> oldValues, TreeElement<TKey, TValue> newValue)
            : this(
            key, 
            oldValues == null ? null : oldValues.ToArray(), 
            newValue == null ? null : new[] { newValue },
            true, false)
        {
            Contract.Requires<ArgumentNullException>(key != null);
        }

        public ChangedChildren(TKey key, TreeElement<TKey, TValue> oldValue, IEnumerable<TreeElement<TKey, TValue>> newValues)
            : this(key,
            oldValue == null ? null : new[] { oldValue },
            newValues == null ? null : newValues.ToArray(), 
            false, true)
        {
            Contract.Requires<ArgumentNullException>(key != null);
        }

        public ChangedChildren(TKey key, IEnumerable<TreeElement<TKey, TValue>> oldValues, IEnumerable<TreeElement<TKey, TValue>> newValues)
            : this(key,
            oldValues == null ? null : oldValues.ToArray(),
            newValues == null ? null : newValues.ToArray(), 
            true, true)
        {
            Contract.Requires<ArgumentNullException>(key != null);
        }

        private ChangedChildren(
            TKey key,
            IEnumerable<TreeElement<TKey, TValue>> oldValues,
            IEnumerable<TreeElement<TKey, TValue>> newValues,
            bool wasArray, 
            bool isArray)
            : this()
        {
            Contract.Requires<ArgumentNullException>(key != null);
            if (oldValues != null)
            {
                if (oldValues.Contains(null)) throw new ArgumentException("oldValues.Contains(null)");
                this.AreOldValuesArray = wasArray;
            }
            if (newValues != null)
            {
                if (newValues.Contains(null)) throw new ArgumentException("newValues.Contains(null)");
                this.AreNewValuesArray = isArray;
            }
            this.Key = key;
            this.OldValues = oldValues;
            this.NewValues = newValues;
        }

        public TKey Key { get; private set; }
        public IEnumerable<TreeElement<TKey, TValue>> OldValues { get; private set; }
        public IEnumerable<TreeElement<TKey, TValue>> NewValues { get; private set; }
        public bool AreOldValuesArray { get; private set; }
        public bool AreNewValuesArray { get; private set; }
    }
}