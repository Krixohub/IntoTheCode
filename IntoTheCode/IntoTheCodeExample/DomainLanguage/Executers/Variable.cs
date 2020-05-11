using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class Variable<TType> : VariableBase
    {
       
        public Variable(string name)
        {
            Name = name;
        }

        TType Value { get; set; }

        protected override DefType GetVariableType()
        {
            if (typeof(TType) == typeof(int)) return DefType.Int;
            if (typeof(TType) == typeof(float)) return DefType.Float;
            if (typeof(TType) == typeof(string)) return DefType.String;
            if (typeof(TType) == typeof(bool)) return DefType.Bool;
            throw new Exception("Unknown variable type.");
        }

    }
}
