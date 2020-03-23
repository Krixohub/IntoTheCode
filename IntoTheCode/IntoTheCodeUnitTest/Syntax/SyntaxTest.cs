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

namespace TestCodeInternal.UnitTest
{
    [TestClass]
    public class GrammarTest
    {
        [TestMethod]
        public void Parser10Words()
        {
            // test loading of basic grammar elements
            // load varname
            TextBuffer textBuffer = Util.NewBufferWs("  sym01  ");

            //            reader.TextBuffer = new FlatBuffer("  sym01  ");
            //var buf = new FlatBuffer("  sym01  ");
            var outNo = new List<CodeElement>();
            var idn = new WordIdent("kurt") { TextBuffer = textBuffer };
            Assert.AreEqual(true, idn.Load(outNo, 0), "Identifier: Can't read");
            var node = outNo[0] as CodeElement;
            Assert.IsNotNull(node, "Identifier: Can't find node after reading");
            Assert.AreEqual("sym01", node.Value, "Identifier: The value is not correct");
            Assert.AreEqual("kurt", node.Name, "Identifier: The name is not correct");
            Assert.AreEqual(2, ((TextSubString)node.SubString).From, "Identifier: The start is not correct");
            Assert.AreEqual(7, ((TextSubString)node.SubString).To, "Identifier: The end is not correct");
            Assert.AreEqual(9, textBuffer.PointerNextChar, "Identifier: The buffer pointer is of after reading");

            // load symbol
            textBuffer = Util.NewBufferWs(" 'Abcde' ");
            var str = new WordString() { TextBuffer = textBuffer };
            Assert.AreEqual(true, str.Load(outNo, 0), "String: Can't read");
            node = outNo[1] as CodeElement;
            Assert.IsNotNull(node, "String: Can't find node after reading");
            Assert.AreEqual("Abcde", node.Value, "String: The value is not correct");
            Assert.AreEqual("string", node.Name, "String: The name is not correct");
            Assert.AreEqual(2, ((TextSubString)node.SubString).From, "String: The start is not correct");
            Assert.AreEqual(7, ((TextSubString)node.SubString).To, "String: The end is not correct");
            Assert.AreEqual(9, textBuffer.PointerNextChar, "String: The buffer pointer is of after reading");


            // load string
            textBuffer = Util.NewBufferWs("  symbol1  ");
            var sym = new WordSymbol("symbol1") { TextBuffer = textBuffer };
            Assert.AreEqual(true, sym.Load(outNo, 0), "Symbol: Can't read");
            Assert.AreEqual(2, outNo.Count, "Symbol: Load should not add any nodes");
            Assert.AreEqual(11, textBuffer.PointerNextChar, "Symbol: The buffer pointer is of after");

            // load string + string + name
            textBuffer = Util.NewBufferWs("  Aname     symbol1      'Fghij'      sym02  ");
            idn.TextBuffer = textBuffer;
            sym.TextBuffer = textBuffer;
            str.TextBuffer = textBuffer;
            idn.TextBuffer = textBuffer;
            Assert.AreEqual(true, idn.Load(outNo, 0), "Can't read a combinded Identifier");
            Assert.AreEqual(true, sym.Load(outNo, 0), "Can't read a combinded Symbol");
            Assert.AreEqual(true, str.Load(outNo, 0), "Can't read a combinded String");
            Assert.AreEqual(true, idn.Load(outNo, 0), "Can't read a combinded Identifier");
            node = outNo[3] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded Quote");
            Assert.AreEqual("Fghij", node.Value, "The combinded Quote value is not correct");
            node = outNo[4] as CodeElement;
            Assert.IsNotNull(node, "Can't find node after reading combinded VarName");
            Assert.AreEqual("sym02", node.Value, "The combinded VarName value is not correct");
            Assert.AreEqual(45, textBuffer.PointerNextChar, "The buffer pointer is of after reading combinded values");

        }

        [TestMethod]
        public void Parser11GrammarHardcode()
        {
            var parser = new Parser();

            // Status er kun til opsamling af fejl
            ParserStatus status = new ParserStatus(null);
            //hardcodeParser = MetaParser.GetHardCodeParser();
            var debug = MetaParser.GetHardCodeParser(status).GetGrammar();
            var outNo = new List<CodeElement>();
            
            string doc = string.Empty;

            Rule rule;
            TextBuffer textBuffer;

            //  Read a varname
            textBuffer = Util.NewBufferWs("  Bname  ");
            GetGrammarTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestIdentifier");
            //parser.TextBuffer = new FlatBuffer("  Bname  ");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation TestIdentifier: cant read");
            doc = ((CodeElement)outNo[0]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestIdentifier>Bname</TestIdentifier>\r\n", doc, "Equation TestOption: document fail");




            // Read a 'or grammar = varname'
            textBuffer = Util.NewBufferWs("  Bcccc  ");
            GetGrammarTestElements(parser, textBuffer);
            //parser.Rules = GetGrammarTestElements(parser, proces);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "grammar");
            //parser.TextBuffer = new FlatBuffer("  Bcccc  ");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation grammar varname: cant read");
            doc = ((CodeElement)outNo[1]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<grammar>\r\n  <TestIdentifier>Bcccc</TestIdentifier>\r\n</grammar>\r\n", doc, "Equation TestOption: document fail");

            // Read a 'or TestString'
            textBuffer = Util.NewBufferWs(" 'Ccccc'  ");
            GetGrammarTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "grammar");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation grammar TestString: cant read");
            doc = ((CodeElement)outNo[2]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<grammar>\r\n  <string>Ccccc</string>\r\n</grammar>\r\n", doc, "Equation TestOption: document fail");

            // Read a TestSeries
            textBuffer = Util.NewBufferWs("  TestSeries jan ole Mat  ");
            GetGrammarTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestSeries");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation TestSeries: cant read");

            doc = ((CodeElement)outNo[3]).ToMarkupProtected(string.Empty);
            Assert.AreEqual(@"<TestSeries>
  <finn>jan</finn>
  <finn>ole</finn>
  <finn>Mat</finn>
</TestSeries>
", doc, "Equation TestOption: document fail");

            // Read a TestOption
            textBuffer = Util.NewBufferWs("  TestOption 'qwerty'  ");
            GetGrammarTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestOption");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[4]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestQuote2>qwerty</TestQuote2>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");
            textBuffer = Util.NewBufferWs("  TestOption wer  ");
            GetGrammarTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestOption");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation TestOption: cant read");
            doc = ((CodeElement)outNo[5]).ToMarkupProtected(string.Empty);
            Assert.AreEqual("<TestOption>\r\n  <TestIdentifier>wer</TestIdentifier>\r\n</TestOption>\r\n", doc, "Equation TestOption: document fail");

            // Read: TestLines       = 'TestLines' { VarName '=' Quote ';' };
            textBuffer = Util.NewBufferWs("  TestLines name = 'Oscar'; addr = 'GoRoad'; \r\n mobile = '555 55'; ");
            GetGrammarTestElements(parser, textBuffer);
            rule = parser.Rules.FirstOrDefault(e => e.Name == "TestLines");
            Assert.AreEqual(true, rule.Load(outNo, 0), "Equation TestLines: cant read");
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



            // Test hard coded grammar
            //parser2 = new Parser();
            // Status er kun til opsamling af fejl
            Parser hardcodeParser = MetaParser.GetHardCodeParser(status);
            string actual = hardcodeParser.GetGrammar();
            string expect = GetExpectHardCodeGrammar();
            string msg = Util.CompareTextLines(actual, expect);
            Assert.AreEqual(string.Empty, msg, "Hardcode grammar diff error");

        }

        [TestMethod]
        public void Parser12GrammarMeta()
        {
            // Build a grammar to test meta grammar
            string actual1, actual2, expect, msg;
            expect = MetaParser.MetaGrammar;

            var parser = new Parser(MetaParser.SoftMetaGrammarAndSettings);
            actual1 = parser.GetGrammar();              // Test Meta grammar after after compilation in a Parser.
            actual2 = MetaParser.Instance.GetGrammar(); // Test Meta grammar in internal Parser.
            msg = Util.CompareTextLines(actual1, expect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar diff error");
            msg = Util.CompareTextLines(actual2, expect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar internal diff error");

            // Compare generated CodeDocument with 'selected nodes'
            string actualTags1, actualTags2, expectTags;
            CodeDocument docExpect = TestMetaGrammarDoc();

            //var metaParser = _metaparser;
            // todo MetaParser has not the settings.
            TextBuffer buffer = new FlatBuffer(MetaParser.SoftMetaGrammarAndSettings);
            CodeDocument docActual1 = parser.ParseString(buffer);
            buffer = new FlatBuffer(MetaParser.SoftMetaGrammarAndSettings);
            CodeDocument docActual2 = MetaParser.Instance.ParseString(buffer);

            expectTags = docExpect.ToMarkup();
            actualTags1 = docActual1.ToMarkup();
            actualTags2 = docActual2.ToMarkup();
            msg = CodeDocument.CompareCode(docActual1, docExpect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar document diff error");
            msg = CodeDocument.CompareCode(docActual2, docExpect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar internal document diff error");

            Assert.AreEqual(docExpect.Name, docActual1.Name, "Document name diff error");
        }

        /// <summary>Test that the parser can read a grammar and load code. Simple.</summary>
        [TestMethod]
        public void Parser13GrammarInternal()
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
        public void Parser15SyntaxError()
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

            // Read 'identifier', EOF error.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "id "); }
            //                                    "1234   
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("Ident EOF error", 1, 4, errMsg1, () => MessageRes.pe01, "failId");

            // Read 'identifier', not allowed first letter.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "id 2r"); }
            //                                    "1234   
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("Ident first letter error", 1, 4, errMsg1, () => MessageRes.pe02, "failId");

            // Read 'string', EOF error.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "string "); }
            //                                    "12345678  
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("String EOF error", 1, 8, errMsg1, () => MessageRes.pe03, "failStr");






            // Read 'string', Expecting string (starting ' not found).
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "string fail"); }
            //                                    "12345678  
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("String not found error", 1, 8, errMsg1, () => MessageRes.pe04, "failStr");

            // Read 'string', ending ' not found.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "string 'fail"); }
            //                                    "123456789
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("String ending not found error", 1, 9, errMsg1, () => MessageRes.pe05, "failStr");

            // Read 'symbol', EOF error.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "symbol fai"); }
            //                                    "12345678901
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("Symbol EOF error", 1, 11, errMsg1, () => MessageRes.pe06, "failSym", "fail");

            // Read 'symbol',  error.
            errMsg1 = null;
            try { doc = CodeDocument.Load(parser, "symbol faiL"); }
            //                                    "12345678
            catch (ParserException e)
            {
                errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos("Symbol error", 1, 8, errMsg1, () => MessageRes.pe07, "failSym", "fail", "faiL");
        }

        private void AreEqualRes(string errorName, string actual, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string expected = DotNetUtil.Msg(resourceExpression, parm);
            Assert.AreEqual(expected, actual, errorName);
        }

        /// <summary>Test different types of grammar error.</summary>
        [TestMethod]
        public void Parser16SyntaxErrorSet()
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
            Util.ParseErrorResPos("missing seperator, EOF error", 1, 10, errMsg1, () => MessageRes.pe06, "cmd", ";");

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

        /// <summary>Test different types of grammar build error.</summary>
        [TestMethod]
        public void Parser17SyntaxErrorBuild()
        {
            string errMsg1, errMsg2;

            // Can't reproduce pb01 - pb04. Problems in the CodeDocument is catched when parsing (before build)

            // Double rule
            errMsg1 = null;
            errMsg2 = null;
            try { var parser = new Parser("stx = ':'; stx = identifier;"); }
            catch (ParserException e) { errMsg1 = e.AllErrors[0].Message; errMsg2 = e.Message; }
            Util.ParseErrorResPos("Double rule, build error", 1, 1, errMsg1, () => MessageRes.pb05, "stx", "stx");
            Util.ParseErrorResPos("Double rule, build error", 1, 12, errMsg2, () => MessageRes.pb05, "stx", "stx");

            // identifier not found
            errMsg1 = null;
            try { var parser = new Parser("stx = hans ;"); }
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("identifier not found, build error", 1, 7, errMsg1, () => MessageRes.pb06, "hans");

            // settings identifier not found
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; settings hans collapse;"); }
            //                            "123456789012345678901
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("settings identifier not found, build error", 1, 21, errMsg1, () => MessageRes.pb07, "hans");

            // settings prop not found
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; settings stx flip;"); }
            //                            "1234567890123456789012345
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("settings prop not found, build error", 1, 25, errMsg1, () => MessageRes.pb08, "stx", "flip");

            // First rule cant have tag=false
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; \r\n settings stx collapse; "); }
            catch (ParserException e) { errMsg1 = e.Message; }
            Util.ParseErrorResPos("First rule cant have tag=false, build error", 1, 1, errMsg1, () => MessageRes.pb09, "stx");

            // todo find this error
            // grammar error expecting string ( 'false' )
            errMsg1 = null;
            try { var parser = new Parser("stx = ':'; \r\n settings stx collapse = false; "); }
            //                             12345678901234\12345678901234567890123456
            catch (ParserException e) { errMsg1 = e.Message;
                string s = string.Join("\r\n", e.AllErrors.Select(err => err.Message).ToArray());
            }
            Util.ParseErrorResPos(" grammar error", 2, 26, errMsg1, () => MessageRes.pe04, "value");
        }

        #region utillity functions

        /// <summary>A grammar for test. Hard coded.</summary>
        private void GetGrammarTestElements(Parser parser, TextBuffer buffer)
        {
            //bool equal = false;
            //bool tag = true;
            List<RuleLink> symbolsToResolve = new List<RuleLink>();
            List<Rule> list = new List<Rule>();
            list.Add(new Rule("grammar",
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
            ParserFactory.InitializeGrammar(parser, parser.Rules, buffer.Status);
            ParserFactory.ValidateGrammar(parser, buffer.Status);
        }

        /// <summary>A hard coded text grammar.</summary>
        private string GetExpectHardCodeGrammar()
        {
            return
         @"
HardGrammar = {Rule} [settings];
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

        /// <summary>To test changes to the meta grammar.</summary>
        private static CodeDocument TestMetaGrammarDoc()
        {
            CodeDocument grammar = new CodeDocument(new List<CodeElement>(), null) { Name = MetaParser.MetaGrammar_ };

            // grammar   = {rule}
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "MetaGrammar"),
                new HardElement("sequence", string.Empty,
                    new HardElement("identifier", "Rule")),
                new HardElement("optional", string.Empty,
                    new HardElement("identifier", MetaParser.Settings___))));

            // rule = identifier '=' expression ';'
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "Rule"),
                new HardElement("identifier", "identifier"),
                new HardElement("symbol", "="),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", ";")));

            // expression = element {[or] element};
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "expression"),
                new HardElement("identifier", MetaParser.Element____),
                new HardElement("sequence", string.Empty,
                    new HardElement("optional", string.Empty,
                        new HardElement("identifier", MetaParser.Or_________)),
                    new HardElement("identifier", MetaParser.Element____))));

            //// element    = identifier | symbol | block; Husk ny block
            //grammar.AddElement(new HardElement("Rule", string.Empty,
            //    new HardElement("identifier", MetaParser.Element____),
            //    new HardElement("identifier", "identifier"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", "symbol"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", MetaParser.Block______)));

            //// block      = sequence | optional | parentheses);
            //grammar.AddElement(new HardElement("Rule", string.Empty,
            //    new HardElement("identifier", MetaParser.Block______),
            //    new HardElement("identifier", "sequence"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", "optional"),
            //    new HardElement("or", string.Empty),
            //    new HardElement("identifier", "parentheses")));
            // element    = identifier | symbol | block; Husk ny block
            grammar.AddElement(new HardElement("Rule", string.Empty,
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
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "sequence"),
                new HardElement("symbol", "{"),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", "}")));

            // optional     = '[' expression ']';
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "optional"),
                new HardElement("symbol", "["),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", "]")));

            // parentheses      = '(' expression ')';
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "parentheses"),
                new HardElement("symbol", "("),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", ")")));

            // or         = '|';
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "or"),
                new HardElement("symbol", "|")));

            //// ruleId      > identifier;
            //grammar.AddElement(new HardElement("Rule", string.Empty,
            //    new HardElement("identifier", "ruleId"),
            //    new HardElement("identifier", "identifier")));

            // symbol     = string;
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "symbol"),
                new HardElement("identifier", "string")));

            // settings    = 'settings' {setter};
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "settings"),
                new HardElement("symbol", "settings"),
                new HardElement("sequence", string.Empty,
                    new HardElement("identifier", MetaParser.Setter_____))));

            // setter      = identifier assignment {',' assignment} ';';
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "setter"),
                new HardElement("identifier", "identifier"),
                new HardElement("identifier", "assignment"),
                new HardElement("sequence", string.Empty,
                    new HardElement("symbol", ","),
                    new HardElement("identifier", "assignment")),
                new HardElement("symbol", ";")));

            // assignment  = property ['=' value];
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "assignment"),
                new HardElement("identifier", MetaParser.Property___),
                new HardElement("optional", string.Empty,
                    new HardElement("symbol", "="),
                    new HardElement("identifier", MetaParser.Value______))));

            // property    = identifier;
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "property"),
                new HardElement("identifier", "identifier")));

            // value       = string;
            grammar.AddElement(new HardElement("Rule", string.Empty,
                new HardElement("identifier", "value"),
                new HardElement("identifier", "string")));

            // settings
            // expression collapse;
            grammar.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "expression"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            // element    collapse;
            grammar.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "element"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            //// block collapse;
            //grammar.AddElement(new HardElement("setter", string.Empty,
            //    new HardElement("identifier", "block"),
            //    new HardElement("assignment", string.Empty,
            //        new HardElement("property", "collapse"))));

            // settings collapse;
            grammar.AddElement(new HardElement("setter", string.Empty,
                new HardElement("identifier", "settings"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            return grammar;
        }

        #endregion utillity functions
    }
}
