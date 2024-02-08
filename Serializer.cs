using System;

namespace Fish.Serialization
{
    /// <summary>
    /// Utilities for serializing a c# object to a byte array
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// A serialized type represents a c# type as a byte id
        /// </summary>
        enum SerializedType : byte
        {
            Unknown,
            Byte,
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong,
            Float,
            Double,
            String
        }

        /// <summary>
        /// Converts a c# type to a serialized type
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns>The serialized type</returns>
        static SerializedType SerializedTypeOf(Type type)
        {
            return type switch
            {
                var t when t == typeof(byte) => SerializedType.Byte,
                var t when t == typeof(short) => SerializedType.Short,
                var t when t == typeof(ushort) => SerializedType.UShort,
                var t when t == typeof(int) => SerializedType.Int,
                var t when t == typeof(uint) => SerializedType.UInt,
                var t when t == typeof(long) => SerializedType.Long,
                var t when t == typeof(ulong) => SerializedType.ULong,
                var t when t == typeof(float) => SerializedType.Float,
                var t when t == typeof(double) => SerializedType.Double,
                var t when t == typeof(string) => SerializedType.String,
                _ => SerializedType.Unknown
            };
        }

        /// <summary>
        /// Recursively calculates the size of an instance of an object
        /// </summary>
        /// <param name="type">The type of the object</param>
        /// <param name="obj">The instance of the object</param>
        /// <returns>The size of the instance of the object in bytes</returns>
        static int SerializedSizeOf(Type type, object obj)
        {
            // Doesnt check for infinite recursion, should fix asap

            const byte FIELD_TYPE_HEADER_SIZE = 1;
            const byte FIELD_SIZE_HEADER_SIZE = 4;

            return FIELD_TYPE_HEADER_SIZE + type switch
            {
                var t when t == typeof(byte) => sizeof(byte),
                var t when t == typeof(short) => sizeof(short),
                var t when t == typeof(ushort) => sizeof(ushort),
                var t when t == typeof(int) => sizeof(int),
                var t when t == typeof(uint) => sizeof(uint),
                var t when t == typeof(long) => sizeof(long),
                var t when t == typeof(ulong) => sizeof(ulong),
                var t when t == typeof(float) => sizeof(float),
                var t when t == typeof(double) => sizeof(double),
                var t when t == typeof(string) => SizeOfString((string)obj),
                _ => SizeOfObject(type, obj)
            };

            int SizeOfString(string str)
            {
                return FIELD_SIZE_HEADER_SIZE + Buffer.encoding.GetByteCount(str);
            }

            int SizeOfObject(Type type, object obj)
            {
                int size = 0;

                var fields = type.GetFields();

                foreach (var field in fields)
                {
                    size += SerializedSizeOf(field.FieldType, field.GetValue(obj));
                }

                return size;
            }
        }

        /// <summary>
        /// Serializes a c# object to a byte array
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The instance of the object to serialize</param>
        /// <param name="outBuffer">The buffer containing the serialized object</param>
        /// <returns>True on success and false on failure</returns>
        public static bool Serialize<T>(T obj, out byte[] outBuffer, int prefix = 0, int suffix = 0)
        {
            int outBufferSize = SerializedSizeOf(typeof(T), obj) + prefix + suffix;
            outBuffer = new byte[outBufferSize];

            Buffer buffer = outBuffer;
            buffer.SetPosition(prefix);

            var fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                buffer.Write((byte)SerializedTypeOf(field.FieldType));
                buffer.Write(field.GetValue(obj), field.FieldType);
            }

            return true;
        }

        /// <summary>
        /// Deserializes a byte array into a c# object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object to hold the deserialized data</param>
        /// <param name="inBuffer">The buffer to Deserialize</param>
        /// <returns>True on success and false on failure</returns>
        public static bool Deserialize<T>(out T obj, byte[] inBuffer, int prefix = 0)
            where T : new()
        {
            obj = new();

            var reference = __makeref(obj);

            Buffer buffer = inBuffer;
            buffer.SetPosition(prefix);

            var fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                buffer.Read(out byte serializedTypeId);

                switch ((SerializedType)serializedTypeId)
                {
                    case SerializedType.Unknown:
                        {
                            return false;
                        }
                    case SerializedType.Byte:
                        {
                            buffer.Read(out byte value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.Short:
                        {
                            buffer.Read(out short value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.UShort:
                        {
                            buffer.Read(out ushort value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.Int:
                        {
                            buffer.Read(out int value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.UInt:
                        {
                            buffer.Read(out uint value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.Long:
                        {
                            buffer.Read(out long value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.ULong:
                        {
                            buffer.Read(out ulong value);
                            field.SetValueDirect(reference, value);
                        }
                        break;

                    case SerializedType.Float:
                        {
                            buffer.Read(out float value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.Double:
                        {
                            buffer.Read(out double value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                    case SerializedType.String:
                        {
                            buffer.Read(out string value);
                            field.SetValueDirect(reference, value);
                        }
                        break;
                }
            }

            return true;
        }
    }
}
