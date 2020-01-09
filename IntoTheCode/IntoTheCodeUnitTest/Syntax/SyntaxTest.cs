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

namespace TestCodeInternal.UnitTest
{
    [TestClass]
    public class SyntaxTest
    {
        [TestMethod]
        public void Parser10Words()
        {
            // test loading of basic syntax elements
            // load varname
            ITextBuffer textBuffer = new FlatBuffer("  sym01  ");
            //            reader.TextBuffer = new FlatBuffer("  sym01  ");
            //var buf = new FlatBuffer("  sym01  ");
            var outNo = new List<TreeNode>();
            var idn = new WordIdent("kurt") { TextBuffer = textBuffer };
            Assert.AreEqual(true, idn.Load(outNo), "Identifier: Can't read");
            var node = outNo[0] as CodeElement;
            Assert.IsNotNull(node, "Identifier: Can't find node after reading");
            Assert.AreEqual("sym01", node.Value, "Identifier: The value is not correct");
            Assert.AreEqual("kurt", node.Name, "Identifier: The name is not correct");
            Assert.AreEqual(2, ((FlatSubString)node.SubString).From, "Identifier: The start is not correct");
            Assert.AreEqual(7, ((FlatSubString)node.SubString).To, "Identifier: The end is not correct");
            Assert.AreEqual(7, ((FlatPointer)textBuffer.PointerNextChar).index, "Identifier: The buffer pointer is of after reading");

            // load symbol
            textBuffer = new FlatBuffer(" 'Abcde' ");
            var str = new WordString() { TextBuffer = textBuffer };
            Assert.AreEqual(true, str.Load(outNo), "String: Can't read");
            node = outNo[1] as CodeElement;
            Assert.IsNotNull(node, "String: Can't find node after reading");
            Assert.AreEqual("Abcde", node.Value, "String: The value is not correct");
            Assert.AreEqual("string", node.Name, "String: The name is not correct");
            Assert.AreEqual(2, ((FlatSubString)node.SubString).From, "String: The start is not correct");
            Assert.AreEqual(7, ((FlatSubString)node.SubString).To, "String: The end is not correct");
            Assert.AreEqual(8, ((FlatPointer)textBuffer.PointerNextChar).index, "String: The buffer pointer is of after reading");


            // load string
            textBuffer = new FlatBuffer("  symbol1  ");
            var sym = new WordSymbol("symbol1") { TextBuffer = textBuffer };
            Assert.AreEqual(true, sym.Load(outNo), "Symbol: Can't read");
            Assert.AreEqual(2, outNo.Count, "Symbol: Load should not add any nodes");
            Assert.AreEqual(9, ((FlatPointer)textBuffer.PointerNextChar).index, "Symbol: The buffer pointer is of after");

            // load string + string + name
            textBuffer = new FlatBuffer("  Aname     symbol1      'Fghij'      sym02  ");
            idn.TextBuffer = textBuffer;
            sym.TextBuffer = textBuffer;
            str.TextBuffer = textBuffer;
            idn.TextBuffer = textBuffer;
            Assert.AreEqual(true, idn.Load(outNo), "Can't read a combinded Identifier");
            Assert.AreEqual(true, sym.Load(outNo), "Can't read a combinded Symbol");
            Assert.AreEqual(true, str.Load(outNo), "Can't read a combinded String");
            Assert.AreEqual(true, idn.Load(outNo), "Can't read a combinded Identifier");
            node = outNo[3] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded Quote");
            Assert.AreEqual("Fghij", node.Value, "The combinded Quote value is not correct");
            node = outNo[4] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded VarName");
            Assert.AreEqual("sym02", node.Value, "The combinded VarName value is not correct");
            Assert.AreEqual(43, ((FlatPointer)textBuffer.PointerNextChar).index, "The buffer pointer is of after reading combinded values");

        }

        [TestMethod]
        public void Parser11SyntaxHardcode()
        {
            var parser = new Parser();

            // Status er kun til opsamling af fejl
            ParserStatus status = new ParserStatus(null);
            //hardcodeParser = MetaParser.GetHardCodeParser();
            var debug = MetaParser.GetHardCodeParser(status).GetSyntax();
            var outNo = new List<TreeNode>();
            
            string doc = string.Empty;
            outNo = new List<TreeNode>();
            Rule rule;
            ITextBuffer textBuffer;

            //  Read a varname
            textBuffer = new FlatBuffer("  Bname  ");
            GetSyntaxTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestIdentifier");
            //parser.TextBuffer = new FlatBuffer("  Bname  ");
            Assert.AreEqual(true, rule.Load(outNo), "Equation TestIdentifier: cant read");
            doc = ((CodeElement)outNo[0]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestIdentifier>Bname</TestIdentifier>\r\n", doc, "Equation TestOption: document fail");




            // Read a 'or syntax = varname'
            textBuffer = new FlatBuffer("  Bcccc  ");
            GetSyntaxTestElements(parser, textBuffer);
            //parser.Rules = GetSyntaxTestElements(parser, proces);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "syntax");
            //parser.TextBuffer = new FlatBuffer("  Bcccc  ");
            Assert.AreEqual(true, rule.Load(outNo), "Equation syntax varname: cant read");
            doc = ((CodeElement)outNo[1]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<syntax>\r\n  <TestIdentifier>Bcccc</TestIdentifier>\r\n</syntax>\r\n", doc, "Equation TestOption: document fail");

            // Read a 'or TestString'
            textBuffer = new FlatBuffer(" 'Ccccc'  ");
            GetSyntaxTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "syntax");
            Assert.AreEqual(true, rule.Load(outNo), "Equation syntax TestString: cant read");
            doc = ((CodeElement)outNo[2]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<syntax>\r\n  <string>Ccccc</string>\r\n</syntax>\r\n", doc, "Equation TestOption: document fail");

            // Read a TestSeries
            textBuffer = new FlatBuffer("  TestSeries jan ole Mat  ");
            GetSyntaxTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestSeries");
            Assert.AreEqual(true, rule.Load(outNo), "Equation TestSeries: cant read");

            doc = ((CodeElement)outNo[3]).ToMarkupProtected(string.Empty);
            Assert.AreEqual(@"<TestSeries>
  <finn>jan</finn>
  <finn>ole</finn>
  <finn>Mat</finn>
</TestSeries>
", doc, "Equation TestOption: document fail");

            // Read a TestOption
            textBuffer = new FlatBuffer("  TestOption 'qwerty'  ");
            GetSyntaxTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestOption");
            Assert.AreEqual(true, rule.Load(outNo), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[4]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestQuote2>qwerty</TestQuote2>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");
            textBuffer = new FlatBuffer("  TestOption wer  ");
            GetSyntaxTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestOption");
            Assert.AreEqual(true, rule.Load(outNo), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[5]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestIdentifier>wer</TestIdentifier>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");

            // Read: TestLines       = 'TestLines' { VarName '=' Quote ';' };
            textBuffer = new FlatBuffer("  TestLines name = 'Oscar'; addr = 'GoRoad'; \r\n mobile = '555 55'; ");
            GetSyntaxTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestLines");
            Assert.AreEqual(true, rule.Load(outNo), "Equation TestLines: cant read");
            doc = ((CodeElement)outNo[6]).ToMarkupProtected(string.Empty);
            Assert.AreEqual(@"<TestLines>
  <localVar>name</localVar>
  <string>Oscar</string>
  <localVar>addr</localVar>
  <string>GoRoad</string>
  <localVar>mobile</localVar>
  <string>555 55</string>
</TestLines>
", doc, "Equation TestOption: document fail");



            // Test hard coded syntax
            //parser2 = new Parser();
            // Status er kun til opsamling af fejl
            var hardcodeParser = MetaParser.GetHardCodeParser(status);
            string actual = hardcodeParser.GetSyntax();
            string expect = GetExpectHardCodeSyntax();
            string msg = CompareTextLines(actual, expect);
            Assert.AreEqual(string.Empty, msg, "Hardcode syntax diff error");

        }

        [TestMethod]
        public void Parser12SyntaxMeta()
        {
            // Build a syntax to test meta syntax
            string actual1, actual2, expect, msg;
            expect = MetaParser.MetaSyntax;

            var parser = new Parser(MetaParser.SoftMetaSyntaxAndSettings);
            actual1 = parser.GetSyntax();              // Test Meta syntax after after compilation in a Parser.
            actual2 = MetaParser.Instance.GetSyntax(); // Test Meta syntax in internal Parser.
            msg = CompareTextLines(actual1, expect);
            Assert.AreEqual(string.Empty, msg, "Meta syntax diff error");
            msg = CompareTextLines(actual2, expect);
            Assert.AreEqual(string.Empty, msg, "Meta syntax internal diff error");

            // Compare generated CodeDocument with 'selected nodes'
            string actualTags1, actualTags2, expectTags;
            CodeDocument docExpect = TestMetaSyntaxDoc();

            //var metaParser = _metaparser;
            // todo MetaParser has not the settings.
            ITextBuffer buffer = new FlatBuffer(MetaParser.SoftMetaSyntaxAndSettings);
            CodeDocument docActual1 = parser.ParseString(buffer);
            buffer = new FlatBuffer(MetaParser.SoftMetaSyntaxAndSettings);
            CodeDocument docActual2 = MetaParser.Instance.ParseString(buffer);

            expectTags = docExpect.ToMarkup();
            actualTags1 = docActual1.ToMarkup();
            actualTags2 = docActual2.ToMarkup();
            msg = CodeDocument.CompareCode(docActual1, docExpect);
            Assert.AreEqual(string.Empty, msg, "Meta syntax document diff error");
            msg = CodeDocument.CompareCode(docActual2, docExpect);
            Assert.AreEqual(string.Empty, msg, "Meta syntax internal document diff error");

            // TODO SKAL RETTES
            Assert.AreEqual(docExpect.Name, docActual1.Name, "Document name diff error");
        }

        /// <summary>Test that the parser can read a syntax and load code. Simple.</summary>
        [TestMethod]
        public void Parser13SyntaxInternal()
        {
            string stx = "syntax = {o};\r\no = 'o';settings o collapse = 'false';";
            var parser = new Parser(stx);
            ITextBuffer buffer = new FlatBuffer("ooo");

            Assert.IsNotNull(parser, "parser er null");
            Assert.AreEqual(string.Empty, parser.DefinitionError, "DifinitionError");
            CodeDocument doc = parser.ParseString(buffer);
            Assert.IsNotNull(doc, "doc er null");
            Assert.AreEqual(string.Empty, buffer.Status.ErrorMsg, "ReadingError");

            string markup = @"<syntax>
  <o/>
  <o/>
  <o/>
</syntax>
";

            Assert.AreEqual(markup, doc.ToMarkup(), "Markup");
        }

        /// <summary>Test different types of syntax error.</summary>
        [TestMethod]
        public void Parser15SyntaxError()
        {
            // Set syntax
            string syntax = @"
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
            var parser = new Parser(syntax);
            //ParserStatus proces;
            CodeDocument doc;

            // What the parser CAN read
            ITextBuffer buf = new FlatBuffer("ooo");
            doc = parser.ParseString(buf);;
            Assert.IsNotNull(doc, "doc er null");
            Assert.AreEqual(string.Empty, buf.Status.ErrorMsg, "Parse error");

            // Error: End of text not reached.
            buf = new FlatBuffer("oop");
            //                   "123   
            doc = parser.ParseString(buf);
            Assert.IsNull(doc, "doc er ikke null");
            AreEqualResPos("EOF error", 1, 3, buf.Status.ErrorMsg, () => MessageRes.p05);
//            Assert.AreEqual("End of input not reached. Line 1, colomn 3", buf.Status.ErrorMsg, "EOF error");

            string stx = parser.GetSyntax();

            // Read 'identifier', EOF error.
            buf = new FlatBuffer("id ");
            //                   "1234
            doc = parser.ParseString(buf); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failId). Expecting identifier, found EOF. Line 1, colomn 4", buf.Status.ErrorMsg, "Ident EOF error");

            // Read 'identifier', not allowed first letter.
            buf = new FlatBuffer("id 2r");
            //                   "1234
            doc = parser.ParseString(buf); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failId). First charactor is not allowed. Line 1, colomn 4", buf.Status.ErrorMsg, "Ident first letter error");

            // Read 'string', EOF error.
            buf = new FlatBuffer("string ");
            //                   "12345678
            doc = parser.ParseString(buf); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failStr). Expecting string, found EOF. Line 1, colomn 8", buf.Status.ErrorMsg, "String EOF error");

            // Read 'string', Expecting string (starting ' not found).
            buf = new FlatBuffer("string fail");
            //                   "12345678
            doc = parser.ParseString(buf); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failStr). Expecting string. Line 1, colomn 8", buf.Status.ErrorMsg, "String not found error");

            // Read 'string', ending ' not found.
            buf = new FlatBuffer("string 'fail");
            //                   "123456789
            doc = parser.ParseString(buf); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failStr). Expecting string ending. Line 1, colomn 9", buf.Status.ErrorMsg, "String ending not found error");

            // Read 'symbol', EOF error.
            buf = new FlatBuffer("symbol fai");
            //                   "12345678901
            doc = parser.ParseString(buf);;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failSym). Expecting symbol 'fail', found EOF. Line 1, colomn 11", buf.Status.ErrorMsg, "Symbol EOF error");

            // Read 'symbol',  error.
            buf = new FlatBuffer("symbol faiL ");
            //                   "12345678
            doc = parser.ParseString(buf);;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (failSym). Expecting symbol 'fail', found 'faiL'. Line 1, colomn 8", buf.Status.ErrorMsg, "Symbol error");
        }

        private void AreEqualResPos(string errorName, int line, int col, string actual, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string expected = DotNetUtil.Res(resourceExpression, parm) + " " + string.Format(MessageRes.LineAndCol, line, col);
            Assert.AreEqual(expected, actual, errorName);
        }
        private void AreEqualRes(string errorName, string actual, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string expected = DotNetUtil.Res(resourceExpression, parm);
            Assert.AreEqual(expected, actual, errorName);
        }

        /// <summary>Test different types of syntax error.</summary>
        [TestMethod]
        public void Parser16SyntaxErrorSet()
        {
            // Set syntax
            string syntax = @"
stx =  {cmd};
cmd = type identifier ';'; 
type = 'string'; ";
            var parser = new Parser(syntax);
            CodeDocument doc;

            // What the parser CAN read
            ITextBuffer buf = new FlatBuffer("string s; string str;");
            doc = parser.ParseString(buf); ;
            Assert.IsNotNull(doc, "doc er null");
            Assert.AreEqual(string.Empty, buf.Status.ErrorMsg, "Parse error");

            // Error: Missing ';'.
            buf = new FlatBuffer("string s ");
            //                   "1234567890   
            doc = parser.ParseString(buf);
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (cmd). Expecting symbol ';', found EOF. Line 1, colomn 10", buf.Status.ErrorMsg, "missing seperator, EOF error");

            // Error: Missing ';'.
            buf = new FlatBuffer("string s dfgsd");
            //                   "1234567890   
            doc = parser.ParseString(buf);
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (cmd). Expecting symbol ';', found 'd'. Line 1, colomn 10", buf.Status.ErrorMsg, "missing seperator");
        }

        #region utillity functions

        /// <summary>A syntax for test. Hard coded.</summary>
        private void GetSyntaxTestElements(Parser parser, ITextBuffer buffer)
        {
            //bool equal = false;
            //bool tag = true;
            List<RuleLink> symbolsToResolve = new List<RuleLink>();
            List<Rule> list = new List<Rule>();
            list.Add(new Rule("syntax",
                    new Or( new RuleLink("TestIdentifier"),
                    new Or( new RuleLink("TestString"),
                    new RuleLink("TestSymbol")))) /*{ Tag = true }*/);
            // TestIdentifier     = varName;
            list.Add(new Rule("TestIdentifier",
                new WordIdent("VarName")) /*{ Tag = true }*/);
            // TestSymbol       = 'Abcde';
            list.Add(new Rule("TestSymbol",
                new WordSymbol("Abcde"))
            { Collapse = true });

            // TestString       = Quote;
            list.Add(new Rule("TestString",
                new WordString())
            { Collapse = true });

            // TestSeries       = 'TestSeries' { VarName };
            list.Add(new Rule("TestSeries",
                new WordSymbol("TestSeries"),
                new Sequence( new WordIdent("finn"))));
            // TestOption       = 'TestOption' [TestIdentifier] [TestString];
            list.Add(new Rule("TestOption",
                new WordSymbol("TestOption"),
                new Optional( new RuleLink("TestIdentifier")),
                new Optional( new RuleLink("TestQuote2")))
            /*{ Tag = true }*/);
            // TestQuote2      = Quote;
            list.Add(new Rule("TestQuote2",
                new WordString()));
            // TestLines       = 'TestLines' { VarName '=' Quote ';' };
            list.Add(new Rule("TestLines",
                new WordSymbol("TestLines"),
                new Sequence(new WordIdent("localVar"), new WordSymbol("="), new WordString(), new WordSymbol(";"))));

            
            parser.Rules = list.Select(r => r.CloneForParse(buffer) as Rule).ToList();
            ParserFactory.InitializeSyntax(parser, parser.Rules, buffer.Status);
            ParserFactory.ValidateSyntax(parser, buffer.Status);
        }

        /// <summary>A hard coded text syntax.</summary>
        private string GetExpectHardCodeSyntax()
        {
            return
         @"
HardSyntax  = {Rule} [settings];
Rule        = identifier '=' expression ';';
expression  = element {[or] element};
element     = identifier | symbol | block;
block       = sequence | optional | parentheses;
sequence    = '{' expression '}';
optional    = '[' expression ']';
parentheses = '(' expression ')';
or          = '|';
symbol      = string;
settings    = 'settings' {setter};
setter      = identifier assignment {',' assignment} ';';
assignment  = property ['=' value];
property    = identifier;
value       = string;";
            //property    = identifier;
            //property    = 'collapse' | 'milestone' | 'ws' | 'wsdef';
        }

        /// <summary>To test changes to the meta syntax.</summary>
        private static CodeDocument TestMetaSyntaxDoc()
        {
            CodeDocument syntax = new CodeDocument { Name = MetaParser.MetaSyntax_ };

            // syntax   = {rule}
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "MetaSyntax"),
                new HardElement("sequence", string.Empty,
                    new HardElement("identifier", "Rule")),
                new HardElement("optional", string.Empty,
                    new HardElement("identifier", MetaParser.Settings___))));

            // rule = identifier '=' expression ';'
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "Rule"),
                new HardElement("identifier", "identifier"),
                new HardElement("symbol", "="),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", ";")));

            // expression = element {[or] element};
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "expression"),
                new HardElement("identifier", MetaParser.Element____),
                new HardElement("sequence", string.Empty,
                    new HardElement("optional", string.Empty,
                        new HardElement("identifier", MetaParser.Or_________)),
                    new HardElement("identifier", MetaParser.Element____))));

            //// element    = identifier | symbol | block; Husk ny block
            //syntax.AddElement(new HardElement("Rule", string.Empty,
            //    new HardElement("identifier", MetaParser.Element____),
            //    new HardElement("identifier", "identifier"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", "symbol"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", MetaParser.Block______)));

            //// block      = sequence | optional | parentheses);
            //syntax.AddElement(new HardElement("Rule", string.Empty,
            //    new HardElement("identifier", MetaParser.Block______),
            //    new HardElement("identifier", "sequence"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", "optional"),
            //    new HardElement("or", string.Empty),
            //    new HardElement("identifier", "parentheses")));
            // element    = identifier | symbol | block; Husk ny block
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", MetaParser.Element____),
                new HardElement("identifier", "identifier"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("identifier", "symbol"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("identifier", "sequence"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("identifier", "optional"),
                new HardElement("or", string.Empty),
                new HardElement("identifier", "parentheses")));

            // sequence      = '{' expression '}';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "sequence"),
                new HardElement("symbol", "{"),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", "}")));

            // optional     = '[' expression ']';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "optional"),
                new HardElement("symbol", "["),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", "]")));

            // parentheses      = '(' expression ')';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "parentheses"),
                new HardElement("symbol", "("),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", ")")));

            // or         = '|';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "or"),
                new HardElement("symbol", "|")));

            //// ruleId      > identifier;
            //syntax.AddElement(new HardElement("Rule", string.Empty,
            //    new HardElement("identifier", "ruleId"),
            //    new HardElement("identifier", "identifier")));

            // symbol     = string;
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "symbol"),
                new HardElement("identifier", "string")));

            // settings    = 'settings' {setter};
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "settings"),
                new HardElement("symbol", "settings"),
                new HardElement("sequence", string.Empty,
                    new HardElement("identifier", MetaParser.Setter_____))));

            // setter      = identifier assignment {',' assignment} ';';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "setter"),
                new HardElement("identifier", "identifier"),
                new HardElement("identifier", "assignment"),
                new HardElement("sequence", string.Empty,
                    new HardElement("symbol", ","),
                    new HardElement("identifier", "assignment")),
                new HardElement("symbol", ";")));

            // assignment  = property ['=' value];
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "assignment"),
                new HardElement("identifier", MetaParser.Property___),
                new HardElement("optional", string.Empty,
                    new HardElement("symbol", "="),
                    new HardElement("identifier", MetaParser.Value______))));

            // property    = identifier;
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "property"),
                new HardElement("identifier", "identifier")));

            // value       = string;
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "value"),
                new HardElement("identifier", "string")));

            // settings
            // expression collapse;
            syntax.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "expression"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            // element    collapse;
            syntax.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "element"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            //// block collapse;
            //syntax.AddElement(new HardElement("setter", string.Empty,
            //    new HardElement("identifier", "block"),
            //    new HardElement("assignment", string.Empty,
            //        new HardElement("property", "collapse"))));

            // settings collapse;
            syntax.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "settings"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            return syntax;
        }

        /// <summary>Compare two strings. Line for line and char for char.</summary>
        /// <returns>Empty string if equal. Message with line and char number if different.</returns>
        private string CompareTextLines(string actual, string expect)
        {
            string[] sep = { "\r\n" };
            string[] actualList = actual.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string[] expectList = expect.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < actualList.Length; i++)
            {
                if (i == expectList.Length)
                    return string.Format("Actual text has {0} lines, expected text only {1} lines", actualList.Length, expectList.Length);
                for (int j = 0; j < actualList[i].Length; j++)
                {
                    if (expectList[i].Length <= j) return string.Format("Actual line {0} is shorter than expected", i + 1);
                    if (actualList[i][j] != expectList[i][j])
                        return string.Format("Difference in line {0}, at positien {1}: actual '{2}', expect '{3}'", i + 1, j + 1, actualList[i][j], expectList[i][j]);
                }
                if (expectList[i].Length > actualList[i].Length) return string.Format("Actual line {0} is longer than expected", i + 1);
            }
            if (actualList.Length < expectList.Length)
                return string.Format("Actual text has only {0} lines, expected text {1} lines", actualList.Length, expectList.Length);
            return string.Empty;
        }

        #endregion utillity functions
    }
}
