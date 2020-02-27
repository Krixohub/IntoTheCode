using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode.Basic
{
    /// <summary>A Element has a parent and a set of sub elements.</summary>
    public abstract class TreeNode // : NotifyChanges
    {
        /// <summary>Creator for <see cref="TreeNode"/>.</summary>
        /// <exclude/>
        internal TreeNode(IList<TreeNode> elements)
        {
            if (elements == null)
                return;
            foreach (var element in elements)
                element.Parent = this;
            SubElements = elements;
        }

        /// <summary>Creator for <see cref="TreeNode"/>.</summary>
        /// <exclude/>
        internal TreeNode(params TreeNode[] elements)
        {
            SubElements = new List<TreeNode>();
            foreach (TreeNode item in elements)
                AddElement(item);
        }

        /// <summary>Property for name.</summary>
        public virtual string Name { get; set; }

        /// <summary>Property for parent element.</summary>
        public TreeNode Parent { get; private set; }

        /// <summary>Property for sub elements (elements).</summary>
        public virtual IList<TreeNode> SubElements { get; private set; }

        internal protected TreeNode AddElement(TreeNode element)
        {
            element.Parent = this;
            SubElements.Add(element);
            return this;
        }

        internal protected TreeNode Add(IEnumerable<TreeNode> elements)
        {
            foreach (var item in elements)
                AddElement(item);
            return this;
        }

        //public string Value { get; protected set; }
        public string Value { get { return GetValue(); } }
        public abstract string GetValue();// { return _value; }
        protected string _value;

        /// <summary>Find sub elements with a given name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<TreeNode> Elements(string name)
        {
            return SubElements.Where(n => n.Name == name);
        }

        /// <summary>Find sub elements with a predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<TreeNode> Elements(Func<TreeNode, bool> predicate)
        {
            return SubElements.Where(predicate);
        }

        /// <summary>Find sub elements with a predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual bool AnyNested(Func<TreeNode, bool> predicate)
        {
            if (SubElements == null)
                return false;
            Func <TreeNode, bool> pred = tn => predicate(tn) || tn.AnyNested(predicate);
            return SubElements.Any(pred);
        }


        public void ReplaceSubElement(int index, TreeNode replacement)
        {
            if (index >= SubElements.Count)
                return;
            SubElements[index] = replacement;
            replacement.Parent = this;
        }

        public void ReplaceSubElement(TreeNode oldNode, TreeNode replacement)
        {
            for (int i = 0; i < SubElements.Count; i++)
                if (SubElements[i] == oldNode)
                    ReplaceSubElement(i, replacement);
        }

        internal protected string ToMarkupProtected(string indent)
        {
            if (SubElements.Count == 0 && string.IsNullOrEmpty(Value))
                return indent + "<" + Name + "/>\r\n";
            if (SubElements.Count == 0)
                return indent + "<" + Name + ">" + Value + "</" + Name + ">\r\n";
            string s = indent + "<" + Name + ">" + Value + "\r\n";
            s = SubElements.Aggregate(s, (current, item) => current + item.ToMarkupProtected(indent + "  "));
            s += indent + "</" + Name + ">\r\n";
            return s;

            //return "hej";
        }
    }
}