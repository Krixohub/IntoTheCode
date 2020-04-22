using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntoTheCode.Read.Element;
using IntoTheCode.Read.Element.Words;
using IntoTheCode.Read;
using IntoTheCode.Read.Element.Struckture;
using IntoTheCodeUnitTest.Read;
using IntoTheCode.Message;
using IntoTheCode;

namespace Read.Element
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
                    new Parentheses(
                        link3,
                        new WordSymbol("*") { Precedence = 2 },
                        link4),
                    new Or(new RuleLink(plusName) { RuleElement = ruleP },
                        new WordIdent())));

            link4.RuleElement = link3.RuleElement = link2.RuleElement = link1.RuleElement = ruleE;

            var expr = new Expression(ruleE, ruleE.SubElements[0] as Or);
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

            var expr = new Expression(ruleE, ruleE.SubElements[0] as Or);
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

            // Set grammar


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
            Util.ParserLoad("expr 1 Simple", grammar, code, markup);

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
            Util.ParserLoad("expr 2 double", grammar, code, markup);

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
            Util.ParserLoad("expr 3 swap", grammar, code, markup);

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
            Util.ParserLoad("expr 4 5 operator", grammar, code, markup);

            // --------------------------------- nested expression ----------------------------
            // Set grammar
            grammar = @"aaa = exp;exp = mul | sum | div | sub | gt | '(' exp ')' | identifier;
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

            code = "a + b * ( c - d)  > e / f ";
            markup = @"<aaa>
  <gt>
    <sum>
      <identifier>a</identifier>
      <mul>
        <identifier>b</identifier>
        <sub>
          <identifier>c</identifier>
          <identifier>d</identifier>
        </sub>
      </mul>
    </sum>
    <div>
      <identifier>e</identifier>
      <identifier>f</identifier>
    </div>
  </gt>
</aaa>
";
            Util.ParserLoad("expr 5 nested", grammar, code, markup);

//            grammar = @"expr   = mul | sum | div | sub | int | identifier;
//mul    = expr '*' expr;
//div    = expr '/' expr;
//sum    = expr '+' expr;
//sub    = expr '-' expr;";
//            Parser prsr = new Parser(grammar);
//            var doc2 = CodeDocument.Load(prsr, "-4+-5 ");

    }


    [TestMethod]
        public void ExpressionError()
        {
            string grammar;
            string code;
            string markup = string.Empty;

            // --------------------------------- Simple expression ---------------------------------

            // --------------------------------- 5 operator ---------------------------------
            // Set grammar
            grammar = @"exp = mul | sum | div | sub | gt | '(' exp ')' | identifier;
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

            //string expected;

            code = "a + b * c - d  > e /  + f";
            //pe08: Syntax error (exp). Expecting value for expression. Line 1, colomn 23
            Util.ParserLoad("EOF error", grammar, code, markup,
                Util.BuildMsg(1, 23, () => MessageRes.pe08, "exp"));


            code = "a + b * c - d  > e &  + f";
            //p05: End of input not reached. Line 1, colomn 20
            Util.ParserLoad("EOF error", grammar, code, markup,
                Util.BuildMsg(1, 19, () => MessageRes.pe09, "exp"));

        }

    }
}
