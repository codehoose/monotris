using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Monotris.Input
{
    internal class KeyCooldown : GameComponent
    {
        private readonly Keys[] _keycode;
        private readonly Action _onKeyDown;
        private readonly Func<bool> _isPaused;
        private readonly float _cooldown;

        private float _time;

        public KeyCooldown(Game game, Keys keycode, float cooldown, Action onKeyDown, Func<bool> isPaused)
            : this(game, [keycode], cooldown, onKeyDown, isPaused)
        {

        }

        public KeyCooldown(Game game, Keys[] keycode, float cooldown, Action onKeyDown, Func<bool> isPaused)
            : base(game)
        {
            _keycode = keycode;
            _cooldown = cooldown;
            _time = cooldown;
            _onKeyDown = onKeyDown;
            _isPaused = isPaused;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_isPaused?.Invoke() ?? false)
            {
                return;
            }

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
