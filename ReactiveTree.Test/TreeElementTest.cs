using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Kirinji.LightWands;
using Kirinji.ReactiveTree;
using Kirinji.LightWands.Tests;
using Kirinji.ReactiveTree.TreeStructures;
using System.Collections.Specialized;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class TreeElementTest : ReactiveTest
    {
        [TestMethod]
        public void LeafTest()
        {
            var leaf = new TreeElement<int, string>("1");

            AssertEx.Catch<InvalidOperationException>(() => leaf.Array.ToString());
            AssertEx.Catch<InvalidOperationException>(() => leaf.ArrayChanged.Subscribe());
            AssertEx.Catch<InvalidOperationException>(() => leaf.NodeChildren.ToString());
            AssertEx.Catch<InvalidOperationException>(() => leaf.NodeChildrenChanged.Subscribe());
            leaf.GrandChildrenChanged.Subscribe(); // throws no exception
            leaf.LeafValue.Is("1");
            leaf.Type.Is(ElementType.Leaf);
        }

        [TestMethod]
        public void NodeConstructorTest()
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
            AssertEx.Catch<ArgumentException>(() => new TreeElement<int, string>(elementsWithSameKey));
            AssertEx.Catch<ArgumentException>(() => new TreeElement<int, string>(elementsWithSameKey, false));

            var node = new TreeElement<int, string>(elementsWithSameKey, true);
            node.NodeChildren[0].Type.Is(ElementType.Array);
            node.NodeChildren[0].Array.Count.Is(2);
            node.NodeChildren[1].Type.Is(ElementType.Leaf);
            node.NodeChildren.Count.Is(2);
        }

        [TestMethod]
        public void NodeTest()
        {
            var elementsWithSameKey = new[]{
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("0")),
                new KeyValuePair<int, TreeElement<int, string>>(0, new TreeElement<int,string>("Zero")),
                new KeyValuePair<int, TreeElement<int, string>>(1, new TreeElement<int, string>("1"))
            };
            var node = new TreeElement<int, string>(elementsWithSameKey, true);

            var nodeChildrenChangedHistory = node.NodeChildrenChanged.SubscribeHistory();
            var grandChildrenChangedHistory = node.GrandChildrenChanged.SubscribeHistory();

            node.Type.Is(ElementType.Node);
            AssertEx.Catch<InvalidOperationException>(() => node.LeafValue.ToString());
            AssertEx.Catch<InvalidOperationException>(() => node.Array.ToString());
            AssertEx.Catch<InvalidOperationException>(() => node.ArrayChanged.ToString());

            node.NodeChildren[0].Array[0].LeafValue.Is("0");
            node.NodeChildren[0].Array[1].LeafValue.Is("Zero");
            node.NodeChildren[0].Array.Count.Is(2);
            node.NodeChildren[1].LeafValue.Is("1");
            node.NodeChildren.Count.Is(2);

            nodeChildrenChangedHistory.Count().Is(0);
            grandChildrenChangedHistory.Count().Is(0);

            node.NodeChildren[0] = new TreeElement<int, string>("A");
            node.NodeChildren.Remove(1);
            node.NodeChildren[1] = new TreeElement<int, string>("B");

            nodeChildrenChangedHistory.Values.Select(x => x.Action).IsSequenceEqual(NotifyCollectionChangedAction.Replace, NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Add);
            grandChildrenChangedHistory.Values.Select(x => x.Value.Action).IsSequenceEqual(NotifyCollectionChangedAction.Replace, NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Add);
        }

        [TestMethod]
        public void ArrayConstructorTest()
        {
            AssertEx.Catch<ArgumentNullException>(() => new TreeElement<int, string>((TreeElement<int, string>[])null));
            AssertEx.Catch<ArgumentNullException>(() => new TreeElement<int, string>((IEnumerable<TreeElement<int, string>>)null));
            AssertEx.Catch<Exception>(() => new TreeElement<int, string>(new[] { new TreeElement<int, string>("A"), null })); // ContractException

            new TreeElement<int, string>(new TreeElement<int, string>("A"));
            new TreeElement<int, string>(new[] { new TreeElement<int, string>("A") }.AsEnumerable());
        }

        [TestMethod]
        public void ArrayTest()
        {
            var array = new TreeElement<int, string>(new TreeElement<int, string>("A"), new TreeElement<int, string>("B"));

            var arrayChangedHistory = array.ArrayChanged.SubscribeHistory();
            var grandChildrenChangedHistory = array.GrandChildrenChanged.SubscribeHistory();

            array.Type.Is(ElementType.Array);
            AssertEx.Catch<InvalidOperationException>(() => array.LeafValue.ToString());
            AssertEx.Catch<InvalidOperationException>(() => array.NodeChildren.ToString());
            AssertEx.Catch<InvalidOperationException>(() => array.NodeChildrenChanged.ToString());

            array.Array[0].LeafValue.Is("A");
            array.Array[1].LeafValue.Is("B");
            array.Array.Count.Is(2);

            arrayChangedHistory.Count().Is(0);
            grandChildrenChangedHistory.Count().Is(0);

            array.Array.Add(new TreeElement<int, string>("C"));
            array.Array.Clear();

            arrayChangedHistory.Values.Select(x => x.Action).IsSequenceEqual(NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Reset);
            grandChildrenChangedHistory.Values.Select(x => x.Value.Action).IsSequenceEqual(NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Reset);
        }

        [TestMethod]
        public void MergeTest()
        {
            var merged = CreateMergedTree();
            var merging = CreateMergingTree();
            merged.Merge(merging, (x, y) =>
                {
                    var xv = x.GetOrDefault(new KeyOrIndex<string>("id"));
                    var yv = y.GetOrDefault(new KeyOrIndex<string>("id"));
                    if (xv == null) return false;
                    return Object.Equals(xv, yv);
                });

            merged.NodeChildren["date"].LeafValue.Is("2013/01/01");
            merged.NodeChildren["users"].Array.Single(u => u.NodeChildren["id"].LeafValue == "10").NodeChildren["name"].LeafValue.Is("Mark");
            merged.NodeChildren["users"].Array.Single(u => u.NodeChildren["id"].LeafValue == "31").NodeChildren["name"].LeafValue.Is("Yumi");
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
            var oldTree = new TreeElement<string, string>();
            oldTree.NodeChildren["date"] = new TreeElement<string, string>("2012/12/31");
            oldTree.NodeChildren["caution"] = new TreeElement<string, string>("Slow!");
            oldTree.NodeChildren["tags"] = new TreeElement<string, string>(new[] { new TreeElement<string, string>("314"), new TreeElement<string, string>("159") });
            oldTree.NodeChildren["users"] = new TreeElement<string, string>(user2, user3WithoutName);
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

            var newTree = new TreeElement<string, string>();
            newTree.NodeChildren["date"] = new TreeElement<string, string>("2013/01/01");
            newTree.NodeChildren["tags"] = new TreeElement<string, string>(new[] { new TreeElement<string, string>("314"), new TreeElement<string, string>("159") });
            newTree.NodeChildren["error"] = new TreeElement<string, string>("Rate limited.");
            newTree.NodeChildren["users"] = new TreeElement<string, string>(user1, user3);
            return newTree;
        }

        [TestMethod]
        public void EqualsAndHashCodeTest()
        {
            var te_a1 = new TreeElement<string, string>("a");
            var te_a2 = new TreeElement<string, string>("a");
            var te_b = new TreeElement<string, string>("b");
            var te_node = new TreeElement<string, string>();

            te_a1.Is(te_a1);
            te_a1.GetHashCode().Is(te_a1.GetHashCode());
            te_a1.Is(te_a2);
            te_a1.GetHashCode().Is(te_a2.GetHashCode());
            te_a1.IsNot(te_b);
            te_a1.IsNot(te_node);
        }
    }
}
