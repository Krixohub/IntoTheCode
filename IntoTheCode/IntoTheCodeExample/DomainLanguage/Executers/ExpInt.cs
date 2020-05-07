using IntoTheCode;
using IntoTheCode.Basic;
using IntoTheCode.Read.Words;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class ExpInt : ExpBase
    {
        private int _value;

        public ExpInt(CodeElement elem)
        {
            int.TryParse(elem.Value, out _value);
            ExpressionType = ExpType.Int;
        }

        public override ValueBase execute()
        {
            return ValueBase.Create(_value);
        }
    }
}
