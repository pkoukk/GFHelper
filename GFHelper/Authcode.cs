namespace GFHelper
{
    using ICSharpCode.SharpZipLib.GZip;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    public class AuthCode
    {
        private static Encoding encoding = Encoding.UTF8;
        public static IntDelegate GetCurrentTimeStampMethod;
        private static int timeOffset;

        private static string AscArr2Str(byte[] b) =>
            Encoding.Unicode.GetString(Encoding.Convert(Encoding.ASCII, Encoding.Unicode, b));

        private static string Authcode(string source, string key, AuthcodeMode operation, int expiry)
        {
            if ((source == null) || (key == null))
            {
                return "";
            }
            int length = 0;
            key = MD5(key);
            string str = MD5(CutString(key, 0x10, 0x10));
            string str2 = MD5(CutString(key, 0, 0x10));
            string str3 = (length > 0) ? ((operation == AuthcodeMode.Decode) ? CutString(source, 0, length) : RandomString(length)) : "";
            string pass = str + MD5(str + str3);
            if (operation == AuthcodeMode.Decode)
            {
                byte[] buffer;
                try
                {
                    buffer = Convert.FromBase64String(CutString(source, length));
                }
                catch
                {
                    try
                    {
                        buffer = Convert.FromBase64String(CutString(source + "=", length));
                    }
                    catch
                    {
                        try
                        {
                            buffer = Convert.FromBase64String(CutString(source + "==", length));
                        }
                        catch
                        {
                            return "";
                        }
                    }
                }
                string str5 = encoding.GetString(RC4(buffer, pass));
                long num2 = long.Parse(CutString(str5, 0, 10));
                if (((num2 == 0) || ((num2 - GetCurrentTimeStamp()) > 0L)) && (CutString(str5, 10, 0x10) == CutString(MD5(CutString(str5, 0x1a) + str2), 0, 0x10)))
                {
                    return CutString(str5, 0x1a);
                }
                return "";
            }
            source = ((expiry == 0) ? "0000000000" : ((expiry + GetCurrentTimeStamp())).ToString()) + CutString(MD5(source + str2), 0, 0x10) + source;
            byte[] inArray = RC4(encoding.GetBytes(source), pass);
            return (str3 + Convert.ToBase64String(inArray));
        }

        public static string AutoDecode(string source,string key)
        {
            if (source.StartsWith("#"))
            {
                using (MemoryStream stream = new MemoryStream(AuthCode.DecodeWithGzip(source.Substring(1), key)))
                {
                    using (Stream stream2 = new GZipInputStream(stream))
                    {
                        using (StreamReader reader = new StreamReader(stream2, Encoding.Default))
                        {
                            string str = reader.ReadToEnd();
                            return str;
                        }
                    }
                }
            }
            else
            {
                return Decode(source,key);
            }
        }
        private static string CutString(string str, int startIndex) =>
            CutString(str, startIndex, str.Length);

        private static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length *= -1;
                    if ((startIndex - length) < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex -= length;
                    }
                }
                if (startIndex > str.Length)
                {
                    return "";
                }
            }
            else if ((length >= 0) && ((length + startIndex) > 0))
            {
                length += startIndex;
                startIndex = 0;
            }
            else
            {
                return "";
            }
            if ((str.Length - startIndex) < length)
            {
                length = str.Length - startIndex;
            }
            return str.Substring(startIndex, length);
        }

        public static string Decode(string source, string key) =>
            Authcode(source, key, AuthcodeMode.Decode, 0xe10);

        public static byte[] DecodeWithGzip(string source, string key) =>
            DecodeWithGzip(source, key, 0xe10);

        public static byte[] DecodeWithGzip(string source, string key, int expiry)
        {
            if ((source != null) && (key != null))
            {
                byte[] buffer;
                int length = 0;
                key = MD5(key);
                string str = MD5(CutString(key, 0x10, 0x10));
                string s = MD5(CutString(key, 0, 0x10));
                string str3 = (length > 0) ? CutString(source, 0, length) : "";
                string pass = str + MD5(str + str3);
                try
                {
                    buffer = Convert.FromBase64String(CutString(source, length));
                }
                catch
                {
                    try
                    {
                        buffer = Convert.FromBase64String(CutString(source + "=", length));
                    }
                    catch
                    {
                        try
                        {
                            buffer = Convert.FromBase64String(CutString(source + "==", length));
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                byte[] bytes = RC4(buffer, pass);
                string str5 = encoding.GetString(bytes);
                long num2 = long.Parse(CutString(str5, 0, 10));
                byte[] destinationArray = new byte[bytes.Length - 0x1a];
                Array.Copy(bytes, 0x1a, destinationArray, 0, bytes.Length - 0x1a);
                byte[] buffer4 = new byte[(bytes.Length - 0x1a) + s.Length];
                Array.Copy(destinationArray, 0, buffer4, 0, destinationArray.Length);
                Array.Copy(encoding.GetBytes(s), 0, buffer4, destinationArray.Length, s.Length);
                if (((num2 == 0) || ((num2 - GetCurrentTimeStamp()) > 0L)) && (CutString(str5, 10, 0x10) == CutString(MD5(buffer4), 0, 0x10)))
                {
                    return destinationArray;
                }
            }
            return null;
        }

        public static string Encode(string source, string key) =>
            Authcode(source, key, AuthcodeMode.Encode, 0xe10);

        private static long GetCurrentTimeStamp() =>
            (long)(GetCurrentTimeStampMethod() / 1000);

        private static byte[] GetKey(byte[] pass, int kLen)
        {
            byte[] buffer = new byte[kLen];
            for (long i = 0L; i < kLen; i += 1L)
            {
                buffer[(int)((IntPtr)i)] = (byte)i;
            }
            long num = 0L;
            for (long j = 0L; j < kLen; j += 1L)
            {
                num = ((num + buffer[(int)((IntPtr)j)]) + pass[(int)((IntPtr)(j % ((long)pass.Length)))]) % ((long)kLen);
                byte num4 = buffer[(int)((IntPtr)j)];
                buffer[(int)((IntPtr)j)] = buffer[(int)((IntPtr)num)];
                buffer[(int)((IntPtr)num)] = num4;
            }
            return buffer;
        }

        public static void Init(IntDelegate method)
        {
            GetCurrentTimeStampMethod = method;
        }

        public static void InitTimeData(int realtime, int loginTime)
        {
            timeOffset = loginTime - realtime;
        }

        public static string MD5(string str) =>
            MD5(encoding.GetBytes(str));

        public static string MD5(byte[] b)
        {
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string str = "";
            for (int i = 0; i < b.Length; i++)
            {
                str = str + b[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        public static string RandomString(int lens)
        {
            char[] chArray = new char[] {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
                'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
                'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            int length = chArray.Length;
            string str = "";
            Random random = new Random();
            for (int i = 0; i < lens; i++)
            {
                str = str + chArray[random.Next(length)].ToString();
            }
            return str;
        }

        private static byte[] RC4(byte[] input, string pass)
        {
            if ((input == null) || (pass == null))
            {
                return null;
            }
            byte[] buffer = new byte[input.Length];
            byte[] key = GetKey(encoding.GetBytes(pass), 0x100);
            long num = 0L;
            long num2 = 0L;
            for (long i = 0L; i < input.Length; i += 1L)
            {
                num = (num + 1L) % ((long)key.Length);
                num2 = (num2 + key[(int)((IntPtr)num)]) % ((long)key.Length);
                byte num4 = key[(int)((IntPtr)num)];
                key[(int)((IntPtr)num)] = key[(int)((IntPtr)num2)];
                key[(int)((IntPtr)num2)] = num4;
                byte num5 = input[(int)((IntPtr)i)];
                byte num6 = key[(key[(int)((IntPtr)num)] + key[(int)((IntPtr)num2)]) % key.Length];
                buffer[(int)((IntPtr)i)] = (byte)(num5 ^ num6);
            }
            return buffer;
        }

        public static long time()
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan span = new TimeSpan(DateTime.UtcNow.Ticks - time.Ticks);
            return (long)span.TotalMilliseconds;
        }

        public static string urlencode(string str)
        {
            string str2 = string.Empty;
            string str3 = "_-.1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < str.Length; i++)
            {
                string str4 = str.Substring(i, 1);
                if (str3.Contains(str4))
                {
                    str2 = str2 + str4;
                }
                else
                {
                    byte[] bytes = encoding.GetBytes(str4);
                    foreach (byte num3 in bytes)
                    {
                        str2 = str2 + "%" + num3.ToString("X");
                    }
                }
            }
            return str2;
        }

        private enum AuthcodeMode
        {
            Encode,
            Decode
        }

        public delegate int IntDelegate();
    }
}

