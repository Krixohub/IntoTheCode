using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class VariableDef : ProgramBase
    {
        public VariableDef(CodeElement elem, Context compileContext, FunctionContext functionContext) : base(DefType.Void)
        {
            var def = new TypeAndId(elem.Codes(WordTypeAndId).FirstOrDefault(), true);

            VarType = def.TheType;
            Name = def.TheName;
            if (elem.Codes().Count() == 3)
            {
                CodeElement expCode = elem.Codes().Last();
                Expression = Factory.Expression(expCode, compileContext);
                if (Expression.ExpressionType != VarType)
                {
                    if (VarType == DefType.Bool)
                        throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                    if (VarType == DefType.Int)
                        throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                    if (VarType == DefType.Float && Expression.ExpressionType != DefType.Int)
                        throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
                }
            }
            if (functionContext.ExistsFunction(Name))
                throw new Exception(string.Format("A Function called '{0}', {1}, is declared", Name, elem.GetLineAndColumn()));
            compileContext.DeclareVariable(VarType, Name, elem);
        }

        public string Name;

        public DefType VarType;

        public ExpBase Expression;

        public override bool Run(Context runtime)
        {
            runtime.DeclareVariable(VarType, Name, Expression);
            return false;
        }
    }
}
