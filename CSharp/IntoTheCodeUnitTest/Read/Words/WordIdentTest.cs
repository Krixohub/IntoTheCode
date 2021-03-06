﻿using IntoTheCode.Read;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Words
{
    [TestClass]
    public class WordIdentTest
    {
        [TestMethod]
        public void ITC10Load()
        {
            var word = new WordIdent();
            Util.ParserLoadWord(word, "  sym01  ", "sym01", MetaParser.WordIdent__, 2, 7, 9);
            //            "123456789
        }


        [TestMethod]
        public void ITC10LoadError()
        {
            string testName = "Ident";
            var word = new WordIdent();

            //                 "1234   Read 'identifier', EOF error.
            Util.WordLoadError("   ", word, testName,
                "itc01: Syntax error (testRule). Expecting identifier, found EOF. Line 1, colomn 4");

            //                 "123456  Read 'identifier', not allowed first letter.
            Util.WordLoadError(" 2r  ", word, testName,
                "itc10: Syntax error (testRule). Expecting identifier, found 2. Line 1, colomn 2");
        }
    }
}
