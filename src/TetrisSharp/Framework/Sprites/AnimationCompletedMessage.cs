using TetrisSharp.Framework.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Sprites
{
    public sealed class AnimationCompletedMessage : Message
    {
        public AnimationCompletedMessage(AnimatedSprite sprite) => Sprite = sprite;

        public AnimatedSprite Sprite { get; }
    }
}
