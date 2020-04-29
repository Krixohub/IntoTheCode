using IntoTheCode.Read;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Read.Words
{
    [TestClass]
    public class WordCommentTest
    {
        // todo flyt til read.

        [TestMethod]
        public void ITC11LoadDoc()
        {
            string grammar;
            string code;
            string markup;

            // Set grammar
            grammar = @"stx = { line }; line = a b; a = 'a'; b = 'b';";

            // --------------------------------- expression "just comment" ---------------------------------
            code = " // comment \r\n ";
            markup = @"<stx>
  <!-- comment --!>
</stx>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

            code = " // comment \r\na b";
            markup = @"<stx>
  <!-- comment --!>
  <line>
    <a/>
    <b/>
  </line>
</stx>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

            code = "a // comment\r\n b";
            markup = @"<stx>
  <line>
    <a/>
    <b/>
  </line>
  <!-- comment--!>
</stx>
";
            //"inside comment"
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

            code = "a  b// comment\r\n";
            //"end comment"
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

            code = "a  b// comment";
            //"end comment"
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

            grammar = @"stx = { line }; line = 'a' 'b'; settings line comment;";
            code = "a // comment\r\n b";
            markup = @"<stx>
  <!-- comment--!>
  <line/>
</stx>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);
            
            code = "a b // comment1\r\n  // comment2";
            markup = @"<stx>
  <line/>
  <!-- comment1--!>
  <!-- comment2--!>
</stx>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);


            grammar = @"stx = { line }; line = a b; a = 'a'; b = 'b'; settings a comment;";
            code = "a // comment\r\n b";
            markup = @"<stx>
  <line>
    <a/>
    <!-- comment--!>
    <b/>
  </line>
</stx>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

        }
    }
}
