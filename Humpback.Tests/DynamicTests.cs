using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Humpback.Parts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Humpback.Tests {
    [TestClass]
    public class DynamicTests {


        [TestMethod]
        public void TestForArray() {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[]{"stuff","morestuff"} };

            Assert.IsNotNull(e.things.Length);
        }

        [TestMethod]
        public void TestForArrayOnString() {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.IsNotNull(e.name.Length);
        }

        [TestMethod]
        public void TestForString() {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.AreEqual(e.name.GetType(), typeof(string));
        }

        [TestMethod]
        public void TestForStringOnArray() {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.AreNotEqual(e.things.GetType(), typeof(string));
        }

        [TestMethod]
        public void TestForTypeOfArray() {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = new { name2 = "jason" }, things = new[] { new { name2 = "jason" }, new { name2 = "jason" } } };

            Console.WriteLine(e.name.GetType());
            Console.WriteLine(e.things.GetType());
            Assert.IsFalse(e.name.GetType().ToString().EndsWith("[]"));
            Assert.IsTrue(e.things.GetType().ToString().EndsWith("[]"));
        }


    }
}
