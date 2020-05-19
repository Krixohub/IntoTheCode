namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Return : OperationBase
    {
        public ExpBase Expression;

        public override bool Run(Variables runtime)
        {
            runtime.SetVariable(ProgramCompiler.VariableReturn, Expression);
            return true;
        }
    }
}
