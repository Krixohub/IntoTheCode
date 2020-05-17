using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class If : ProgramBase
    {
        public ExpTyped<bool> Expression;
        public ProgramBase TrueStatement;
        public ProgramBase ElseStatement;

        public override bool Run(Variables runtime)
        {
            if (Expression.Run(runtime))
                return TrueStatement.Run(runtime);
            else
                return ElseStatement.Run(runtime);
        }
    }
}
