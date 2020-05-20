using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpBool : ExpTyped<bool>
    {
        private bool _value;

        public ExpBool(bool val)
        {
            _value = val;
        }

        public override bool Compute(Variables runtime)
        {
            return _value;
        }
    }
}
