using IntoTheCode;
using System;
using System.Data;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Assign : OperationBase
    {

        public string VariableName;

        public DefType VariableType;

        public ExpBase Expression;

        public override bool Run(Variables runtime)
        {
            runtime.SetVariable(VariableName, Expression);
            return false;
        }
    }
}
