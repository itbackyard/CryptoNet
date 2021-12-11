﻿using CryptoNetLib.helpers;

namespace CryptoNetLib
{
    public interface ICryptoNet
    {
        KeyHelper.KeyType GetKeyType();
        string ExportPublicKey();
        string ExportPrivateKey();
        byte[] Encrypt(string content);
        string Decrypt(byte[] bytes);
        void Save(string filename, byte[] bytes);
        void SaveKey(string filename, string content);
    }
}