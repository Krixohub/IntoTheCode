using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMinusInt : ExpTyped<int>
    {
        private Func<Variables, int> _run;

        public ExpMinusInt(ExpBase op1, ExpBase op2)
        {
            _run = (runtime) => op1.RunAsInt(runtime) - op2.RunAsInt(runtime);
        }

        public override int Compute(Variables runtime)
        {
            return _run(runtime);
        }
    }
}
