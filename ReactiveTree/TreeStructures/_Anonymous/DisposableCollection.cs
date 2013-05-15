using Kirinji.LightWands;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeStructures
{
    internal class DisposableCollection<T> : Disposable, ICollection<T>
    {
        private readonly ICollection<T> inner;

        public DisposableCollection(ICollection<T> collection)
        {
            Contract.Requires<ArgumentNullException>(collection != null);

            this.inner = collection;
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
            ThrowExceptionIfDisposed();

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
