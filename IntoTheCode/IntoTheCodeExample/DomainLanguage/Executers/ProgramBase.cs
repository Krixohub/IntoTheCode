namespace IntoTheCodeExample.DomainLanguage.Executers
{
    public abstract class ProgramBase
    {
        public const string WordFunctionDef = "functionDef";
        public const string WordVariableDef = "variableDef";
        public const string WordTypeAndId = "typeAndId";
        public const string WordFunc = "func";
        public const string WordAssign = "assign";
        public const string WordReturn = "return";
        public const string WordLoop = "loop";
        public const string WordIf = "if";
        public const string WordTypeInt = "tInt";
        public const string WordTypeStr = "tString";
        public const string WordTypeReal = "tReal";
        public const string WordTypeBool = "tBool";
        public const string WordTypeVoid = "tVoid";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType">If the statement is part of a function, this is the return type.</param>
        public ProgramBase(DefType resultType)
        {
            ResultType = resultType;
        }

        /// <summary>If the statement is part of a function, this is the return type.</summary>
        public readonly DefType ResultType;

        /// <summary>Runtime execution of statement.</summary>
        /// <param name="runtime">The context.</param>
        /// <returns>True: the statement or body has executed a 'return' statement.</returns>
        public abstract bool Run(Context runtime);
    }
}
