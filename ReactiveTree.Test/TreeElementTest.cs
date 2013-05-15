using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Kirinji.LightWands;
using Kirinji.ReactiveTree.TreeElements;
using Kirinji.ReactiveTree;
using Kirinji.LightWands.Tests;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class TreeElementTest : ReactiveTest
    {
        [TestMethod]
        public void LeafTest()
        {
            var leaf = new TreeElement<int, string>("1");

            leaf.Type.Is(ElementType.Leaf);
            AssertEx.Catch<InvalidOperationException>(() => leaf.ChildrenChanged.Subscribe());
            AssertEx.Catch<InvalidOperationException>(() => leaf.GetSingleChildOrDefault(0));
            AssertEx.Catch<InvalidOperationException>(() => leaf.GetAllChildren());
            AssertEx.Catch<InvalidOperationException>(() => leaf.GetArrayChildOrDefault(0));
            AssertEx.Catch<InvalidOperationException>(() => leaf.ModifyArrayChild(0, ary => ary.Add(new TreeElement<int, string>())));
            AssertEx.Catch<InvalidOperationException>(() => leaf.ModifyArrayChild(0, _ => new[] { new TreeElement<int, string>() }));
            AssertEx.Catch<InvalidOperationException>(() => leaf.SetSingleChild(1, new TreeElement<int, string>("1")));
            AssertEx.Catch<InvalidOperationException>(() => { var _ = leaf[0]; });
            AssertEx.Catch<InvalidOperationException>(() => { leaf[0] = new TreeElement<int,string>(); });
            leaf.LeafValue.Is("1");
            leaf.Type.Is(ElementType.Leaf);
        }

        [TestMethod]
        public void NodeReadAndCreateTest()
        {
            AssertEx.Catch<ArgumentNullException>(() =>
                new TreeElement<int, string>((KeyValuePair<int, TreeElement<int, string>>[])null));
            AssertEx.Catch<ArgumentNullException>(() =>
                new TreeElement<int, string>((IEnumerable<KeyValuePair<int, TreeElement<int, string>>>)null));
            AssertEx.Catch<Exception>(() =>
                new TreeElement<int, string>(new KeyValuePair<int, TreeElement<int, string>>(1, null))); // ContractException

            var elementsWithSameKey = new[]{
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("0")),
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("Zero")),
                new KeyValuePair<int, TreeElement<int, string>>(1, new TreeElement<int, string>("1"))
            };
            AssertEx.Catch<ArgumentException>(() =>
                new TreeElement<int, string>(elementsWithSameKey));
            AssertEx.Catch<ArgumentException>(() =>
                new TreeElement<int, string>(false, elementsWithSameKey));

            var node = new TreeElement<int, string>(true, elementsWithSameKey);
            var history = node.ChildrenChanged.SubscribeHistory();

            node.Type.Is(ElementType.Node);

            AssertEx.Catch<InvalidOperationException>(() => node.LeafValue.ToString());

            node.GetSingleChildOrDefault(0).IsNull();
            AssertEx.Catch<KeyNotFoundException>(() => node[0].ToString());
            node.GetSingleChildOrDefault(1).LeafValue.Is("1");
            node[1].LeafValue.Is("1");
            node.GetSingleChildOrDefault(2).IsNull();
            AssertEx.Catch<KeyNotFoundException>(() => node[2].ToString());

            node.GetArrayChildOrDefault(0).Select(c => c.LeafValue).SequenceEqual(new[] { "0", "Zero" }).IsTrue();
            node.GetArrayChildOrDefault(1).IsNull();
            node.GetArrayChildOrDefault(2).IsNull();

            node.GetAllChildren().Count().Is(2);
            var child0 = node.GetAllChildren().Where(c => c.Key == 0).Single();
            child0.IsArray.IsTrue();
            child0.Values.Select(c => c.LeafValue).SequenceEqual(new[] { "0", "Zero" }).IsTrue();
            var child1 = node.GetAllChildren().Where(c => c.Key == 1).Single();
            child1.IsArray.IsFalse();
            child1.Values.Single().LeafValue.Is("1");

            node.Type.Is(ElementType.Node);

            history.Count().Is(0);
        }

        [TestMethod]
        public void NodeReadExtensionsTest()
        {
            var elementsWithSameKey = new[]{
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("0")),
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("Zero")),
                new KeyValuePair<int, TreeElement<int, string>>(1, new TreeElement<int, string>("1"))
            };
            var node = new TreeElement<int, string>(true, elementsWithSameKey);

            var history = node.ChildrenChanged.SubscribeHistory();

            var child0 = node.GetChild(0);
            child0.Value.Key.Is(0);
            child0.Value.IsArray.IsTrue();
            child0.Value.Values.Select(e => e.LeafValue).SequenceEqual(new[] { "0", "Zero" }).IsTrue();
            var child1 = node.GetChild(1);
            child1.Value.Key.Is(1);
            child1.Value.Values.Single().LeafValue.Is("1");
            node.GetChild(2).IsNull();

            node.GetSingleChildOrDefault(new int[] { }).Is(node);
            node.GetSingleChildOrDefault(new[] { 0 }).IsNull();
            node.GetSingleChildOrDefault(null, new[] { 0 }).IsNull();
            node.GetSingleChildOrDefault((ary, key) => ary.First(), new[] { 0 }).LeafValue.Is("0");
            node.GetSingleChildOrDefault(new[] { 1 }).LeafValue.Is("1");
            node.GetSingleChildOrDefault(null, new[] { 1 }).LeafValue.Is("1");
            node.GetSingleChildOrDefault((ary, key) => ary.First(), new[] { 1 }).LeafValue.Is("1");
            node.GetSingleChildOrDefault(new[] { 2 }).IsNull();
            node.GetSingleChildOrDefault(null, new[] { 2 }).IsNull();
            node.GetSingleChildOrDefault((ary, key) => ary.First(), new[] { 2 }).IsNull();

            node.GetSingleValue().IsExist.IsFalse();
            new TreeElement<int, string>("root_value").GetSingleValue().Value.Is("root_value");
            node.GetSingleValue(new[] { 0 }).IsExist.IsFalse();
            node.GetSingleValue(null, new[] { 0 }).IsExist.IsFalse();
            node.GetSingleValue((ary, key) => ary.First(), new[] { 0 }).Value.Is("0");
            node.GetSingleValue(new[] { 1 }).Value.Is("1");
            node.GetSingleValue(null, new[] { 1 }).Value.Is("1");
            node.GetSingleValue((ary, key) => ary.First(), new[] { 1 }).Value.Is("1");
            node.GetSingleValue(new[] { 2 }).IsExist.IsFalse();
            node.GetSingleValue(null, new[] { 2 }).IsExist.IsFalse();
            node.GetSingleValue((ary, key) => ary.First(), new[] { 2 }).IsExist.IsFalse();

            history.Count().Is(0);
            node.Type.Is(ElementType.Node);
        }

        [TestMethod]
        public void NodeSetTest()
        {
            var elementsWithSameKey = new[]{
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("0")),
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("Zero")),
                new KeyValuePair<int, TreeElement<int, string>>(1, new TreeElement<int, string>("1"))
            };
            var node = new TreeElement<int, string>(true, elementsWithSameKey);

            var history = node.ChildrenChanged.SubscribeHistory();

            AssertEx.Catch<InvalidOperationException>(() => node[0] = new TreeElement<int, string>("Zero"));
            node[1] = new TreeElement<int, string>("One");
            node[2] = new TreeElement<int, string>("Two");

            node[1].LeafValue.Is("One");
            node[2].LeafValue.Is("Two");
            node.GetAllChildren().Count().Is(3);
            history.Count().Is(2);
            var node1History = history.Values.ElementAt(0);
            node1History.Key.Is(1);
            node1History.AreOldValuesArray.IsFalse();
            node1History.AreNewValuesArray.IsFalse();
            node1History.OldValues.Single().LeafValue.Is("1");
            node1History.NewValues.Single().LeafValue.Is("One");
            var node2History = history.Values.ElementAt(1);
            node2History.Key.Is(2);
            node2History.AreOldValuesArray.IsFalse();
            node2History.AreNewValuesArray.IsFalse();
            node2History.OldValues.IsNull();
            node2History.NewValues.Single().LeafValue.Is("Two");
        }

        [TestMethod]
        public void ModifyArrayChildTest()
        {
            var elementsWithSameKey = new[]{
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int, string>("0")),
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int, string>("Zero")),
                new KeyValuePair<int, TreeElement<int, string>>(1, new TreeElement<int, string>("1"))
            };

            var node = new TreeElement<int, string>(true, elementsWithSameKey);

            var history = node.ChildrenChanged.SubscribeHistory();

            node.ModifyArrayChild(0, _ => { throw new Exception(); }, ary => ary.Add(new TreeElement<int, string>("Empty")));
            node.ModifyArrayChild(1, _ => null, ary => { throw new Exception(); });
            node.ModifyArrayChild(2, _ => new[] { new TreeElement<int, string>("2"), new TreeElement<int, string>("Two") });

            node.GetArrayChildOrDefault(0).Select(ac => ac.LeafValue).SequenceEqual(new[] { "0", "Zero", "Empty" }).IsTrue();
            node.GetArrayChildOrDefault(2).Select(ac => ac.LeafValue).SequenceEqual(new[] { "2", "Two" }).IsTrue();

            history.Count().Is(2);
            var history0 = history.Values.ElementAt(0);
            history0.AreOldValuesArray.IsTrue();
            history0.AreNewValuesArray.IsTrue();
            history0.OldValues.Select(ac => ac.LeafValue).SequenceEqual(new[] { "0", "Zero" }).IsTrue();
            history0.NewValues.Select(ac => ac.LeafValue).SequenceEqual(new[] { "0", "Zero", "Empty" }).IsTrue();
            var history2 = history.Values.ElementAt(1);
            history2.AreOldValuesArray.IsFalse();
            history2.AreNewValuesArray.IsTrue();
            history2.OldValues.IsNull();
            history2.NewValues.Select(ac => ac.LeafValue).SequenceEqual(new[] { "2", "Two" }).IsTrue();
        }

        [TestMethod]
        public void ReplaceToSingleChildTest()
        {
            var elementsWithSameKey = new[]{
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("0")),
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("Zero")),
                new KeyValuePair<int, TreeElement<int, string>>(1, new TreeElement<int, string>("1"))
            };

            var node = new TreeElement<int, string>(true, elementsWithSameKey);

            var history = node.ChildrenChanged.SubscribeHistory();
            node.SetSingleChild(0, new TreeElement<int, string>("0"));
            node.SetSingleChild(1, new TreeElement<int, string>("1"));
            node.SetSingleChild(2, new TreeElement<int, string>("2"));
            var history0 = history.Values.ElementAt(0);
            history0.AreOldValuesArray.IsTrue();
            history0.AreNewValuesArray.IsFalse();
            history0.OldValues.Select(ac => ac.LeafValue).SequenceEqual(new[] { "0", "Zero" }).IsTrue();
            history0.NewValues.Single().LeafValue.Is("0");
            var history1 = history.Values.ElementAt(1);
            history1.AreOldValuesArray.IsFalse();
            history1.AreNewValuesArray.IsFalse();
            history1.OldValues.IsNull();
            history1.NewValues.Single().LeafValue.Is("2");
            history.Count().Is(2);
        }

        private static TreeElement<string, string> CreateMergedTree()
        {
            var user2 = new TreeElement<string, string>(new[]{
                new KeyValuePair<string, TreeElement<string, string>>("name", new TreeElement<string, string>("Ken")),
                new KeyValuePair<string, TreeElement<string, string>>("id", new TreeElement<string, string>("22")),
            });
            var user3WithoutName = new TreeElement<string, string>(new[]{
                new KeyValuePair<string, TreeElement<string, string>>("id", new TreeElement<string, string>("31")),
            });
            var oldUsers = new TreeElement<string, string>();
            oldUsers.ModifyArrayChild("users", _ => new[] { user2, user3WithoutName });
            var oldTree = new TreeElement<string, string>();
            oldTree.SetSingleChild("date", new TreeElement<string, string>("2012/12/31"));
            oldTree.SetSingleChild("caution", new TreeElement<string, string>("Slow!"));
            oldTree.ModifyArrayChild("tags", _ => new[] { new TreeElement<string, string>("314"), new TreeElement<string, string>("159") });
            oldTree.ModifyArrayChild("users", _ => new[] { user2, user3WithoutName });
            return oldTree;
        }

        private static TreeElement<string, string> CreateMergingTree()
        {
            var user1 = new TreeElement<string, string>(new[]{
                new KeyValuePair<string, TreeElement<string, string>>("name", new TreeElement<string, string>("Mark")),
                new KeyValuePair<string, TreeElement<string, string>>("id", new TreeElement<string, string>("10")),
            });
            var user3 = new TreeElement<string, string>(new[]{
                new KeyValuePair<string, TreeElement<string, string>>("name", new TreeElement<string, string>("Yumi")),
                new KeyValuePair<string, TreeElement<string, string>>("id", new TreeElement<string, string>("31")),
            });
            var newUsers = new TreeElement<string, string>();
            newUsers.ModifyArrayChild("users", _ => new[] { user1, user3 });
            var newTree = new TreeElement<string, string>();
            newTree.SetSingleChild("date", new TreeElement<string, string>("2013/01/01"));
            newTree.ModifyArrayChild("tags", _ => new[] { new TreeElement<string, string>("314"), new TreeElement<string, string>("-1") });
            newTree.SetSingleChild("error", new TreeElement<string, string>("Rate limited."));
            newTree.ModifyArrayChild("users", _ => new[] { user3, user1 });
            return newTree;
        }

        [TestMethod]
        public void MergeAnotherTreeTest()
        {
            var oldTree = CreateMergedTree();
            var newTree = CreateMergingTree();

            oldTree.Merge(newTree, (o, n, _) => n.ToChildWithoutKey());
            
            oldTree["error"].LeafValue.Is("Rate limited.");
            oldTree["date"].LeafValue.Is("2013/01/01");
            oldTree["caution"].LeafValue.Is("Slow!");
            oldTree.GetArrayChildOrDefault("users").Select(c => c.GetSingleValue("name").Value).NonSequenceEqual(new[] { "Mark", "Yumi" }).IsTrue();
            oldTree.GetArrayChildOrDefault("users").Select(c => c.GetSingleValue("id").Value).NonSequenceEqual(new[] { "10", "31" }).IsTrue();
            oldTree.GetArrayChildOrDefault("tags").Select(c => c.LeafValue).NonSequenceEqual(new[] { "314", "-1" }).IsTrue();

            oldTree.GetAllChildren().Count().Is(5);
        }

        [TestMethod]
        public void MergeAnotherTreeWithArraySelectorTest()
        {
            var oldTree = CreateMergedTree();
            var newTree = CreateMergingTree();

            oldTree.MergeWithArraySelector(newTree, (e, _) => e.Type == ElementType.Node ? e["id"].LeafValue : (object)e);

            oldTree["error"].LeafValue.Is("Rate limited.");
            oldTree["date"].LeafValue.Is("2013/01/01");
            oldTree["caution"].LeafValue.Is("Slow!");
            oldTree.GetArrayChildOrDefault("users").Select(c => c.GetSingleValue("name").Value).NonSequenceEqual(new[] { "Mark", "Yumi", "Ken" }).IsTrue();
            oldTree.GetArrayChildOrDefault("users").Select(c => c.GetSingleValue("id").Value).NonSequenceEqual(new[] { "10", "22", "31" }).IsTrue();
            oldTree.GetArrayChildOrDefault("tags").Select(c => c.LeafValue).NonSequenceEqual(new[] { "314", "159", "-1" }).IsTrue();

            oldTree.GetAllChildren().Count().Is(5);
        }

        [TestMethod]
        public void GetGrandChildrenTest()
        {
            var j = @"
{
    ""OS"": [
        ""Win"", 
        ""Mac"", 
        ""Linux""
    ], 
    ""Users"": [
        {
            ""Birth"": [
                ""1970"", 
                ""1"", 
                ""1""
            ], 
            ""Name"": ""Yamada""
        }, 
        {
            ""Birth"": [
                ""1980"", 
                ""2"", 
                ""2""
            ], 
            ""Name"": ""Tanaka""
        }
    ],
    ""Version"": ""1.0""
}
";
            var rootId = TreeElementConverter.RootId;
            var te = TreeElementConverter.ConvertJson(j);
            var versionGrandChildren = te.GetAllGrandChildren(new[] { rootId, "Version" });
            var osGrandChildren = te.GetAllGrandChildren(new[] { rootId, "OS" });
            var usersBirthHistory = te.GetAllGrandChildren(new[] { rootId, "Users", "Birth" });

            versionGrandChildren.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.CastOrDefault<string>().Is("1.0");
            versionGrandChildren.Count().Is(1);

            osGrandChildren.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrDefault<string>().Is("Win");
            osGrandChildren.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1 })).Value.LeafValue.CastOrDefault<string>().Is("Mac");
            osGrandChildren.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 2 })).Value.LeafValue.CastOrDefault<string>().Is("Linux");
            osGrandChildren.Count().Is(3);

            usersBirthHistory.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0, 0 })).Value.LeafValue.CastOrDefault<string>().Is("1970");
            usersBirthHistory.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0, 1 })).Value.LeafValue.CastOrDefault<string>().Is("1");
            usersBirthHistory.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0, 2 })).Value.LeafValue.CastOrDefault<string>().Is("1");
            usersBirthHistory.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1, 0 })).Value.LeafValue.CastOrDefault<string>().Is("1980");
            usersBirthHistory.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1, 1 })).Value.LeafValue.CastOrDefault<string>().Is("2");
            usersBirthHistory.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1, 2 })).Value.LeafValue.CastOrDefault<string>().Is("2");
            usersBirthHistory.Count().Is(6);
        }

        [TestMethod]
        public void EqualsAndHashCodeTest()
        {
            var te_a1 = new TreeElement<string, string>("a");
            var te_a2 = new TreeElement<string, string>("a");
            var te_b = new TreeElement<string, string>("b");
            var te = new TreeElement<string, string>();

            te_a1.Is(te_a1);
            te_a1.GetHashCode().Is(te_a1.GetHashCode());
            te_a1.Is(te_a2);
            te_a1.GetHashCode().Is(te_a2.GetHashCode());
            te_a1.IsNot(te_b);
            te_a1.IsNot(te);
        }
    }
}
