using System;
using System.Collections.Generic;
using System.Text;
using TetrisSharp.Framework.Scenes;
using TetrisSharp.Models;

namespace TetrisSharp.Scenes
{
    internal interface IGameScene : IScene
    {
        GameBoard GameBoard { get; }
    }
}
