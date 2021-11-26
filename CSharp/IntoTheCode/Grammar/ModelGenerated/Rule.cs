using System.Collections.Generic;
using IntoTheCode.Buffer;
using IntoTheCode.Basic.Util;
using IntoTheCode.Grammar;
using System.Linq;
using System;

namespace IntoTheCode.Grammar
{
    /// <remarks>Inherids <see cref="SetOfElementsBase"/></remarks>
    public partial class Rule
    {
        /// <summary>
        /// Statement, Action, 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        internal Rule(string name, params ParserElementBase[] elements) : base(elements)
        {
            Name = name;

            // Set 'trust' property if this is the last of many elements
            Trust = GetTrustAuto();

            _simplify = elements.Length == 1 && elements[0] is WordBase;
        }

    }
}