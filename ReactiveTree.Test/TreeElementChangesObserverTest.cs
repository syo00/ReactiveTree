using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using Kirinji.ReactiveTree.TreeElements;
using Kirinji.LightWands.Tests;
using Kirinji.LightWands;
using Kirinji.ReactiveTree.Merging;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class TreeElementChangesObserverTest
    {
        [TestMethod]
        public void ValueChangedTest()
        {
            var tree = new TreeElement<int, string>();
            tree[1] = new TreeElement<int, string>(new KeyValuePair<int, TreeElement<int, string>>(2, new TreeElement<int, string>("1/2")));
            // 1/2 => "1/2"
            var obs1 = new TreeElementChangesObserver<int, string>(tree, new[] { 1, 2 });
            var obs2 = new TreeElementChangesObserver<int, string>(tree, new[] { 1 });

            var history1 = obs1.ValueChanged.SubscribeHistory();
            var history2 = obs2.ValueChanged.SubscribeHistory();

            obs1.Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.Is("1/2");
            obs2.Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null })).Value[2].LeafValue.Is("1/2");

            tree.GetSingleChildOrDefault(new[] { 1 }).SetSingleChild(2, new TreeElement<int, string>("1/2(2)"));
            // 1/2 => "1/2(2)"
            tree.SetSingleChild(1, new TreeElement<int, string>("1"));
            // 1 => "1"

            obs1.Value.Any().IsFalse();
            obs2.Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null })).Value.LeafValue.Is("1");

            history1.Values.ElementAt(0).Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.Is("1/2(2)");
            history1.Values.ElementAt(1).Any().IsFalse();
            history1.Count().Is(2);
            
            history2.Values.ElementAt(0).Single(gc => gc.Indexes.SequenceEqual(new int?[] { null })).Value[2].LeafValue.Is("1/2(2)");
            history2.Values.ElementAt(1).Single(gc => gc.Indexes.SequenceEqual(new int?[] { null })).Value.LeafValue.Is("1");
            history2.Count().Is(2);

            obs2.IsSubscribing.IsTrue();
            obs2.StopSubscription();
            obs2.IsSubscribing.IsFalse();
            tree.SetSingleChild(1, new TreeElement<int, string>("2"));
            history2.Last().Kind.Is(NotificationKind.OnCompleted);
            history2.Count().Is(3);
        }
    }
}
