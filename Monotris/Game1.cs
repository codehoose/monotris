using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monotris.Input;
using Monotris.Models;
using System.Collections.Generic;

namespace Monotris
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _block;
        private List<Piece> _pieces;
        private CurrentPiece _piece;
        private float _dropSpeed = 1f;
        private float _dropTimer = 0f;
        private KeyCooldown _dropKey;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _block = new Texture2D(GraphicsDevice, 1, 1);
            _block.SetData([Color.White]);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pieces = PickBag.GetPieces();
            _piece = new CurrentPiece();
            _piece.Piece = new Piece(_pieces[0]);

            Components.Add(new KeyTap(this, Keys.Q, _piece.RotateLeft));
            Components.Add(new KeyTap(this, Keys.E, _piece.RotateRight));
            Components.Add(new KeyCooldown(this, [Keys.A, Keys.Left], .2f, MoveLeft));
            Components.Add(new KeyCooldown(this, [Keys.D, Keys.Right], .2f, MoveRight));
            Components.Add(_dropKey = new KeyCooldown(this, [Keys.S, Keys.Down], .2f, MoveDown));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            var manualDrop = _dropKey.IsKeyHeld();
            if (manualDrop)
            {
                _dropTimer = 0f;
            }

            _dropTimer += gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (_dropTimer >= _dropSpeed && !manualDrop)
            {
                _dropTimer -= _dropSpeed;
                _piece.Y++;
            }

            var (min, max) = _piece.GetMinAndMaxY();
            if (_piece.Y + max >= 20)
            {
                // Reset everything
                ResetToNewPiece();
            }
        }

        private void UpdateBag()
        {
            _pieces.RemoveAt(0);
            if (_pieces.Count < 4)
            {
                _pieces.AddRange(PickBag.GetPieces());
            }
        }

        private void ResetToNewPiece()
        {
            UpdateBag();

            _dropTimer = 0;
            _piece.Piece = new Piece(_pieces[0]);
            _piece.X = 0;
            _piece.Y = 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            DrawBarrier(10);
            DrawPiece(_piece);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawPiece(CurrentPiece piece)
        {
            var shape = piece.Piece.Shape;
            var matrix = piece.Piece.Rotations[piece.CurrentRotation];
            var size = piece.Piece.Size;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var index = y * size + x;
                    if (shape[matrix[index]] == 1)
                    {
                        DrawBlock(piece.X + x, piece.Y + y);
                    }
                }
            }
        }

        private void DrawBlock(int x, int y)
        {
            var sx = x * 30;
            var sy = y * 30;
            _spriteBatch.Draw(_block, new Rectangle(sx, sy, 30, 30), Color.White);
        }

        private void DrawBarrier(int x)
        {
            var px = x * 30;
            for (int y = 0; y < 20; y++)
            {
                var py = y * 30;
                _spriteBatch.Draw(_block, new Rectangle(px, py, 30, 30), Color.Gray);
            }
        }

        private void MoveLeft()
        {
            var (min, max) = _piece.GetMinAndMaxX();

            if (_piece.X - 1 + min < 0)
                return;

            _piece.X--;
        }

        private void MoveRight()
        {
            var (min, max) = _piece.GetMinAndMaxX();

            if (_piece.X + 2 + max > 10)
                return;

            _piece.X++;
        }

        private void MoveDown()
        {
            var (min, max) = _piece.GetMinAndMaxY();
            if (_piece.Y + max >= 20)
                return;

            _piece.Y++;
        }
    }
}
