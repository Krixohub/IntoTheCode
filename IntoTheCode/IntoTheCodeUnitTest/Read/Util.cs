using IntoTheCode;
using IntoTheCode.Basic.Util;
using IntoTheCode.Buffer;
using IntoTheCode.Message;
using IntoTheCode.Read;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace IntoTheCodeUnitTest.Read
{
    class Util
    {
        public static void ParseErrorResPos(string errorName, int line, int col, string actual, Expression<Func<string>> resourceExpression, params object[] parm)
        {
            string expected = DotNetUtil.Msg(resourceExpression, parm) + " " + string.Format(MessageRes.LineAndCol, line, col);
            Assert.AreEqual(expected, actual, errorName);
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


    }
}
