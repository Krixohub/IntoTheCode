namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpMinusFloat : ExpBaseTyped<float>
    {
        private ExpBaseTyped<float> _op1;
        private ExpBaseTyped<float> _op2;

        public ExpMinusFloat(ExpBase op1, ExpBase op2)
        {
            _op1 = GetFloatOperant(op1);
            _op2 = GetFloatOperant(op2);
        }

        public override float Calculate()
        {
            return _op1.Calculate() - _op2.Calculate();
        }
    }
}
