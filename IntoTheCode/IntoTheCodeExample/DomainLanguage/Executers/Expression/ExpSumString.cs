using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpSumString : ExpTyped<string>
    {
        private Func<Variables, string> _run1;
        private Func<Variables, string> _run2;

        public ExpSumString(ExpBase op1, ExpBase op2)
        {
            _run1 = op1.RunAsString;
            _run2 = op2.RunAsString;
        }

        public override string Compute(Variables runtime)
        {
            return _run1(runtime) + _run2(runtime);
        }
    }
}
