using IntoTheCode;
using System.Linq;

namespace IntoTheCodeExample.Expression.Executers
{
    public class Divide : ExpressionBase
    {
        private ExpressionBase _op1;
        private ExpressionBase _op2;

        public Divide(TextElement elem)
        {
            CodeElement first = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault();
            CodeElement next = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault(c => c != first);
            _op1 = ExpressionBuilder.BuildExp(first);
            _op2 = ExpressionBuilder.BuildExp(next);
        }

        public override float execute()
        {
            return _op1.execute() / _op2.execute();
        }
    }
}
