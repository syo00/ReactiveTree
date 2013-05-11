using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kirinji.ReactiveTree.TreeElements;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class GrandChildrenContainerTest
    {
        [TestMethod]
        public void EqualsAndHashCodeTest()
        {
            var gc = new GrandChild<string, string>(new int?[] { 1, null, 1 }, new TreeElement<string, string>("text"));

            new GrandChildrenContainer<string, string>(new[] { "a", "b" }, new[] { gc })
            .Is(new GrandChildrenContainer<string, string>(new[] { "a", "b" }, new[] { gc }));

            new GrandChildrenContainer<string, string>(new[] { "a", "b" }, new[] { gc }).GetHashCode()
            .Is(new GrandChildrenContainer<string, string>(new[] { "a", "b" }, new[] { gc }).GetHashCode());

            Assert.AreNotEqual(new GrandChildrenContainer<string, string>(new[] { "a", "b" }, new[] { gc }),
            new GrandChildrenContainer<string, string>(new[] { "b", "a" }, new[] { gc }));
        }
    }
}
