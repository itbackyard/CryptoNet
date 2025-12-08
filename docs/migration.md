# Migration Guide: CryptoNet v2.4.0 → v3.4.3

This document describes the main breaking and behavioral changes for the **AES** and **RSA** examples when migrating from **v2.4.0** to **v3.4.3** of CryptoNet.

---
## 1. Common Changes

### 1.1 Utility changes

Some helper methods moved from `CryptoNet.Utils` to `ExtShared.ExtShared`.

- AES: byte array comparison.
- RSA: loading X509 certificates from the store, plus other shared helpers.

You will typically:
- Remove `using CryptoNet.Utils;`
- Add a reference/using for `ExtShared` (e.g. `using ExtShared;` if needed in your project).

---

## 2. AES Migration (ExampleAes)

### 2.1 Overview

**v2.4.0**

~~~csharp
using CryptoNet.Models;
using CryptoNet.Utils;
~~~

**v3.4.3**

~~~csharp
using CryptoNet.Models;
~~~

The AES encryption/decryption API (`ICryptoNet` + `CryptoNetAes`) remains mostly the same. The main breaking changes are related to **key management** and **utility usage**.

### 2.2 AES key management API changes

#### 2.2.1 Generating a key in memory

~~~csharp
// v2.4.0
ICryptoNet cryptoNet = new CryptoNetAes();
var key = cryptoNet.ExportKey();

// v3.4.3
ICryptoNetAes cryptoNet = new CryptoNetAes();
var key = cryptoNet.GetKey();
~~~

**Changes:**
- Interface: `ICryptoNet` → `ICryptoNetAes` for key retrieval.
- Method: `ExportKey()` → `GetKey()`.

**Action:**
- When you need to retrieve the AES key, use `ICryptoNetAes` and `GetKey()`.

You still use `ICryptoNet` for encryption/decryption with the retrieved key:

~~~csharp
ICryptoNet encryptClient = new CryptoNetAes(key);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetAes(key);
var decrypt = decryptClient.DecryptToString(encrypt);
~~~

#### 2.2.2 Saving a symmetric key to file

~~~csharp
// v2.4.0
ICryptoNet cryptoNet = new CryptoNetAes();
var file = new FileInfo(SymmetricKeyFile);
cryptoNet.ExportKeyAndSave(file);

// v3.4.3
ICryptoNetAes cryptoNet = new CryptoNetAes();
var file = new FileInfo(SymmetricKeyFile);
cryptoNet.SaveKey(file);
~~~

**Changes:**
- Interface: `ICryptoNet` → `ICryptoNetAes`.
- Method: `ExportKeyAndSave(file)` → `SaveKey(file)`.

**Action:**
- Update both the interface type and the method name when saving AES keys to disk.

### 2.3 AES Examples 3 & 4 (custom/human-readable keys)

Examples 3 and 4 are functionally identical between v2.4.0 and v3.4.3:

- Example 3: uses user-defined 32-char key and 16-char IV.
- Example 4: generates a human-readable key and IV via `UniqueKeyGenerator`.

No changes are required for the encryption/decryption code in these examples.

### 2.4 File encryption example (Example 5)

#### 2.4.1 Method name

~~~csharp
// v2.4.0
public static void Example_5_Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test(string filename)

// v3.4.3
public static void Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test(string filename)
~~~

**Change:** Method name updated to reflect support for arbitrary files, not just PDFs.

**Action:** Update any callers to use the new method name.

#### 2.4.2 Key retrieval

~~~csharp
// v2.4.0
ICryptoNet cryptoNet = new CryptoNetAes();
var key = cryptoNet.ExportKey();

// v3.4.3
ICryptoNetAes cryptoNet = new CryptoNetAes();
var key = cryptoNet.GetKey();
~~~

Same pattern as in 2.2.1.

#### 2.4.3 Byte array comparison

~~~csharp
// v2.4.0
var isIdenticalFile = CryptoNetUtils.ByteArrayCompare(pdfFileBytes, decrypt);

// v3.4.3
var isIdenticalFile = ExtShared.ExtShared.ByteArrayCompare(pdfFileBytes, decrypt);
~~~

**Change:** The helper moved from `CryptoNetUtils` to `ExtShared.ExtShared`.

**Action:** Replace the call and ensure `ExtShared` is referenced in your project.

### 2.5 UniqueKeyGenerator

The `UniqueKeyGenerator` function is unchanged between versions; no migration is needed here.

### 2.6 AES Migration Checklist

- [ ] Update namespace to `CryptoNet.Examples`.
- [ ] Replace AES key export calls:
  - `ICryptoNet` → `ICryptoNetAes` for key retrieval/saving.
  - `ExportKey()` → `GetKey()`.
  - `ExportKeyAndSave(file)` → `SaveKey(file)`.
- [ ] Update Example 5 method name if you reference it.
- [ ] Replace `CryptoNetUtils.ByteArrayCompare` with `ExtShared.ExtShared.ByteArrayCompare`.
- [ ] Run all AES examples and confirm `Debug.Assert` checks still pass.

---

## 3. RSA Migration (ExampleRsa)

### 3.1 Overview

**v2.4.0**

~~~csharp
using CryptoNet.Models;
using CryptoNet.Utils;

namespace CryptoNet.Cli;
~~~

**v3.4.3**

~~~csharp
using CryptoNet.Models;

namespace CryptoNet.Examples;
~~~

The RSA encryption/decryption calls via `ICryptoNet` remain conceptually the same. The major changes are centralized in **key management APIs** and **certificate utilities**.

### 3.2 RSA key management API changes

#### 3.2.1 Generating keys in memory

~~~csharp
// v2.4.0
ICryptoNet cryptoNet = new CryptoNetRsa();

var privateKey = cryptoNet.ExportKey(true);
var publicKey  = cryptoNet.ExportKey(false);

// v3.4.3
ICryptoNetRsa cryptoNet = new CryptoNetRsa();

var privateKey = cryptoNet.GetKey(true);
var publicKey  = cryptoNet.GetKey(false);
~~~

**Changes:**
- Interface: `ICryptoNet` → `ICryptoNetRsa` for RSA key retrieval.
- Method: `ExportKey(bool)` → `GetKey(bool)`.

**Action:**
- When working with RSA keys (as strings), migrate to `ICryptoNetRsa` and `GetKey(...)`.

Encryption/decryption continues to use `ICryptoNet` with `CryptoNetRsa(key)`:

~~~csharp
ICryptoNet encryptClient = new CryptoNetRsa(publicKey);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetRsa(privateKey);
var decrypt = decryptClient.DecryptToString(encrypt);
~~~

#### 3.2.2 Saving keys to disk

~~~csharp
// v2.4.0
ICryptoNet cryptoNet = new CryptoNetRsa();

cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile),  false);

// v3.4.3
ICryptoNetRsa cryptoNet = new CryptoNetRsa();

cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
cryptoNet.SaveKey(new FileInfo(PublicKeyFile),  false);
~~~

**Changes:**
- Interface: `ICryptoNet` → `ICryptoNetRsa`.
- Method: `ExportKeyAndSave(FileInfo, bool)` → `SaveKey(FileInfo, bool)`.

**Action:**
- Update interface type and method names anywhere you persist RSA keys.

### 3.3 X509 certificate usage

#### 3.3.1 Certificate lookup (Examples 4, 5, 7)

~~~csharp
// v2.4.0
// Find and replace CN=Maytham with your own certificate
X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

// v3.4.3
// Find and replace CN=localhost with your own certificate
X509Certificate2? certificate = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");
~~~

**Changes:**
- Utility: `CryptoNetUtils.GetCertificateFromStore` → `ExtShared.ExtShared.GetCertificateFromStore`.
- Example subject string changed from `"CN=Maytham"` → `"CN=localhost"` (documentation/sample only).

**Action:**
- Replace all `CryptoNetUtils.GetCertificateFromStore` calls with `ExtShared.ExtShared.GetCertificateFromStore`.
- Adjust the subject string to match your certificate (e.g. `"CN=yourcert"`).

This affects:

- `Example_4_Using_X509_Certificate`
- `Example_5_Export_Public_Key_For_X509_Certificate`
- `Example_7_Customize`

#### 3.3.2 Exporting public key from certificate (Example 5)

~~~csharp
// v2.4.0
X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

ICryptoNet cryptoNetWithPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
var publicKey = cryptoNetWithPublicKey.ExportKey(false);

// v3.4.3
X509Certificate2? certificate = ExtShared.ExtShared.GetCertificateFromStore("CN=localhost");

ICryptoNetRsa cryptoNetWithPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
var publicKey = cryptoNetWithPublicKey.GetKey(false);
~~~

**Changes:**
- Certificate retrieval via `ExtShared.ExtShared` instead of `CryptoNetUtils`.
- Interface: `ICryptoNet` → `ICryptoNetRsa` for key extraction.
- Method: `ExportKey(false)` → `GetKey(false)`.

**Action:**
- Use `ICryptoNetRsa` + `GetKey(false)` when exporting the RSA public key from an X509 certificate.

### 3.4 Examples 3 & 7 and helpers

- `Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content`:
  - Constructs `CryptoNetRsa` with key files and uses `ICryptoNet` for encryption/decryption.
  - No signature or behavioral change; no migration needed.

- `Example_7_Customize`:
  - `ExportPemCertificate`, `ExportPemKey`, `ImportPemKey`, `ExportPemKeyWithPassword`, `ImportPemKeyWithPassword` are unchanged.
  - Only difference is certificate fetching:
    - `CryptoNetUtils.GetCertificateFromStore` → `ExtShared.ExtShared.GetCertificateFromStore`.

### 3.5 RSA Migration Checklist

- [ ] Update namespace to `CryptoNet.Examples`.
- [ ] Replace RSA key-related usage:
  - `ICryptoNet` → `ICryptoNetRsa` for key retrieval and saving.
  - `ExportKey(bool)` → `GetKey(bool)`.
  - `ExportKeyAndSave(FileInfo, bool)` → `SaveKey(FileInfo, bool)`.
- [ ] Update all certificate retrieval calls:
  - `CryptoNetUtils.GetCertificateFromStore` → `ExtShared.ExtShared.GetCertificateFromStore`.
- [ ] For certificate-based public key export (Example 5), use `ICryptoNetRsa` and `GetKey(false)`.
- [ ] Run all RSA examples to confirm `Debug.Assert` checks still pass.

---

## 4. Summary

When migrating from **v2.4.0** to **v3.4.3**:

- **Namespaces** moved from `CryptoNet.Cli` to `CryptoNet.Examples` for AES and RSA examples.
- **Key management responsibilities** were split into more specific interfaces:
  - `ICryptoNetAes` for AES keys (`GetKey`, `SaveKey`).
  - `ICryptoNetRsa` for RSA keys (`GetKey`, `SaveKey`).
- **Utility functions** were consolidated into `ExtShared.ExtShared` (byte array comparison, certificate lookup, etc.).
- Core encryption/decryption usage via `ICryptoNet` and `CryptoNetAes`/`CryptoNetRsa` with provided keys remains largely the same.

After updating types and method names as outlined, you should be able to build against **v3.4.3** and run the AES and RSA examples successfully.
