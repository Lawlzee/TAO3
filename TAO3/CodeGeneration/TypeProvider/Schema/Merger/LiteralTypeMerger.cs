using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class LiteralTypeMerger : ILiteralTypeMerger
    {
        private readonly Dictionary<(Type, Type), Type> _mergeTable;

        public LiteralTypeMerger()
        {
            _mergeTable = new Dictionary<(Type, Type), Type>();
            //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions
            this[typeof(sbyte), typeof(short)] = typeof(short);
            this[typeof(sbyte), typeof(int)] = typeof(int);
            this[typeof(sbyte), typeof(long)] = typeof(long);
            this[typeof(sbyte), typeof(float)] = typeof(float);
            this[typeof(sbyte), typeof(double)] = typeof(double);
            this[typeof(sbyte), typeof(decimal)] = typeof(decimal);
            this[typeof(sbyte), typeof(nint)] = typeof(nint);
            this[typeof(byte), typeof(short)] = typeof(short);
            this[typeof(byte), typeof(ushort)] = typeof(ushort);
            this[typeof(byte), typeof(int)] = typeof(int);
            this[typeof(byte), typeof(uint)] = typeof(uint);
            this[typeof(byte), typeof(long)] = typeof(long);
            this[typeof(byte), typeof(ulong)] = typeof(ulong);
            this[typeof(byte), typeof(float)] = typeof(float);
            this[typeof(byte), typeof(double)] = typeof(double);
            this[typeof(byte), typeof(decimal)] = typeof(decimal);
            this[typeof(byte), typeof(nint)] = typeof(nint);
            this[typeof(byte), typeof(nuint)] = typeof(nuint);
            this[typeof(short), typeof(int)] = typeof(int);
            this[typeof(short), typeof(long)] = typeof(long);
            this[typeof(short), typeof(float)] = typeof(float);
            this[typeof(short), typeof(double)] = typeof(double);
            this[typeof(short), typeof(decimal)] = typeof(decimal);
            this[typeof(short), typeof(nint)] = typeof(nint);
            this[typeof(ushort), typeof(int)] = typeof(int);
            this[typeof(ushort), typeof(uint)] = typeof(uint);
            this[typeof(ushort), typeof(long)] = typeof(long);
            this[typeof(ushort), typeof(ulong)] = typeof(ulong);
            this[typeof(ushort), typeof(float)] = typeof(float);
            this[typeof(ushort), typeof(double)] = typeof(double);
            this[typeof(ushort), typeof(decimal)] = typeof(decimal);
            this[typeof(ushort), typeof(nint)] = typeof(nint);
            this[typeof(ushort), typeof(nuint)] = typeof(nuint);
            this[typeof(int), typeof(long)] = typeof(long);
            this[typeof(int), typeof(float)] = typeof(float);
            this[typeof(int), typeof(double)] = typeof(double);
            this[typeof(int), typeof(decimal)] = typeof(decimal);
            this[typeof(int), typeof(nint)] = typeof(nint);
            this[typeof(uint), typeof(long)] = typeof(long);
            this[typeof(uint), typeof(ulong)] = typeof(ulong);
            this[typeof(uint), typeof(float)] = typeof(float);
            this[typeof(uint), typeof(double)] = typeof(double);
            this[typeof(uint), typeof(decimal)] = typeof(decimal);
            this[typeof(uint), typeof(nuint)] = typeof(nuint);
            this[typeof(long), typeof(float)] = typeof(float);
            this[typeof(long), typeof(double)] = typeof(double);
            this[typeof(long), typeof(decimal)] = typeof(decimal);
            this[typeof(ulong), typeof(float)] = typeof(float);
            this[typeof(ulong), typeof(double)] = typeof(double);
            this[typeof(ulong), typeof(decimal)] = typeof(decimal);
            this[typeof(float), typeof(double)] = typeof(double);
            this[typeof(nint), typeof(long)] = typeof(long);
            this[typeof(nint), typeof(float)] = typeof(float);
            this[typeof(nint), typeof(double)] = typeof(double);
            this[typeof(nint), typeof(decimal)] = typeof(decimal);
            this[typeof(nuint), typeof(ulong)] = typeof(ulong);
            this[typeof(nuint), typeof(float)] = typeof(float);
            this[typeof(nuint), typeof(double)] = typeof(double);
            this[typeof(nuint), typeof(decimal)] = typeof(decimal);

            //For formats with integer boolean types
            //todo: move to Sql only
            this[typeof(bool), typeof(sbyte)] = typeof(sbyte);
            this[typeof(bool), typeof(byte)] = typeof(byte);
            this[typeof(bool), typeof(short)] = typeof(short);
            this[typeof(bool), typeof(ushort)] = typeof(ushort);
            this[typeof(bool), typeof(int)] = typeof(int);
            this[typeof(bool), typeof(uint)] = typeof(uint);
            this[typeof(bool), typeof(long)] = typeof(long);
            this[typeof(bool), typeof(ulong)] = typeof(ulong);
            this[typeof(bool), typeof(float)] = typeof(float);
            this[typeof(bool), typeof(nint)] = typeof(nint);
            this[typeof(bool), typeof(nuint)] = typeof(nuint);
        }

        protected Type this[Type typeA, Type typeB]
        {
            set {
                _mergeTable[(typeA, typeB)] = value;
                _mergeTable[(typeB, typeA)] = value;
            }
        }

        public Type Merge(Type typeA, Type typeB)
        {
            if (typeA == typeB)
            {
                return typeA;
            }

            return _mergeTable.GetValueOrDefault((typeA, typeB))
                ?? _mergeTable.GetValueOrDefault((typeB, typeA))
                ?? MergeWhenNoMatch(typeA, typeB);

        }

        protected virtual Type MergeWhenNoMatch(Type typeA, Type typeB)
        {
            return typeof(string);
        }
    }
}
