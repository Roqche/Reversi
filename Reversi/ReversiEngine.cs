using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi
{
    public class ReversiEngine
    {
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }

        private int[,] board;
        public int PlayerNumberMakingNextMove { get; set; }

        private static int OpponentNumber(int playerNumber)
        {
            return (playerNumber == 1) ? 2 : 1;
        }

        private bool IsCorrectCoordinates(int horizontal, int vertical)
        {
            return horizontal >= 0 && horizontal < BoardWidth && vertical >= 0 && vertical < BoardHeight;
        }

        public int DownloadFieldCondition(int horizontal, int vertical)
        {
            if (!IsCorrectCoordinates(horizontal, vertical))
                throw new Exception("Nieprawidłowe współrzędne pola");
            return board[horizontal, vertical];
        }

        private void CleanBoard()
        {
            for (int i = 0; i < BoardWidth; i++)
                for (int j = 0; j < BoardHeight; j++)
                    board[i, j] = 0;

            int centerWidth = BoardWidth / 2;
            int centerHeight = BoardHeight / 2;
            board[centerWidth - 1, centerHeight - 1] = board[centerWidth, centerHeight] = 1;
            board[centerWidth, centerHeight - 1] = board[centerWidth - 1, centerHeight] = 2;
        }

        public ReversiEngine(int playerNumberStarting, int boardWidth = 8, int boardHeight = 8)
        {
            if (playerNumberStarting < 1 || playerNumberStarting > 2)
                throw new Exception("Nieprawidłowy numer gracza rozpoczynającego grę");

            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            board = new int[BoardWidth, BoardHeight];

            CleanBoard();

            PlayerNumberMakingNextMove = playerNumberStarting;

            CalculateFieldNumbers();
        }

        private void ChangeCurrentPlayer()
        {
            PlayerNumberMakingNextMove = OpponentNumber(PlayerNumberMakingNextMove);
        }

        protected int PutStone(int horizontal, int vertical, bool onlyTest)
        {
            //Are coordinates correct?
            if (!IsCorrectCoordinates(horizontal, vertical))
                throw new Exception("Nieprawidłowe współrzędne pola");

            //Is the field no longer occupied?
            if (board[horizontal, vertical] != 0) return -1;

            int howMuchFieldsTaken = 0;

            //Loop after 8 directions.
            for (int directHorizontal = -1; directHorizontal <= 1; directHorizontal++)
                for (int directVertical = -1; directVertical <=1; directVertical++)
                {
                    // forced omission when both variables are equal to 0
                    if (directHorizontal == 0 && directVertical == 0) continue;

                    // looking for player's stones in one of 8 directions
                    int i = horizontal;
                    int j = vertical;
                    bool foundOpponentStone = false;
                    bool foundPlayerStone = false;
                    bool foundEmptyField = false;
                    bool achievedEdgeBoard = false;
                    do
                    {
                        i += directHorizontal;
                        j += directVertical;
                        if (!IsCorrectCoordinates(i, j))
                            achievedEdgeBoard = true;
                        if (!achievedEdgeBoard)
                        {
                            if (board[i, j] == PlayerNumberMakingNextMove) foundPlayerStone = true;
                            if (board[i, j] == 0) foundEmptyField = true;
                            if (board[i, j] == OpponentNumber(PlayerNumberMakingNextMove)) foundOpponentStone = true;
                        }
                    } while (!(achievedEdgeBoard || foundPlayerStone || foundEmptyField));

                    // Checking the condition of correctness movement
                    bool puttingStoneIsPossible = foundOpponentStone && foundPlayerStone && !foundEmptyField;

                    // "reverse" stones in fulfilled condition
                    if (puttingStoneIsPossible)
                    {
                        int max_index = Math.Max(Math.Abs(i - horizontal), Math.Abs(j - vertical));
                        if (!onlyTest)
                        {
                            for (int index = 0; index < max_index; index++)
                                board[horizontal + index * directHorizontal, vertical + index * directVertical] = PlayerNumberMakingNextMove;
                        }

                        howMuchFieldsTaken += max_index - 1;
                    }
                } //end of loop after condition

            //changing player when the move is done
            if (howMuchFieldsTaken > 0 && !onlyTest)
                ChangeCurrentPlayer();

            CalculateFieldNumbers();

            //variable howMuchFieldTaken doesn't contain inserted stone
            return howMuchFieldsTaken;
        }

        public bool PutStone(int horizontal, int vertical)
        {
            return PutStone(horizontal, vertical, false) > 0;
        }

        private int[] fieldNumbers = new int[3]; //empty, player 1, player 2

        private void CalculateFieldNumbers()
        {
            for (int i = 0; i < fieldNumbers.Length; ++i) fieldNumbers[i] = 0;

            for (int i = 0; i < BoardWidth; i++)
                for (int j = 0; j < BoardHeight; j++)
                    fieldNumbers[board[i, j]]++;
        }

        private bool CanCurrentPlayerMakeMove()
        {
            int numberCorrectFields = 0;
            for (int i = 0; i < BoardWidth; ++i)
                for (int j = 0; j < BoardHeight; ++j)
                {
                    if (board[i, j] == 0 && PutStone(i, j, true) > 0)
                        numberCorrectFields++;
                }

            return numberCorrectFields > 0;
        }

        public void Pass()
        {
            if (CanCurrentPlayerMakeMove())
                throw new Exception("Gracz nie może oddać ruchu, jeżeli wykonanie ruchu jest możliwe");

            ChangeCurrentPlayer();
        }

        public enum SituationOnBoard
        {
            RuchJestMożliwy,
            BieżącyGraczNieMożeWykonaćRuchu,
            ObajGraczeNieMogąWykonaćRuchu,
            WszystkiePolaPlanszyZajęte
        }

        public SituationOnBoard InspectSituationOnBoard()
        {
            if (NumberOfEmptyFields == 0) return SituationOnBoard.WszystkiePolaPlanszyZajęte;

            bool canMakeMove = CanCurrentPlayerMakeMove();
            if (canMakeMove) return SituationOnBoard.RuchJestMożliwy;
            else
            {
                ChangeCurrentPlayer();
                bool canOpponentMakeMove = CanCurrentPlayerMakeMove();
                ChangeCurrentPlayer();
                if (canOpponentMakeMove)
                    return SituationOnBoard.BieżącyGraczNieMożeWykonaćRuchu;
                else return SituationOnBoard.ObajGraczeNieMogąWykonaćRuchu;
            }
        }

        public int PlayerNumberWithAdvantage
        {
            get
            {
                if (NumberOfPlayer1Fields == NumberOfPlayer2Fields) return 0;
                else return (NumberOfPlayer1Fields > NumberOfPlayer2Fields) ? 1 : 2;
            }
        }

        public int NumberOfEmptyFields { get => fieldNumbers[0]; }
        public int NumberOfPlayer1Fields { get => fieldNumbers[1]; }
        public int NumberOfPlayer2Fields { get => fieldNumbers[2]; }
    }
}
