using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TetrisSharp.Framework.Messaging;
using TetrisSharp.Framework.Transitions;

namespace TetrisSharp.Framework.Scenes
{
    public abstract class Scene : IScene
    {
        #region Private Fields

        private static readonly object endingSyncLock = new object();
        private static readonly object lockRoot = new object();
        private readonly List<IComponent> gameComponents = new List<IComponent>();
        private volatile bool ended = false;
        private volatile bool ending = false;
        #endregion Private Fields

        #region Protected Constructors

        protected Scene(IOvowGame game)
            : this(game, null, Color.CornflowerBlue)
        { }

        protected Scene(IOvowGame game, Texture2D sceneTexture)
            : this(game, sceneTexture, Color.CornflowerBlue)
        { }

        protected Scene(IOvowGame game, Color backgrounColor)
            : this(game, null, backgrounColor)
        { }

        protected Scene(IOvowGame game, Texture2D sceneTexture, Color backgroundColor)
        {
            this.Game = game;
            this.Texture = sceneTexture;
            this.BackgroundColor = backgroundColor;
            this.OffsetX = 0F;
            this.OffsetY = 0F;
            AutoRemoveInactiveComponents = true;
        }

        #endregion Protected Constructors

        #region Private Destructors

        ~Scene()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Properties

        public bool AutoRemoveInactiveComponents { get; protected set; }

        public Color BackgroundColor { get; }
        public Rectangle BoundingBox => new Rectangle((int)OffsetX, (int)OffsetX, Width, Height);
        public bool CollisionDetective => false;
        public int Count => gameComponents.Count;
        public bool Ended => ended;
        public IOvowGame Game { get; }
        public int Height => Texture == null ? 0 : Texture.Height;
        public Guid Id { get; } = Guid.NewGuid();
        public ITransition In { get; protected set; }
        public bool IsActive { get; set; }
        public bool IsReadOnly => false;
        public IScene Next
        {
            get
            {
                if (string.IsNullOrEmpty(NextSceneName))
                {
                    return null;
                }

                return Game.GetSceneByName(NextSceneName);
            }
        }

        public float OffsetX { get; set; }

        public float OffsetY { get; set; }

        public ITransition Out { get; protected set; }
        public Vector2 Position => new Vector2(OffsetX, OffsetY);
        public Texture2D Texture { get; }
        public int ViewportHeight => Game.GraphicsDevice.Viewport.Height;
        public int ViewportWidth => Game.GraphicsDevice.Viewport.Width;
        public int Width => Texture == null ? 0 : Texture.Width;

        #endregion Public Properties

        #region Public Methods

        public virtual int Layer { get; set; } = 0;

        protected virtual string NextSceneName { get; set; }

        public void Add(IComponent item)
        {
            lock (lockRoot)
            {
                gameComponents.Add(item);
            }
        }

        public void Clear()
        {
            lock (lockRoot)
            {
                gameComponents.Clear();
            }
        }
        public bool Contains(IComponent item) => gameComponents.Contains(item);

        public void CopyTo(IComponent[] array, int arrayIndex) => gameComponents.CopyTo(array, arrayIndex);

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.Game.GraphicsDevice.Clear(BackgroundColor);

            lock (lockRoot)
            {
                gameComponents
                    .Where(c => c is IVisibleComponent)
                    .Select(c => c as IVisibleComponent)
                    .OrderBy(c => c.Layer)
                    .ToList()
                    .ForEach(vc => vc.Draw(gameTime, spriteBatch));
            }

            if (this.ending && !this.ended && this.Out != null)
            {
                this.Out.Draw(gameTime, spriteBatch);
            }
        }

        public virtual void End()
        {
            if (!ended)
            {
                lock (endingSyncLock)
                {
                    if (!ended)
                    {
                        ending = true;
                        if (this.Out == null)
                        {
                            DoEnd();
                        }
                    }
                }
            }
        }

        public void EndTo(string sceneName)
        {
            NextSceneName = sceneName;
            End();
        }
        public virtual void Enter() { }

        public bool Equals(IComponent other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var otherScene = other as IScene;
            if (otherScene == null)
            {
                return false;
            }

            return Id.Equals(otherScene.Id);
        }

        public IEnumerator<IComponent> GetEnumerator() => gameComponents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => gameComponents.GetEnumerator();

        public virtual void Leave() { }

        public abstract void Load(ContentManager contentManager);

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            Game.MessageDispatcher.DispatchMessageAsync(this, message);
        }

        public bool Remove(IComponent item)
        {
            lock (lockRoot)
            {
                return gameComponents.Remove(item);
            }
        }

        public void RemoveAll<TComponent>(Predicate<TComponent> predicate = null)
            where TComponent : IComponent
        {
            lock (lockRoot)
            {
                if (predicate == null)
                {
                    gameComponents.RemoveAll(_ => true);
                }

                gameComponents.RemoveAll(item => item is TComponent tc && predicate(tc));
            }
        }

        public void Subscribe<TMessage>(Action<object, TMessage> handler) where TMessage : IMessage => Game.MessageDispatcher.RegisterHandler(handler);

        public virtual void Update(GameTime gameTime)
        {
            (from comp in gameComponents where comp.IsActive select comp)
                .AsParallel()
                .ForAll(c => c.Update(gameTime));

            lock (lockRoot)
            {
                gameComponents.RemoveAll(c => !c.IsActive);
            }

            if (ending && !ended && this.Out != null)
            {
                this.Out.Update(gameTime);
                if (this.Out.Ended)
                {
                    DoEnd();
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing) { }

        #endregion Protected Methods

        #region Private Methods

        private void DoEnd()
        {
            Clear();
            Publish(new SceneEndedMessage(this));
            ended = true;
        }

        #endregion Private Methods
    }
}
