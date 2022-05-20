﻿// <copyright file="ExampleRsa.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using CryptoNet.Models;
using CryptoNet.Utils;

namespace CryptoNet.Cli;

public class ExampleRsa
{
    private const string ConfidentialDummyData = @"Some Secret Data";

    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;

    internal static string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    internal static string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    public static void Test()
    {
        Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey();
        Example_2_SelfGenerated_And_Save_AsymmetricKey();
        Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content();
        Example_4_Using_X509_Certificate();
        Example_5_Export_Public_Key_For_X509_Certificate();
        Example_7_Customize();
    }

    public static void Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();

        var privateKey = cryptoNet.ExportKey(true);
        var publicKey = cryptoNet.ExportKey(false);

        ICryptoNet encryptClient = new CryptoNetRsa(publicKey);
        var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetRsa(privateKey);
        var decrypt = decryptClient.DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_2_SelfGenerated_And_Save_AsymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();

        cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
        cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

        Debug.Assert(File.Exists(new FileInfo(PrivateKeyFile).FullName));
        Debug.Assert(File.Exists(new FileInfo(PublicKeyFile).FullName));

        ICryptoNet cryptoNetPubKey = new CryptoNetRsa(new FileInfo(PublicKeyFile));
        var encrypt = cryptoNetPubKey.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNetPriKey = new CryptoNetRsa(new FileInfo(PrivateKeyFile));
        var decrypt = cryptoNetPriKey.DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content()
    {
        ICryptoNet cryptoNetWithPublicKey = new CryptoNetRsa(new FileInfo(PublicKeyFile));
        var encryptWithPublicKey = cryptoNetWithPublicKey.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNetWithPrivateKey = new CryptoNetRsa(new FileInfo(PrivateKeyFile));
        var decryptWithPrivateKey = cryptoNetWithPrivateKey.DecryptToString(encryptWithPublicKey);

        Debug.Assert(ConfidentialDummyData == decryptWithPrivateKey);
    }

    public static void Example_4_Using_X509_Certificate()
    {
        // Find and replace CN=Maytham with your own certificate
        X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        ICryptoNet cryptoNetWithPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
        var encryptWithPublicKey = cryptoNetWithPublicKey.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNetWithPrivateKey = new CryptoNetRsa(certificate, KeyType.PrivateKey);
        var decryptWithPrivateKey = cryptoNetWithPrivateKey.DecryptToString(encryptWithPublicKey);

        Debug.Assert(ConfidentialDummyData == decryptWithPrivateKey);
    }

    public static void Example_5_Export_Public_Key_For_X509_Certificate()
    {
        // Find and replace CN=Maytham with your own certificate
        X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        ICryptoNet cryptoNetWithPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
        var publicKey = cryptoNetWithPublicKey.ExportKey(false);

        Debug.Assert(!string.IsNullOrEmpty(publicKey));
    }

    /// <summary>
    /// CryptoNet interact with .net 5/6 for customization, like import/export PEM
    /// Work in Progress, not finished
    /// </summary>
    public static void Example_7_Customize()
    {
        X509Certificate2? cert = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        var pubKeyPem = CryptoNetUtils.ExportPemKey(cert!, false);
        var priKeyPem = CryptoNetUtils.ExportPemKey(cert!);
        
        var password = "password";
        var encryptedPriKeyBytes = CryptoNetUtils.ExportPemKeyWithPassword(cert!, password);
        
        ICryptoNet cryptoNet1 = CryptoNetUtils.ImportPemKeyWithPassword(encryptedPriKeyBytes, password);
        var encrypt1 = cryptoNet1.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNet2 = CryptoNetUtils.ImportPemKey(pubKeyPem);
        var encrypt2 = cryptoNet2.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNet3 = CryptoNetUtils.ImportPemKey(priKeyPem);
        var decrypt2 = cryptoNet3.DecryptToString(encrypt2);
        
        Debug.Assert(ConfidentialDummyData == decrypt2);

        var decrypt1 = cryptoNet3.DecryptToString(encrypt1);
        
        Debug.Assert(ConfidentialDummyData == decrypt1);
    }

}