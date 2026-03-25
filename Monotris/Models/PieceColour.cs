using Microsoft.Xna.Framework;
using System;

namespace Monotris.Models
{
    internal class PieceColour
    {
        public static Color I = Color.Cyan;
        public static Color O = Color.Yellow;
        public static Color T = Color.Purple;
        public static Color S = Color.Green;
        public static Color Z = Color.Red;
        public static Color J = Color.Blue;
        public static Color L = Color.Orange;

        public static Color[] Colours = [Color.White, I, O, T, S, Z, J, L];

        public static int GetIndex(Color colour) => Array.IndexOf(Colours, colour);
    }
}
