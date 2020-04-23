using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IntoTheCode;
using IntoTheCode.Buffer;
using IntoTheCode.Read;
using IntoTheCodeUnitTest.Read;

namespace Read
{
    /// <summary>
    /// Some tests are name 'ITC##______________'
    /// The number is an indication of how narrow/independent the test is. Low numbered tests should be fixed first.
    /// Higthernumbered tests are depending on other tests.
    /// Number
    ///  00-09 buffer
    ///  10-19 words
    ///  20-29 MetaParser Hardcoded-parser parser-factory
    ///  30-   
    /// </summary>
    [TestClass]
    public class MetaParserTest
    {
        [TestMethod]
        public void ITC23HardCodeParser()
        {
            // Test hard coded grammar
            // Status is for collection of errors
            ParserStatus status = new ParserStatus(null);
            Parser hardcodeParser = MetaParser.GetHardCodeParser(status);
            string actual = hardcodeParser.GetGrammar();
            string expect = GetExpectHardCodeGrammar();
            string msg = Util.CompareTextLines(actual, expect);
            Assert.AreEqual(string.Empty, msg, "Hardcode grammar diff error");
        }

        [TestMethod]
        public void ITC24MetaSyntax()
        {
            // Build a grammar to test meta grammar
            string actual1, actual2, expect, msg;
            expect = MetaParser.SoftMetaGrammarAndSettings;

            var parser = new Parser(MetaParser.SoftMetaGrammarAndSettings);
            actual1 = parser.GetGrammar();              // Test Meta grammar after after compilation in a Parser.
            actual2 = MetaParser.Instance.GetGrammar(); // Test Meta grammar in internal Parser.
            msg = Util.CompareTextLines(actual1, expect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar diff error");
            msg = Util.CompareTextLines(actual2, expect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar internal diff error");

            // Compare generated CodeDocument with 'selected nodes'
            string actualTags1, actualTags2, expectTags;
            TextDocument docExpect = TestMetaGrammarDoc();

            //var metaParser = _metaparser;
            // todo MetaParser has not the settings.
            TextBuffer buffer = new FlatBuffer(MetaParser.SoftMetaGrammarAndSettings);
            TextDocument docActual1 = parser.ParseString(buffer);
            buffer = new FlatBuffer(MetaParser.SoftMetaGrammarAndSettings);
            TextDocument docActual2 = MetaParser.Instance.ParseString(buffer);

            expectTags = docExpect.ToMarkup();
            actualTags1 = docActual1.ToMarkup();
            actualTags2 = docActual2.ToMarkup();

            msg = TextDocument.CompareCode(docActual1, docExpect);
            Assert.AreEqual(string.Empty, msg, "Meta grammar document diff error");
            msg = TextDocument.CompareCode(docActual2, docExpect);
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
HardGrammar = {rule} [settings];
rule        = identifier '=' expression ';';
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
value       = string;
settings
expression  collapse;
element     collapse;
block       collapse;
settings    collapse;";
            //property    = identifier;
            //property    = 'collapse' | 'milestone' | 'ws' | 'wsdef';
        }

        /// <summary>To test changes to the meta grammar.</summary>
        private static TextDocument TestMetaGrammarDoc()
        {
            TextDocument grammar = new TextDocument(new List<CodeElement>(), MetaParser.MetaGrammar_);

            // grammar   = {rule}
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "MetaGrammar"),
                new HardElement("sequence", string.Empty,
                    new HardElement("identifier", "rule")),
                new HardElement("optional", string.Empty,
                    new HardElement("identifier", MetaParser.Settings___))));

            // rule = identifier '=' expression ';'
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "rule"),
                new HardElement("identifier", "identifier"),
                new HardElement("symbol", "="),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", ";")));

            // expression = element {[or] element};
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "expression"),
                new HardElement("identifier", MetaParser.Element____),
                new HardElement("sequence", string.Empty,
                    new HardElement("optional", string.Empty,
                        new HardElement("identifier", MetaParser.Or_________)),
                    new HardElement("identifier", MetaParser.Element____))));

            //// element    = identifier | symbol | block; Husk ny block
            //grammar.AddElement(new HardElement("rule", string.Empty,
            //    new HardElement("identifier", MetaParser.Element____),
            //    new HardElement("identifier", "identifier"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", "symbol"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", MetaParser.Block______)));

            //// block      = sequence | optional | parentheses);
            //grammar.AddElement(new HardElement("rule", string.Empty,
            //    new HardElement("identifier", MetaParser.Block______),
            //    new HardElement("identifier", "sequence"),
            //    new HardElement(MetaParser.Or_________, string.Empty),
            //    new HardElement("identifier", "optional"),
            //    new HardElement("or", string.Empty),
            //    new HardElement("identifier", "parentheses")));
            // element    = identifier | symbol | block; Husk ny block
            grammar.Add(new HardElement("rule", string.Empty,
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
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "sequence"),
                new HardElement("symbol", "{"),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", "}")));

            // optional     = '[' expression ']';
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "optional"),
                new HardElement("symbol", "["),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", "]")));

            // parentheses      = '(' expression ')';
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "parentheses"),
                new HardElement("symbol", "("),
                new HardElement("identifier", "expression"),
                new HardElement("symbol", ")")));

            // or         = '|';
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "or"),
                new HardElement("symbol", "|")));

            //// ruleId      > identifier;
            //grammar.AddElement(new HardElement("rule", string.Empty,
            //    new HardElement("identifier", "ruleId"),
            //    new HardElement("identifier", "identifier")));

            // symbol     = string;
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "symbol"),
                new HardElement("identifier", "string")));

            // settings    = 'settings' {setter};
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "settings"),
                new HardElement("symbol", "settings"),
                new HardElement("sequence", string.Empty,
                    new HardElement("identifier", MetaParser.Setter_____))));

            // setter      = identifier assignment {',' assignment} ';';
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "setter"),
                new HardElement("identifier", "identifier"),
                new HardElement("identifier", "assignment"),
                new HardElement("sequence", string.Empty,
                    new HardElement("symbol", ","),
                    new HardElement("identifier", "assignment")),
                new HardElement("symbol", ";")));

            // assignment  = property ['=' value];
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "assignment"),
                new HardElement("identifier", MetaParser.Property___),
                new HardElement("optional", string.Empty,
                    new HardElement("symbol", "="),
                    new HardElement("identifier", MetaParser.Value______))));

            // property    = identifier;
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "property"),
                new HardElement("identifier", "identifier")));

            // value       = string;
            grammar.Add(new HardElement("rule", string.Empty,
                new HardElement("identifier", "value"),
                new HardElement("identifier", "string")));

            // settings
            // expression collapse;
            grammar.Add(new HardElement("setter", string.Empty,
                new HardElement("identifier", "expression"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            // element    collapse;
            grammar.Add(new HardElement("setter", string.Empty,
                new HardElement("identifier", "element"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            //// block collapse;
            //grammar.AddElement(new HardElement("setter", string.Empty,
            //    new HardElement("identifier", "block"),
            //    new HardElement("assignment", string.Empty,
            //        new HardElement("property", "collapse"))));

            // settings collapse;
            grammar.Add(new HardElement("setter", string.Empty,
                new HardElement("identifier", "settings"),
                new HardElement("assignment", string.Empty,
                    new HardElement("property", "collapse"))));

            return grammar;
        }

        #endregion utillity functions
    }
}
