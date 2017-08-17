using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleExtension.Config;
using Newtonsoft.Json.Linq;

namespace ExtensionUnitTests
{
    [TestClass]
    public class ExtensionConfigTests
    {
        [TestMethod]
        public void TestConfig()
        {
            // Test that the extension config serializes as json 
            // This is important because extension config can be pulled from host.json. 
            var obj = new JObject();
            var root = @"c:\root";
            obj["Root"] = root;

            var ext = obj.ToObject<SampleExtensions>();

            Assert.AreEqual(root, ext.Root);
        }
    }
}
