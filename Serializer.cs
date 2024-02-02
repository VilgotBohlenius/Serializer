using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Fish.Serialization
{
    public sealed class Serializer
    {
        static Encoding encoding = Encoding.Unicode;

        public byte[] buffer { get; private set; }
        int position = 0;

        static event Action<string> log;
        static event Action<string> logWarning;
        static event Action<string> logError;

        Serializer(){ }

        /// <summary>
        /// Sets the procs for log events.
        /// </summary>
        /// <param name="logProc">The normal log event.</param>
        /// <param name="logWarningProc">The warning log event.</param>
        /// <param name="logErrorProc">The error log event.</param>
        public static void SetLogProcs(Action<string> logProc, Action<string> logWarningProc, Action<string> logErrorProc){
            log = logProc;
            logWarning = logWarningProc;
            logError = logErrorProc;
        }

        /// <summary>
        /// Writes the type of the serialized object.
        /// </summary>
        /// <param name="type">The type to serialize</param>
        void WriteTypeHeader(SerializedField.SerializedType type)
        {
            BitConverter.GetBytes((int)type).CopyTo(buffer, position);
            position += sizeof(int);
        }

        /// <summary>
        /// Reads the type of the serialized object.
        /// </summary>
        SerializedField.SerializedType ReadTypeHeader()
        {
            SerializedField.SerializedType type = (SerializedField.SerializedType)BitConverter.ToInt32(buffer, position);
            position += sizeof(int);

            return type;
        }

        /// <summary>
        /// Sets the string encoding type.
        /// </summary>
        /// <param name="encoding">The encoding to use.</param>
        public static void SetStringEncoding(Encoding encoding){
            Serializer.encoding = encoding;
        }

        /// <summary>
        /// Serializes a c# object into a byte array.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="instance">The instance to serialize.</param>
        /// <param name="buffer">The buffer containing the serialized object.</param>
        /// <param name="prefix">The length of the prefix.</param>
        /// <param name="suffix">The length of the suffix.</param>
        /// <returns></returns>
        public static bool Serialize<T>(T instance, out byte[] buffer, int prefix = 0, int suffix = 0)
        {
            buffer = Array.Empty<byte>();
            
            log?.Invoke($"Trying to serialize object of type '{typeof(T)}'");

            if (instance is null)
                return false;

            if(SerializedObject.Serialize(out SerializedObject serializedObject, instance))
            {
                buffer = new byte[serializedObject.size + prefix + suffix];

                Serializer serializer = new Serializer(){
                    buffer = buffer,
                    position = prefix
                };

                foreach (SerializedField value in serializedObject.serializedFields)
                {
                    serializer.Write(value);
                }

                return true;
            }

            log?.Invoke($"Successfully serialized object of type '{typeof(T)}'");

            return false;
        }

        /// <summary>
        /// Writes the specified types value as bytes at the buffers write-position.
        /// </summary>
        /// <param name="serializedField">The serialized field to write.</param>
        void Write(SerializedField serializedField)
        {
            log?.Invoke($"  -> Writing '{serializedField.type}' of size '{serializedField.size} to buffer at position '{position}'");

            switch (serializedField.type)
            {
                case SerializedField.SerializedType.Byte: WriteByte((byte)serializedField.value); break;
                case SerializedField.SerializedType.Char: WriteChar((char)serializedField.value); break;
                case SerializedField.SerializedType.String: WriteString((string)serializedField.value); break;
                case SerializedField.SerializedType.Bool: WriteBool((bool)serializedField.value); break;
                case SerializedField.SerializedType.Short: WriteShort((short)serializedField.value); break;
                case SerializedField.SerializedType.Int: WriteInt((int)serializedField.value); break;
                case SerializedField.SerializedType.Long: WriteLong((long)serializedField.value); break;
                case SerializedField.SerializedType.UShort: WriteUShort((ushort)serializedField.value); break;
                case SerializedField.SerializedType.UInt: WriteUInt((uint)serializedField.value); break;
                case SerializedField.SerializedType.ULong: WriteULong((ulong)serializedField.value); break;
                case SerializedField.SerializedType.Float: WriteFloat((float)serializedField.value); break;
                case SerializedField.SerializedType.Double: WriteDouble((double)serializedField.value); break;
            }

            void WriteByte(byte value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Byte);

                buffer[position] = value;
                position += sizeof(byte);
            }

            void WriteChar(char value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Char);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(char);
            }

            void WriteString(string value)
            {
                WriteTypeHeader(SerializedField.SerializedType.String);

                int length = encoding.GetByteCount(value, 0, value.Length);

                BitConverter.GetBytes(length).CopyTo(buffer, position);
                position += sizeof(int);

                encoding.GetBytes(value).CopyTo(buffer, position);
                position += length;
            }

            void WriteBool(bool value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Bool);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(bool);
            }

            void WriteUShort(ushort value)
            {
                WriteTypeHeader(SerializedField.SerializedType.UShort);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(ushort);
            }

            void WriteUInt(uint value)
            {
                WriteTypeHeader(SerializedField.SerializedType.UInt);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(uint);
            }

            void WriteULong(ulong value)
            {
                WriteTypeHeader(SerializedField.SerializedType.ULong);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(ulong);
            }

            void WriteShort(short value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Short);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(short);
            }

            void WriteInt(int value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Int);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(int);
            }

            void WriteLong(long value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Long);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(long);
            }

            void WriteFloat(float value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Float);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(float);
            }

            void WriteDouble(double value)
            {
                WriteTypeHeader(SerializedField.SerializedType.Double);

                BitConverter.GetBytes(value).CopyTo(buffer, position);
                position += sizeof(double);
            }
        }

        /// <summary>
        /// Deserializes a byte array into a c# object.
        /// </summary>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="instance">An instance to the object to deserialize.</param>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="position">The position in the buffer where the serializer will start from.</param>
        public static bool Deserialize<T>(ref T instance, byte [] buffer, int position = 0)
        {
            log?.Invoke($"Trying to deserialize object of type '{typeof(T)}'");

            Serializer serializer = new Serializer()
            {
                buffer = buffer,
                position = position
            };

            FieldInfo[] fields = SerializerEx.GetAllFieldsDecoratedWithAttribute<T, SerializableAttribute>();

            if(fields.Length == 0){
                logError?.Invoke($"  -> Failed to deserialize object, no fields to deserialize.");

                return false;
            }

            try
            {
                foreach (FieldInfo field in fields)
                {
                    log?.Invoke($"  -> Trying to deserialize field of type '{field.FieldType}'");

                    object value = serializer.Read();
                    if(value is null){
                        logError?.Invoke($"  -> Value of field '{field.Name}' is null, unable to deserialize");
                        return false;
                    }

                    field.SetValue(instance, value);

                    log?.Invoke($"  -> Succesfully deserialized field of type '{field.FieldType}' with value '{field.GetValue(instance)}'");
                }
            }
            catch
            {
                logError?.Invoke($"  -> Exception encountered during deserialization");

                return false;
            }

            log?.Invoke($"Succesfully deserialized object of type '{typeof(T)}'");

            return true;
        }

        /// <summary>
        /// Reads from the buffer at the current read-position.
        /// </summary>
        /// <returns>The object read from the buffer.</returns>
        object Read()
        {
            log?.Invoke($"  -> Trying to read field");

            SerializedField.SerializedType type = ReadTypeHeader();

            switch (type)
            {
                case SerializedField.SerializedType.Byte: return ReadByte();
                case SerializedField.SerializedType.Char: return ReadChar();
                case SerializedField.SerializedType.String: return ReadString();
                case SerializedField.SerializedType.Bool: return ReadBool();
                case SerializedField.SerializedType.Short: return ReadShort();
                case SerializedField.SerializedType.Int: return ReadInt();
                case SerializedField.SerializedType.Long: return ReadLong();
                case SerializedField.SerializedType.UShort: return ReadUShort();
                case SerializedField.SerializedType.UInt: return ReadUInt();
                case SerializedField.SerializedType.ULong: return ReadULong();
                case SerializedField.SerializedType.Float: return ReadFloat();
                case SerializedField.SerializedType.Double: return ReadDouble();
            }

            logError?.Invoke($"  -> Failed while reading field, invalid field type id '{(int)type}'");

            return null;

            byte ReadByte()
            {
                byte value = buffer[position];

                position += sizeof(byte);

                return value;
            }

            char ReadChar()
            {
                char value = BitConverter.ToChar(buffer, position);

                position += sizeof(char);

                return value;
            }

            string ReadString()
            {
                int length = BitConverter.ToInt32(buffer, position);
                position += sizeof(int);

                string value = encoding.GetString(buffer, position, length);

                position += length;

                return value;
            }

            bool ReadBool()
            {
                bool value = BitConverter.ToBoolean(buffer, position);

                position += sizeof(bool);

                return value;
            }

            ushort ReadUShort()
            {
                ushort value = BitConverter.ToUInt16(buffer, position);

                position += sizeof(ushort);

                return value;
            }

            uint ReadUInt()
            {
                uint value = BitConverter.ToUInt32(buffer, position);

                position += sizeof(uint);

                return value;
            }

            ulong ReadULong()
            {
                ulong value = BitConverter.ToUInt64(buffer, position);

                position += sizeof(ulong);

                return value;
            }

            short ReadShort()
            {
                short value = BitConverter.ToInt16(buffer, position);

                position += sizeof(short);

                return value;
            }

            int ReadInt()
            {
                int value = BitConverter.ToInt32(buffer, position);

                position += sizeof(int);

                return value;
            }

            long ReadLong()
            {
                long value = BitConverter.ToInt64(buffer, position);

                position += sizeof(long);

                return value;
            }

            float ReadFloat()
            {
                float value = BitConverter.ToSingle(buffer, position);

                position += sizeof(float);

                return value;
            }

            double ReadDouble()
            {
                double value = BitConverter.ToDouble(buffer, position);

                position += sizeof(double);

                return value;
            }
        }

        /// <summary>
        /// Represents a c# class as a serialized object.
        /// </summary>
        struct SerializedObject
        {
            public SerializedField[] serializedFields { get; private set; }
            public int size { get; private set; }

            public SerializedObject(int a = 0)
            {
                serializedFields = Array.Empty<SerializedField>();
                size = 0;
            }

            public static bool Serialize<T>(out SerializedObject serializedObject, T instance)
            {
                log?.Invoke($"Trying to serialize object '{typeof(T)}' into serialized object");

                serializedObject = new();

                if (instance is null){
                    logError?.Invoke($"  -> Failed to serialize object '{typeof(T)}' into serialized object");

                    return false;
                }

                FieldInfo[] fields = SerializerEx.GetAllFieldsDecoratedWithAttribute<T, SerializableAttribute>();

                serializedObject.serializedFields = new SerializedField[fields.Length];

                try
                {
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value = fields[i].GetValue(instance);
                        Type type = fields[i].FieldType;

                        log?.Invoke($"  -> Trying to serialize field '{i}' of type '{type}' with value '{value}' into a serializedField");

                        if (value is null){
                            logError?.Invoke($"  -> Failed to serialize field '{i}' of type '{type}' with value '{value}' into a serializedField");

                            return false;
                        }

                        serializedObject.serializedFields[i] = new(value, SerializedField.ToSerializableType(type));
                        serializedObject.size += serializedObject.serializedFields[i].size;
                    }
                }
                catch
                {
                    log?.Invoke($"  -> Encountered exception while serializing object into serialized object");

                    return false;
                }

                log?.Invoke($"Successfully serialized type '{instance.GetType()}' into a serializedObject");

                return true;
            }
        }

        /// <summary>
        /// Represents a c# object type as a serialized object.
        /// </summary>
        struct SerializedField
        {
            public object value { get; private set; }
            public SerializedType type { get; private set; }
            public int size { get; private set; }

            public enum SerializedType
            {
                Error,
                Byte,
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

            static readonly Dictionary<Type, SerializedType> typeTable = new()
            {
                { typeof(byte), SerializedType.Byte },
                { typeof(char), SerializedType.Char },
                { typeof(string), SerializedType.String },
                { typeof(bool), SerializedType.Bool },
                { typeof(short), SerializedType.Short },
                { typeof(int), SerializedType.Int },
                { typeof(long), SerializedType.Long },
                { typeof(ushort), SerializedType.UShort },
                { typeof(uint), SerializedType.UInt },
                { typeof(ulong), SerializedType.ULong },
                { typeof(float), SerializedType.Float },
                { typeof(double), SerializedType.Double },
            };

            public static SerializedType ToSerializableType(Type type)
            {
                if (type is not null && typeTable.TryGetValue(type, out SerializedType serializableType))
                    return serializableType;

                return SerializedType.Error;
            }

            /// <summary>
            /// Uses object and type info to calculate the serialized size
            /// </summary>
            /// <param name="obj">The object to get the size of</param>
            /// <param name="type">The serializable type to get the size from</param>
            /// <returns>The serialized size of the object in bytes</returns>
            static int SizeOf(object obj, SerializedType type)
            {
                return sizeof(int) + type switch
                {
                    SerializedType.Byte => sizeof(byte),
                    SerializedType.Char => sizeof(char),
                    SerializedType.String => SizeOfString((string)obj),
                    SerializedType.Bool => sizeof(bool),
                    SerializedType.UShort => sizeof(ushort),
                    SerializedType.UInt => sizeof(uint),
                    SerializedType.ULong => sizeof(ulong),
                    SerializedType.Short => sizeof(short),
                    SerializedType.Int => sizeof(int),
                    SerializedType.Long => sizeof(long),
                    SerializedType.Float => sizeof(float),
                    SerializedType.Double => sizeof(double),
                    _ => -1
                };

                int SizeOfString(string s)
                {
                    return s is null ? -1 : sizeof(int) + sizeof(char) * s.Length;
                }
            }

            public SerializedField(object value, SerializedType type)
            {
                this.value = value;
                this.type = type;

                size = SizeOf(value, type);
            }
        }
    }

    /// <summary>
    /// Extension and utility functions for serialization.
    /// </summary>
    public static class SerializerEx
    {
        public static FieldInfo[] GetAllFieldsDecoratedWithAttribute<T0, T1>()
            where T1 : Attribute
        {
            return typeof(T0)
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(T1)))
                .ToArray();
        }
    }

    /// <summary>
    /// Tag indicating that the field should be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    class SerializableAttribute : Attribute{ }
}
