using Soduko;
using System;
using System.Diagnostics;

namespace Soduko
{
    public class Solver
    {
        private bool stopping = false;

        public bool FillRecursively(MainGame matrix)
        {
            bool success = true;
            DateTime start = DateTime.Now;
            success = SolveRecursevly(matrix, 0);
            TimeSpan elapsed = DateTime.Now - start;
            Debug.WriteLine($"Elapsed minutes: {elapsed.TotalMinutes}, Success: {success}");
            return success;
        }

        private bool SolveRecursevly(MainGame matrix, int depth)
        {
            if (stopping)
                return false;

            string originalState = matrix.Serialize();
            FillOnes(matrix);

            if (EmptySquares(matrix) == 0)
            {
                return true;
            }

            var firstdeadSquares = matrix.board.FindFirstSquare(square =>
            {
                if (square.Value == SodukoSet.EmptyValue && square.GetFreeValues().Count == 0)
                    return true;
                else
                    return false;
            });

            if (firstdeadSquares != null)
            {
                matrix.Deserialize(originalState);
                return false;
            }

            SodukoSquare next = matrix.board.FindFirstSquare(square =>
            {
                return square.Value == SodukoSet.EmptyValue;
            });

            var possiblevalues = next.GetFreeValues();

            foreach (string testValue in possiblevalues)
            {
                next.Value = testValue;
                int empty = EmptySquares(matrix);
                if (SolveRecursevly(matrix, depth + 1))
                    return true;
            }

            matrix.Deserialize(originalState);
            return false;
        }

        public void Stop()
        {
            stopping = true;
        }

        private int FillOnes(MainGame matrix)
        {
            int updatedThisRound = 0;
            int totalUpdates = 0;

            do
            {
                updatedThisRound = matrix.board.IterateBoard(square =>
                {
                    if (square.Value != SodukoSet.EmptyValue)
                        return 0; ;
                    var freeValues = square.GetFreeValues();
                    if (freeValues.Count == 1)
                    {
                        square.Value = freeValues[0];
                        return 1;
                    }
                    return 0;
                });
                totalUpdates += updatedThisRound;
            } while (updatedThisRound != 0);

            return totalUpdates;
        }

        public int EmptySquares(MainGame matrix)
        {
            return matrix.board.IterateBoard(square =>
            {
                if (square.Value == SodukoSet.EmptyValue)
                    return 1;
                else
                    return 0;
            });
        }
    }
}
