using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpSumString : ExpTyped<string>
    {
        private Func<Context, string> _run;

        public ExpSumString(ExpBase op1, ExpBase op2)
        {
            _run = (runtime) => op1.RunAsString(runtime) + op2.RunAsString(runtime);
        }

        public override string Run(Context runtime)
        {
            return _run(runtime);
        }
    }
}
