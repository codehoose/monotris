namespace Monotris.Models
{
    internal class CurrentPiece
    {
        private Piece _piece;

        public int X;
        public int Y;
        public int CurrentRotation;
        public Piece Piece
        {
            get => _piece;
            set
            {
                _piece = value;
                ResetRotation();
            }
        }

        public int Size => _piece?.Size ?? 0;

        public void ResetRotation()
        {
            CurrentRotation = 0;
        }

        public void RotateLeft()
        {
            CurrentRotation--;
            if (CurrentRotation < 0)
            {
                CurrentRotation += Piece.Rotations.Count;
            }
        }

        public void RotateRight()
        {
            CurrentRotation = (CurrentRotation + 1) % Piece.Rotations.Count;
        }

        public int[] GetRotatedShape()
        {
            var shapeArray = new int[Piece.Size * Piece.Size];
            var shape = Piece.Shape;
            var matrix = Piece.Rotations[CurrentRotation];
            var size = Piece.Size;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var index = y * size + x;
                    shapeArray[index] = shape[matrix[index]];
                }
            }

            return shapeArray;
        }

        public (int min, int max) GetMinAndMaxX()
        {
            var minimum = 10;
            var maximum = -1;
            var shape = Piece.Shape;
            var matrix = Piece.Rotations[CurrentRotation];
            var size = Piece.Size;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var index = y * size + x;
                    if (shape[matrix[index]] == 1)
                    {
                        if (x < minimum)
                        {
                            minimum = x;
                        }
                        if (x > maximum)
                        {
                            maximum = x;
                        }
                    }
                }
            }

            return (minimum, maximum);
        }

        public (int min, int max) GetMinAndMaxY()
        {
            var minimum = 10;
            var maximum = -1;
            var shape = Piece.Shape;
            var matrix = Piece.Rotations[CurrentRotation];
            var size = Piece.Size;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var index = y * size + x;
                    if (shape[matrix[index]] == 1)
                    {
                        if (y < minimum)
                        {
                            minimum = y;
                        }
                        if (y > maximum)
                        {
                            maximum = y;
                        }
                    }
                }
            }

            return (minimum, maximum);
        }
    }
}
