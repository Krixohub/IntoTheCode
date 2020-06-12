using IntoTheCode;
using IntoTheCodeExample.Expression.Executers;
using System;
using System.Linq;

namespace IntoTheCodeExample.Expression
{
    public class ExpressionVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"1 + 2 * (4 - 1)";

        private string _initialGrammar = @"input  = [exp];
exp    = power | mul | sum | div | sub | par | number;
mul    = exp '*' exp;
div    = exp '/' exp;
sum    = exp '+' exp;
sub    = exp '-' exp;
power  = exp '^' exp;
number = int;
par    = '(' exp ')';
settings
exp    collapse;
mul    Precedence = '2';
sum    Precedence = '1';
div    Precedence = '2';
sub    Precedence = '1';
power  RightAssociative;
//number collapse;
//par collapse;";

        public ExpressionVievModel ()
        {
            Grammar = _initialGrammar;
            Input = _initialInput;
        }

        protected override void ProcessOutput(TextDocument doc)
        {
            // Compile expression
            ExpressionBase expression;
            try
            {
                expression = ExpressionBuilder.BuildExp(doc.ChildNodes.OfType<CodeElement>().FirstOrDefault());
            }
            catch (Exception e)
            {
                Output = "Expression does not compile.\r\n(The processing of parser output fails)\r\n" + e.Message;
                return;
            }

            // execute expression
            float result;
            try
            {
                result = expression.execute();
            }
            catch (Exception e)
            {
                Output = "Expression cant execute.\r\n" + e.Message;
                return;
            }

            Output = "Expression result: " + result;
        }

    }
}
