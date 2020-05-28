using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ExpBase
    {
        protected abstract DefType GetResultType();

        public DefType ExpressionType { get { return GetResultType(); } }

        #region Check expression type 

        public static bool IsInt(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.Int) return false;
            return true;
        }

        public static bool IsNumber(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.Int && op.ExpressionType != DefType.Float) return false;
            return true;
        }

        public static bool IsBool(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.Bool) return false;
            return true;
        }

        public static bool IsString(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.String) return false;
            return true;
        }

        #endregion Check expression type 

        #region run expression as given type 

        public Func<Variables, float> RunAsFloat
        {
            get
            {
                switch (ExpressionType)
                {
                    case DefType.Int: return v => ((ExpTyped<int>)this).Compute(v);
                    case DefType.Float: return ((ExpTyped<float>)this).Compute;
                    default: throw new Exception(string.Format("Expression is not a number: '{0}'", ExpressionType));
                }
            }
        }

        public Func<Variables, string> RunAsString
        {
            get
            {
                switch (ExpressionType)
                {
                    case DefType.Int: return v => ((ExpTyped<int>)this).Compute(v).ToString();
                    case DefType.Float: return v => ((ExpTyped<float>)this).Compute(v).ToString();
                    case DefType.Bool: return v => ((ExpTyped<bool>)this).Compute(v).ToString();
                    case DefType.String: return ((ExpTyped<string>)this).Compute;
                    default: throw new Exception(string.Format("Unknown expression type: '{0}'", ExpressionType));
                }
            }
        }

        #endregion run expression as given type 
    }

}
