using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Diagnostics.Contracts;

namespace Kirinji.LightWands
{
    public class CashedReplaySubject<T> : ISubject<T>
    {
        IList<Tuple<ItemType, T, Exception>> m_cache;
        IScheduler m_scheduler;
        readonly Subject<T> m_source = new Subject<T>();
        bool m_isCompleted;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.m_cache != null);
            Contract.Invariant(this.m_source != null);
        }

        public CashedReplaySubject(IScheduler scheduler = null)
        {
            this.m_cache = new List<Tuple<ItemType, T, Exception>>();
            this.m_scheduler = scheduler;
        }

        public CashedReplaySubject(int bufferSize, IScheduler scheduler = null)
        {
            this.m_cache = new List<Tuple<ItemType, T, Exception>>(bufferSize);
            this.m_scheduler = scheduler;
        }

        public void OnCompleted()
        {
            if (!this.m_isCompleted)
            {
                this.m_isCompleted = true;
                this.m_source.OnCompleted();
                this.m_cache = null;
            }
        }

        public void OnError(Exception error)
        {
            if (!this.m_isCompleted)
            {
                this.m_source.OnError(error);
                this.m_cache.Add(new Tuple<ItemType, T, Exception>(ItemType.OnErrorValue, default(T), error));
            }
        }

        public void OnNext(T value)
        {
            if (!this.m_isCompleted)
            {
                this.m_source.OnNext(value);
                this.m_cache.Add(new Tuple<ItemType, T, Exception>(ItemType.OnNextValue, value, null));
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Observable.Merge(this.ReplayCache(), this.m_source).Subscribe(observer);
        }

        public IObservable<T> ReplayCache()
        {
            ReplaySubject<T> returnSubject;
            if (this.m_scheduler != null)
            {
                returnSubject = new System.Reactive.Subjects.ReplaySubject<T>(this.m_scheduler);
            }
            else
            {
                returnSubject = new System.Reactive.Subjects.ReplaySubject<T>();
            }
            foreach (var t in this.m_cache)
            {
                switch(t.Item1)
                {
                    case ItemType.OnNextValue:
                        returnSubject.OnNext(t.Item2);
                        break;
                    case ItemType.OnErrorValue:
                        returnSubject.OnError(t.Item3);
                        break;
                }
            }
            returnSubject.OnCompleted();
            return returnSubject.AsObservable();
        }

        enum ItemType
        {
            OnNextValue = 0,
            OnErrorValue = 1,
        }
    }
}
