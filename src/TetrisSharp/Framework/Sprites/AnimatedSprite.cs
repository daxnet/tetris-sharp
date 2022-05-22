using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TetrisSharp.Framework.Scenes;

namespace TetrisSharp.Framework.Sprites
{
    public sealed class AnimatedSprite : Sprite
    {
        private readonly AnimatedSpriteDefinition definition;
        private int currentFrameIndex = 0;
        private TimeSpan threshold;
        private TimeSpan elapsed = TimeSpan.Zero;
        private float fps;
        private string action;
        private AnimatedSpriteActionDefinition actionDefinition;
        private int maxWidth;
        private int maxHeight;
        private bool stop = false;

        public AnimatedSprite(IScene scene, Texture2D texture, Vector2 position, AnimatedSpriteDefinition definition, string action)
            : this(scene, texture, position, definition, action, 25)
        {
        }

        public AnimatedSprite(IScene scene, Texture2D texture, Vector2 position, AnimatedSpriteDefinition definition, string action, float fps)
            : base(scene, texture, position)
        {
            this.definition = definition;
            Action = action;
            Fps = fps;
        }

        public float Fps
        {
            get => fps;
            set
            {
                fps = value;
                threshold = TimeSpan.FromMilliseconds(1000.0F / value);
            }
        }

        public string Action
        {
            get => action;
            set
            {
                action = value;
                actionDefinition = definition.Actions.FirstOrDefault(a => a.Name == action);
                maxHeight = actionDefinition.MaxFrameHeight;
                maxWidth = actionDefinition.MaxFrameWidth;
            }
        }

        public void Stop() => stop = true;

        public override void Update(GameTime gameTime)
        {
            if (!stop)
            {
                elapsed += gameTime.ElapsedGameTime;
                if (elapsed >= threshold)
                {
                    currentFrameIndex = (currentFrameIndex + 1) % actionDefinition.Frames.Count;
                    if (currentFrameIndex == 0)
                    {
                        Publish(new AnimationCompletedMessage(this));
                    }

                    elapsed = TimeSpan.Zero;
                }

                base.Update(gameTime);
            }
        }

        protected override void DoDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!stop)
            {
                var curFrame = actionDefinition.Frames[currentFrameIndex];
                var srcRect = new Rectangle(curFrame.X, curFrame.Y, curFrame.Width, curFrame.Height);
                var destRect = new Rectangle((int)X + (maxWidth - curFrame.Width) / 2,
                    (int)Y + (maxHeight - curFrame.Height) / 2, curFrame.Width, curFrame.Height);
                spriteBatch.Draw(Texture, destRect, srcRect, Color.White);
            }
        }
    }
}
