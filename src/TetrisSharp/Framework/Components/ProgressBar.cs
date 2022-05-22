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
    public sealed class ProgressBar : VisibleComponent
    {
        private float minimum = 0.0f;
        private float maximum = 100.0f;
        private float progress = 0;

        private Rectangle borderOuterRect;
        private Rectangle borderInnerRect;
        private Rectangle backgroundRect;
        private Rectangle fillRect;

        private Color borderColorOuter;
        private Color borderColorInner;
        private Color fillColor;
        private Color backgroundColor;

        private Color[] outerData;
        private Color[] innerData;
        private Color[] fillData;
        private Color[] backgroundData;
        private Texture2D outerTexture;
        private Texture2D innerTexture;
        private Texture2D backgroundTexture;
        private Texture2D fillTexture;

        public ProgressBar(IScene scene, Rectangle rectangle)
            : this(scene, rectangle, Orientation.HorizontalLeftToRight)
        { }


        public ProgressBar(IScene scene, Rectangle rectangle, Orientation barOrientation)
            : base(scene)
        {
            CollisionDetective = false;
            borderOuterRect = rectangle;
            BarOrientation = barOrientation;
            Initialize();
        }

        public ProgressBar(IScene scene, int x, int y, int width, int height)
            : this(scene, new Rectangle(x, y, width, height)) { }

        public ProgressBar(IScene scene, int x, int y, int width, int height, Orientation barOrientation)
            : this(scene, new Rectangle(x, y, width, height), barOrientation)
        { }

        public override float X
        {
            get => borderOuterRect.X;
            set => borderOuterRect.X = (int)value;
        }

        public override float Y
        {
            get => borderOuterRect.Y;
            set => borderOuterRect.Y = (int)value;
        }

        public override int Height => borderOuterRect.Height;

        public override int Width => borderOuterRect.Width;

        public float Value
        {
            get => progress;
            set
            {
                progress = value;
                if (progress < minimum)
                {
                    progress = minimum;
                }
                if (progress > maximum)
                {
                    progress = maximum;
                }

                UpdateRectangles();
            }
        }

        private void Initialize()
        {
            // create some textures.  These will actually be overwritten when colors are set below.
            outerTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            innerTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            backgroundTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
            fillTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);

            // initialize data arrays for building textures
            outerData = new Color[1];
            innerData = new Color[1];
            fillData = new Color[1];
            backgroundData = new Color[1];

            // initialize colors
            BorderColorOuter = Color.Gray;
            BorderColorInner = Color.Black;
            FillColor = Color.DarkBlue;
            BackgroundColor = Color.White;

            // set border thickness
            BorderThicknessInner = 2;
            BorderThicknessOuter = 3;

            // calculate the rectangles for displaying the progress bar
            UpdateRectangles();
        }

        private void UpdateRectangles()
        {
            // figure out inner border
            borderInnerRect = borderOuterRect;
            borderInnerRect.Inflate(BorderThicknessOuter * -1, BorderThicknessOuter * -1);

            // figure out background rectangle
            backgroundRect = borderInnerRect;
            backgroundRect.Inflate(BorderThicknessInner * -1, BorderThicknessInner * -1);

            // figure out fill rectangle based on progress.
            fillRect = backgroundRect;
            float percentProgress = (progress - minimum) / (maximum - minimum);
            // calculate fill properly according to orientation
            switch (BarOrientation)
            {
                case Orientation.HorizontalLeftToRight:
                    fillRect.Width = (int)((float)fillRect.Width * percentProgress); break;
                case Orientation.HorizontalRightToLeft:
                    // right to left means short the fill rect as usual, but it must justified to the right
                    fillRect.Width = (int)((float)fillRect.Width * percentProgress);
                    fillRect.X = backgroundRect.Right - fillRect.Width;
                    break;
                case Orientation.VerticalBottomToTop:
                    //justify the fill to the bottom
                    fillRect.Height = (int)((float)fillRect.Height * percentProgress);
                    fillRect.Y = backgroundRect.Bottom - fillRect.Height;
                    break;
                case Orientation.VerticalTopToBottom:
                    fillRect.Height = (int)((float)fillRect.Height * percentProgress); break;
                default:// default is HORIZONTAL_LR
                    fillRect.Width = (int)((float)fillRect.Width * percentProgress); break;
            }
        }

        public float Minimum
        {
            get => minimum;
            set
            {
                minimum = value;
                this.Value = progress;
            }
        }

        public float Maximum
        {
            get => maximum;
            set
            {
                maximum = value;
                this.Value = progress;
            }
        }

        public Color BorderColorOuter
        {
            get
            {
                return borderColorOuter;
            }
            set
            {
                if (borderColorOuter != value)
                {
                    borderColorOuter = value;
                    outerData[0] = borderColorOuter;
                    outerTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
                    outerTexture.SetData(outerData);
                }
            }
        }

        public int BorderThicknessOuter { get; set; }

        public Color BorderColorInner
        {
            get
            {
                return borderColorInner;
            }
            set
            {
                if (borderColorInner != value)
                {
                    borderColorInner = value;
                    innerData[0] = borderColorInner;
                    innerTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
                    innerTexture.SetData(innerData);
                }
            }
        }

        public int BorderThicknessInner { get; set; }

        public Color FillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    fillData[0] = fillColor;
                    fillTexture.Dispose();
                    fillTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
                    fillTexture.SetData(fillData);
                }
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    backgroundData[0] = backgroundColor;
                    backgroundTexture = new Texture2D(Scene.Game.GraphicsDevice, 1, 1, true, SurfaceFormat.Color);
                    backgroundTexture.SetData(backgroundData);
                }
            }
        }

        public Orientation BarOrientation { get; }

        protected override void DoDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw the outer border
            spriteBatch.Draw(outerTexture, borderOuterRect, Color.White);
            // draw the inner border
            spriteBatch.Draw(innerTexture, borderInnerRect, Color.White);
            // draw the background color
            spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);
            // draw the progress
            spriteBatch.Draw(fillTexture, fillRect, Color.White);
        }

        public enum Orientation
        {
            HorizontalLeftToRight,
            HorizontalRightToLeft,
            VerticalTopToBottom,
            VerticalBottomToTop
        }
    }
}
