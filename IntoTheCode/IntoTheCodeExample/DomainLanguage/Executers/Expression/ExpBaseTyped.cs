using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public abstract class ExpBaseTyped<TResult> : ExpBase
    {
        public abstract TResult Calculate();

        protected override DefType GetResultType()
        {
            if (typeof(TResult) == typeof(int)) return DefType.Int;
            if (typeof(TResult) == typeof(float)) return DefType.Float;
            if (typeof(TResult) == typeof(string)) return DefType.String;
            if (typeof(TResult) == typeof(bool)) return DefType.Bool;
            throw new Exception("Unknown expression type.");
        }

        protected ExpBaseTyped<float> GetFloatOperant(ExpBase op)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return new ExpIntToFloat((ExpBaseTyped<int>)op);
                case DefType.Float: return (ExpBaseTyped<float>)op;
                default: throw new Exception(string.Format("Expression is not a numger: '{0}'", op.ExpressionType));
            }
        }

        protected ExpBaseTyped<string> GetStringOperant(ExpBase op)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return new ExpToString<int>((ExpBaseTyped<int>)op);
                case DefType.Float: return new ExpToString<float>((ExpBaseTyped<float>)op);
                case DefType.Bool: return new ExpToString<bool>((ExpBaseTyped<bool>)op);
                case DefType.String: return (ExpBaseTyped<string>)op;
                default: throw new Exception(string.Format("Unknown expression type: '{0}'", op.ExpressionType));
            }
        }
    }
}
