using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Soduko
{
    public class MainGame
    {
        public SodukoBoard3d board { get; set; }

        public Dictionary<Tuple<int, int>, SodukoBoard2d> SodukoParts = new Dictionary<Tuple<int, int>, SodukoBoard2d>();

        public MainGame()
        {
            board = new SodukoBoard3d();

            for (int dimension = 0; dimension < 3; dimension++)
            {
                for (int index = 0; index < 9; index++)
                {
                    SodukoParts.Add(new Tuple<int, int>(dimension, index), new SodukoBoard2d(board, dimension, index));
                }
            }
            Add1dVerifyers();
            AddSubSquares();
        }

        private void AddSubSquares()
        {
            for (int dimension = 0; dimension < 3; dimension++)
            {
                for (int index = 0; index < 9; index++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            SodukoSubBoard sodukoSubBoard = new SodukoSubBoard(board, dimension, index, i, j);
                            Verifyer verifyer = new Verifyer(sodukoSubBoard);
                            foreach (var square in sodukoSubBoard.To9List())
                                square.AddVerifyer(verifyer);
                        }
                    }
                }
            }
        }

        public async Task FetchSolutionAsync(int emptySquares)
        {
            await Task.Run(() => FetchSolution(emptySquares));
        }

        private void FetchSolution(int emptySquares)
        {
            board.Clear();
            string solution = System.Text.Encoding.UTF8.GetString(Properties.Resources.Soduko);
            Deserialize(solution);
            Scramble();
            Scramble();
            Scramble();
            RemoveValues(emptySquares);
            LockValues();
        }

        public string VerifyJson(string json)
        {
            Deserialize(json);
            Verify();
            return Serialize();
        }

        public bool Verify()
        {
            board.IterateBoard(square =>
            {
                square.Verify();
                return 1;
            });

            int unfinishedSquares = board.IterateBoard(square =>
             {
                 if (square.Valid == false  || square.Value == SodukoSet.EmptyValue)
                     return 1;
                 return 0;
             });

            return unfinishedSquares == 0;
        }

        private void LockValues()
        {
            board.IterateBoard(square =>
            {
                if (square.Value != SodukoSet.EmptyValue)
                {
                    square.Lock();
                    return 1;
                }
                return 0;
            });
        }

        private void RemoveValues(int v)
        {
            Debug.WriteLine($"Clearing {v} squares");
            for (int i = 0; i < v; i++)
            {
                Index randomeSquare = Index.GetRandomNonEmpty(this);
                board[randomeSquare].Value = SodukoSet.EmptyValue;

            }
        }

        private void Scramble()
        {
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                ScrambleDimension(i, random);
            }
        }

        private void ScrambleDimension(int dimension, Random random)
        {
            SubSwap(0, dimension, random);
            SubSwap(1, dimension, random);
            SubSwap(2, dimension, random);
        }

        private void SubSwap(int subsquare, int dimension, Random random)
        {
            int baseindex = subsquare * 3;
            if (random.Next(2) == 1)
                SwapLayers(baseindex, baseindex + 1, dimension);
            if (random.Next(2) == 1)
                SwapLayers(baseindex + 1, baseindex + 2, dimension);
            if (random.Next(2) == 1)
                SwapLayers(baseindex, baseindex + 2, dimension);
        }

        private void SwapLayers(int v1, int v2, int dimension)
        {
            Debug.WriteLine($"Swapping layer {v1} and {v2} in dimension {dimension}");
            var layer1 = Get2dBoard(dimension, v1);
            var layer2 = Get2dBoard(dimension, v2);

            string layer1json = layer1.GetJson();
            string layer2json = layer2.GetJson();

            layer1.LoadJson(layer2json);
            layer2.LoadJson(layer1json);

        }

        public void Deserialize(string sodukoJson)
        {
            if (string.IsNullOrEmpty(sodukoJson))
                return;
            try
            {
                var newboard = JsonConvert.DeserializeObject<SodukoSquare[,,]>(sodukoJson);
                board.Clear();
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        for (int k = 0; k < 9; k++)
                        {
                            board.Board[i, j, k].Value = newboard[i, j, k].Value;
                            board.Board[i, j, k].Valid = newboard[i, j, k].Valid;
                            if (newboard[i, j, k].Locked)
                                board.Board[i, j, k].Lock();
                        }
                    }
                }
            }
            catch (Exception)
            {
                board.Clear();
            }
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(board.Board);
        }

        private void Add1dVerifyers()
        {
            //Dim = 0
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Verifyer verifyer = new Verifyer(new SodukoBoard1d(board, 0, i, j));
                    for (int k = 0; k < 9; k++)
                    {
                        var square = board.Board[k, i, j];
                        square.AddVerifyer(verifyer);
                    }
                }
            }
            //Dim = 1
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Verifyer verifyer = new Verifyer(new SodukoBoard1d(board, 1, i, j));
                    for (int k = 0; k < 9; k++)
                    {
                        var square = board.Board[i, k, j];
                        square.AddVerifyer(verifyer);
                    }
                }
            }
            //Dim = 2
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Verifyer verifyer = new Verifyer(new SodukoBoard1d(board, 2, i, j));
                    for (int k = 0; k < 9; k++)
                    {
                        var square = board.Board[i, j, k];
                        square.AddVerifyer(verifyer);
                    }
                }
            }
        }

        public SodukoBoard2d Get2dBoard(int dimension, int index)
        {
            return SodukoParts[new Tuple<int, int>(dimension, index)];
        }
    }
}
