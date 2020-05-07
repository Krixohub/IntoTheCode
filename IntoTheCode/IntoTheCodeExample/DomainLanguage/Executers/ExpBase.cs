using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ExpTyped<T> : ExpBase
    {
        public abstract T execute2();
    }

    public abstract class ExpBase
    {
        public abstract ValueBase execute();

        public ExpType ExpressionType { get; protected set; }

        public static ExpBase Factory2(CodeElement elem)
        {
            if (elem == null) throw new Exception("Missing expression element");

            // First non operator values
            switch (elem.Name)
            {
                case "int":
                    return new ExpInt(elem);
                case "exp":
                    return Factory(elem.ChildNodes.OfType<CodeElement>().FirstOrDefault());
                case "par":
                    return Factory(elem.ChildNodes.OfType<CodeElement>().FirstOrDefault());
            }


            // Then binary operator values
            // Resolve type
            ExpBase _op1;
            ExpBase _op2;
            CodeElement first = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault();
            CodeElement next = elem.ChildNodes.OfType<CodeElement>().FirstOrDefault(c => c != first);
            _op1 = Factory2(first);
            _op2 = Factory2(next);


            switch (elem.Name)
            {
                case "mul":
                    return new ExpMultiply(elem);
                case "div":
                    return new ExpDivide(elem);
                case "sum":
                    return new ExpSum(elem);
                case "sub":
                    if (IsInt(_op1, _op2)) return new ExpMinusInt(elem, _op1, _op2);
                    else return new ExpMinusFloat(_op1, _op2);
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }

        private static bool IsInt(ExpBase op1, ExpBase op2)
        {
            return op1.ExpressionType == ExpType.Int && op2.ExpressionType == ExpType.Int;
        }

        public static ExpBase Factory(CodeElement elem)
        {
            if (elem == null) throw new Exception("Missing expression element");

            switch (elem.Name)
            {
                case "int":
                    return new ExpInt(elem);
                case "exp":
                    return Factory(elem.ChildNodes.OfType<CodeElement>().FirstOrDefault());
                case "par":
                    return Factory(elem.ChildNodes.OfType<CodeElement>().FirstOrDefault());
            
                case "mul":
                    return new ExpMultiply(elem);
                case "div":
                    return new ExpDivide(elem);
                case "sum":
                    return new ExpSum(elem);
                case "sub":
                    return new ExpMinus(elem);
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }

    }
}
