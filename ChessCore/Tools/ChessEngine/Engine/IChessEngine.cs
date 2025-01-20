namespace ChessCore.Tools.ChessEngine.Engine
{
    public interface IChessEngine : IDisposable
    {
        NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 3);
    }
}
