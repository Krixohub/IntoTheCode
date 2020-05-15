using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class VariableDef : ProgramBase
    {
        //public VariableDef(string name, DefType variableType, ExpBase exp) : base()
        //{
        //    // variableDef = typeAndId '=' exp ';';

        //    VariableType = variableType;
        //    Name = name;
        //    Expression = exp;
        //}

        public string Name;

        public DefType VariableType;

        public ExpBase Expression;

        public override bool Run(Context runtime)
        {
            runtime.DeclareVariable(VariableType, Name, Expression);
            return false;
        }
    }
}
