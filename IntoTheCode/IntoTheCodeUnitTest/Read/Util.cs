using IntoTheCode;
using IntoTheCode.Basic.Util;
using IntoTheCode.Buffer;
using IntoTheCode.Message;
using IntoTheCode.Read;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IntoTheCodeUnitTest.Read
{
    class Util
    {
        public static void ParseErrorResPos(string errorName, int line, int col, string actual, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string expected = BuildMsg(line, col, resourceExpression, parm);
            Assert.AreEqual(expected, actual, errorName);
        }

        public static string BuildMsg(int line, int col, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            return DotNetUtil.Msg(resourceExpression, parm) + " " + string.Format(MessageRes.LineAndCol, line, col);
        }

        public static void ExpressionGrammarParse(string test, string grammar, string code, string expect)
        {
            var parser = new Parser(grammar);
            var buf = new FlatBuffer(code);
            CodeDocument doc = parser.ParseString(buf);
            string errMsg = buf.Status.Error?.Message;
            Assert.IsNotNull(doc, test + " doc er null " + errMsg ?? "");
            string actual = doc.ToMarkup();
            string err = CompareTextLines(actual, expect);

            Assert.AreEqual(string.Empty, err, test + " AST");
        }

        /// <summary>Compare two strings. Line for line and char for char.</summary>
        /// <returns>Empty string if equal. Message with line and char number if different.</returns>
        public static string CompareTextLines(string actual, string expect)
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

        /// <summary>Create new buffer with a function to skip whitespaces.</summary>
        /// <param name="code">Contains of buffer.</param>
        /// <returns>A TextBuffer.</returns>
        public static TextBuffer NewBufferWs(string code)
        {
            Action<TextBuffer> findNextWord = buf =>
            {
                // Skip whitespaces.
                while (!buf.IsEnd() && " \r\n\t".Contains(buf.GetChar()))
                    buf.IncPointer();

                // todo: Read comments
            };


            TextBuffer textBuffer = new FlatBuffer(code);
            //textBuffer.FindNextWordAction = findNextWord;
            textBuffer.ReaderWhitespace.Load(null, 0);
            return textBuffer;
        }

        /// <summary>Test the simple hard coded grammar.</summary>
        /// <param name="buf">String to parse.</param>
        /// <param name="expected">Expected markup from rule output.</param>
        /// <param name="rulename">Name of rule to test.</param>
        public static void MetaHard(string buf, string expected, string rulename)
        {
            var parser = new Parser();
            var outElements = new List<CodeElement>();

            TextBuffer textBuffer = Util.NewBufferWs(buf);
            SetHardCodedTestRules(parser, textBuffer);
            Rule rule = parser.Rules.FirstOrDefault(e => e.Name == rulename);
            Assert.AreEqual(true, rule.Load(outElements, 0), string.Format("rule '{0}': cant read", rulename));
            string actual = ((CodeElement)outElements[0]).ToMarkupProtected(string.Empty);
            Assert.AreEqual(expected, actual, "Equation TestOption: document fail");
        }

        #region utillity functions

        /// <summary>A grammar for test. Hard coded.</summary>
        private static void SetHardCodedTestRules(Parser parser, TextBuffer buffer)
        {
            //bool equal = false;
            //bool tag = true;
            List<RuleLink> symbolsToResolve = new List<RuleLink>();
            List<Rule> list = new List<Rule>();
            list.Add(new Rule("grammar",
                    new Or(new RuleLink("TestIdentifier"),
                    new Or(new RuleLink("TestString"),
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
                new Sequence(new WordIdent("finn"))));
            // TestOption       = 'TestOption' [TestIdentifier] [TestString];
            list.Add(new Rule("TestOption",
                new WordSymbol("TestOption"),
                new Optional(new RuleLink("TestIdentifier")),
                new Optional(new RuleLink("TestQuote2")))
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

        #endregion utillity functions

        public static CodeElement WordLoad(string buf, WordBase word, string value, string name, int from, int to, int end)
        {
            string n = string.Empty;
            TextBuffer textBuffer = Util.NewBufferWs(buf);
            word.TextBuffer = textBuffer;

            var outNo = new List<CodeElement>();
            //var idn = new WordIdent("kurt") { TextBuffer = textBuffer };
            Assert.AreEqual(true, word.Load(outNo, 0), n + "Identifier: Can't read");

            CodeElement node = null;
            if (outNo.Count > 0 || to > 0)
            {
                node = outNo[0] as CodeElement;
                Assert.IsNotNull(node, n + "Identifier: Can't find node after reading");
                Assert.AreEqual(value, node.Value, n + "Identifier: The value is not correct");
                Assert.AreEqual(name, node.Name, n + "Identifier: The name is not correct");
                Assert.AreEqual(from, ((TextSubString)node.SubString).From, n + "Identifier: The start is not correct");
                Assert.AreEqual(to, ((TextSubString)node.SubString).To, n + "Identifier: The end is not correct");
            }
            Assert.AreEqual(end, textBuffer.PointerNextChar, n + "Identifier: The buffer pointer is of after reading");

            return node;
        }

        public static void WordLoadError(string buf, WordBase word, string testName, string expected)
        {
            TextBuffer textBuffer = Util.NewBufferWs(buf);
            word.TextBuffer = textBuffer;
            Rule rule = new Rule("testRule", word);

            //rule.add
            var outNo = new List<CodeElement>();
            //var idn = new WordIdent("kurt") { TextBuffer = textBuffer };
            Assert.AreEqual(false, word.Load(outNo, 0), "No read error");
            Assert.AreEqual(false, word.ResolveErrorsForward(), "No read error");
            Assert.IsNotNull(textBuffer.Status.Error, testName + " No error object");

            string actual = textBuffer.Status.Error.Message;
            string s = string.Join("\r\n", textBuffer.Status.AllErrors.Select(err => err.Message).ToArray());
            
            //ParseErrorResPos(testName, line, col, errMsg1, resourceExpression, parm);
            //string expected = 
            Assert.AreEqual(expected, actual, testName);
        }

    }
}