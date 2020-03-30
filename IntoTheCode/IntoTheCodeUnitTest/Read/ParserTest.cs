using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntoTheCode;
using IntoTheCode.Basic;
using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Message;
using System.Linq.Expressions;
using IntoTheCode.Basic.Util;
using IntoTheCodeUnitTest.Read;

namespace Read
{
    [TestClass]
    public class ParserTest
    {
        /// <summary>Test that the parser can read a grammar and load code. Simple.</summary>
        [TestMethod]
        public void Parser13ParseString()
        {
            string stx = "grammar = {o};// remark\r\n// remark2\r\n o = 'o';settings o collapse = 'false';";
            var parser = new Parser(stx);
            TextBuffer buffer = new FlatBuffer("ooo// remark \r\n o");

            Assert.IsNotNull(parser, "parser er null");
            Assert.AreEqual(string.Empty, parser.DefinitionError, "DifinitionError");
            CodeDocument doc = parser.ParseString(buffer);
            Assert.IsNotNull(doc, "doc er null");
            Assert.IsNull(buffer.Status.Error, "ReadingError");

            string markup = @"<grammar>
  <o/>
  <o/>
  <o/>
  <!-- remark --!>
  <o/>
</grammar>
";
            string actual = doc.ToMarkup();

            Assert.AreEqual(markup, actual, "Markup");
        }

        /// <summary>Test different types of grammar error.</summary>
        [TestMethod]
        public void Parser15ParseString_SyntaxError()
        {
            // Set grammar
            string grammar = @"
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
            var parser = new Parser(grammar);
            //ParserStatus proces;
            CodeDocument doc;
            string errMsg1;

            // What the parser CAN read
            TextBuffer buf = new FlatBuffer("ooo");
            doc = parser.ParseString(buf);;
            Assert.IsNotNull(doc, "doc er null");
            Assert.IsNull(buf.Status.Error, "Parse error");

            // Error: End of text not reached.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "oop"); }
            //                                    "123   
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("EOF error", 1, 3, errMsg1, () => MessageRes.p05);

            
            
            
            string stx = parser.GetGrammar();
        
        }

        private void AreEqualRes(string errorName, string actual, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string expected = DotNetUtil.Msg(resourceExpression, parm);
            Assert.AreEqual(expected, actual, errorName);
        }

        /// <summary>Test different types of grammar error.</summary>
        [TestMethod]
        public void Parser16ParseString_SyntaxErrorSet()
        {
            // Set grammar
            string grammar = @"
stx =  {cmd};
cmd = type identifier ';'; 
type = 'string'; ";
            var parser = new Parser(grammar);
            CodeDocument doc;
            string errMsg1;

            // What the parser CAN read
            TextBuffer buf = new FlatBuffer("string s; string str;");
            doc = parser.ParseString(buf); ;
            Assert.IsNotNull(doc, "doc er null");
            Assert.IsNull(buf.Status.Error, "Parse error");

            // identifier not found
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "string s "); }
            //                                        "1234567890   
            catch (ParserException e) {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("missing seperator, EOF error", 1, 10, errMsg1, () => MessageRes.pe10, "cmd", "symbol ';'", "EOF");

            // Error: Missing ';'.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "string s dfgsd"); }
            //                                    "1234567890   
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("missing seperator", 1, 10, errMsg1, () => MessageRes.pe07, "cmd", ";", "d");
        }

    }
}
