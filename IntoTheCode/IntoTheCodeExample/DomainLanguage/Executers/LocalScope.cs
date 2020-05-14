using IntoTheCode;
using System;
using System.Collections.Generic;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class LocalScope : ProgramBase
    {
        public FunctionContext _functions;

        private List<ProgramBase> _commands;


        public LocalScope(DefType resultType, CodeElement elem, Context compileContext, FunctionContext parentFunctions) : base(resultType)
        {
            _functions = new FunctionContext(parentFunctions);

            foreach (CodeElement item in elem.Codes(WordFunctionDef))
                _functions.DeclareFunction(new FunctionDef(item, compileContext, parentFunctions), item);

            _commands = new List<ProgramBase>();
            foreach (CodeElement item in elem.Codes(c => c.Name != WordFunctionDef))
            {
                ProgramBase stm = null;
                
                switch (item.Name)
                {
                    case WordVariableDef:
                        stm = new VariableDef(item, compileContext, _functions);
                        break;
                    case WordAssign:
                        stm = new Assign(item);
                        break;
                    case WordIf: 
                        stm = new If(ResultType, item);
                        break;
                    case WordLoop:
                        stm = new While(ResultType, item);
                        break;
                    case WordFunc:
                        stm = new Func(ResultType, item);
                        break;
                    default: throw new Exception(string.Format("The command type '{0}' is not allowed, {1}", item.Name, item.GetLineAndColumn()));
                }

                // Check resulttype of statement
                if (stm.ResultType != DefType.Void && stm.ResultType != ResultType)
                    throw new Exception(string.Format("The command '{0}' is not allowed, {1}", item.Name, item.GetLineAndColumn()));

                _commands.Add(stm);

            }
        }

        public override bool Run(Context runtime)
        {
            foreach (ProgramBase item in _commands)
                if (item.Run(runtime)) return true;
            
            return false;
        }
    }
}
