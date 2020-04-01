using IntoTheCode.Message;
using IntoTheCode.Read.Element.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Element.Words
{
    [TestClass]
    public class WordStringTest
    {
        [TestMethod]
        public void ITC10Load()
        {
            // load string
            var word = new WordString();
            Util.WordLoad(" 'Abcde' ", word, "Abcde", "string", 2, 7, 9);
            //            "1234567890  
        }

        [TestMethod]
        public void ITC10LoadError()
        {
            string testName = "String";
            var word = new WordString();

            //                 "1234  Read 'string', EOF error.
            Util.WordLoadError("   ", word, testName,
                "pe03: Grammar error (testRule). Expecting string, found EOF. Line 1, colomn 4");

            //                 "12345678  Read 'string', Expecting string (starting ' not found).
            Util.WordLoadError("   fail", word, testName,
                "pe10: Syntax error (testRule). Expecting ', found f. Line 1, colomn 4");

            //                 "123456789  Read 'string', ending ' not found.
            Util.WordLoadError("   'fail", word, testName,
                "pe05: Grammar error (testRule). Expecting string ending. Line 1, colomn 5");


        }
    }
}
