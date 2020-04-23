using IntoTheCode;
using System.Linq;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Minus : ExpressionBase
    {
        private ExpressionBase _op1;
        private ExpressionBase _op2;

        public Minus(TextElement elem)
        {
            CodeElement first = elem.SubElements.OfType<CodeElement>().FirstOrDefault();
            CodeElement next = elem.SubElements.OfType<CodeElement>().FirstOrDefault(c => c != first);
            _op1 = Factory(first);
            _op2 = Factory(next);
        }

        public override float execute()
        {
            return _op1.execute() - _op2.execute();
        }
    }
}
