using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TetrisSharp.Framework.Scenes;
using TetrisSharp.Framework.Sprites;
using TetrisSharp.Models;
using TetrisSharp.Scenes;

namespace TetrisSharp.Sprites
{
    internal sealed class Block : Sprite
    {
        private readonly BlockDefinition _definition;
        private readonly IGameScene _gameScene;
        private readonly int _tileSize;
        private readonly Texture2D _tileTexture;
        private int _currentRotationIndex = 0;
        private int _x;
        private int _y;

        public Block(IGameScene scene, Texture2D tileTexture, BlockDefinition definition)
            : this(scene, tileTexture, definition, -1, -1)
        { }

        public Block(IGameScene scene, Texture2D tileTexture, BlockDefinition definition, int x, int y)
            : base(scene, tileTexture, Vector2.Zero)
        {
            _tileTexture = tileTexture;
            _definition = definition;
            if (x == -1 && y == -1)
            {
                _x = (Constants.NumberOfTilesX - definition.Rotations[0].Width) / 2;
                _y = 0;
            }
            else
            {
                _x = x;
                _y = y;
            }
            _tileSize = tileTexture.Width;
            _gameScene = scene;
        }


        public bool CanMoveLeft
        {
            get
            {
                if (_x == 0)
                {
                    return false;
                }

                for (var y = 0; y < CurrentRotation.Height; y++)
                {
                    if (_x > 0 && 
                        CurrentRotation.Matrix[0, y] == 1 && 
                        _gameScene.GameBoard.BoardMatrix[_x - 1, _y + y] == 1)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool CanMoveRight
        {
            get
            {
                if (_x == Constants.NumberOfTilesX - CurrentRotation.Width)
                {
                    return false;
                }

                for (var y = 0; y < CurrentRotation.Height; y++)
                {
                    if (_x < Constants.NumberOfTilesX - 1 && 
                        CurrentRotation.Matrix[CurrentRotation.Width - 1, y] == 1 &&
                        _gameScene.GameBoard.BoardMatrix[_x + CurrentRotation.Width, _y + y] == 1)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool CanRotate
        {
            get
            {
                var nextRotation = _definition.Rotations[(_currentRotationIndex + 1) % _definition.Rotations.Count];
                var rotatedX = _x;
                var rotatedY = _y;
                if (rotatedX + nextRotation.Width > Constants.NumberOfTilesX)
                {
                    rotatedX = Constants.NumberOfTilesX - nextRotation.Width;
                }

                if (rotatedY + nextRotation.Height > Constants.NumberOfTilesY)
                {
                    rotatedY = Constants.NumberOfTilesY - nextRotation.Height;
                }

                for (var y = 0; y < nextRotation.Height; y++)
                {
                    for (var x = 0; x < nextRotation.Width; x++)
                    {
                        if (nextRotation.Matrix[x, y] == 1 && _gameScene.GameBoard.BoardMatrix[rotatedX + x, rotatedY + y] == 1)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public BlockRotation CurrentRotation => _definition.Rotations[_currentRotationIndex];

        public override float X
        {
            get => _x;
            set => _x = (int)value;
        }

        public override float Y
        {
            get => _y;
            set => _y = (int)value;
        }

        public bool CollisionsWithBoard()
            => CollisionsWithBoard(_currentRotationIndex);

        public bool CollisionsWithBoard(int rotationIndex)
            => CollisionsWithBoard(_definition.Rotations[rotationIndex]);

        public bool CollisionsWithBoard(BlockRotation rotation)
        {
            foreach (var (cx, cy) in rotation.BottomEdge)
            {
                var posX = cx + _x;
                var posY = cy + _y;
                if (posY + 1 >= Constants.NumberOfTilesY ||
                    _gameScene.GameBoard.BoardMatrix[posX, posY + 1] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void Rotate()
        {
            if (!CanRotate)
            {
                return;
            }

            _currentRotationIndex = (++_currentRotationIndex) % _definition.Rotations.Count;

            if (_x + CurrentRotation.Width > Constants.NumberOfTilesX)
            {
                _x = Constants.NumberOfTilesX - CurrentRotation.Width;
            }

            if (_y + CurrentRotation.Height > Constants.NumberOfTilesY)
            {
                _y = Constants.NumberOfTilesY - CurrentRotation.Height;
            }
        }

        protected override void DoDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var rotation = CurrentRotation;

            for (var tileY = 0; tileY < rotation.Height; tileY++)
                for (var tileX = 0; tileX < rotation.Width; tileX++)
                {
                    if (rotation.Matrix[tileX, tileY] == 1)
                    {
                        var posX = (_x + tileX) * _tileSize;
                        var posY = (_y + tileY) * _tileSize;
                        spriteBatch.Draw(_tileTexture, new Vector2(posX, posY), Color.White);
                    }
                }
        }
    }
}
