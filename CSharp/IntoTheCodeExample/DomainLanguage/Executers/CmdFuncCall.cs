namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class CmdFuncCall : OperationBase
    {

        public FuncCall Call;

        public override bool Run(Variables runtime)
        {
            Variables localRuntime = Call.InitiateVariables(runtime);
            Call.Func.Run(localRuntime);
            return false;
        }
    }
}
