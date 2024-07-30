using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LucidDesk.Manager.Security
{
   public static  class SecurityManager
    {
        private readonly static string Key = "Desk123456789012"; // 16-byte secret key
        private readonly static string InitializationVector = "Desk123456789012";  // 16-byte initialization vector (IV)



        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(InitializationVector);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public static string Decrypt(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(InitializationVector);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decryptedBytes = Convert.FromBase64String(encryptedText);
                string decryptedString = Encoding.UTF8.GetString(decryptor.TransformFinalBlock(decryptedBytes, 0, decryptedBytes.Length));
                return decryptedString;
            }
        }
    }
}                                              