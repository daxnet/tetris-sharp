using Microsoft.Xna.Framework.Input;

namespace TetrisSharp
{
    public class KeyPress
    {
        private static KeyboardState _state;

        private readonly Keys _key;
        private bool _isHeld;

        public KeyPress(Keys key)
        {
            _key = key;
            _isHeld = false;
        }

        public bool IsPressed
        {
            get
            {

                if (_state.IsKeyDown(_key))
                {
                    if (_isHeld) return false;
                    else
                    {
                        _isHeld = true;
                        return true;
                    }
                }
                else
                {
                    if (_isHeld) _isHeld = false;
                    return false;
                }
            }
        }

        public static void Update()
        {
            _state = Keyboard.GetState();
        }
    }
}
