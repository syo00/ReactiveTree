using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace Kirinji.LightWands.Tests
{
    public static class IObservableExtensions
    {
        /// <summary>Starts subscribing and cache pushed values.</summary>
        public static History<T> SubscribeHistory<T>(this IObservable<T> source)
        {
            return new History<T>(source);
        }
    }

    /// <summary>Indicates pushed values.</summary>
    public class History<T> : IEnumerable<Notification<T>>
    {
        private readonly IList<Notification<T>> notifications = new List<Notification<T>>();

        /// <summary>Creates instance and starts subscribing.</summary>
        public History(IObservable<T> observable)
        {
            if (observable == null) throw new ArgumentNullException("IObservable<T> is null.");
            observable
                .Synchronize()
                .Subscribe(
                t => notifications.Add(Notification.CreateOnNext(t)),
                ex => notifications.Add(Notification.CreateOnError<T>(ex)), 
                () => notifications.Add(Notification.CreateOnCompleted<T>())
                );
        }

        /// <summary>Gets values history.</summary>
        public IEnumerable<T> Values
        {
            get
            {
                return this.notifications
                    .Where(n => n.Kind == NotificationKind.OnNext)
                    .Select(n => n.Value);
            }
        }

        /// <summary>Gets exceptions history.</summary>
        public IEnumerable<Exception> Exceptions
        {
            get
            {
                return this.notifications
                    .Where(n => n.Kind == NotificationKind.OnError)
                    .Select(n => n.Exception);
            }
        }

        /// <summary>Indicates called OnCompleted.</summary>
        public bool IsCompleted
        {
            get
            {
                return this.notifications
                    .Any(n => n.Kind == NotificationKind.OnCompleted);
            }
        }

        public void Clear()
        {
            this.notifications.Clear();
        }

        public IEnumerator<Notification<T>> GetEnumerator()
        {
            return this.notifications.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
