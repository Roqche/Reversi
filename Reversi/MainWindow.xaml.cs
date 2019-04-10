using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static Reversi.BoardForTwoPlayers;

namespace Reversi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReversiEngineAI engine = new ReversiEngineAI(1, 8, 8);
        private bool _gameAgainstComputer = true;
        private DispatcherTimer timer;

        string[] playersNames = { "", "zielony", "brązowy" };

        private void AgreeBoardContent()
        {
            for (int i = 0; i < engine.BoardWidth; i++)
                for (int j = 0; j < engine.BoardHeight; j++)
                {
                    boardControl.MarkMove(new FieldCoordinates(i, j), (FieldCondition)engine.DownloadFieldCondition(i, j));
                }

            buttonPlayerColor.Background = boardControl.BrushForCondition((FieldCondition)engine.PlayerNumberMakingNextMove);
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

        private void boardControl_ClickingField(object sender, BoardEventArgs e)
        {
            int clickedHorizontal = e.FieldCoordinates.Horizontal;
            int clickedVertical = e.FieldCoordinates.Vertical;

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
            SituationOnBoard situationOnBoard = engine.InspectSituationOnBoard();
            bool gameOver = false;
            switch (situationOnBoard)
            {
                case SituationOnBoard.BieżącyGraczNieMożeWykonaćRuchu:
                    MessageBox.Show("Gracz " + playersNames[engine.PlayerNumberMakingNextMove] + " zmuszony jest do oddania ruchu");
                    engine.Pass();
                    AgreeBoardContent();
                    break;
                case SituationOnBoard.ObajGraczeNieMogąWykonaćRuchu:
                    MessageBox.Show("Obaj gracze nie mogą wykonać ruchu");
                    gameOver = true;
                    break;
                case SituationOnBoard.WszystkiePolaPlanszyZajęte:
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
                    boardControl.IsEnabled = false;
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
            boardControl.IsEnabled = true;
            buttonPlayerColor.IsEnabled = true;
        }

        private FieldCoordinates? DetermineTheBestMove()
        {
            if (!boardControl.IsEnabled) return null;

            if (engine.NumberOfEmptyFields == 0)
            {
                MessageBox.Show("Nie ma już wolnych pól na planszy", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            try
            {
                int horizontal, vertical;
                engine.SuggestTheBestMove(out horizontal, out vertical);
                return new FieldCoordinates() { Horizontal = horizontal, Vertical = vertical };
            }
            catch
            {
                MessageBox.Show("Bieżący gracz nie może wykonać ruchu", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void MarkTheBestMove()
        {
            FieldCoordinates? fieldCoordinate = DetermineTheBestMove();
            if (fieldCoordinate.HasValue)
            {
                boardControl.MarkPrompt(fieldCoordinate.Value, (FieldCondition)engine.PlayerNumberMakingNextMove);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            AgreeBoardContent();
        }

        private void ButtonPlayerColor_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) MakeTheBestMove();
            else MarkTheBestMove();
        }

        private void MakeTheBestMove()
        {
            FieldCoordinates? fieldCoordinate = DetermineTheBestMove();
            if (fieldCoordinate.HasValue)
            {
                boardControl_ClickingField(boardControl, new BoardEventArgs() { FieldCoordinates = fieldCoordinate.Value });
            }
        }

        private void MenuItem_NewGameFor1Player_StartingComputer_Click(object sender, RoutedEventArgs e)
        {
            _gameAgainstComputer = true;
            Title = "Reversi - 1 gracz";
            PrepareBoardToNewGame(2);
            MakeTheBestMove();
        }

        private void MenuItem_NewGameFor1Player_Click(object sender, RoutedEventArgs e)
        {
            _gameAgainstComputer = true;
            Title = " Reversi - 1 gracz";
            PrepareBoardToNewGame(1);
        }

        private void MenuItem_NewGameFor2Players_Click(object sender, RoutedEventArgs e)
        {
            Title = "Reversi - 2 graczy";
            _gameAgainstComputer = false;
            PrepareBoardToNewGame(1);
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_MovePrompt_Click(object sender, RoutedEventArgs e)
        {
            MarkTheBestMove();
        }

        private void MenuItem_MoveMakedByComputer_Click(object sender, RoutedEventArgs e)
        {
            MakeTheBestMove();
        }

        private void MenuItem_GameRules_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("W grze Reversi gracze zajmują na przemian pola planszy, przejmując przy tym wszystkie pola przeciwnika znajdujące się między nowo zajętym polem a innymi polami gracza wykonującego ruch." +
                "Celem gry jest zdobycie większej liczby pól niż przeciwnik.\n" +
                "Gracz może zająć jedynie takie pole, które pozwoli mu przejąć przynajmniej jedno pole przeciwnika. Jeżeli takiego pola nie ma, musi oddać rych.\n" +
                "Gra kończy się w momencie zajęcia wszystkich pól lub gdy żaden z graczy nie może wykonać ruchu.\n",
                "Reversi - Zasady gry");
        }

        private void MenuItem_ComputerStrategy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Komputer kieruje się następującymi priorytetami (od najwyższego):\n" +
                "1. Ustawić pionek w rogu.\n" +
                "2. Unikać ustawienia pionka tuż przy rogu.\n" +
                "3. Ustawić pionek przy krawędzi planszy.\n" +
                "4. Unikać ustawienia pionka w wierszu lub kolumnie oddalonej o jedno pole od krawędzi planszy.\n" +
                "5. Wybierać pole, w wyniku którego zdobyta zostanie największa liczba pól przeciwnika.\n",
                "Reversi - Strategia komputera");
        }

        private void MenuItem_Website_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("");
        }

        private void MenuItem_Information_Click(object sender, RoutedEventArgs e)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show("Reversi Mobile\nwersja " + version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString() + "." + version.Revision.ToString() +
                "\n(C) Roqche 2019", "Reversi - Informacje o programie");
        }
    }
}
