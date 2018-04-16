using GalaSoft.MvvmLight.Command;
using Soduko;
using SodukoGui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace SodukoGui
{
    internal class MainBoardViewModel : INotifyPropertyChanged
    {
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
            SetDimensionCommand = new RelayCommand<string>(SetDimension);
            SetIndexCommand = new RelayCommand<string>(SetIndex);
            SelectRowCommand = new RelayCommand<string>(SelectRow);
            SelectColumnCommand = new RelayCommand<string>(SelectColumn);

            displayedBoard = mainGame.Get2dBoard(0, 0);
            mainGame.Deserialize(Settings.Default.CurrentSolution);
            SetHeader();
        }

        public ICommand SolveCommand { get; }
        public ICommand StopSolveCommand { get; set; }
        public ICommand StartCommand { get; }
        public ICommand VerifyCommand { get; }
        public ICommand SetDimensionCommand { get; }
        public ICommand SetIndexCommand { get; }
        public ICommand SelectRowCommand { get; }
        public ICommand SelectColumnCommand { get; }

        public bool Enabled { get; private set; } = true;
        public string Header { get; private set; }
        public bool Solving { get; set; }

        /// <summary>
        /// 0-based index of dimensions
        /// </summary>
        public short FixedDimension { get; private set; }

        /// <summary>
        /// 0-based index of level
        /// </summary>
        public short FixedIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SelectColumn(string column)
        {
            if (FixedDimension == 0)
                FixedDimension = 1;
            else if (FixedDimension == 1)
                FixedDimension = 0;
            else if (FixedDimension == 2)
                FixedDimension = 0;
            FixedIndex = (short)(short.Parse(column) - 1);
            UpdateAllModels();
            SetHeader();
        }

        private void SelectRow(string row)
        {
            if (FixedDimension == 0)
                FixedDimension = 2;
            else if (FixedDimension == 1)
                FixedDimension = 2;
            else if (FixedDimension == 2)
                FixedDimension = 1;
            FixedIndex = (short)(short.Parse(row) - 1);
            UpdateAllModels();
            SetHeader();
        }

        private void SetIndex(string index)
        {
            FixedIndex = short.Parse(index);
            UpdateAllModels();
            SetHeader();
        }

        private void SetDimension(string dimension)
        {
            FixedDimension = short.Parse(dimension);
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
            if (mainGame.Verify())
                Header = "Congratulations! You solved the soduko!";
            else
                Header = "Sorry, not done yet.";

            foreach (var singleSquareViewModels in SingleSquareViewModels)
            {
                singleSquareViewModels.Value.FirePropertyChanged(nameof(singleSquareViewModels.Value.Valid));
            }
        }

        private void StopSolve()
        {
            solver.Stop();
        }

        private async void FetchSolution(object difficulty)
        {
            if (Solving) return;
            Enabled = false;

            if (difficulty.ToString() == "1")
                await mainGame.FetchSolutionAsync(9 * 9 * 9 - 400);
            else if (difficulty.ToString() == "2")
                await mainGame.FetchSolutionAsync(9 * 9 * 9 - 300);
            else if (difficulty.ToString() == "3")
                await mainGame.FetchSolutionAsync(9 * 9 * 9 - 200);
            else
                throw new ArgumentException("Expected aruments are 1, 2 or 3");

            foreach (var singleSquareViewModels in SingleSquareViewModels)
            {
                singleSquareViewModels.Value.FireAll();
            }

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
            {
                singleSquareViewModels.Value.FireAll();
            }
        }

        internal void Close()
        {
            solver.Stop();
            Settings.Default.CurrentSolution = mainGame.Serialize();
            Settings.Default.Save();
        }

        private Dictionary<Tuple<int, int>, SingleSquareViewModel> SingleSquareViewModels = new Dictionary<Tuple<int, int>, SingleSquareViewModel>();

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