using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Speed
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    internal static class EncryptionHelper
    {

        static string password = "abc";
        static string salt = "123";

        public static Object EncryptObject(object obj)
        {

            byte[] key = EncryptionHelper.GetHashKey(password, salt);
            Type t = obj.GetType();

            PropertyInfo[] fi = t.GetProperties();

            object o = null;
            foreach (PropertyInfo f in fi)
            {

                if (f.PropertyType == typeof(string))
                {
                    PropertyInfo pi = t.GetProperty(f.Name, BindingFlags.Public | BindingFlags.Instance);

                    try
                    {
                        o = pi.GetValue(obj, null);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    string s = (string)o;
                    f.SetValue(obj, EncryptionHelper.Encrypt(key, s), null);
                }

            }
            return obj;
        }

        public static Object DecryptObject(object obj)
        {
            byte[] key = EncryptionHelper.GetHashKey(password, salt);
            Type t = obj.GetType();
            PropertyInfo[] fi = t.GetProperties();
            object o = null;
            foreach (PropertyInfo f in fi)
            {
                if (f.PropertyType == typeof(string))
                {
                    PropertyInfo pi = t.GetProperty(f.Name, BindingFlags.Public | BindingFlags.Instance);

                    try
                    {
                        o = pi.GetValue(obj, null);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    string s = (string)o;
                    f.SetValue(obj, EncryptionHelper.Decrypt(key, s), null);
                }
            }
            return obj;
        }

        internal static byte[] GetHashKey(string hashKey, string salt)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] saltBytes = encoder.GetBytes(salt);

            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(hashKey, saltBytes);

            // Return the key
            return rfc.GetBytes(16);
        }

        internal static string Encrypt(byte[] key, string dataToEncrypt)
        {
            AesManaged encryptor = new AesManaged();

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = key;

            // create memory stream
            using (MemoryStream encryptionStream = new MemoryStream())
            {
                // Create crypto stream
                using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // Encrypt
                    byte[] data = UTF8Encoding.UTF8.GetBytes(dataToEncrypt);
                    encrypt.Write(data, 0, data.Length);
                    encrypt.FlushFinalBlock();
                    encrypt.Close();

                    // Return encrypted data as base64 string
                    return Convert.ToBase64String(encryptionStream.ToArray());
                }
            }
        }

        internal static string Decrypt(byte[] key, string encryptedString)
        {
            AesManaged decryptor = new AesManaged();

            // convert base64 string to byte array
            byte[] encryptedData = Convert.FromBase64String(encryptedString);

            // Set key and IV
            decryptor.Key = key;
            decryptor.IV = key;

            // create  memory stream
            using (MemoryStream decryptionStream = new MemoryStream())
            {
                // Create crypto stream
                using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    // Encrypt
                    decrypt.Write(encryptedData, 0, encryptedData.Length);
                    decrypt.Flush();
                    decrypt.Close();

                    // Return unencrypted data
                    byte[] decryptedData = decryptionStream.ToArray();
                    return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
                }
            }
        }
    }
}