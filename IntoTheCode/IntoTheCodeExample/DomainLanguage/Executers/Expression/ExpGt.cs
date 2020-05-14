using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpGt : ExpTyped<bool>
    {
        private Func<Context, bool> _run;

        public ExpGt(ExpBase op1, ExpBase op2)
        {
            if (IsInt(op1, op2))
                _run = (runtime) => op1.RunAsInt(runtime) > op2.RunAsInt(runtime);
            else if (IsNumber(op1, op2))
                _run = (runtime) => op1.RunAsFloat(runtime) > op2.RunAsFloat(runtime);
        }

        public override bool Run(Context runtime)
        {
            return _run(runtime);
        }
    }
}
