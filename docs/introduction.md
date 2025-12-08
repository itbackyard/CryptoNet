# Introduction

This document gives a short introduction to CryptoNet and how the library is typically used.

Supported targets
- Core libraries target: .NET Standard 2.0 (for maximum compatibility).
- Examples and tests target: .NET 8 and .NET 10.

Overview

CryptoNet supports two primary cryptographic scenarios:

- Symmetric encryption - a single shared secret (key) is used for both encryption and decryption.
- Asymmetric encryption - a public/private key pair is used; the public key encrypts and the private key decrypts.

Symmetric encryption
- Use a secret key (same key for encrypt/decrypt).
- Protect the secret key: anyone with the key can decrypt the data.

Asymmetric encryption
- The library can generate RSA key pairs (private/public) for you, or it can use keys from an X.509 certificate.
- Typical pattern:
  - Use the **Public key** to encrypt data.
  - Use the **Private key** to decrypt data.

Key lifecycle and security notes
- Private keys must be kept confidential. Do not distribute private keys.
- If a private key is leaked, an attacker can decrypt any content encrypted with the corresponding public key. Rotate (revoke and reissue) the key pair if compromise is suspected.
- Conversely, if you lose a private key and you do not have a backup, you will not be able to decrypt content that was encrypted for that key—make secure backups as appropriate.

Using X.509 certificates
- CryptoNet can use the public/private keys stored in X.509 certificates as an alternative to self-generated keys. This can simplify key distribution and lifecycle when using enterprise PKI.

Further reading
- Asymmetric encryption overview: https://www.cloudflare.com/learning/ssl/what-is-asymmetric-encryption/
- Examples and usage (see the examples documentation): `docs/examples.md`

Example code lives under the `CryptoNet.Examples` namespace and demonstrates AES (symmetric), DSA (signatures) and RSA (asymmetric) scenarios.

See also
- Documentation and generation: `docs/tooling.md`
- Examples reference: `docs/examples.md`
