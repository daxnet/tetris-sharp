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
    /// Represents that the implemented classes are game services.
    /// </summary>
    /// <remarks>
    /// Game service is a game component that performs calculations
    /// and emits game messages. Game services might be resource-comsuming,
    /// as a result, developers can choose whether a game service needs
    /// to be included in a game scene.
    /// </remarks>
    /// <seealso cref="TetrisSharp.Framework.IComponent" />
    /// <seealso cref="TetrisSharp.Framework.Messaging.IMessagePublisher" />
    public interface IGameService : IComponent, IMessagePublisher
    {
        #region Public Properties

        /// <summary>
        /// Gets the instance of the scene in which current service
        /// has involved..
        /// </summary>
        /// <value>
        /// The instance of the scene.
        /// </value>
        IScene Scene { get; }

        #endregion Public Properties
    }
}