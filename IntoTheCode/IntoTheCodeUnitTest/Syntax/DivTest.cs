
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCode.Buffer;

namespace TestCodeInternal.UnitTest
{
    [TestClass]
    public class DivTest
    {
        [TestMethod]
        public void Parser01Buffer()
        {
            string str = @"Firstline
SecondlineX
Thirdline
 Z";
            var buffer = new FlatBuffer(str);
            int pos = str.IndexOf("F");
            Assert.AreEqual("Line 1, colomn 1", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find F");
            pos = str.IndexOf("X");
            Assert.AreEqual("Line 2, colomn 11", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find X");
            pos = str.IndexOf("Z");
            Assert.AreEqual("Line 4, colomn 2", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find Z");
        }
    }
}
