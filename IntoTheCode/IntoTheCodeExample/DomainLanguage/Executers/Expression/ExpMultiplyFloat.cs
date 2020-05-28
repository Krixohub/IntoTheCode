using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMultiplyFloat : ExpTyped<float>
    {
        private Func<Variables, float> _run1;
        private Func<Variables, float> _run2;

        public ExpMultiplyFloat(ExpBase op1, ExpBase op2)
        {
            _run1 = op1.RunAsFloat;
            _run2 = op2.RunAsFloat;
        }

        public override float Compute(Variables runtime)
        {
            return _run1(runtime) * _run2(runtime);
        }
    }
}
