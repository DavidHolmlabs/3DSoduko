using System;

namespace Soduko
{
    public class SodukoBoard3d
    {
        public SodukoBoard3d()
        {
            Board = new SodukoSquare[9, 9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        Board[i, j, k] = new SodukoSquare(new Index(i, j, k));
                    }
                }
            }
        }

        public SodukoSquare[,,] Board { get; private set; }

        public int IterateBoard(Func<SodukoSquare, int> SodukoSquareFunk)
        {
            int res = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        res += SodukoSquareFunk(Board[i, j, k]);
                    }
                }
            }
            return res;
        }

        public SodukoSquare this[Index index]
        {
            get
            {
                return Board[index.X, index.Y, index.Z];
            }
            set
            {
                Board[index.X, index.Y, index.Z] = value;
            }
        }

        public SodukoSquare FindFirstSquare(Func<SodukoSquare, bool> SodukoSquareFunk)
        {
            Index index = FindFirstIndex(SodukoSquareFunk);
            if (index == null) return null;
            return this[index];
        }

        public Index FindFirstIndex(Func<SodukoSquare, bool> SodukoSquareFunk)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if (SodukoSquareFunk(Board[i, j, k]))
                            return new Index(i, j, k);
                    }
                }
            }
            return null;
        }

        internal void Clear()
        {
            IterateBoard(x => {
                x.Unlock();
                x.Value = SodukoSet.EmptyValue;
                return 1;
            });
        }
    }
}
