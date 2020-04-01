
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

            grammar = "grammar = {o};// remark\r\n// remark2\r\n o = 'o';settings o collapse = 'false';";
            code = "ooo// remark \r\n o";
            markup = @"<grammar>
  <o/>
  <o/>
  <o/>
  <!-- remark --!>
  <o/>
</grammar>
";

            Util.ParserLoad("Markup", grammar, code, markup);

            grammar = "sntx = [identifier];";
            markup = "<sntx/>";
            // code = "";
            Util.ParserLoad("Empty", grammar, "", markup);

            grammar = "sntx = identifier;";
            // code = "";  // p02: Can't read 'sntx' Line 1, colomn 1
            Util.ParserLoad("Empty", grammar, "", "",
                Util.BuildMsg(1, 1, () => MessageRes.p02, "sntx"));

            // code = null;  // p06: The input is null.
            Util.ParserLoad("Null", grammar, null, "",
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
            Util.ParserLoad("CAN read", grammar, code, markup);

            // p05: End of input not reached. Line 1, colomn 3
            Util.ParserLoad("no EOF", grammar, "oop", null,
                Util.BuildMsg(1, 3, () => MessageRes.p05));

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
            Util.ParserLoad("CAN read", grammar, code, markup);

            // pe10: Syntax error (cmd). Expecting symbol ';', found EOF. Line 1, colomn 10
            code = "string s ";
            Util.ParserLoad("missing seperator, EOF error", grammar, code, markup,
                Util.BuildMsg(1, 10, () => MessageRes.pe10, "cmd", "symbol ';'", "EOF"));

            // pe07: Grammar error (cmd). Expecting symbol ';', found 'd' Line 1, colomn 10
            code = "string s dfgsd";
            Util.ParserLoad("missing seperator", grammar, code, markup,
                Util.BuildMsg(1, 10, () => MessageRes.pe07, "cmd", ";", "d"));
        }

    }
}
