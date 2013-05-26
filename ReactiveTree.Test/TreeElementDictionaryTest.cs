using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kirinji.ReactiveTree.TreeStructures;
using Kirinji.ReactiveTree.Merging;
using Kirinji.LightWands.Tests;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class TreeElementDictionaryTest : ReactiveTest
    {
        [TestMethod]
        public void MergeTest()
        {
            var dictionary = new TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject>(tree => tree.GetOrDefault(KeyOrIndex.Key("id")));

            string jsonText1 = @"{
    ""id"": 12345,
    ""text"": ""Good morning!"",
    ""status"": {
      ""code"": 0,
      ""msg"":""No action taken""
    }
}";
            string jsonText2 = @"{
    ""id"": 67890,
    ""text"": ""I caught a cold..."",
    ""status"": {
      ""code"": 0,
      ""msg"":""No action taken""
    }
}";
            string jsonText3 = @"{
    ""id"": 12345,
    ""text"": ""Good morning!"",
    ""status"": {
      ""code"": 0,
      ""msg"":""No action taken""
    }
}";

            string jsonText4 = @"{
    ""id"": 12345,
    ""text"": ""Good night..."",
    ""status"": {
      ""code"": 1,
      ""msg"":""Modified""
    }
}";
            var rootId = TreeElementConverter.RootId;
            var notifier12345_1 = dictionary.Merge(TreeElementConverter.ConvertJson(jsonText1).NodeChildren[rootId], null);
            var notifier12345TextHistory = notifier12345_1.ValueChanged(KeyOrIndex.Key("text")).SubscribeHistory();
            var notifier12345MsgHistory = notifier12345_1.ValueChanged(KeyOrIndex.Key("status"), KeyOrIndex.Key("msg")).SubscribeHistory();
            var notifier67890 = dictionary.Merge(TreeElementConverter.ConvertJson(jsonText2).NodeChildren[rootId], null);
            var notifier67890TextHistory = notifier67890.ValueChanged(KeyOrIndex.Key("text")).SubscribeHistory();
            var notifier12345_2 = dictionary.Merge(TreeElementConverter.ConvertJson(jsonText3).NodeChildren[rootId], null);
            var notifier12345_3 = dictionary.Merge(TreeElementConverter.ConvertJson(jsonText4).NodeChildren[rootId], null);

            notifier12345TextHistory.Values.Select(x => x.Key.Single().NodeKey).Is("text", "text");
            notifier12345TextHistory.Values.Select(x => x.Value.LeafValue.CastOrDefault<string>()).Is("Good morning!", "Good night...");
            notifier12345MsgHistory.Values.Select(x => x.Value.LeafValue.CastOrDefault<string>()).Is("No action taken", "Modified");
            notifier67890TextHistory.Values.Count.Is(0);
            notifier12345_1.IsSameReferenceAs(notifier12345_2);
            notifier12345_1.IsSameReferenceAs(notifier12345_3);

            dictionary.GetAllTreeElement().Count.Is(2);
        }
    }
}
