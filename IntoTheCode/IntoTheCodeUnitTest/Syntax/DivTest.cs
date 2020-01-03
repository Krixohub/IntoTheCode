
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
            int line, column;
            var buffer = new FlatBuffer(str);
            int pos = str.IndexOf("F");
            buffer.GetLineAndColumn(out line, out column, new FlatPointer() { index = pos });
            Assert.AreEqual(1, line, "find F line");
            Assert.AreEqual(1, column, "find F col");

            pos = str.IndexOf("X");
            buffer.GetLineAndColumn(out line, out column, new FlatPointer() { index = pos });
            Assert.AreEqual(2, line, "find X line");
            Assert.AreEqual(11, column, "find X col");
            //Assert.AreEqual("Line 2, colomn 11", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find X");

            pos = str.IndexOf("Z");
            buffer.GetLineAndColumn(out line, out column, new FlatPointer() { index = pos });
            Assert.AreEqual(4, line, "find Z line");
            Assert.AreEqual(2, column, "find Z col");
//            Assert.AreEqual("Line 4, colomn 2", buffer.GetLineAndColumn(new FlatPointer() { index = pos }), "find Z");
        }
    }
}
