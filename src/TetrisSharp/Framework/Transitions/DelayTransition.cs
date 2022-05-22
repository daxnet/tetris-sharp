using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TetrisSharp.Framework.Scenes;

namespace TetrisSharp.Framework.Transitions
{
    public class DelayTransition : Transition
    {
        private TimeSpan counter = TimeSpan.Zero;
        private readonly TimeSpan delay;

        public DelayTransition(IScene scene, TimeSpan delay) : base(scene)
        {
            this.delay = delay;
        }

        public override void Update(GameTime gameTime)
        {
            counter += gameTime.ElapsedGameTime;
            if (counter >= delay)
            {
                Ended = true;
            }
        }
    }
}
