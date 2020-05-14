//using System;

//namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
//{
//    public abstract class ValueBase
//    {
//        //public static ValueInt Create(int val) { return new ValueInt(val); }
//        public static ValueFloat Create(float val) { return new ValueFloat(val); }
//        public static ValueString Create(string val) { return new ValueString(val); }
//        public static ValueBool Create(bool val) { return new ValueBool(val); }
//        public static ValueVoid Create() { return new ValueVoid(); }

//     }

//    public static class ValueExtension
//    {
//        public static float ToFloat(this ValueBase val)
//        {
//            if (val is ValueInt) return ((ValueInt)val).Value;
//            if (val is ValueFloat) return ((ValueFloat)val).Value;

//            throw new Exception("The value must be a number");
//        }
//    }

//    public class ValueString : ValueBase
//    {
//        public ValueString(string val) { Value = val; }

//        public string Value { get; protected set; }

//    }
//    public class ValueInt : ValueBase
//    {
//        public ValueInt(int val) { Value = val; }

//        public int Value { get; protected set; }

//    }
//    public class ValueFloat : ValueBase
//    {
//        public ValueFloat(float val) { Value = val; }

//        public float Value { get; protected set; }

//    }
//    public class ValueVoid : ValueBase
//    {

//    }

//    public class ValueBool : ValueBase
//    {
//        public ValueBool(bool val) { Value = val; }

//        public bool Value { get; protected set; }

//    }
//}
