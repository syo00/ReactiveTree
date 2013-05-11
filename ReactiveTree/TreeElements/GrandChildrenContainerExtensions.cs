using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public static class GrandChildrenContainerExtensions
    {
        public static T GetValueOrDefault<T>(this GrandChildrenContainer<string, IDataObject> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var singleLeafOrDefault = source.LeafValues.SingleOrDefault();
            return singleLeafOrDefault == null ? default(T) : singleLeafOrDefault.CastOrDefault<T>();
        }

        public static T? GetValueOrNull<T>(this GrandChildrenContainer<string, IDataObject> source)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var singleLeafOrDefault = source.LeafValues.SingleOrDefault();
            return singleLeafOrDefault == null ? null : singleLeafOrDefault.CastOrNull<T>();
        }
    }
}
