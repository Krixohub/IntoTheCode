using IntoTheCode.Buffer;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElements"/></remarks>
    internal partial class Parentheses
    {
        /// <summary>Creator for <see cref="Parentheses"/>.</summary>
        internal Parentheses(params ParserElementBase[] elements)
            : base(elements)
        {
        }
    }
}