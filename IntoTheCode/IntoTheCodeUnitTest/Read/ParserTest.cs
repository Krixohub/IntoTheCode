
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCode.Message;
using IntoTheCodeUnitTest.Read;

namespace Read
{
    [TestClass]
    public class ParserTest
    {
        /// <summary>Test that the parser can read a grammar and load code. Simple.</summary>
        [TestMethod]
        public void ITC13ParseString()
        {
            string grammar;
            string code;
            string markup;

            grammar = " grammar = {o}; o = 'o';settings o collapse = 'false', comment;";
            code = "ooo// remark \r\n o";
            markup = @"<grammar>
  <o/>
  <o/>
  <o/>
  <!-- remark --!>
  <o/>
</grammar>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);

            grammar = " sntx = [identifier];";
            markup = "<sntx/>";
            // code = "";
            Util.ParserLoad(grammar, "", markup);

            grammar = "sntx = identifier;";
            // code = "";  // p02: Can't read 'sntx' Line 1, colomn 1
            Util.ParserLoad(grammar, "", "grammar ok",
                Util.BuildMsg(1, 1, () => MessageRes.p02, "sntx"));

            // code = null;  // p06: The input is null.
            Util.ParserLoad(grammar, null, "grammar ok",
                Util.BuildMsg(0, 0, () => MessageRes.p06));


        }

        /// <summary>Test different types of grammar error.</summary>
        [TestMethod]
        public void ITC15ParseString_SyntaxError()
        {
            string grammar;
            string code;
            string markup;

            // Set grammar
            grammar = @"
stx =  symRule | seq | strRule | idRule;
seq = {o};
o = 'o';
strRule = fstr failStr;
symRule = fsym failSym;
idRule  = fid failId;
fstr = 'string'; failStr = string;
fid  = 'id';   failId = identifier;
fsym = 'symbol'; failSym = 'fail';
settings fid trust; fstr trust; fsym trust;";

            // What the parser CAN read
            code = "ooo";
            markup = "<stx>\r\n  <seq>\r\n    <o/>\r\n    <o/>\r\n    <o/>\r\n  </seq>\r\n</stx>\r\n";
            Util.ParserLoad(grammar, code, markup);

            // p05: End of input not reached. Line 1, colomn 3
            // pe07: Grammar error (o). Expecting symbol 'o', found 'p' Line 1, colomn 3
            Util.ParserLoad(grammar, "oop", null,
                Util.BuildMsg(1, 3, () => MessageRes.pe07, "o", "o", "p"));

        }

        /// <summary>Test different types of grammar error.</summary>
        [TestMethod]
        public void ITC16ParseString_SyntaxErrorSet()
        {
            string grammar;
            string code;
            string markup = string.Empty;

            grammar = @"
stx =  {cmd};
cmd = type identifier ';'; 
type = 'string'; ";

            // What the parser CAN read
            code = "string s; string str;";
            Util.ParserLoad(grammar, code, markup);

            // "missing seperator, EOF error"
            // pe10: Syntax error (cmd). Expecting symbol ';', found EOF. Line 1, colomn 10
            code = "string s ";
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 10, () => MessageRes.pe10, "cmd", "symbol ';'", "EOF"));

            // "missing seperator", 
            // pe07: Grammar error (cmd). Expecting symbol ';', found 'd' Line 1, colomn 10
            code = "string s dfgsd";
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 10, () => MessageRes.pe07, "cmd", ";", "d"));
        }

    }
}
