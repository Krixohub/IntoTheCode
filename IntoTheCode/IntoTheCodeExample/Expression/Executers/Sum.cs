using IntoTheCode;
using IntoTheCode.Basic;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Sum : ExpressionBase
    {
        private ExpressionBase _op1;
        private ExpressionBase _op2;

        public Sum(TopElement elem)
        {
            _op1 = CreateExpression(elem.SubElements[0]);
            _op2 = CreateExpression(elem.SubElements[1]);
        }

        public override float execute()
        {
            return _op1.execute() + _op2.execute();
        }
    }
}
