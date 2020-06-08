using IntoTheCode;
using IntoTheCode.Buffer;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Read.Words
{
    [TestClass]
    public class WordMixTest
    {
        [TestMethod]
        public void ITC11LoadWord()
        {
            //// test loading of basic grammar elements
            var outNo = new List<TextElement>();
            var idn = new WordIdent();
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
            textBuffer.FindNextWord(null, false);
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
