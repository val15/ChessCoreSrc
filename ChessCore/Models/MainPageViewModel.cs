﻿using ChessCore.Tools;
using ChessCore.Tools.ChessEngine;
using ChessCore.Tools.ChessEngine.Engine;
using ChessCore.Tools.ChessEngine.Engine.Interfaces;

namespace ChessCore.Models
{
    public class MainPageViewModel
    {

        public List<IChessEngine> Engines { get; set; } = new List<IChessEngine>();

        public IChessEngine SelectedEngine { get; set; } = new ChessEngine3();
        public IChessEngine SelectedWhiteEngine { get; set; } = new ChessEngine3();
        public IChessEngine SelectedBlackEngine { get; set; } = new ChessEngine3();

        public int[] Levels = [1, 2, 3, 4, 5, 6, 7];
        public int[] ReflectionTimesInMinute = [1, 2, 5, 10,15,30,60];
        public int SelectedReflectionTimeInMinute { get; set; }

        public int SelectedLevel { get; set; }
        public int SelectedWhiteLevel { get; set; }
        public int SelectedBlackLevel { get; set; }
        public bool IsFormLoander { get; set; } = false;

        public string FromGridIndex { get; set; }

        public string ToGridIndex { get; set; }
        public Board MainBord { get; set; }
        public BoardCE MainBordGPT { get; set; }

        public string[] Cases { get; set; }

        public List<Pawn> PawnCases { get; set; }

        public string RevertWrapperClass { get; set; }
        public List<string> MovingList { get; set; }

        private int _whiteScore;

        public int WhiteScore
        {
            get { return _whiteScore; }
            set
            {
                _whiteScore = value / 10;
            }
        }
        public string GetWhiteScoreString()
        {
            if (WhiteScore > 0)
                return $"+ {WhiteScore}";
            else
                return " ";

        }


        private int _blackScore;

        public int BlackScore
        {
            get { return _blackScore; }
            set
            {
                _blackScore = value / 10;
            }
        }

        public string GetBlackScoreString()
        {
            if (BlackScore > 0)
                return $"+ {BlackScore}";
            else
                return " ";

        }


        public List<string> HuntingBoardWhiteImageList { get; set; }
        public List<string> HuntingBoardBlackImageList { get; set; }



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

                var shortcolor = "";
                if (!String.IsNullOrEmpty(pawnColor))
                    shortcolor = pawnColor[0].ToString();
                Pawn pawn = new Pawn(index, pawnImageSrc, imageIsExist, shortcolor);
                PawnCases.Add(pawn);
                index++;
            }

        }

        public MainPageViewModel(Board board)
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
                var pawnImageSrc = $"../../Images/{pawnName}{pawnColor}.png";
                var shortcolor = "";
                if (!String.IsNullOrEmpty(pawnColor))
                    shortcolor = pawnColor[0].ToString();
                Pawn pawn = new Pawn(index, pawnImageSrc, imageIsExist, shortcolor);
                PawnCases.Add(pawn);
                index++;
            }


        }
    }

}
