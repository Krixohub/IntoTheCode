using IntoTheCode;
using System;
using System.Data;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Assign : ProgramBase
    {

        public Assign(CodeElement elem, Context compileContext) : base()
        {
            // assign      = identifier '=' exp ';';
            CodeElement idElem = elem.Codes(WordIdentifier).First();
            VariableName = idElem.Value;
            VariableType = compileContext.ExistsVariable(VariableName, idElem);

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

        public string VariableName;

        public DefType VariableType;

        public ExpBase Expression;

        public override bool Run(Context runtime)
        {
            throw new NotImplementedException("lige her");
        }
    }
}
