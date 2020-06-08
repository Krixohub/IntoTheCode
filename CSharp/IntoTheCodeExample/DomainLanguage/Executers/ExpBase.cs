using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ExpBase
    {
        //protected abstract DefType GetResultType();

        //public DefType ExpressionType { get { return GetResultType(); } }

        #region Check expression type 

        public static bool IsInt(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (!(op is ExpTyped<int>)) return false;
            return true;
        }

        public static bool IsNumber(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (!(op is ExpTyped<int>) && !(op is ExpTyped<float>)) return false;
            return true;
        }

        public static bool IsBool(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (!(op is ExpTyped<bool>)) return false;
            return true;
        }

        public static bool IsString(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (!(op is ExpTyped<string>)) return false;
            return true;
        }

        #endregion Check expression type 

        #region run expression as given type 

        public Func<Variables, float> RunAsFloat
        {
            get
            {
                if (this is ExpTyped<int>) return v => ((ExpTyped<int>)this).Compute(v);
                if (this is ExpTyped<float>) return v => ((ExpTyped<float>)this).Compute(v);
                throw new Exception(string.Format("Expression is not a float"));
            }
        }

        public Func<Variables, string> RunAsString
        {
            get
            {
                if (this is ExpTyped<int>) return v => ((ExpTyped<int>)this).Compute(v).ToString();
                if (this is ExpTyped<float>) return v => ((ExpTyped<float>)this).Compute(v).ToString();
                if (this is ExpTyped<bool>) return v => ((ExpTyped<bool>)this).Compute(v).ToString();
                if (this is ExpTyped<string>) return v => ((ExpTyped<string>)this).Compute(v);
                throw new Exception(string.Format("Unknown expression type: '{0}'", GetType().Name));
            }
        }

        #endregion run expression as given type 
    }

}
