// <copyright file="Example.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNetCmd project</summary>

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;
using CryptoNetLib;
using CryptoNetLib.helpers;

namespace CryptoNetCmd;

public class Example
{
    private const string ConfidentialDummyData = @"Some Secret Data";

    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;

    internal static string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    internal static string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    public static void Main()
    {
        //Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey();
        //Example_2_SelfGenerated_And_Save_AsymmetricKey();
        //Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content();
        //Example_4_Using_X509_Certificate();
        //Example_5_Export_Public_Key_For_X509_Certificate();
        //Example_6_Encrypt_Decrypt_Content_With_SymmetricKey();
        //Example_7_Customize();
        //CiYamlGenerator();
    }

    public static void Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey()
    {
        Console.WriteLine("Example 1");

        ICryptoNet cryptoNet = new CryptoNet();

        var privateKey = cryptoNet.ExportPrivateKey();
        var publicKey = cryptoNet.ExportPublicKey();

        ICryptoNet encryptClient = new CryptoNet(publicKey);
        var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine($"1- We will encrypt following text:\n{ConfidentialDummyData}\n");
        Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encrypt)}\n");

        ICryptoNet decryptClient = new CryptoNet(privateKey);
        var decrypt = decryptClient.DecryptToString(encrypt);
        Console.WriteLine($"3- And we will decrypt it back to:\n{decrypt}\n");
        Console.WriteLine();
    }

    public static void Example_2_SelfGenerated_And_Save_AsymmetricKey()
    {
        Console.WriteLine("Example 2");

        ICryptoNet cryptoNet = new CryptoNet();

        CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
        CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

        var privateKey = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
        Console.WriteLine($"The private key generated and saved to file {PrivateKeyFile}");
        Console.WriteLine(privateKey);

        var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
        Console.WriteLine($"\nThe public key generated and saved to file {PublicKeyFile}");
        Console.WriteLine(publicKey);
        Console.WriteLine();
    }

    // This example depending on example 2
    public static void Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content()
    {
        Console.WriteLine("Example 3");

        ICryptoNet cryptoNetWithPublicKey = new CryptoNet(CryptoNetUtils.LoadFileToString(PublicKeyFile));
        var encryptWithPublicKey = cryptoNetWithPublicKey.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine($"1- We load public key to encrypt following text:\n{ConfidentialDummyData}\n");
        Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encryptWithPublicKey)}\n");

        ICryptoNet cryptoNetWithPrivateKey = new CryptoNet(CryptoNetUtils.LoadFileToString(PrivateKeyFile));
        var decryptWithPrivateKey = cryptoNetWithPrivateKey.DecryptToString(encryptWithPublicKey);
        Console.WriteLine($"3- And we load private key to decrypt it back to:\n{decryptWithPrivateKey}");
    }

    public static void Example_4_Using_X509_Certificate()
    {
        Console.WriteLine("Example 4");

        // Find and replace CN=Maytham with your own certificate
        X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        ICryptoNet cryptoNetWithPublicKey = new CryptoNet(certificate, KeyHelper.KeyType.PublicKey);
        var encryptWithPublicKey = cryptoNetWithPublicKey.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine($"1- We get public key from Certificate to encrypt following text:\n{ConfidentialDummyData}\n");
        Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encryptWithPublicKey)}\n");

        ICryptoNet cryptoNetWithPrivateKey = new CryptoNet(certificate, KeyHelper.KeyType.PrivateKey);
        var decryptWithPrivateKey = cryptoNetWithPrivateKey.DecryptToString(encryptWithPublicKey);
        Console.WriteLine($"3- And we get private key from Certificate to decrypt it back to:\n{decryptWithPrivateKey}");
    }

    public static void Example_5_Export_Public_Key_For_X509_Certificate()
    {
        Console.WriteLine("Example 4");

        // Find and replace CN=Maytham with your own certificate
        X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        ICryptoNet cryptoNetWithPublicKey = new CryptoNet(certificate, KeyHelper.KeyType.PublicKey);
        var publicKey = cryptoNetWithPublicKey.ExportPublicKey();
        Console.WriteLine($"We export public key from Certificate so we can use it on other clients to encrypt content:\n{publicKey}\n");
    }

    public static void Example_6_Encrypt_Decrypt_Content_With_SymmetricKey()
    {
        var symmetricKey = "AnySecretKey";

        ICryptoNet encryptClient = new CryptoNet(symmetricKey, true);
        var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine($"1- We will encrypt following text:\n{ConfidentialDummyData}\n");
        Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encrypt)}\n");

        ICryptoNet decryptClient = new CryptoNet(symmetricKey, true);
        var decrypt = decryptClient.DecryptToString(encrypt);
        Console.WriteLine($"3- And we will decrypt it back to:\n{decrypt}\n");
        Console.WriteLine();
    }

    /// <summary>
    /// CryptoNet interact with .net 5/6 for customization, like import/export PEM
    /// </summary>
    public static void Example_7_Customize()
    {
        X509Certificate2? cert = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");
        var privateKey = cert?.GetRSAPrivateKey();
        var publicKey = cert?.GetRSAPublicKey();

        byte[] certBytes = cert!.RawData;
        char[] certPem = PemEncoding.Write("CERTIFICATE", certBytes);
        Console.WriteLine($"Export PEM Certificate \n{new string(certPem)}");

        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;
        byte[] pubKeyBytes = rsa.ExportSubjectPublicKeyInfo();
        byte[] priKeyBytes = rsa.ExportPkcs8PrivateKey();
        char[] pubKeyPem = PemEncoding.Write("PUBLIC KEY", pubKeyBytes);
        Console.WriteLine($"Export PEM public key \n{new string(pubKeyPem)}");

        char[] priKeyPem = PemEncoding.Write("PRIVATE KEY", priKeyBytes);
        Console.WriteLine($"Export PEM private key \n{new string(priKeyPem)}");

        byte[] password = System.Text.Encoding.UTF8.GetBytes("password");
        byte[] encryptedPriKeyBytes = rsa.ExportEncryptedPkcs8PrivateKey(
            password,
            new PbeParameters(
                PbeEncryptionAlgorithm.Aes256Cbc,
                HashAlgorithmName.SHA256,
                iterationCount: 100_000));
        Console.WriteLine($"Export encrypted PEM private key \n{new string(priKeyPem)}");
        Console.WriteLine(CryptoNetUtils.BytesToString(encryptedPriKeyBytes));

        using var rsa1 = RSA.Create();
        rsa1.ImportEncryptedPkcs8PrivateKey(password, encryptedPriKeyBytes, out _);

        ICryptoNet cryptoNet2 = new CryptoNet();
        RSA rsa2 = cryptoNet2.Rsa;
        rsa2.ImportFromPem(pubKeyPem);

        var encryptContent = cryptoNet2.EncryptFromBytes(CryptoNetUtils.StringToBytes(ConfidentialDummyData));

        ICryptoNet cryptoNet3 = new CryptoNet();
        RSA rsa3 = cryptoNet3.Rsa;
        rsa3.ImportFromPem(priKeyPem);

        var result = cryptoNet3.DecryptToString(encryptContent);

        Console.WriteLine(result);
    }

    public static void CiYamlGenerator()
    {
        var adoClient = new ADotNetClient();

        var aspNetPipeline = new GithubPipeline()
        {
            Name = ".NET",

            OnEvents = new Events()
            {
                Push = new PushEvent()
                {
                    Branches = new[]
                    {
                        "main",
                        "feature/*",
                        "!feature/ci*"
                    }
                },
                PullRequest = new PullRequestEvent()
                {
                    Branches = new[]
                    {
                        "main"
                    }
                }
            },

            Jobs = new Jobs()
            {
                Build = new BuildJob()
                {
                    RunsOn = "ubuntu-latest",
                    Steps = new List<GithubTask>()
                    {
                        new CheckoutTaskV2()
                        {
                            Name = "Checkout",
                            Uses = "actions/checkout@v2"
                        },
                        new SetupDotNetTaskV1()
                        {
                            Name = "Setup .NET",
                            Uses = "actions/setup-dotnet@v1",
                            TargetDotNetVersion = new TargetDotNetVersion()
                            {
                                DotNetVersion = "6.0.x"
                            }
                        },
                        new DotNetBuildTask()
                        {
                            Name = "Build",
                            Run = "dotnet build --configuration Release"
                        },
                        new TestTask()
                        {
                            Name = "Test",
                            Run = "dotnet test --configuration Release --no-build"
                        }
                    }
                }
            }
        };

        var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
        var workflowPath = $"{solutionRoot}\\.github\\workflows";

        string workflowFile = Path.Combine(workflowPath, "ci-auto-generated.yaml");

        adoClient.SerializeAndWriteToFile(aspNetPipeline, workflowFile);
    }


}
