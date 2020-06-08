namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpVariable<TType> : ExpTyped<TType>
    {
        public ExpVariable(string name)
        {
            Name = name;
        }

        public string Name;

        public override TType Compute(Variables runtime)
        {
            ValueTyped<TType> value = runtime.GetVariable(Name) as ValueTyped<TType>;
            return value.Value;
        }

    }
}
