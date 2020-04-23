using IntoTheCode;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Parenthese : ExpressionBase
    {
        private ExpressionBase _op;

        public Parenthese(CodeElement elem)
        {
            _op = Factory(elem.SubElements[0]);
        }

        public override float execute()
        {
            return _op.execute();
        }
    }
}
