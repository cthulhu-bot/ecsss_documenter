using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ECSSS_Documenter.src
{
    [TestFixture]
    class DocumenterTester
    {
        public static string[] targetDirectories = { 
                                                       "web"
                                                       ,"src"
                                                       ,"pages"
                                                   };
        public static string[] targetExtensions = {
                                                      ".js"
                                                      ,".cs"
                                                      ,".query"
                                                  };
        [Test]
        public void testDirectoryDepth()
        {
            FileSystem fs = new FileSystem("C:\\", targetExtensions, targetDirectories);
            Assert.AreEqual(fs.calculateDirectoryDepth(), 0);
        }
    }
}
