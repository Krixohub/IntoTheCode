using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class While : OperationBase
    {
        public ExpTyped<bool> Expression;
        public OperationBase Body;

        public override bool Run(Variables runtime)
        {
            while (Expression.Compute(runtime))
                if (Body.Run(runtime)) return true;

            return false;
        }
    }
}
