namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpIntToFloat : ExpBaseTyped<float>
    {

        private ExpBaseTyped<int> _op;

        public ExpIntToFloat(ExpBaseTyped<int> op)
        {
            _op = op;
        }

        public override float Calculate()
        {
            return _op.Calculate();
        }
    }
}
