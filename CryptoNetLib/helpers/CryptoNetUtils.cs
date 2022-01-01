// <copyright file="CryptoNetUtils.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of helpers project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CryptoNetLib.helpers
{
    public static class CryptoNetUtils
    {
        public static byte[] LoadFileToBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public static string LoadFileToString(string filename)
        {
            return BytesToString(LoadFileToBytes(filename));
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[] StringToBytes(string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

        public static void SaveKey(string filename, byte[] bytes)
        {
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
        }

        public static void SaveKey(string filename, string content)
        {
            var bytes = StringToBytes(content);
            SaveKey(filename, bytes);
        }

        public static RSAParameters GetParameters(X509Certificate2? certificate, KeyHelper.KeyType keyType)
        {
            return certificate.GetRSAPrivateKey().ExportParameters(keyType == KeyHelper.KeyType.PrivateKey);
        }

        public static X509Certificate2? GetCertificateFromStore(string certName)
        {
            X509Store store = new X509Store(StoreLocation.CurrentUser);

            try
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = store.Certificates;
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
                return signingCert.Count == 0 ? null : signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }

    }
}
