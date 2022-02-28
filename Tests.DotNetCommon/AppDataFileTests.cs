using DotNetCommon.PersistenceHelpers;
using DotNetCommon.SystemHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.DotNetCommon
{
    [TestClass]
    public class AppDataFileTests
    {
        [TestMethod]
        public void WhenConstrucutedWithFilePath_ShouldHaveDirectoryAndFileName()
        {
            string directory = SystemFunctions.CombineDirectoryComponents(Environment.CurrentDirectory, "TestDir");
            string fileName = SystemFunctions.CombineDirectoryComponents(directory, "test.json");
            AppDataFile appDataFile = new AppDataFile(fileName);

            Assert.AreEqual(directory, appDataFile.RootSaveDirectory);
            Assert.AreEqual("test.json", appDataFile.DefaultFileName);
        }
    }
}