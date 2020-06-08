using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#pragma warning disable 1591 // no warning for missing comments

namespace IntoTheCode.Basic
{
    /// <summary>A Element has a parent, a string value (text) and a set of sub elements.</summary>
    public abstract class TreeNode<TElement> where TElement : TreeNode<TElement>// : NotifyChanges
    {
        /// <summary>Creator for <see cref="TreeNodeG"/>.</summary>
        /// <exclude/>
        internal TreeNode(IList<TElement> elements)
        {
            if (elements == null)
                return;
            foreach (var element in elements)
                element.Parent = this as TElement;
            ChildNodes = elements;
        }

        /// <summary>Creator for <see cref="TreeNodeG"/>.</summary>
        /// <exclude/>
        internal TreeNode(params TElement[] elements)
        {
            ChildNodes = new List<TElement>();
            foreach (TElement item in elements)
                Add(item);
        }

        /// <summary>Property for name.</summary>
        public virtual string Name { get; set; }

        /// <summary>Property for parent element.</summary>
        public TElement Parent { get; private set; }

        /// <summary>Property for sub elements (elements).</summary>
        public virtual IList<TElement> ChildNodes { get; private set; }

        internal protected TElement Add(TElement element)
        {
            element.Parent = this as TElement;
            ChildNodes.Add(element);
            return this as TElement;
        }

        internal protected TElement AddRange(IEnumerable<TElement> elements)
        {
            foreach (var item in elements)
                Add(item);
            return this as TElement;
        }

        //public string Value { get; protected set; }
        public string Value { get { return GetValue(); } }

        /// <summary>Get the text of this node.</summary>
        /// <returns>The text value.</returns>
        protected virtual string GetValue() { return _value; }
        protected string _value = string.Empty;

        /// <summary>Find sub elements with a given name.</summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<TElement> Nodes(string name)
        {
            return ChildNodes.Where(n => n.Name == name);
        }

        /// <summary>Find sub elements with a predicate.</summary>
        /// <param name="predicate">The predicate to filter with.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual IEnumerable<TElement> Nodes(Func<TElement, bool> predicate)
        {
            return ChildNodes.Where(predicate);
        }

        /// <summary>Find sub elements with a predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A enumerable of elements.</returns>
        public virtual bool AnyNested(Func<TElement, bool> predicate)
        {
            if (ChildNodes == null)
                return false;
            Func<TElement, bool> pred = tn => predicate(tn) || tn.AnyNested(predicate);
            return ChildNodes.Any(pred);
        }


        public void ReplaceSubElement(int index, TElement replacement)
        {
            if (index >= ChildNodes.Count)
                return;
            ChildNodes[index] = replacement;
            replacement.Parent = this as TElement;
        }

        public void ReplaceSubElement(TElement oldNode, TElement replacement)
        {
            for (int i = 0; i < ChildNodes.Count; i++)
                if (ChildNodes[i] == oldNode)
                    ReplaceSubElement(i, replacement);
        }

        internal protected virtual string ToMarkupProtected(string indent, bool xmlEncode = false)
        {
            string name, value;
            if (xmlEncode)
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

            if (ChildNodes.Count == 0 && string.IsNullOrEmpty(value))
                return indent + "<" + name + "/>\r\n";
            if (ChildNodes.Count == 0)
                return indent + "<" + name + ">" + value + "</" + name + ">\r\n";
            string s = indent + "<" + name + ">" + value + "\r\n";
            s = ChildNodes.Aggregate(s, (current, item) => current + item.ToMarkupProtected(indent + "  ", xmlEncode));
            s += indent + "</" + name + ">\r\n";
            return s;
        }
    }
}