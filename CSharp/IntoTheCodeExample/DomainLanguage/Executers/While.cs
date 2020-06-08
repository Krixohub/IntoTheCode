using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class While : OperationBase
    {
        public ExpTyped<bool> Expression;
        public OperationBase Body;

        public override bool Run(Variables runtime)
        {
            int _iterations = 0;
            while (Expression.Compute(runtime))
                if (++_iterations > Program.MaxIterations)
                    throw new Exception(string.Format("Stackoverflow. Max loop iterations is {0}", Program.MaxIterations));
                else
                    if (Body.Run(runtime)) return true;

            return false;
        }
    }
}
