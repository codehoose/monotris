using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monotris.FX
{
    internal class ParticleComponent : DrawableGameComponent
    {
        private readonly Texture2D _texture;
        private readonly SpriteBatch _spriteBatch;
        private readonly Color _colour;
        private readonly int _size;
        private Vector2 _position;
        private Vector2 _speed;

        private float _time = 0f;
        private float _duration = 0f;
        
        private float _alpha = 1f;

        public ParticleComponent(Game game, Texture2D texture, Vector2 position, Color colour, int size, float duration) : base(game)
        {
            _texture = texture;
            _spriteBatch = ((Game1)game).SpriteBatch;
            _position = position;
            _duration = duration;
            _colour = colour;
            _size = size;
            var rnd = new Random();
            _speed = new Vector2(0, (float)-rnd.NextDouble()) * 30;
            float randomAngle = (float)(rnd.NextDouble() * MathF.PI * 2);
            _speed = RotateVector(_speed, randomAngle);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _time += gameTime.TotalGameTime.Milliseconds / 1000f / _duration;
            _alpha = float.Lerp(1f, 0f, _time);

            if (_time>=1f)
            {
                Game.Components.Remove(this);
            }

            _position += _speed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _spriteBatch.Draw(_texture,
                              new Rectangle((int)_position.X, (int)_position.Y, _size, _size),
                              _colour * _alpha);
        }

        Vector2 RotateVector(Vector2 v, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
        }
    }
}
