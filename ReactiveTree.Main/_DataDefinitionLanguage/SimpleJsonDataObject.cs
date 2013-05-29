using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree
{
    /// <summary>JSON value container which can convert easily.</summary>
    internal class SimpleJsonDataObject : DataObject, IEquatable<SimpleJsonDataObject>
    {
        private readonly SimpleJsonDataObjectType type;

        public SimpleJsonDataObject(object obj)
            : base(obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null);

            if (obj is double)
            {
                this.type = SimpleJsonDataObjectType.Double;
            }
            else if (obj is string)
            {
                this.type = SimpleJsonDataObjectType.String;
            }
            else if (obj is bool)
            {
                this.type = SimpleJsonDataObjectType.Boolean;
            }
            else if(obj is long)
            {
                this.type = SimpleJsonDataObjectType.Long;
            }
            else
            {
                throw new ArgumentException("obj is not an object SimpleJson supports.");
            }
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.type != SimpleJsonDataObjectType.None);
        }

        public override bool TryCast<T>(out T value)
        {
            var tType = typeof(T);

            switch (this.type)
            {
                case SimpleJsonDataObjectType.Boolean:
                    {
                        if (tType == typeof(bool) || tType == typeof(bool?))
                        {
                            value = (T)this.InnerObject;
                            return true;
                        }
                        else if (tType == typeof(string))
                        {
                            value = (T)(object)this.InnerObject.ToString();
                            return true;
                        }
                        else
                        {
                            value = default(T);
                            return false;
                        }
                    }
                case SimpleJsonDataObjectType.String:
                    {
                        if (tType == typeof(string))
                        {
                            value = (T)this.InnerObject;
                            return true;
                        }
                        else
                        {
                            value = default(T);
                            return false;
                        }
                    }
                case SimpleJsonDataObjectType.Double:
                    {
                        var d = (double)this.InnerObject;

                        if (tType == typeof(double) || tType == typeof(double?))
                        {
                            value = (T)(object)d;
                            return true;
                        }
                        else if (tType == typeof(string))
                        {
                            value = (T)(object)this.InnerObject.ToString();
                            return true;
                        }
                        else if (tType == typeof(sbyte) || tType == typeof(sbyte?))
                        {
                            if (sbyte.MinValue <= d && d <= sbyte.MaxValue)
                            {
                                value = (T)(object)(sbyte)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(byte) || tType == typeof(byte?))
                        {
                            if (byte.MinValue <= d && d <= byte.MaxValue)
                            {
                                value = (T)(object)(byte)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(short) || tType == typeof(short?))
                        {
                            if (short.MinValue <= d && d <= short.MaxValue)
                            {
                                value = (T)(object)(short)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(ushort) || tType == typeof(ushort?))
                        {
                            if (ushort.MinValue <= d && d <= ushort.MaxValue)
                            {
                                value = (T)(object)(ushort)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(int) || tType == typeof(int?))
                        {
                            if (int.MinValue <= d && d <= int.MaxValue)
                            {
                                value = (T)(object)(int)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(uint) || tType == typeof(uint?))
                        {
                            if (uint.MinValue <= d && d <= uint.MaxValue)
                            {
                                value = (T)(object)(uint)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(long) || tType == typeof(long?))
                        {
                            if (long.MinValue <= d && d <= long.MaxValue)
                            {
                                value = (T)(object)(long)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(ulong) || tType == typeof(ulong?))
                        {
                            if (ulong.MinValue <= d && d <= ulong.MaxValue)
                            {
                                value = (T)(object)(ulong)d;
                                return true;
                            }
                        }
                        else if (tType == typeof(float) || tType == typeof(float?))
                        {
                            if (float.MinValue <= d && d <= float.MaxValue)
                            {
                                value = (T)(object)(float)d;
                                return true;
                            }
                        }
                        value = default(T);
                        return false;
                    }
                case SimpleJsonDataObjectType.Long:
                    {
                        var l = (long)this.InnerObject;

                        if (tType == typeof(long) || tType == typeof(long?))
                        {
                            value = (T)(object)l;
                            return true;
                        }
                        else if (tType == typeof(string))
                        {
                            value = (T)(object)this.InnerObject.ToString();
                            return true;
                        }
                        else if (tType == typeof(sbyte) || tType == typeof(sbyte?))
                        {
                            if (sbyte.MinValue <= l && l <= sbyte.MaxValue)
                            {
                                value = (T)(object)(sbyte)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(byte) || tType == typeof(byte?))
                        {
                            if (byte.MinValue <= l && l <= byte.MaxValue)
                            {
                                value = (T)(object)(byte)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(short) || tType == typeof(short?))
                        {
                            if (short.MinValue <= l && l <= short.MaxValue)
                            {
                                value = (T)(object)(short)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(ushort) || tType == typeof(ushort?))
                        {
                            if (ushort.MinValue <= l && l <= ushort.MaxValue)
                            {
                                value = (T)(object)(ushort)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(int) || tType == typeof(int?))
                        {
                            if (int.MinValue <= l && l <= int.MaxValue)
                            {
                                value = (T)(object)(int)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(uint) || tType == typeof(uint?))
                        {
                            if (uint.MinValue <= l && l <= uint.MaxValue)
                            {
                                value = (T)(object)(uint)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(ulong) || tType == typeof(ulong?))
                        {
                            if (0 <= l)
                            {
                                value = (T)(object)Convert.ToUInt64(l);
                                return true;
                            }
                        }
                        else if (tType == typeof(float) || tType == typeof(float?))
                        {
                            if (float.MinValue <= l && l <= float.MaxValue)
                            {
                                value = (T)(object)(float)l;
                                return true;
                            }
                        }
                        else if (tType == typeof(double) || tType == typeof(double?))
                        {
                            if (double.MinValue <= l && l <= double.MaxValue)
                            {
                                value = (T)(object)(double)l;
                                return true;
                            }
                        }
                        value = default(T);
                        return false;
                    }
                default:
                    {
                        value = default(T);
                        return false;
                    }
            }
        }

        public override string ToString()
        {
            return this.InnerObject.ToString();
        }

        public virtual bool Equals(SimpleJsonDataObject other)
        {
            if (other == null) return false;

            return Object.Equals(this.InnerObject, other.InnerObject);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;

            return this.Equals((SimpleJsonDataObject)obj);
        }

        public override int GetHashCode()
        {
            return this.InnerObject.GetHashCode();
        }

        private enum SimpleJsonDataObjectType
        {
            None,
            String,
            Long,
            Double,
            Boolean,
        }
    }
}
