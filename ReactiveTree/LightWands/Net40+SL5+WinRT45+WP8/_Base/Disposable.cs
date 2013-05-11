using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    public abstract class Disposable : IDisposable
    {
        protected bool IsDisposed
        {
            get;
            private set;
        }

        protected void ThrowExceptionIfDisposed()
        {
            lock (this)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().FullName + " has been already disposed.");
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static bool TryDisposeAndRelease<T>(ref T disposingValue) where T : class
        {
            if (disposingValue == null) return false;
            var d = disposingValue as IDisposable;
            disposingValue = null;
            if (d != null) d.Dispose();
            return true;
        }

        public static bool TryDispose<T>(T disposingValue)
        {
            if (disposingValue == null) return false;
            var d = disposingValue as IDisposable;
            if (d == null) return false;
            d.Dispose();
            return true;
        }

        private void Dispose(bool isDisposing)
        {
            lock (this)
            {
                if (IsDisposed) return;               
                if (isDisposing) OnDisposingUnManagedResources();
                OnDisposingManagedResources();
                IsDisposed = true;
            }
        }

        protected virtual void OnDisposingManagedResources()
        { }

        protected virtual void OnDisposingUnManagedResources()
        { }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
