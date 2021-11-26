using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using System;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="ParserElementBase"/></remarks>
    internal partial class Or
    {
        /// <summary>Creator for <see cref="Or"/>.</summary>
        internal Or(ParserElementBase element1, ParserElementBase element2)
        {
            Add(element1);
            Add(element2);
        }
    }
}