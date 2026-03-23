using System;

namespace Monotris.Models
{
    internal static class PieceCollection
    {
        public static Piece I;
        public static Piece J;
        public static Piece L;
        public static Piece O;
        public static Piece S;
        public static Piece T;
        public static Piece Z;

        static PieceCollection()
        {
            O = new Piece(2,
                [1, 1, 1, 1], [[0, 1, 2, 3]]);

            I = new Piece(4,
                [
                    0,0,0,0,
                    0,0,0,0,
                    1,1,1,1,
                    0,0,0,0
                ],
                [
                    [
                        0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15
                    ],
                    [
                        12,8,4,0,13,9,5,1,14,10,6,2,15,11,7,3
                    ]
                ]);

            T = new Piece(3,
                [
                    0,0,0,
                    0,1,0,
                    1,1,1
                ],
                [
                    [
                        0,1,2,3,4,5,6,7,8
                    ],
                    [
                        6,3,0,7,4,1,8,5,2
                    ],
                    [
                        8,7,6,5,4,3,2,1,0
                    ],
                    [
                        2,5,8,1,4,7,0,3,6
                        ]
                ]);
            J = new Piece(3,
                [
                    0,0,0,
                    1,1,1,
                    0,0,1
                ],
                [
                    [
                        0,1,2,3,4,5,6,7,8
                    ],
                    [
                        6,3,0,7,4,1,8,5,2
                    ],
                    [
                        8,7,6,5,4,3,2,1,0
                    ],
                    [
                        2,5,8,1,4,7,0,3,6
                        ]
                ]);
            L = new Piece(3,
                [
                    0,0,0,
                    1,1,1,
                    1,0,0
                ],
                [
                    [
                        0,1,2,3,4,5,6,7,8
                    ],
                    [
                        6,3,0,7,4,1,8,5,2
                    ],
                    [
                        8,7,6,5,4,3,2,1,0
                    ],
                    [
                        2,5,8,1,4,7,0,3,6
                        ]
                ]);
            S = new Piece(3,
                [
                    0,0,0,
                    0,1,1,
                    1,1,0
                ],
                [
                    [
                        0,1,2,3,4,5,6,7,8
                    ],
                    [
                        6,3,0,7,4,1,8,5,2
                    ]
                ]);
            Z = new Piece(3,
                [
                    0,0,0,
                    1,1,0,
                    0,1,1
                ],
                [
                    [
                        0,1,2,3,4,5,6,7,8
                    ],
                    [
                        6,3,0,7,4,1,8,5,2
                    ]
                ]);
        }
    }
}
