using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace X0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            buttons = new Button[,]
            {
                {btn00, btn01, btn02},
                {btn10, btn11, btn12},
                {btn20, btn21, btn22},
            };

            NewGame();
        }

        private bool isPlayerTurn = true;
        private char[,] board = new char[3, 3];
        private Button[,] buttons;
        private botLevel level = botLevel.Medium;
        private enum botLevel
        {
            Easy, Medium, Hard
        }

        private void NewGame()
        {
            if (buttons == null) return;

            isPlayerTurn = true;
            board = new char[3, 3];

            foreach (Button b in buttons)
            {
                b.Content = "";
                b.IsEnabled = true;
                b.FontSize = 32;
                b.Background = Brushes.LightGray;
            }

            Status.Content = "Ваш ход (X)";
            Status.Foreground = Brushes.Black;
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void btnSelectLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            level = (botLevel)btnSelectLevel.SelectedIndex;
            NewGame();
        }

        private void Click_cell(object sender, RoutedEventArgs e)
        {
            if (!isPlayerTurn) return;
            Button btn = (Button)sender;
            GetPosition(btn, out int r, out int c);

            if (board[r, c] != '\0') return;

            Move(r, c, 'X');

            if (CheckWin('X'))
            {
                EndGame("Вы победили", Brushes.Green);
                return;
            }

            if (IsBoardFull()) 
            {
                EndGame("Ничья", Brushes.Aqua);
                return;
            }

            isPlayerTurn = false;
            Status.Content = "Ход противника (O)";

            BotMove();
        }


        private void GetPosition(Button btn, out int r, out int c)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (buttons[i, j] == btn)
                    {
                        r = i; c = j;
                        return;
                    }
            r = c = 0;
        }

        private void Move(int r, int c, char simbol)
        {
            board[r, c] = simbol;
            buttons[r, c].Content = simbol.ToString();
            buttons[r, c].IsEnabled = false;
        }

        private bool CheckWin(char simbol) 
        {
            for (int i = 0; i < 3; i++)
                if (board[i, 0] == simbol && board[i, 1] == simbol && board[i, 2] == simbol) return true;

            for (int j = 0; j < 3; j++)
                if (board[0, j] == simbol && board[1, j] == simbol && board[2, j] == simbol) return true;

            return (board[0, 0] == simbol && board[1, 1] == simbol && board[2, 2] == simbol)
                || (board[0, 2] == simbol && board[1, 1] == simbol && board[2, 0] == simbol);
        }

        private bool IsBoardFull() 
        {
            foreach (char c in board)
                if (c == '\0') return false;
            return true;
        }

        private void BotMove()
        {
            int r = -1;
            int c = -1;

            switch (level)
            {
                case botLevel.Easy:
                    (r, c) = RandomMove();
                    break;

                case botLevel.Medium:
                    if (new Random().Next(2) == 0)
                        (r, c) = RandomMove();
                    else
                        (r, c) = BestMove(2);
                    break;

                case botLevel.Hard:
                    (r, c) = BestMove(9);
                    break;
            }

            Move(r, c, 'O');

            if (CheckWin('O')) 
            {
                EndGame("Противник победил", Brushes.Red);
                return;
            }

            if (IsBoardFull()) 
            {
                EndGame("Ничья", Brushes.Aqua);
                return;
            }

            isPlayerTurn = true;
            Status.Content = "Ваш ход (X)";
        }

        private (int, int) RandomMove()
        {
            Random rand = new Random();
            int r, c;
            do
            {
                r = rand.Next(3);
                c = rand.Next(3);
            } while (board[r, c] != '\0');
            return (r, c);
        }

        private (int, int) BestMove(int maxDepth)
        {
            int bestScore = int.MinValue;
            int br = 0, bc = 0;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] == '\0')
                    {
                        board[i, j] = 'O';
                        int score = Minimax(0, false, maxDepth);
                        board[i, j] = '\0';

                        if (score > bestScore)
                        {
                            bestScore = score;
                            br = i;
                            bc = j;
                        }
                    }

            return (br, bc);
        }

        private int Minimax(int depth, bool isMax, int maxDepth)
        {
            if (CheckWin('O')) return 1;
            if (CheckWin('X')) return -1;
            if (IsBoardFull() || depth >= maxDepth) return 0;

            int best = isMax ? int.MinValue : int.MaxValue;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] == '\0')
                    {
                        board[i, j] = isMax ? 'O' : 'X';
                        int score = Minimax(depth + 1, !isMax, maxDepth);
                        board[i, j] = '\0';

                        best = isMax
                            ? Math.Max(best, score)
                            : Math.Min(best, score);
                    }

            return best;
        }

        private void EndGame(string mes, Brush color)
        {
            Status.Content = mes;
            Status.Foreground = color;

            foreach (Button b in buttons)
            {
                b.IsEnabled = false;
            }
        }
    }
}
