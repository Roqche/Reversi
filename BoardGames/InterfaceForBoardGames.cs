namespace Reversi
{
    public enum SituationOnBoard
    {
        RuchJestMożliwy,
        BieżącyGraczNieMożeWykonaćRuchu,
        ObajGraczeNieMogąWykonaćRuchu,
        WszystkiePolaPlanszyZajęte
    }
    public interface IEngineGameForTwoPlayers
    {
        int BoardWidth { get; }
        int BoardHeight { get; }

        int PlayerNumberMakingNextMove { get; }
        int PlayerNumberWithAdvantage { get; }
        int DownloadFieldCondition(int horizontal, int vertical);

        int NumberOfEmptyFields { get; }
        int NumberOfPlayer1Fields { get; }
        int NumberOfPlayer2Fields { get; }

        void Pass();

        SituationOnBoard InspectSituationOnBoard();
    }

    public interface IEngineGameForOnePlayer : IEngineGameForTwoPlayers
    {
        void SuggestTheBestMove(out int theBestMoveHorizontal, out int theBestMoveVertical);
    }
}
