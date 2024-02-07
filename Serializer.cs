using Fish.Buffer

namespace Fish.Serialization
{
    /// <summary>
    /// Utilities for serializing a c# object to a byte array
    /// </summary>
    public class Serializer
    {
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
    
            return type switch
            {
                var t when t == typeof(byte) => 1,
                var t when t == typeof(short) => 2,
                var t when t == typeof(ushort) => 2,
                var t when t == typeof(int) => 4,
                var t when t == typeof(uint) => 4,
                var t when t == typeof(long) => 8,
                var t when t == typeof(ulong) => 8,
                var t when t == typeof(float) => 4,
                var t when t == typeof(double) => 8,
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
    
                foreach(var field in fields)
                {
                    size += FIELD_TYPE_HEADER_SIZE + SerializedSizeOf(field.FieldType, field.GetValue(obj));
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
        public static bool Serialize<T>(T obj, out byte[] outBuffer)
        {
            outBuffer = new byte[SerializedSizeOf(typeof(T), obj)];
    
            Buffer buffer = outBuffer;
    
            var fields = typeof(T).GetFields();
    
            foreach(var field in fields)
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
        public static bool Deserialize<T>(out T obj, byte[] inBuffer)
            where T : new()
        {
            obj = new();
    
            Buffer buffer = inBuffer;
    
            var fields = typeof(T).GetFields();
    
            foreach(var field in fields)
            {
                buffer.Read(out byte serializedTypeId);
    
                switch ((SerializedType)serializedTypeId)
                {
                    case var t when t == SerializedType.Byte:
                        {
                            buffer.Read(out byte value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.Short:
                        {
                            buffer.Read(out short value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.UShort:
                        {
                            buffer.Read(out ushort value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.Int:
                        {
                            buffer.Read(out int value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.UInt:
                        {
                            buffer.Read(out uint value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.Long:
                        {
                            buffer.Read(out long value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.ULong:
                        {
                            buffer.Read(out ulong value);
                            field.SetValue(obj, value);
                        }
                        break;
    
                    case var t when t == SerializedType.Float:
                        {
                            buffer.Read(out float value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.Double:
                        {
                            buffer.Read(out double value);
                            field.SetValue(obj, value);
                        }
                        break;
                    case var t when t == SerializedType.String:
                        {
                            buffer.Read(out string value);
                            field.SetValue(obj, value);
                        }
                        break;
                }
            }
    
            return true;
        }
    }
}