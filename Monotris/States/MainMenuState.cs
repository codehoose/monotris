using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monotris.Input;

namespace Monotris.States
{
    internal class MainMenuState : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _russianSmall;
        private SpriteFont _russianLarge;
        private bool _show = true;
        private float _flash = 0f;
        private float _flashSpeed = 1.5f;
        private Game1 _game;

        public MainMenuState(Game game) : base(game)
        {
            _game = game as Game1;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _russianSmall = Game.Content.Load<SpriteFont>("Russian");
            _russianSmall = Game.Content.Load<SpriteFont>("RussianSmall");
            _russianLarge = Game.Content.Load<SpriteFont>("RussianTitle");

            Game.Components.Add(new KeyTap(Game, Keys.Enter, () => {
                _game?.PlayGame();
            }, () => false));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _flash += gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (_flash >= _flashSpeed)
            {
                _flash -= _flashSpeed;
                _show = !_show;
                // Hide the text for less time
                _flashSpeed = _show ? 1f : .25f;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _spriteBatch.Begin();
            DrawStringCentre("MONOTRIS", _russianLarge, new Vector2(404, 96), Color.Blue);
            DrawStringCentre("MONOTRIS", _russianLarge, new Vector2(400, 92), Color.Yellow);

            DrawStringCentre("WASD TO MOVE SHAPES", _russianSmall, new Vector2(400, 228), Color.GreenYellow);
            DrawStringCentre("Q ROTATE LEFT AND E ROTATE RIGHT", _russianSmall, new Vector2(400, 268), Color.GreenYellow);

            if (_show)
            {
                DrawStringCentre("PRESS ENTER KEY TO START", _russianSmall, new Vector2(400, 420), Color.Red);
            }

            DrawStringCentre("A GAME BY SLOAN KELLY", _russianSmall, new Vector2(400, 550), Color.LightGray);

            _spriteBatch.End();
        }

        private void DrawString(string text, SpriteFont font, Vector2 pos, Color colour)
        {
            _spriteBatch.DrawString(font, text, pos, colour);
        }

        private void DrawStringCentre(string text, SpriteFont font, Vector2 pos, Color colour)
        {
            var size = font.MeasureString(text);
            var vec = pos - size / 2;
            DrawString(text, font, vec, colour);
        }
    }
}
