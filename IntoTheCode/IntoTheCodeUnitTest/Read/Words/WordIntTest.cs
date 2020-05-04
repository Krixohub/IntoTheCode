using System;
using IntoTheCode.Message;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Words
{
    [TestClass]
    public class WordIntTest
    {
        [TestMethod]
        public void ITC10Load()
        {
            var word = new WordInt();
            Util.ParserLoadWord(word, "  1235  ", "1235", "int", 2, 6, 8);
            //             123456789

            Util.ParserLoadWord(word, "  -123  ", "-123", "int", 2, 6, 8);
            //             123456789

            Util.ParserLoadWord(word, "  2     ", "2", "int", 2, 3, 8);
            //             123456789
        }

        [TestMethod]
        public void ITC10LoadError()
        {
            var word = new WordInt();

            //                 "1234567890123456
            Util.WordLoadError("  a234567  ", word, "Int no ciffer",
                "itc10: Syntax error (testRule). Expecting int, found a. Line 1, colomn 3");

            Util.WordLoadError("  -a23456  ", word, "Int no ciffer",
                "itc10: Syntax error (testRule). Expecting int, found a. Line 1, colomn 4");

            //                 "1234567890123456
            Util.WordLoadError("  12345678901  ", word, "Int too long",
                "pe11: Can't parse 'testRule' to integer (too long). Line 1, colomn 12");

        }
    }
}
