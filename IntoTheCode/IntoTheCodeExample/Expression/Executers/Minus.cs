using IntoTheCode;
using IntoTheCode.Basic;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Minus : ExpressionBase
    {
        private ExpressionBase _op1;
        private ExpressionBase _op2;

        public Minus(CodeElement elem)
        {
            _op1 = Factory(elem.SubElements[0]);
            _op2 = Factory(elem.SubElements[1]);
        }

        public override float execute()
        {
            return _op1.execute() - _op2.execute();
        }
    }
}
