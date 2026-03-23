using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Monotris.Input
{
    internal class KeyTap : GameComponent
    {
        private readonly Keys _key;
        private readonly Action _onKeyPressed;

        private bool _pressed;

        public KeyTap(Game game, Keys key, Action onKeyPressed) : base(game)
        {
            _key = key;
            _onKeyPressed = onKeyPressed;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
