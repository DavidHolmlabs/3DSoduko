using GalaSoft.MvvmLight.Command;
using Soduko;
using SodukoGui.Properties;
using SodukoSolver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace SodukoGui
{
    internal class MainBoardViewModel : INotifyPropertyChanged
    {
        private MainBackend mainBackend;
        private SodukoBoard2d displayedBoard;
        private Thread solveThread;
        private MainSolver mainSolver = new MainSolver();

        public ICommand SolveCommand { get; }
        public ICommand StopSolveCommand { get; set; }
        public ICommand StartCommand { get; }
        public ICommand VerifyCommand { get; }
        public ICommand SetDimensionCommand { get; }
        public ICommand SetIndexCommand { get; }
        public ICommand SelectRowCommand { get; }
        public ICommand SelectColumnCommand { get; }

        public MainBoardViewModel()
        {
            mainBackend = new MainBackend();
            SolveCommand = new RelayCommand(DispatchSolve);
            StartCommand = new RelayCommand<string>(StartGame);
            StopSolveCommand = new RelayCommand(StopSolve);
            VerifyCommand = new RelayCommand(Verify);
            SetDimensionCommand = new RelayCommand<string>(SetDimension);
            SetIndexCommand = new RelayCommand<string>(SetIndex);
            SelectRowCommand = new RelayCommand<string>(SelectRow);
            SelectColumnCommand = new RelayCommand<string>(SelectColumn);

            displayedBoard = mainBackend.Get2dBoard(0, 0);
            mainBackend.Deserialize(Settings.Default.CurrentSolution);
            SetHeader();
        }

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

        public string Header { get; private set; }

        private void Verify()
        {
            if (mainBackend.Verify())
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
            mainSolver.Stop();
        }

        public bool Solving { get; set; }

        private void StartGame(string difficulty)
        {
            if (difficulty == "1")
                mainBackend.FetchSolution(9 * 9 * 9 - 400);
            else if (difficulty == "2")
                mainBackend.FetchSolution(9 * 9 * 9 - 300);
            else if (difficulty == "3")
                mainBackend.FetchSolution(9 * 9 * 9 - 200);
            else
                throw new ArgumentException("Expected aruments are 1, 2 or 3");

            foreach (var singleSquareViewModels in SingleSquareViewModels)
            {
                singleSquareViewModels.Value.FireAll();
            }
        }

        private void DispatchSolve()
        {
            solveThread = new Thread(solve);
            solveThread.Start();
            Solving = true;
        }

        private void solve()
        {
            mainSolver.FillRecursively(mainBackend);
            Solving = false;
        }

        internal void Close()
        {
            mainSolver.Stop();
            Settings.Default.CurrentSolution = mainBackend.Serialize();
            Settings.Default.Save();
        }


        /// <summary>
        /// 0-based index of dimensions
        /// </summary>
        public short FixedDimension { get; private set; }

        /// <summary>
        /// 0-based index of level
        /// </summary>
        public short FixedIndex { get; private set; }

        private Dictionary<Tuple<int, int>, SingleSquareViewModel> SingleSquareViewModels = new Dictionary<Tuple<int, int>, SingleSquareViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        internal SingleSquareViewModel GetSingleSquareViewModel(int i, int j)
        {
            var vm = new SingleSquareViewModel();
            vm.SetModel(displayedBoard[i, j]);
            SingleSquareViewModels[new Tuple<int, int>(i, j)] = vm;
            return vm;
        }

        private void UpdateAllModels()
        {
            displayedBoard = mainBackend.Get2dBoard(FixedDimension, FixedIndex);

            foreach (var item in SingleSquareViewModels)
            {
                item.Value.SetModel(displayedBoard[item.Key.Item1, item.Key.Item2]);
            }
        }
    }
}