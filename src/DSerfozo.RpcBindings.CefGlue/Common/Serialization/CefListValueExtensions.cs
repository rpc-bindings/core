using System;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Common.Serialization
{
    public static class CefListValueExtensions
    {
        public static bool IsType(this CefListValue @this, int index, CefTypes type)
        {
            if (@this.GetValueType(index) != CefValueType.Binary)
                return false;

            using (var cefBinaryValue = @this.GetBinary(index))
            {
                var buffer = new byte[1];
                cefBinaryValue.GetData(buffer, 1, 0);

                return type == (CefTypes) buffer[0];
            }
        }

        public static void SetInt64(this CefListValue @this, int index, long value)
        {
            var buffer = new byte[sizeof(long) + 1];
            buffer[0] = (byte)CefTypes.Int64;
            var int64Bytes = BitConverter.GetBytes(value);
            Array.Copy(int64Bytes, 0, buffer, 1, int64Bytes.Length);

            using (var binaryValue = CefBinaryValue.Create(buffer))
            {
                @this.SetBinary(index, binaryValue);
            }
        }

        public static long GetInt64(this CefListValue @this, int index)
        {
            if (@this.GetValueType(index) != CefValueType.Binary)
                return 0L;

            using (var binaryValue = @this.GetBinary(index))
            {
                var buffer = new byte[binaryValue.Size];
                binaryValue.GetData(buffer, binaryValue.Size, 0);

                return BitConverter.ToInt64(buffer, 1);
            }
        }
    }
}
