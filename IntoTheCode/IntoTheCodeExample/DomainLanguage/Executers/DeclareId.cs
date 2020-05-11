using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class DeclareId
    {
        public DeclareId(CodeElement elem)
        {
            CodeElement defType = elem.Codes("defType").FirstOrDefault();
            CodeElement identifier = elem.Codes("identifier").FirstOrDefault();

            switch (defType.Value)
            {
                case "int": TheType = DefType.Int; break;
                case "string":
                    TheType = DefType.String; break;
                case "real":
                    TheType = DefType.Float; break;
                case "bool":
                    TheType = DefType.Bool; break;
                case "void":
                    TheType = DefType.Void; break;
                default: throw new Exception(string.Format("The type '{0}' is not defined. {1}", defType.Value, defType.GetLineAndColumn()));
            }

            TheName = identifier.Value;
        }

        public DefType TheType { get; private set; }
        public string TheName { get; private set; }
    }
}