using System;
using IntoTheCode.Message;
using IntoTheCode.Grammar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCodeUnitTest.Read;

namespace Read.Words
{
    [TestClass]
    public class WordBoolTest
    {
        [TestMethod]
        public void ITC10Load()
        {
            var word = new WordBool();
            Util.ParserLoadWord(word, "  true  ", "true", "bool", 2, 6, 8);
            //             123456789

            Util.ParserLoadWord(word, "  false ", "false", "bool", 2, 7, 8);
            //             123456789

            Util.ParserLoadWord(word, "  true", "true", "bool", 2, 6, 6);
            //             123456789
        }

        [TestMethod]
        public void ITC10LoadError()
        {
            var word = new WordBool();

            //                 "1234567890123456
            Util.WordLoadError("  tru4567  ", word, "bool not true",
                "itc10: Syntax error (testRule). Expecting bool, found 'tru'. Line 1, colomn 3");

            Util.WordLoadError("  fals56  ", word, "bool not false",
                "itc10: Syntax error (testRule). Expecting bool, found 'fals'. Line 1, colomn 3");

            //                 "1234567890123456
            Util.WordLoadError("   ", word, "bool not found",
                "itc10: Syntax error (testRule). Expecting bool, found EOF. Line 1, colomn 4");

        }
    }
}
