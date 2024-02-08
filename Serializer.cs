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
                if (!buffer.Write(field.GetValue(obj), field.FieldType))
                    return false;
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
                if (!buffer.Read(out object value, field.FieldType))
                    return false;

                field.SetValueDirect(reference, value);
            }

            return true;
        }
    }
}
