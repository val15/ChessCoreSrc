namespace ChessCore.Tools.ChessEngine
{
    public class Move
    {
        public Move() { }
        public Move(int fromIndex, int toIndex)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }


}
