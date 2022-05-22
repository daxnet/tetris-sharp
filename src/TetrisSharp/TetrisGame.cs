using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Xml.Serialization;
using TetrisSharp.Framework;
using TetrisSharp.Models;
using TetrisSharp.Scenes;
using TetrisSharp.Sprites;

namespace TetrisSharp
{
    public class TetrisGame : OvowGame
    {
        public TetrisGame()
            : base(new OvowGameWindowSettings
            {
                MouseVisible = true,
                Width = 25 * Constants.NumberOfTilesX + Constants.ScoreBoardWidth,
                Height = 25 * Constants.NumberOfTilesY,
                AllowUserResizing = false,
                IsFullScreen = false,
                Title = "Tetris#"
            })
        {
            AddScene<GameScene>("main", true);
        }
    }
}
