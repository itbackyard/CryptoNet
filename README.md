![Cryptonet](https://raw.githubusercontent.com/maythamfahmi/CryptoNet/main/img/CryptoNetLogo.svg)

[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet?color=blue)](https://www.nuget.org/packages/CryptoNet/)
[![CryptoNet NuGet pre-release version](https://img.shields.io/nuget/vpre/CryptoNet)](https://www.nuget.org/packages/CryptoNet/)

CryptoNet is a simple and a lightweight symmetric and asymmetric RSA encryption NuGet library. 
It is a 100% native C# implementation based on RSACryptoServiceProvider class.

## Installation

You can download CryptoNet via [NuGet](https://www.nuget.org/packages/CryptoNet/).

## Versions

![Nuget](https://img.shields.io/nuget/v/cryptonet?style=social) is latest version and are maintained. 

## Issues

Please report issues [here](https://github.com/maythamfahmi/CryptoNet/issues).

## Using / Documentation

### Short intro

There are 2 ways to encrypt and decrypt content:
 1. Symmetric way (By default): 
    - Use same key to encrypt and decrypt.
 3. Asymmetric way
    - You have 2 keys, Private and Public key.
    - Use Public key to encrypt.
    - Use Private key to decrypt.
    - You need to generate RSA key pair first (Private/Public key).

You find the comlete and all [examples](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNetCmd/Example.cs) here.

Here is some of the examples:

### Example: Encrypt and Decrypt with Symmetric Key
```csharp
ICryptoNet encryptClient = new CryptoNet(SymmetricKey);
Console.WriteLine("1- We will encrypt following:");
Console.WriteLine(ConfidentialDummyData);

var encrypted = encryptClient.EncryptFromString(ConfidentialDummyData);
Console.WriteLine("2- To:");
Console.WriteLine(CryptoNetUtils.BytesToString(encrypted));

ICryptoNet decryptClient = new CryptoNet(SymmetricKey);
var decrypted = decryptClient.DecryptToString(encrypted);
Console.WriteLine("3- And we will decrypt it back with correct key:");
Console.WriteLine(decrypted);

ICryptoNet decryptClientWithWrongKey = new CryptoNet("wrong key");
var decryptWithWrongKey = decryptClientWithWrongKey.DecryptToString(encrypted);
Console.WriteLine("4- And we will not be able decrypt it back with wrong key:");
Console.WriteLine(decryptWithWrongKey);
```

### Example: Generate and Export Asymmetric Key (Private/Public) Key (RasKeyPair)
```csharp
ICryptoNet cryptoNet = new CryptoNet("My-Secret-Key");

CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

var certificate = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
```

### Example: Encrypt with Public Key and later Decrypt with Private Key
```csharp
var certificate = CryptoNetUtils.LoadFileToString(RsaKeyPair);
// Export public key
ICryptoNet cryptoNet = new CryptoNet(certificate, true);
var publicKey = cryptoNet.ExportPublicKey();
CryptoNetUtils.SaveKey(PublicKeyFile, publicKey);

// Import public key and encrypt
var importPublicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNet(importPublicKey, true);
var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.EncryptFromString(ConfidentialDummyData);
Console.WriteLine("1- This time we use a certificate public key to encrypt");
Console.WriteLine(CryptoNetUtils.BytesToString(encryptWithPublicKey));

ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNet(certificate, true);
var decryptWithPrivateKey = cryptoNetDecryptWithPublicKey.DecryptToString(encryptWithPublicKey);
Console.WriteLine("6- And use the same certificate to decrypt");
Console.WriteLine(decryptWithPrivateKey);
```


## Contributing

I need your help, so if you have good knowledge of C# and Cryptography just grab one of the issues and add a pull request.
The same is valid, if you have idea for improvement or adding new feature.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not a ware of how to do it.
