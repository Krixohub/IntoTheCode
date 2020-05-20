using IntoTheCode;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpFloat : ExpTyped<float>
    {
        private float _value;

        public ExpFloat(CodeElement elem)
        {
            float.TryParse(elem.Value, out _value);
        }

        public override float Compute(Variables runtime)
        {
            return _value;
        }
    }
}
