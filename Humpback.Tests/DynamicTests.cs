using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Humpback.Tests {
    [TestClass]
    public class DynamicTests {
        [TestMethod]
        public void TestForArrayAttempts() {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[]{"stuff","morestuff"} };

            Assert.IsNotNull(e.things.Length);
        }
    }
}
