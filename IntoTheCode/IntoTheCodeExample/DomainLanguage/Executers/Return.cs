using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Return : ProgramBase
    {


        public ExpBase Expression;

        public override bool Run(Variables runtime)
        {
            runtime.SetVariable(ProgramCompiler.VariableReturn, Expression);
            return true;
        }
    }
}
