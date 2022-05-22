// ----------------------------------------------------------------------------
//   ____                    ____                                   __
//  / __ \_  _____ _    __  / __/______ ___ _  ___ _    _____  ____/ /__
// / /_/ / |/ / _ \ |/|/ / / _// __/ _ `/  ' \/ -_) |/|/ / _ \/ __/  '_/
// \____/|___/\___/__,__/ /_/ /_/  \_,_/_/_/_/\__/|__,__/\___/_/ /_/\_\
//
// A 2D gaming framework on MonoGame
//
// Copyright (C) 2019 by daxnet.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace TetrisSharp.Framework.Sprites
{
    /// <summary>
    /// Represents the data structure of a Sprite Sheet.
    /// </summary>
    public class SpriteSheet : IDisposable, INotifyPropertyChanged
    {
        private readonly Bitmap bitmap;
        private int[,] maskMatrix;
        private int[,] edgeMatrix;
        private Color backgroundColor;
        private IEnumerable<KeyValuePair<int, List<Point>>> islands;

        private SpriteSheet(string fileName, Color backgroundColor = default(Color))
        {
            this.bitmap = (Bitmap)Image.FromFile(fileName);
            this.FileName = fileName;
            this.Width = this.bitmap.Width;
            this.Height = this.bitmap.Height;
            this.backgroundColor = backgroundColor;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Description("Width of the sprite sheet.")]
        public int Width { get; }

        [Description("Height of the sprite sheet.")]
        public int Height { get; }

        [Description("Bitmap file name of the sprite sheet.")]
        public string FileName { get; }

        [Description("The background color used for identifying the edge and background pixels.")]
        public Color BackgroundColor
        {
            get { return this.backgroundColor; }
            set
            {
                this.backgroundColor = value;
                this.maskMatrix = null;
                this.edgeMatrix = null;
                this.islands = null;

                this.NotifyPropertyChanged("BackgroundColor");
            }
        }

        [Browsable(false)]
        public int[,] MaskMatrix
        {
            get
            {
                if (this.maskMatrix == null)
                {
                    var result = new int[Width, Height];
                    for (var i = 0; i < Width; i++)
                    {
                        for (var j = 0; j < Height; j++)
                        {
                            result[i, j] = IsBackgroundPixel(i, j) ? 0 : 1;
                        }
                    }

                    this.maskMatrix = result;
                }

                return this.maskMatrix;
            }
        }

        [Browsable(false)]
        public int[,] EdgeMatrix
        {
            get
            {
                if (this.edgeMatrix == null)
                {
                    var result = new int[Width, Height];
                    for (var i = 0; i < Width; i++)
                    {
                        for (var j = 0; j < Height; j++)
                        {
                            result[i, j] = IsEdgePixel(i, j) ? 1 : 0;
                        }
                    }

                    this.edgeMatrix = result;
                }

                return this.edgeMatrix;
            }
        }

        [Browsable(false)]
        public IEnumerable<KeyValuePair<int, List<Point>>> Islands
        {
            get
            {
                if (this.islands == null)
                {
                    var edgeMatrix = EdgeMatrix;
                    var index = 0;
                    var result = new Dictionary<int, List<Point>>();

                    for (var x = 0; x < Width; x++)
                    {
                        for (var y = 0; y < Height; y++)
                        {
                            if (edgeMatrix[x, y] == 1)
                            {
                                var list = new List<Point>();
                                MarkIsland(edgeMatrix, x, y, index++, ref list);
                                result.Add(index, list);
                            }
                        }
                    }

                    this.islands = result;
                }

                return this.islands;
            }
        }

        [Description("The image of the sprite sheet.")]
        public Image Bitmap => bitmap;

        [Description("Total number of the sprites in the current sprite sheet.")]
        public int TotalSprites => this.SpriteBoundingBoxes.Count();

        [Description("Total number of the graphics islands in the current sprite sheet.")]
        public int TotalIslands => this.IslandBoundingBoxes.Count();

        [Browsable(false)]
        public IEnumerable<KeyValuePair<int, Rectangle>> IslandBoundingBoxes
        {
            get
            {
                foreach (var island in Islands)
                {
                    var x1 = island.Value.Select(_ => _.X).Min();
                    var y1 = island.Value.Select(_ => _.Y).Min();
                    var x2 = island.Value.Select(_ => _.X).Max();
                    var y2 = island.Value.Select(_ => _.Y).Max();
                    yield return new KeyValuePair<int, Rectangle>(island.Key, new Rectangle(x1, y1, x2 - x1, y2 - y1));
                }
            }
        }

        [Browsable(false)]
        public IEnumerable<KeyValuePair<int, Rectangle>> SpriteBoundingBoxes
        {
            get
            {
                var result = new Dictionary<int, Rectangle>();
                var islandBoundingBoxes = IslandBoundingBoxes;

                foreach (var boundingBox in islandBoundingBoxes)
                {
                    if (islandBoundingBoxes.Any(bb =>
                        bb.Value.X < boundingBox.Value.X &&
                        bb.Value.Y < boundingBox.Value.Y &&
                        bb.Value.X + bb.Value.Width > boundingBox.Value.X + boundingBox.Value.Width &&
                        bb.Value.Y + bb.Value.Height > boundingBox.Value.Y + boundingBox.Value.Height))
                    {
                        continue;
                    }

                    result.Add(boundingBox.Key, boundingBox.Value);
                }
                
                return result;
            }
        }

        public static SpriteSheet CreateFromFile(string fileName, Color transparentColor = default(Color)) => new SpriteSheet(fileName, transparentColor);

        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsEdgePixel(int x, int y)
        {
            var hasBackgroundPixelAround = false;
            var hasMaskPixelAround = false;

            for (var px = Math.Max(x - 1, 0); px <= Math.Min(x + 1, Width - 1); px++)
            {
                for (var py = Math.Max(y - 1, 0); py <= Math.Min(y + 1, Height - 1); py++)
                {
                    if (px == x && py == y)
                    {
                        continue;
                    }

                    if (IsBackgroundPixel(px, py))
                    {
                        hasBackgroundPixelAround = true;
                    }
                    else
                    {
                        hasMaskPixelAround = true;
                    }
                }
            }

            return hasBackgroundPixelAround && hasMaskPixelAround;
        }

        private bool IsBackgroundPixel(int x, int y) => BackgroundColor == default(Color) || BackgroundColor == Color.Transparent ? bitmap.GetPixel(x, y).A == 0 : bitmap.GetPixel(x, y).RgbEquals(BackgroundColor);

        private void MarkIsland(int[,] edgeMatrix, int x, int y, int index, ref List<Point> coordinates)
        {
            edgeMatrix[x, y] = -1;
            coordinates.Add(new Point(x, y));

            if (x - 1 >= 0)
            {
                // (i-1, j-1)
                if (y - 1 >= 0 && edgeMatrix[x - 1, y - 1] == 1)
                    MarkIsland(edgeMatrix, x - 1, y - 1, index, ref coordinates);
                // (i-1, j)
                if (edgeMatrix[x - 1, y] == 1)
                    MarkIsland(edgeMatrix, x - 1, y, index, ref coordinates);
                // (i-1, j+1)
                if (y + 1 < Height && edgeMatrix[x - 1, y + 1] == 1)
                    MarkIsland(edgeMatrix, x - 1, y + 1, index, ref coordinates);
            }
            if (x + 1 < Width)
            {
                // (i+1, j-1)
                if (y - 1 >= 0 && edgeMatrix[x + 1, y - 1] == 1)
                    MarkIsland(edgeMatrix, x + 1, y - 1, index, ref coordinates);
                // (i+1, j)
                if (edgeMatrix[x + 1, y] == 1)
                    MarkIsland(edgeMatrix, x + 1, y, index, ref coordinates);
                // (i+1, j+1)
                if (y + 1 < Height && edgeMatrix[x + 1, y + 1] == 1)
                    MarkIsland(edgeMatrix, x + 1, y + 1, index, ref coordinates);
            }
            // (i, j-1)
            if (y - 1 >= 0 && edgeMatrix[x, y - 1] == 1)
                MarkIsland(edgeMatrix, x, y - 1, index, ref coordinates);
            // (i, j+1)
            if (y + 1 < Height && edgeMatrix[x, y + 1] == 1)
                MarkIsland(edgeMatrix, x, y + 1, index, ref coordinates);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.bitmap.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SpriteSheet() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}