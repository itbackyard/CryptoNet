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

public static class ExampleDsa
{
    private const string ConfidentialDummyData = @"Some Secret Data";
    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;

    internal static readonly string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    internal static readonly string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    public static void Example_1_Sign_Validate_Content_With_SelfGenerated_AsymmetricKey()
    {
        ICryptoNetDsa client = new CryptoNetDsa();
        var privateKey = client.GetKey(true);

        ICryptoNetDsa signatureClient = new CryptoNetDsa(privateKey);
        var signature = signatureClient.CreateSignature(ConfidentialDummyData);

        ICryptoNetDsa verifyClient = new CryptoNetDsa(privateKey);
        var confidentialAsBytes = ExtShared.ExtShared.StringToBytes(ConfidentialDummyData);

        bool isVerified = verifyClient.IsContentVerified(confidentialAsBytes, signature);

        Debug.Assert(isVerified == true);
    }

    public static void Example_2_SelfGenerated_And_Save_AsymmetricKey()
    {
        ICryptoNetDsa cryptoNet = new CryptoNetDsa();

        cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
        cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);

        Debug.Assert(File.Exists(new FileInfo(PrivateKeyFile).FullName));
        Debug.Assert(File.Exists(new FileInfo(PublicKeyFile).FullName));

        ICryptoNetDsa dsaWithPrivateKey = new CryptoNetDsa(new FileInfo(PrivateKeyFile));
        var signature = dsaWithPrivateKey.CreateSignature(ConfidentialDummyData);

        ICryptoNetDsa dsaWithPublicKey = new CryptoNetDsa(new FileInfo(PublicKeyFile));
        var confidentialAsBytes = ExtShared.ExtShared.StringToBytes(ConfidentialDummyData);
        var isVerified = dsaWithPublicKey.IsContentVerified(confidentialAsBytes, signature);

        Debug.Assert(isVerified == true);
    }
    
}