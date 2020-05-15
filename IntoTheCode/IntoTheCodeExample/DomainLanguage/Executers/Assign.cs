using IntoTheCode;
using System;
using System.Data;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Assign : ProgramBase
    {

        public string VariableName;

        public DefType VariableType;

        public ExpBase Expression;

        public override bool Run(Context runtime)
        {
            throw new NotImplementedException("lige her");
        }
    }
}
