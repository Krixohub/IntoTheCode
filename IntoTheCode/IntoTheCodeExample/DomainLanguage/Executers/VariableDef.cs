using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class VariableDef : ProgramBase
    {
        public VariableDef(CodeElement elem, Context compileContext) : base()
        {
            // variableDef = typeAndId '=' exp ';';

            var def = new TypeAndId(elem.Codes(WordTypeAndId).First(), true);

            VariableType = def.TheType;
            Name = def.TheName;
            if (elem.Codes().Count() == 3)
            {
                CodeElement expCode = elem.Codes().Last();
                Expression = Factory.Expression(expCode, compileContext);
                if (Expression.ExpressionType != VariableType)
                {
                    if (VariableType == DefType.Bool)
                        throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                    if (VariableType == DefType.Int)
                        throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                    if (VariableType == DefType.Float && Expression.ExpressionType != DefType.Int)
                        throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
                }
            }
            if (compileContext.FunctionScope.ExistsFunction(Name))
                throw new Exception(string.Format("A Function called '{0}', {1}, is declared", Name, elem.GetLineAndColumn()));

            compileContext.DeclareVariable(VariableType, Name, elem);
        }

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
