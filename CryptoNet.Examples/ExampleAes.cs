// <copyright file="ExampleAes.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CryptoNet.Models;

namespace CryptoNet.Examples;

/// <summary>
/// Example usage scenarios for AES symmetric encryption using the CryptoNet library.
/// Demonstrates generating, saving/loading symmetric keys and encrypting/decrypting
/// both content and files.
/// </summary>
public static class ExampleAes
{
    private const string ConfidentialDummyData = @"Some Secret Data";

    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string SymmetricKeyFilePath = Path.Combine(BaseDirectory, $"{KeyType.SymmetricKey}.xml");

    /// <summary>
    /// Generate a symmetric key, encrypt and decrypt a string using self-generated key.
    /// </summary>
    public static void EncryptDecryptWithSelfGeneratedSymmetricKey()
    {
        ICryptoNetAes cryptoNet = new CryptoNetAes();
        string key = cryptoNet.GetKey();

        ICryptoNet encryptClient = new CryptoNetAes(key);
        byte[] encrypted = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetAes(key);
        string decrypted = decryptClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Generate a symmetric key and save it to disk, then load it to decrypt previously encrypted content.
    /// </summary>
    public static void GenerateAndSaveSymmetricKey()
    {
        ICryptoNetAes cryptoNet = new CryptoNetAes();
        FileInfo keyFile = new FileInfo(SymmetricKeyFilePath);
        cryptoNet.SaveKey(keyFile);

        Debug.Assert(File.Exists(keyFile.FullName));

        byte[] encrypted = cryptoNet.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNetImported = new CryptoNetAes(keyFile);
        string decrypted = cryptoNetImported.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Encrypt and decrypt using a provided raw symmetric key and IV.
    /// </summary>
    public static void EncryptDecryptWithProvidedSymmetricKey()
    {
        string symmetricKey = "12345678901234567890123456789012";
        if (symmetricKey.Length != 32)
        {
            Console.WriteLine("Key should be 32 characters long");
            Environment.Exit(0);
        }

        string secret = "1234567890123456";
        if (secret.Length != 16)
        {
            Console.WriteLine("IV should be 16 characters long");
            Environment.Exit(1);
        }

        byte[] key = Encoding.UTF8.GetBytes(symmetricKey);
        byte[] iv = Encoding.UTF8.GetBytes(secret);

        ICryptoNet encryptClient = new CryptoNetAes(key, iv);
        byte[] encrypted = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetAes(key, iv);
        string decrypted = decryptClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Generate a human-readable key/secret pair and use them for encryption/decryption.
    /// </summary>
    public static void EncryptDecryptWithHumanReadableKeySecret()
    {
        string symmetricKey = GenerateUniqueKey("symmetricKey");
        string secret = new string(GenerateUniqueKey("password").Take(16).ToArray());

        byte[] key = Encoding.UTF8.GetBytes(symmetricKey);
        byte[] iv = Encoding.UTF8.GetBytes(secret);

        ICryptoNet encryptClient = new CryptoNetAes(key, iv);
        byte[] encrypted = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetAes(key, iv);
        string decrypted = decryptClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Encrypt and decrypt a file using a self-generated symmetric key and verify content equality.
    /// </summary>
    /// <param name="filename">Relative path under TestFiles directory to the file to encrypt.</param>
    public static void EncryptAndDecryptFileWithSymmetricKeyTest(string filename)
    {
        ICryptoNetAes cryptoNet = new CryptoNetAes();
        string key = cryptoNet.GetKey();

        FileInfo fileInfo = new FileInfo(filename);

        ICryptoNet encryptClient = new CryptoNetAes(key);
        string sourceFilePath = Path.Combine(BaseDirectory, filename);
        byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
        byte[] encrypted = encryptClient.EncryptFromBytes(fileBytes);

        ICryptoNet decryptClient = new CryptoNetAes(key);
        byte[] decrypted = decryptClient.DecryptToBytes(encrypted);
        string decryptedFilePath = $"TestFiles\\{Path.GetFileNameWithoutExtension(fileInfo.Name)}-decrypted{fileInfo.Extension}";
        File.WriteAllBytes(decryptedFilePath, decrypted);

        bool isIdenticalFile = ExtShared.ExtShared.ByteArrayCompare(fileBytes, decrypted);
        Debug.Assert(isIdenticalFile);
    }

    /// <summary>
    /// Create a deterministic unique key string from input using MD5 (example helper).
    /// </summary>
    /// <param name="input">Input string to derive the key from.</param>
    /// <returns>Hexadecimal string representation of MD5 hash.</returns>
    public static string GenerateUniqueKey(string input)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hash = MD5.HashData(inputBytes);

        StringBuilder sb = new StringBuilder();
        foreach (byte b in hash)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}