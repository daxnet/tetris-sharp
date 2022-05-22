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

using TetrisSharp.Framework.Messaging;
using TetrisSharp.Framework.Scenes;

namespace TetrisSharp.Framework.Services
{
    /// <summary>
    /// Represents the base class for the game services.
    /// </summary>
    /// <seealso cref="TetrisSharp.Framework.Component" />
    /// <seealso cref="TetrisSharp.Framework.Services.IGameService" />
    public abstract class GameService : Component, IGameService
    {
        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="scene">The scene to which the current game service belongs.</param>
        protected GameService(IScene scene)
        {
            this.Scene = scene;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Gets the instance of the scene in which current service
        /// has involved.
        /// </summary>
        /// <value>
        /// The instance of the scene.
        /// </value>
        public IScene Scene { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            Scene.Game.MessageDispatcher.DispatchMessageAsync(this, message);
        }

        #endregion Public Methods
    }
}