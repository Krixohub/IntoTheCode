namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpFuncCall<TType> : ExpTyped<TType> // : ValueBase
    {
        public ExpFuncCall(FuncCall call)
        {
            Call = call;
        }

        public FuncCall Call;

        public override TType Compute(Variables runtime)
        {
            Variables local = Call.InitiateVariables(runtime);
            Call.Func.Run(local);

            ValueTyped<TType> value = local.GetVariable(ProgramBuilder.VariableReturn) as ValueTyped<TType>;
            return value.Value;
        }

    }
}
