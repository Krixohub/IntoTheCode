using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpEquals : ExpTyped<bool>
    {
        private Func<Variables, bool> _run;

        public ExpEquals(ExpBase op1, ExpBase op2)
        {
            if (IsInt(op1, op2))
            {
                Func<Variables, int> run1 = ((ExpTyped<int>)op1).Compute;
                Func<Variables, int> run2 = ((ExpTyped<int>)op2).Compute;
                _run = (runtime) => run1(runtime) == run2(runtime);
            }
            else if (IsNumber(op1, op2))
            {
                Func<Variables, float> run1 = op1.RunAsFloat;
                Func<Variables, float> run2 = op2.RunAsFloat;
                _run = (runtime) => run1(runtime) == run2(runtime);
            }
            else
            {
                Func<Variables, string> run1 = op1.RunAsString;
                Func<Variables, string> run2 = op2.RunAsString;
                _run = (runtime) => run1(runtime) == run2(runtime);
            }
        }

        public override bool Compute(Variables runtime)
        {
            return _run(runtime);
        }
    }
}
