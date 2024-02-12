namespace BNRNew_API.utils
{
    using BNRNew_API.config;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class RSAEncryptionTools
    {
        public static void tryEnc()
        {
            // Generate or obtain the RSA public and private keys as strings
            string privateKeyPem = File.ReadAllText(AppConstant.ENCRYPTION_PRIVATE_FILE_PATH);
            string publicKeyPem = File.ReadAllText(AppConstant.ENCRYPTION_PUBLIC_FILE_PATH);

            // Encrypt the message using the public key
            string originalMessage = "Hello, world! test aja y 123 jangan salag sangka y gan, ini hanya test doang";
            byte[] encryptedMessage = EncryptRSA(originalMessage, publicKeyPem);


            // Decrypt the message using the private key
            string decryptedMessage = DecryptRSA(encryptedMessage, privateKeyPem);

            // Output results
            Console.WriteLine($"Original: {originalMessage}");
            Console.WriteLine($"Encrypted: {Convert.ToBase64String(encryptedMessage)}");
        }

        public static byte[] EncryptRSA(string plainText, string privateKeyPem)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKeyPem);
                return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.Pkcs1);
            }
        }

        public static string DecryptRSA(byte[] encryptedData, string privateKeyPem)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKeyPem);
                byte[] decryptedBytes = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
