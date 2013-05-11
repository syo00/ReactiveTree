using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kirinji.LightWands.Tests
{
    public static class PrivateObjectExtensions
    {
        public static object Invoke<T>(this PrivateObject source, string name, T param)
        {
            return source.Invoke(name, new Type[] { typeof(T) }, new object[] { param });
        }

        public static object Invoke<T1, T2>(this PrivateObject source, string name, T1 param1, T2 param2)
        {
            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2) }, new object[] { param1, param2 });
        }

        public static object Invoke<T1, T2, T3>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3)
        {
            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, new object[] { param1, param2, param3});
        }

        public static object Invoke<T1, T2, T3, T4>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4)}, new object[] { param1, param2, param3, param4 });
        }

        public static object Invoke<T1, T2, T3, T4, T5>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, new object[] { param1, param2, param3, param4, param5 });
        }

        public static object Invoke<T1, T2, T3, T4, T5, T6>(this PrivateObject source, string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            return source.Invoke(name, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, new object[] { param1, param2, param3, param4, param5, param6 });
        }
    }
}
