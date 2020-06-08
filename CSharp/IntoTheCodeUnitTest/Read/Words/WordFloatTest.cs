using System;
using IntoTheCode.Message;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Words
{
    [TestClass]
    public class WordFloatTest
    {
        [TestMethod]
        public void ITC10Load()
        {
            var word = new WordFloat();
            Util.ParserLoadWord(word, "  1235.6  ", "1235.6", "float", 2, 8, 10);
            //                         123456789

            Util.ParserLoadWord(word, "  -123.4 ", "-123.4", "float", 2, 8, 9);
            //                         123456789

            Util.ParserLoadWord(word, "  2.345 ", "2.345", "float", 2, 7, 8);
            //                         123456789
            Util.ParserLoadWord(word, "  0.123 ", "0.123", "float", 2, 7, 8);
            //                         123456789
            Util.ParserLoadWord(word, "  -0.12 ", "-0.12", "float", 2, 7, 8);
            //                         123456789
        }

        [TestMethod]
        public void ITC10LoadError()
        {
            var word = new WordFloat();

            //                 "1234567890123456
            Util.WordLoadError("  a234567  ", word, "Float no ciffer",
                "itc10: Syntax error (testRule). Expecting digit, found 'a'. Line 1, colomn 3");

            Util.WordLoadError("  -23456  ", word, "Float no comma",
                "itc10: Syntax error (testRule). Expecting '.', found ' '. Line 1, colomn 10");

            //                 "1234567890123456
            Util.WordLoadError("  123456.  ", word, "Float too long",
                "itc10: Syntax error (testRule). Expecting digit, found ' '. Line 1, colomn 10");

        }
    }
}
