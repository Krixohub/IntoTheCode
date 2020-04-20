using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode.Basic
{
    /// <summary>A Element has a parent, a string value (text) and a set of sub elements.</summary>
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
        
        /// <summary>Get the text of this node.</summary>
        /// <returns>The text value.</returns>
        public abstract string GetValue();// { return _value; }
        protected string _value = string.Empty;

        /// <summary>Find sub elements with a given name.</summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<TreeNode> Elements(string name)
        {
            return SubElements.Where(n => n.Name == name);
        }

        /// <summary>Find sub elements with a predicate.</summary>
        /// <param name="predicate">The predicate to filter with.</param>
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

        internal protected virtual string ToMarkupProtected(string indent, bool xmlEncode = false)
        {
            string name, value;
            if(xmlEncode)
            {
                XElement elem = new XElement("e", Value);
                name = XmlConvert.EncodeName(Name);
                value = string.IsNullOrEmpty(Value) ? Value : elem.LastNode.ToString();
            }
            else
            {
                name = Name;
                value = Value;
            }
             
            if (SubElements.Count == 0 && string.IsNullOrEmpty(value))
                return indent + "<" + name + "/>\r\n";
            if (SubElements.Count == 0)
                return indent + "<" + name + ">" + value + "</" + name + ">\r\n";
            string s = indent + "<" + name + ">" + value + "\r\n";
            s = SubElements.Aggregate(s, (current, item) => current + item.ToMarkupProtected(indent + "  ", xmlEncode));
            s += indent + "</" + name + ">\r\n";
            return s;
        }
    }
}