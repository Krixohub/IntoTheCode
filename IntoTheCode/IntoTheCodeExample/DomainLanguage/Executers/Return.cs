using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class Return : ProgramBase
    {
        private ExpBase _expression;

        public Return(DefType resultType, CodeElement elem) : base(resultType)
        {

        }

        public override bool Run(Context runtime)
        {
            // todo der skal være en result type
            runtime.SetVariable("return", _expression);
            return true;
        }
    }
}
