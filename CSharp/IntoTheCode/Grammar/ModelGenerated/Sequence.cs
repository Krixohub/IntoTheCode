using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System.Linq;
using IntoTheCode.Basic.Util;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    internal partial class Sequence
    {
        /// <summary>Creator for <see cref="Sequence"/>.</summary>
        internal Sequence(params ParserElementBase[] elements) : base(elements)
        {
            //Attributter = new ObservableCollection<Attribute>();
        }
    }
}