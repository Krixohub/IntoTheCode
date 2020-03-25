using IntoTheCode;
using IntoTheCode.Buffer;
using IntoTheCode.Read.Element.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Read.Element.Words
{
    [TestClass]
    public class WordSymbolTest
    {
        [TestMethod]
        public void Load()
        {
            // load symbol
            var word = new WordSymbol("symbol1");
            Util.WordLoad("  symbol1  ", word, "Abcde", "string", 0, 0, 11);
        }

        [TestMethod]
        public void LoadError()
        {
            string testName = "Symbol";
            var word = new WordSymbol("loop");

            //                 "1234  Read 'symbol', EOF error.
            Util.WordLoadError("   ", word, testName,
                "pe10: Syntax error (testRule). Expecting symbol 'loop', found EOF. Line 1, colomn 4");

            //                 "1234  Read 'symbol', EOF error.
            Util.WordLoadError(" lo", word, testName,
                "pe10: Syntax error (testRule). Expecting symbol 'loop', found EOF. Line 1, colomn 4");

            //                 "12345678  Read 'symbol',  error.
            Util.WordLoadError(" looP  ", word, testName,
                "pe07: Grammar error (testRule). Expecting symbol 'loop', found 'looP' Line 1, colomn 2");
         }

        [TestMethod]
        public void LoadWordMix()
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
