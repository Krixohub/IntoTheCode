using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.Expression.Executers
{
    public static class ExpressionBuilder
    {
        public static ExpressionBase BuildExp(CodeElement elem)
        {
            if (elem == null) throw new Exception("Missing expression element");

            switch (elem.Name)
            {
                case "power":
                    return new Power(elem);
                case "mul":
                    return new Multiply(elem);
                case "div":
                    return new Divide(elem);
                case "sum":
                    return new Sum(elem);
                case "sub":
                    return new Minus(elem);
                case "number":
                case "int":
                    return new Number(elem);
                case "exp":
                    return BuildExp(elem.Codes().FirstOrDefault());
                case "par":
                    return BuildExp(elem.Codes().FirstOrDefault());
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }
    }
}
