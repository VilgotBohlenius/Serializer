using System;
using System.Reflection;
using System.Text;

namespace Fish.Serialization
{
    public sealed class ByteSerializer
    {
        byte[] buffer;
        int writePosition = 0;
        int readPosition = 0;

        public ByteSerializer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public byte[] GetBuffer() => buffer;

        void WriteTypeHeader(Serializable.Type type)
        {
            BitConverter.GetBytes((int)type).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);
        }

        Serializable.Type ReadTypeHeader()
        {
            Serializable.Type type = (Serializable.Type)BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            return type;
        }

        #region Write

        void WriteChar(char value)
        {
            WriteTypeHeader(Serializable.Type.Char);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(char);
        }

        void WriteString(string value)
        {
            WriteTypeHeader(Serializable.Type.String);

            int length = value.Length;

            BitConverter.GetBytes(length).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            Encoding.UTF8.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(char) * length;
        }

        void WriteBool(bool value)
        {
            WriteTypeHeader(Serializable.Type.Bool);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(bool);
        }

        void WriteUShort(ushort value)
        {
            WriteTypeHeader(Serializable.Type.UShort);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(ushort);
        }

        void WriteUInt(uint value)
        {
            WriteTypeHeader(Serializable.Type.UInt);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(uint);
        }

        void WriteULong(ulong value)
        {
            WriteTypeHeader(Serializable.Type.ULong);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(ulong);
        }

        void WriteShort(short value)
        {
            WriteTypeHeader(Serializable.Type.Short);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(short);
        }

        void WriteInt(int value)
        {
            WriteTypeHeader(Serializable.Type.Int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);
        }

        void WriteLong(long value)
        {
            WriteTypeHeader(Serializable.Type.Long);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(long);
        }

        void WriteFloat(float value)
        {
            WriteTypeHeader(Serializable.Type.Float);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(float);
        }

        void WriteDouble(double value)
        {
            WriteTypeHeader(Serializable.Type.Double);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(double);
        }

        #endregion

        #region Read

        char ReadChar()
        {
            char value = BitConverter.ToChar(buffer, readPosition);

            readPosition += sizeof(char);

            return value;
        }

        string ReadString()
        {
            int length = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            string value = Encoding.UTF8.GetString(buffer, readPosition, length);

            readPosition += sizeof(char) * length;

            return value;
        }

        bool ReadBool()
        {
            bool value = BitConverter.ToBoolean(buffer, readPosition);

            readPosition += sizeof(bool);

            return value;
        }

        ushort ReadUShort()
        {
            ushort value = BitConverter.ToUInt16(buffer, readPosition);

            readPosition += sizeof(ushort);

            return value;
        }

        uint ReadUInt()
        {
            uint value = BitConverter.ToUInt32(buffer, readPosition);

            readPosition += sizeof(uint);

            return value;
        }

        ulong ReadULong()
        {
            ulong value = BitConverter.ToUInt64(buffer, readPosition);

            readPosition += sizeof(ulong);

            return value;
        }

        short ReadShort()
        {
            short value = BitConverter.ToInt16(buffer, readPosition);

            readPosition += sizeof(short);

            return value;
        }

        int ReadInt()
        {
            int value = BitConverter.ToInt32(buffer, readPosition);

            readPosition += sizeof(int);

            return value;
        }

        long ReadLong()
        {
            long value = BitConverter.ToInt64(buffer, readPosition);

            readPosition += sizeof(long);

            return value;
        }

        float ReadFloat()
        {
            float value = BitConverter.ToSingle(buffer, readPosition);

            readPosition += sizeof(float);

            return value;
        }

        double ReadDouble()
        {
            double value = BitConverter.ToDouble(buffer, readPosition);

            readPosition += sizeof(double);

            return value;
        }

        #endregion


        public static ByteSerializer? Serialize<T>(T instance)
        {
            if (instance is null)
                return null;

            FieldInfo[] fields = instance.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(ByteSerializableAttribute)))
                .ToArray();

            Serializable[] serializebleFields = new Serializable[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                object? value = fields[i].GetValue(instance);
                Type type = fields[i].FieldType;

                if (value is null)
                    return null;

                serializebleFields[i] = new(value, type.ToSerializableType());
            }

            return SerializeFields(serializebleFields);
        }

        static ByteSerializer SerializeFields(Serializable[] values)
        {
            int writeSize = 0;
            foreach (Serializable value in values)
            {
                writeSize += value.size;
            }

            byte[] buffer = new byte[writeSize];

            ByteSerializer serializer = new(buffer);

            foreach (Serializable value in values)
            {
                serializer.Write(value);
            }

            return serializer;
        }

        void Write(Serializable serializable)
        {
            switch (serializable.type)
            {
                case Serializable.Type.Char: WriteChar((char)serializable.value); break;
                case Serializable.Type.String: WriteString((string)serializable.value); break;
                case Serializable.Type.Bool: WriteBool((bool)serializable.value); break;
                case Serializable.Type.Short: WriteShort((short)serializable.value); break;
                case Serializable.Type.Int: WriteInt((int)serializable.value); break;
                case Serializable.Type.Long: WriteLong((long)serializable.value); break;
                case Serializable.Type.UShort: WriteUShort((ushort)serializable.value); break;
                case Serializable.Type.UInt: WriteUInt((uint)serializable.value); break;
                case Serializable.Type.ULong: WriteULong((ulong)serializable.value); break;
                case Serializable.Type.Float: WriteFloat((float)serializable.value); break;
                case Serializable.Type.Double: WriteDouble((double)serializable.value); break;
            }
        }

        public static void Deserialize<T>(ByteSerializer serializer, T instance)
            where T : class
        {
            FieldInfo[] fields = instance.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(ByteSerializableAttribute)))
                .ToArray();

            Serializable[] serializebleFields = new Serializable[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].SetValue(instance, serializer.Read().value);
            }
        }

        Serializable Read()
        {
            Serializable.Type type = ReadTypeHeader();

            switch (type)
            {
                case Serializable.Type.Char: return ReadChar();
                case Serializable.Type.String: return ReadString();
                case Serializable.Type.Bool: return ReadBool();
                case Serializable.Type.Short: return ReadShort();
                case Serializable.Type.Int: return ReadInt();
                case Serializable.Type.Long: return ReadLong();
                case Serializable.Type.UShort: return ReadUShort();
                case Serializable.Type.UInt: return ReadUInt();
                case Serializable.Type.ULong: return ReadULong();
                case Serializable.Type.Float: return ReadFloat();
                case Serializable.Type.Double: return ReadDouble();
            }

            return Serializable.Error();
        }
    }

    public partial struct Serializable
    {
        public object value { get; private set; }
        public Type type { get; private set; }
        public int size { get; private set; }

        public enum Type
        {
            Error,
            Char,
            String,
            Bool,
            UShort,
            UInt,
            ULong,
            Short,
            Int,
            Long,
            Float,
            Double
        }

        public static Serializable Error() => new() { value = null, type = Type.Error };

        static int SizeOf(object obj, Type type)
        {
            return sizeof(int) + type switch
            {
                Type.Char => sizeof(char),
                Type.String => SizeOfString((string)obj),
                Type.Bool => sizeof(bool),
                Type.Short => sizeof(short),
                Type.Int => sizeof(int),
                Type.Long => sizeof(long),
                Type.UShort => sizeof(ushort),
                Type.UInt => sizeof(uint),
                Type.ULong => sizeof(ulong),
                Type.Float => sizeof(float),
                Type.Double => sizeof(double),
                _ => -1
            };

            int SizeOfString(string s)
            {
                return s is null ? -1 : sizeof(int) + sizeof(char) * s.Length;
            }
        }

        public Serializable(object value, Type type)
        {
            this.value = value;
            this.type = type;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(char o) => new(o);
        public static implicit operator char(Serializable o) => (char)o.value;

        public Serializable(char value)
        {
            this.value = value;
            type = Type.Char;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(string o) => new(o);
        public static implicit operator string(Serializable o) => (string)o.value;

        public Serializable(string value)
        {
            this.value = value;
            type = Type.String;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(bool o) => new(o);
        public static implicit operator bool(Serializable o) => (bool)o.value;

        public Serializable(bool value)
        {
            this.value = value;
            type = Type.Bool;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(ushort o) => new(o);
        public static implicit operator ushort(Serializable o) => (ushort)o.value;

        public Serializable(ushort value)
        {
            this.value = value;
            type = Type.UShort;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(uint o) => new(o);
        public static implicit operator uint(Serializable o) => (uint)o.value;

        public Serializable(uint value)
        {
            this.value = value;
            type = Type.UInt;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(ulong o) => new(o);
        public static implicit operator ulong(Serializable o) => (ulong)o.value;

        public Serializable(ulong value)
        {
            this.value = value;
            type = Type.ULong;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(short o) => new(o);
        public static implicit operator short(Serializable o) => (short)o.value;

        public Serializable(short value)
        {
            this.value = value;
            type = Type.Short;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(int o) => new(o);
        public static implicit operator int(Serializable o) => (int)o.value;

        public Serializable(int value)
        {
            this.value = value;
            type = Type.Int;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(long o) => new(o);
        public static implicit operator long(Serializable o) => (long)o.value;

        public Serializable(long value)
        {
            this.value = value;
            type = Type.Long;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(float o) => new(o);
        public static implicit operator float(Serializable o) => (float)o.value;

        public Serializable(float value)
        {
            this.value = value;
            type = Type.Float;

            size = SizeOf(value, type);
        }

        public static implicit operator Serializable(double o) => new(o);
        public static implicit operator double(Serializable o) => (double)o.value;

        public Serializable(double value)
        {
            this.value = value;
            type = Type.Double;

            size = SizeOf(value, type);
        }
    }

    public static class SerializableEx
    {
        public static readonly Dictionary<Type, Serializable.Type> typeTable = new()
        {
            { typeof(char), Serializable.Type.Char },
            { typeof(string), Serializable.Type.String },
            { typeof(bool), Serializable.Type.Bool },
            { typeof(short), Serializable.Type.Short },
            { typeof(int), Serializable.Type.Int },
            { typeof(long), Serializable.Type.Long },
            { typeof(ushort), Serializable.Type.UShort },
            { typeof(uint), Serializable.Type.UInt },
            { typeof(ulong), Serializable.Type.ULong },
            { typeof(float), Serializable.Type.Float },
            { typeof(double), Serializable.Type.Double },
        };

        public static Serializable.Type ToSerializableType(this Type type)
        {
            if (type is not null && typeTable.TryGetValue(type, out Serializable.Type serializableType))
                return serializableType;

            return Serializable.Type.Error;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    class ByteSerializableAttribute : Attribute
    {

    }
}
