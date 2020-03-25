﻿using System;
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
    public class MetaParserTest
    {

        [TestMethod]
        public void HardcodedTestGrammar()
        {
            string expected;

            //  Read a varname
            expected = "<TestIdentifier>Bname</TestIdentifier>\r\n";
            Util.MetaHard("  Bname  ", expected, "TestIdentifier");

            // Read a 'or grammar = varname'
            expected = "<grammar>\r\n  <TestIdentifier>Bcccc</TestIdentifier>\r\n</grammar>\r\n";
            Util.MetaHard("  Bcccc  ", expected, "grammar");

            // Read a 'or TestString'
            expected = "<grammar>\r\n  <string>Ccccc</string>\r\n</grammar>\r\n";
            Util.MetaHard(" 'Ccccc'  ", expected, "grammar");

            // Read a TestSeries
            expected = @"<TestSeries>
  <finn>jan</finn>
  <finn>ole</finn>
  <finn>Mat</finn>
</TestSeries>
";
            Util.MetaHard("  TestSeries jan ole Mat  ", expected, "TestSeries");


            // Read a TestOption
            expected = "<TestOption>\r\n  <TestQuote2>qwerty</TestQuote2>\r\n</TestOption>\r\n";
            Util.MetaHard("  TestOption 'qwerty'  ", expected, "TestOption");


            // Read a TestOption
            expected = "<TestOption>\r\n  <TestIdentifier>wer</TestIdentifier>\r\n</TestOption>\r\n";
            Util.MetaHard("  TestOption wer  ", expected, "TestOption");


            // Read: TestLines       = 'TestLines' { VarName '=' Quote ';' };
            expected = @"<TestLines>
  <localVar>name</localVar>
  <string>Oscar</string>
  <localVar>addr</localVar>
  <string>GoRoad</string>
  <localVar>mobile</localVar>
  <string>555 55</string>
</TestLines>
";
            Util.MetaHard("  TestLines name = 'Oscar'; addr = 'GoRoad'; \r\n mobile = '555 55'; ", expected, "TestLines");
        }

        [TestMethod]
        public void HardCodeParser()
        {
            // Test hard coded grammar
            // Status er kun til opsamling af fejl
            ParserStatus status = new ParserStatus(null);
            Parser hardcodeParser = MetaParser.GetHardCodeParser(status);
            string actual = hardcodeParser.GetGrammar();
            string expect = GetExpectHardCodeGrammar();
            string msg = Util.CompareTextLines(actual, expect);
            Assert.AreEqual(string.Empty, msg, "Hardcode grammar diff error");
        }

        [TestMethod]
        public void MetaSyntax()
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

            Assert.AreEqual(expectTags, actualTags1, "Meta grammar document.toMarkup diff error");
            Assert.AreEqual(expectTags, actualTags2, "Meta grammar document.toMarkup diff error");

        }

        #region utillity functions

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
