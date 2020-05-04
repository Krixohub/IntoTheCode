using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.Expression.Executers
{
    public abstract class ExpressionBase
    {
        public abstract float execute();

        public static ExpressionBase Factory(CodeElement elem)
        {
            if (elem == null) throw new Exception("Missing expression element");

            switch (elem.Name)
            {
                case "mul":
                    return new Multiply(elem);
                case "div":
                    return new Divide(elem);
                case "sum":
                    return new Sum(elem);
                case "sub":
                    return new Minus(elem);
                case "number":
                    return new Number(elem);
                case "exp":
                    return Factory(elem.ChildNodes.OfType<CodeElement>().FirstOrDefault());
                case "par":
                    return Factory(elem.ChildNodes.OfType<CodeElement>().FirstOrDefault());
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }
    }
}
