using NUnit.Framework;
using TG.Common;

namespace UnitTests
{
    public class TestAssemblyInfo
    {
        [SetUp]
        public void Setup()
        {
            AssemblyInfo.ReferenceAssembly = typeof(TestAssemblyInfo).Assembly;
        }

        [Test]
        public void TestVersion()
        {
            Assert.AreEqual(AssemblyInfo.VersionString, "1.2.3.4");
        }

        [Test]
        public void TestInformationVersion()
        {
            Assert.AreEqual(AssemblyInfo.InformationVersion, "1.2 Test");
        }

        [Test]
        public void TestDescription()
        {
            Assert.AreEqual(AssemblyInfo.Description, "Unit Tests");
        }
    }
}