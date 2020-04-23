
using IntoTheCode.Basic;

namespace IntoTheCode.Read
{
    /// <summary>A hard coded element.</summary>
    /// <remarks>Inherids <see cref="CodeElement"/></remarks>
    internal class HardElement : CodeElement
    {
        public HardElement(string name, string value, params HardElement[] elements)
        {
            Name = name;
            //GrammarElement = element;
            _value = value;
            if (elements != null)
                foreach (var element in elements)
                    SubElements.Add(element);
        }

        public override string GetValue() { return _value; }
    }
}
