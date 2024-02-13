using Microsoft.VisualBasic.FileIO;
using System;

namespace Fish.Serialization
{
    /// <summary>
    /// Utilities for serializing a c# object to a byte array
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Recursively calculates the size of an instance of an object
        /// </summary>
        /// <param name="type">The type of the object</param>
        /// <param name="obj">The instance of the object</param>
        /// <returns>The size of the instance of the object in bytes</returns>
        static int SerializedSizeOf(Type type, object obj)
        {
            return SerializedSizeOfRecursion(type, obj);

            static int SerializedSizeOfRecursion(Type type, object obj)
            {
                const byte FIELD_SIZE_HEADER_SIZE = 4;

                return type switch
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
                        size += SerializedSizeOfRecursion(field.FieldType, field.GetValue(obj));
                    }

                    return size;
                }
            }
        }

        /// <summary>
        /// Serializes a c# object to a byte array
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="instance">The instance of the object to serialize</param>
        /// <param name="outBuffer">The buffer containing the serialized object</param>
        /// <returns>True on success and false on failure</returns>
        public static bool Serialize<T>(T instance, out byte[] outBuffer, int prefix = 0, int suffix = 0)
        {
            int size = SerializedSizeOf(typeof(T), instance) + prefix + suffix;
            outBuffer = new byte[size];

            Buffer buffer = outBuffer;
            buffer.SetPosition(prefix);

            return SerializeRecursion(instance, typeof(T), buffer);

            static bool SerializeRecursion(object obj, Type type, Buffer buffer, int depth = 0)
            {
                var fields = type.GetFields();

                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    object? fieldValue = field.GetValue(obj);

                    if (fieldValue is null)
                        return false;

                    // If 'buffer.Write' returns false the type of the current field is not a base type
                    // therefore it needs to be serialized before writing
                    if (!buffer.Write(fieldValue, fieldType))
                        SerializeRecursion(fieldValue, fieldType, buffer, depth + 1);          
                }

                return true;
            }
        }

        /// <summary>
        /// Recursively deserializes a byte array into a c# object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="instance">The instance which will be created from the serialized data</param>
        /// <param name="inBuffer">The buffer to deserialize</param>
        /// <returns>True on success and false on failure</returns>
        public static bool Deserialize<T>(out T instance, byte[] inBuffer, int prefix = 0)
            where T : new()
        {
            Buffer buffer = inBuffer;
            buffer.SetPosition(prefix);

            if (!DeserializeRecursion(out object obj, typeof(T), buffer))
                return false;

            instance = (T)obj;

            return true;

            static bool DeserializeRecursion(out object obj, Type type, Buffer buffer, int depth = 0)
            {
                if (!NewInstanceOf(out obj, type))
                    return false;

                var reference = __makeref(obj);

                var fields = type.GetFields();
                
                foreach(var field in fields)
                {
                    var fieldType = field.FieldType;
                    object fieldValue;

                    // If 'buffer.Read returns' false the type of the current field is not a base type
                    // therefore it needs to be deserialized before assignement
                    if (!buffer.Read(out fieldValue, fieldType))
                        DeserializeRecursion(out fieldValue, fieldType, buffer, depth + 1);

                    field.SetValueDirect(reference, fieldValue);
                }

                return true;
            }

            /// <summary>
            /// Creates a new instance of an object
            /// </summary>
            /// <param name="obj">The new instance of the object</param>
            /// <param name="type">The type of the object</param>
            /// <returns>True on success and false on failure</returns>
            static bool NewInstanceOf(out object obj, Type type)
            {
                obj = Activator.CreateInstance(type);
                return obj is not null;
            }
        }
    }
}
