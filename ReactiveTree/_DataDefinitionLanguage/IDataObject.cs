using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree
{
    /// <summary>Indicates a <c>System.Object</c> container.</summary>
    public interface IDataObject
    {
        bool TryCast<T>(out T value);
    }

    public static class IDataObjectExtensions
    {
        public static T CastOrDefault<T>(this IDataObject source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            T value;
            if (source.TryCast(out value))
            {
                return value;
            }
            else
            {
                return default(T);
            }
        }

        public static T? CastOrNull<T>(this IDataObject source) where T : struct
        {
            Contract.Requires<ArgumentNullException>(source != null);

            T value;
            if (source.TryCast(out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}
