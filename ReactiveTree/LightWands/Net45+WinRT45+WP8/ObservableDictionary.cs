using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Collections.ObjectModel;

namespace Kirinji.LightWands
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly ObservableCollection<TKey> keys = new ObservableCollection<TKey>();
        private readonly ObservableCollection<TValue> values = new ObservableCollection<TValue>();
        private readonly ObservableCollection<KeyValuePair<TKey, TValue>> keyValuePairs = new ObservableCollection<KeyValuePair<TKey, TValue>>();
        private readonly ReadOnlyObservableCollection<TKey> readOnlyKeys;
        private readonly ReadOnlyObservableCollection<TValue> readOnlyValues;
        private readonly ReadOnlyObservableCollection<KeyValuePair<TKey, TValue>> readOnlyKeyValuePairs;

        public ObservableDictionary()
        {
            this.readOnlyKeys = this.keys.ToReadOnly();
            this.readOnlyValues = this.values.ToReadOnly();
            this.readOnlyKeyValuePairs = this.keyValuePairs.ToReadOnly();
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.keys != null);
            Contract.Invariant(this.values != null);
            Contract.Invariant(this.keyValuePairs != null);
            Contract.Invariant(this.readOnlyKeys != null);
            Contract.Invariant(this.readOnlyValues != null);
            Contract.Invariant(this.readOnlyKeyValuePairs != null);

            // 6つのコレクションの個数は常に全部等しい
            Contract.Invariant(this.keys.Count == this.values.Count && this.values.Count == this.keyValuePairs.Count);
            Contract.Invariant(this.readOnlyKeys.Count == this.readOnlyValues.Count && this.readOnlyValues.Count == this.readOnlyKeyValuePairs.Count);
            Contract.Invariant(this.keyValuePairs.Count == this.readOnlyKeyValuePairs.Count);
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key)) throw new ArgumentException();
            this.keys.Add(key);
            this.values.Add(value);
            this.keyValuePairs.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            return this.keys.Contains(key);
        }

        public ReadOnlyObservableCollection<TKey> Keys
        {
            get
            {
                Contract.Ensures(Contract.Result<ReadOnlyObservableCollection<TKey>>() != null);

                return this.readOnlyKeys;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return this.Keys; 
            }
        }

        public bool Remove(TKey key)
        {
            var index = this.keys.IndexOf(key);
            if (index == -1) return false;
            this.keys.RemoveAt(index);
            this.values.RemoveAt(index);
            this.keyValuePairs.RemoveAt(index);
            Contract.Assume(this.keyValuePairs.Count == this.readOnlyKeyValuePairs.Count);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var index = this.keys.IndexOf(key);
            if (index == -1)
            {
                value = default(TValue);
                return false;
            }
            value = this.values[index];
            return true;
        }

        public ReadOnlyObservableCollection<TValue> Values
        {
            get
            {
                Contract.Ensures(Contract.Result<ReadOnlyObservableCollection<TValue>>() != null);

                return this.readOnlyValues;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return this.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var index = this.keys.IndexOf(key);
                if (index == -1) throw new KeyNotFoundException();
                return this.values[index];
            }
            set
            {
                var index = this.keys.IndexOf(key);
                if (index == -1)
                {
                    Add(key, value);
                }
                this.values[index] = value;
                this.keyValuePairs[index] = new KeyValuePair<TKey, TValue>(this.keys[index], value);
            }
        }

        public ReadOnlyObservableCollection<KeyValuePair<TKey, TValue>> KeyValuePairs
        {
            get
            {
                Contract.Ensures(Contract.Result<ReadOnlyObservableCollection<KeyValuePair<TKey, TValue>>>() != null);

                return this.readOnlyKeyValuePairs;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.keys.Clear();
            this.values.Clear();
            this.keyValuePairs.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.keyValuePairs.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.keyValuePairs.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.keyValuePairs.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var index = this.keyValuePairs.IndexOf(item);
            if (index == -1) return false;
            this.keys.RemoveAt(index);
            this.values.RemoveAt(index);
            this.keyValuePairs.RemoveAt(index);
            Contract.Assume(this.keyValuePairs.Count == this.readOnlyKeyValuePairs.Count);
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.keyValuePairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}   
