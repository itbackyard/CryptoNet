# CryptoNet – How to Use (AES, DSA, RSA)

This document shows **how to use CryptoNet** with the built-in examples for:

- AES (symmetric encryption)
- DSA (digital signatures)
- RSA (asymmetric encryption + certificates)

All examples live under:

```csharp
namespace CryptoNet.Examples;
```

and use the shared dummy content:

```csharp
private const string ConfidentialDummyData = @"Some Secret Data";
```

---

## 1. AES – Symmetric Encryption (`ExampleAes`)

### 1.1. Encrypt/Decrypt a String with a Self-Generated Key

```csharp
// Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey
ICryptoNetAes cryptoNet = new CryptoNetAes();
var key = cryptoNet.GetKey();

ICryptoNet encryptClient = new CryptoNetAes(key);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetAes(key);
var decrypt = decryptClient.DecryptToString(encrypt);
```

Run:

```csharp
ExampleAes.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey();
```

---

### 1.2. Generate & Save AES Key to File

```csharp
ICryptoNetAes cryptoNet = new CryptoNetAes();
var file = new FileInfo(SymmetricKeyFile);
cryptoNet.SaveKey(file);

var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);

ICryptoNet cryptoNetKeyImport = new CryptoNetAes(file);
var decrypt = cryptoNetKeyImport.DecryptToString(encrypt);
```

Run:

```csharp
ExampleAes.Example_2_SelfGenerated_And_Save_SymmetricKey();
```

---

### 1.3. Encrypt/Decrypt Using Your Own Key and IV

```csharp
var key = Encoding.UTF8.GetBytes("32-char-string-key................");
var iv  = Encoding.UTF8.GetBytes("16-char-secret..");

ICryptoNet encryptClient = new CryptoNetAes(key, iv);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetAes(key, iv);
var decrypt = decryptClient.DecryptToString(encrypt);
```

Run:

```csharp
ExampleAes.Example_3_Encrypt_Decrypt_Content_With_Own_SymmetricKey();
```

---

### 1.4. Human-Readable Key + IV

```csharp
var symmetricKey = UniqueKeyGenerator("symmetricKey");
var secret = new string(UniqueKeyGenerator("password").Take(16).ToArray());

ICryptoNet crypto = new CryptoNetAes(
    Encoding.UTF8.GetBytes(symmetricKey),
    Encoding.UTF8.GetBytes(secret));
```

Run:

```csharp
ExampleAes.Example_4_Encrypt_Decrypt_Content_With_Human_Readable_Key_Secret_SymmetricKey();
```

---

### 1.5. Encrypt/Decrypt File

```csharp
byte[] fileBytes = File.ReadAllBytes(filename);
var encrypt = encryptClient.EncryptFromBytes(fileBytes);
var decrypt = decryptClient.DecryptToBytes(encrypt);
```

Run:

```csharp
ExampleAes.Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test("file.pdf");
```

---

## 2. DSA – Digital Signatures (`ExampleDsa`)

### 2.1. Sign & Verify with Self-Generated Key

```csharp
ICryptoNetDsa client = new CryptoNetDsa();
var privateKey = client.GetKey(true);

var signature = new CryptoNetDsa(privateKey).CreateSignature(ConfidentialDummyData);

var verified = new CryptoNetDsa(privateKey)
    .IsContentVerified(ExtShared.ExtShared.StringToBytes(ConfidentialDummyData), signature);
```

Run:

```csharp
ExampleDsa.Example_1_Sign_Validate_Content_With_SelfGenerated_AsymmetricKey();
```

---

### 2.2. Generate, Save, and Reuse Keys

```csharp
cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);

var signature = new CryptoNetDsa(new FileInfo(PrivateKeyFile))
    .CreateSignature(ConfidentialDummyData);

var verified = new CryptoNetDsa(new FileInfo(PublicKeyFile))
    .IsContentVerified(ExtShared.ExtShared.StringToBytes(ConfidentialDummyData), signature);
```

Run:

```csharp
ExampleDsa.Example_2_SelfGenerated_And_Save_AsymmetricKey();
```

---

## 3. RSA – Asymmetric Encryption (`ExampleRsa`)

### 3.1. Self-Generated RSA Keys

```csharp
ICryptoNetRsa cryptoNet = new CryptoNetRsa();
var privateKey = cryptoNet.GetKey(true);
var publicKey = cryptoNet.GetKey(false);

var encrypt = new CryptoNetRsa(publicKey).EncryptFromString(ConfidentialDummyData);
var decrypt = new CryptoNetRsa(privateKey).DecryptToString(encrypt);
```

Run:

```csharp
ExampleRsa.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey();
```

---

### 3.2. Save RSA Keys & Reuse

```csharp
cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);

var encrypt = new CryptoNetRsa(new FileInfo(PublicKeyFile)).EncryptFromString(ConfidentialDummyData);
var decrypt = new CryptoNetRsa(new FileInfo(PrivateKeyFile)).DecryptToString(encrypt);
```

Run:

```csharp
ExampleRsa.Example_2_SelfGenerated_And_Save_AsymmetricKey();
```

---

### 3.3. Encrypt with Public Key, Decrypt with Private Key

Run:

```csharp
ExampleRsa.Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content();
```

---

### 3.4. Use X509 Certificate for RSA Encryption

```csharp
var cert = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

var encrypt = new CryptoNetRsa(cert, KeyType.PublicKey)
    .EncryptFromString(ConfidentialDummyData);

var decrypt = new CryptoNetRsa(cert, KeyType.PrivateKey)
    .DecryptToString(encrypt);
```

Run:

```csharp
ExampleRsa.Example_4_Using_X509_Certificate();
```

---

### 3.5. Export RSA Public Key from Certificate

Run:

```csharp
ExampleRsa.Example_5_Export_Public_Key_For_X509_Certificate();
```

---

### 3.6. PEM Import/Export (Advanced)

Run:

```csharp
ExampleRsa.Example_7_Customize();
```

---

## 4. Running All Examples

```csharp
using CryptoNet.Examples;

ExampleAes.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey();
ExampleDsa.Example_1_Sign_Validate_Content_With_SelfGenerated_AsymmetricKey();
ExampleRsa.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey();
```

---

## End
