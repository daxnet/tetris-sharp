using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TetrisSharp.Framework.Scenes;
using TetrisSharp.Models;
using TetrisSharp.Scenes;

namespace TetrisSharp.Sprites
{
    internal sealed class BlockGenerator
    {
        private static readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private BlockDefinitions _blockDefinitions;

        public BlockGenerator()
        {
        }

        public void LoadFromFile(string fileName)
        {
            _blockDefinitions = BlockDefinitions.LoadFromFile(fileName);
        }

        public Block CreateBlock(IGameScene scene, Texture2D[] tileTextures, int x = -1, int y = -1)
            => CreateBlock(scene, tileTextures, _rnd.Next(_blockDefinitions.Definitions.Count), x, y);

        public Block CreateBlock(IGameScene scene, Texture2D[] tileTextures, int index, int x = -1, int y = -1) 
            => new(scene, tileTextures[index % Constants.TileTextureCount], _blockDefinitions.Definitions[index], x, y)
            {
                CollisionDetective = false
            };

        public BlockDefinitions BlockDefinitions => _blockDefinitions;
    }
}
