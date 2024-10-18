﻿// <copyright file="CryptoNetRsa.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CryptoNet.Extensions;
using CryptoNet.Utils;
using CryptoNet.Models;

namespace CryptoNet;

public class CryptoNetRsa : ICryptoNet
{
    private RSA Rsa { get; }
    public CryptoNetInfo Info { get; }

    public CryptoNetRsa(int keySize = 2048)
    {
        Rsa = RSA.Create();
        Rsa.KeySize = keySize;
        Info = CreateInfo();
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    public CryptoNetRsa(string key, int keySize = 2048)
    {
        Rsa = RSA.Create();
        Rsa.KeySize = keySize;
        Info = CreateInfo();
        CreateAsymmetricKey(key);
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    public CryptoNetRsa(FileInfo fileInfo, int keySize = 2048)
    {
        Rsa = RSA.Create();
        Rsa.KeySize = keySize;
        Info = CreateInfo();
        CreateAsymmetricKey(CryptoNetUtils.LoadFileToString(fileInfo.FullName));
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    // ToDo: Needs unit test coverage.
    public CryptoNetRsa(X509Certificate2? certificate, KeyType keyType, int keySize = 2048)
    {
        Rsa = RSA.Create();
        Rsa.KeySize = keySize;
        Info = CreateInfo();
        RSAParameters @params = CryptoNetExtensions.GetParameters(certificate, keyType);
        Rsa.ImportParameters(@params);
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    private byte[] TryGetKey(bool privateKey = true)
    {
        try
        {
            if (privateKey)
            {
                return CryptoNetExtensions.StringToBytes(ExportKey(KeyType.PrivateKey));
            }
            else
            {
                return CryptoNetExtensions.StringToBytes(ExportKey(KeyType.PublicKey));
            }
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
    }

    private KeyType CheckKeyType()
    {
        try
        {
            Rsa.ExportParameters(true);
            return KeyType.PrivateKey;
        }
        catch (CryptographicException)
        {
            return KeyType.PublicKey;
        }
        catch (Exception)
        {
            return KeyType.NotSet;
        }
    }

    private void CreateAsymmetricKey(string? key = null)
    {
        if (!string.IsNullOrEmpty(key))
        {
            Rsa.FromXmlString(key);
        }
    }

    private CryptoNetInfo CreateInfo()
    {
        return new CryptoNetInfo()
        {
            RsaDetail = new RsaDetail()
            {
                Rsa = Rsa
            },
            EncryptionType = EncryptionType.Rsa,
            KeyType = KeyType.NotSet
        };
    }

    public string ExportKey(bool? privateKey = null)
    {
        return privateKey!.Value ? ExportKey(KeyType.PrivateKey) : ExportKey(KeyType.PublicKey);
    }

    public void ExportKeyAndSave(FileInfo fileInfo, bool? privateKey = false)
    {
        string key = privateKey!.Value ? ExportKey(KeyType.PrivateKey) : ExportKey(KeyType.PublicKey);
        if (!string.IsNullOrEmpty(key))
        {
            CryptoNetUtils.SaveKey(fileInfo.FullName, key);
        }
    }

    private string ExportKey(KeyType keyType)
    {
        string result = keyType switch
        {
            KeyType.NotSet => string.Empty,
            KeyType.PublicKey => Rsa.ToXmlString(false),
            KeyType.PrivateKey => Rsa.ToXmlString(true),
            _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
        };

        return result;
    }

    #region encryption logic
    public byte[] EncryptFromString(string content)
    {
        return EncryptContent(CryptoNetExtensions.StringToBytes(content));
    }

    public byte[] EncryptFromBytes(byte[] bytes)
    {
        return EncryptContent(bytes);
    }

    public string DecryptToString(byte[] bytes)
    {
        return CryptoNetExtensions.BytesToString(DecryptContent(bytes));
    }

    public byte[] DecryptToBytes(byte[] bytes)
    {
        return DecryptContent(bytes);
    }

    private byte[] EncryptContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        byte[] result;

        var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;

        var encryptor = aes.CreateEncryptor();

        var keyEncrypted = Rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA1);

        var lKey = keyEncrypted.Length;
        var lenK = BitConverter.GetBytes(lKey);
        var lIv = aes.IV.Length;
        var lenIv = BitConverter.GetBytes(lIv);

        using (var msOut = new MemoryStream())
        {
            msOut.Write(lenK, 0, 4);
            msOut.Write(lenIv, 0, 4);
            msOut.Write(keyEncrypted, 0, lKey);
            msOut.Write(aes.IV, 0, lIv);

            using (var csEncryptedOut = new CryptoStream(msOut, encryptor, CryptoStreamMode.Write))
            {
                var blockSizeBytes = aes.BlockSize / 8;
                var data = new byte[blockSizeBytes];

                using (var msIn = new MemoryStream(bytes))
                {
                    int count;
                    do
                    {
                        count = msIn.Read(data, 0, blockSizeBytes);
                        csEncryptedOut.Write(data, 0, count);
                    } while (count > 0);

                    msIn.Close();
                }

                csEncryptedOut.FlushFinalBlock();
                csEncryptedOut.Close();
            }

            result = msOut.ToArray();

            msOut.Close();
        }

        return result;
    }

    private byte[] DecryptContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        byte[] result;

        var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;

        var lenKByte = new byte[4];
        var lenIvByte = new byte[4];

        using (var inMs = new MemoryStream(bytes))
        {
            inMs.Seek(0, SeekOrigin.Begin);
            inMs.Seek(0, SeekOrigin.Begin);
            inMs.Read(lenKByte, 0, 3);
            inMs.Seek(4, SeekOrigin.Begin);
            inMs.Read(lenIvByte, 0, 3);

            var lenK = BitConverter.ToInt32(lenKByte, 0);
            var lenIv = BitConverter.ToInt32(lenIvByte, 0);

            var startC = lenK + lenIv + 8;

            var keyEncrypted = new byte[lenK];
            var iv = new byte[lenIv];

            inMs.Seek(8, SeekOrigin.Begin);
            inMs.Read(keyEncrypted, 0, lenK);
            inMs.Seek(8 + lenK, SeekOrigin.Begin);
            inMs.Read(iv, 0, lenIv);

            var keyDecrypted = Rsa.Decrypt(keyEncrypted, RSAEncryptionPadding.OaepSHA1);

            var decryptor = aes.CreateDecryptor(keyDecrypted, iv);

            using (var outMs = new MemoryStream())
            {
                var blockSizeBytes = aes.BlockSize / 8;
                var data = new byte[blockSizeBytes];

                inMs.Seek(startC, SeekOrigin.Begin);
                using (var csDecryptedOut = new CryptoStream(outMs, decryptor, CryptoStreamMode.Write))
                {
                    int count;
                    do
                    {
                        count = inMs.Read(data, 0, blockSizeBytes);
                        csDecryptedOut.Write(data, 0, count);
                    } while (count > 0);

                    csDecryptedOut.FlushFinalBlock();
                    csDecryptedOut.Close();
                }

                result = outMs.ToArray();

                outMs.Close();
            }

            inMs.Close();
        }

        return result;
    }
    #endregion
}