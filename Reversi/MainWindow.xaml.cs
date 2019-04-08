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

namespace Reversi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReversiEngine engine = new ReversiEngine(1);

        private SolidColorBrush[] colors = { Brushes.Ivory, Brushes.Green, Brushes.Sienna };
        string[] playersNames = { "", "zielony", "brązowy" };

        private Button[,] board;

        private bool BoardInitialized => board[engine.BoardWidth - 1, engine.BoardHeight - 1] != null;

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

        void ClickingBoardField(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            FieldCoordinate coordinate = (FieldCoordinate)clickedButton.Tag;
            int clickedHorizontal = coordinate.Horizontal;
            int clickedVertical = coordinate.Vertical;

            //doing movement
            int rememberedPlayerNumber = engine.PlayerNumberMakingNextMove;
            if (engine.PutStone(clickedHorizontal, clickedVertical)) AgreeBoardContent();
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
    }
}
