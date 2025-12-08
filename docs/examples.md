# CryptoNet – How to Use (AES, DSA, RSA)

This document shows **how to use CryptoNet** with the built-in examples for:

- AES (symmetric encryption)
- DSA (digital signatures)
- RSA (asymmetric encryption + certificates)

All examples live under:

~~~csharp
namespace CryptoNet.Examples;
~~~

---

## 1. AES – Symmetric Encryption (`ExampleAes`)

### 1.1. Encrypt/Decrypt a String with a Self-Generated Key

~~~csharp
// EncryptDecryptWithSelfGeneratedKey
ICryptoNetAes cryptoNet = new CryptoNetAes();
var key = cryptoNet.GetKey();

ICryptoNet encryptClient = new CryptoNetAes(key);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetAes(key);
var decrypt = decryptClient.DecryptToString(encrypt);
~~~

### 1.2. Generate & Save AES Key to File

~~~csharp
ICryptoNetAes cryptoNet = new CryptoNetAes();
var file = new FileInfo(SymmetricKeyFile);
cryptoNet.SaveKey(file);

var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);

ICryptoNet cryptoNetKeyImport = new CryptoNetAes(file);
var decrypt = cryptoNetKeyImport.DecryptToString(encrypt);
~~~

### 1.3. Encrypt/Decrypt Using Your Own Key and IV

~~~csharp
var key = Encoding.UTF8.GetBytes("32-char-string-key................");
var iv  = Encoding.UTF8.GetBytes("16-char-secret..");

ICryptoNet encryptClient = new CryptoNetAes(key, iv);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetAes(key, iv);
var decrypt = decryptClient.DecryptToString(encrypt);
~~~

### 1.4. Human-Readable Key + IV

~~~csharp
var symmetricKey = GenerateUniqueKey("symmetricKey");
var secret = new string(GenerateUniqueKey("password").Take(16).ToArray());

ICryptoNet crypto = new CryptoNetAes(
    Encoding.UTF8.GetBytes(symmetricKey),
    Encoding.UTF8.GetBytes(secret));
~~~

### 1.5. Encrypt/Decrypt File

~~~csharp
byte[] fileBytes = File.ReadAllBytes(filename);
var encrypt = encryptClient.EncryptFromBytes(fileBytes);
var decrypt = decryptClient.DecryptToBytes(encrypt);
~~~

---

## 2. DSA – Digital Signatures (`ExampleDsa`)

### 2.1. Sign & Verify with Self-Generated Key

~~~csharp
ICryptoNetDsa client = new CryptoNetDsa();
var privateKey = client.GetKey(true);

var signature = new CryptoNetDsa(privateKey).CreateSignature(ConfidentialDummyData);

var verified = new CryptoNetDsa(privateKey)
    .IsContentVerified(ExtShared.ExtShared.StringToBytes(ConfidentialDummyData), signature);
~~~

### 2.2. Generate, Save, and Reuse Keys

~~~csharp
cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);

var signature = new CryptoNetDsa(new FileInfo(PrivateKeyFile))
    .CreateSignature(ConfidentialDummyData);

var verified = new CryptoNetDsa(new FileInfo(PublicKeyFile))
    .IsContentVerified(ExtShared.ExtShared.StringToBytes(ConfidentialDummyData), signature);
~~~

---

## 3. RSA – Asymmetric Encryption (`ExampleRsa`)

### 3.1. Self-Generated RSA Keys

~~~csharp
ICryptoNetRsa cryptoNet = new CryptoNetRsa();
var privateKey = cryptoNet.GetKey(true);
var publicKey = cryptoNet.GetKey(false);

var encrypt = new CryptoNetRsa(publicKey).EncryptFromString(ConfidentialDummyData);
var decrypt = new CryptoNetRsa(privateKey).DecryptToString(encrypt);
~~~

### 3.2. Save RSA Keys & Reuse

~~~csharp
cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);

var encrypt = new CryptoNetRsa(new FileInfo(PublicKeyFile)).EncryptFromString(ConfidentialDummyData);
var decrypt = new CryptoNetRsa(new FileInfo(PrivateKeyFile)).DecryptToString(encrypt);
~~~

### 3.3. Encrypt with Public Key, Decrypt with Private Key

~~~csharp
ICryptoNet pubKeyClient = new CryptoNetRsa(new FileInfo(PublicKeyFile));
var encrypted = pubKeyClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet priKeyClient = new CryptoNetRsa(new FileInfo(PrivateKeyFile));
var decrypted = priKeyClient.DecryptToString(encrypted);
~~~

### 3.4. Use X509 Certificate for RSA Encryption

~~~csharp
var cert = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

var encrypt = new CryptoNetRsa(cert, KeyType.PublicKey)
    .EncryptFromString(ConfidentialDummyData);

var decrypt = new CryptoNetRsa(cert, KeyType.PrivateKey)
    .DecryptToString(encrypt);
~~~

### 3.5. Export RSA Public Key from Certificate

~~~csharp
X509Certificate2? certificate = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

ICryptoNetRsa certClient = new CryptoNetRsa(certificate, KeyType.PublicKey);
var publicKey = certClient.GetKey(isPrivate: false);
~~~

### 3.6. Working with Special Characters

~~~csharp
string confidentialWithSpecialChars = "Top secret 😃😃";

ICryptoNetRsa cryptoNet = new CryptoNetRsa();
var privateKeyXml = cryptoNet.GetKey(isPrivate: true);
var publicKeyXml = cryptoNet.GetKey(isPrivate: false);

ICryptoNet encryptClient = new CryptoNetRsa(publicKeyXml);
var encrypted = encryptClient.EncryptFromBytes(Encoding.UTF8.GetBytes(confidentialWithSpecialChars));

ICryptoNet decryptClient = new CryptoNetRsa(privateKeyXml);
var decryptedBytes = decryptClient.DecryptToBytes(encrypted);
var decryptedString = Encoding.UTF8.GetString(decryptedBytes);
~~~

### 3.7. PEM Import/Export (Advanced)
> Note: This example requires the CryptoNet.Extensions package. and still in development.

~~~csharp
// Export PEM, encrypted PEM, import PEM (with/without password)
char[] pubPem = ExampleRsa.ExportPemKey(certificate, privateKey: false);
char[] priPem = ExampleRsa.ExportPemKey(certificate);
byte[] encryptedPriPem = ExampleRsa.ExportPemKeyWithPassword(certificate, "password");

ICryptoNet importedFromPub = ExampleRsa.ImportPemKey(pubPem);
ICryptoNet importedFromPri = ExampleRsa.ImportPemKey(priPem);
ICryptoNet importedFromEncryptedPri = ExampleRsa.ImportPemKeyWithPassword(encryptedPriPem, "password");
~~~

---

## Running All Examples (quick reference)

~~~csharp
using CryptoNet.Examples;

// AES
ExampleAes.EncryptDecryptWithSelfGeneratedKey();
ExampleAes.GenerateAndSaveSymmetricKey();
ExampleAes.EncryptDecryptWithProvidedSymmetricKey();
ExampleAes.EncryptDecryptWithHumanReadableKeySecret();
ExampleAes.EncryptAndDecryptFileWithSymmetricKeyTest("TestFiles\\test.docx");

// DSA
ExampleDsa.SignAndValidateWithSelfGeneratedKey();
ExampleDsa.GenerateAndSaveDsaKeyPair();

// RSA
ExampleRsa.EncryptDecryptWithSelfGeneratedKey();
ExampleRsa.GenerateAndSaveAsymmetricKey();
ExampleRsa.EncryptWithPublicKeyAndDecryptWithPrivateKey();
ExampleRsa.UseX509Certificate();
ExampleRsa.ExportPublicKeyFromX509Certificate();
ExampleRsa.WorkWithSpecialCharacterText();
ExampleRsa.CustomizePemExamples();
~~~

---

## Examples coverage checklist

~~~text
[ ] AES - EncryptDecryptWithSelfGeneratedKey
[ ] AES - GenerateAndSaveSymmetricKey
[ ] AES - EncryptDecryptWithProvidedSymmetricKey
[ ] AES - EncryptDecryptWithHumanReadableKeySecret
[ ] AES - EncryptAndDecryptFileWithSymmetricKeyTest
[ ] DSA - SignAndValidateWithSelfGeneratedKey
[ ] DSA - GenerateAndSaveDsaKeyPair
[ ] RSA - EncryptDecryptWithSelfGeneratedKey
[ ] RSA - GenerateAndSaveAsymmetricKey
[ ] RSA - EncryptWithPublicKeyAndDecryptWithPrivateKey
[ ] RSA - UseX509Certificate
[ ] RSA - ExportPublicKeyFromX509Certificate
[ ] RSA - WorkWithSpecialCharacterText
[ ] RSA - CustomizePemExamples
~~~

## End