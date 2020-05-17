using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpEquals : ExpTyped<bool>
    {
        private Func<Variables, bool> _run;

        public ExpEquals(ExpBase op1, ExpBase op2)
        {
            if (IsInt(op1, op2))
                _run = (runtime) => op1.RunAsInt(runtime) == op2.RunAsInt(runtime);
            else if (IsNumber(op1, op2))
                _run = (runtime) => op1.RunAsFloat(runtime) == op2.RunAsFloat(runtime);
            else
                _run = (runtime) => op1.RunAsString(runtime) == op2.RunAsString(runtime);
        }

        public override bool Run(Variables runtime)
        {
            return _run(runtime);
        }
    }
}
