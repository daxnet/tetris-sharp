using System.Linq;
using System.Collections.Generic;
using System;

namespace TetrisSharp.Models
{
    internal sealed class GameBoard
    {
        private readonly int[,] _boardMatrix;

        public GameBoard(int width, int height)
            => (Width, Height, _boardMatrix) = (width, height, new int[width, height]);

        public int[,] BoardMatrix => _boardMatrix;
        public int Height { get; }

        /// <summary>
        /// Gets the index of the lines that are ready to be removed.
        /// </summary>
        public IEnumerable<int> RemovingLines
        {
            get
            {
                for (var y = 0; y < Constants.NumberOfTilesY; y++)
                {
                    var numOfFilledTiles = 0;
                    for (var x = 0; x < Constants.NumberOfTilesX; x++)
                    {
                        if (_boardMatrix[x, y] == 1)
                        {
                            numOfFilledTiles++;
                        }
                    }
                    if (numOfFilledTiles == Constants.NumberOfTilesX)
                    {
                        yield return y;
                    }
                }
            }
        }

        public int Width { get; }

        public void Merge(BlockRotation rotation, int x, int y, Action mergeCallback)
        {
            for (var tileY = 0; tileY < rotation.Height; tileY++)
            {
                for (var tileX = 0; tileX < rotation.Width; tileX++)
                {
                    if (_boardMatrix[tileX + x, tileY + y] == 0 && rotation.Matrix[tileX, tileY] == 1)
                    {
                        _boardMatrix[tileX + x, tileY + y] = 1;
                    }
                }
            }

            mergeCallback();
        }

        public int CleanupFilledRows(Action<int> beforeRemoveRowCallback)
        {
            var rows = 0;
            for (var y = 0; y < Constants.NumberOfTilesY; y++)
            {
                var isFilledRow = true;
                for (var x = 0; x < Constants.NumberOfTilesX; x++)
                {
                    if (_boardMatrix[x, y] == 0)
                    {
                        isFilledRow = false;
                        break;
                    }
                }
                if (isFilledRow)
                {
                    beforeRemoveRowCallback(y);
                    for (var my = y - 1; my > 0; my--)
                    {
                        for (var mx = 0; mx < Constants.NumberOfTilesX; mx++)
                        {
                            _boardMatrix[mx, my + 1] = _boardMatrix[mx, my];
                        }
                    }

                    rows++;
                }
            }

            return rows;
        }
    }
}