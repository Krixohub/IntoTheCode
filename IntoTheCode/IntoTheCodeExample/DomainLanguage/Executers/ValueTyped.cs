using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class ValueTyped<TType> : ValueBase
    {
       
        public ValueTyped()
        {
            //Name = name;
        }

        public TType Value { get; set; }

        public override void SetValue(Variables runtime, ExpBase exp)
        {
            if (typeof(TType) == typeof(int)) (this as ValueTyped<int>).Value = exp.RunAsInt(runtime);
            if (typeof(TType) == typeof(float)) (this as ValueTyped<float>).Value = exp.RunAsFloat(runtime);
            if (typeof(TType) == typeof(string)) (this as ValueTyped<string>).Value = exp.RunAsString(runtime);
            if (typeof(TType) == typeof(bool)) (this as ValueTyped<bool>).Value = exp.RunAsBool(runtime);
            throw new Exception("Unknown variable type.");
        }
    }

    //public class VariableInt : VariableBase
    //{

    //    public VariableInt(string name)
    //    {
    //        Name = name;
    //    }

    //    int Value { get; set; }

    //    public override DefType ResultType
    //    {
    //        get
    //        {
    //            return DefType.Int;
    //        }
    //    }
    //    //protected override DefType GetVariableType()
    //    //{
    //    //    return DefType.Int;
    //    //}

    //    protected override void SetValue(Context runtime, ExpBase exp)
    //    {
    //        Value = exp.RunAsInt(runtime);
    //    }
    //}

    //public class VariableFloat : VariableBase
    //{

    //    public VariableFloat(string name)
    //    {
    //        Name = name;
    //    }

    //    float Value { get; set; }

    //    public override DefType ResultType
    //    {
    //        get
    //        {
    //            return DefType.Float;
    //        }
    //    }

    //    //protected override DefType GetVariableType()
    //    //{
    //    //    return DefType.Float;
    //    //}

    //    protected override void SetValue(Context runtime, ExpBase exp)
    //    {
    //        Value = exp.RunAsFloat(runtime);
    //    }
    //}

    //public class VariableString : VariableBase
    //{

    //    public VariableString(string name)
    //    {
    //        Name = name;
    //    }

    //    string Value { get; set; }

    //    public override DefType ResultType
    //    {
    //        get
    //        {
    //            return DefType.String;
    //            //if (this is Variable<bool>) return DefType.Bool;
    //            //throw new Exception("Unknown variable type.");
    //        }
    //    }

    //    //protected override DefType GetVariableType()
    //    //{
    //    //    return DefType.String;
    //    //}

    //    protected override void SetValue(Context runtime, ExpBase exp)
    //    {
    //        Value = exp.RunAsString(runtime);
    //    }
    //}

    //public class VariableBool : VariableBase
    //{

    //    public VariableBool(string name)
    //    {
    //        Name = name;
    //    }

    //    bool Value { get; set; }

    //    public override DefType ResultType
    //    {
    //        get
    //        {
    //            return DefType.Bool;
    //        }
    //    }
    //    //protected override DefType GetVariableType()
    //    //{
    //    //    return DefType.Bool;
    //    //}

    //    protected override void SetValue(Context runtime, ExpBase exp)
    //    {
    //        Value = exp.RunAsBool(runtime);
    //    }
    //}
}
