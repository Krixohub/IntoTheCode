﻿using IntoTheCode.Read.Element.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Element.Words
{
    [TestClass]
    public class WordIdentTest
    {
        [TestMethod]
        public void Load()
        {
            var idn = new WordIdent("kurt");
            Util.WordLoad("  sym01  ", idn, "sym01", "kurt", 2, 7, 9);
            //             123456789
        }


        [TestMethod]
        public void LoadError()
        {
            string testName = "Ident";
            var word = new WordIdent("kurt");

            //                 "1234   Read 'identifier', EOF error.
            Util.WordLoadError("   ", word, testName,
                "pe01: Grammar error (testRule). Expecting identifier, found EOF. Line 1, colomn 4");

            //                 "123456  Read 'identifier', not allowed first letter.
            Util.WordLoadError(" 2r  ", word, testName,
                "pe10: Syntax error (testRule). Expecting identifier, found 2. Line 1, colomn 2");
        }
    }
}