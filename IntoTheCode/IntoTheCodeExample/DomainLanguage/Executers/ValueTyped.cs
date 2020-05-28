using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class ValueTyped<TType> : ValueBase
    {
        public TType Value { get; set; }

        public override void SetValue(Variables runtime, ExpBase exp)
        {
            if (typeof(TType) == typeof(int)) (this as ValueTyped<int>).Value = ((ExpTyped<int>)exp).Compute(runtime);
            else if (typeof(TType) == typeof(float)) (this as ValueTyped<float>).Value = exp.RunAsFloat(runtime);
            else if (typeof(TType) == typeof(string)) (this as ValueTyped<string>).Value = exp.RunAsString(runtime);
            else if (typeof(TType) == typeof(bool)) (this as ValueTyped<bool>).Value = ((ExpTyped<bool>)exp).Compute(runtime);
            else throw new Exception("Unknown variable type.");
        }
    }
}
