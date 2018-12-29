using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GFHelper
{

    public class XXTEA
    {
        public XXTEA()
        {

        }

        public static byte[] decrypt(byte[] arg3, byte[] arg4)
        {
            if (arg3.Length != 0)
            {
                arg3 = XXTEA.toByteArray(XXTEA.decrypt(XXTEA.toIntArray(arg3, false), XXTEA.toIntArray(arg4, false)), false);
            }

            return arg3;
        }

        public static int[] decrypt(int[] arg13, int[] arg14)
        {
            int v2 = arg13.Length;
            int v0 = -1640531527;
            int v5 = (52 / v2 + 6) * v0;
            int v6 = arg13[0];
            do
            {
                int v1 = moveRight(v5,2) & 3;
                int v3;
                for (v3 = v2 - 1; v3 > 0; --v3)
                {
                    v6 = arg13[v3] - ((moveRight(arg13[v3 - 1], 5) ^ v6 << 2) + (moveRight(v6, 3) ^ arg13[v3 - 1] << 4) ^ (v5 ^ v6) + (arg14[v3 & 3 ^ v1] ^ arg13[v3 - 1]));
                    arg13[v3] = v6;
                }

                v6 = arg13[0] - ((moveRight(arg13[v2 - 1], 5) ^ v6 << 2) + (moveRight(v6, 3) ^ arg13[v2 - 1] << 4) ^ (v5 ^ v6) + (arg14[v3 & 3 ^ v1] ^ arg13[v2 - 1]));
                arg13[0] = v6;
                v5 -= v0;
            }
            while (v5 != 0);

            return arg13;
        }

        public static byte[] encrypt(byte[] arg3, byte[] arg4)
        {
            if (arg3.Length != 0)
            {
                arg3 = XXTEA.toByteArray(XXTEA.encrypt(XXTEA.toIntArray(arg3, false), XXTEA.toIntArray(arg4, false)), false);
            }

            return arg3;
        }

        public static int[] encrypt(int[] arg13, int[] arg14)
        {
            int v2 = arg13.Length;
            int v4 = 52 / v2 + 6;
            int v5 = 0;
            int v7 = arg13[v2 - 1];
            int v0 = -1640531527;
            do
            {
                v5 += v0;
                int v1 = moveRight(v5, 2) & 3;
                int v3;
                for (v3 = 0; v3 < v2 - 1; ++v3)
                {
                    v7 = arg13[v3] + ((moveRight(v7, 5) ^ arg13[v3 + 1] << 2) + (moveRight(arg13[v3 + 1], 3) ^ v7 << 4) ^ (v5 ^ arg13[v3 + 1]) + (arg14[v3 & 3 ^ v1] ^ v7));
                    arg13[v3] = v7;
                }

                int v8 = v2 - 1;
                v7 = arg13[v8] + ((moveRight(v7, 5) ^ arg13[0] << 2) + (moveRight(arg13[0], 3) ^ v7 << 4) ^ (v5 ^ arg13[0]) + (arg14[v3 & 3 ^ v1] ^ v7));
                arg13[v8] = v7;
                --v4;
            }
            while (v4 > 0);

            return arg13;
        }

        private static byte[] toByteArray(int[] arg6, bool arg7)
        {
            byte[] v3;
            int v2 = arg6.Length << 2;

            if (arg7)
            {
                int v1 = arg6[arg6.Length - 1];
                if (v1 > v2)
                {
                    v3 = null;
                    return v3;
                }
                else
                {
                    v2 = v1;
                }
            }

            v3 = new byte[v2];
            int v0;
            for (v0 = 0; v0 < v2; ++v0)
            {
                v3[v0] = ((byte)(moveRight(arg6[moveRight(v0, 2)], ((v0 & 3)) << 3) & 255));
            }

            return v3;
        }

        private static int[] toIntArray(byte[] arg7, bool arg8)
        {
            int[] v2;
            int v1 = (arg7.Length & 3) == 0 ? moveRight(arg7.Length, 2) : moveRight(arg7.Length, 2) + 1;
            if (arg8)
            {
                v2 = new int[v1 + 1];
                v2[v1] = arg7.Length;
            }
            else
            {
                v2 = new int[v1];
            }

            v1 = arg7.Length;
            int v0;
            for (v0 = 0; v0 < v1; ++v0)
            {
                int v3 = moveRight(v0, 2);
                v2[v3] |= (arg7[v0] & 255) << ((v0 & 3) << 3);
            }

            return v2;
        }

        private static int moveRight(int input, int pos)
        {
            return (int)((uint)input >> pos);
        }

        public static String stringToMD5(String arg8)
        {
            byte[] v2;
            String v4 = null;
            v2 = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(arg8));
            StringBuilder v3 = new StringBuilder(v2.Length * 2);
            int v5 = v2.Length;
            int v4_1;
            for (v4_1 = 0; v4_1 < v5; ++v4_1)
            {
                int v0 = v2[v4_1];
                if ((v0 & 255) < 16)
                {
                    v3.Append("0");
                }

                v3.Append((v0 & 255).ToString("x"));
            }

            return v3.ToString();
        }

    }


}
