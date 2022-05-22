using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TetrisSharp.Framework.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Components
{
    public sealed class Text : VisibleComponent
    {
        private readonly SpriteFont font;
        private readonly TextRenderingOptions options;

        public Text(string value, IScene scene, SpriteFont font) 
            : this(value, scene, font, TextRenderingOptions.DefaultOptions, Vector2.Zero)
        {
        }

        public Text(string value, IScene scene, SpriteFont font, TextRenderingOptions options, Vector2 position) 
            : base(scene, font.Texture, position)
        {
            CollisionDetective = false;
            this.font = font;
            this.Value = value;
            this.options = options;
        }

        public Text(string value, IScene scene, SpriteFont font, Color color, Vector2 position)
            : this(value, scene, font, new TextRenderingOptions(false, color), position)
        { }

        public Text(string value, IScene scene, SpriteFont font, Color color)
            : this(value, scene, font, color, Vector2.Zero)
        { }

        protected override void DoDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.font, this.Value, this.Position, this.options.Color);
        }

        public string Value { get; set; }

        public float TextWidth => this.font.MeasureString(Value).X;

        public float TextHeight => this.font.MeasureString(Value).Y;
    }
}
