using TetrisSharp.Framework.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Transitions
{
    public interface ITransition : IVisibleComponent
    {
        IScene Scene { get; }

        bool Ended { get; }
    }
}
