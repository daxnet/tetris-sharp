using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TetrisSharp.Models
{
    public sealed class BlockRotation
    { 
        private string _rotationDefinitions;
        private int[,] _matrix;
        private int _width;
        private int _height;

        [XmlElement("definition")]
        public string RotationDefinition
        {
            get => _rotationDefinitions;
            set
            {
                _rotationDefinitions = value;
                if (!string.IsNullOrEmpty(value))
                {
                    var splitted = value.Split(' ');
                    if (splitted.Length > 0)
                    {
                        _width = splitted[0].Length;
                        _height = splitted.Length;
                        _matrix = new int[_width, _height];
                        for (var y = 0; y < _height; y++)
                            for (var x = 0; x < _width; x++)
                            {
                                _matrix[x, y] = Convert.ToInt32(splitted[y][x].ToString());
                            }
                    }
                }
            }
        }

        [XmlIgnore]
        public int[,] Matrix => _matrix;

        [XmlIgnore]
        public int Width => _width;

        [XmlIgnore]
        public int Height => _height;

        [XmlIgnore]
        public IEnumerable<(int, int)> BottomEdge
        {
            get
            {
                var result = new List<(int, int)>();
                for (var tileX = 0; tileX < _width; tileX++)
                {
                    for (var tileY = _height - 1; tileY >= 0; tileY--)
                    {
                        if (_matrix[tileX, tileY] == 1)
                        {
                            result.Add((tileX, tileY));
                            break;
                        }
                    }
                }
                return result;
            }
        }

        public override string ToString()
        {
            if (_matrix == null || _width == 0 || _height == 0)
            {
                return base.ToString();
            }

            var sb = new StringBuilder();
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    sb.Append(_matrix[x, y]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
