using System;
using System.Collections.Generic;

namespace Soduko
{
    public class SodukoBoard1d : IGet9List
    {
        private SodukoBoard3d m_Board;
        private int m_Dimension;
        private int m_Index1;
        private int m_Index2;

        public SodukoBoard1d(SodukoBoard3d board, int dimension, int index1, int index2)
        {
            if (dimension != 0 && dimension != 1 && dimension != 2)
                throw new ArgumentOutOfRangeException();
            index1.VerifyIndex();
            index2.VerifyIndex();

            m_Board = board;
            m_Dimension = dimension;
            m_Index1 = index1;
            m_Index2 = index2;
        }

        public SodukoSquare this[int x]
        {
            get
            {
                if (m_Dimension == 0)
                    return m_Board.Board[x, m_Index1, m_Index2];
                if (m_Dimension == 1)
                    return m_Board.Board[m_Index1, x, m_Index2];
                if (m_Dimension == 2)
                    return m_Board.Board[m_Index1, m_Index2, x];

                throw new ArgumentOutOfRangeException();
            }
        }

        public List<SodukoSquare> To9List()
        {
            var res = new List<SodukoSquare>();
            for (int i = 0; i < 9; i++)
            {
                res.Add(this[i]);
            }
            return res;
        }
    }
}
