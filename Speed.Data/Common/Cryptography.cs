using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Speed.Common
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public class Cryptography
    {
        static SHA1Managed hash = new SHA1Managed();

        public static string Hash(string text)
        {
            // hash the supplied password for storing in the database.
            byte[] passwordBytes = Encoding.UTF8.GetBytes(text);
            byte[] passwordHashed = hash.ComputeHash(passwordBytes);
            return Convert.ToBase64String(passwordHashed);
        }

        public static string Encrypt(string input)
        {
            return Encrypt(input, string.Concat(Enumerable.Range(0, 10).ToArray()));
        }

        public static string Decrypt(string input)
        {
            return Decrypt(input, string.Concat(Enumerable.Range(0, 10).ToArray()));
        }

        /// <summary>
        /// Incrypt the input using password provided
        /// </summary>
        /// <param name="input">Input string to encrypt</param>
        /// <param name="password">Password to use</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt(string input, string password)
        {

            string data = input;
            byte[] utfdata = UTF8Encoding.UTF8.GetBytes(data);
            byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(password);



            // Our symmetric encryption algorithm
            AesManaged aes = new AesManaged();

            // We're using the PBKDF2 standard for password-based key generation
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);

            // Setting our parameters
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // Encryption
            ICryptoTransform encryptTransf = aes.CreateEncryptor();

            // Output stream, can be also a FileStream
            MemoryStream encryptStream = new MemoryStream();
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);

            encryptor.Write(utfdata, 0, utfdata.Length);
            encryptor.Flush();
            encryptor.Close();

            byte[] encryptBytes = encryptStream.ToArray();
            string encryptedString = Convert.ToBase64String(encryptBytes);

            return encryptedString;
        }

        /// <summary>
        /// Decrypt string using password provided
        /// </summary>
        /// <param name="base64Input">Input to decrypt</param>
        /// <param name="password">Password to use</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string base64Input, string password)
        {

            byte[] encryptBytes = Convert.FromBase64String(base64Input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);

            // Our symmetric encryption algorithm
            AesManaged aes = new AesManaged();

            // We're using the PBKDF2 standard for password-based key generation
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);

            // Setting our parameters
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // Now, decryption
            ICryptoTransform decryptTrans = aes.CreateDecryptor();

            // Output stream, can be also a FileStream
            MemoryStream decryptStream = new MemoryStream();
            CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);

            decryptor.Write(encryptBytes, 0, encryptBytes.Length);
            decryptor.Flush();
            decryptor.Close();

            byte[] decryptBytes = decryptStream.ToArray();
            string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            return decryptedString;
        }

    }

}