using System;
using System.Collections.Generic;

namespace Monotris.Models
{
    internal class Piece
    {
        public int Size = 3;
        public int[] Shape;
        public readonly List<int[]> Rotations = new();
        public int CurrentRotation = 0;

        public Piece(Piece copy) : this(copy.Size, copy.Shape, copy.Rotations)
        {

        }

        public Piece(int size, int[] shape, IEnumerable<int[]> rotations)
        {
            Size = size;
            Shape = new int[shape.Length];
            Array.Copy(shape, Shape, shape.Length);

            if (rotations is null) return;

            foreach (var rotation in rotations)
            {
                var tmp = new int[rotation.Length];
                Array.Copy(rotation, tmp, rotation.Length);
                Rotations.Add(tmp);
            }
        }
    }
}
