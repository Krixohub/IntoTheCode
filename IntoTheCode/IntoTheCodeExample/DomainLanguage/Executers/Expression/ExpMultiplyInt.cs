using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMultiplyInt : ExpTyped<int>
    {
        private Func<Context, int> _run;

        public ExpMultiplyInt(ExpBase op1, ExpBase op2)
        {
            _run = (runtime) => op1.RunAsInt(runtime) * op2.RunAsInt(runtime);
        }

        public override int Run(Context runtime)
        {
            return _run(runtime);
        }
    }
}
