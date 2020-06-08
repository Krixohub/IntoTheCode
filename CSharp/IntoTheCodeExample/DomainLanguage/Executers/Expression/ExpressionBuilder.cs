using IntoTheCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers.Expression
{
    public static class ExpressionBuilder
    {
        // Expression rules
        public const string WordExp = "exp";
        public const string WordPar = "par";
        public const string WordMul = "mul";
        public const string WordDiv = "div";
        public const string WordSum = "sum";
        public const string WordSub = "sub";
        public const string WordGt = "gt";
        public const string WordLt = "lt";
        public const string WordEq = "eq";
        public const string WordValue = "value"; // collapsed
        public const string WordString = "string";
        public const string WordBool = "bool";
        //public const string WordTrue = "true";
        //public const string WordFalse = "false";
        public const string WordReal = "float";
        public const string WordVar = "variable";
        public const string WordInt = "int";
        public const string WordFuncCall = "funcCall";

        public static ExpBase CreateExpression(CodeElement elem, Scope scope)
        {
            if (elem == null) throw new Exception("Missing expression element");

            // First non operator values
            switch (elem.Name)
            {
                case WordInt:
                    return new ExpInt(elem);
                case WordString:
                    return new ExpString(elem);
                case WordReal:
                    return new ExpFloat(elem);
                case WordBool:
                    return new ExpBool(elem);
                case WordExp:
                    return CreateExpression(elem.Codes().FirstOrDefault(), scope);
                case WordPar:
                    return CreateExpression(elem.Codes().FirstOrDefault(), scope);
                case "identifier":
                    return CreateExpVariable(elem, scope);
                case WordFuncCall:
                    return CreateExpFuncCall(elem, scope);
            }

            // Then binary operator values. All binary operators has two operants.
            CodeElement first = elem.Codes().FirstOrDefault();
            CodeElement next = elem.Codes().FirstOrDefault(c => c != first);
            ExpBase op1 = CreateExpression(first, scope);
            ExpBase op2 = CreateExpression(next, scope);

            // For multiplication, division, subtraction, greaterThan and lowerThan the operants must be numbers.
            if (elem.Name == WordMul || elem.Name == WordDiv || elem.Name == WordSub || elem.Name == WordGt || elem.Name == WordLt)
            {
                // check type
                if (!ExpBase.IsNumber(op1))
                    throw new Exception(string.Format("The left operant of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
                if (!ExpBase.IsNumber(op2))
                    throw new Exception(string.Format("The right operant of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
            }

            // For 'equals' the operants must both be numbers or both be strings.
            if (elem.Name == WordEq)
            {
                if (!(ExpBase.IsNumber(op1, op2) || ExpBase.IsString(op1, op2)))
                    throw new Exception(string.Format("Operants must both be numbers or both be strings: '{0}', {1}", elem.Name, elem.GetLineAndColumn()));
            }

            switch (elem.Name)
            {
                case WordGt: return new ExpGt(op1, op2);
                case WordLt: return new ExpLt(op1, op2);
                case WordEq: return new ExpEquals(op1, op2);
                case WordDiv: return new ExpDivide(op1, op2);
                case WordMul:
                    if (ExpBase.IsInt(op1, op2)) return new ExpMultiplyInt(op1, op2);
                    else return new ExpMultiplyFloat(op1, op2);
                case WordSub:
                    if (ExpBase.IsInt(op1, op2)) return new ExpMinusInt(op1, op2);
                    else return new ExpMinusFloat(op1, op2);
                case WordSum:
                    if (ExpBase.IsInt(op1, op2)) return new ExpSumInt(op1, op2);
                    else if (ExpBase.IsNumber(op1, op2)) return new ExpSumFloat(op1, op2);
                    else return new ExpSumString(op1, op2);
                default:
                    throw new Exception(string.Format("Unknown expression element: '{0}'", elem.Name));
            }
        }

        private static ExpBase CreateExpVariable(CodeElement elem, Scope scope)
        {
            string name = elem.Value;

            DefType theType = scope.ExistsVariable(name, elem);
            ExpBase ExpVar;
            switch (theType)
            {
                case DefType.Int: ExpVar = new ExpVariable<int>(name); break;
                case DefType.String: ExpVar = new ExpVariable<string>(name); break;
                case DefType.Float: ExpVar = new ExpVariable<float>(name); break;
                case DefType.Bool: ExpVar = new ExpVariable<bool>(name); break;
                default:
                    throw new Exception("Unknown variable type.");
            }

            return ExpVar;
        }

        public static FuncCall CreateFuncCall(CodeElement elem, Scope scope)
        {
            // funcCall    = identifier '(' [exp {',' exp}] ')';

            CodeElement idElem = elem.Codes("identifier").First();
            string name = idElem.Value;

            Function theFunc = scope.GetFunction(name);

            var parameters = new List<ExpBase>();
            int i = -1;
            foreach (CodeElement item in elem.Codes().Where(e => e != idElem))
            {
                var parm = CreateExpression(item, scope);
                if (++i == theFunc.Parameters.Count)
                    throw new Exception(string.Format("Too many parameters for function '{0}', {1}", name, item.GetLineAndColumn()));
                if (theFunc.Parameters[i].TheType == DefType.Int && !ExpBase.IsInt(parm))
                    throw new Exception(string.Format("The parameter for function '{0}' must be an integer, {1}", name, item.GetLineAndColumn()));
                if (theFunc.Parameters[i].TheType == DefType.Bool && !ExpBase.IsBool(parm))
                    throw new Exception(string.Format("The parameter for function '{0}' must be a boolean, {1}", name, item.GetLineAndColumn()));
                if (theFunc.Parameters[i].TheType == DefType.Bool && !ExpBase.IsNumber(parm))
                    throw new Exception(string.Format("The parameter for function '{0}' must be a number, {1}", name, item.GetLineAndColumn()));

                parameters.Add(parm);
            }
            if (i + 1 != theFunc.Parameters.Count)
                throw new Exception(string.Format("Too few parameters for function '{0}', {1}", name, elem.GetLineAndColumn()));

            var call = new FuncCall(theFunc, parameters);

            return call;
        }

        private static ExpBase CreateExpFuncCall(CodeElement elem, Scope scope)
        {
            // funcCall    = identifier '(' [exp {',' exp}] ')';
            FuncCall call = CreateFuncCall(elem, scope);

            ExpBase expFunc = null;
            switch (call.Func.FuncType)
            {
                case DefType.Int: expFunc = new ExpFuncCall<int>(call); break;
                case DefType.String: expFunc = new ExpFuncCall<string>(call); break;
                case DefType.Float: expFunc = new ExpFuncCall<float>(call); break;
                case DefType.Bool: expFunc = new ExpFuncCall<bool>(call); break;
                default:
                    throw new Exception("Unknown variable type.");
            }

            return expFunc;
        }
    }
}
