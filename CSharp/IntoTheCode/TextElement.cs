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
        /// <summary>Get all child code elements.</summary>
        /// <returns>A enumerable of codes.</returns>
        public virtual IEnumerable<CodeElement> Codes()
        {
            return ChildNodes.OfType<CodeElement>();
        }

        /// <summary>Find child code elements with a given name.</summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A enumerable of codes.</returns>
        public virtual IEnumerable<CodeElement> Codes(string name)
        {
            return ChildNodes.OfType<CodeElement>().Where(n => n.Name == name);
        }

        /// <summary>Find child code elements with a predicate.</summary>
        /// <param name="predicate">The predicate to filter with.</param>
        /// <returns>A enumerable of codes.</returns>
        public virtual IEnumerable<CodeElement> Codes(Func<CodeElement, bool> predicate)
        {
            return ChildNodes.OfType<CodeElement>().Where(predicate);
        }
    }
}
