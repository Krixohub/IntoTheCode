using IntoTheCode;
using IntoTheCode.Basic.Util;
using IntoTheCode.Buffer;
using IntoTheCode.Grammar;
using IntoTheCode.Message;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IntoTheCodeUnitTest.Read
{
    class Util
    {
        /// <summary>Test the hard coded rules.
        /// One rule is testet with the load function.</summary>
        /// <param name="code">String to parse.</param>
        /// <param name="markup">Expected markup from rule output.</param>
        /// <param name="rules">List of rules to test. The first rule is loaded.</param>
        public static void ParserLoadRule(List<Rule> rules, string code, string markup)
        {
            var parser = new Parser();
            var outElements = new List<TextElement>();

            TextBuffer buffer = Util.NewBufferWs(code);
            parser.Rules = rules.Select(r => r.CloneForParse(buffer) as Rule).ToList();
            ParserBuilder.InitializeGrammar(parser, parser.Rules, buffer.Status);
            ParserBuilder.ValidateGrammar(parser, buffer.Status);

            string syntax = parser.GetGrammar();

            Rule rule = parser.Rules[0];
            Assert.AreEqual(true, rule.Load(outElements, 0), string.Format("rule '{0}': cant read", rule.Name));
            string actual = outElements[0].ToMarkupProtected(string.Empty);
            Assert.AreEqual(markup, actual, "Equation TestOption: document fail");
        }

        /// <summary>Test Parser Elements.
        /// First element is testet with the load function.</summary>
        /// <param name="code">String to parse.</param>
        /// <param name="markup">Expected markup from rule output.</param>
        /// <param name="elements">List of rules to test. The first rule is loaded.</param>
        public static void ParserLoadElement(List<ParserElementBase> elements, string code, string markup)
        {
            //var parser = new Parser();
            var outElements = new List<TextElement>();

            TextBuffer buffer = Util.NewBufferWs(code);
            for (int i = 0; i < elements.Count; i++)
                elements[i] = elements[i].CloneForParse(buffer);
            //foreach (var item in elements)
            //    item = item.CloneForParse(buffer);


            //parser.Rules = elements.Select(r => r.CloneForParse(buffer) as Rule).ToList();
            //ParserFactory.InitializeGrammar(parser, parser.Rules, buffer.Status);
            //ParserFactory.ValidateGrammar(parser, buffer.Status);

            ParserElementBase elem = elements[0];
            string syntax = elem.GetGrammar();

            Assert.AreEqual(true, elem.Load(outElements, 0), string.Format("rule '{0}': cant read", elem.Name));
            if (markup.Length > 0)
            {
                string actual = outElements.Aggregate("", (str, e) => str + e.ToMarkupProtected(string.Empty));
                Assert.AreEqual(markup, actual, "Equation TestOption: document fail");
            }
        }

        public static TextElement ParserLoadWord(WordBase word, string code, string value, string name, int from, int to, int end)
        {
            string n = string.Empty;
            TextBuffer textBuffer = Util.NewBufferWs(code);
            word.TextBuffer = textBuffer;

            var outNo = new List<TextElement>();
            //var idn = new WordIdent("kurt") { TextBuffer = textBuffer };
            Assert.AreEqual(true, word.Load(outNo, 0), n + "Identifier: Can't read");

            textBuffer.FindNextWord(null, false);
            TextElement text = null;
            if (outNo.Count > 0 || to > 0)
                text = outNo[0];

            if (text != null && text is CodeElement)
            {
                var codeElem = text as CodeElement;
                Assert.IsNotNull(codeElem, n + "Identifier: Can't find node after reading");
                Assert.AreEqual(value, codeElem.Value, n + "Identifier: The value is not correct");
                Assert.AreEqual(name, codeElem.Name, n + "Identifier: The name is not correct");
                Assert.AreEqual(from, codeElem.SubString.From, n + "Identifier: The start is not correct");
                Assert.AreEqual(to, codeElem.SubString.To, n + "Identifier: The end is not correct");
            }
            if (text != null && text is CommentElement)
            {
                var comm = outNo[0] as CommentElement;
                Assert.IsNotNull(comm, n + "Identifier: Can't find node after reading");
                Assert.AreEqual(value, comm.Value, n + "Identifier: The value is not correct");
                Assert.AreEqual(name, comm.Name, n + "Identifier: The name is not correct");
                Assert.AreEqual(from, comm.SubString.From, n + "Identifier: The start is not correct");
                Assert.AreEqual(to, comm.SubString.To, n + "Identifier: The end is not correct");
            }
            Assert.AreEqual(end, textBuffer.PointerNextChar, n + "Identifier: The buffer pointer is of after reading");

            return text;
        }

        public static void WordLoadError(string buf, WordBase word, string testName, string expected)
        {
            TextBuffer textBuffer = Util.NewBufferWs(buf);
            word.TextBuffer = textBuffer;
            Rule rule = new Rule("testRule", word);

            //rule.add
            var outNo = new List<TextElement>();
            //var idn = new WordIdent("kurt") { TextBuffer = textBuffer };
            Assert.AreEqual(false, word.Load(outNo, 0), "No read error");
            Assert.AreEqual(false, word.ResolveErrorsForward(0), "No read error");
            Assert.IsNotNull(textBuffer.Status.Error, testName + " No error object");

            string actual = textBuffer.Status.Error.Message;
            string s = string.Join("\r\n", textBuffer.Status.AllErrors.Select(err => err.Message).ToArray());

            //ParseErrorResPos(testName, line, col, errMsg1, resourceExpression, parm);
            //string expected = 
            Assert.AreEqual(expected, actual, testName);
        }

        public static string ParserLoad(string grammar, string code, string markup, params string[] errors)
        {
            // Try create parser
            //Parser parser = ParserGrammar(grammar, errors);
            Parser parser = null;
            try { parser = new Parser(grammar); }
            catch (ParserException e)
            {
                Assert.IsTrue(errors.Length > 0, "Grammar error: " + e.Message);
                Assert.IsTrue(e.AllErrors.Count >= errors.Length, "Grammar error: missing error 1");
                e.AllErrors.Sort(ParserError.Compare);
                for (int i = 0; i < e.AllErrors.Count; i++)
                    if (i < errors.Length)
                        Assert.AreEqual(errors[i], e.AllErrors[i].Message, "Grammar error: Build error " + i);
            }

            if (errors.Length == 0)
                Assert.IsNotNull(parser, "Grammar error: parser is null");
            else if (string.IsNullOrEmpty(code) &&  markup != "grammar ok")
                Assert.IsNull(parser, "Grammar error expected");

            if (parser == null || (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(markup))) return string.Empty;


            // Try read the code
            string s = parser.GetGrammar();

            var buf = new FlatBuffer(code);
            CodeDocument doc = parser.ParseString(buf);
            if (buf.Status.Error == null)
            {
                Assert.IsTrue(errors.Count() == 0, " Expecting error");
                Assert.IsNotNull(doc, " doc er null");

                string actual = doc.ToMarkup();
                if (!string.IsNullOrEmpty(markup))
                {
                    string err = CompareTextLines(actual, markup);
                    Assert.AreEqual(string.Empty, err, " AST");
                }
                return actual;
            }
            else
            {
                Assert.IsTrue(errors.Count() > 0, " Parsing error: " + buf.Status.Error.Message);

                Assert.IsTrue(buf.Status.AllErrors.Count >= errors.Length, " Load missing error 1");
                buf.Status.AllErrors.Sort(ParserError.Compare);
                for (int i = 0; i < buf.Status.AllErrors.Count; i++)
                    if (i < errors.Length)
                        Assert.AreEqual(errors[i], buf.Status.AllErrors[i].Message, " Load error " + i);
                    else
                        return buf.Status.AllErrors[i].Message;
            }
            return string.Empty;
        }

        #region utillity functions

        /// <summary>Create new buffer with a function to skip whitespaces.</summary>
        /// <param name="code">Contains of buffer.</param>
        /// <returns>A TextBuffer.</returns>
        public static TextBuffer NewBufferWs(string code)
        {
            TextBuffer textBuffer = new FlatBuffer(code);
            textBuffer.FindNextWord(null, false);
            return textBuffer;
        }

        public static string BuildMsg(int line, int col, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string pos = (line > 0) ? " " + string.Format(MessageRes.LineAndCol, line, col) : string.Empty;
            return DotNetUtil.Msg(resourceExpression, parm) + pos;
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

        #endregion utillity functions

    }
}