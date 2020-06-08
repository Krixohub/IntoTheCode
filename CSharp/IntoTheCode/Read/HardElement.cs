
using IntoTheCode.Basic;

namespace IntoTheCode.Read
{
    /// <summary>A hard coded element.</summary>
    /// <remarks>Inherids <see cref="TextElement"/></remarks>
    internal class HardElement : TextElement
    {
        public HardElement(string name, string value, params HardElement[] elements)
        {
            Name = name;
            _value = value;

            if (elements == null) return;
            AddRange(elements);
        }

        protected override string GetValue() { return _value; }
    }
}
