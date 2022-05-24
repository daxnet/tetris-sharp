using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisSharp
{
    public class KeyPress
    {
        public KeyPress(Keys Key)
        {
            key = Key;
            isHeld = false;
        }

        public bool IsPressed { get { return isPressed(); } }

        public static void Update() { state = Keyboard.GetState(); }

        private Keys key;
        private bool isHeld;
        private static KeyboardState state;
        private bool isPressed()
        {
            if (state.IsKeyDown(key))
            {
                if (isHeld) return false;
                else
                {
                    isHeld = true;
                    return true;
                }
            }
            else
            {
                if (isHeld) isHeld = false;
                return false;
            }
        }
    }
}
