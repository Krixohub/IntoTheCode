using IntoTheCode;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class FunctionBase
    {
        public FunctionBase(CodeElement elem, Context context)
        {
            // the CodeElement is a: funcDef   = declareId '(' [declareId {',' declareId} ]')' body; 
            LocalContext = context;

            CodeElement decItem = elem.Codes("declareId").First();
            DeclareId decId = new DeclareId(decItem);
            ReturnType = decId.TheType;

            context.DeclareFunction(elem, decId.TheName, this);

            // make a local context with parameters
            foreach (CodeElement item in elem.Codes(c => c != decItem))
            {
                DeclareId parm = new DeclareId(decItem);

            }
        }

        public Context LocalContext { get; private set; }
        public DefType ReturnType { get; private set; }

        public VariableBase Execute()
        {
            return null;
        }

    }
}
