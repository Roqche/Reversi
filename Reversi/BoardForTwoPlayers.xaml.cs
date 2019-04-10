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
    /// Interaction logic for BoardForTwoPlayers.xaml
    /// </summary>
    public partial class BoardForTwoPlayers : UserControl
    {
        #region Auxiliary Types
        public enum FieldCondition { Empty = 0, Player1 = 1, Player2 = 2 }

        public struct FieldCoordinates
        {
            public int Horizontal, Vertical;
            
            public FieldCoordinates(int horizontal, int vertical)
            {
                this.Horizontal = horizontal;
                this.Vertical = vertical;
            }
        }
        #endregion

        private int width, height;
        private FieldCondition[,] boardConditions;
        private Button[,] boardButtons;

        private void CreateBoard(int width, int height)
        {
            this.height = height;
            this.width = width;

            //division grid to rows and cols
            boardGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < width; i++)
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            boardGrid.RowDefinitions.Clear();
            for (int j = 0; j < height; j++)
                boardGrid.RowDefinitions.Add(new RowDefinition());

            //creating conditions array
            boardConditions = new FieldCondition[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    boardConditions[i, j] = FieldCondition.Empty;

            //creating buttons
            boardButtons = new Button[width, height];
            for (int i = 0; i<width; i++)
                for(int j = 0; j < height; j++)
                {
                    Button button = new Button();
                    button.Margin = new Thickness(0);
                    boardGrid.Children.Add(button);
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    button.Tag = new FieldCoordinates { Horizontal = i, Vertical = j };
                    button.Click += new RoutedEventHandler(
                        (s, e) =>
                        {
                            Button clickingButton = s as Button;
                            FieldCoordinates coordinates = (FieldCoordinates)clickingButton.Tag;
                            int clickingHorizontal = coordinates.Horizontal;
                            int clickingVertical = coordinates.Vertical;
                            OnClickingField(coordinates);
                        });
                    boardButtons[i, j] = button;
                }
            ChangeColorsAllButtons();
        }

        public BoardForTwoPlayers()
        {
            InitializeComponent();

            CreateBoard(8, 8);
        }

        public int Width
        {
            get => width;
            set
            {
                CreateBoard(value, height);
            }
        }

        public int Height
        {
            get => height;
            set
            {
                CreateBoard(width, value);
            }
        }

        #region Colors
        private SolidColorBrush brushEmptyField = Brushes.Ivory;
        private SolidColorBrush brushPlayer1 = Brushes.Green;
        private SolidColorBrush brushPlayer2 = Brushes.Sienna;

        public SolidColorBrush BrushForCondition(FieldCondition fieldCondition)
        {
            switch (fieldCondition)
            {
                default:
                case FieldCondition.Empty: return brushEmptyField;
                case FieldCondition.Player1: return brushPlayer1;
                case FieldCondition.Player2: return brushPlayer2;
            }
        }

        private void ChangeColorsAllButtons()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    boardButtons[i, j].Background = BrushForCondition(boardConditions[i, j]);
        }

        public Color ColorEmptyField
        {
            get => brushEmptyField.Color;
            set
            {
                brushEmptyField = new SolidColorBrush(value);
                ChangeColorsAllButtons();
            }
        }

        public Color ColorPlayer1
        {
            get => brushPlayer1.Color;
            set
            {
                brushPlayer1 = new SolidColorBrush(value);
                ChangeColorsAllButtons();
            }
        }

        public Color ColorPlayer2
        {
            get => brushPlayer2.Color;
            set
            {
                brushPlayer2 = new SolidColorBrush(value);
                ChangeColorsAllButtons();
            }
        }
        #endregion

        #region Changing fields condition
        public void MarkMove(FieldCoordinates fieldCoordinates, FieldCondition fieldCondition)
        {
            boardConditions[fieldCoordinates.Horizontal, fieldCoordinates.Vertical] = fieldCondition;
            boardButtons[fieldCoordinates.Horizontal, fieldCoordinates.Vertical].Background = BrushForCondition(fieldCondition);
        }

        public void MarkPrompt(FieldCoordinates fieldCoordinates, FieldCondition fieldCondition)
        {
            if (fieldCondition == FieldCondition.Empty)
                throw new Exception("Nie można zaznaczyć podpowiedzi dla stanu pustego pola");
            SolidColorBrush brushPrompt = BrushForCondition(fieldCondition).Lerp(brushEmptyField, 0.5f);
            boardButtons[fieldCoordinates.Horizontal, fieldCoordinates.Vertical].Background = brushPrompt;
        }
        #endregion

        #region Event
        public class BoardEventArgs : RoutedEventArgs
        {
            public FieldCoordinates FieldCoordinates;
        }

        public delegate void BoardEventHandler(object sender, BoardEventArgs e);

        public event BoardEventHandler ClickingField;

        protected virtual void OnClickingField(FieldCoordinates fieldCoordinates)
        {
            ClickingField?.Invoke(this, new BoardEventArgs
            {
                FieldCoordinates = fieldCoordinates
            });
        }
        #endregion
    }
}
