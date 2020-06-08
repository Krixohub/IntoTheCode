using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class VariableDef : OperationBase
    {
        public string Name;

        public DefType VariableType;

        public ExpBase Expression;

        public override bool Run(Variables runtime)
        {
            runtime.DeclareVariable(VariableType, Name, Expression);
            return false;
        }
    }
}
