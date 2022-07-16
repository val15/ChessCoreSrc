using ChessCore.Models;

namespace ChessCore.Tools
{
    public static class MainUtils
    {
        //pour éviter le bug: le cpu bouge plusieurs foix d'affilé
        public static int CpuCount { get; set; } = 0;
        public static int DeepLevel { get; set; }
        public static List<string>? CaseList { get; set; }
        public static string? ZipFileName { get; set; }
        public static string? ZipFilePath { get; set; }
        public static int InitialDuration { get; set; }
        public static int FromGridIndex { get; set; } = -1;

        public static int ToGridIndex { get; set; } = -1;

        public static MainPageViewModel VM { get; set; }

        public static string? CurrentTurnColor { get; set; }

        public static int TurnNumber { get; set; }
        public static string? CPUColor { get; set; }

        public static int MovingListIndex { get; set; } =-1; //pour les next et le preview
        public static List<string>? MovingList { get; set; }

        public static List<string>? HuntingBoardWhiteImageList { get; set; }//pour le print
        public static List<string>? HuntingBoardBlackImageList { get; set; }//pour le print


    }

}
