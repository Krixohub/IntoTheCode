namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMultiplyInt : ExpBaseTyped<int>
    {
        private ExpBaseTyped<int> _op1;
        private ExpBaseTyped<int> _op2;

        public ExpMultiplyInt(ExpBase op1, ExpBase op2)
        {
            _op1 = (ExpBaseTyped<int>)op1;
            _op2 = (ExpBaseTyped<int>)op2;
        }

        public override int Calculate()
        {
            return _op1.Calculate() * _op2.Calculate();
        }
    }
}
