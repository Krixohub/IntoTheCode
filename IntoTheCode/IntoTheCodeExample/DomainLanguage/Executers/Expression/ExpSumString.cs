using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpSumString : ExpBaseTyped<string>
    {
        private ExpBaseTyped<string> _op1;
        private ExpBaseTyped<string> _op2;

        public ExpSumString(ExpBase op1, ExpBase op2)
        {
            _op1 = GetStringOperant(op1);
            _op2 = GetStringOperant(op2);
        }

        public override string Calculate()
        {
            return _op1.Calculate() + _op2.Calculate();
        }
    }
}
