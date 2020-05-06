using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage
{
    public class DomainLanguageVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"int a = 4;
int b = 6;
while (3)
{ 
   int a = 1;
}";

        private string _initialGrammar = @"program   = {statement};
block     = statement | '{' {statement} '}';
statement = assign | if | loop;
assign    = type identifier '=' exp ';';
if        = 'if' '(' exp ')' block ['else' block];
loop      = 'while' '(' exp ')' block;
type      = tbool | tstring | tint;
tbool     = 'bool';
tstring   = 'string';
tint      = 'int';

exp       = mul | div | sum | sub | number | identifier | '(' exp ')';
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

            Output = "Expression result: brae" ;
        }

    }
}
