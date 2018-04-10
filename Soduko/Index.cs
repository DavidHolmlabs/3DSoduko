using System;

namespace Soduko
{
    public class Index
    {
        private readonly int Dimension1;
        private readonly int Dimension2;
        private readonly int Dimension3;
        public Index(int dimension1, int dimension2, int dimension3)
        {
            Dimension1 = dimension1;
            Dimension2 = dimension2;
            Dimension3 = dimension3;
        }

        public int X => Dimension1;
        public int Y => Dimension2;
        public int Z => Dimension3;

        internal static Index GetRandomNonEmpty(MainBackend matrix)
        {
            Index possible;
            do
            {
                possible = GetRandom();
            } while (matrix.board[possible].Value == SodukoSet.EmptyValue);
            return possible;
        }

        public static Index GetRandom()
        {
            Random rand = new Random();
            return new Index(rand.Next(9), rand.Next(9), rand.Next(9));
        }
    }
}