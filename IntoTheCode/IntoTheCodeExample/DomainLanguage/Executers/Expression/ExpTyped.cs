using System;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public abstract class ExpTyped<TResult> : ExpBase
    {
        public abstract TResult Run(Context runtime);

        protected override DefType GetResultType()
        {
            if (typeof(TResult) == typeof(int)) return DefType.Int;
            if (typeof(TResult) == typeof(float)) return DefType.Float;
            if (typeof(TResult) == typeof(string)) return DefType.String;
            if (typeof(TResult) == typeof(bool)) return DefType.Bool;
            throw new Exception("Unknown expression type.");
        }
    }
}
