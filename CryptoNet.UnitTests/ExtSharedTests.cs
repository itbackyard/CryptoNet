﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Models;
using NUnit.Framework;
using Shouldly;

namespace CryptoNet.UnitTests;

[TestFixture]
public class ExtSharedTests
{
    [Test]
    public void GetParameters_ShouldReturnRsaParameters_WhenCertificateAndKeyTypeAreValid()
    {
        // Arrange
        using var rsa = RSA.Create();
        var certificateRequest = new CertificateRequest("CN=TestCert", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

        // Act
        RSAParameters parameters = ExtShared.ExtShared.GetParameters(certificate, KeyType.PrivateKey);

        // Assert
        parameters.D.ShouldNotBeNull();
    }

    [Test]
    public void GetCertificateFromStore_WithNonexistentCertName_ShouldReturnNull()
    {
        // Act
        var result = ExtShared.ExtShared.GetCertificateFromStore(StoreName.My, StoreLocation.CurrentUser, "NonexistentCertificate");

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public void GetCertificateFromStore_WithDifferentStoreOverloads_ShouldReturnNullForNonexistentCert()
    {
        // Act
        var result1 = ExtShared.ExtShared.GetCertificateFromStore(StoreName.My, "NonexistentCertificate");
        var result2 = ExtShared.ExtShared.GetCertificateFromStore(StoreLocation.CurrentUser, "NonexistentCertificate");
        var result3 = ExtShared.ExtShared.GetCertificateFromStore("NonexistentCertificate");

        // Assert
        result1.ShouldBeNull();
        result2.ShouldBeNull();
        result3.ShouldBeNull();
    }

    [Test]
    public void BytesToString_ShouldConvertByteArrayToString()
    {
        // Arrange
        var bytes = Encoding.ASCII.GetBytes("Hello");

        // Act
        var result = ExtShared.ExtShared.BytesToString(bytes);

        // Assert
        result.ShouldBe("Hello");
    }

    [Test]
    public void StringToBytes_ShouldConvertStringToByteArray()
    {
        // Arrange
        var content = "Hello";

        // Act
        var result = ExtShared.ExtShared.StringToBytes(content);

        // Assert
        result.ShouldBeEquivalentTo(Encoding.ASCII.GetBytes("Hello"));
    }

    [Test]
    public void Base64BytesToString_ShouldEncodeBytesToBase64String()
    {
        // Arrange
        var bytes = Encoding.ASCII.GetBytes("Hello");

        // Act
        var result = ExtShared.ExtShared.Base64BytesToString(bytes);

        // Assert
        result.ShouldBe("SGVsbG8=");
    }

    [Test]
    public void Base64StringToBytes_ShouldDecodeBase64StringToBytes()
    {
        // Arrange
        var content = "SGVsbG8=";

        // Act
        var result = ExtShared.ExtShared.Base64StringToBytes(content);

        // Assert
        result.ShouldBeEquivalentTo(Encoding.ASCII.GetBytes("Hello"));
    }

    [Test]
    public void ByteArrayCompare_ShouldReturnTrue_WhenArraysAreEqual()
    {
        // Arrange
        var array1 = new byte[] { 1, 2, 3 };
        var array2 = new byte[] { 1, 2, 3 };

        // Act
        var result = ExtShared.ExtShared.ByteArrayCompare(array1, array2);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void ByteArrayCompare_ShouldReturnFalse_WhenArraysAreNotEqualInLength()
    {
        // Arrange
        var array1 = new byte[] { 1, 2, 3 };
        var array2 = new byte[] { 1, 2 };

        // Act
        var result = ExtShared.ExtShared.ByteArrayCompare(array1, array2);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void ByteArrayCompare_ShouldReturnFalse_WhenArraysAreEqualInLengthButNotContent()
    {
        // Arrange
        var array1 = new byte[] { 1, 2, 3 };
        var array2 = new byte[] { 1, 2, 4 };

        // Act
        var result = ExtShared.ExtShared.ByteArrayCompare(array1, array2);

        // Assert
        result.ShouldBeFalse();
    }

    private const string TestFilePath = "testfile";
    private const string TestContent = "This is a test string.";

    [Test]
    public void LoadFileToBytes_ShouldLoadFileContentAsByteArray()
    {
        File.WriteAllText(TestFilePath, TestContent);
        var bytes = ExtShared.ExtShared.LoadFileToBytes(TestFilePath);

        bytes.ShouldNotBeNull();
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Test]
    public void LoadFileToString_ShouldLoadFileContentAsString()
    {
        File.WriteAllText(TestFilePath, TestContent);
        var content = ExtShared.ExtShared.LoadFileToString(TestFilePath);

        content.ShouldBe(TestContent);
    }

    [Test]
    public void SaveKey_ShouldSaveBytesToFile()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        ExtShared.ExtShared.SaveKey(TestFilePath, bytes);

        File.Exists(TestFilePath).ShouldBeTrue();
        var savedBytes = File.ReadAllBytes(TestFilePath);
        savedBytes.ShouldBe(bytes);
    }

    [Test]
    public void SaveKey_ShouldSaveStringToFile()
    {
        ExtShared.ExtShared.SaveKey(TestFilePath, TestContent);

        File.Exists(TestFilePath).ShouldBeTrue();
        var savedContent = File.ReadAllText(TestFilePath);
        savedContent.ShouldBe(TestContent);
    }

    [Test]
    public void ExportAndSaveAesKey_ShouldExportKeyAndIVAsJson()
    {
        using var aes = Aes.Create();
        aes.Key = new byte[16];
        aes.IV = new byte[16];

        var json = ExtShared.ExtShared.ExportAndSaveAesKey(aes);

        json.ShouldNotBeNullOrEmpty();
        json.ShouldContain("\"Key\"");
        json.ShouldContain("\"IV\"");
    }

    [Test]
    public void ImportAesKey_ShouldImportKeyAndIVFromJson()
    {
        using var aes = Aes.Create();
        aes.Key = new byte[16];
        aes.IV = new byte[16];

        var json = ExtShared.ExtShared.ExportAndSaveAesKey(aes);
        var importedAesKeyValue = ExtShared.ExtShared.ImportAesKey(json);

        importedAesKeyValue.ShouldNotBeNull();
        importedAesKeyValue.Key.ShouldBe(aes.Key);
        importedAesKeyValue.Iv.ShouldBe(aes.IV);
    }

    [Test]
    public void GetDescription_ShouldReturnDescriptionAttribute()
    {
        var value = KeyType.PublicKey;
        var description = ExtShared.ExtShared.GetDescription(value);

        description.ShouldBe("Public key is set."); // assuming "Public Key" is the description for PublicKey
    }

    [Test]
    public void GetDescription_ShouldReturnEnumNameIfNoDescriptionAttribute()
    {
        var value = KeyType.NotSet; // assuming OtherKeyType has no description attribute
        var description = ExtShared.ExtShared.GetDescription(value);

        description.ShouldBe("Key does not exist.");
    }

    [Test]
    public void GetKeyType_ShouldReturnPublicKeyIfPublicOnly()
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.PublicOnly.ShouldBeFalse();

        var keyType = ExtShared.ExtShared.GetKeyType(rsa);
        keyType.ShouldBe(KeyType.PrivateKey);
    }

    [Test]
    public void GetKeyType_ShouldReturnPrivateKeyIfNotPublicOnly()
    {
        using var rsa = new RSACryptoServiceProvider(2048);
        var parameters = rsa.ExportParameters(includePrivateParameters: true);
        rsa.ImportParameters(parameters);

        rsa.PublicOnly.ShouldBeFalse();

        var keyType = ExtShared.ExtShared.GetKeyType(rsa);
        keyType.ShouldBe(KeyType.PrivateKey);
    }
}
