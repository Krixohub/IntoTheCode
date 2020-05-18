using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class While : ProgramBase
    {
        public ExpTyped<bool> Expression;
        public ProgramBase Body;


        public override bool Run(Variables runtime)
        {
            while (Expression.Compute(runtime))
                if (Body.Run(runtime)) return true;

            return false;
        }
    }
}
