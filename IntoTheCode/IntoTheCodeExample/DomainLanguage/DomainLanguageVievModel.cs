using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage
{
    public class DomainLanguageVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"int a = 4;
int b = 6;
while (3 > 2 *1)
{ 
   int a = 1;
}

int n2(int n)
{
  int d = n * n;
}
";

        private string _initialGrammar = @"program   = {statement};
block     = statement | '{' {statement} '}';
body      = '{' {statement} '}';
statement = varDef | funcDef | assign | if | loop | (func ';') | return ;

varDef    = declareId '=' exp ';';
funcDef   = declareId '(' [declareId {',' declareId} ]')' body;
declareId = defType identifier;
defType   = 'bool' | 'string' | 'int' | 'real' | 'void';

assign    = identifier '=' exp ';';
func      = identifier '(' [exp {',' exp} ]')';
return    = 'return' exp ';';
if        = 'if' '(' exp ')' block ['else' block];
loop      = 'while' '(' exp ')' block;

exp       = mul | div | sum | sub | gt | lt | eq | value | '(' exp ')';
mul       = exp '*' exp;
div       = exp '/' exp;
sum       = exp '+' exp;
sub       = exp '-' exp;
gt        = exp '>' exp;
lt        = exp '<' exp;
eq        = exp '==' exp;
value     = int | real | string | func | var;
real      = int;
var       = identifier;

settings
exp       collapse;
declareId collapse;
value     collapse;
mul       Precedence = '2';
div       Precedence = '2';
sum       Precedence = '1';
sub       Precedence = '1';";

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
