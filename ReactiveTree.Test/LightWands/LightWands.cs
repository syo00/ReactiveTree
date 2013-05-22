//-----------------------------------------------------------------------
// <copyright file="LightWands.cs">
//    Copyright (c) 2013, syo00.
//
//    Licensed under the MIT License (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.opensource.org/licenses/mit-license.php
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <website>https://github.com/syo00/LightWands</website>
//-----------------------------------------------------------------------


// VERSION: 0.1.3


/***** public or internal ******/
// NOTE: uncomment the following line to make LightWands class internal.
//#define USE_INTERNAL


/***** targeting projects ******/
// NOTE: select one number from (1), (2), or (3) by your project and uncomment its corresponding #define. You do not have to uncomment more than two #define lines in (1) to (3).

// (1) If you want to apply for below projects, uncomment below #define TESTS
//     * tests for .NET Framework 4.5
#define TESTS

// (2) Else if you want to apply for below projects (including portable class libraries), uncomment below #define NET45_WINRT45_WP8
//     * .NET Framework 4.5
//     * Windows store application     
//     * Windows Phone 8
//#define NET45_WINRT45_WP8

// (3) Else if you want to apply for below projects (including portable class libraries), uncomment below #define NET40_SL5_WINRT45_WP8
//     * .NET Framework 4.0
//     * Silverlight 5
//     * Windows store application
//     * Windows Phone 8
//#define NET40_SL5_WINRT45_WP8



#if TESTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

#if NET45_WINRT45_WP8 || TESTS
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;


#if TESTS
namespace Kirinji.LightWands.Tests
{

    #region IEnumerableExtensions

#if USE_INTERNAL
    internal
#else
    public
#endif
        static class IEnumerableExtensions
    {
        public static void IsSequenceEqual<T>(this IEnumerable<T> source, params T[] second)
        {
            source.IsSequenceEqual(second.AsEnumerable());
        }

        public static void IsSequenceEqual<T>(this IEnumerable<T> source, IEnumerable<T> second, string message = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (second == null) throw new ArgumentNullException("second");

            var actual = source.SequenceEqual(second);
            if (message == null)
            {
                Assert.AreEqual(true, actual);
            }
            else
            {
                Assert.AreEqual(true, actual, message);
            }
        }

        public static void IsNonSequenceEqual<T>(this IEnumerable<T> source, params T[] second)
        {
            source.IsNonSequenceEqual(second.AsEnumerable());
        }

        public static void IsNonSequenceEqual<T>(this IEnumerable<T> source, IEnumerable<T> second, string message = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (second == null) throw new ArgumentNullException("second");

            var actual = source.NonSequenceEqual(second);
            if (message == null)
            {
                Assert.AreEqual(true, actual);
            }
            else
            {
                Assert.AreEqual(true, actual, message);
            }
        }
    }

    #endregion


    #region IObservableExtensions

#if USE_INTERNAL
    internal
#else
    public
#endif
        static class IObservableExtensions
    {
        /// <summary>Starts subscribing and cache pushed values.</summary>
        public static History<T> SubscribeHistory<T>(this IObservable<T> source)
        {
            return new History<T>(source);
        }
    }

    /// <summary>Indicates pushed values.</summary>
#if USE_INTERNAL
    internal
#else
    public
#endif
        class History<T> : IEnumerable<Notification<T>>
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
        public IReadOnlyList<T> Values
        {
            get
            {
                return this.notifications
                    .Where(n => n.Kind == NotificationKind.OnNext)
                    .Select(n => n.Value)
                    .ToList();
            }
        }

        /// <summary>Gets exceptions history.</summary>
        public IReadOnlyList<Exception> Exceptions
        {
            get
            {
                return this.notifications
                    .Where(n => n.Kind == NotificationKind.OnError)
                    .Select(n => n.Exception)
                    .ToList();
            }
        }

        /// <summary>Gets all notifications.</summary>
        public IReadOnlyList<Notification<T>> Notifications
        {
            get
            {
                return notifications.ToList();
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

        [Obsolete]
        public IEnumerator<Notification<T>> GetEnumerator()
        {
            return this.notifications.GetEnumerator();
        }

        [Obsolete]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    #endregion


    #region PrivateObjectExtensions

#if USE_INTERNAL
    internal
#else
    public
#endif
        static class PrivateObjectExtensions
    {
        public static object Invoke<T>(this PrivateObject source, string name, T param)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(name != null);

            return source.Invoke(name, new Type[] { typeof(T) }, new object[] { param });
        }

        public static object Invoke<T1, T2>(this PrivateObject source, string name, T1 param1, T2 param2)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(name != null);

            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2) }, new object[] { param1, param2 });
        }

        public static object Invoke<T1, T2, T3>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(name != null);

            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, new object[] { param1, param2, param3 });
        }

        public static object Invoke<T1, T2, T3, T4>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(name != null);

            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new object[] { param1, param2, param3, param4 });
        }

        public static object Invoke<T1, T2, T3, T4, T5>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(name != null);

            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, new object[] { param1, param2, param3, param4, param5 });
        }

        public static object Invoke<T1, T2, T3, T4, T5, T6>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(name != null);

            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, new object[] { param1, param2, param3, param4, param5, param6 });
        }
    }

    #endregion

}
#endif