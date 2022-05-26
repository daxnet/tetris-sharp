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
using Microsoft.Xna.Framework.Audio;

namespace TetrisSharp.Framework.Sounds
{
    /// <summary>
    /// Represents a sound effect in the game.
    /// </summary>
    /// <seealso cref="TetrisSharp.Framework.Component" />
    public sealed class Sound : Component
    {
        #region Private Fields

        private static readonly object lockObj = new object();
        private readonly SoundEffect soundEffect;
        private readonly float volume;
        private SoundEffectInstance soundEffectInstance;
        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="soundEffect">The sound effect asset for playing the sound.</param>
        /// <param name="volume">The volume of the sound.</param>
        public Sound(SoundEffect soundEffect, float volume = 1.0F)
        {
            this.soundEffect = soundEffect;
            this.volume = volume;
        }

        #endregion Public Constructors

        #region Public Methods

        public SoundState State
        {
            get
            {
                if (soundEffectInstance != null && !soundEffectInstance.IsDisposed)
                {
                    return soundEffectInstance.State;
                }

                return SoundState.Stopped;
            }
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            Stop();

            lock (lockObj)
            {
                soundEffectInstance = soundEffect.CreateInstance();
                soundEffectInstance.Volume = this.volume;
                soundEffectInstance.Play();
            }
        }
        /// <summary>
        /// Stops playing the sound.
        /// </summary>
        public void Stop()
        {
            lock (lockObj)
            {
                if (soundEffectInstance != null &&
                    !soundEffectInstance.IsDisposed)
                {
                    try
                    {
                        soundEffectInstance.Stop(true);
                        soundEffectInstance.Dispose();
                    }
                    catch { }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        #endregion Public Methods
    }
}