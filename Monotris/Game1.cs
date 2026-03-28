using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monotris.Input;
using Monotris.Models;
using System;
using System.Collections.Generic;

namespace Monotris
{
    public class Game1 : Game
    {
        private enum GameState
        {
            Dropping,
            RemovingRows
        }

        private static int ONE_LINE_PTS = 40;
        private static int TWO_LINE_PTS = 100;
        private static int THREE_LINE_PTS = 300;
        private static int FOUR_LINE_PTS = 1200;

        private static int LEFT_OFFSET = 220;
        private static int PREVIEW_OFFSET = 655;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _block;
        private SpriteFont _font;
        private List<Piece> _pieces;
        private CurrentPiece _piece;
        private float _removeTime;
        private GameState _state = GameState.Dropping;
        private float _dropSpeed = 1f;
        private float _dropTimer = 0f;
        private KeyCooldown _dropKey;
        private int[] _board;
        private int _level;
        private int _rowClearCount;
        private int _score;
        private int _totalRowsCompleted;

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
            _font = Content.Load<SpriteFont>("Font");

            // Get the first set of pieces. They are the IJLOSTZ pieces in 
            // a random order. We then assign the current piece.
            _pieces = PickBag.GetPieces();
            _piece = new CurrentPiece();
            _piece.Piece = new Piece(_pieces[0]);

            // Fill the board with zeros
            ResetBoard();

            // Set up the game keys
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

            if (HasCompletedRow() && _state != GameState.RemovingRows)
            {
                _removeTime = 0f;
                _totalRowsCompleted = 0;
                _state = GameState.RemovingRows;
            }

            switch (_state)
            {
                case GameState.Dropping:
                    PlayGame(gameTime);
                    break;
                case GameState.RemovingRows:
                    RemoveRows(gameTime);
                    break;
            }
        }

        private void RemoveRows(GameTime gameTime)
        {
            if (_removeTime < 0.25f)
            {
                _removeTime += gameTime.TotalGameTime.Milliseconds / 1000f;
            }
            else
            {
                var rowToRemove = GetCompletedRow();
                _removeTime = 0f;
                if (rowToRemove >= 0)
                {
                    RemoveRowAt(rowToRemove);
                    _totalRowsCompleted++;
                }
                else
                {
                    var pts = GetPointsForDrops(_level, _totalRowsCompleted);
                    _score += pts;

                    // Increment current Level?
                    _rowClearCount += _totalRowsCompleted;
                    if (_rowClearCount>=10)
                    {
                        _rowClearCount -= 10;
                        _level++;
                    }

                    _state = GameState.Dropping;
                }
            }
        }

        private void RemoveRowAt(int rowToRemove)
        {
            var tmp = new List<int>();
            tmp.AddRange(_board);
            tmp.RemoveRange(rowToRemove * 10, 10);
            tmp.InsertRange(0, [0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            _board = tmp.ToArray();
        }

        private void PlayGame(GameTime gameTime)
        {
            UpdateBag();

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

            if (HitBottom(_piece))
            {
                // Reset everything
                StampPiece(_piece);
                ResetToNewPiece();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            DrawBarrier(0);
            DrawBarrier(11);
            DrawPiece(_piece);
            DrawBoard();
            DrawNextPieces();

            _spriteBatch.DrawString(_font, $"Score: {_score}", new Vector2(20,20), Color.White);
            _spriteBatch.DrawString(_font, $"Level: {_level}", new Vector2(20, 40), Color.Yellow);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawNextPieces()
        {
            const int blockSize = 20;
            var ox = 0; var oy = 0;

            for (int i = 1; i <= 4; i++)
            {
                var piece = _pieces[i];
                if (piece.Size == 2)
                {
                    oy += blockSize;
                }

                for (var y = 0; y < piece.Size; y++)
                {
                    for (var x = 0; x < piece.Size; x++)
                    {
                        var index = y * piece.Size + x;
                        if (piece.Shape[index] != 0)
                        DrawBlockPreview(x * blockSize + ox, y * blockSize + oy, piece.Colour, blockSize);
                    }
                }

                oy += blockSize * 3;
            }
        }

        private bool BlockedHorizontally(CurrentPiece piece, int direction = 1)
        {
            var shape = piece.GetRotatedShape();
            for (var y = 0; y < piece.Size; y++)
            {
                for (var x = 0; x < piece.Size; x++)
                {
                    var pieceIndex = y * piece.Size + x;
                    if (shape[pieceIndex] != 0)
                    {
                        var boardIndex = (piece.Y + y) * 10 + x + piece.X + direction;
                        var isOccupiedOrBoundary = boardIndex < 0 ||
                            boardIndex >= 10 * 20 ||
                            _board[boardIndex] != 0;
                        if (isOccupiedOrBoundary)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool DrawHorizontally(CurrentPiece piece, int direction, Color color)
        {
            var shape = piece.GetRotatedShape();
            for (var y = 0; y < piece.Size; y++)
            {
                for (var x = 0; x < piece.Size; x++)
                {
                    var pieceIndex = y * piece.Size + x;
                    if (shape[pieceIndex] != 0)
                    {
                        var boardIndex = (piece.Y + y) * 10 + x + piece.X + direction;
                        var isOccupiedOrBoundary = boardIndex < 0 ||
                            boardIndex >= 10 * 20 ||
                            _board[boardIndex] != 0;
                        if (isOccupiedOrBoundary)
                        {
                            DrawBlock(x + piece.X + direction, piece.Y + y, color);
                        }
                        else
                        {
                            var translucent = new Color(Color.Yellow, 0.1f);
                            DrawBlock(x + piece.X + direction, piece.Y + y, translucent);
                        }
                    }
                }
            }

            return false;
        }

        private bool HitBottom(CurrentPiece piece)
        {
            var (min, max) = _piece.GetMinAndMaxY();
            if (_piece.Y + max >= 20) return true;

            var shape = piece.GetRotatedShape();
            for (var y = 0; y < piece.Size; y++)
            {
                for (var x = 0; x < piece.Size; x++)
                {
                    var boardIndex = (piece.Y + y - 1) * 10 + x + piece.X;
                    var pieceIndex = y * piece.Size + x;
                    if (boardIndex < 10 * 20 && shape[pieceIndex] != 0)
                    {
                        var oneRowDown = (piece.Y + y) * 10 + x + piece.X;
                        if (_board[oneRowDown] != 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void UpdateBag()
        {
            if (_pieces.Count <= 4)
            {
                _pieces.AddRange(PickBag.GetPieces());
            }
        }

        private void ResetToNewPiece()
        {
            _pieces.RemoveAt(0);
            UpdateBag();
            _dropTimer = 0;
            _piece.Piece = new Piece(_pieces[0]);
            _piece.X = 0;
            _piece.Y = 0;
        }

        private void DrawPiece(CurrentPiece piece)
        {
            var shape = piece.Piece.Shape;
            var matrix = piece.Piece.Rotations[piece.CurrentRotation];
            var size = piece.Size;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var index = y * size + x;
                    if (shape[matrix[index]] != 0)
                    {
                        DrawBlock(piece.X + x, piece.Y + y, piece.Piece.Colour);
                    }
                }
            }
        }

        private void DrawBoard()
        {
            for (var y = 0; y < 20; y++)
            {
                for (var x = 0; x < 10; x++)
                {
                    var index = y * 10 + x;
                    var colourIndex = _board[index];
                    if (colourIndex != 0)
                    {
                        DrawBlock(x, y, PieceColour.Colours[colourIndex]);
                    }
                }
            }
        }

        private void DrawBlock(int x, int y) => DrawBlock(x, y, Color.White);

        private void DrawBlock(int x, int y, Color color)
        {
            var sx = x * 30;
            var sy = y * 30;
            _spriteBatch.Draw(_block, new Rectangle(sx + LEFT_OFFSET + 30, sy, 30, 30), color);
        }

        private void DrawBlockPreview(int x, int y, Color colour, int size)
        {
            _spriteBatch.Draw(_block, new Rectangle(x + PREVIEW_OFFSET, y, size, size), colour);
        }

        private void DrawBarrier(int x)
        {
            var px = x * 30;
            for (int y = 0; y < 20; y++)
            {
                var py = y * 30;
                _spriteBatch.Draw(_block, new Rectangle(px + LEFT_OFFSET, py, 30, 30), Color.Gray);
            }
        }

        private void MoveLeft()
        {
            var (min, max) = _piece.GetMinAndMaxX();

            if (_piece.X - 1 + min < 0)
                return;

            if (BlockedHorizontally(_piece, -1))
                return;

            _piece.X--;
        }

        private void MoveRight()
        {
            var (min, max) = _piece.GetMinAndMaxX();

            if (_piece.X + 2 + max > 10)
                return;

            if (BlockedHorizontally(_piece))
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

        private void StampPiece(CurrentPiece piece)
        {
            var shape = piece.GetRotatedShape();
            for (var y = 0; y < piece.Size; y++)
            {
                for (var x = 0; x < piece.Size; x++)
                {
                    var boardIndex = (piece.Y + y - 1) * 10 + x + piece.X;
                    var pieceIndex = y * piece.Size + x;
                    if (boardIndex < 10 * 20 && shape[pieceIndex] != 0)
                    {
                        _board[boardIndex] = PieceColour.GetIndex(piece.Colour);
                    }
                }
            }
        }

        private void ResetBoard()
        {
            if (_board == null)
            {
                _board = new int[10 * 20];
            }

            Array.Fill(_board, 0);
        }

        private bool HasCompletedRow() => GetCompletedRow() >= 0;

        private int GetCompletedRow()
        {
            // Go UP because rows get completed from the bottom
            for (var y = 19; y >= 0; y--)
            {
                var count = 0;
                for (var x = 0; x < 10; x++)
                {
                    var index = y * 10 + x;
                    if (_board[index] != 0) count++;
                }

                if (count == 10) return y;
            }

            return -1;
        }

        private int GetPointsForDrops(int level, int numRows)
        {
            // 40 * (n + 1)	100 * (n + 1)	300 * (n + 1)	1200 * (n + 1)
            var pts = 0;
            switch (numRows)
            {
                case 2:
                    pts = TWO_LINE_PTS; break;
                case 3:
                    pts = THREE_LINE_PTS; break;
                case 4:
                    pts = FOUR_LINE_PTS; break;
                default:
                    pts = ONE_LINE_PTS; break;
            }

            return pts * (level + 1);
        }
    }
}
