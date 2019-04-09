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
using System.Windows.Threading;

namespace Reversi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReversiEngineAI engine = new ReversiEngineAI(1, 8, 8);

        private SolidColorBrush[] colors = { Brushes.Ivory, Brushes.Green, Brushes.Sienna };
        string[] playersNames = { "", "zielony", "brązowy" };

        private Button[,] board;

        private bool BoardInitialized => board[engine.BoardWidth - 1, engine.BoardHeight - 1] != null;

        private bool _gameAgainstComputer = true;
        private DispatcherTimer timer;

        private void AgreeBoardContent()
        {
            if (!BoardInitialized) return;

            for (int i = 0; i < engine.BoardWidth; i++)
                for (int j = 0; j < engine.BoardHeight; j++)
                {
                    board[i, j].Background = colors[engine.DownloadFieldCondition(i, j)];
                    board[i, j].Content = engine.DownloadFieldCondition(i, j).ToString();
                }

            buttonPlayerColor.Background = colors[engine.PlayerNumberMakingNextMove];
            numberFieldGreen.Text = engine.NumberOfPlayer1Fields.ToString();
            numberFieldBrown.Text = engine.NumberOfPlayer2Fields.ToString();
        }

        private struct FieldCoordinate
        {
            public int Horizontal, Vertical;
        }

        private static string FieldBadge(int horizontal, int vertical)
        {
            if (horizontal > 25 || vertical > 8) return "(" + horizontal.ToString() + "," + vertical.ToString() + ")";
            return "" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[horizontal] + "1234567890"[vertical];
        }

        void ClickingBoardField(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            FieldCoordinate coordinate = (FieldCoordinate)clickedButton.Tag;
            int clickedHorizontal = coordinate.Horizontal;
            int clickedVertical = coordinate.Vertical;

            //doing movement
            int rememberedPlayerNumber = engine.PlayerNumberMakingNextMove;
            if (engine.PutStone(clickedHorizontal, clickedVertical))
            {
                AgreeBoardContent();

                //list of moves
                switch (rememberedPlayerNumber)
                {
                    case 1: listOfMoveGreen.Items.Add(FieldBadge(clickedHorizontal, clickedVertical));
                        break;
                    case 2: listOfMoveBrown.Items.Add(FieldBadge(clickedHorizontal, clickedVertical));
                        break;
                }
                listOfMoveGreen.SelectedIndex = listOfMoveGreen.Items.Count - 1;
                listOfMoveBrown.SelectedIndex = listOfMoveBrown.Items.Count - 1;
            }

            //special situation
            ReversiEngine.SituationOnBoard situationOnBoard = engine.InspectSituationOnBoard();
            bool gameOver = false;
            switch (situationOnBoard)
            {
                case ReversiEngine.SituationOnBoard.BieżącyGraczNieMożeWykonaćRuchu:
                    MessageBox.Show("Gracz " + playersNames[engine.PlayerNumberMakingNextMove] + " zmuszony jest do oddania ruchu");
                    engine.Pass();
                    AgreeBoardContent();
                    break;
                case ReversiEngine.SituationOnBoard.ObajGraczeNieMogąWykonaćRuchu:
                    MessageBox.Show("Obaj gracze nie mogą wykonać ruchu");
                    gameOver = true;
                    break;
                case ReversiEngine.SituationOnBoard.WszystkiePolaPlanszyZajęte:
                    gameOver = true;
                    break;
            }

            if (gameOver)
            {
                int winnerNumber = engine.PlayerNumberWithAdvantage;
                if (winnerNumber != 0) MessageBox.Show("Wygrał gracz " + playersNames[winnerNumber], Title, MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Remis", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                if (MessageBox.Show("Czy rozpocząć grę od nowa?", "Reversi", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    PrepareBoardToNewGame(1, engine.BoardWidth, engine.BoardHeight);
                }
                else
                {
                    boardGrid.IsEnabled = false;
                    buttonPlayerColor.IsEnabled = false;
                }
            }
            else
            {
                if(_gameAgainstComputer && engine.PlayerNumberMakingNextMove == 2)
                {
                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
                        timer.Tick += (_sender, _e) => { timer.IsEnabled = false; MakeTheBestMove(); };
                    }
                    timer.Start();
                }
            }
        }

        private void PrepareBoardToNewGame(int playerNumberStarting, int boardWidth = 8, int boardHeight = 8)
        {
            engine = new ReversiEngineAI(playerNumberStarting, boardWidth, boardHeight);
            listOfMoveGreen.Items.Clear();
            listOfMoveBrown.Items.Clear();
            AgreeBoardContent();
            boardGrid.IsEnabled = true;
            buttonPlayerColor.IsEnabled = true;
        }

        private FieldCoordinate? DetermineTheBestMove()
        {
            if (!boardGrid.IsEnabled) return null;

            if (engine.NumberOfEmptyFields == 0)
            {
                MessageBox.Show("Nie ma już wolnych pól na planszy", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            try
            {
                int horizontal, vertical;
                engine.SuggestTheBestMove(out horizontal, out vertical);
                return new FieldCoordinate() { Horizontal = horizontal, Vertical = vertical };
            }
            catch
            {
                MessageBox.Show("Bieżący gracz nie może wykonać ruchu", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void MarkTheBestMove()
        {
            FieldCoordinate? fieldCoordinate = DetermineTheBestMove();
            if (fieldCoordinate.HasValue)
            {
                SolidColorBrush colorPrompt = colors[engine.PlayerNumberMakingNextMove].Lerp(colors[0], 0.5f);
                board[fieldCoordinate.Value.Horizontal, fieldCoordinate.Value.Vertical].Background = colorPrompt;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //division grid to rows and cols
            for (int i = 0; i < engine.BoardWidth; i++)
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int j = 0; j < engine.BoardHeight; j++)
                boardGrid.RowDefinitions.Add(new RowDefinition());

            //creating buttons
            board = new Button[engine.BoardWidth, engine.BoardHeight];
            for (int i = 0; i < engine.BoardWidth; i++)
                for (int j = 0; j < engine.BoardWidth; j++)
                {
                    Button button = new Button();
                    button.Margin = new Thickness(0);
                    boardGrid.Children.Add(button);
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    button.Tag = new FieldCoordinate { Horizontal = i, Vertical = j };
                    button.Click += new RoutedEventHandler(ClickingBoardField);
                    board[i, j] = button;
                }

            AgreeBoardContent();
        }

        private void ButtonPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) MakeTheBestMove();
            else MarkTheBestMove();
        }

        private void MakeTheBestMove()
        {
            FieldCoordinate? fieldCoordinate = DetermineTheBestMove();
            if (fieldCoordinate.HasValue)
            {
                Button button = board[fieldCoordinate.Value.Horizontal, fieldCoordinate.Value.Vertical];
                ClickingBoardField(button, null);
            }
        }

       
    }
}
