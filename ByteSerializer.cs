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

        public void WriteChar(char value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(char);
        }

        public void WriteString(string value)
        {
            int length = value.Length;

            BitConverter.GetBytes(length).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);

            Encoding.UTF8.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(char) * length;
        }

        public void WriteBool(bool value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(bool);
        }

        public void WriteUShort(ushort value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(ushort);
        }

        public void WriteUInt(uint value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(uint);
        }

        public void WriteULong(ulong value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(ulong);
        }

        public void WriteShort(short value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(short);
        }

        public void WriteInt(int value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(int);
        }

        public void WriteLong(long value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(long);
        }

        public void WriteFloat(float value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(float);
        }

        public void WriteDouble(double value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, writePosition);
            writePosition += sizeof(double);
        }

        public char ReadChar()
        {
            char value = BitConverter.ToChar(buffer, readPosition);

            readPosition += sizeof(char);

            return value;
        }

        public string ReadString()
        {
            int length = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            string value = Encoding.UTF8.GetString(buffer, readPosition, length);

            readPosition += sizeof(char) * length;

            return value;
        }

        public bool ReadBool()
        {
            bool value = BitConverter.ToBoolean(buffer, readPosition);

            readPosition += sizeof(bool);

            return value;
        }

        public ushort ReadUShort()
        {
            ushort value = BitConverter.ToUInt16(buffer, readPosition);

            readPosition += sizeof(ushort);

            return value;
        }

        public uint ReadUInt()
        {
            uint value = BitConverter.ToUInt32(buffer, readPosition);

            readPosition += sizeof(uint);

            return value;
        }

        public ulong ReadULong()
        {
            ulong value = BitConverter.ToUInt64(buffer, readPosition);

            readPosition += sizeof(ulong);

            return value;
        }

        public short ReadShort()
        {
            short value = BitConverter.ToInt16(buffer, readPosition);

            readPosition += sizeof(short);

            return value;
        }

        public int ReadInt()
        {
            int value = BitConverter.ToInt32(buffer, readPosition);

            readPosition += sizeof(int);

            return value;
        }

        public long ReadLong()
        {
            long value = BitConverter.ToInt64(buffer, readPosition);

            readPosition += sizeof(long);

            return value;
        }

        public float ReadFloat()
        {
            float value = BitConverter.ToSingle(buffer, readPosition);

            readPosition += sizeof(float);

            return value;
        }

        public double ReadDouble()
        {
            double value = BitConverter.ToDouble(buffer, readPosition);

            readPosition += sizeof(double);

            return value;
        }
    }
}
