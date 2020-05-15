using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public static class Compiler
    {
        // program rules
        public const string WordLocalScope = "localScope";
        public const string WordFunctionDef = "functionDef";
        public const string WordVariableDef = "variableDef";
        public const string WordDeclare = "declare";
        public const string WordFunc = "func";
        public const string WordBody = "body";
        public const string WordAssign = "assign";
        public const string WordReturn = "return";
        public const string WordLoop = "loop";
        public const string WordIf = "if";
        public const string WordTypeInt = "defInt";
        public const string WordTypeStr = "defString";
        public const string WordTypeReal = "defReal";
        public const string WordTypeBool = "defBool";
        public const string WordTypeVoid = "defVoid";
        public const string WordIdentifier = "identifier";

        public static Program CreateProgram(TextDocument doc, Dictionary<string, Function> functions, Dictionary<string, ValueBase> parameters)
        {
            // todo add functions to rootScope;
            var prog = new Program();


            LocalScope externalScope = functions == null ? null : new LocalScope(null, functions);
            Context externalContext = new Context(null, parameters, externalScope);
            prog.RootScope = CreateLocalScope(doc.Codes(WordLocalScope).First(), externalContext, DefType.Void, out bool alwaysReturn);

            return prog;
        }


        public static LocalScope CreateLocalScope(CodeElement elem, Context parentContextCompiletime, DefType resultType, out bool alwaysReturnValue) //, bool mustReturn, LocalScope parentScope
        {
            //bool mustReturn = false;
            alwaysReturnValue = false;

            var scope = new LocalScope(parentContextCompiletime.FunctionScope); //, resultType, parentScope, mustReturn
            Context compileContext = new Context(parentContextCompiletime, null, scope);

            foreach (CodeElement item in elem.Codes(WordFunctionDef))
            {
                Function func = CreateFunctionDef(item, compileContext);
                scope.Functions.Add(func.Name, func);
            }

            List<ProgramBase> commands = new List<ProgramBase>();
            foreach (CodeElement item in elem.Codes(c => c.Name != WordFunctionDef))
            {
                if (alwaysReturnValue)
                    throw new Exception(string.Format("The statement can not be reached, {0}, {1}", item.Name, item.GetLineAndColumn()));

                commands.Add(CreateVariableOrCommand(item, compileContext, resultType, out alwaysReturnValue));
            }

            scope.Commands = commands;

            return scope;
        }

        public static Function CreateFunctionDef(CodeElement elem, Context compileContext)
        {
            // functionDef = typeAndId '(' [typeAndId {',' typeAndId}] ')' '{' localScope '}';

            CodeElement defElem = elem.Codes(WordDeclare).First();
            var def = CreateDeclare(defElem, false);

            if (compileContext.FunctionScope.Functions.ContainsKey(def.TheName))
                // todo check type and parameters
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", def.TheName, elem.GetLineAndColumn()));


            var parms = new List<Declare>();
            Context bodyContext = new Context(compileContext, null, null);

            if (def.TheType != DefType.Void)
                bodyContext.BuildVariable(def.TheType, "returnValue", elem);

            foreach (CodeElement item in elem.Codes(WordDeclare).Where(e => e != defElem))
            {
                var parm = CreateDeclare(item, true);
                parms.Add(parm);
                bodyContext.BuildVariable(parm.TheType, parm.TheName, item);
            }

            Function funcDef = new Function() { FuncType = def.TheType, Name = def.TheName, Parameters = parms };

            compileContext.FunctionScope.Functions.Add(def.TheName, funcDef);

            CodeElement scopeElem = elem.Codes(WordLocalScope).First();


            bool alwaysReturns;
            funcDef.FunctionScope = CreateLocalScope(scopeElem, bodyContext, def.TheType, out alwaysReturns); //, compileContext.FunctionScope

            if (def.TheType != DefType.Void && !alwaysReturns)
                throw new Exception(string.Format("The function '{0}' must return a value, {1}", def.TheName, elem.GetLineAndColumn()));

            return funcDef;
        }

        public static ProgramBase CreateVariableOrCommand(CodeElement elem, Context compileContext, DefType resultType, out bool alwaysReturnValue)
        {
            alwaysReturnValue = false;
            ProgramBase stm;

            switch (elem.Name)
            {
                case WordVariableDef:
                    stm = CreateVariableDef(elem, compileContext);
                    break;
                case WordAssign:
                    stm = CreateAssign(elem, compileContext);
                    break;
                case WordIf:
                    stm = CreateIf(elem, compileContext, resultType, out alwaysReturnValue);
                    // todo check always returns
                    break;
                case WordLoop:
                    stm = CreateWhile(elem, compileContext, resultType);
                    // todo check always returns
                    break;
                case WordFunc:
                    stm = CreateFuncCall(elem, compileContext);
                    break;
                case WordReturn:
                    stm = CreateReturn(elem, compileContext, resultType);
                    alwaysReturnValue = true;
                    break;
                default:
                    throw new Exception(string.Format("The command type '{0}' is not allowed, {1}", elem.Name, elem.GetLineAndColumn()));
            }

            return stm;
        }


        public static VariableDef CreateVariableDef(CodeElement elem, Context compileContext)
        {
            // variableDef = typeAndId '=' exp ';';
            var def = CreateDeclare(elem.Codes(WordDeclare).First(), true);

            ExpBase exp = null;
            if (elem.Codes().Count() == 3)
            {
                CodeElement expCode = elem.Codes().Last();
                exp = Compiler.Expression(expCode, compileContext);
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
            if (compileContext.FunctionScope.ExistsFunction(def.TheName))
                throw new Exception(string.Format("A Function called '{0}', {1}, is declared", def.TheName, elem.GetLineAndColumn()));

            var varDef = new VariableDef() { Name = def.TheName, VariableType = def.TheType, Expression = exp };

            compileContext.BuildVariable(def.TheType, def.TheName, elem);
            return varDef;
        }

        public static Assign CreateAssign(CodeElement elem, Context compileContext)
        {
            // assign      = identifier '=' exp ';';
            var cmd = new Assign();
            CodeElement idElem = elem.Codes(WordIdentifier).First();
            cmd.VariableName = idElem.Value;
            cmd.VariableType = compileContext.ExistsVariable(cmd.VariableName, idElem);

            CodeElement expCode = elem.Codes().Last();
            cmd.Expression = Expression(expCode, compileContext);
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

        public static ProgramBase CreateBody(CodeElement elem, Context compileContext, DefType resultType, out bool alwaysReturnValue)
        {
            ProgramBase cmd = null;
            CodeElement innerCode = elem.Codes().First();
            if (innerCode.Name == WordLocalScope)
            {
                cmd = CreateLocalScope(innerCode, compileContext, resultType, out alwaysReturnValue);
            }
            else
                cmd = CreateVariableOrCommand(innerCode, compileContext, resultType, out alwaysReturnValue);

            return cmd;
        }

        public static If CreateIf(CodeElement elem, Context compileContext, DefType resultType, out bool alwaysReturnValue)
        {
            // if          = 'if' '(' exp ')' body ['else' body];
            ExpBase expbase = Expression(elem.Codes().First(), compileContext);
            if (expbase.ExpressionType != DefType.Bool)
                throw new Exception(string.Format("The expression type must be a boolean, {0}", elem.GetLineAndColumn()));

            var cmd = new If();
            cmd.Expression = expbase as ExpTyped<bool>;
            bool trueReturn;
            cmd.TrueStatement = CreateBody(elem.Codes(WordBody).First(), compileContext, resultType, out trueReturn);
            bool elseReturn;
            cmd.ElseStatement = CreateBody(elem.Codes(WordBody).Last(), compileContext, resultType, out elseReturn);
            alwaysReturnValue = trueReturn && elseReturn;
            return cmd;
        }

        public static Return CreateReturn(CodeElement elem, Context compileContext, DefType resultType)
        {
            // return      = 'return' [exp] ';';

            var cmd = new Return();

            CodeElement expCode = elem.Codes().FirstOrDefault();
            if (expCode != null)
            {
                cmd.Expression = Compiler.Expression(expCode, compileContext);

                // Check resulttype of command
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

        public static While CreateWhile(CodeElement elem, Context compileContext, DefType resultType)
        {
            // loop        = 'while' '(' exp ')' body;
            ExpBase expbase = Expression(elem.Codes().First(), compileContext);
            if (expbase.ExpressionType != DefType.Bool)
                throw new Exception(string.Format("The expression type must be a boolean, {0}", elem.GetLineAndColumn()));

            var cmd = new While();
            cmd.Expression = expbase as ExpTyped<bool>;
            cmd.Body = CreateBody(elem.Codes(WordBody).First(), compileContext, resultType, out bool ret);
            return cmd;
        }

        public static FuncCall CreateFuncCall(CodeElement elem, Context compileContext)
        {

            return new FuncCall();
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
        public const string WordValue = "value";
        public const string WordReal = "real";
        public const string WordVar = "var";
        public const string WordInt = "int";


        public static ExpBase Expression(CodeElement elem, Context compileContext)
        {
            if (elem == null) throw new Exception("Missing expression element");

            // First non operator values
            switch (elem.Name)
            {
                case WordInt:
                    return new ExpInt(elem);
                case WordExp:
                    return Expression(elem.Codes().FirstOrDefault(), compileContext);
                case WordPar:
                    return Expression(elem.Codes().FirstOrDefault(), compileContext);
            }

            // Then binary operator values. All binary operators has two operants.
            CodeElement first = elem.Codes().FirstOrDefault();
            CodeElement next = elem.Codes().FirstOrDefault(c => c != first);
            ExpBase op1 = Expression(first, compileContext);
            ExpBase op2 = Expression(next, compileContext);

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
