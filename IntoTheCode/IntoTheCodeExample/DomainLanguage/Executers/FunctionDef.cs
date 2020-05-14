using IntoTheCode;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class FunctionDef : ProgramBase
    {
        public FunctionDef(CodeElement elem, Context compileContext, FunctionContext parentFunctions) : base(DefType.Void)
        {
            CodeElement defElem = elem.Codes(WordTypeAndId).First();
            var def = new TypeAndId(defElem, false);

            FuncType = def.TheType;
            Name = def.TheName;
            Parameters = new List<TypeAndId>();
            Context bodyContext = new Context(compileContext);

            foreach (CodeElement item in elem.Codes(WordTypeAndId).Where(e => e != defElem))
            {
                var parm = new TypeAndId(item, true);
                Parameters.Add(parm);
                bodyContext.DeclareVariable(parm.TheType, parm.TheName, item);
            }

            FunctionBody = new LocalScope(FuncType, elem.Codes("body").First(), bodyContext, parentFunctions);

            //compileContext.DeclareFunction(VarType, Name, elem);
        }

        public string Name;

        public DefType FuncType;

        public List<TypeAndId> Parameters;
        
        public LocalScope FunctionBody;

        public override bool Run(Context runtime)
        {
            throw new System.NotImplementedException();
        }
    }
}
