using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public class ExpVariable<TType> : ExpTyped<TType> // : ValueBase
    {
        public ExpVariable(string name)
        {
            Name = name;
        }

        public string Name;

        public override TType Compute(Variables runtime)
        {
            ValueTyped<TType> value = runtime.Vars[Name] as ValueTyped<TType>;
            return value.Value;
        }

    }
}
