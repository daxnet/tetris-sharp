using Microsoft.Xna.Framework;

namespace TetrisSharp.Framework.Components
{
    /// <summary>
    /// Represents the rendering options for the texts.
    /// </summary>
    public struct TextRenderingOptions
    {
        public static readonly TextRenderingOptions DefaultOptions = new TextRenderingOptions(false, Color.Black);

        public TextRenderingOptions(bool centerScreen, Color color) : this()
        {
            CenterScreen = centerScreen;
            Color = color;
        }

        public bool CenterScreen { get; }

        public Color Color { get; }
    }
}