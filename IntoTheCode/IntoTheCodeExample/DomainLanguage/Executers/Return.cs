using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Return : ProgramBase
    {

        public Return(CodeElement elem, Context compileContext) : base()
        {
            // return      = 'return' [exp] ';';
            CodeElement expCode = elem.Codes().FirstOrDefault();
            if (expCode != null)
            {
                Expression = Factory.Expression(expCode, compileContext);

                // Check resulttype of command
                if (compileContext.FunctionScope.ResultType != Expression.ExpressionType)
                {
                    if (compileContext.FunctionScope.ResultType == DefType.Bool)
                        throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                    if (compileContext.FunctionScope.ResultType == DefType.Int)
                        throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                    if (compileContext.FunctionScope.ResultType == DefType.Float && Expression.ExpressionType != DefType.Int)
                        throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
                }
            }
            else
                if (compileContext.FunctionScope.ResultType != DefType.Void)
                    throw new Exception(string.Format("This function must return a value. {0}", elem.GetLineAndColumn()));
        }

        public ExpBase Expression;

        public override bool Run(Context runtime)
        {
            // todo der skal være en result type
            runtime.SetVariable("return", Expression);
            return true;
        }
    }
}
