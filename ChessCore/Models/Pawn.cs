namespace ChessCore.Models
{
    public class Pawn
    {
        public string PawnImageSrc { get; set; }

        public int Index { get; set; }
        public string PawnColor { get; set; }

        public bool ImageIsExist { get; set; }

        public string SelectedClass { get; set; } = "";

        public Pawn(int index, string pawnImageSrc, bool imageIsExist, string pawnColor)
        {
            PawnImageSrc = pawnImageSrc;
            Index = index;
            ImageIsExist = imageIsExist;
            PawnColor = pawnColor;

        }

    }

}
