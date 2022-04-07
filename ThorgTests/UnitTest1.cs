using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GolemUI.Utils;

namespace ThorgTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(MathUtils.RoundToInt(1.2), 2.0);

        }
    }
}
