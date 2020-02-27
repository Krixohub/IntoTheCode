using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Read;
using IntoTheCode;
using IntoTheCode.Buffer;

namespace IntoTheCodeUnitTest.Read.Element
{
    [TestClass]
    public class Read_Element_ExpressionTest
    {
        [TestMethod]
        public void ExpressionCreate()
        {
            string exprName = "expr";
            string plusName = "plus";

            // "plus = expr '+' expr;"
            var link1 = new RuleLink(exprName);
            var link2 = new RuleLink(exprName);
            var link3 = new RuleLink(exprName);
            var link4 = new RuleLink(exprName);

            var ruleP = new Rule(
                plusName,
                link1,
                new WordSymbol("+"),
                link2
                );

            // "expr = expr * expr | plus | identifyer;"
            var ruleE = new Rule(exprName,
                new Or(
                    new Parentheses(
                        link3,
                        new WordSymbol("*"),
                        link4),
                    new Or(new RuleLink(plusName) { RuleElement = ruleP },
                        new WordIdent(MetaParser.WordIdent__))));

            link4.RuleElement = link3.RuleElement = link2.RuleElement = link1.RuleElement = ruleE;

            var expr = new Expression(ruleE, ruleE.SubElements[0] as Or);
            //ruleE.ReplaceSubElement(ruleE.SubElements[0], expr);
            Assert.AreEqual(2, expr._binaryOperators.Count, "There should be 2 operators.");
            Assert.AreEqual("*", expr._binaryOperators[0].Name, "There multiplier name.");
            Assert.AreEqual(plusName, expr._binaryOperators[1].Name, "There sum name.");
            Assert.AreEqual(1, expr._otherForms.Count, "There should be 1 other form.");
            Assert.IsInstanceOfType(expr._otherForms[0], typeof(WordIdent), "Type of other form");
            Assert.AreEqual("identifier", ((WordIdent)expr._otherForms[0]).Name, "Name of ident.");
        }

        [TestMethod]
        public void ExpressionGrammar()
        {
            // Set grammar
            string grammar = @"exp =  exp '*' exp | identifier;";
            var parser = new Parser(grammar);
            //ParserStatus proces;
            CodeDocument doc;
            string errMsg1;

            // What the parser CAN read
            TextBuffer buf = new FlatBuffer("a * b");
            doc = parser.ParseString(buf); ;
            Assert.IsNotNull(doc, "doc er null " + buf.Status.Error.Message);
            Assert.IsNull(buf.Status.Error, "Parse error");

        }
    }
}
