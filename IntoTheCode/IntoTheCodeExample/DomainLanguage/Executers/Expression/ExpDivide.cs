using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpDivide : ExpTyped<float>
    {
        private Func<Variables, float> _run;

        public ExpDivide(ExpBase op1, ExpBase op2)
        {
            if (IsInt(op1, op2))
            {
                Func<Variables, int> run1 = ((ExpTyped<int>)op1).Compute;
                Func<Variables, int> run2 = ((ExpTyped<int>)op2).Compute;
                _run = (runtime) => run1(runtime) / run2(runtime);
            }
            else
            {
                Func<Variables, float> run1 = op1.RunAsFloat;
                Func<Variables, float> run2 = op2.RunAsFloat;
                _run = (runtime) => run1(runtime) / run2(runtime);
            }
        }

        public override float Compute(Variables runtime)
        {
            return _run(runtime);
        }
    }
}
