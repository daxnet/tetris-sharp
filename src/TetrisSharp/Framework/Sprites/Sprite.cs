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
using TetrisSharp.Framework.Scenes;

namespace TetrisSharp.Framework.Sprites
{
    public class Sprite : VisibleComponent
    {
        public Sprite(IScene scene, Texture2D texture, Vector2 position)
            : base(scene, texture, position)
        {
        }

        protected override void DoDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.Wheat);
        }
    }
}