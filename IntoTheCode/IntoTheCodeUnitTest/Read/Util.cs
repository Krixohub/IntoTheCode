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