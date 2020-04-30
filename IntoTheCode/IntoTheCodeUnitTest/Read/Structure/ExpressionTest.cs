using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCode.Read.Structure;
using IntoTheCode.Read.Words;
using IntoTheCodeUnitTest.Read;
using IntoTheCode.Message;

namespace Read.Structure
{
    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void ITC15ExpressionCreate()
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
                    new SetOfElements(
                        link3,
                        new WordSymbol("*") { Precedence = 2 },
                        link4),
                    new Or(new RuleLink(plusName) { RuleElement = ruleP },
                        new WordIdent())));

            link4.RuleElement = link3.RuleElement = link2.RuleElement = link1.RuleElement = ruleE;

            var expr = new Expression(ruleE, ruleE.ChildNodes[0] as Or);
            //ruleE.ReplaceSubElement(ruleE.SubElements[0], expr);
            Assert.AreEqual(2, expr._binaryOperators.Count, "There should be 2 operators.");
            Assert.AreEqual("*", expr._binaryOperators[0].Name, "There multiplier name.");
            Assert.AreEqual(2, expr._binaryOperators[0].Precedence, "There multiplier precedence.");
            Assert.AreEqual(plusName, expr._binaryOperators[1].Name, "There sum name.");
            Assert.AreEqual(1, expr._otherForms.Count, "There should be 1 other form.");
            Assert.IsInstanceOfType(expr._otherForms[0], typeof(WordIdent), "Type of other form");
            Assert.AreEqual("identifier", ((WordIdent)expr._otherForms[0]).Name, "Name of ident.");
        }

        [TestMethod]
        public void ITC15ExpressionSetPrecedence()
        {
            string exprName = "expr";
            string plusName = "plus";

            // "plus = expr '+' expr;"
            var link1 = new RuleLink(exprName);
            var link2 = new RuleLink(exprName);
           
            var ruleP = new Rule(
                plusName,
                link1,
                new WordSymbol("+"),
                link2
                );

            // "expr = expr * expr | plus | identifyer;"
            var ruleE = new Rule(exprName,
                new Or(new RuleLink(plusName) { RuleElement = ruleP },
                        new WordIdent()));

            link2.RuleElement = link1.RuleElement = ruleE;

            var expr = new Expression(ruleE, ruleE.ChildNodes[0] as Or);
            //ruleE.ReplaceSubElement(ruleE.SubElements[0], expr);
            Assert.AreEqual(1, expr._binaryOperators.Count, "expr 1: 1 operators.");
            Assert.AreEqual(1, expr._binaryOperators[0].Precedence, "expr 1: precedence = 1.");

            ExpressionSetPrecedenceMakeOperators(expr, "expr x", new[] { 0, 0, 0, 0, 0 });
            ExpressionSetPrecedenceTestOperators(expr, "expr 2", new[] { 5, 4, 3, 2, 1 });

            ExpressionSetPrecedenceMakeOperators(expr, "expr x", new[] { 0, 2, 0, 2, 0, 1, 0, 1, 0 });
            ExpressionSetPrecedenceTestOperators(expr, "expr 3", new[] { 9, 8, 7, 8, 6, 5, 4, 5, 3 });

            ExpressionSetPrecedenceMakeOperators(expr, "expr x", new[] { 0, 9, 0, 9, 0, 7, 0, 7, 0 });
            ExpressionSetPrecedenceTestOperators(expr, "expr 4", new[] { 9, 8, 7, 8, 6, 5, 4, 5, 3 });

            // silly order test
            ExpressionSetPrecedenceMakeOperators(expr, "expr x", new[] { 0, 3, 0, 3, 0, 7, 0, 7, 0 });
            ExpressionSetPrecedenceTestOperators(expr, "expr 5", new[] { 9, 7, 6, 7, 5, 8, 4, 8, 3 });

            // silly order test
            ExpressionSetPrecedenceMakeOperators(expr, "expr  x", new[] { 0, 3, 0, 3, 0, 7, 0,  8, 0, 2, 2 });
            ExpressionSetPrecedenceTestOperators(expr, "expr 6", new[] { 11, 8, 7, 8, 6, 9, 5, 10, 4, 3, 3 });

        }

        private void ExpressionSetPrecedenceMakeOperators(Expression expr, string align, int[] precedence)
        {
            expr._binaryOperators.Clear();
            foreach (int pre in precedence)
                expr._binaryOperators.Add(new WordBinaryOperator(new WordSymbol("a"), null, null) { Precedence = pre });
            expr.SetPrecedence();
        }

        private void ExpressionSetPrecedenceTestOperators(Expression expr, string test, int[] precedence)
        {
            for (int i = 0; i < precedence.Length; i++)
                Assert.AreEqual(precedence[i], 
                    expr._binaryOperators[i].Precedence, 
                    string.Format("{0}: operator {1} precedence.", test, i));
        }

        [TestMethod]
        public void ExpressionGrammar()
        {
            string grammar;
            string code;
            string markup;

            // --------------------------------- Simple expression ---------------------------------
            grammar = @"exp =  exp '*' exp | exp '+' exp | identifier;";
            code = "a * b";
            markup = @"<exp>
  <*>
    <identifier>a</identifier>
    <identifier>b</identifier>
  </*>
</exp>
";
            Util.ParserLoad(grammar, code, markup);

            // --------------------------------- double expression ---------------------------------
            code = "a * b + c";
            markup = @"<exp>
  <+>
    <*>
      <identifier>a</identifier>
      <identifier>b</identifier>
    </*>
    <identifier>c</identifier>
  </+>
</exp>
";
            Util.ParserLoad(grammar, code, markup);

            // --------------------------------- double expression swap ---------------------------------
            code = "a + b * c";
            markup = @"<exp>
  <+>
    <identifier>a</identifier>
    <*>
      <identifier>b</identifier>
      <identifier>c</identifier>
    </*>
  </+>
</exp>
";
            Util.ParserLoad(grammar, code, markup);

            // --------------------------------- 5 operator ---------------------------------
            // Set grammar
            grammar = @"exp = mul | sum | div | sub | gt | identifier;
mul = exp '*' exp;
sum = exp '+' exp;
div = exp '/' exp;
sub = exp '-' exp;
gt  = exp '>' exp;
settings
mul Precedence = '2';
div Precedence = '2';
sum Precedence = '1';
sub Precedence = '1';
";
            code = "a + b * c - d  > e / f";
            markup = @"<exp>
  <gt>
    <sub>
      <sum>
        <identifier>a</identifier>
        <mul>
          <identifier>b</identifier>
          <identifier>c</identifier>
        </mul>
      </sum>
      <identifier>d</identifier>
    </sub>
    <div>
      <identifier>e</identifier>
      <identifier>f</identifier>
    </div>
  </gt>
</exp>
";
            Util.ParserLoad(grammar, code, markup);

        }


        [TestMethod]
        public void ExpressionNested()
        {
            string grammar;
            string code;
            string markup;
            // ------------------------ 5 operator, nested expression ----------------------------
            // Set grammar
            grammar = @"aaa = exp;exp = mul | sum | div | sub | gt | '(' exp ')' | int | identifier;
mul = exp '*' exp;
sum = exp '+' exp;
div = exp '/' exp;
sub = exp '-' exp;
gt  = exp '>' exp;
settings
mul Precedence = '2';
div Precedence = '2';
sum Precedence = '1';
sub Precedence = '1';
exp collapse;
";

            code = "a + b * ( c - d) / e ";
            markup = @"<aaa>
  <sum>
    <identifier>a</identifier>
    <div>
      <mul>
        <identifier>b</identifier>
        <sub>
          <identifier>c</identifier>
          <identifier>d</identifier>
        </sub>
      </mul>
      <identifier>e</identifier>
    </div>
  </sum>
</aaa>
";
            markup = Util.ParserLoad(grammar, code, markup);

            code = "(a + b) * ( c - d) / e ";
            markup = @"<aaa>
  <div>
    <mul>
      <sum>
        <identifier>a</identifier>
        <identifier>b</identifier>
      </sum>
      <sub>
        <identifier>c</identifier>
        <identifier>d</identifier>
      </sub>
    </mul>
    <identifier>e</identifier>
  </div>
</aaa>
";
            markup = Util.ParserLoad(grammar, code, markup);

            // --------------------------------- Expression Error ---------------------------------

            code = "a + b * c - d  > e /  + f";
            //pe08: Syntax error (exp). Expecting value for expression. Line 1, colomn 23
            markup = "";
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 23, () => MessageRes.pe08, "exp"));

            code = "a + b &";
            // pe07: Grammar error (mul). Expecting symbol '*', found '&' Line 1, colomn 7
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 7, () => MessageRes.pe07, "mul", "*", "&"));

            code = "a + b * c - d  > e &  + f";
            //p05: End of input not reached. Line 1, colomn 20
            //pe07: Grammar error (mul). Expecting symbol '*', found '&' Line 1, colomn 20
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 20, () => MessageRes.pe07, "mul", "*", "&"));

            code = "a + (b ) ";
            Util.ParserLoad(grammar, code, markup);

            code = " ( b ";
            // pe10: Syntax error (exp). Expecting symbol ')', found EOF. Line 1, colomn 6
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 6, () => MessageRes.pe10, "exp", "symbol ')'", "EOF"));

            code = "a + ( b ";
            markup = "";
            // pe10: Syntax error (exp). Expecting symbol ')', found EOF. Line 1, colomn 6
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 9, () => MessageRes.pe10, "exp", "symbol ')'", "EOF"));

            code = "a + b) ";
            markup = "";
            // pe07: Grammar error (mul). Expecting symbol '*', found ')' Line 1, colomn 6
            Util.ParserLoad(grammar, code, markup,
                Util.BuildMsg(1, 6, () => MessageRes.pe07, "mul", "*", ")"));

            code = "a + 2 * (c + 4)";
            markup = @"<aaa>
  <sum>
    <identifier>a</identifier>
    <mul>
      <int>2</int>
      <sum>
        <identifier>c</identifier>
        <int>4</int>
      </sum>
    </mul>
  </sum>
</aaa>
";
            //markup = "";
            markup = Util.ParserLoad(grammar, code, markup);
            code = "(a + 2) * (c + (2>1 ) * 4)";
            markup = @"<aaa>
  <mul>
    <sum>
      <identifier>a</identifier>
      <int>2</int>
    </sum>
    <sum>
      <identifier>c</identifier>
      <mul>
        <gt>
          <int>2</int>
          <int>1</int>
        </gt>
        <int>4</int>
      </mul>
    </sum>
  </mul>
</aaa>
";
            markup = "";
            markup = Util.ParserLoad(grammar, code, markup);
        }

    }
}
