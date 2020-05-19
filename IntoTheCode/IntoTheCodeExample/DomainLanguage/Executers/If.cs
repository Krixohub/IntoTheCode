using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class If : OperationBase
    {
        public ExpTyped<bool> Expression;
        public OperationBase TrueStatement;
        public OperationBase ElseStatement;

        public override bool Run(Variables runtime)
        {
            if (Expression.Compute(runtime))
                return TrueStatement.Run(runtime);
            else
                return ElseStatement.Run(runtime);
        }
    }
}
