using System;
using System.Security.Cryptography;

namespace iiFramework.Util
{
    public sealed class GuidHelper
    {
        /// <summary>
        /// 获取有序的唯一ID。
        /// </summary>
        /// <returns></returns>
        public static Guid GenerateComb(SequentialGuidType sequentialGuidType = SequentialGuidType.SequentialAtEnd)
        {
            return SequentialGuidGenerator.NewSequentialGuid(sequentialGuidType);
        }

        /// <summary>
        /// 根据枚举生成不同的有序GUID
        /// http://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database
        /// </summary>
        private static class SequentialGuidGenerator
        {
            private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

            public static Guid NewSequentialGuid(SequentialGuidType guidType)
            {
                var randomBytes = new byte[10];
                Rng.GetBytes(randomBytes);

                var timestamp = DateTime.UtcNow.Ticks / 10000L;
                var timestampBytes = BitConverter.GetBytes(timestamp);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(timestampBytes);
                }

                var guidBytes = new byte[16];

                switch (guidType)
                {
                    case SequentialGuidType.SequentialAsString:
                    case SequentialGuidType.SequentialAsBinary:
                        Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                        Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                        // If formatting as a string, we have to reverse the order
                        // of the Data1 and Data2 blocks on little-endian systems.
                        if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(guidBytes, 0, 4);
                            Array.Reverse(guidBytes, 4, 2);
                        }
                        break;
                    case SequentialGuidType.SequentialAtEnd:
                        Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                        Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("guidType", guidType, null);
                }

                return new Guid(guidBytes);
            }
        }

        /// <summary>
        /// 有序GUID的类型（sqlServer用AtEnd，mysql用AsString或者AsBinary，oracle用AsBinary，postgresql用AsString或者AsBinary）
        /// </summary>
        public enum SequentialGuidType
        {
            SequentialAsString,
            SequentialAsBinary,
            SequentialAtEnd
        }

        /// <summary>
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>
        public static long GuidToInt64()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// 根据已有GUID获取19位的唯一数字序列 
        /// </summary>
        public static long GuidToInt64(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary> 
        /// 生成有序GUID，Generate a new <see cref="Guid"/> using the comb algorithm. 
        /// </summary> 
        public static Guid GenerateCombGUID()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;

            // Get the days and milliseconds which will be used to build    
            //the byte string    
            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = now.TimeOfDay;

            // Convert to a byte array        
            // Note that SQL Server is accurate to 1/300th of a    
            // millisecond so we divide by 3.333333    
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)
              (msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering    
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid start
            Array.Copy(daysArray, daysArray.Length - 2, guidArray,
              0, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray,
              2, 4);

            // Copy the bytes into the guid end
            //Array.Copy(daysArray, daysArray.Length - 2, guidArray,
            //  guidArray.Length - 6, 2);
            //Array.Copy(msecsArray, msecsArray.Length - 4, guidArray,
            //  guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }


        /// <summary>
        /// 生成最大10位的唯一字符串
        /// </summary>
        /// <returns></returns>
        public static string GuidToUint32()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToUInt32(buffer, 8).ToString();
        }

        /// <summary>
        /// 唯一订单号生成
        /// </summary>
        /// <returns></returns>
        public static string GenerateOrderNumber()
        {
            string strDateTimeNumber = DateTime.Now.ToString("yyyyMMddHHmmssms");
            string strRandomResult = NextRandom(1000, 1).ToString();
            return strDateTimeNumber + strRandomResult;
        }
        /// <summary>
        /// 参考：msdn上的RNGCryptoServiceProvider例子
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int NextRandom(int numSeeds, int length)
        {
            // Create a byte array to hold the random value.  
            byte[] randomNumber = new byte[length];
            // Create a new instance of the RNGCryptoServiceProvider.  
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.  
            rng.GetBytes(randomNumber);
            // Convert the byte to an uint value to make the modulus operation easier.  
            uint randomResult = 0x0;
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)randomNumber[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds) + 1;
        }

    }
}
