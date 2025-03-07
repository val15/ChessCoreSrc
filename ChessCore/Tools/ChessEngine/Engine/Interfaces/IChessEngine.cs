namespace ChessCore.Tools.ChessEngine.Engine.Interfaces
{
    public interface IChessEngine : IDisposable
    {
        
        NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2);
        string GetName();
        string GetShortName();
    }
}
