using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework
{
    public sealed class OvowGameWindowSettings
    {
        public static readonly OvowGameWindowSettings FullScreen = new OvowGameWindowSettings
        {
            IsFullScreen = true
        };

        public static readonly OvowGameWindowSettings NormalScreenShowMouse = new OvowGameWindowSettings
        {
            IsFullScreen = false,
            Width = 1024,
            Height = 768,
            MouseVisible = true,
            AllowUserResizing = true
        };

        public static readonly OvowGameWindowSettings NormalScreenNoMouse = new OvowGameWindowSettings
        {
            IsFullScreen = false,
            Width = 1024,
            Height = 768,
            MouseVisible = false,
            AllowUserResizing = true
        };

        public static readonly OvowGameWindowSettings NormalScreenShowMouseFixed = new OvowGameWindowSettings
        {
            IsFullScreen = false,
            Width = 1024,
            Height = 768,
            MouseVisible = true,
            AllowUserResizing = false
        };

        public static readonly OvowGameWindowSettings NormalScreenNoMouseFixed = new OvowGameWindowSettings
        {
            IsFullScreen = false,
            Width = 1024,
            Height = 768,
            MouseVisible = false,
            AllowUserResizing = false
        };


        public bool IsFullScreen { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
        
        public bool MouseVisible { get; set; }

        public bool AllowUserResizing { get; set; }

        public string Title { get; set; }
    }
}
