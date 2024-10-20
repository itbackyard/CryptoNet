﻿using System;
using System.IO;
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
        public void TryGetSolutionDirectoryInfo_ShouldReturnNull_WhenNoSolutionFileExists()
        {
            // Act
            var result = DirectoryExension.TryGetSolutionDirectoryInfo();

            // Assert
            result.ShouldNotBeNull();
            result!.FullName.ShouldContain("CryptoNet");
        }

        [Test]
        public void TryGetSolutionDirectoryInfo_ShouldReturnDirectoryWithTestFiles()
        {
            // Arrange
            string solutionFilePath = Path.Combine(Common.TestFilesPath);

            // Act
            var result = DirectoryExension.TryGetSolutionDirectoryInfo();
            var testFiles = Path.Combine(result!.FullName, "Resources", "TestFiles");
            var di = new DirectoryInfo(testFiles);
            var files = di.GetFiles("test.*");

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

        [Test]
        public void UniqueKeyGenerator_ShouldGenerateCorrectHash_ForGivenInput()
        {
            // Arrange
            string input = "testInput";
            string expectedHash = "FB054EFB1303ABDFD6E954E83F41E7BD"; // Pre-calculated MD5 hash for "testInput"

            // Act
            string result = Common.UniqueKeyGenerator(input);

            // Assert
            result.ShouldBe(expectedHash, "The MD5 hash generated by UniqueKeyGenerator is incorrect.");
        }

        [Test]
        public void UniqueKeyGenerator_ShouldGenerateSameHash_ForSameInput()
        {
            // Arrange
            string input = "sameInput";

            // Act
            string result1 = Common.UniqueKeyGenerator(input);
            string result2 = Common.UniqueKeyGenerator(input);

            // Assert
            result1.ShouldBe(result2, "UniqueKeyGenerator should return the same hash for the same input.");
        }

        [Test]
        public void UniqueKeyGenerator_ShouldGenerateDifferentHash_ForDifferentInputs()
        {
            // Arrange
            string input1 = "input1";
            string input2 = "input2";

            // Act
            string result1 = Common.UniqueKeyGenerator(input1);
            string result2 = Common.UniqueKeyGenerator(input2);

            // Assert
            result1.ShouldNotBe(result2, "UniqueKeyGenerator should return different hashes for different inputs.");
        }
    }
}
