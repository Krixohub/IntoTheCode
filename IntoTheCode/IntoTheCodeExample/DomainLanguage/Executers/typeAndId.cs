using IntoTheCode;
using System;
using System.Linq;

namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public class TypeAndId
    {

        public TypeAndId(CodeElement elem, bool isVariable)
        {
            CodeElement defType = elem.Codes().FirstOrDefault();
            CodeElement identifier = elem.Codes("identifier").FirstOrDefault();

            switch (defType.Name)
            {
                case ProgramBase.WordTypeInt: TheType = DefType.Int; break;
                case ProgramBase.WordTypeStr:
                    TheType = DefType.String; break;
                case ProgramBase.WordTypeReal:
                    TheType = DefType.Float; break;
                case ProgramBase.WordTypeBool:
                    TheType = DefType.Bool; break;
                case ProgramBase.WordTypeVoid:
                    if (isVariable)
                        throw new Exception(string.Format("Only functions can be 'void'. {0}", defType.GetLineAndColumn()));
                    TheType = DefType.Void; 
                    break;
                default: throw new Exception(string.Format("The type '{0}' is not defined. {1}", defType.Value, defType.GetLineAndColumn()));
            }

            TheName = identifier.Value;
        }

        public DefType TheType { get; private set; }
        public string TheName { get; private set; }
    }
}