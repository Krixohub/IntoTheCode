using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public abstract class ExpBase
    {
        protected abstract DefType GetResultType();

        public DefType ExpressionType { get { return GetResultType(); } }

        public static ExpBase Factory2(CodeElement elem)
        {
            if (elem == null) throw new Exception("Missing expression element");

            // First non operator values
            switch (elem.Name)
            {
                case "int":
                    return new ExpInt(elem);
                case "exp":
                    return Factory2(elem.Codes().FirstOrDefault());
                case "par":
                    return Factory2(elem.Codes().FirstOrDefault());
            }


            // Then binary operator values. All binary operators has two operants.
            CodeElement first = elem.Codes().FirstOrDefault();
            CodeElement next = elem.Codes().FirstOrDefault(c => c != first);
            ExpBase op1 = Factory2(first);
            ExpBase op2 = Factory2(next);

            // For multiplication, division, subtraction, greaterThan and lowerThan the operants must be numbers.
            if (elem.Name == "mul" || elem.Name == "div" || elem.Name == "sub" || elem.Name == "gt" || elem.Name == "lt") 
                { 
                // check type
                if (!IsNumber(op1))
                    throw new Exception(string.Format( "The left operator of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
                if (op2 == null)
                    throw new Exception(string.Format("The right operator of '{0}', {1}, not a number", elem.Name, elem.GetLineAndColumn()));
            }

            switch (elem.Name)
            {
                case "gt":
                    if (IsInt(op1, op2)) return new ExpGtInt(op1, op2);
                    else return new ExpGtFloat(op1, op2);
                case "lt":
                    if (IsInt(op1, op2)) return new ExpLtInt(op1, op2);
                    else return new ExpLtFloat(op1, op2);
                case "mul":
                    if (IsInt(op1, op2)) return new ExpMultiplyInt(op1, op2);
                    else return new ExpMultiplyFloat(op1, op2);
                case "div":
                    if (IsInt(op1, op2)) return new ExpDivideInt(op1, op2);
                    else return new ExpDivideFloat(op1, op2);
                case "sub":
                    if (IsInt(op1, op2)) return new ExpMinusInt(op1, op2);
                    else return new ExpMinusFloat(op1, op2);
                case "sum":
                    if (IsInt(op1, op2)) return new ExpSumInt(op1, op2);
                    else if (IsNumber(op1, op2)) return new ExpSumFloat(op1, op2);
                    else return new ExpSumString(op1, op2);
                case "equals":
                    if (IsInt(op1, op2)) return new ExpEqualsInt(op1, op2);
                    else if (IsNumber(op1, op2)) return new ExpEqualsFloat(op1, op2);
                    else return new ExpEqualsString(op1, op2);
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }

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
