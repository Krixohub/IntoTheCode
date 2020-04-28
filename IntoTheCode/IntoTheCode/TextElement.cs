using IntoTheCode.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode
{
    /// <summary>The Elements that build up a TextDokument.</summary>
    /// <remarks>Inherids <see cref="TreeNode"/></remarks>
    public abstract class TextElement : TreeNode<TextElement>
    {
        /// <summary>Find sub elements with a given name.</summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<CodeElement> ChildCodes(string name)
        {
            return ChildNodes.OfType<CodeElement>().Where(n => n.Name == name);
        }

        /// <summary>Find sub elements with a predicate.</summary>
        /// <param name="predicate">The predicate to filter with.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<CodeElement> ChildCodes(Func<CodeElement, bool> predicate)
        {
            return ChildNodes.OfType<CodeElement>().Where(predicate);
        }
    }
}
