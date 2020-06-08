using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ValueBase
    {
        //public string Name;

        public abstract void SetValue(Variables runtime, ExpBase exp);

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

        public static ValueBase Compile(DefType theType, Variables runtime, ExpBase exp)
        {
            ValueBase variable;
            switch (theType)
            {
                case DefType.Int: variable = new ValueTyped<int>(); break;
                case DefType.String: variable = new ValueTyped<string>() { Value = string.Empty }; break;
                case DefType.Float: variable = new ValueTyped<float>() ; break;
                case DefType.Bool: variable = new ValueTyped<bool>() ; break;
                case DefType.Void:
                    throw new Exception("Can't create a variable of type void.");
                default:
                    throw new Exception("Unknown variable type.");
            }

            return variable;
        }

        public static ValueBase Create(DefType theType)
        {
            ValueBase variable;
            switch (theType)
            {
                case DefType.Int: variable = new ValueTyped<int>(); break;
                case DefType.String: variable = new ValueTyped<string>(); break;
                case DefType.Float: variable = new ValueTyped<float>(); break;
                case DefType.Bool: variable = new ValueTyped<bool>(); break;
                case DefType.Void:
                    throw new Exception("Can't create a variable of type void.");
                default:
                    throw new Exception("Unknown variable type.");
            }

            return variable;
        }

        public static ValueBase Create(DefType theType, Variables runtime, ExpBase exp)
        {
            ValueBase variable;
            switch (theType)
            {
                case DefType.Int: variable = new ValueTyped<int>() { Value = ((ExpTyped<int>)exp).Compute(runtime) }; break;
                case DefType.String: variable = new ValueTyped<string>() { Value = exp.RunAsString(runtime) }; break;
                case DefType.Float: variable = new ValueTyped<float>() { Value = exp.RunAsFloat(runtime) }; break;
                case DefType.Bool: variable = new ValueTyped<bool>() { Value = ((ExpTyped<bool>)exp).Compute(runtime) }; break;
                case DefType.Void:
                    throw new Exception("Can't create a variable of type void.");
                default:
                    throw new Exception("Unknown variable type.");
            }

            return variable;
        }
    }
}
