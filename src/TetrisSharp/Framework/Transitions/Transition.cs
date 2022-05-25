using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TetrisSharp.Framework.Messaging;
using TetrisSharp.Framework.Scenes;

namespace TetrisSharp.Framework.Transitions
{
    public abstract class Transition : ITransition
    {
        private readonly Guid id = Guid.NewGuid();
        
        protected Transition(IScene scene)
        {
            this.Scene = scene;
        }

        public Rectangle BoundingBox => Scene.BoundingBox;

        public Vector2 Position => Vector2.Zero;

        public Texture2D Texture => Scene.Texture;

        public Guid Id => id;

        public IScene Scene { get; }

        public bool Ended { get; protected set; }

        public bool IsActive { get; set; }

        public bool CollisionDetective => false;

        public int Layer { get; set; } = 0;

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        { }

        public bool Equals(IComponent other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var transition = other as ITransition;
            if (transition == null)
            {
                return false;
            }

            return this.id.Equals(transition.Id);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            this.Scene?.Game?.MessageDispatcher.DispatchMessageAsync(this, message);
        }

        public void Subscribe<TMessage>(Action<object, TMessage> handler) where TMessage : IMessage
        {
            this.Scene?.Game.MessageDispatcher.RegisterHandler(handler);
        }

        public virtual void Update(GameTime gameTime)
        { }
    }
}
