using System;
using Xunit;

namespace Humpback.Tests
{
    public class DynamicTests
    {


        [Fact]
        public void TestForArray()
        {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.NotNull(e.things.Length);
        }

        [Fact]
        public void TestForArrayOnString()
        {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.NotNull(e.name.Length);
        }

        [Fact]
        public void TestForString()
        {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.Equal(e.name.GetType(), typeof(string));
        }

        [Fact]
        public void TestForStringOnArray()
        {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = "jason", things = new[] { "stuff", "morestuff" } };

            Assert.NotEqual(e.things.GetType(), typeof(string));
        }

        [Fact]
        public void TestForTypeOfArray()
        {

            //dynamic d = new { name = "jason", things = new{val="stuff"} }; // throws error testing for Length property
            dynamic e = new { name = new { name2 = "jason" }, things = new[] { new { name2 = "jason" }, new { name2 = "jason" } } };

            Console.WriteLine(e.name.GetType());
            Console.WriteLine(e.things.GetType());
            Assert.False(e.name.GetType().ToString().EndsWith("[]"));
            Assert.True(e.things.GetType().ToString().EndsWith("[]"));
        }


    }
}
