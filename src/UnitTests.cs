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
        [Test]
        public void testDirectoryDepth()
        {
            FileSystem fs = new FileSystem("C:\\");
            Assert.AreEqual(fs.calculateDirectoryDepth(), 0);
        }
    }
}
