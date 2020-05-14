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

        private string _initialGrammar = @"program     = localScope;
body        = command | '{' localScope '}';
localScope  = {variableDef | functionDef | command};
command     = assign | if | loop | func ';' | return;

variableDef = typeAndId '=' exp ';';
functionDef = typeAndId '(' [typeAndId {',' typeAndId}] ')' '{' localScope '}';

typeAndId   = (defInt | defString | defReal | defBool | defVoid) identifier;
defInt      = 'int';
defString   = 'string';
defReal     = 'real';
defBool     = 'bool';
defVoid     = 'void';

assign      = identifier '=' exp ';';
func        = identifier '(' [exp {',' exp}] ')';
return      = 'return' [exp] ';';
if          = 'if' '(' exp ')' body ['else' body];
loop        = 'while' '(' exp ')' body;

exp         = mul | div | sum | sub | gt | lt | eq | value | '(' exp ')';
mul         = exp '*' exp;
div         = exp '/' exp;
sum         = exp '+' exp;
sub         = exp '-' exp;
gt          = exp '>' exp;
lt          = exp '<' exp;
eq          = exp '==' exp;
value       = int | real | string | func | var;
real        = int;
var         = identifier;

settings
command     collapse;
exp         collapse;
mul         Precedence = '7';
div         Precedence = '7';
sum         Precedence = '6';
sub         Precedence = '6';
value       collapse;";

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
