using IntoTheCode;
using IntoTheCodeExample.DomainLanguage.Executers.Expression;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public static class ProgramBuilder
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
        public const string WordLoop = "myloop";
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
            var prog = new Program();

            Scope externalScope = functions == null ? null : new Scope(null, null, functions);
            Variables variables = new Variables(null, parameters);
            prog.RootScope = CreateScope(doc.Codes(WordScope).First(), variables, externalScope, DefType.Void, out bool alwaysReturn);

            return prog;
        }

        public static Scope CreateScope(CodeElement elem, Variables innerVariables, Scope parentScope, DefType resultType, out bool alwaysReturnValue) //, bool mustReturn
        {
            // scope       = {functionDef | variableDef | operation};
            alwaysReturnValue = false;

            var scope = new Scope(parentScope, innerVariables);

            //Create all function declarations before funtion-scopes.
            foreach (CodeElement item in elem.Codes(WordFunctionDef))
                CreateFunctionDef(item, scope);

            // Create function-scopes (that may make calls to other functions in same scope)
            foreach (CodeElement item in elem.Codes(WordFunctionDef))
                CreateFunctionInnerScope(item, scope);

            List<OperationBase> operations = new List<OperationBase>();
            foreach (CodeElement item in elem.Codes(c => c.Name != WordFunctionDef))
                if (alwaysReturnValue)
                    throw new Exception(string.Format("The statement can not be reached, {0}, {1}", item.Name, item.GetLineAndColumn()));
                else
                    operations.Add(CreateVariableOrOperation(item, scope, resultType, out alwaysReturnValue));

            scope.Operations = operations;

            return scope;
        }

        public static void CreateFunctionDef(CodeElement elem, Scope scope)
        {
            // functionDef = typeAndId '(' [typeAndId {',' typeAndId}] ')' '{' scope '}';
            CodeElement defElem = elem.Codes(WordDeclare).First();
            var def2 = CreateDeclare(defElem, false);
            Function funcDef = new Function() { FuncType = def2.TheType, Name = def2.TheName };

            if (scope.Functions.ContainsKey(funcDef.Name))
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", funcDef.Name, elem.GetLineAndColumn()));

            funcDef.Parameters = new List<Declare>();
            foreach (CodeElement item in elem.Codes(WordDeclare).Where(e => e != defElem))
            {
                var parm = CreateDeclare(item, true);
                funcDef.Parameters.Add(parm);
            }

            scope.Functions.Add(funcDef.Name, funcDef);
        }

        public static void CreateFunctionInnerScope(CodeElement elem, Scope scope)
        {
            // functionDef = typeAndId '(' [typeAndId {',' typeAndId}] ')' '{' scope '}';
            CodeElement defElem = elem.Codes(WordDeclare).First();
            var def2 = CreateDeclare(defElem, false);
            Function funcDef;
            scope.Functions.TryGetValue(def2.TheName, out funcDef);

            Variables innerVariables = new Variables(null, null);

            foreach (CodeElement item in elem.Codes(WordDeclare).Where(e => e != defElem))
            {
                var parm = CreateDeclare(item, true);
                innerVariables.BuildVariable(parm.TheType, parm.TheName, item);
            }

            CodeElement scopeElem = elem.Codes(WordScope).First();

            bool alwaysReturns;
            funcDef.FunctionScope = CreateScope(scopeElem, innerVariables, scope, funcDef.FuncType, out alwaysReturns);

            if (funcDef.FuncType != DefType.Void && !alwaysReturns)
                throw new Exception(string.Format("The function '{0}' must return a value, {1}", funcDef.Name, elem.GetLineAndColumn()));
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
                    break;
                case WordWhile:
                case WordLoop:
                    stm = CreateWhile(elem, scope, resultType);
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
            var cmd = new VariableDef() { Name = def.TheName, VariableType = def.TheType };

            if (elem.Codes().Count() == 2)
            {
                CodeElement expCode = elem.Codes().Last();
                cmd.Expression = ExpressionBuilder.CreateExpression(expCode, scope);

                if (cmd.VariableType == DefType.Bool && !ExpBase.IsBool(cmd.Expression))
                    throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                if (cmd.VariableType == DefType.Int && !ExpBase.IsInt(cmd.Expression))
                    throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                if (cmd.VariableType == DefType.Float && !ExpBase.IsNumber(cmd.Expression))
                    throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
            }
            if (scope.ExistsFunction(cmd.Name))
                throw new Exception(string.Format("A Function called '{0}', {1}, is declared", cmd.Name, elem.GetLineAndColumn()));

            scope.Vars.BuildVariable(cmd.VariableType, cmd.Name, elem);
            return cmd;
        }

        public static Assign CreateAssign(CodeElement elem, Scope scope)
        {
            // assign      = identifier '=' exp ';';
            var cmd = new Assign();
            CodeElement idElem = elem.Codes(WordIdentifier).First();
            cmd.VariableName = idElem.Value;
            cmd.VariableType = scope.ExistsVariable(cmd.VariableName, idElem);

            CodeElement expCode = elem.Codes().Last();
            cmd.Expression = ExpressionBuilder.CreateExpression(expCode, scope);

            if (cmd.VariableType == DefType.Bool && !ExpBase.IsBool(cmd.Expression))
                throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
            if (cmd.VariableType == DefType.Int && !ExpBase.IsInt(cmd.Expression))
                throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
            if (cmd.VariableType == DefType.Float && !ExpBase.IsNumber(cmd.Expression))
                throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));

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
            ExpBase expbase = ExpressionBuilder.CreateExpression(elem.Codes().First(), scope);
            var cmd = new If();
            cmd.Expression = expbase as ExpTyped<bool>;
            if (cmd.Expression == null)
                throw new Exception(string.Format("The expression type must be a boolean, {0}", elem.GetLineAndColumn()));

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
                cmd.Expression = ExpressionBuilder.CreateExpression(expCode, scope);

                if (resultType == DefType.Bool && !ExpBase.IsBool(cmd.Expression))
                    throw new Exception(string.Format("The expression must be a bool'. {0}", expCode.GetLineAndColumn()));
                if (resultType == DefType.Int && !ExpBase.IsInt(cmd.Expression))
                    throw new Exception(string.Format("The expression must be an integer'. {0}", expCode.GetLineAndColumn()));
                if (resultType == DefType.Float && !ExpBase.IsNumber(cmd.Expression))
                    throw new Exception(string.Format("The expression must be a number'. {0}", expCode.GetLineAndColumn()));
            }
            else
                if (resultType != DefType.Void)
                throw new Exception(string.Format("This function must return a value. {0}", elem.GetLineAndColumn()));

            return cmd;
        }

        public static While CreateWhile(CodeElement elem, Scope scope, DefType resultType)
        {
            // loop        = 'while' '(' exp ')' body;
            ExpBase expbase = ExpressionBuilder.CreateExpression(elem.Codes().First(), scope);
            var cmd = new While();
            cmd.Expression = expbase as ExpTyped<bool>;
            if (cmd.Expression == null)
                throw new Exception(string.Format("The expression type must be a boolean, {0}", elem.GetLineAndColumn()));

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

        public static CmdFuncCall CreateOperationFuncCall(CodeElement elem, Scope scope)
        {
            FuncCall call = ExpressionBuilder.CreateFuncCall(elem, scope);
            return new CmdFuncCall { Call = call };
        }
    }
}
