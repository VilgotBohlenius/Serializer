using System;
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

        void WriteByteArray(byte[] value)
        {
            BitConverter.GetBytes((int)Serializable.Type.ByteArray).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            int length = value.Length;

            BitConverter.GetBytes(length).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            value.CopyTo(buffer, writePosition);
            writePosition += sizeof(byte) * length;
        }

        void WriteChar(char value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Char).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(char);
        }

        void WriteString(string value)
        {
            BitConverter.GetBytes((int)Serializable.Type.String).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            int length = value.Length;

            BitConverter.GetBytes(length).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            Encoding.UTF8.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(char) * length;
        }
        
        void WriteBool(bool value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Bool).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(bool);
        }

        void WriteUShort(ushort value)
        {
            BitConverter.GetBytes((int)Serializable.Type.UShort).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(ushort);
        }

        void WriteUInt(uint value)
        {
            BitConverter.GetBytes((int)Serializable.Type.UInt).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(uint);
        }

        void WriteULong(ulong value)
        {
            BitConverter.GetBytes((int)Serializable.Type.ULong).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(ulong);
        }

        void WriteShort(short value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Short).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(short);
        }

        void WriteInt(int value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Int).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);
        }

        void WriteLong(long value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Long).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(long);
        }

        void WriteFloat(float value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Float).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(float);
        }

        void WriteDouble(double value)
        {
            BitConverter.GetBytes((int)Serializable.Type.Double).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(double);
        }

        byte[] ReadByteArray()
        {
            int length = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            byte[] value = new byte[length];
            Array.Copy(buffer, readPosition, value, 0, length);

            return value;
        }

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

        public static ByteSerializer Write(params Serializable[] values)
        {
            int writeSize = 0;
            foreach (Serializable value in values)
            {
                writeSize += value.size;

                Console.WriteLine($"Added '{value.type}'");
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
                case Serializable.Type.ByteArray: WriteByteArray((byte[])serializable.value); return;
                case Serializable.Type.Char: WriteChar((char)serializable.value); return;
                case Serializable.Type.String: WriteString((string)serializable.value); return;
                case Serializable.Type.Bool: WriteBool((bool)serializable.value); return;
                case Serializable.Type.Short: WriteShort((short)serializable.value); return;
                case Serializable.Type.Int: WriteInt((int)serializable.value); return;
                case Serializable.Type.Long: WriteLong((long)serializable.value); return;
                case Serializable.Type.UShort: WriteUShort((ushort)serializable.value); return;
                case Serializable.Type.UInt: WriteUInt((uint)serializable.value); return;
                case Serializable.Type.ULong: WriteULong((ulong)serializable.value); return;
                case Serializable.Type.Float: WriteFloat((float)serializable.value); return;
                case Serializable.Type.Double: WriteDouble((double)serializable.value); return;
            }
        }

        public Serializable Read()
        {
            Serializable.Type type = (Serializable.Type)BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            switch (type)
            {
                case Serializable.Type.ByteArray: return ReadByteArray();
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

            return Serializable.Unknown();
        }
    }

    public struct Serializable
    {
        public object value { get; private set; }
        public Type type { get; private set; }
        public int size { get; private set; }

        public enum Type
        {
            Unknown,
            ByteArray,
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

        public static implicit operator Serializable(byte[] o) => new(o);
        public static implicit operator byte[](Serializable o) => (byte[])o.value;

        public static implicit operator Serializable(char o) => new(o);
        public static implicit operator char(Serializable o) => (char)o.value;

        public static implicit operator Serializable(string o) => new(o);
        public static implicit operator string(Serializable o) => (string)o.value;

        public static implicit operator Serializable(bool o) => new(o);
        public static implicit operator bool(Serializable o) => (bool)o.value;

        public static implicit operator Serializable(ushort o) => new(o);
        public static implicit operator ushort(Serializable o) => (ushort)o.value;

        public static implicit operator Serializable(uint o) => new(o);
        public static implicit operator uint(Serializable o) => (uint)o.value;

        public static implicit operator Serializable(ulong o) => new(o);
        public static implicit operator ulong(Serializable o) => (ulong)o.value;

        public static implicit operator Serializable(short o) => new(o);
        public static implicit operator short(Serializable o) => (short)o.value;

        public static implicit operator Serializable(int o) => new(o);
        public static implicit operator int(Serializable o) => (int)o.value;

        public static implicit operator Serializable(long o) => new(o);
        public static implicit operator long(Serializable o) => (long)o.value;

        public static implicit operator Serializable(float o) => new(o);
        public static implicit operator float(Serializable o) => (float)o.value;

        public static implicit operator Serializable(double o) => new(o);
        public static implicit operator double(Serializable o) => (double)o.value;

        public static Serializable Unknown() => new() { value = null, type = Type.Unknown };

        public Serializable(byte[] value)
        {
            this.value = value;
            type = Type.ByteArray;

            size = sizeof(int) + sizeof(int) + value.Length;
        }

        public Serializable(char value)
        {
            this.value = value;
            type = Type.Char;

            size = sizeof(int) + sizeof(char);
        }

        public Serializable(string value)
        {
            this.value = value;
            type = Type.String;

            size = sizeof(int) + sizeof(int) + sizeof(char) * value.Length;
        }

        public Serializable(bool value)
        {
            this.value = value;
            type = Type.Bool;

            size = sizeof(int) + sizeof(bool);
        }

        public Serializable(ushort value)
        {
            this.value = value;
            type = Type.UShort;

            size = sizeof(int) + sizeof(ushort);
        }

        public Serializable(uint value)
        {
            this.value = value;
            type = Type.UInt;

            size = sizeof(int) + sizeof(uint);
        }

        public Serializable(ulong value)
        {
            this.value = value;
            type = Type.ULong;

            size = sizeof(int) + sizeof(ulong);
        }

        public Serializable(short value)
        {
            this.value = value;
            type = Type.Short;

            size = sizeof(int) + sizeof(short);
        }

        public Serializable(int value)
        {
            this.value = value;
            type = Type.Int;

            size = sizeof(int) + sizeof(int);
        }

        public Serializable(long value)
        {
            this.value = value;
            type = Type.Long;

            size = sizeof(int) + sizeof(long);
        }

        public Serializable(float value)
        {
            this.value = value;
            type = Type.Float;

            size = sizeof(int) + sizeof(float);
        }

        public Serializable(double value)
        {
            this.value = value;
            type = Type.Double;

            size = sizeof(int) + sizeof(double);
        }
    }
}
