using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Soduko
{
    [DebuggerDisplay("2D board D:{m_Dimension}, I:{m_Index}")]
    public class SodukoBoard2d
    {
        private SodukoBoard3d m_Board;
        int m_Dimension;
        int m_Index;

        public SodukoBoard2d(SodukoBoard3d mainBoard, int dimension, int index)
        {
            m_Board = mainBoard;
            m_Dimension = dimension;
            m_Index = index;
        }

        public SodukoSquare this[int x, int y]
        {
            get { return getValue(x, y); }
        }

        private SodukoSquare getValue(int x, int y)
        {
            if (m_Dimension == 0)
                return m_Board.Board[m_Index, x, y];
            if (m_Dimension == 1)
                return m_Board.Board[x, m_Index, y];
            if (m_Dimension == 2)
                return m_Board.Board[x, y, m_Index];

            throw new ArgumentOutOfRangeException();
        }

        internal string GetJson()
        {
            SodukoSquare[,] tmp = new SodukoSquare[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tmp[i, j] = this[i, j];
                }
            }

            string res = JsonConvert.SerializeObject(tmp);
            return res;
        }

        internal void LoadJson(string layerjson)
        {
            var newboard = JsonConvert.DeserializeObject<SodukoSquare[,]>(layerjson);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    this[i, j].Value = newboard[i, j].Value;
                }
            }
        }
    }
}
