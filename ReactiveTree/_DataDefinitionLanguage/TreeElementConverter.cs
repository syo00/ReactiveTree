using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using Kirinji.ReactiveTree.TreeElements;

namespace Kirinji.ReactiveTree
{
    /// <summary>Converter for JSON -&gt; TreeElement.</summary>
    public static class TreeElementConverter
    {
        public static readonly string RootId = "$!root";
        public static readonly string NestedId = "$!nested";

        /// <summary>Converts JSON to TreeElement.</summary>
        public static TreeElement<string, IDataObject> ConvertJson(string jsonText)
        {
            Contract.Requires<ArgumentNullException>(jsonText != null);

            object deserializedObject;
            var isDeserialized = SimpleJson.TryDeserializeObject(jsonText, out deserializedObject);
            if (!isDeserialized) return null;
            var deserializedObjectWithRootId = new Dictionary<string, object>();
            deserializedObjectWithRootId[RootId] = deserializedObject;
            return ConvertJsonCore(deserializedObjectWithRootId);
        }

        private static TreeElement<string, IDataObject> ConvertJsonCore(object json)
        {
            Contract.Ensures(Contract.Result<TreeElement<string, IDataObject>>() != null);

            var jObject = json as IDictionary<string, object>;
            if (jObject != null)
            {
                var nodes = jObject.SelectMany(p =>
                {
                    var jArrayInNode = p.Value as IList<object>;
                    if (jArrayInNode == null)
                    {
                        return new[] { 
                            new KeyValuePair<string, TreeElement<string, IDataObject>>(p.Key, ConvertJsonCore(p.Value))};
                    }
                    else
                    {
                        return jArrayInNode.Select(t =>
                            new KeyValuePair<string, TreeElement<string, IDataObject>>(p.Key, ConvertJsonCore(t)));
                    }
                });
                return new TreeElement<string, IDataObject>(true, nodes);
            }
            var jArray = json as IList<object>;
            if (jArray != null)
            {
                var d = new Dictionary<string, object>();
                d[NestedId] = json;
                return ConvertJsonCore(d);
            }

            if (json == null) return new TreeElement<string, IDataObject>((IDataObject)null);
            return new TreeElement<string, IDataObject>(new SimpleJsonDataObject(json));

        }
    }
}
