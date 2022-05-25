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
using System.Collections.Generic;

namespace TetrisSharp.Framework
{
    public class OvowGame : Game, IOvowGame
    {
        #region Protected Fields

        protected SpriteBatch spriteBatch;

        #endregion Protected Fields

        #region Private Fields

        private static readonly object sync = new object();
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        // private readonly List<IScene> scenes = new List<IScene>();
        private readonly Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();
        private readonly OvowGameWindowSettings windowSettings;
        private bool disposed = false;

        // private int sceneIndex = 0;

        #endregion Private Fields

        #region Public Constructors

        public OvowGame()
            : this(OvowGameWindowSettings.NormalScreenShowMouse)
        { }

        public OvowGame(OvowGameWindowSettings windowSettings)
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            this.windowSettings = windowSettings;
            Content.RootDirectory = "Content";

            this.MessageDispatcher.RegisterHandler<SceneEndedMessage>((publisher, message) =>
            {
                lock (sync)
                {
                    ActiveScene?.Leave();

                    ActiveScene = ActiveScene.Next;

                    if (ActiveScene == null)
                    {
                        Exit();
                        return;
                    }

                    ActiveScene?.Enter();
                }
            });
        }

        #endregion Public Constructors

        #region Public Properties

        public IScene ActiveScene { get; set; }

        public int Count => scenes.Count;

        public bool IsReadOnly => false;

        public IMessageDispatcher MessageDispatcher { get; } = new MessageDispatcher();

        #endregion Public Properties

        #region Public Methods

        public void AddScene(string name, IScene item, bool isEntryScene = false)
        {
            if (isEntryScene)
            {
                if (ActiveScene == null)
                {
                    ActiveScene = item;
                }
                else
                {
                    throw new InvalidOperationException("There is already a scene that is marked as the entry scene.");
                }
            }

            this.scenes.Add(name, item);
        }

        #endregion Public Methods

        #region Protected Methods

        protected void AddScene<TScene>(string name, bool isEntryScene = false)
            where TScene : Scene
            => AddScene(name, (TScene)Activator.CreateInstance(typeof(TScene), this), isEntryScene);

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (var kvp in scenes)
                    {
                        kvp.Value.Dispose();
                    }
                }

                base.Dispose(disposing);
                disposed = true;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin();

            ActiveScene?.Draw(gameTime, this.spriteBatch);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            if (!string.IsNullOrEmpty(windowSettings.Title))
            {
                this.Window.Title = windowSettings.Title;
            }

            if (this.scenes?.Count == 0 || this.ActiveScene == null)
            {
                throw new InvalidOperationException("No active scene has been defined.");
            }

            graphicsDeviceManager.IsFullScreen = this.windowSettings.IsFullScreen;
            if (!this.windowSettings.IsFullScreen)
            {
                graphicsDeviceManager.PreferredBackBufferWidth = this.windowSettings.Width;
                graphicsDeviceManager.PreferredBackBufferHeight = this.windowSettings.Height;
            }
            else
            {
                graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            }

            Window.AllowUserResizing = this.windowSettings.AllowUserResizing;
            this.IsMouseVisible = this.windowSettings.MouseVisible;
            graphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach(var kvp in this.scenes)
            {
                kvp.Value.Load(Content);
            }

            ActiveScene?.Enter();
        }

        protected override void Update(GameTime gameTime)
        {
            ActiveScene?.Update(gameTime);

            base.Update(gameTime);
        }

        public IScene GetSceneByName(string sceneName) => scenes[sceneName];

        #endregion Protected Methods
    }
}