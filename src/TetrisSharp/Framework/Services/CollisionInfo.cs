using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Services
{
    public struct CollisionInfo
    {
        public static readonly CollisionInfo Empty = new CollisionInfo(Orientation.NotDefined, 0F);

        public CollisionInfo(Orientation orientation, float angle) : this()
        {
            Orientation = orientation;
            Angle = angle;
        }

        public Orientation Orientation { get; set; }

        public float Angle { get; set; }
    }
}
