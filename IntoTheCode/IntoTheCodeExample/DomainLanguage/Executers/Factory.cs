using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public static class Factory
    {
        public static ExpBase Expression(CodeElement elem, Context compileContext)
        {
            if (elem == null) throw new Exception("Missing expression element");

            // First non operator values
            switch (elem.Name)
            {
                case "int":
                    return new ExpInt(elem);
                case "exp":
                    return Expression(elem.Codes().FirstOrDefault(), compileContext);
                case "par":
                    return Expression(elem.Codes().FirstOrDefault(), compileContext);
            }

            // Then binary operator values. All binary operators has two operants.
            CodeElement first = elem.Codes().FirstOrDefault();
            CodeElement next = elem.Codes().FirstOrDefault(c => c != first);
            ExpBase op1 = Expression(first, compileContext);
            ExpBase op2 = Expression(next, compileContext);

            // For multiplication, division, subtraction, greaterThan and lowerThan the operants must be numbers.
            if (elem.Name == "mul" || elem.Name == "div" || elem.Name == "sub" || elem.Name == "gt" || elem.Name == "lt") 
            { 
                // check type
                if (!IsNumber(op1))
                    throw new Exception(string.Format( "The left operator of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
                if (!IsNumber(op2))
                    throw new Exception(string.Format("The right operator of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
            }

            switch (elem.Name)
            {
                case "gt": return new ExpGt(op1, op2);
                case "lt": return new ExpLt(op1, op2);
                case "eq": return new ExpEquals(op1, op2);
                case "div": return new ExpDivide(op1, op2);
                case "mul":
                    if (IsInt(op1, op2)) return new ExpMultiplyInt(op1, op2);
                    else return new ExpMultiplyFloat(op1, op2);
                case "sub":
                    if (IsInt(op1, op2)) return new ExpMinusInt(op1, op2);
                    else return new ExpMinusFloat(op1, op2);
                case "sum":
                    if (IsInt(op1, op2)) return new ExpSumInt(op1, op2);
                    else if (IsNumber(op1, op2)) return new ExpSumFloat(op1, op2);
                    else return new ExpSumString(op1, op2);
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }

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

        public static int RunAsInt(this ExpBase op, Context runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return ((ExpTyped<int>)op).Run(runtime);
                default: throw new Exception(string.Format("Expression is not an integer: '{0}'", op.ExpressionType));
            }
        }

        public static float RunAsFloat(this ExpBase op, Context runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return ((ExpTyped<int>)op).Run(runtime);
                case DefType.Float: return ((ExpTyped<float>)op).Run(runtime);
                default: throw new Exception(string.Format("Expression is not a number: '{0}'", op.ExpressionType));
            }
        }

        public static string RunAsString(this ExpBase op, Context runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return ((ExpTyped<int>)op).Run(runtime).ToString();
                case DefType.Float: return ((ExpTyped<float>)op).Run(runtime).ToString();
                case DefType.Bool: return ((ExpTyped<bool>)op).Run(runtime).ToString();
                case DefType.String: return ((ExpTyped<string>)op).Run(runtime);
                default: throw new Exception(string.Format("Unknown expression type: '{0}'", op.ExpressionType));
            }
        }

        public static bool RunAsBool(this ExpBase op, Context runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Bool: return ((ExpTyped<bool>)op).Run(runtime);
                default: throw new Exception(string.Format("Expression is not an integer: '{0}'", op.ExpressionType));
            }
        }

        public static void RunAsVoid(this ExpBase op, Context runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: ((ExpTyped<int>)op).Run(runtime); break;
                case DefType.Float: ((ExpTyped<float>)op).Run(runtime); break;
                case DefType.Bool: ((ExpTyped<bool>)op).Run(runtime); break;
                case DefType.String: ((ExpTyped<string>)op).Run(runtime); break;
                default: throw new Exception(string.Format("Unknown expression type: '{0}'", op.ExpressionType));
            }
        }


    }
}
