using System.Security.Cryptography;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

public static class AESHelper
{
    public static string GenerateRandomKey()
    {
        byte[] key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }

    public static string GenerateRandomIV()
    {
        byte[] iv = new byte[16];
        RandomNumberGenerator.Fill(iv);
        return Convert.ToBase64String(iv);
    }

    public static string Encrypt(string plainText, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using (var writer = new StreamWriter(cryptoStream))
        {
            writer.Write(plainText);
        }
        var encryptedBytes = memoryStream.ToArray();
        var iv = aes.IV;
        var combined = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, combined, iv.Length, encryptedBytes.Length);
        return Convert.ToBase64String(combined);
    }

    public static string Decrypt(string cipherTextBase64, string key)
    {
        var cipherBytes = Convert.FromBase64String(cipherTextBase64);
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.Mode = CipherMode.CBC;
        var iv = new byte[aes.BlockSize / 8];
        Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
        aes.IV = iv;
        aes.Padding = PaddingMode.PKCS7;
        var contentBytes = new byte[cipherBytes.Length - iv.Length];
        Buffer.BlockCopy(cipherBytes, iv.Length, contentBytes, 0, contentBytes.Length);
        using var memoryStream = new MemoryStream(contentBytes);
        using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        return reader.ReadToEnd();
    }
}
