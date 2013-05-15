using Kirinji.LightWands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    internal class DisposableList<T> : Disposable, IList<T>
    {
        private readonly IList<T> inner;

        public DisposableList(IList<T> list)
        {
            Contract.Requires<ArgumentNullException>(list != null);

            this.inner = list;
        }

        public int IndexOf(T item)
        {
            ThrowExceptionIfDisposed();

            return inner.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ThrowExceptionIfDisposed();

            inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ThrowExceptionIfDisposed();

            inner.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                ThrowExceptionIfDisposed();

                return inner[index];
            }
            set
            {
                ThrowExceptionIfDisposed();

                inner[index] = value;
            }
        }

        public void Add(T item)
        {
            ThrowExceptionIfDisposed();

            inner.Add(item);
        }

        public void Clear()
        {
            ThrowExceptionIfDisposed();

            inner.Clear();
        }

        public bool Contains(T item)
        {
            ThrowExceptionIfDisposed();

            return inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
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

        public bool Remove(T item)
        {
            return inner.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
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
            // empty
        }
    }
}
