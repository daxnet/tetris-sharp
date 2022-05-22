using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Services
{
    internal sealed class CollisionDetector
    {
        public CollisionDetector()
        { }

        public bool Collides(IVisibleComponent a, IVisibleComponent b, out CollisionInfo ciA, out CollisionInfo ciB, bool byPixel = false)
        {
            if (a.CollisionDetective && b.CollisionDetective)
            {
                var widthMe = a.Texture.Width;
                var heightMe = a.Texture.Height;
                var widthOther = b.Texture.Width;
                var heightOther = b.Texture.Height;

                if (byPixel &&                                // if we need per pixel
                    ((Math.Min(widthOther, heightOther) > 128) ||  // at least avoid doing it
                    (Math.Min(widthMe, heightMe) > 128)))          // for small sizes (nobody will notice :P)
                {
                    return SimpleIntersects(a.BoundingBox, b.BoundingBox, out ciA, out ciB)
                        && PerPixelCollision(a, b);
                }


                return SimpleIntersects(a.BoundingBox, b.BoundingBox, out ciA, out ciB);
            }

            ciA = CollisionInfo.Empty;
            ciB = CollisionInfo.Empty;
            return false;
        }

        private static bool SimpleIntersects(Rectangle a, Rectangle b, out CollisionInfo ciA, out CollisionInfo ciB)
        {
            var intersection = Rectangle.Intersect(a, b);
            if (intersection != Rectangle.Empty)
            {
                ciA = CalculationCollisionInfo(a, intersection);
                ciB = CalculationCollisionInfo(b, intersection);

                return true;
            }

            ciA = CollisionInfo.Empty;
            ciB = CollisionInfo.Empty;
            return false;
        }

        private static CollisionInfo CalculationCollisionInfo(Rectangle candidate, Rectangle intersection)
        {
            var orientation = Orientation.NotDefined;

            if (intersection.Center.X == candidate.Center.X)
            {
                orientation = intersection.Center.Y > candidate.Center.Y ? Orientation.South : Orientation.North;
            }
            else if (intersection.Center.Y == candidate.Center.Y)
            {
                orientation = intersection.Center.X > candidate.Center.X ? Orientation.East : Orientation.West;
            }
            else
            {
                if (intersection.Center.X < candidate.Center.X)
                {
                    orientation = Orientation.West;
                }
                else
                {
                    orientation = Orientation.East;
                }

                if (intersection.Center.Y < candidate.Center.Y)
                {
                    orientation |= Orientation.North;
                }
                else
                {
                    orientation |= Orientation.South;
                }
            }

            return new CollisionInfo(orientation, 0);
        }

        private static bool PerPixelCollision(IVisibleComponent a, IVisibleComponent b)
        {
            // Get Color data of each Texture
            Color[] bitsA = new Color[a.Texture.Width * a.Texture.Height];
            a.Texture.GetData(bitsA);
            Color[] bitsB = new Color[b.Texture.Width * b.Texture.Height];
            b.Texture.GetData(bitsB);

            // Calculate the intersecting rectangle
            int x1 = Math.Max(a.Texture.Bounds.X, b.Texture.Bounds.X);
            int x2 = Math.Min(a.Texture.Bounds.X + a.Texture.Bounds.Width, b.Texture.Bounds.X + b.Texture.Bounds.Width);

            int y1 = Math.Max(a.Texture.Bounds.Y, b.Texture.Bounds.Y);
            int y2 = Math.Min(a.Texture.Bounds.Y + a.Texture.Bounds.Height, b.Texture.Bounds.Y + b.Texture.Bounds.Height);

            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    Color ca = bitsA[(x - a.Texture.Bounds.X) + (y - a.Texture.Bounds.Y) * a.Texture.Width];
                    Color cb = bitsB[(x - b.Texture.Bounds.X) + (y - b.Texture.Bounds.Y) * b.Texture.Width];

                    if (ca.A != 0 && cb.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            // If no collision occurred by now, we're clear.
            return false;
        }
    }
}
