using System;
using System.Collections.Generic;

namespace Soduko
{
    public class SodukoSubBoard : IGet9List
    {
        private SodukoBoard3d m_Board;
        private int m_Dimension;
        private int m_Index;
        private int m_SubIndex1;
        private int m_SubIndex2;

        public SodukoSubBoard(SodukoBoard3d board, int dimension, int index, int subIndex1, int subIndex2)
        {
            subIndex1.VerifySmallIndex();
            subIndex2.VerifySmallIndex();
            dimension.VerifySmallIndex();
            index.VerifyIndex();

            m_Board = board;
            m_Dimension = dimension;
            m_Index = index;
            m_SubIndex1 = subIndex1;
            m_SubIndex2 = subIndex2;
        }

        public List<SodukoSquare> To9List()
        {
            List<SodukoSquare> res = new List<SodukoSquare>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    res.Add(GetSquare(i,j));
                }
            }
            return res;
        }

        private SodukoSquare GetSquare(int i, int j)
        {
            switch (m_Dimension)
            {
                case 0:
                    return m_Board.Board[m_Index, 3 * m_SubIndex1 + i, 3 * m_SubIndex2 + j];
                case 1:
                    return m_Board.Board[3 * m_SubIndex1 + i, m_Index, 3 * m_SubIndex2 + j];
                case 2:
                    return m_Board.Board[3 * m_SubIndex1 + i, 3 * m_SubIndex2 + j, m_Index];
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}
