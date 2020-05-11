namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class VariableBase
    {
        public string Name;

        protected abstract DefType GetVariableType();

        public DefType ResultType { get { return GetVariableType(); } }
    }
}
