using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Monotris.Input
{
    internal class KeyCooldown : GameComponent
    {
        private readonly Keys[] _keycode;
        private readonly Action _onKeyDown;
        private readonly float _cooldown;

        private float _time;

        public KeyCooldown(Game game, Keys keycode, float cooldown, Action onKeyDown)
            : this(game, [keycode], cooldown, onKeyDown)
        {

        }

        public KeyCooldown(Game game, Keys[] keycode, float cooldown, Action onKeyDown)
            : base(game)
        {
            _keycode = keycode;
            _cooldown = cooldown;
            _time = cooldown;
            _onKeyDown = onKeyDown;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keyboard = Keyboard.GetState();
            if (IsKeyDown(keyboard) && _time == 0f)
            {
                _onKeyDown?.Invoke();
                _time = _cooldown;
            }
            else
            {
                _time -= gameTime.ElapsedGameTime.Milliseconds / 1000f;
                if (_time < 0f)
                {
                    _time = 0f;
                }
            }
        }

        public bool IsKeyHeld()
        {
            var state = Keyboard.GetState();
            return IsKeyDown(state);
        }

        private bool IsKeyDown(KeyboardState state)
        {
            foreach (var key in _keycode)
            {
                if (state.IsKeyDown(key))
                    return true;

            }

            return false;
        }
    }
}
