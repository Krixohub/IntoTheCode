using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class While : ProgramBase
    {
        private ExpTyped<bool> _expression;
        private ProgramBase _statement;

        public While(DefType resultType, CodeElement elem) : base(resultType)
        {

        }

        public override bool Run(Context runtime)
        {
            while (_expression.Run(runtime))
                if (_statement.Run(runtime)) return true;

            return false;
        }
    }
}
