using ChessCore.Tools;

namespace ChessCore.Models
{
    public class DetailsViewModel
    {
        public DateTime DateTimeNow { get; set; }

        public int InitialDuration { get; set; }
        public int WhiteTimeInSecond { get; set; }
        public int BlackTimeInSecond { get; set; }

        public int FromGridIndex { get; set; }

        public int ToGridIndex { get; set; }
        public Board MainBord { get; set; }

        public string[] Cases { get; set; }

        public List<Pawn> PawnCases { get; set; }

        public string CurrentTurn { get; set; }

        public string ComputerColor { get; set; }

        public string RevertWrapperClass { get; set; }

        public string WhiteScore { get; set; }

        public string BlackScore { get; set; }

        public List<string> HuntingBoardWhiteImageList { get; set; }
        public List<string> HuntingBoardBlackImageList { get; set; }
        public List<string> MovingList { get; set; }



        public string GetIsComputerTurn()
        {
            if (String.IsNullOrEmpty(CurrentTurn))
                return "0";
            if (String.IsNullOrEmpty(ComputerColor))
                return "0";

            if (CurrentTurn == ComputerColor)
                return "1";
            else
                return "0";
        }


        public Pawn GetPawn(int index)
        {
            return PawnCases[index];
        }


        public void Refresh(Board board)
        {
            PawnCases = new List<Pawn>();
            MainBord = board;
            Cases = MainBord.GetCases();
            var index = 0;

            foreach (var caseContain in Cases)
            {
                var imageIsExist = false;
                var pawnName = "";
                var pawnColor = "";
                if (caseContain.Contains("|"))
                {
                    imageIsExist = true;
                    var datas = caseContain.Split('|');
                    pawnName = datas[0];
                    pawnColor = datas[1];
                    if (pawnName == "B")
                    {
                        pawnName = "Bishop";

                    }
                    if (pawnName == "K")
                    {
                        pawnName = "King";
                    }
                    if (pawnName == "Q")
                    {
                        pawnName = "Queen";
                    }
                    if (pawnName == "T")
                    {
                        pawnName = "Rook";
                    }
                    if (pawnName == "C")
                    {
                        pawnName = "Knight";
                    }
                    if (pawnName == "P")
                    {
                        pawnName = "SimplePawn";
                    }




                    if (pawnColor == "B")
                    {
                        pawnColor = "Black";
                    }
                    else
                    {

                        pawnColor = "White";
                    }
                }
                //  / BishopBlack.png
                var pawnImageSrc = $"../../Images00/{pawnName}{pawnColor}.png";
                Pawn pawn = new Pawn(index, pawnImageSrc, imageIsExist, pawnColor[0].ToString());
                PawnCases.Add(pawn);
                index++;
            }

        }

        public DetailsViewModel(Board board, int whiteTimeInSecond, int blackTimeInSecond, int fromGridIndex = -1, List<int> posiblesMoveListSelectedPawn = null, int oldLocationIndex = -1, int newLocationIndex = -1)
        {
            WhiteTimeInSecond = whiteTimeInSecond;
            BlackTimeInSecond = blackTimeInSecond;

            HuntingBoardWhiteImageList = board.HuntingBoardWhiteImageList;
            HuntingBoardBlackImageList = board.HuntingBoardBlackImageList;
            MainUtils.HuntingBoardWhiteImageList = HuntingBoardWhiteImageList;
            MainUtils.HuntingBoardBlackImageList = HuntingBoardBlackImageList;

            MovingList = board.MovingList;
            board.CalculeScores();
            PawnCases = new List<Pawn>();
            MainBord = board;
            Cases = MainBord.GetCases();
            var index = 0;

            foreach (var caseContain in Cases)
            {
                var imageIsExist = false;
                var pawnName = "";
                var pawnColor = "";
                if (caseContain.Contains("|"))
                {
                    imageIsExist = true;
                    var datas = caseContain.Split('|');
                    pawnName = datas[0];
                    pawnColor = datas[1];
                    if (pawnName == "B")
                    {
                        pawnName = "Bishop";

                    }
                    if (pawnName == "K")
                    {
                        pawnName = "King";
                    }
                    if (pawnName == "Q")
                    {
                        pawnName = "Queen";
                    }
                    if (pawnName == "T")
                    {
                        pawnName = "Rook";
                    }
                    if (pawnName == "C")
                    {
                        pawnName = "Knight";
                    }
                    if (pawnName == "P")
                    {
                        pawnName = "SimplePawn";
                    }




                    if (pawnColor == "B")
                    {
                        pawnColor = "Black";
                    }
                    else
                    {

                        pawnColor = "White";
                    }
                }
                //  / BishopBlack.png
                var pawnImageSrc = $"../../Images/{pawnName}{pawnColor}.png";

                var shortcolor = "";
                if (!String.IsNullOrEmpty(pawnColor))
                    shortcolor = pawnColor[0].ToString();
                Pawn pawn = new Pawn(index, pawnImageSrc, imageIsExist, shortcolor);
                if (fromGridIndex == index)
                    pawn.SelectedClass = "bgYellow";
                if (posiblesMoveListSelectedPawn != null && fromGridIndex != -1)
                {

                    foreach (var item in posiblesMoveListSelectedPawn)
                    {
                        if (item == index)
                            pawn.SelectedClass = "bgOrange";
                    }
                    // if (posiblesMoveListSelectedPawn.Contains(index))
                    //   pawn.SelectedClass = "bgYellow";
                }

                //colocation de l'antien position et du nouveau
                if (index == oldLocationIndex || index == newLocationIndex)
                    pawn.SelectedClass = "bgYellow";
                PawnCases.Add(pawn);
                index++;
            }
            // if (Utils.MainBord == null)
            Utils.MovingList = MainBord.MovingList;


        }

    }

}
