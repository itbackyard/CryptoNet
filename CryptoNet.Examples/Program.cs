using CryptoNet.Examples;

ExampleDsa.SignAndValidateWithSelfGeneratedKey();
ExampleDsa.GenerateAndSaveDsaKeyPair();

ExampleAes.EncryptDecryptWithSelfGeneratedSymmetricKey();
ExampleAes.GenerateAndSaveSymmetricKey();
ExampleAes.EncryptDecryptWithProvidedSymmetricKey();
ExampleAes.EncryptDecryptWithHumanReadableKeySecret();
ExampleAes.EncryptAndDecryptFileWithSymmetricKeyTest("TestFiles\\test.docx");
ExampleAes.EncryptAndDecryptFileWithSymmetricKeyTest("TestFiles\\test.xlsx");
ExampleAes.EncryptAndDecryptFileWithSymmetricKeyTest("TestFiles\\test.pdf");
ExampleAes.EncryptAndDecryptFileWithSymmetricKeyTest("TestFiles\\test.png");

// Updated RSA example method names to follow PascalCase and remove underscores
ExampleRsa.EncryptDecryptWithSelfGeneratedKey();
ExampleRsa.GenerateAndSaveAsymmetricKey();
ExampleRsa.EncryptWithPublicKeyAndDecryptWithPrivateKey();
ExampleRsa.UseX509Certificate();
ExampleRsa.ExportPublicKeyFromX509Certificate();
ExampleRsa.WorkWithSpecialCharacterText();
ExampleRsa.CustomizePemExamples();


