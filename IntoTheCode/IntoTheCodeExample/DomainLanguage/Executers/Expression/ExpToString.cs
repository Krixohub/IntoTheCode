namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpToString<TOp> : ExpBaseTyped<string>
    {

        private ExpBaseTyped<TOp> _op;

        public ExpToString(ExpBaseTyped<TOp> op)
        {
            _op = op;
        }

        public override string Calculate()
        {
            return _op.Calculate().ToString();
        }
    }
}
