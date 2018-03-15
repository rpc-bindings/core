using System;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Common.Serialization
{
    public static class CefValueExtensions
    {
        private static readonly DateTime DateTime = new DateTime(1970, 1, 1).ToUniversalTime();

        public static bool IsType(this CefValue @this, CefTypes type)
        {
            if (@this.GetValueType() != CefValueType.Binary)
                return false;

            using (var cefBinaryValue = @this.GetBinary())
            {
                var buffer = new byte[1];
                cefBinaryValue.GetData(buffer, 1, 0);

                return type == (CefTypes) buffer[0];
            }
        }

        public static void SetTime(this CefValue @this, DateTime value)
        {
            var totalSecondsBytes = BitConverter.GetBytes(value.ToBinary());
            var buffer = new byte[totalSecondsBytes.Length + 1];
            buffer[0] = (byte) CefTypes.Time;
            Array.Copy(totalSecondsBytes, 0, buffer, 1, totalSecondsBytes.Length);

            using (var binaryValue = CefBinaryValue.Create(buffer))
            {
                @this.SetBinary(binaryValue);
            }
        }

        public static DateTime GetTime(this CefValue @this)
        {
            if (@this.GetValueType() != CefValueType.Binary)
                return default(DateTime);

            using (var binaryValue = @this.GetBinary())
            {
                var buffer = new byte[binaryValue.Size];
                binaryValue.GetData(buffer, binaryValue.Size, 0);

                return DateTime.FromBinary(BitConverter.ToInt64(buffer, 1));
            }
        }

        public static long GetInt64(this CefValue @this)
        {
            if (@this.GetValueType() != CefValueType.Binary)
                return 0L;

            using (var binaryValue = @this.GetBinary())
            {
                var buffer = new byte[binaryValue.Size];
                binaryValue.GetData(buffer, binaryValue.Size, 0);

                return BitConverter.ToInt64(buffer, 1);
            }
        }

        public static void SetInt64(this CefValue @this, long value)
        {
            var buffer= new byte[sizeof(long) + 1];
            buffer[0] = (byte) CefTypes.Int64;
            var int64Bytes = BitConverter.GetBytes(value);
            Array.Copy(int64Bytes, 0, buffer, 1, int64Bytes.Length);

            using (var binaryValue = CefBinaryValue.Create(buffer))
            {
                @this.SetBinary(binaryValue);
            }
        }
    }
}
