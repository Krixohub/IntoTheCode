using IntoTheCode;
using IntoTheCodeExample.Expression.Executers;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage
{
    public class DomainLanguageVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"int a = 4;
int b = 6;
while (3)
{ 
   int a = 1;
};";

        private string _initialGrammar = @"program   = {statement ';'};
block     = statement | '{' {statement ';'} '}';
statement = assign | if | loop;
assign    = type identifier '=' exp;
if        = 'if' '(' exp ')' block ['else' block];
loop      = 'while' '(' exp ')' block;
type      = tbool | tstring | tint;
tbool     = 'bool';
tstring   = 'string';
tint      = 'int';

exp       = mul | div | sum | sub | number | '(' exp ')';
mul       = exp '*' exp;
div       = exp '/' exp;
sum       = exp '+' exp;
sub       = exp '-' exp;
number    = int;

settings
exp       collapse;
mul       Precedence = '4';
div       Precedence = '4';";

        public DomainLanguageVievModel()
        {
            Grammar = _initialGrammar;
            Input = _initialInput;
        }

        protected override void ProcessOutput(TextDocument doc)
        {
            //// Compile expression
            //ExpressionBase expression;
            //try
            //{
            //    expression = ExpressionBase.Factory(doc.ChildNodes.OfType<CodeElement>().FirstOrDefault());
            //}
            //catch (Exception e)
            //{
            //    Output = "Expression does not compile.\r\n(The processing of parser output fails)\r\n" + e.Message;
            //    return;
            //}

            //// execute expression
            //float result;
            //try
            //{
            //    result = expression.execute();
            //}
            //catch (ParserException e)
            //{
            //    Output = "Expression cant execute.\r\n" + e.Message;
            //    return;
            //}

            Output = "Expression result: brae" ;
        }

    }
}
