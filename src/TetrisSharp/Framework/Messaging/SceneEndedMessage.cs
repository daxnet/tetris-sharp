using TetrisSharp.Framework.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Messaging
{
    public sealed class SceneEndedMessage : Message
    {
        public SceneEndedMessage(IScene scene)
        {
            this.Scene = scene;
        }

        public IScene Scene { get; }
    }
}
