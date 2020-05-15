using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class While : ProgramBase
    {
        public ExpTyped<bool> Expression;
        public ProgramBase Body;


        public override bool Run(Context runtime)
        {
            while (Expression.Run(runtime))
                if (Body.Run(runtime)) return true;

            return false;
        }
    }
}
