namespace ChessCore.Tools.ChessEngine
{
    public class NodeCE
    {
        public List<NodeCE> EquivalentBestNodeCEList { get; set; }
        public List<NodeCE> AllNodeCEList { get; set; } = new List<NodeCE>();
        public int Weight { get; set; }
        public int Level { get; set; }
        public string Colore { get; set; }
        public int FromIndex { get; set; } // Index de départ (0-63)
        public int ToIndex { get; set; } // Index d'arrivée (0-63)

        public string Location => GetPositionFromIndex(FromIndex); // Position d'origine en notation échiquier
        public string BestChildPosition => GetPositionFromIndex(ToIndex); // Position de destination en notation échiquier

        public BoardCE BoardCE { get; set; }
        public NodeCE MaxNode { get; set; }
        public TimeSpan ReflectionTime { get; set; }

        public NodeCE()
        {

        }

        public NodeCE(BoardCE boardCE, Move move, int weight, int level, TimeSpan reflectionTime= new TimeSpan())
        {
            BoardCE = boardCE;
            FromIndex = move.FromIndex;
            ToIndex = move.ToIndex;
            Weight = weight;
            Level = level;
            ReflectionTime = reflectionTime;
        }


        /// <summary>
        /// Convertit un index de l'échiquier (0-63) en position échiquier (ex : 0 => a8).
        /// </summary>
        public static string GetPositionFromIndex(int index)
        {
            if (index < 0 || index > 63)
                return null;

            char file = (char)('a' + index % 8); // 'a' à 'h'
            int rank = 8 - index / 8; // '8' à '1'

            return $"{file}{rank}";
        }

        public override string ToString()
        {
            return $"{Level}:   {Location} ({FromIndex}) => {BestChildPosition} ({ToIndex}) : {Weight} ({ReflectionTime.ToString(@"hh\:mm\:ss")})".ToUpper();
        }

    }

}
