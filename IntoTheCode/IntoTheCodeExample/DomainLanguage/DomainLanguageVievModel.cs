﻿using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage
{
    public class DomainLanguageVievModel : ExampleVievModelBase
    {
        private string _initialInput = @"int a = 3;
int b = 6;
Write(a);

while (b > 4)
{ 
   int a = 2;
   Write(a);
   Write(b);
   b = n2(2);
}

int n2(int n)
{
  int a = n * n;
  Write(a);
  return a;
}
";

        private string _initialInput2 = @"int a = 4;
int b = 6;
while (3 > 2 *1)
{ 
   int a = 1;
}
";

        private string _initialGrammar = @"program     = scope;
body        = command | '{' scope '}';
scope       = {functionDef | variableDef | command};
command     = assign | if | while | funcCall ';' | return;

functionDef = declare '(' [declare {',' declare}] ')' '{' scope '}';
variableDef = declare '=' exp ';';

declare     = (defInt | defString | defReal | defBool | defVoid) identifier;
defInt      = 'int';
defString   = 'string';
defReal     = 'real';
defBool     = 'bool';
defVoid     = 'void';

assign      = identifier '=' exp ';';
return      = 'return' [exp] ';';
if          = 'if' '(' exp ')' body ['else' body];
while       = 'while' '(' exp ')' body;
funcCall    = identifier '(' [exp {',' exp}] ')';

exp         = mul | div | sum | sub | gt | lt | eq | value | '(' exp ')';
mul         = exp '*' exp;
div         = exp '/' exp;
sum         = exp '+' exp;
sub         = exp '-' exp;
gt          = exp '>' exp;
lt          = exp '<' exp;
eq          = exp '==' exp;
value       = int | real | string | funcCall | variable;
real        = int;
variable    = identifier;

settings
command     collapse;
exp         collapse;
mul         Precedence = '2';
div         Precedence = '2';
sum         Precedence = '1';
sub         Precedence = '1';
value       collapse;";

        public DomainLanguageVievModel()
        {
            Grammar = _initialGrammar;
            Input = _initialInput;
        }

        protected override void ProcessOutput(TextDocument doc)
        {
            Output = string.Empty;

            // Compile expression
            Program program;
            Dictionary<string, Function> functions = Program.AddFunction(null, "Write", WriteLine, "str");
            Dictionary<string, ValueBase> parameters = null;
            try
            {
                program = ProgramCompiler.CreateProgram(doc, functions, parameters);
            }
            catch (Exception e)
            {
                Output = "Program does not compile.\r\n(Compiling the parser output fails)\r\n" + e.Message;
                return;
            }

            // execute program
            try
            {
                program.RunProgram(parameters);
            }
            catch (Exception e)
            {
                Output = "Program cant execute.\r\n" + e.Message;
                return;
            }

            Output += "Program has executed!" ;
        }

        private void WriteLine(string line)
        {
            Output += line + "\r\n";
        }

}
}
