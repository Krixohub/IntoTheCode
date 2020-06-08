using IntoTheCode.Message;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Words
{
    [TestClass]
    public class WordStringTest
    {
        [TestMethod]
        public void ITC10Load()
        {
            // load string
            var word = new WordString();
            Util.ParserLoadWord(word, " 'Abcde' ", "Abcde", "string", 2, 7, 9);
            //            "1234567890  
        }

        [TestMethod]
        public void ITC10LoadError()
        {
            string testName = "String";
            var word = new WordString();

            //"itc03: Syntax error (testRule). Expecting string, found EOF. Line 1, colomn 4");
            //                 "1234  Read 'string', EOF error.
            Util.WordLoadError("   ", word, testName,
                Util.BuildMsg(1, 4, () => MessageRes.itc03, "testRule"));

            //"itc10: Syntax error (testRule). Expecting ', found f. Line 1, colomn 4");
            //                 "12345678  
            Util.WordLoadError("   fail", word, testName,
                Util.BuildMsg(1, 4, () => MessageRes.itc10, "testRule", "'", "f"));

            //"itc05: Syntax error (testRule). Expecting string ending. Line 1, colomn 5");
            //                 "123456789  Read 'string', ending ' not found.
            Util.WordLoadError("   'fail", word, testName,
                Util.BuildMsg(1, 5, () => MessageRes.itc05, "testRule"));


        }
    }
}
