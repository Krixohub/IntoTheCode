using IntoTheCode;
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Read.Element.Words
{
    [TestClass]
    public class WordMixTest
    {
        [TestMethod]
        public void LoadWord()
        {
            //// test loading of basic grammar elements
            var outNo = new List<CodeElement>();
            var idn = new WordIdent("kurt");
            var str = new WordString();
            var sym = new WordSymbol("symbol1");

            // load string + string + name
            TextBuffer textBuffer = Util.NewBufferWs("  Aname     symbol1      'Fghij'      sym02  ");
            idn.TextBuffer = textBuffer;
            sym.TextBuffer = textBuffer;
            str.TextBuffer = textBuffer;
            idn.TextBuffer = textBuffer;
            Assert.AreEqual(true, idn.Load(outNo, 0), "Can't read a combinded Identifier");
            Assert.AreEqual(true, sym.Load(outNo, 0), "Can't read a combinded Symbol");
            Assert.AreEqual(true, str.Load(outNo, 0), "Can't read a combinded String");
            Assert.AreEqual(true, idn.Load(outNo, 0), "Can't read a combinded Identifier");
            CodeElement node = outNo[1] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded Quote");
            Assert.AreEqual("Fghij", node.Value, "The combinded Quote value is not correct");
            node = outNo[2] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded VarName");
            Assert.AreEqual("sym02", node.Value, "The combinded VarName value is not correct");
            Assert.AreEqual(45, textBuffer.PointerNextChar, "The buffer pointer is of after reading combinded values");
        }
    }
}
