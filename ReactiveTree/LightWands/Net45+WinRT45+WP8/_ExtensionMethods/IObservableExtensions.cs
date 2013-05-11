using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kirinji.LightWands
{
    public static class IObservableExtensions
    {
        public static T MostRecentValue<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            return source.MostRecent(default(T)).First();
        }

        public static T MostRecentValue<T>(this IObservable<T> source, T missingValue)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            return source.MostRecent(missingValue).First();
        }

        /// <summary>Invokes actions when subscriptions count changes 0 to 1 or 1 to 0.</summary>
        public static IObservable<T> OnSubscriptionChanged<T>(this IObservable<T> source, Action onStarted, Action onPaused)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(onStarted != null);
            Contract.Requires<ArgumentNullException>(onPaused != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var refCount = 0;

            return Observable.Create<T>(obs =>
            {
                if (refCount == 0) onStarted();
                refCount++;

                return source
                    .Finally(() =>
                    {
                        refCount--;
                        if (refCount == 0) onPaused();
                    })
                    .Subscribe(obs);
            });
        }

        /// <summary>Passes values when subscribed.</summary>
        public static IObservable<T> WhenSubscribed<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var refCount = 0;

            return Observable.Create<T>(obs =>
            {
                refCount++;

                return source
                    .Finally(() => refCount--)
                    .Where(_ => refCount >= 1)
                    .Subscribe(obs);
            });
        }

        public class SelectorResult<T>
        {
            internal static SelectorResult<T> OnNext(T value, IObservable<T> source, int sourceIndex)
            {
                Contract.Requires<ArgumentNullException>(source != null);
                Contract.Ensures(Contract.Result<SelectorResult<T>>() != null);

                var r = new SelectorResult<T>();
                r.Kind = NotificationKind.OnNext;
                r.Value = value;
                r.Source = source;
                r.SourceIndex = sourceIndex;
                return r;
            }

            internal static SelectorResult<T> OnError(Exception exception, IObservable<T> source, int sourceIndex)
            {
                Contract.Requires<ArgumentNullException>(source != null);
                Contract.Ensures(Contract.Result<SelectorResult<T>>() != null);

                var r = new SelectorResult<T>();
                r.Kind = NotificationKind.OnError;
                r.Exception = exception;
                r.Source = source;
                r.SourceIndex = sourceIndex;
                return r;
            }

            internal static SelectorResult<T> OnCompleted(IObservable<T> source, int sourceIndex)
            {
                Contract.Requires<ArgumentNullException>(source != null);
                Contract.Ensures(Contract.Result<SelectorResult<T>>() != null);

                var r = new SelectorResult<T>();
                r.Kind = NotificationKind.OnCompleted;
                r.Source = source;
                r.SourceIndex = sourceIndex;
                return r;
            }

            public T Value { get; private set; }
            public Exception Exception { get; private set; }
            public NotificationKind Kind { get; private set; }
            public IObservable<T> Source { get; private set; }
            public int SourceIndex { get; private set; }
        }

        /// <summary>Switches multiple sequences.</summary>
        /// <remarks>sources should implements IList&gt;T&lt;.</remarks>
        public static IObservable<SelectorResult<T>> Selector<T>(this IObservable<IEnumerable<int>> selector, IEnumerable<IObservable<T>> sources)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Ensures(Contract.Result<IObservable<SelectorResult<T>>>() != null);
            
            CompositeDisposable disposable = new CompositeDisposable();

            return Observable.Create<SelectorResult<T>>(observer =>
                {
                    return selector.Subscribe(ary =>
                        {
                            disposable.Dispose();
                            disposable = new CompositeDisposable();
                            if (ary == null)
                            {
                                observer.OnError(new NullReferenceException());
                                return;
                            }

                            foreach (var i in ary)
                            {
                                var selected = sources.ElementAtOrDefault(i);
                                if (selected == null) return;
                                disposable.Add(selected.Subscribe(
                                    x => observer.OnNext(SelectorResult<T>.OnNext(x, selected, i)),
                                    ex => observer.OnNext(SelectorResult<T>.OnError(ex, selected, i)),
                                    () => observer.OnNext(SelectorResult<T>.OnCompleted(selected, i))
                                    ));
                            }
                        },
                    observer.OnError,
                    observer.OnCompleted);
                });
        }

        public static IObservable<SelectorResult<T>> Selector<T>(this IObservable<IEnumerable<int>> selector, params IObservable<T>[] sources)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Ensures(Contract.Result<IObservable<SelectorResult<T>>>() != null);

            return selector.Selector(sources.AsEnumerable());
        }

        public static IObservable<SelectorResult<T>> Selector<T>(this IObservable<int> selector, IEnumerable<IObservable<T>> sources)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Ensures(Contract.Result<IObservable<SelectorResult<T>>>() != null);

            return selector.Select(i => new[] { i }).Selector(sources);
        }

        public static IObservable<SelectorResult<T>> Selector<T>(this IObservable<int> selector, params IObservable<T>[] sources)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Ensures(Contract.Result<IObservable<SelectorResult<T>>>() != null);

            return selector.Selector(sources.AsEnumerable());
        }

        public sealed class ValueOrError<TValue, TException> : IEquatable<ValueOrError<TValue, TException>> where TException : Exception
        {
            public ValueOrError(TValue value)
            {
                this.Value = value;
            }

            public ValueOrError(TException error)
            {
                Contract.Requires<ArgumentNullException>(error != null);

                this.IsError = true;
                this.Error = error;
            }

            public bool IsError { get; private set; }
            public TValue Value { get; private set; }
            public TException Error { get; private set; }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                var casted = obj as ValueOrError<TValue, TException>;
                return Equals(casted);
            }

            public override int GetHashCode()
            {
                if (this.IsError)
                {
                    return this.Error.GetHashCode();
                }
                else
                {
                    return this.Value == null ? 0 : this.Value.GetHashCode();
                }
            }

            public bool Equals(ValueOrError<TValue, TException> other)
            {
                if (other == null) return false;
                if (this.IsError && other.IsError)
                {
                    return Object.Equals(this.Error, other.Error);
                }
                else if (!this.IsError && !other.IsError)
                {
                    return Object.Equals(this.Value, other.Value);
                }
                else
                {
                    return false;
                }
            }
        }

        public static IObservable<ValueOrError<TValue, TException>> TakeError<TValue, TException>(this IObservable<TValue> source) where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<ValueOrError<TValue, TException>>>() != null);

            var f = Observable.Merge(Observable.Throw<ValueOrError<TValue, TException>>(new Exception()), Observable.Empty<ValueOrError<TValue, TException>>());

            return source
                .Select(v => new ValueOrError<TValue, TException>(v)) // Catch では射影できないので、まずここで通常の値を変換
                .Catch((TException ex) => Observable.Return(new ValueOrError<TValue, TException>(ex))); // そしてここでエラーを変換
        }

        public static IObservable<TValue> ExtractError<TValue, TException>(this IObservable<ValueOrError<TValue, TException>> source) where TException : Exception
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<TValue>>() != null);

            return Observable.Create<TValue>(observer =>
                {
                    var s = source
                        .Where(x => x != null)
                        .Subscribe(x =>
                            {
                                if (x.IsError)
                                {
                                    observer.OnError(x.Error);
                                }
                                else
                                {
                                    observer.OnNext(x.Value);
                                }
                            },
                            observer.OnError,
                            observer.OnCompleted);
                    return s;
                });
        }
    }
}
