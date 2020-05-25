using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public static class ProgramCompiler
    {
        // program rules
        public const string WordScope = "scope";
        public const string WordFunctionDef = "functionDef";
        public const string WordVariableDef = "variableDef";
        public const string WordDeclare = "declare";
        public const string WordFuncCall = "funcCall";
        public const string WordBody = "body";
        public const string WordAssign = "assign";
        public const string WordReturn = "return";
        public const string WordWhile = "while";
        public const string WordIf = "if";
        public const string WordTypeInt = "defInt";
        public const string WordTypeStr = "defString";
        public const string WordTypeReal = "defReal";
        public const string WordTypeBool = "defBool";
        public const string WordTypeVoid = "defVoid";
        public const string WordIdentifier = "identifier";

        public const string VariableReturn = "returnValue";
        public static Program CreateProgram(TextDocument doc, Dictionary<string, Function> functions, Dictionary<string, ValueBase> parameters)
        {
            // todo add functions to rootScope;
            var prog = new Program();

            Scope externalScope = functions == null ? null : new Scope(null, null, functions);
            Variables variables = new Variables(null, parameters); //, externalScope);
            prog.RootScope = CreateScope(doc.Codes(WordScope).First(), variables, externalScope, DefType.Void, out bool alwaysReturn);

            return prog;
        }

        public static Scope CreateScope(CodeElement elem, Variables innerVariables, Scope parentScope, DefType resultType, out bool alwaysReturnValue) //, bool mustReturn
        {
            //bool mustReturn = false;
            alwaysReturnValue = false;

            var scope = new Scope(parentScope, innerVariables); //, resultType, parentScope, mustReturn

            //Variables variables = new Variables(localVariables, null);

            foreach (CodeElement item in elem.Codes(WordFunctionDef))
                CreateFunctionDef(item, scope);

            List<OperationBase> operations = new List<OperationBase>();
            foreach (CodeElement item in elem.Codes(c => c.Name != WordFunctionDef))
                if (alwaysReturnValue)
                    throw new Exception(string.Format("The statement can not be reached, {0}, {1}", item.Name, item.GetLineAndColumn()));
                else
                    operations.Add(CreateVariableOrOperation(item, scope, resultType, out alwaysReturnValue));

            scope.Operations = operations;

            return scope;
        }

        public static Function CreateFunctionDef(CodeElement elem, Scope scope)
        {
            // functionDef = typeAndId '(' [typeAndId {',' typeAndId}] ')' '{' scope '}';

            CodeElement defElem = elem.Codes(WordDeclare).First();
            var def = CreateDeclare(defElem, false);

            if (scope.Functions.ContainsKey(def.TheName))
                // todo check type and parameters
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", def.TheName, elem.GetLineAndColumn()));


            var parms = new List<Declare>();
            Variables innerVariables = new Variables(null, null);

            if (def.TheType != DefType.Void)
                innerVariables.BuildVariable(def.TheType, "returnValue", elem);

            foreach (CodeElement item in elem.Codes(WordDeclare).Where(e => e != defElem))
            {
                var parm = CreateDeclare(item, true);
                parms.Add(parm);
                innerVariables.BuildVariable(parm.TheType, parm.TheName, item);
            }

            Function funcDef = new Function() { FuncType = def.TheType, Name = def.TheName, Parameters = parms };

            scope.Functions.Add(def.TheName, funcDef);

            CodeElement scopeElem = elem.Codes(WordScope).First();


            bool alwaysReturns;
            funcDef.FunctionScope = CreateScope(scopeElem, innerVariables, scope, def.TheType, out alwaysReturns); //, scope.FunctionScope

            if (def.TheType != DefType.Void && !alwaysReturns)
                throw new Exception(string.Format("The function '{0}' must return a value, {1}", def.TheName, elem.GetLineAndColumn()));

            return funcDef;
        }

        public static OperationBase CreateVariableOrOperation(CodeElement elem, Scope scope, DefType resultType, out bool alwaysReturnValue)
        {
            alwaysReturnValue = false;
            OperationBase stm;

            switch (elem.Name)
            {
                case WordVariableDef:
                    stm = CreateVariableDef(elem, scope);
                    break;
                case WordAssign:
                    stm = CreateAssign(elem, scope);
                    break;
                case WordIf:
                    stm = CreateIf(elem, scope, resultType, out alwaysReturnValue);
                    // todo check always returns
                    break;
                case WordWhile:
                    stm = CreateWhile(elem, scope, resultType);
                    // todo check always returns
                    break;
                case WordFuncCall:
                    stm = CreateOperationFuncCall(elem, scope);
                    break;
                case WordReturn:
                    stm = CreateReturn(elem, scope, resultType);
                    alwaysReturnValue = true;
                    break;
                default:
                    throw new Exception(string.Format("The operation type '{0}' is not allowed, {1}", elem.Name, elem.GetLineAndColumn()));
            }

            return stm;
        }


        public static VariableDef CreateVariableDef(CodeElement elem, Scope scope)
        {
            // variableDef = typeAndId '=' exp ';';
            var def = CreateDeclare(elem.Codes(WordDeclare).First(), true);

            ExpBase exp = null;
            if (elem.Codes().Count() == 2)
            {
                CodeElement expCode = elem.Codes().Last();
                exp = ProgramCompiler.Expression(expCode, scope);
                if (exp.ExpressionType != def.TheType)
                {
                    if (def.TheType == DefType.Bool)
                        throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                    if (def.TheType == DefType.Int)
                        throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                    if (def.TheType == DefType.Float && exp.ExpressionType != DefType.Int)
                        throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
                }
            }
            if (scope.ExistsFunction(def.TheName))
                throw new Exception(string.Format("A Function called '{0}', {1}, is declared", def.TheName, elem.GetLineAndColumn()));

            var varDef = new VariableDef() { Name = def.TheName, VariableType = def.TheType, Expression = exp };

            scope.Vars.BuildVariable(def.TheType, def.TheName, elem);
            return varDef;
        }

        public static Assign CreateAssign(CodeElement elem, Scope scope)
        {
            // assign      = identifier '=' exp ';';
            var cmd = new Assign();
            CodeElement idElem = elem.Codes(WordIdentifier).First();
            cmd.VariableName = idElem.Value;
            cmd.VariableType = scope.ExistsVariable(cmd.VariableName, idElem);

            CodeElement expCode = elem.Codes().Last();
            cmd.Expression = Expression(expCode, scope);
            if (cmd.Expression.ExpressionType != cmd.VariableType)
            {
                if (cmd.VariableType == DefType.Bool)
                    throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                if (cmd.VariableType == DefType.Int)
                    throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                if (cmd.VariableType == DefType.Float && cmd.Expression.ExpressionType != DefType.Int)
                    throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
            }

            return cmd;
        }

        public static OperationBase CreateBody(CodeElement elem, Scope scope, DefType resultType, out bool alwaysReturnValue)
        {
            CodeElement innerCode = elem.Codes().First();
            OperationBase cmd;
            if (innerCode.Name == WordScope)
                cmd = CreateScope(innerCode, null, scope, resultType, out alwaysReturnValue);
            else
                cmd = CreateVariableOrOperation(innerCode, scope, resultType, out alwaysReturnValue);

            return cmd;
        }

        public static If CreateIf(CodeElement elem, Scope scope, DefType resultType, out bool alwaysReturnValue)
        {
            // if          = 'if' '(' exp ')' body ['else' body];
            ExpBase expbase = Expression(elem.Codes().First(), scope);
            if (expbase.ExpressionType != DefType.Bool)
                throw new Exception(string.Format("The expression type must be a boolean, {0}", elem.GetLineAndColumn()));

            var cmd = new If();
            cmd.Expression = expbase as ExpTyped<bool>;
            bool trueReturn;
            cmd.TrueStatement = CreateBody(elem.Codes(WordBody).First(), scope, resultType, out trueReturn);
            bool elseReturn;
            cmd.ElseStatement = CreateBody(elem.Codes(WordBody).Last(), scope, resultType, out elseReturn);
            alwaysReturnValue = trueReturn && elseReturn;
            return cmd;
        }

        public static Return CreateReturn(CodeElement elem, Scope scope, DefType resultType)
        {
            // return      = 'return' [exp] ';';

            var cmd = new Return();

            CodeElement expCode = elem.Codes().FirstOrDefault();
            if (expCode != null)
            {
                cmd.Expression = ProgramCompiler.Expression(expCode, scope);

                // Check resulttype of operation
                if (resultType != cmd.Expression.ExpressionType)
                {
                    if (resultType == DefType.Bool)
                        throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                    if (resultType == DefType.Int)
                        throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                    if (resultType == DefType.Float && cmd.Expression.ExpressionType != DefType.Int)
                        throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
                }
            }
            else
                if (resultType != DefType.Void)
                throw new Exception(string.Format("This function must return a value. {0}", elem.GetLineAndColumn()));

            return cmd;
        }

        public static While CreateWhile(CodeElement elem, Scope scope, DefType resultType)
        {
            // loop        = 'while' '(' exp ')' body;
            ExpBase expbase = Expression(elem.Codes().First(), scope);
            if (expbase.ExpressionType != DefType.Bool)
                throw new Exception(string.Format("The expression type must be a boolean, {0}", elem.GetLineAndColumn()));

            var cmd = new While();
            cmd.Expression = expbase as ExpTyped<bool>;
            cmd.Body = CreateBody(elem.Codes(WordBody).First(), scope, resultType, out bool ret);
            return cmd;
        }

        public static Declare CreateDeclare(CodeElement elem, bool isVariable)
        {
            CodeElement defType = elem.Codes().FirstOrDefault();
            CodeElement identifier = elem.Codes("identifier").FirstOrDefault();

            Declare dec = new Declare() { TheName = identifier.Value };
            switch (defType.Name)
            {
                case WordTypeInt: dec.TheType = DefType.Int; break;
                case WordTypeStr: dec.TheType = DefType.String; break;
                case WordTypeReal: dec.TheType = DefType.Float; break;
                case WordTypeBool: dec.TheType = DefType.Bool; break;
                case WordTypeVoid:
                    if (isVariable) throw new Exception(string.Format("Only functions can be 'void'. {0}", defType.GetLineAndColumn()));
                    dec.TheType = DefType.Void;
                    break;
                default: throw new Exception(string.Format("The type '{0}' is not defined. {1}", defType.Value, defType.GetLineAndColumn()));
            }

            return dec;
        }






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


        public static ExpBase Expression(CodeElement elem, Scope scope)
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
                    return Expression(elem.Codes().FirstOrDefault(), scope);
                case WordPar:
                    return Expression(elem.Codes().FirstOrDefault(), scope);
                case "identifier":
                    return CreateExpVariable(elem, scope);
                //case WordVar:
                //    return CreateExpVariable(elem, scope);
                case WordFuncCall:
                    return CreateExpFuncCall(elem, scope);
            }

            // Then binary operator values. All binary operators has two operants.
            CodeElement first = elem.Codes().FirstOrDefault();
            CodeElement next = elem.Codes().FirstOrDefault(c => c != first);
            ExpBase op1 = Expression(first, scope);
            ExpBase op2 = Expression(next, scope);

            // For multiplication, division, subtraction, greaterThan and lowerThan the operants must be numbers.
            if (elem.Name == WordMul || elem.Name == WordDiv || elem.Name == WordSub || elem.Name == WordGt || elem.Name == WordLt)
            {
                // check type
                if (!IsNumber(op1))
                    throw new Exception(string.Format("The left operator of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
                if (!IsNumber(op2))
                    throw new Exception(string.Format("The right operator of '{0}', {1}, is not a number", elem.Name, elem.GetLineAndColumn()));
            }

            switch (elem.Name)
            {
                case WordGt: return new ExpGt(op1, op2);
                case WordLt: return new ExpLt(op1, op2);
                case WordEq: return new ExpEquals(op1, op2);
                case WordDiv: return new ExpDivide(op1, op2);
                case WordMul:
                    if (IsInt(op1, op2)) return new ExpMultiplyInt(op1, op2);
                    else return new ExpMultiplyFloat(op1, op2);
                case WordSub:
                    if (IsInt(op1, op2)) return new ExpMinusInt(op1, op2);
                    else return new ExpMinusFloat(op1, op2);
                case WordSum:
                    if (IsInt(op1, op2)) return new ExpSumInt(op1, op2);
                    else if (IsNumber(op1, op2)) return new ExpSumFloat(op1, op2);
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

        private static FuncCall CreateFuncCall(CodeElement elem, Scope scope)
        {
            // funcCall    = identifier '(' [exp {',' exp}] ')';

            CodeElement idElem = elem.Codes("identifier").First();
            string name = idElem.Value;

            Function theFunc = scope.GetFunction(name);

            var parameters = new List<ExpBase>();
            int i = -1;
            foreach (CodeElement item in elem.Codes().Where(e => e != idElem))
            {
                var parm = Expression(item, scope);
                if (++i == theFunc.Parameters.Count)
                    throw new Exception(string.Format("Too many parameters for function '{0}', {1}", name, item.GetLineAndColumn()));
                if (theFunc.Parameters[i].TheType == DefType.Int && !IsInt(parm))
                    throw new Exception(string.Format("The parameter for function '{0}' must be an integer, {1}", name, item.GetLineAndColumn()));
                if (theFunc.Parameters[i].TheType == DefType.Bool && !IsBool(parm))
                    throw new Exception(string.Format("The parameter for function '{0}' must be a boolean, {1}", name, item.GetLineAndColumn()));
                if (theFunc.Parameters[i].TheType == DefType.Bool && !IsNumber(parm))
                    throw new Exception(string.Format("The parameter for function '{0}' must be a number, {1}", name, item.GetLineAndColumn()));

                parameters.Add(parm);
            }
            if (i + 1 != theFunc.Parameters.Count)
                throw new Exception(string.Format("Too few parameters for function '{0}', {1}", name, elem.GetLineAndColumn()));

            var call = new FuncCall(theFunc, parameters);

            return call;
        }

        public static CmdFuncCall CreateOperationFuncCall(CodeElement elem, Scope scope)
        {
            FuncCall call = CreateFuncCall(elem, scope);
            return new CmdFuncCall { Call = call };
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

        public static int RunAsInt(this ExpBase op, Variables runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return ((ExpTyped<int>)op).Compute(runtime);
                default: throw new Exception(string.Format("Expression is not an integer: '{0}'", op.ExpressionType));
            }
        }

        public static float RunAsFloat(this ExpBase op, Variables runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return ((ExpTyped<int>)op).Compute(runtime);
                case DefType.Float: return ((ExpTyped<float>)op).Compute(runtime);
                default: throw new Exception(string.Format("Expression is not a number: '{0}'", op.ExpressionType));
            }
        }

        public static string RunAsString(this ExpBase op, Variables runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: return ((ExpTyped<int>)op).Compute(runtime).ToString();
                case DefType.Float: return ((ExpTyped<float>)op).Compute(runtime).ToString();
                case DefType.Bool: return ((ExpTyped<bool>)op).Compute(runtime).ToString();
                case DefType.String: return ((ExpTyped<string>)op).Compute(runtime);
                default: throw new Exception(string.Format("Unknown expression type: '{0}'", op.ExpressionType));
            }
        }

        public static bool RunAsBool(this ExpBase op, Variables runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Bool: return ((ExpTyped<bool>)op).Compute(runtime);
                default: throw new Exception(string.Format("Expression is not an integer: '{0}'", op.ExpressionType));
            }
        }

        public static void RunAsVoid(this ExpBase op, Variables runtime)
        {
            switch (op.ExpressionType)
            {
                case DefType.Int: ((ExpTyped<int>)op).Compute(runtime); break;
                case DefType.Float: ((ExpTyped<float>)op).Compute(runtime); break;
                case DefType.Bool: ((ExpTyped<bool>)op).Compute(runtime); break;
                case DefType.String: ((ExpTyped<string>)op).Compute(runtime); break;
                default: throw new Exception(string.Format("Unknown expression type: '{0}'", op.ExpressionType));
            }
        }


    }
}
