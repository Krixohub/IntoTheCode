﻿using IntoTheCode;
using System;

namespace IntoTheCodeExample.Expression.Executers
{
    public abstract class ExpressionBase
    {
        public abstract float execute();

        public static ExpressionBase Factory(CodeElement elem)
        {
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
                    return Factory(elem.SubElements[0]);
                case "par":
                    return Factory(elem.SubElements[0]); // new Parentese(elem);
                default:
                    throw new Exception("Unknown expression element");
            }
        }
    }
}
