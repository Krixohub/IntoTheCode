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
            LoadProces reading = new LoadProces(new FlatBuffer("  sym01  "));
            //            reader.TextBuffer = new FlatBuffer("  sym01  ");
            //var buf = new FlatBuffer("  sym01  ");
            var outNo = new List<TreeNode>();
            var idn = new WordIdent("kurt");
            Assert.AreEqual(true, idn.Load(reading, outNo), "Identifier: Can't read");
            var node = outNo[0] as CodeElement;
            Assert.IsNotNull(node, "Identifier: Can't find node after reading");
            Assert.AreEqual("sym01", node.Value, "Identifier: The value is not correct");
            Assert.AreEqual("kurt", node.Name, "Identifier: The name is not correct");
            Assert.AreEqual(2, ((FlatSubString)node.ValuePointer).From, "Identifier: The start is not correct");
            Assert.AreEqual(7, ((FlatSubString)node.ValuePointer).To, "Identifier: The end is not correct");
            Assert.AreEqual(7, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "Identifier: The buffer pointer is of after reading");

            // load symbol
            reading.TextBuffer = new FlatBuffer(" 'Abcde' ");
            var str = new WordString();
            Assert.AreEqual(true, str.Load(reading, outNo), "String: Can't read");
            node = outNo[1] as CodeElement;
            Assert.IsNotNull(node, "String: Can't find node after reading");
            Assert.AreEqual("Abcde", node.Value, "String: The value is not correct");
            Assert.AreEqual("string", node.Name, "String: The name is not correct");
            Assert.AreEqual(2, ((FlatSubString)node.ValuePointer).From, "String: The start is not correct");
            Assert.AreEqual(7, ((FlatSubString)node.ValuePointer).To, "String: The end is not correct");
            Assert.AreEqual(8, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "String: The buffer pointer is of after reading");


            // load string
            reading.TextBuffer = new FlatBuffer("  symbol1  ");
            var sym = new WordSymbol("symbol1");
            Assert.AreEqual(true, sym.Load(reading, outNo), "Symbol: Can't read");
            Assert.AreEqual(2, outNo.Count, "Symbol: Load should not add any nodes");
            Assert.AreEqual(9, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "Symbol: The buffer pointer is of after");

            // load string + string + name
            reading.TextBuffer = new FlatBuffer("  Aname     symbol1      'Fghij'      sym02  ");
            Assert.AreEqual(true, idn.Load(reading, outNo), "Can't read a combinded Identifier");
            Assert.AreEqual(true, sym.Load(reading, outNo), "Can't read a combinded Symbol");
            Assert.AreEqual(true, str.Load(reading, outNo), "Can't read a combinded String");
            Assert.AreEqual(true, idn.Load(reading, outNo), "Can't read a combinded Identifier");
            node = outNo[3] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded Quote");
            Assert.AreEqual("Fghij", node.Value, "The combinded Quote value is not correct");
            node = outNo[4] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded VarName");
            Assert.AreEqual("sym02", node.Value, "The combinded VarName value is not correct");
            Assert.AreEqual(43, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "The buffer pointer is of after reading combinded values");

        }

        [TestMethod]
        public void Parser11SyntaxHardcode()
        {
            var parser = new Parser();
            parser.Rules = GetSyntaxTestElements(parser);

            var debug = MetaParser.GetHardCodeParser().GetSyntax();
            var outNo = new List<TreeNode>();
            LoadProces proces = new LoadProces(null);
            string doc = string.Empty;
            outNo = new List<TreeNode>();
            Rule rule;

            //  Read a varname
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestIdentifier");
            proces.TextBuffer = new FlatBuffer("  Bname  ");
            //parser.TextBuffer = new FlatBuffer("  Bname  ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation TestIdentifier: cant read");
            doc = ((CodeElement)outNo[0]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestIdentifier>Bname</TestIdentifier>\r\n", doc, "Equation TestOption: document fail");

            // Read a 'or syntax = varname'
            rule = parser.Rules.FirstOrDefault(e => e.Name == "syntax");
            proces.TextBuffer = new FlatBuffer("  Bcccc  ");
            //parser.TextBuffer = new FlatBuffer("  Bcccc  ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation syntax varname: cant read");
            doc = ((CodeElement)outNo[1]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<syntax>\r\n  <TestIdentifier>Bcccc</TestIdentifier>\r\n</syntax>\r\n", doc, "Equation TestOption: document fail");

            // Read a 'or TestString'
            rule = parser.Rules.FirstOrDefault(e => e.Name == "syntax");
            proces.TextBuffer = new FlatBuffer(" 'Ccccc'  ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation syntax TestString: cant read");
            doc = ((CodeElement)outNo[2]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<syntax>\r\n  <string>Ccccc</string>\r\n</syntax>\r\n", doc, "Equation TestOption: document fail");

            // Read a TestSeries
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestSeries");
            proces.TextBuffer = new FlatBuffer("  TestSeries jan ole Mat  ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation TestSeries: cant read");

            doc = ((CodeElement)outNo[3]).ToMarkupProtected(string.Empty);
            Assert.AreEqual(@"<TestSeries>
  <finn>jan</finn>
  <finn>ole</finn>
  <finn>Mat</finn>
</TestSeries>
", doc, "Equation TestOption: document fail");

            // Read a TestOption
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestOption");
            proces.TextBuffer = new FlatBuffer("  TestOption 'qwerty'  ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[4]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestQuote2>qwerty</TestQuote2>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");
            proces.TextBuffer = new FlatBuffer("  TestOption wer  ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[5]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestIdentifier>wer</TestIdentifier>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");

            // Read: TestLines       = 'TestLines' { VarName '=' Quote ';' };
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestLines");
            proces.TextBuffer = new FlatBuffer("  TestLines name = 'Oscar'; addr = 'GoRoad'; \r\n mobile = '555 55'; ");
            Assert.AreEqual(true, rule.Load(proces, outNo), "Equation TestLines: cant read");
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
            var hardcodeParser = MetaParser.GetHardCodeParser();
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
            LoadProces reading = new LoadProces(new FlatBuffer(MetaParser.SoftMetaSyntaxAndSettings));
            CodeDocument docActual1 = CodeDocument.Load(parser, reading);
            reading = new LoadProces(new FlatBuffer(MetaParser.SoftMetaSyntaxAndSettings));
            CodeDocument docActual2 = CodeDocument.Load(MetaParser.Instance, reading);

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
            LoadProces reading = new LoadProces(new FlatBuffer("ooo"));

            Assert.IsNotNull(parser, "parser er null");
            Assert.AreEqual(string.Empty, parser.DefinitionError, "DifinitionError");
            CodeDocument doc = CodeDocument.Load(parser, reading);
            Assert.IsNotNull(doc, "doc er null");
            Assert.AreEqual(string.Empty, reading.ErrorMsg, "ReadingError");

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
            LoadProces proces;
            CodeDocument doc;

            // What the parser CAN read
            var buf = new FlatBuffer("ooo");
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf));;
            Assert.IsNotNull(doc, "doc er null");
            Assert.AreEqual(string.Empty, proces.ErrorMsg, "Parse error");

            // Error: End of text not reached.
            buf = new FlatBuffer("oop");
            //                   "123   
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf));
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("End of input not reached. Line 1, colomn 3", proces.ErrorMsg, "EOF error");

            string stx = parser.GetSyntax();

            // Read 'identifier', EOF error.
            buf = new FlatBuffer("id ");
            //                   "1234
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf)); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (idRule). Expecting identifier, found EOF. Line 1, colomn 4", proces.ErrorMsg, "Ident EOF error");

            // Read 'identifier', not allowed first letter.
            buf = new FlatBuffer("id 2r");
            //                   "1234
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf)); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (idRule). First charactor is not allowed. Line 1, colomn 4", proces.ErrorMsg, "Ident first letter error");

            // Read 'string', EOF error.
            buf = new FlatBuffer("string ");
            //                   "12345678
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf)); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (strRule). Expecting string, found EOF. Line 1, colomn 8", proces.ErrorMsg, "String EOF error");

            // Read 'string', Expecting string (starting ' not found).
            buf = new FlatBuffer("string fail");
            //                   "12345678
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf)); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (strRule). Expecting string. Line 1, colomn 8", proces.ErrorMsg, "String not found error");

            // Read 'string', ending ' not found.
            buf = new FlatBuffer("string 'fail");
            //                   "123456789
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf)); ;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (strRule). Expecting string ending. Line 1, colomn 9", proces.ErrorMsg, "String ending not found error");

            // Read 'symbol', EOF error.
            buf = new FlatBuffer("symbol fai");
            //                   "12345678901
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf));;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (symRule). Expecting symbol 'fail', found EOF. Line 1, colomn 11", proces.ErrorMsg, "Symbol EOF error");

            // Read 'symbol',  error.
            buf = new FlatBuffer("symbol faiL ");
            //                   "12345678
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf));;
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (symRule). Expecting symbol 'fail', found 'faiL'. Line 1, colomn 8", proces.ErrorMsg, "Symbol error");
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
            LoadProces proces;
            CodeDocument doc;

            // What the parser CAN read
            var buf = new FlatBuffer("string s; string str;");
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf)); ;
            Assert.IsNotNull(doc, "doc er null");
            Assert.AreEqual(string.Empty, proces.ErrorMsg, "Parse error");

            // Error: Missing ';'.
            buf = new FlatBuffer("string s ");
            //                   "1234567890   
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf));
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (cmd). Expecting symbol ';', found EOF. Line 1, colomn 10", proces.ErrorMsg, "missing seperator, EOF error");

            // Error: Missing ';'.
            buf = new FlatBuffer("string s dfgsd");
            //                   "1234567890   
            doc = CodeDocument.Load(parser, proces = new LoadProces(buf));
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("Syntax error (cmd). Expecting symbol ';', found 'd'. Line 1, colomn 10", proces.ErrorMsg, "missing seperator");
        }

        #region utillity functions

        /// <summary>A syntax for test. Hard coded.</summary>
        private List<Rule> GetSyntaxTestElements(Parser syntax)
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
                new Sequence( new WordIdent("finn")))
            /*{ Tag = true }*/);
            // TestOption       = 'TestOption' [TestIdentifier] [TestString];
            list.Add(new Rule("TestOption",
                new WordSymbol("TestOption"),
                new Optional( new RuleLink("TestIdentifier")),
                new Optional( new RuleLink("TestQuote2")))
            /*{ Tag = true }*/);
            // TestQuote2      = Quote;
            list.Add(new Rule("TestQuote2",
                new WordString())
            /*{ Tag = true }*/);
            // TestLines       = 'TestLines' { VarName '=' Quote ';' };
            list.Add(new Rule("TestLines",
                new WordSymbol("TestLines"),
                new Sequence( new WordIdent("localVar"), new WordSymbol("="), new WordString(), new WordSymbol(";")))
            /*{ Tag = true }*/);
            // TestQuote2      = Quote;
            //list.Add(new Equation("TestQuote2",
            //    new Quote()));

            syntax.Rules = list;
            foreach (var eq in list) eq.Parser = syntax;
            ParserFactory.InitializeSyntax(syntax);
            ParserFactory.ValidateSyntax(syntax);
            return list;
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

            // element    = identifier | symbol | block; Husk ny block
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", MetaParser.Element____),
                new HardElement("identifier", "identifier"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("identifier", "symbol"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("identifier", MetaParser.Block______)));

            // block      = sequence | optional | parentheses);
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", MetaParser.Block______),
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

            // block collapse;
            syntax.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "block"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

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
