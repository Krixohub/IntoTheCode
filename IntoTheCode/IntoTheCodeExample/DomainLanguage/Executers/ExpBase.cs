namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ExpBase
    {
        protected abstract DefType GetResultType();

        public DefType ExpressionType { get { return GetResultType(); } }

        protected static bool IsInt(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.Int) return false;
            return true;
        }

        protected static bool IsNumber(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.Int && op.ExpressionType != DefType.Float) return false;
            return true;
        }


        protected static bool IsBool(params ExpBase[] operants)
        {
            foreach (ExpBase op in operants)
                if (op.ExpressionType != DefType.Bool) return false;
            return true;
        }
    }
}
