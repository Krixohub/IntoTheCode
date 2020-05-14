using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ValueBase
    {
        //public string Name;

        public abstract void SetValue(Context runtime, ExpBase exp);

        public virtual DefType ResultType
        {
            get
            {
                if (this is ValueTyped<int>) return DefType.Int;
                if (this is ValueTyped<float>) return DefType.Float;
                if (this is ValueTyped<string>) return DefType.String;
                if (this is ValueTyped<bool>) return DefType.Bool;
                throw new Exception("Unknown variable type.");
            }
        }

        public static ValueBase Create(DefType theType, Context runtime, ExpBase exp)
        {
            ValueBase variable;
            switch (theType)
            {
                case DefType.Int: variable = new ValueTyped<int>() { Value = exp.RunAsInt(runtime) }; break;
                case DefType.String: variable = new ValueTyped<string>() { Value = exp.RunAsString(runtime) }; break;
                case DefType.Float: variable = new ValueTyped<float>() { Value = exp.RunAsFloat(runtime) }; break;
                case DefType.Bool: variable = new ValueTyped<bool>() { Value = exp.RunAsBool(runtime) }; break;
                case DefType.Void:
                    throw new Exception("Can't create a variable of type void.");
                default:
                    throw new Exception("Unknown variable type.");
            }

            return variable;
        }
    }
}
