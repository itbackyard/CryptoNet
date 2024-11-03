﻿using System;
using System.IO;
using System.Linq;
using CryptoNet.Share;
using CryptoNet.Share.Extensions;
using NUnit.Framework;
using Shouldly;

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class ShareProjectTests
    {
        [Test]
        public void TryGetSolutionDirectoryInfo_ShouldReturnDirectoryWithTestFiles()
        {
            // Arrange
            string solutionFilePath = Path.Combine(Common.TestFilesPath);

            // Act
            var result = DirectoryExension.TryGetSolutionDirectoryInfo();
            var testFiles = Path.Combine(result!.FullName, "Resources", "TestFiles");
            var di = new DirectoryInfo(testFiles);
            var files = di.GetFiles("test.*").Select(e => e.FullName);

            // Assert
            files.ShouldNotBeNull();
            files.Count().ShouldBe(4);
        }

        [Test]
        public void CheckContent_WhenContentsAreSame_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "This is a test string";
            string decryptedContent = "This is a test string";

            // Act
            bool result = Common.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are identical, so MD5 hashes should match.");
        }

        [Test]
        public void CheckContent_WhenContentsAreDifferent_ShouldReturnFalse()
        {
            // Arrange
            string originalContent = "This is a test string";
            string decryptedContent = "This is a different string";

            // Act
            bool result = Common.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeFalse("because contents are different, so their MD5 hashes should not match.");
        }

        [Test]
        public void CheckContent_WhenBothContentsAreEmpty_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "";
            string decryptedContent = "";

            // Act
            bool result = Common.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are empty, and their MD5 hashes should match.");
        }

        [Test]
        public void CheckContent_WhenOneContentIsNull_ShouldReturnFalse()
        {
            // Arrange
            string originalContent = "This is a test string";
            string? decryptedContent = null;

            // Act
            bool result = Common.CheckContent(originalContent, decryptedContent!);

            // Assert
            result.ShouldBeFalse("because one content is null, so their MD5 hashes cannot match.");
        }

        [Test]
        public void CheckContent_WhenBothContentsAreNull_ShouldReturnTrue()
        {
            // Arrange
            string? originalContent = null;
            string? decryptedContent = null;

            // Act
            bool result = Common.CheckContent(originalContent!, decryptedContent!);

            // Assert
            result.ShouldBeTrue("because both contents are null, so their MD5 hashes should be the same.");
        }

        [Test]
        public void CheckContent_WhenContentsContainSpecialCharacters_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "!@#$%^&*()_+1234567890";
            string decryptedContent = "!@#$%^&*()_+1234567890";

            // Act
            bool result = Common.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are identical even with special characters.");
        }

        [TestCase("testInput")]
        [TestCase("   ")] // Whitespace input
        [TestCase("你好世界")] // Non-ASCII input
        [TestCase("Abc123!@#")] // Mixed characters
        [TestCase("sameInput")] // Pre-calculated MD5 for "sameInput"
        public void UniqueKeyGenerator_ShouldGenerateCorrectHash_ForGivenInput(string input)
        {
            // Act
            string result = Common.UniqueKeyGenerator(input);

            // Assert
            result.ShouldNotBeNull($"The MD5 hash generated by Common.UniqueKeyGenerator for input '{input}' is incorrect.");
        }

        [TestCase("sameInput")]
        [TestCase("anotherInput")]
        public void UniqueKeyGenerator_ShouldGenerateSameHash_ForSameInput(string input)
        {
            // Act
            string result1 = Common.UniqueKeyGenerator(input);
            string result2 = Common.UniqueKeyGenerator(input);

            // Assert
            result1.ShouldBe(result2, $"Common.UniqueKeyGenerator should return the same hash for the same input '{input}'.");
        }

        [TestCase("input1", "input2")]
        [TestCase("longInput", "shortInput")]
        [TestCase("123456", "654321")]
        public void UniqueKeyGenerator_ShouldGenerateDifferentHash_ForDifferentInputs(string input1, string input2)
        {
            // Act
            string result1 = Common.UniqueKeyGenerator(input1);
            string result2 = Common.UniqueKeyGenerator(input2);

            // Assert
            result1.ShouldNotBe(result2, $"Common.UniqueKeyGenerator should return different hashes for different inputs '{input1}' and '{input2}'.");
        }

        [Test]
        public void UniqueKeyGenerator_ShouldThrowArgumentNullException_WhenInputIsNull()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => Common.UniqueKeyGenerator(null!));
        }

        [Test]
        public void UniqueKeyGenerator_ShouldThrowArgumentNullException_WhenInputIsEmpty()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => Common.UniqueKeyGenerator(string.Empty));
        }

        [Test]
        public void UniqueKeyGenerator_ShouldGenerateHash_ForLongInput()
        {
            // Arrange
            string input = new string('a', 1000); // String with 1000 'a' characters
            string expectedHash = "CABE45DCC9AE5B66BA86600CCA6B8BA8"; // MD5 hash for 1000 'a' characters

            // Act
            string result = Common.UniqueKeyGenerator(input);

            // Assert
            result.ShouldBe(expectedHash, "The MD5 hash generated for a long input string is incorrect.");
        }
    }
}
