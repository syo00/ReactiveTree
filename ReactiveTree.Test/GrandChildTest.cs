using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kirinji.ReactiveTree.TreeElements;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class GrandChildTest
    {
        [TestMethod]
        public void EqualsAndHashCodeTest()
        {
            var te = new TreeElement<string, string>("text");

            new GrandChild<string, string>(new int?[] { null, 0, 1 }, te)
            .Is(new GrandChild<string, string>(new int?[] { null, 0, 1 }, te));

            new GrandChild<string, string>(new int?[] { null, 0, 1 }, te).GetHashCode()
            .Is(new GrandChild<string, string>(new int?[] { null, 0, 1 }, te).GetHashCode());

            new GrandChild<string, string>(new int?[] { null, 0, 1 }, new TreeElement<string, string>("aaa"))
            .IsNot(new GrandChild<string, string>(new int?[] { null, 0, 1 }, new TreeElement<string, string>("aaa2")));

            new GrandChild<string, string>(new int?[] { null, 0, 1 }, te)
            .IsNot(new GrandChild<string, string>(new int?[] { null, null, 1 }, te));
        }
    }
}
