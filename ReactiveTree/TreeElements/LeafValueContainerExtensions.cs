using Kirinji.ReactiveTree;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.TreeElements
{
    public static class LeafValueContainerExtensions
    {
        public static T GetValueOrDefault<T>(this LeafValueContainer<IDataObject> source)
        {
            if (!source.IsExist) return default(T);
            return source.Value.CastOrDefault<T>();
        }

        public static T? GetValueOrNull<T>(this LeafValueContainer<IDataObject> source)
            where T : struct
        {
            if (!source.IsExist) return null;
            return source.Value.CastOrNull<T>();
        }
    }
}
