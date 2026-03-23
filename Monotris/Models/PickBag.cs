using System;
using System.Collections.Generic;
using System.Linq;

namespace Monotris.Models
{
    internal static class PickBag
    {
        public static List<Piece> GetPieces() =>
         new Piece[] {
                PieceCollection.I,
                PieceCollection.J,
                PieceCollection.L,
                PieceCollection.O,
                PieceCollection.T,
                PieceCollection.S,
                PieceCollection.Z}.OrderBy(x => new Random()
                                  .Next(100))
                                  .ToList();
    }
}
