using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Kirinji.LightWands
{
    /// <summary>Creates <c>EqualityComparer&lt;T&gt;</c> by delegates.</summary>
    internal class AnonymousEqualityComparer<T> : EqualityComparer<T>
    {
        private Func<T, T, bool> comparerDelegate;
        private Func<T, int> getHashCodeDelegate;

        /// <remarks>Not recommended to use this constructor because GetHashCode always returns same value and it makes programs slow.</remarks>
        public AnonymousEqualityComparer(Func<T, T, bool> comparerDelegate) : this(comparerDelegate, _ => 1)
        {
            Contract.Requires<ArgumentNullException>(comparerDelegate != null);
        }

        public AnonymousEqualityComparer(Func<T, T, bool> comparerDelegate, Func<T, int> getHashCodeDelegate)
        {
            Contract.Requires<ArgumentNullException>(comparerDelegate != null);
            Contract.Requires<ArgumentNullException>(getHashCodeDelegate != null);

            this.comparerDelegate = comparerDelegate;
            this.getHashCodeDelegate = getHashCodeDelegate;
        }

        public override bool Equals(T x, T y)
        {
            return this.comparerDelegate(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return this.getHashCodeDelegate(obj);
        }
    }

    /// <summary>Supports creating <c>EqualityComparer&lt;T&gt;</c>.</summary>
    public static class EqualityComparer
    {
        /// <remarks>Not recommended to use this method because GetHashCode always returns same value and it makes programs slow.</remarks>
        public static EqualityComparer<T> Create<T>(Func<T, T, bool> comparer)
        {
            Contract.Requires<ArgumentNullException>(comparer != null);

            return new AnonymousEqualityComparer<T>(comparer);
        }

        public static EqualityComparer<T> Create<T>(Func<T, T, bool> comparer, Func<T, int> hashCodeCreator)
        {
            Contract.Requires<ArgumentNullException>(comparer != null);
            Contract.Requires<ArgumentNullException>(hashCodeCreator != null);

            return new AnonymousEqualityComparer<T>(comparer, hashCodeCreator);
        }

        /// <summary>Creates <c>EqualityComparer&lt;T&gt;</c> by specifying parameters or methods.</summary>
        public static EqualityComparer<T> Create<T>(IEnumerable<Func<T, object>> comparingParameters)
        {
            Contract.Requires<ArgumentNullException>(comparingParameters != null);

            Func<T, T, bool> comparer = (x, y) => comparingParameters.All(f => Object.Equals(f(x), f(y)));
            Func<T, int> hashCodeCreator = t => comparingParameters
                .Select(f => f(t))
                .Select(p => p == null ? 0 : p.GetHashCode())
                .Aggregate((l, r) => l ^ r);
            return new AnonymousEqualityComparer<T>(comparer, hashCodeCreator);
        }

        /// <summary>Creates <c>EqualityComparer&lt;T&gt;</c> by specifying parameters or methods.</summary>
        public static EqualityComparer<T> Create<T>(params Func<T, object>[] comparingParameters)
        {
            Contract.Requires<ArgumentNullException>(comparingParameters != null);

            return Create(comparingParameters.AsEnumerable());
        }

        /// <summary>Creates <c>EqualityComparer&lt;T&gt;</c> of using references of objects.</summary>
        /// <remarks>Be careful not using boxed objects.</remarks>
        public static EqualityComparer<T> ReferenceEquals<T>() where T : class
        {
            Func<T, T, bool> comparer = (x, y) => Object.ReferenceEquals(x, y);
            Func<T, int> hashCodeCreator = t => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(t);

            return new AnonymousEqualityComparer<T>(comparer, hashCodeCreator);
        }

        /// <summary>Creates <c>EqualityComparer&lt;IEnumerable&lt;T&gt;&gt;</c> to compare sequentially.</summary>
        public static EqualityComparer<IEnumerable<T>> EnumerableOf<T>()
        {
            return EnumerableOfInner<T>(false, Comparer<T>.Default);
        }

        /// <summary>Creates <c>EqualityComparer&lt;IEnumerable&lt;T&gt;&gt;</c> to compare the number of each values.</summary>
        public static EqualityComparer<IEnumerable<T>> EnumerableOfUnordered<T>()
        {
            return EnumerableOfInner<T>(true, Comparer<T>.Default);
        }

        /// <summary>Creates <c>EqualityComparer&lt;IEnumerable&lt;T&gt;&gt;</c> to compare the number of each values.</summary>
        public static EqualityComparer<IEnumerable<T>> EnumerableOfUnordered<T>(IComparer<T> comparer)
        {
            Contract.Requires<ArgumentNullException>(comparer != null);

            return EnumerableOfInner<T>(true, comparer);
        }

        // ignoreOrder = true のとき、順序がバラバラでも要素の個数が合っていれば Equal となる
        private static EqualityComparer<IEnumerable<T>> EnumerableOfInner<T>(bool ignoreOrder, IComparer<T> orderingComparer)
        {
            Contract.Requires<ArgumentNullException>(orderingComparer != null);

            Func<IEnumerable<T>, int> hashCodeCreator = e => 
                e.Count() == 0
                ? 1
                : e
                    .Select(p => p == null ? 0 : p.GetHashCode())
                    .Aggregate((l, r) => l ^ r);
            if (ignoreOrder)
            {
                return EqualityComparer.Create<IEnumerable<T>>(
                       (e1, e2) =>
                       {
                           if (e1 == null || e2 == null) return e1 == e2;
                           return e1
                               .OrderBy(p => p, orderingComparer)
                               .SequenceEqual(e2.OrderBy(p => p, orderingComparer));
                       },
                       hashCodeCreator);
            }
            else
            {
                return EqualityComparer.Create<IEnumerable<T>>(
                    (e1, e2) =>
                    {
                        if (e1 == null || e2 == null) return e1 == e2;
                        return e1.SequenceEqual(e2);
                    },
                    hashCodeCreator);
            }
        }
    }
}
