using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi
{
    public class ReversiEngineAI : ReversiEngine
    {
        public  ReversiEngineAI(int playerNumberStarting, int boardWidth = 8, int boardHeight = 8):base(playerNumberStarting, boardWidth, boardHeight)
        {
        }

        private struct PossiblyMove : IComparable<PossiblyMove>
        {
            public int horizontal;
            public int vertical;
            public int priority;

            public PossiblyMove(int horizontal, int vertical, int priority)
            {
                this.horizontal = horizontal;
                this.vertical = vertical;
                this.priority = priority;
            }

            public int CompareTo(PossiblyMove otherMove)
            {
                return otherMove.priority - this.priority;
            }

        }

        public void SuggestTheBestMove(out int bestMoveHorizontal, out int bestMoveVertical)
        {
            //declaration of possibly move array
            List<PossiblyMove> possiblyMoves = new List<PossiblyMove>();

            int priorityJump = BoardWidth * BoardHeight;

            //searching for possibly moves
            for (int horizontal = 0; horizontal < BoardWidth; horizontal++)
                for (int vertical = 0; vertical < BoardHeight; vertical++)
                    if (DownloadFieldCondition(horizontal, vertical) == 0)
                    {
                        //number of occupied fields
                        int priority = PutStone(horizontal, vertical, true);
                        if (priority > 0){
                            PossiblyMove pm = new PossiblyMove(horizontal, vertical, priority);

                            //field in corner +
                            if ((pm.horizontal == 0 || pm.vertical == BoardWidth - 1) && (pm.vertical == 0 || pm.vertical == BoardHeight - 1))
                                pm.priority += priorityJump * priorityJump;

                            //field neighboring with corner on diagonals -
                            if ((pm.horizontal == 1 || pm.vertical == BoardWidth - 2) && (pm.vertical == 1 || pm.vertical == BoardHeight - 2))
                                pm.priority -= priorityJump * priorityJump;

                            //field neighboring with corner in vertical -
                            if ((pm.horizontal == 0 || pm.vertical == BoardWidth - 1) && (pm.vertical == 1 || pm.vertical == BoardHeight - 2))
                                pm.priority -= priorityJump * priorityJump;

                            //field neighboring with corner in horizontal -
                            if ((pm.horizontal == 1 || pm.vertical == BoardWidth - 2) && (pm.vertical == 0 || pm.vertical == BoardHeight - 1))
                                pm.priority -= priorityJump * priorityJump;

                            //field on edge +
                            if ((pm.horizontal == 0 || pm.vertical == BoardWidth - 1) && (pm.vertical == 0 || pm.vertical == BoardHeight - 1))
                                pm.priority += priorityJump;

                            //field neighboring with edge -
                            if ((pm.horizontal == 1 || pm.vertical == BoardWidth - 2) && (pm.vertical == 1 || pm.vertical == BoardHeight - 2))
                                pm.priority -= priorityJump;

                            possiblyMoves.Add(pm);
                        }
                    }

            //picking field with the highest priority
            if (possiblyMoves.Count() > 0)
            {
                possiblyMoves.Sort();
                bestMoveHorizontal = possiblyMoves[0].horizontal;
                bestMoveVertical = possiblyMoves[0].vertical;
            } else
            {
                throw new Exception("Brak możliwych ruchów");
            }
        }
    }
}
