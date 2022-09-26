using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Random aiGenerator = new Random();
        private readonly int[,] _offsets = {
            { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1,-1 }
        };

        private string _currentTurningCharacter;
        private int _completedTurns;
        private string[,] _gridCache;
        private bool _gameRunning;
        private bool _aiEnabled;

        public MainWindow() {
            InitializeComponent();
            _currentTurningCharacter = "X";
            _completedTurns = 0;
            _gridCache = new string[,] {
                { "", "", "" },
                { "", "", "" },
                { "", "", "" }
            };
            _gameRunning = true;
            _aiEnabled = false;
        }

        private void OnStartClick(object sender, RoutedEventArgs e) {
            if (!_gameRunning)
                CleanGame();
            Button button = (Button)sender;
            button.Visibility = Visibility.Hidden;
            AIToggle.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
            NextTurnLabel.Visibility = Visibility.Visible;
            _aiEnabled = (bool)AIToggle.IsChecked;
            NextTurnLabel.Content = $"{_currentTurningCharacter} következik";
        }

        private void OnTicTacToeClick(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;

            // Button already pressed
            if (button.Content != "" || !_gameRunning) {
                return;
            }

            int x = int.Parse(button.Name.Split('_')[1]), y = int.Parse(button.Name.Split('_')[2]);
            SetGridValue(x, y, _currentTurningCharacter);

            if (_aiEnabled) {
                if (!_gameRunning)
                    return;
                while (true) {
                    int i = aiGenerator.Next(0, 3), j = aiGenerator.Next(0, 3);
                    if (_gridCache[i, j] == "") {
                        SetGridValue(i, j, "O");
                        break;
                    }
                }
            } else {
                _currentTurningCharacter = _currentTurningCharacter == "X" ? "O" : "X";
                NextTurnLabel.Content = $"{_currentTurningCharacter} következik";
            }
        }

        private void CheckForWinner(string currentPlayer, int placedX, int placedY) {
            for (int i = 0; i < _offsets.GetLength(0); i++) {
                if (GetCharacterByOffset(placedX, placedY, new int[] { _offsets[i, 0], _offsets[i, 1] }) == currentPlayer) {
                    int[,] _rays = {
                        { _offsets[i, 0] * 2, _offsets[i, 1] * 2 },
                        { -_offsets[i, 0], -_offsets[i, 1] },
                        { -_offsets[i, 0] * 2, -_offsets[i, 1] * 2 },
                    };
                    for (int j = 0; j < _rays.GetLength(0); j++) {
                        if (GetCharacterByOffset(placedX, placedY, new int[] { _rays[j, 0], _rays[j, 1] }) == currentPlayer) {
                            // Won by someone
                            EndGame();
                            break;
                        }
                    }
                }
            }
        }

        private void EndGame() {
            // Stop the game
            _gameRunning = false;

            // Show Winner
            WinnerLabel.Content = $"{_currentTurningCharacter} győzött!";
            NextTurnLabel.Visibility = Visibility.Hidden;
            WinnerLabel.Visibility = Visibility.Visible;

            // Display Restart button
            StartButton.Content = "Új játék";
            StartButton.Visibility = Visibility.Visible;
        }

        private void CleanGame() {
            // Clean the buttons
            UIElementCollection tttButtons = MainGrid.Children;
            for (int i = 0; i < tttButtons.Count; i++) {
                ((Button)tttButtons[i]).Content = "";
            }

            // Hide winner text
            WinnerLabel.Visibility = Visibility.Hidden;

            // Reset variables
            _currentTurningCharacter = "X";
            _completedTurns = 0;
            _gridCache = new string[,] {
                { "", "", "" },
                { "", "", "" },
                { "", "", "" }
            };

            // Start the game
            _gameRunning = true;
        }

        /* HELPER */
        private string GetCharacterByOffset(int originX, int originY, int[] offset) {
            int x = originX + offset[0];
            int y = originY + offset[1];
            if (x < 0 || x > 2 || y < 0 || y > 2)
                return "";
            return _gridCache[originX + offset[0], originY + offset[1]];
        }

        private void SetGridValue(int x, int y, string value) {
            _gridCache[x, y] = value;
            object _gridButton = MainGrid.FindName($"ttt_{x}_{y}");
            if (_gridButton is Button) {
                ((Button)_gridButton).Content = value;
            }
            _completedTurns++;
            if (_completedTurns >= 5) {
                CheckForWinner(value, x, y);
            }
        }
    }
}
