using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class If : ProgramBase
    {
        private ExpTyped<bool> _expression;
        private ProgramBase _trueStatement;
        private ProgramBase _elseStatement;

        public If(DefType resultType, CodeElement elem) : base(resultType)
        {

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
