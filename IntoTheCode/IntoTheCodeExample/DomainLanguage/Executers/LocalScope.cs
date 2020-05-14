using IntoTheCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class LocalScope : ProgramBase
    {
        private LocalScope _parentScope;

        private List<ProgramBase> _commands;


        public LocalScope(DefType resultType, CodeElement elem, Context compileContext, LocalScope parentScope) : base()
        {
            ResultType = resultType;

            _parentScope = parentScope;
            compileContext.FunctionScope = this;

            foreach (CodeElement item in elem.Codes(WordFunctionDef))
                DeclareFunction(new FunctionDef(item, compileContext), item);

            _commands = new List<ProgramBase>();
            CodeElement lastCommant;
            foreach (CodeElement item in elem.Codes(c => c.Name != WordFunctionDef))
            {
                ProgramBase stm = null;
                
                switch (item.Name)
                {
                    case WordVariableDef:
                        stm = new VariableDef(item, compileContext);
                        break;
                    case WordAssign:
                        stm = new Assign(item, compileContext);
                        break;
                    case WordIf: 
                        stm = new If(item, compileContext);
                        // todo check always returns
                        break;
                    case WordLoop:
                        stm = new While(item, compileContext);
                        // todo check always returns
                        break;
                    case WordFunc:
                        stm = new Func(item, compileContext);
                        break;
                    case WordReturn:
                        stm = new Return(item, compileContext);
                        if (item != elem.Codes(c => c.Name != WordFunctionDef).Last())
                            throw new Exception(string.Format("The return statement is not last in scope, {0}", item.GetLineAndColumn()));
                        break;
                    default: throw new Exception(string.Format("The command type '{0}' is not allowed, {1}", item.Name, item.GetLineAndColumn()));
                }
                lastCommant = item;


                _commands.Add(stm);

            }

            // Check resulttype last command
            if (ResultType != DefType.Void && (_commands.Count == 0 || !(_commands.Last() is Return)))
                throw new Exception(string.Format("This function must return a value, {0}", elem.GetLineAndColumn()));

        }

        /// <summary>If the statement is part of a function, this is the return type.</summary>
        public readonly DefType ResultType;

        public readonly bool MustReturn;
        public readonly bool AlwaysReturn;

        public override bool Run(Context runtime)
        {
            foreach (ProgramBase item in _commands)
                if (item.Run(runtime)) return true;
            
            return false;
        }

        #region Function context


        public Dictionary<string, FunctionDef> Functions { get; private set; }

        public void DeclareFunction(FunctionDef func, CodeElement elem)
        {
            if (Functions.ContainsKey(func.Name))
                // todo check type and parameters
                throw new Exception(string.Format("A function called '{0}', {1}, is allready declared", func.Name, elem.GetLineAndColumn()));

            Functions.Add(func.Name, func);
        }

        public bool ExistsFunction(string name)
        {
            return Functions.ContainsKey(name);
        }

        private FunctionDef GetFunction(string name, List<DefType> types)
        {
            FunctionDef function;
            if (Functions.TryGetValue(name, out function))
                // todo check type and parameters
                return function;
            else if (_parentScope != null)
                return _parentScope.GetFunction(name, types);
            else
                throw new Exception(string.Format("This function is not declared '{0}'", name));
        }

        #endregion
    }
}
