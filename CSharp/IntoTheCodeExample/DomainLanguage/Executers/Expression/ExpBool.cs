using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpBool : ExpTyped<bool>
    {
        private bool _value;

        public ExpBool(CodeElement elem)
        {
            bool.TryParse(elem.Value, out _value);
        }

        public override bool Compute(Variables runtime)
        {
            return _value;
        }
    }
}
