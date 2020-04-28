using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Read.Words
{
    [TestClass]
    public class WordCommentTest
    {
        // todo flyt til read.

        [TestMethod]
        public void ITC11Load()
        {
            //// load string
            //var word = new WordComment();
            //Util.WordLoad(" // cmmnt ", word, " cmmnt ", "comment", 3, 10, 10);
            ////            "12345678901  

            //Util.WordLoad(" // cmmnt \r\n  ", word, " cmmnt ", "comment", 3, 10, 12);
            ////            "12345678901 2 345  
        }

        [TestMethod]
        public void ITC11LoadDoc()
        {
            string grammar;
            string code;
            string markup;

            // Set grammar
            grammar = @"stx = { line }; line = a b; a = 'a'; b = 'b';";

            // --------------------------------- expression comment ---------------------------------
            code = " // comment \r\n ";
            markup = @"<stx>
  <!-- comment --!>
</stx>
";
            markup = Util.ParserLoad("just comment", grammar, code, markup);

            code = "a // comment \r\n b";
            markup = @"<stx>
  <line>
    <a/>
    <!-- comment --!>
    <b/>
  </line>
</stx>
"; 
            markup = Util.ParserLoad("inside comment", grammar, code, markup);

        }
    }
}
