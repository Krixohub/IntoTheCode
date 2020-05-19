namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class CmdFuncCall : OperationBase
    {

        public FuncCall Call;

        public override bool Run(Variables runtime)
        {
            Variables local = Call.InitiateVariables(runtime);
            Call.Func.Run(local);
            return false;
        }
    }
}
