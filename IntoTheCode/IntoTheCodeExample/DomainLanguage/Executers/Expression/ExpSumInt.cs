using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpSumInt : ExpTyped<int>
    {
        private Func<Variables, int> _run1;
        private Func<Variables, int> _run2;

        public ExpSumInt(ExpBase op1, ExpBase op2)
        {
            _run1 = ((ExpTyped<int>)op1).Compute;
            _run2 = ((ExpTyped<int>)op2).Compute;
        }

        public override int Compute(Variables runtime)
        {
            return _run1(runtime) + _run2(runtime);
        }
    }
}
