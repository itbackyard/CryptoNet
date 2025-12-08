// <copyright file="ExampleRsa.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Models;

namespace CryptoNet.Examples;

/// <summary>
/// Example usage scenarios for RSA operations using the CryptoNet library.
/// Methods demonstrate generating keys, saving/loading keys, encrypting/decrypting
/// content and working with X509 certificates and PEM formats.
/// </summary>
public static class ExampleRsa
{
    private const string ConfidentialDummyData = @"Some Secret Data";
    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    internal static readonly string PrivateKeyFilePath = Path.Combine(BaseDirectory, "privateKey");
    internal static readonly string PublicKeyFilePath = Path.Combine(BaseDirectory, "publicKey.pub");

    /// <summary>
    /// Demonstrates generating a self-contained RSA key pair and using them to
    /// encrypt and decrypt a string.
    /// </summary>
    public static void EncryptDecryptWithSelfGeneratedKey()
    {
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();

        string privateKeyXml = cryptoNet.GetKey(true);
        string publicKeyXml = cryptoNet.GetKey(false);

        ICryptoNet encryptClient = new CryptoNetRsa(publicKeyXml);
        byte[] encrypted = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetRsa(privateKeyXml);
        string decrypted = decryptClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Demonstrates generating keys and saving them to files, then loading
    /// those files to perform encryption/decryption.
    /// </summary>
    public static void GenerateAndSaveAsymmetricKey()
    {
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();

        cryptoNet.SaveKey(new FileInfo(PrivateKeyFilePath), true);
        cryptoNet.SaveKey(new FileInfo(PublicKeyFilePath), false);

        Debug.Assert(File.Exists(new FileInfo(PrivateKeyFilePath).FullName));
        Debug.Assert(File.Exists(new FileInfo(PublicKeyFilePath).FullName));

        ICryptoNet publicKeyClient = new CryptoNetRsa(new FileInfo(PublicKeyFilePath));
        byte[] encrypted = publicKeyClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet privateKeyClient = new CryptoNetRsa(new FileInfo(PrivateKeyFilePath));
        string decrypted = privateKeyClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Encrypts with a public key file and decrypts with a private key file.
    /// </summary>
    public static void EncryptWithPublicKeyAndDecryptWithPrivateKey()
    {
        ICryptoNet publicKeyClient = new CryptoNetRsa(new FileInfo(PublicKeyFilePath));
        byte[] encrypted = publicKeyClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet privateKeyClient = new CryptoNetRsa(new FileInfo(PrivateKeyFilePath));
        string decrypted = privateKeyClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Demonstrates using an X509 certificate's public/private key for encryption/decryption.
    /// Replace the subject name with a certificate available on your machine.
    /// </summary>
    public static void UseX509Certificate()
    {
        // Replace subject with your certificate subject (for example "CN=localhost")
        X509Certificate2? certificate = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

        ICryptoNet publicKeyClient = new CryptoNetRsa(certificate, KeyType.PublicKey);
        byte[] encrypted = publicKeyClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet privateKeyClient = new CryptoNetRsa(certificate, KeyType.PrivateKey);
        string decrypted = privateKeyClient.DecryptToString(encrypted);

        Debug.Assert(ConfidentialDummyData == decrypted);
    }

    /// <summary>
    /// Exports the public key portion from an X509 certificate using the CryptoNetRsa GetKey API.
    /// </summary>
    public static void ExportPublicKeyFromX509Certificate()
    {
        // Replace subject with your certificate subject (for example "CN=localhost")
        X509Certificate2? certificate = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

        ICryptoNetRsa certClient = new CryptoNetRsa(certificate, KeyType.PublicKey);
        string publicKeyXml = certClient.GetKey(false);

        Debug.Assert(!string.IsNullOrEmpty(publicKeyXml));
    }

    /// <summary>
    /// Demonstrates encrypting/decrypting content that includes special (emoji) characters.
    /// Ensures UTF-8 byte-level operations are used.
    /// </summary>
    public static void WorkWithSpecialCharacterText()
    {
        string confidentialWithSpecialChars = "Top secret 😃😃";

        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        string privateKeyXml = cryptoNet.GetKey(true);
        string publicKeyXml = cryptoNet.GetKey(false);

        ICryptoNet encryptClient = new CryptoNetRsa(publicKeyXml);
        byte[] encrypted = encryptClient.EncryptFromBytes(Encoding.UTF8.GetBytes(confidentialWithSpecialChars));

        ICryptoNet decryptClient = new CryptoNetRsa(privateKeyXml);
        byte[] decryptedBytes = decryptClient.DecryptToBytes(encrypted);
        string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

        Debug.Assert(confidentialWithSpecialChars == decryptedString);
    }

    /// <summary>
    /// Work-in-progress: demonstrates PEM export/import and encrypted PEM import with password.
    /// </summary>
    public static void CustomizePemExamples()
    {
        X509Certificate2? certificate = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

        char[] pubKeyPem = ExportPemKey(certificate!, privateKey: false);
        char[] priKeyPem = ExportPemKey(certificate!);

        string password = "password";
        byte[] encryptedPriKeyBytes = ExportPemKeyWithPassword(certificate!, password);

        ICryptoNet decryptClientWithPassword = ImportPemKeyWithPassword(encryptedPriKeyBytes, password);
        byte[] encrypted1 = decryptClientWithPassword.EncryptFromString(ConfidentialDummyData);

        ICryptoNet encryptClientFromPubPem = ImportPemKey(pubKeyPem);
        byte[] encrypted2 = encryptClientFromPubPem.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClientFromPriPem = ImportPemKey(priKeyPem);
        string decrypted2 = decryptClientFromPriPem.DecryptToString(encrypted2);

        Debug.Assert(ConfidentialDummyData == decrypted2);

        string decrypted1 = decryptClientFromPriPem.DecryptToString(encrypted1);

        Debug.Assert(ConfidentialDummyData == decrypted1);
    }

    /// <summary>
    /// Export a certificate as PEM-encoded character array.
    /// </summary>
    /// <param name="cert">Certificate to export.</param>
    /// <returns>PEM characters for the certificate.</returns>
    public static char[] ExportPemCertificate(X509Certificate2 cert)
    {
        byte[] certBytes = cert!.RawData;
        char[] certPem = PemEncoding.Write("CERTIFICATE", certBytes);
        return certPem;
    }

    /// <summary>
    /// Export a certificate's RSA key as PEM (public or private).
    /// </summary>
    /// <param name="cert">Certificate containing the RSA key.</param>
    /// <param name="privateKey">True to export private key, false for public key.</param>
    /// <returns>PEM characters for the key.</returns>
    public static char[] ExportPemKey(X509Certificate2 cert, bool privateKey = true)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;

        if (privateKey)
        {
            byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            return PemEncoding.Write("PRIVATE KEY", privateKeyBytes);
        }

        byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
        return PemEncoding.Write("PUBLIC KEY", publicKeyBytes);
    }

    /// <summary>
    /// Import a PEM key into a newly created CryptoNetRsa instance.
    /// </summary>
    /// <param name="key">PEM characters representing the key.</param>
    /// <returns>ICryptoNet instance with the imported key.</returns>
    public static ICryptoNet ImportPemKey(char[] key)
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail!.Rsa?.ImportFromPem(key);
        return cryptoNet;
    }

    /// <summary>
    /// Export an encrypted PKCS#8 private key using a password and AES-256-CBC PBES.
    /// </summary>
    /// <param name="cert">Certificate containing the private key to export.</param>
    /// <param name="password">Password to encrypt the private key.</param>
    /// <returns>Encrypted private key bytes.</returns>
    public static byte[] ExportPemKeyWithPassword(X509Certificate2 cert, string password)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] encryptedPrivateKey = rsa.ExportEncryptedPkcs8PrivateKey(passwordBytes,
            new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, iterationCount: 100_000));
        return encryptedPrivateKey;
    }

    /// <summary>
    /// Import an encrypted PKCS#8 private key using the provided password into a new CryptoNetRsa instance.
    /// </summary>
    /// <param name="encryptedPrivateKey">Encrypted PKCS#8 private key bytes.</param>
    /// <param name="password">Password used to decrypt the private key.</param>
    /// <returns>ICryptoNet instance with imported private key.</returns>
    public static ICryptoNet ImportPemKeyWithPassword(byte[] encryptedPrivateKey, string password)
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail?.Rsa?.ImportEncryptedPkcs8PrivateKey(password, encryptedPrivateKey, out _);
        return cryptoNet;
    }
}