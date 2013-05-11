using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kirinji.LightWands;

namespace Kirinji.LightWands.Tests
{
    public static class IEnumerableExtensions
    {
        public static void IsSequenceEqual<T>(this IEnumerable<T> source, params T[] second)
        {
            source.IsSequenceEqual(second.AsEnumerable());
        }

        public static void IsSequenceEqual<T>(this IEnumerable<T> source, IEnumerable<T> second)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (second == null) throw new ArgumentNullException("second");
            source.SequenceEqual(second).IsTrue();
        }

        public static void IsNonSequenceEqual<T>(this IEnumerable<T> source, params T[] second)
        {
            source.IsNonSequenceEqual(second.AsEnumerable());
        }

        public static void IsNonSequenceEqual<T>(this IEnumerable<T> source, IEnumerable<T> second)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (second == null) throw new ArgumentNullException("second");
            source.NonSequenceEqual(second).IsTrue();
        }
    }
}
