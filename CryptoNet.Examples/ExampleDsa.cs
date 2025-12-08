// <copyright file="ExampleDsa.cs" company="NextBix" year="2025">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>07-12-2025 20:04:00</date>
// <summary>part of CryptoNet project</summary>

using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Models;

namespace CryptoNet.Examples;

/// <summary>
/// Example usage scenarios for DSA operations using the CryptoNet library.
/// Demonstrates signing, verification and saving/loading DSA keys.
/// </summary>
public static class ExampleDsa
{
    private const string ConfidentialDummyData = @"Some Secret Data";
    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    internal static readonly string PrivateKeyFilePath = Path.Combine(BaseDirectory, "privateKey");
    internal static readonly string PublicKeyFilePath = Path.Combine(BaseDirectory, "publicKey.pub");

    /// <summary>
    /// Demonstrates signing content with a self-generated DSA key and validating the signature.
    /// </summary>
    public static void SignAndValidateWithSelfGeneratedKey()
    {
        ICryptoNetDsa client = new CryptoNetDsa();
        string privateKeyXml = client.GetKey(true);

        ICryptoNetDsa signatureClient = new CryptoNetDsa(privateKeyXml);
        byte[] signature = signatureClient.CreateSignature(ConfidentialDummyData);

        ICryptoNetDsa verifyClient = new CryptoNetDsa(privateKeyXml);
        byte[] contentBytes = ExtShared.ExtShared.StringToBytes(ConfidentialDummyData);

        bool isVerified = verifyClient.IsContentVerified(contentBytes, signature);

        Debug.Assert(isVerified == true);
    }

    /// <summary>
    /// Demonstrates generating DSA key pair, saving them to files, and verifying signatures using loaded keys.
    /// </summary>
    public static void GenerateAndSaveDsaKeyPair()
    {
        ICryptoNetDsa cryptoNet = new CryptoNetDsa();

        cryptoNet.SaveKey(new FileInfo(PrivateKeyFilePath), true);
        cryptoNet.SaveKey(new FileInfo(PublicKeyFilePath), false);

        Debug.Assert(File.Exists(new FileInfo(PrivateKeyFilePath).FullName));
        Debug.Assert(File.Exists(new FileInfo(PublicKeyFilePath).FullName));

        ICryptoNetDsa dsaWithPrivateKey = new CryptoNetDsa(new FileInfo(PrivateKeyFilePath));
        byte[] signature = dsaWithPrivateKey.CreateSignature(ConfidentialDummyData);

        ICryptoNetDsa dsaWithPublicKey = new CryptoNetDsa(new FileInfo(PublicKeyFilePath));
        byte[] confidentialBytes = ExtShared.ExtShared.StringToBytes(ConfidentialDummyData);
        bool isVerified = dsaWithPublicKey.IsContentVerified(confidentialBytes, signature);

        Debug.Assert(isVerified == true);
    }
}