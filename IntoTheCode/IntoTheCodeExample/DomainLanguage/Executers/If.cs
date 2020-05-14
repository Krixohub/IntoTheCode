using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class If : ProgramBase
    {
        private ExpTyped<bool> _expression;
        private ProgramBase _trueStatement;
        private ProgramBase _elseStatement;

        public If(CodeElement elem, Context compileContext) : base()
        {
            //DefType resultType = compileContext.FunctionScope.ResultType;

        }

        //public override bool AlwaysReturnsValue()
        //{
        //    return _ifStatement.AlwaysReturnsValue() && _elseStatement.AlwaysReturnsValue();
        //}

        public override bool Run(Context runtime)
        {
            if (_expression.Run(runtime))
                return _trueStatement.Run(runtime);
            else
                return _elseStatement.Run(runtime);
        }
    }
}
