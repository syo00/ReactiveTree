using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class TreeElementConverterTest
    {
        [TestMethod]
        public void ConvertJsonTest()
        {
            string jsonText = @"{
    ""original"": ""http://www.foo.com/"",
    ""short"": ""krehqk"",
    ""response"": null,
    ""error"": {
      ""code"":0,
      ""msg"":""No action taken""
    }
    ""location"": [
      [ 1.936498, 48.753778 ], 
      [ 1.936498, 48.80043 ], 
      [ 1.985875, 48.80043 ], 
      [ 1.985875, 48.753778 ]
    ]
}";
            var tree = TreeElementConverter.ConvertJson(jsonText);
            tree.NodeChildren[TreeElementConverter.RootId].NodeChildren["original"].LeafValue.CastOrDefault<string>().Is("http://www.foo.com/");
            tree.NodeChildren[TreeElementConverter.RootId].NodeChildren["response"].LeafValue.IsNull();
            tree.NodeChildren[TreeElementConverter.RootId]
                .NodeChildren["location"]
                .Array
                .First()
                .Array
                .First()
                .LeafValue
                .CastOrNull<double>()
                .Is(1.936498);
        }
    }
}
