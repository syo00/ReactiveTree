using Kirinji.LightWands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    internal class DisposableDictionary<TKey, TValue> : Disposable, IDictionary<TKey, TValue>
    {
        readonly IDictionary<TKey, TValue> inner;

        public DisposableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            Contract.Requires<ArgumentNullException>(dictionary != null);

            this.inner = dictionary;
        }

        public void Add(TKey key, TValue value)
        {
            ThrowExceptionIfDisposed();

            inner.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            ThrowExceptionIfDisposed();

            return inner.ContainsKey(key);
        }

        DisposableCollection<TKey> keys;
        public ICollection<TKey> Keys
        {
            get
            {
                ThrowExceptionIfDisposed();

                if (keys == null)
                {
                    keys = new DisposableCollection<TKey>(inner.Keys);
                }

                return keys;
            }
        }

        public bool Remove(TKey key)
        {
            ThrowExceptionIfDisposed();

            return inner.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            ThrowExceptionIfDisposed();

            return inner.TryGetValue(key, out value);
        }

        DisposableCollection<TValue> values;
        public ICollection<TValue> Values
        {
            get
            {
                ThrowExceptionIfDisposed();

                if (values == null)
                {
                    values = new DisposableCollection<TValue>(inner.Values);
                }

                return values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                ThrowExceptionIfDisposed();

                return inner[key];
            }
            set
            {
                ThrowExceptionIfDisposed();

                inner[key] = value;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ThrowExceptionIfDisposed();

            inner.Add(item);
        }

        public void Clear()
        {
            ThrowExceptionIfDisposed();

            inner.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            ThrowExceptionIfDisposed();

            return inner.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ThrowExceptionIfDisposed();

            inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                ThrowExceptionIfDisposed();

                return inner.Count; 
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                ThrowExceptionIfDisposed();

                return inner.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowExceptionIfDisposed();

            return inner.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            ThrowExceptionIfDisposed();

            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            ThrowExceptionIfDisposed();

            return GetEnumerator();
        }

        protected override void OnDisposingManagedResources()
        {
            Disposable.TryDispose(keys);
            Disposable.TryDispose(values);
        }
    }
}
