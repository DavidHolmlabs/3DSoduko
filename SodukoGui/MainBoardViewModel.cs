using GalaSoft.MvvmLight.Command;
using Soduko;
using SodukoGui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace SodukoGui
{
    internal class MainBoardViewModel : INotifyPropertyChanged
    {
        private readonly string mainEndpoint = $"http://soduko3d.azurewebsites.net/api/";
        private MainGame mainGame;
        private SodukoBoard2d displayedBoard;
        private Thread workerThread;
        private Solver solver = new Solver();

        public MainBoardViewModel()
        {
            mainGame = new MainGame();
            SolveCommand = new RelayCommand(DispatchSolve);
            StartCommand = new RelayCommand<string>(FetchSolution);
            StopSolveCommand = new RelayCommand(StopSolve);
            VerifyCommand = new RelayCommand(Verify);
            SelectRowCommand = new RelayCommand<string>(SelectIndex);
            SelectColumnCommand = new RelayCommand<string>(SelectIndex);

            displayedBoard = mainGame.Get2dBoard(0, 0);
            mainGame.Deserialize(Settings.Default.CurrentSolution);
            SetHeader();
        }

        public ICommand SolveCommand { get; }
        public ICommand StopSolveCommand { get; set; }
        public ICommand StartCommand { get; }
        public ICommand VerifyCommand { get; }
        public ICommand SelectRowCommand { get; }
        public ICommand SelectColumnCommand { get; }

        public bool Enabled { get; private set; } = true;
        public string Header { get; private set; }
        public bool Solving { get; set; }

        /// <summary>
        /// 0-based index of dimensions
        /// </summary>
        public short FixedDimension
        {
            get => _fixedDimension;
            set
            {
                _fixedDimension = value;
                UpdateAllModels();
                SetHeader();
            }
        }

        /// <summary>
        /// 0-based index of level
        /// </summary>
        public short FixedIndex
        {
            get => _fixedIndex;
            set
            {
                _fixedIndex = value;
                UpdateAllModels();
                SetHeader();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SelectIndex(string index)
        {
            if (_fixedDimension == 0)
                _fixedDimension = 1;
            else if (_fixedDimension == 1)
                _fixedDimension = 0;
            else if (_fixedDimension == 2)
                _fixedDimension = 0;
            FixedIndex = (short)(short.Parse(index) - 1);
            UpdateAllModels();
            SetHeader();
        }

        private void SetHeader()
        {
            if (FixedDimension == 0)
                Header = $"Showning level {FixedIndex + 1} along X-dimension";
            if (FixedDimension == 1)
                Header = $"Showning level {FixedIndex + 1} along Y-dimension";
            if (FixedDimension == 2)
                Header = $"Showning level {FixedIndex + 1} along Z-dimension";
        }

        private void Verify()
        {
            VerifyViaCloud();

            foreach (var singleSquareViewModels in SingleSquareViewModels)
            {
                singleSquareViewModels.Value.FirePropertyChanged(nameof(singleSquareViewModels.Value.Valid));
            }
        }

        private void VerifyViaCloud()
        {
            WebRequest request = WebRequest.Create(mainEndpoint + "VerifySoduko");
            request.Method = "POST";
            string postData = mainGame.Serialize();
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            mainGame.Deserialize(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();
        }

        private void StopSolve()
        {
            solver.Stop();
        }

        private async void FetchSolution(object difficulty)
        {
            if (Solving) return;
            Enabled = false;

            int emptySquares = 9 * 9 * 9 - 400;
            if (difficulty.ToString() == "1")
                emptySquares = 9 * 9 * 9 - 400;
            if (difficulty.ToString() == "2")
                emptySquares = 9 * 9 * 9 - 300;
            if (difficulty.ToString() == "3")
                emptySquares = 9 * 9 * 9 - 200;

            string endpoint = mainEndpoint + $"GetInitialBoard?emptySquares={emptySquares}";

            WebRequest request = WebRequest.Create(endpoint);
            using (WebResponse response = await request.GetResponseAsync())
            {
                Stream dataStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string responseFromServer = await reader.ReadToEndAsync();
                    mainGame.Deserialize(responseFromServer.Replace("\\", ""));
                }
            }

            foreach (var singleSquareViewModels in SingleSquareViewModels)
                singleSquareViewModels.Value.FireAll();

            Enabled = true;
        }

        private void DispatchSolve()
        {
            workerThread = new Thread(Solve);
            workerThread.Start();
            Solving = true;
        }

        private void Solve()
        {
            solver.FillRecursively(mainGame);
            Solving = false;

            foreach (var singleSquareViewModels in SingleSquareViewModels)
                singleSquareViewModels.Value.FireAll();
        }

        internal void Close()
        {
            solver.Stop();
            Settings.Default.CurrentSolution = mainGame.Serialize();
            Settings.Default.Save();
        }

        private Dictionary<Tuple<int, int>, SingleSquareViewModel> SingleSquareViewModels = new Dictionary<Tuple<int, int>, SingleSquareViewModel>();
        private short _fixedDimension;
        private short _fixedIndex;

        internal SingleSquareViewModel GetSingleSquareViewModel(int i, int j)
        {
            var vm = new SingleSquareViewModel();
            vm.SetModel(displayedBoard[i, j]);
            SingleSquareViewModels[new Tuple<int, int>(i, j)] = vm;
            return vm;
        }

        private void UpdateAllModels()
        {
            displayedBoard = mainGame.Get2dBoard(FixedDimension, FixedIndex);

            foreach (var item in SingleSquareViewModels)
            {
                item.Value.SetModel(displayedBoard[item.Key.Item1, item.Key.Item2]);
            }
        }
    }
}