using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpDivide : ExpTyped<float>
    {
        private Func<Context, float> _run;

        public ExpDivide(ExpBase op1, ExpBase op2)
        {
            if (IsInt(op1, op2))
                _run = (runtime) => op1.RunAsInt(runtime) / op2.RunAsInt(runtime);
            else
                _run = (runtime) => op1.RunAsFloat(runtime) / op2.RunAsFloat(runtime);
        }

        public override float Run(Context runtime)
        {
            return _run(runtime);
        }
    }
}
