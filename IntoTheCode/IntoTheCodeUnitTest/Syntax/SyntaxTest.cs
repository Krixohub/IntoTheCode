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
        public void Parser11SyntaxHardcode()
        {
            //Parser parser = new Parser();
            var syntax = new Parser();

            // test loading of basic syntax elements
            // load varname
            LoadProces reading = new LoadProces(new FlatBuffer("  sym01  "));
//            reader.TextBuffer = new FlatBuffer("  sym01  ");
            //var buf = new FlatBuffer("  sym01  ");
            var outNo = new List<TreeNode>();
            var idn = new WordName("kurt");
            Assert.AreEqual(true, idn.Load(reading, outNo), "Identifier: Can't read");
            var node = outNo[0] as CodeElement;
            Assert.IsNotNull(node, "Identifier: Can't find node after reading");
            Assert.AreEqual("sym01", node.Value, "Identifier: The value is not correct");
            Assert.AreEqual("kurt", node.Name, "Identifier: The wordname is not correct");
            Assert.AreEqual(2, ((FlatSubString)node.ValuePointer).From, "Identifier: The start is not correct");
            Assert.AreEqual(7, ((FlatSubString)node.ValuePointer).To, "Identifier: The end is not correct");
            Assert.AreEqual(7, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "Identifier: The buffer pointer is of after reading");

            // load quote
            reading.TextBuffer = new FlatBuffer(" 'Abcde' ");
            var str = new WordString();
            Assert.AreEqual(true, str.Load(reading, outNo), "String: Can't read");
            node = outNo[1] as CodeElement;
            Assert.IsNotNull(node, "String: Can't find node after reading");
            Assert.AreEqual("Abcde", node.Value, "String: The value is not correct");
            Assert.AreEqual("string", node.Name, "String: The wordname is not correct");
            Assert.AreEqual(2, ((FlatSubString)node.ValuePointer).From, "String: The start is not correct");
            Assert.AreEqual(7, ((FlatSubString)node.ValuePointer).To, "String: The end is not correct");
            Assert.AreEqual(8, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "String: The buffer pointer is of after reading");


            // load string
            reading.TextBuffer = new FlatBuffer("  symbol1  ");
            var sym = new Quote("symbol1");
            Assert.AreEqual(true, sym.Load(reading, outNo), "Symbol: Can't read");
            Assert.AreEqual(2, outNo.Count, "Symbol: Load should not add any nodes");
            Assert.AreEqual(9, ((FlatPointer)reading.TextBuffer.PointerNextChar).index, "Symbol: The buffer pointer is of after");

            // load string + string + wordname
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



            // Build a syntax to test basic syntactic elements
           // parser = new SyntaxReader();
           // var reader = new SyntaxReader();
            string doc = string.Empty;
            syntax.Rules = GetSyntaxTestElements(syntax);
            //parser.Syntax = reader;
            //parser.LinkSyntax(parser.Syntax);
            //reader.LinkSyntax();
            //reader.LinkSyntax(reader);
            outNo = new List<TreeNode>();
            Rule eq = null;
            //  Read a varname
            eq = syntax.Rules.FirstOrDefault(e => e.Name == "TestIdentifier");
            reading.TextBuffer = new FlatBuffer("  Bname  ");
            //parser.TextBuffer = new FlatBuffer("  Bname  ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation TestIdentifier: cant read");
            doc = ((CodeElement)outNo[0]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestIdentifier>Bname</TestIdentifier>\r\n", doc, "Equation TestOption: document fail");

            // Read a 'or syntax = varname'
            eq = syntax.Rules.FirstOrDefault(e => e.Name == "syntax");
            reading.TextBuffer = new FlatBuffer("  Bcccc  ");
            //parser.TextBuffer = new FlatBuffer("  Bcccc  ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation syntax varname: cant read");
            doc = ((CodeElement)outNo[1]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<syntax>\r\n  <TestIdentifier>Bcccc</TestIdentifier>\r\n</syntax>\r\n", doc, "Equation TestOption: document fail");

            // Read a 'or TestString'
            eq = syntax.Rules.FirstOrDefault(e => e.Name == "syntax");
            reading.TextBuffer = new FlatBuffer(" 'Ccccc'  ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation syntax TestString: cant read");
            doc = ((CodeElement)outNo[2]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<syntax>\r\n  <string>Ccccc</string>\r\n</syntax>\r\n", doc, "Equation TestOption: document fail");

            // Read a TestSeries
            eq = syntax.Rules.FirstOrDefault(e => e.Name == "TestSeries");
            reading.TextBuffer = new FlatBuffer("  TestSeries jan ole Mat  ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation TestSeries: cant read");

            doc = ((CodeElement)outNo[3]).ToMarkupProtected(string.Empty);
            Assert.AreEqual(@"<TestSeries>
  <finn>jan</finn>
  <finn>ole</finn>
  <finn>Mat</finn>
</TestSeries>
", doc, "Equation TestOption: document fail");

            // Read a TestOption
            eq = syntax.Rules.FirstOrDefault(e => e.Name == "TestOption");
            reading.TextBuffer = new FlatBuffer("  TestOption 'qwerty'  ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[4]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestQuote2>qwerty</TestQuote2>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");
            reading.TextBuffer = new FlatBuffer("  TestOption wer  ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[5]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestIdentifier>wer</TestIdentifier>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");

            // Read: TestLines       = 'TestLines' { VarName '=' Quote ';' };
            eq = syntax.Rules.FirstOrDefault(e => e.Name == "TestLines");
            reading.TextBuffer = new FlatBuffer("  TestLines name = 'Oscar'; addr = 'GoRoad'; \r\n mobile = '555 55'; ");
            Assert.AreEqual(true, eq.Load(reading, outNo), "Equation TestLines: cant read");
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
            Assert.AreEqual(string.Empty, reading.LoadError, "ReadingError");

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
        public void Parser14SyntaxError()
        {
            // Set syntax
            string syntax = @"
stx = nam | seq | str;
seq = {o};
o = 'o';
str = 'str' | fail;
fail = 'fail';
nam = fnam lnam;
fnam = 'name';
lnam = wordname;";
            var parser = new Parser(syntax);
            LoadProces proces;
            CodeDocument doc;

            //// What the parser CAN read
            //proces = new LoadProces(new FlatBuffer("ooo"));
            //doc = CodeDocument.Load(parser, proces);
            //Assert.IsNotNull(doc, "doc er null");
            //Assert.AreEqual(string.Empty, proces.LoadError, "Parse error");

            //// Error: End of text not reached.
            //proces = new LoadProces(new FlatBuffer("oop"));
            //doc = CodeDocument.Load(parser, proces);
            //Assert.IsNull(doc, "doc er ikke null");
            //Assert.AreEqual("End of input not reached. line 1, colomn 2", proces.LoadError, "EOF error");


            // Read word-name, EOF error.
            
            // Read word-name, not allowed first letter.
            // todo fejl: der bruges ikke 'div' ; sæt 'div' når der er læst en quote
            proces = new LoadProces(new FlatBuffer("name 2r"));
            doc = CodeDocument.Load(parser, proces);
            Assert.IsNull(doc, "doc er ikke null");
            Assert.AreEqual("syntax error. First charactor is not allowed. line 1, colomn 5", proces.LoadError, "First letter error");

            // Read word-quote, EOF error.

            // Read word-quote, starting ' not found.

            // Read word-quote, ending ' not found.

        }

        /// <summary>A syntax for test. Hard coded.</summary>
        private List<Rule> GetSyntaxTestElements(Parser syntax)
        {
            //bool equal = false;
            //bool tag = true;
            List<Symbol> symbolsToResolve = new List<Symbol>();
            List<Rule> list = new List<Rule>();
            list.Add(new Rule("syntax",
                    new Or( new Symbol("TestIdentifier"),
                    new Or( new Symbol("TestString"),
                    new Symbol("TestSymbol")))) /*{ Tag = true }*/);
            // TestIdentifier     = varName;
            list.Add(new Rule("TestIdentifier",
                new WordName("VarName")) /*{ Tag = true }*/);
            // TestSymbol       = 'Abcde';
            list.Add(new Rule("TestSymbol",
                new Quote("Abcde"))
            { Collapse = true });

            // TestString       = Quote;
            list.Add(new Rule("TestString",
                new WordString())
            { Collapse = true });

            // TestSeries       = 'TestSeries' { VarName };
            list.Add(new Rule("TestSeries",
                new Quote("TestSeries"),
                new Sequence( new WordName("finn")))
            /*{ Tag = true }*/);
            // TestOption       = 'TestOption' [TestIdentifier] [TestString];
            list.Add(new Rule("TestOption",
                new Quote("TestOption"),
                new Optional( new Symbol("TestIdentifier")),
                new Optional( new Symbol("TestQuote2")))
            /*{ Tag = true }*/);
            // TestQuote2      = Quote;
            list.Add(new Rule("TestQuote2",
                new WordString())
            /*{ Tag = true }*/);
            // TestLines       = 'TestLines' { VarName '=' Quote ';' };
            list.Add(new Rule("TestLines",
                new Quote("TestLines"),
                new Sequence( new WordName("localVar"), new Quote("="), new WordString(), new Quote(";")))
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
Rule        = symbol '=' expression ';';
expression  = element {[or] element};
element     = symbol | quote | block;
block       = sequence | optional | parentheses;
sequence    = '{' expression '}';
optional    = '[' expression ']';
parentheses = '(' expression ')';
or          = '|';
symbol      = wordname;
quote       = wordstring;
settings    = 'settings' {setter};
setter      = wordname assignment {',' assignment} ';';
assignment  = property ['=' value];
property    = wordname;
value       = wordstring;";
            //property    = wordname;
            //property    = 'collapse' | 'milestone' | 'ws' | 'wsdef';
        }

        /// <summary>To test changes to the meta syntax.</summary>
        private static CodeDocument TestMetaSyntaxDoc()
        {
            CodeDocument syntax = new CodeDocument { Name = MetaParser.MetaSyntax_ };

            // syntax   = {rule}
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "MetaSyntax"),
                new HardElement("sequence", string.Empty,
                    new HardElement("symbol", "Rule")),
                new HardElement("optional", string.Empty,
                    new HardElement("symbol", MetaParser.Settings___))));

            // rule = symbol '=' expression ';'
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "Rule"),
                new HardElement("symbol", "symbol"),
                new HardElement("quote", "="),
                new HardElement("symbol", "expression"),
                new HardElement("quote", ";")));

            // expression = element {[or] element};
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "expression"),
                new HardElement("symbol", MetaParser.Element____),
                new HardElement("sequence", string.Empty,
                    new HardElement("optional", string.Empty,
                        new HardElement("symbol", MetaParser.Or_________)),
                    new HardElement("symbol", MetaParser.Element____))));

            // element    = symbol | quote | block; Husk ny block
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", MetaParser.Element____),
                new HardElement("symbol", "symbol"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("symbol", "quote"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("symbol", MetaParser.Block______)));

            // block      = sequence | optional | parentheses);
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", MetaParser.Block______),
                new HardElement("symbol", "sequence"),
                new HardElement(MetaParser.Or_________, string.Empty),
                new HardElement("symbol", "optional"),
                new HardElement("or", string.Empty),
                new HardElement("symbol", "parentheses")));

            // sequence      = '{' expression '}';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "sequence"),
                new HardElement("quote", "{"),
                new HardElement("symbol", "expression"),
                new HardElement("quote", "}")));

            // optional     = '[' expression ']';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "optional"),
                new HardElement("quote", "["),
                new HardElement("symbol", "expression"),
                new HardElement("quote", "]")));

            // parentheses      = '(' expression ')';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "parentheses"),
                new HardElement("quote", "("),
                new HardElement("symbol", "expression"),
                new HardElement("quote", ")")));

            // or         = '|';
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "or"),
                new HardElement("quote", "|")));

            // symbol      > wordname;
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "symbol"),
                new HardElement("symbol", "wordname")));

            // quote     = wordstring;
            syntax.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("symbol", "quote"),
                new HardElement("symbol", "wordstring")));

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
    }
}
