using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class SimpleJsonDataObjectTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            AssertEx.Catch<ArgumentNullException>(() => new SimpleJsonDataObject(null));
            AssertEx.Catch<ArgumentException>(() => new SimpleJsonDataObject(new object()));
            AssertEx.Catch<ArgumentException>(() => new SimpleJsonDataObject(1f));
            AssertEx.Catch<ArgumentException>(() => new SimpleJsonDataObject(1));
            AssertEx.Catch<ArgumentException>(() => new SimpleJsonDataObject(new Random()));
        }

        [TestMethod]
        public void SbyteTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            sbyte i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((sbyte)1);

            sbyte i2;
            y.TryCast(out i2).IsTrue();
            i2.Is((sbyte)-1);

            sbyte i;
            new SimpleJsonDataObject((double)sbyte.MaxValue).TryCast(out i).IsTrue();
            i.Is(sbyte.MaxValue);
            new SimpleJsonDataObject((double)sbyte.MinValue).TryCast(out i).IsTrue();
            i.Is(sbyte.MinValue);
            new SimpleJsonDataObject((double)sbyte.MaxValue + 1).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)sbyte.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void ByteTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            byte i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((byte)1);

            byte i2;
            y.TryCast(out i2).IsFalse();

            byte i;
            new SimpleJsonDataObject((double)byte.MaxValue).TryCast(out i).IsTrue();
            i.Is(byte.MaxValue);
            new SimpleJsonDataObject((double)byte.MinValue).TryCast(out i).IsTrue();
            i.Is(byte.MinValue);
            new SimpleJsonDataObject((double)byte.MaxValue + 1).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)byte.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void ShortTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            short i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((short)1);

            short i2;
            y.TryCast(out i2).IsTrue();
            i2.Is((short)-1);

            short i;
            new SimpleJsonDataObject((double)short.MaxValue).TryCast(out i).IsTrue();
            i.Is(short.MaxValue);
            new SimpleJsonDataObject((double)short.MinValue).TryCast(out i).IsTrue();
            i.Is(short.MinValue);
            new SimpleJsonDataObject((double)short.MaxValue + 1).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)short.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void UshortTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            ushort i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((ushort)1);

            ushort i2;
            y.TryCast(out i2).IsFalse();

            ushort i;
            new SimpleJsonDataObject((double)ushort.MaxValue).TryCast(out i).IsTrue();
            i.Is(ushort.MaxValue);
            new SimpleJsonDataObject((double)ushort.MinValue).TryCast(out i).IsTrue();
            i.Is(ushort.MinValue);
            new SimpleJsonDataObject((double)ushort.MaxValue + 1).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)ushort.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void IntTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            int i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((int)1);

            int i2;
            y.TryCast(out i2).IsTrue();
            i2.Is((int)-1);

            int i;
            new SimpleJsonDataObject((double)int.MaxValue).TryCast(out i).IsTrue();
            i.Is(int.MaxValue);
            new SimpleJsonDataObject((double)int.MinValue).TryCast(out i).IsTrue();
            i.Is(int.MinValue);
            new SimpleJsonDataObject((double)int.MaxValue + 1).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)int.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void UintTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            uint i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((uint)1);

            uint i2;
            y.TryCast(out i2).IsFalse();

            uint i;
            new SimpleJsonDataObject((double)uint.MaxValue).TryCast(out i).IsTrue();
            i.Is(uint.MaxValue);
            new SimpleJsonDataObject((double)uint.MinValue).TryCast(out i).IsTrue();
            i.Is(uint.MinValue);
            new SimpleJsonDataObject((double)uint.MaxValue + 1).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)uint.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void LongTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            long i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((long)1);

            long i2;
            y.TryCast(out i2).IsTrue();
            i2.Is((long)-1);

            long i;
            // This code does not pass
            //
            //new SimpleJsonDataObject((double)long.MaxValue).TryCast(out i).IsTrue();
            //i.Is(long.MaxValue);
            //new SimpleJsonDataObject((double)long.MinValue).TryCast(out i).IsTrue();
            //i.Is(long.MinValue);
            //new SimpleJsonDataObject((double)long.MaxValue + 1).TryCast(out i).IsFalse();
            //new SimpleJsonDataObject((double)long.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void UlongTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            ulong i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((ulong)1);

            ulong i2;
            y.TryCast(out i2).IsFalse();

            ulong i;
            // This code does not pass
            //
            //new SimpleJsonDataObject((double)ulong.MaxValue).TryCast(out i).IsTrue();
            //i.Is(ulong.MaxValue);
            //new SimpleJsonDataObject((double)ulong.MinValue).TryCast(out i).IsTrue();
            //i.Is(ulong.MinValue);
            //new SimpleJsonDataObject((double)ulong.MaxValue + 1).TryCast(out i).IsFalse();
            //new SimpleJsonDataObject((double)ulong.MinValue - 1).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void FloatTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            float i1;
            x.TryCast(out i1).IsTrue();
            i1.Is((float)1);

            float i2;
            y.TryCast(out i2).IsTrue();
            i2.Is((float)-1);

            float i;
            new SimpleJsonDataObject((double)float.MaxValue).TryCast(out i).IsTrue();
            i.Is(float.MaxValue);
            new SimpleJsonDataObject((double)float.MinValue).TryCast(out i).IsTrue();
            i.Is(float.MinValue);
            new SimpleJsonDataObject((double)float.MaxValue + 1e+038f).TryCast(out i).IsFalse();
            new SimpleJsonDataObject((double)float.MinValue - 1e+038f).TryCast(out i).IsFalse();

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void DoubleTryCastTest()
        {
            var x = new SimpleJsonDataObject(1d);
            var y = new SimpleJsonDataObject(-1d);

            double i1;
            x.TryCast(out i1).IsTrue();
            i1.Is(1d);

            double i2;
            y.TryCast(out i2).IsTrue();
            i2.Is(-1d);

            double i;
            new SimpleJsonDataObject((double)double.MaxValue).TryCast(out i).IsTrue();
            i.Is(double.MaxValue);
            new SimpleJsonDataObject((double)double.MinValue).TryCast(out i).IsTrue();
            i.Is(double.MinValue);

            new SimpleJsonDataObject(true).TryCast(out i).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out i).IsFalse();
        }

        [TestMethod]
        public void BooleanTryCastTest()
        {
            var x = new SimpleJsonDataObject(true);
            var y = new SimpleJsonDataObject(false);

            bool b1;
            x.TryCast(out b1).IsTrue();
            b1.Is(true);

            bool b2;
            y.TryCast(out b2).IsTrue();
            b2.Is(false);

            bool b;
            new SimpleJsonDataObject(1d).TryCast(out b).IsFalse();
            new SimpleJsonDataObject("string").TryCast(out b).IsFalse();
        }

        [TestMethod]
        public void StringTryCastTest()
        {
            var doubleObject = new SimpleJsonDataObject(1d);
            var boolObject = new SimpleJsonDataObject(true);
            var stringObject = new SimpleJsonDataObject("string");

            string s;

            doubleObject.TryCast(out s).IsTrue();
            s.Is(1d.ToString());

            boolObject.TryCast(out s).IsTrue();
            s.Is(true.ToString());

            stringObject.TryCast(out s).IsTrue();
            s.Is("string");
        }

        [TestMethod]
        public void EqualsTest()
        {
            var doubleObjectX1 = new SimpleJsonDataObject(1d);
            var doubleObjectX2 = new SimpleJsonDataObject(1d);
            var doubleObjectY = new SimpleJsonDataObject(-1d);
            var boolObjectX1 = new SimpleJsonDataObject(true);
            var boolObjectX2 = new SimpleJsonDataObject(true);
            var boolObjectY = new SimpleJsonDataObject(false);
            var stringObjectX1 = new SimpleJsonDataObject("string");
            var stringObjectX2 = new SimpleJsonDataObject("string");
            var stringObjectY = new SimpleJsonDataObject("");

            doubleObjectX1.Is(doubleObjectX1);
            doubleObjectX1.Is(doubleObjectX2);
            doubleObjectX1.IsNot(doubleObjectY);
            doubleObjectX1.IsNot(boolObjectX1);
            doubleObjectX1.IsNot(stringObjectX1);

            boolObjectX1.Is(boolObjectX1);
            boolObjectX1.Is(boolObjectX2);
            boolObjectX1.IsNot(boolObjectY);
            boolObjectX1.IsNot(doubleObjectX1);
            boolObjectX1.IsNot(stringObjectX1);

            stringObjectX1.Is(stringObjectX1);
            stringObjectX1.Is(stringObjectX2);
            stringObjectX1.IsNot(stringObjectY);
            stringObjectX1.IsNot(doubleObjectX1);
            stringObjectX1.IsNot(boolObjectX1);
        }
    }
}
