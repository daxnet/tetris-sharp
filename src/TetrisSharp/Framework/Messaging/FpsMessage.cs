using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Messaging
{
    public sealed class FpsMessage : Message
    {
        public FpsMessage(float fps) => Fps = fps;

        public float Fps { get; }
    }
}
