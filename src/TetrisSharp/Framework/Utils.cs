using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework
{
    public static class Utils
    {
        #region Extension Methods
        public static bool RgbEquals(this System.Drawing.Color src, System.Drawing.Color c)
        {
            return src.R.Equals(c.R) &&
                src.G.Equals(c.G) &&
                src.B.Equals(c.B);
        }

        public static System.Drawing.Color Inverse(this System.Drawing.Color src)
        {
            return System.Drawing.Color.FromArgb(255 - src.R, 255 - src.G, 255 - src.B);
        }
        #endregion
    }
}
