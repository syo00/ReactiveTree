using Kirinji.ReactiveTree.Merging;
using Kirinji.ReactiveTree.TreeStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.LightWands.Tests;
using System.Reactive.Linq;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class ReactiveDictionaryTest
    {
        [TestMethod]
        public void Test()
        {
            var json1 =
@"{
    ""a"": 123, 
    ""name"": ""Yamada""
}
";
            var json2 =
@"{
    ""a"": 123, 
    ""user"": {
        ""id"": 1
    }
}
";
            var json3 =
@"{
    ""name"": ""Mark"",
    ""user"": {
        ""id"": 2
    }
}
";

            var rootId = TreeElementConverter.RootId;
            var j = new TreeElementNotifier<string, IDataObject>(TreeElementConverter.ConvertJson("{ }").NodeChildren[rootId]);
            var dic = new ReactiveDirectory(j);

            dic.GetValue<string>(KeyOrIndex.Key("name")).IsNull();
            dic.GetValue<int?>(KeyOrIndex.Key("user"), KeyOrIndex.Key("id")).IsNull();
            var history = dic
                .ObserveChanges(x => new { 
                    Name = x.GetValue<string>(KeyOrIndex.Key("name")),
                    UserId = x.GetValue<int?>(KeyOrIndex.Key("user"), KeyOrIndex.Key("id"))
                })
                .SubscribeHistory();

            j.ModifyCurrentTreeStraight(tree => tree.Merge(TreeElementConverter.ConvertJson(json1).NodeChildren[rootId], (x, y) => false));
            j.ModifyCurrentTreeStraight(tree => tree.Merge(TreeElementConverter.ConvertJson(json2).NodeChildren[rootId], (x, y) => false));
            j.ModifyCurrentTreeStraight(tree => tree.Merge(TreeElementConverter.ConvertJson(json3).NodeChildren[rootId], (x, y) => false));

            history.Values[0].Name.Is("Yamada");
            history.Values[0].UserId.IsNull();
            history.Values[1].Name.Is("Yamada");
            history.Values[1].UserId.Is(1);
            history.Values[2].Name.Is("Mark");
            history.Values[2].UserId.Is(2);
            history.Values.Count.Is(3);
        }
    }
}
