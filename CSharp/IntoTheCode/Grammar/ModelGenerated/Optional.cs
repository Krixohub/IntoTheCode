using System.Collections.Generic;
using System.Linq;
using IntoTheCode.Buffer;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal partial class Optional
    {
        /// <summary>Creator for <see cref="Optional"/>.</summary>
        internal Optional(params ParserElementBase[] elements)
            : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }
    }
}