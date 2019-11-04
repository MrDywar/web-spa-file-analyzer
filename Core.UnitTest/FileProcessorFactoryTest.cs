﻿using System;
using System.Collections.Generic;
using Core.FilePrcossor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.UnitTest
{
    [TestClass]
    public class FileProcessorFactoryTest
    {
        private static IFileProcessorFactory _factory;

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            _factory = new FileProcessorFactory();
        }

        [TestMethod]
        [DynamicData(nameof(GetSupportedData), DynamicDataSourceType.Method)]
        public void Create_Supported_ReturnsProcessor(string fileExtension, Type type)
        {
            var fileProcessor = _factory.Create(fileExtension);

            Assert.IsNotNull(fileProcessor);
            Assert.AreEqual(fileProcessor.GetType(), type);
        }

        [TestMethod]
        [DynamicData(nameof(GetUnsupportedData), DynamicDataSourceType.Method)]
        public void Create_Unsupported_ThrowNotSupportedException(string fileExtension)
        {
            Action action = () => _factory.Create(fileExtension);

            Assert.ThrowsException<NotSupportedException>(action);
        }

        [TestMethod]
        [DynamicData(nameof(GetSupportedData), DynamicDataSourceType.Method)]
        public void TryCreate_Supported_ReturnsProcessor(string fileExtension, Type type)
        {
            var isSuccessful = _factory.TryCreate(fileExtension, out var fileProcessor);

            Assert.IsTrue(isSuccessful);
            Assert.IsNotNull(fileProcessor);
            Assert.AreEqual(fileProcessor.GetType(), type);
        }

        [TestMethod]
        [DynamicData(nameof(GetUnsupportedData), DynamicDataSourceType.Method)]
        public void TryCreate_Unsupported_ThrowNotSupportedException(string fileExtension)
        {
            var isSuccessful = _factory.TryCreate(fileExtension, out var fileProcessor);

            Assert.IsFalse(isSuccessful);
            Assert.IsNull(fileProcessor);
        }

        public static IEnumerable<object[]> GetSupportedData()
        {
            yield return new object[] { FileProcessorFactory.TextFileExtension, typeof(TextFileProcessor) };
        }

        public static IEnumerable<object[]> GetUnsupportedData()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
            yield return new object[] { "dat" };
        }
    }
}
