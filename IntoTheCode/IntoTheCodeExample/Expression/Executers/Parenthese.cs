using IntoTheCode;
using IntoTheCode.Basic;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Parenthese : ExpressionBase
    {
        private ExpressionBase _op;

        public Parenthese(TreeNode elem)
        {
            _op = CreateExpression(elem.SubElements[0]);
        }

        public override float execute()
        {
            return _op.execute();
        }
    }
}
