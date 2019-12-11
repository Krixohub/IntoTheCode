
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
            Assert.AreEqual("line 1, colomn 0", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find F");
            pos = str.IndexOf("X");
            Assert.AreEqual("line 2, colomn 10", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find X");
            pos = str.IndexOf("Z");
            Assert.AreEqual("line 4, colomn 1", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find Z");
        }
    }
}
