using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Monotris.Input
{
    internal class KeyTap : GameComponent
    {
        private readonly Keys _key;
        private readonly Action _onKeyPressed;
        private readonly Func<bool> _isPaused;

        private bool _pressed;

        public KeyTap(Game game, Keys key, Action onKeyPressed, Func<bool> isPaused) : base(game)
        {
            _key = key;
            _onKeyPressed = onKeyPressed;
            _isPaused = isPaused;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_isPaused?.Invoke() ?? false)
            {
                return;
            }

            var state = Keyboard.GetState();
            if (state.IsKeyDown(_key) && !_pressed)
            {
                _pressed = true;
                _onKeyPressed?.Invoke();
            }

            if (state.IsKeyUp(_key) && _pressed)
            {
                _pressed = false;
            }
        }
    }
}
