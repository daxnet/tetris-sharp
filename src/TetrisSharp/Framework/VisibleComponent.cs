// ----------------------------------------------------------------------------
//   ____                    ____                                   __
//  / __ \_  _____ _    __  / __/______ ___ _  ___ _    _____  ____/ /__
// / /_/ / |/ / _ \ |/|/ / / _// __/ _ `/  ' \/ -_) |/|/ / _ \/ __/  '_/
// \____/|___/\___/__,__/ /_/ /_/  \_,_/_/_/_/\__/|__,__/\___/_/ /_/\_\
//
// A 2D gaming framework on MonoGame
//
// Copyright (C) 2019 by daxnet.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TetrisSharp.Framework.Messaging;
using TetrisSharp.Framework.Scenes;
using System;

namespace TetrisSharp.Framework
{
    /// <summary>
    /// Represents a visible component on a game scene.
    /// </summary>
    public abstract class VisibleComponent : Component, IVisibleComponent
    {
        protected VisibleComponent(IScene scene)
            : this(scene, null)
        { }

        protected VisibleComponent(IScene scene, Texture2D texture)
            : this(scene, texture, Vector2.Zero)
        {
        }

        protected VisibleComponent(IScene scene, Texture2D texture, Vector2 position)
        {
            this.Scene = scene;
            this.Texture = texture;
            this.X = position.X;
            this.Y = position.Y;
            CollisionDetective = true;
        }

        public Rectangle BoundingBox => new Rectangle((int)X, (int)Y, Width, Height);
        public virtual bool CollisionDetective { get; set; }
        public virtual int Height => this.Texture?.Height ?? 0;

        public bool HitViewportBoundary
        {
            get
            {
                var viewport = Scene.Game.GraphicsDevice.Viewport;
                return (X <= 0) || (Y <= 0) || (X >= viewport.Width - Width) || (Y >= viewport.Height - Height);
            }
        }

        public bool OutOfViewport
        {
            get
            {
                var viewport = Scene.Game.GraphicsDevice.Viewport;
                return (X + Width <= 0) || (Y + Height <= 0) || (X >= viewport.Width) || (Y >= viewport.Height);
            }
        }

        public Vector2 Position => new Vector2(X, Y);
        public Texture2D Texture { get; }
        public bool Visible { get; set; } = true;
        public virtual int Width => this.Texture?.Width ?? 0;
        public virtual float X { get; set; }

        public virtual float Y { get; set; }

        public virtual int Layer { get; set; } = 0;

        protected IScene Scene { get; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                DoDraw(gameTime, spriteBatch);
            }
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            Scene.Game.MessageDispatcher.DispatchMessageAsync(this, message);
        }

        public void Subscribe<TMessage>(Action<object, TMessage> handler)
            where TMessage : IMessage
        {
            Scene.Game.MessageDispatcher.RegisterHandler<TMessage>(handler);
        }

        public override string ToString() => this.Id.ToString();

        public override void Update(GameTime gameTime)
        {
            var viewport = Scene.Game.GraphicsDevice.Viewport;
            var b = Boundary.None;
            if (X <= 0)
            {
                b |= Boundary.Left;
            }
            if (Y <= 0)
            {
                b |= Boundary.Top;
            }
            if (X >= viewport.Width - Width)
            {
                b |= Boundary.Right;
            }
            if (Y >= viewport.Height - Height)
            {
                b |= Boundary.Bottom;
            }

            this.Publish(new ReachBoundaryMessage(b));
        }

        protected abstract void DoDraw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}