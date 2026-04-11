using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monotris.States;
using System;

namespace Monotris
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private GameComponent _currentComponent;
        private SpriteBatch _spriteBatch;

        public SpriteBatch SpriteBatch => _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void PlayGame()
        {
            Components.Clear();
            AddComponent<MonotrisGameState>();
        }

        public void MainMenu()
        {
            Components.Clear();
            AddComponent<MainMenuState>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentComponent = AddComponent<MainMenuState>();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            base.Draw(gameTime);
            _spriteBatch.End();
        }

        private GameComponent AddComponent<T>()
        {
            var component = (GameComponent)Activator.CreateInstance(typeof(T), [this]);
            component.Initialize();
            Components.Add(component);
            return component;
        }
    }
}
