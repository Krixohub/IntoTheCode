using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpString : ExpTyped<string>
    {
        private string _value;

        public ExpString(CodeElement elem)
        {
            _value = elem.Value;
        }

        public override string Compute(Variables runtime)
        {
            return _value;
        }
    }
}
