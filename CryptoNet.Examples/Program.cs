//using CryptoNet;
//using System.Diagnostics;

//const string ConfidentialDummyData = "Top secret 🤣🤣"; //

//ICryptoNetAes cryptoNet = new CryptoNetAes();
//var key = cryptoNet.GetKey();

//ICryptoNet encryptClient = new CryptoNetAes(key);
//var encrypted = encryptClient.EncryptFromBytes(ConfidentialDummyData);

//ICryptoNet decryptClient = new CryptoNetAes(key);
//var decrypted = decryptClient.DecryptToString(encrypted);

//Debug.Assert(ConfidentialDummyData == decrypted);

using CryptoNet.Examples;

ExampleDsa.Example_1_Sign_Validate_Content_With_SelfGenerated_AsymmetricKey();
ExampleDsa.Example_2_SelfGenerated_And_Save_AsymmetricKey();

//ExampleAes.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey();
//ExampleAes.Example_2_SelfGenerated_And_Save_SymmetricKey();
//ExampleAes.Example_3_Encrypt_Decrypt_Content_With_Own_SymmetricKey();
//ExampleAes.Example_4_Encrypt_Decrypt_Content_With_Human_Readable_Key_Secret_SymmetricKey();
//ExampleAes.Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test("TestFiles\\test.docx");
//ExampleAes.Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test("TestFiles\\test.xlsx");
//ExampleAes.Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test("TestFiles\\test.pdf");
//ExampleAes.Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test("TestFiles\\test.png");

//ExampleRsa.Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey();
//ExampleRsa.Example_2_SelfGenerated_And_Save_AsymmetricKey();
//ExampleRsa.Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content();
//ExampleRsa.Example_4_Using_X509_Certificate();
//ExampleRsa.Example_5_Export_Public_Key_For_X509_Certificate();
//ExampleRsa.Example_7_Customize();


