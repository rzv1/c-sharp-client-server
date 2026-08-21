namespace Persistence;

using Services;
using System;
using System.Security.Cryptography;
using System.Text;

   public static class EncryptUtils
   {
      private static byte[] PrepareSecretKey(string myKey)
      {
         var key = Encoding.UTF8.GetBytes(myKey);
         var hash = SHA1.HashData(key);
             
         var truncatedKey = new byte[16];
         Array.Copy(hash, truncatedKey, 16);
         return truncatedKey;
      }

      public static string Encrypt(string strToEncrypt, string secret)
      {
         try
         {
            var key = PrepareSecretKey(secret);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB; 
            aes.Padding = PaddingMode.PKCS7;

            var encryptor= aes.CreateEncryptor();
            var inputBytes = Encoding.UTF8.GetBytes(strToEncrypt);
            var encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
         }
         catch (Exception ex)
         {
            Console.Error.WriteLine($"Encryption error: {ex.Message}");
            throw new AppException("Encryption error");
         }
      }
   }