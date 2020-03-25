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
    }
}
