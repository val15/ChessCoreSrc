using System.Diagnostics;

namespace ChessCore.Tools
{
    public static class Utils
    {

        #region CG and memories

        public static void GCColect()
        {
            WritelineAsync($"Memory used before collection: {Utils.SizeSuffix(GC.GetTotalMemory(false))}");
            GC.Collect();
            WritelineAsync($"Memory used before collection: {Utils.SizeSuffix(GC.GetTotalMemory(false))}");
        }
        public static async Task WritelineAsync(string text)
        {
            try
            {
                Debug.WriteLine(text);
                Console.WriteLine(text);


                //using (StreamWriter w = File.AppendText("log.txt"))
                //{

                //    await w.WriteLineAsync(text);

                //    w.Flush();
                //    Thread.Sleep(100);
                //}

            }
            catch (Exception)
            {

                throw;
            }


        }


        static readonly string[] SizeSuffixes =
             { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }
        #endregion

        public static int DeepLevel { get; set; } = 4;
        public static bool DeepLevelPrime { get; set; } = true;
        public static DateTime StartedProcessTime { get; set; }
        //Pour T41, on limite le temps de reflection, si le temps depasse le seul, on ne fait plus de verification, in Chess2Utils.TargetColorIsInChess() au niveau 4
        public static double LimitationForT41InMn { get; set; } = 1.5;


        public static string NavigationStoryCursor { get; set; } = "-";//07-07-2022
                                                                       //pour T07a et T07b
        public static Board MainBoard { get; set; }

        public static List<NodeChess2> NodeLoseList { get; set; } = new List<NodeChess2>();
        //  public static List<NodeChess2> NodeLoseList2 {get;set;} = new List<NodeChess2>();
        //  public static List<NodeChess2> NodeLoseList3 {get;set;} = new List<NodeChess2>();
        // public static NodeChess2 BestNode {get;set;} 
        private static int[] _evolutionPawnIndexBlack =
       {
      56,57,58,59,60,61,62,63
    };
        private static int[] _evolutionPawnIndexWhite =
        {
      0,1,2,3,4,5,6,7
    };
        public static int[] GetEvolutionPawnIndexWhite()
        {
            return _evolutionPawnIndexWhite;
        }
        public static int[] GetEvolutionPawnIndexBlack()
        {
            return _evolutionPawnIndexBlack;
        }

        /// <summary>
        /// Pour l'affichage dans l'historique
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string LineToSymbol(string line)
        {
            try
            {
                //51(P|W)>35(__) => symbole(index)>symbole(destIndex)
                var data = line.Split('>');
                var from = indexPanwToSymboleIndex(data[0]);
                var to = indexPanwToSymboleIndex(data[1]);
                return $"{from} > {to}";
            }
            catch (Exception ex)
            {

                WritelineAsync(ex.Message);
                return "ERROR";
            }
        }
        public static string indexPanwToSymboleIndex(string inText)//ex: 51(P|W) => ♙(51)
        {
            var data = inText.Split('(');
            var index = data[0];
            var panw = data[1];
            panw = panw.Replace(")", "");
            return $"{ToSympbol(panw)}({index})";
        }

        public static char ToSympbol(string inText)
        {
            try
            {
                switch (inText)
                {
                    case "P|W":
                        return '♙'; // Pion blanc
                    case "P|B":
                        return '♟'; // Pion noir
                    case "T|W":
                        return '♖'; // Tour blanche
                    case "T|B":
                        return '♜'; // Tour noire
                    case "C|W":
                        return '♘'; // Cavalier blanc
                    case "C|B":
                        return '♞'; // Cavalier noir
                    case "B|W":
                        return '♗'; // Fou blanc
                    case "B|B":
                        return '♝'; // Fou noir
                    case "Q|W":
                        return '♕'; // Reine blanche
                    case "Q|B":
                        return '♛'; // Reine noire
                    case "K|W":
                        return '♔'; // Roi blanc
                    case "K|B":
                        return '♚'; // Roi noir
                    default:
                        return ' '; // Case vide
                }


            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.Message);
                return ' ';
            }
        }



        private static string[] _coord = {
"a8","b8","c8","d8","e8","f8","g8","h8",
"a7","b7","c7","d7","e7","f7","g7","h7",
"a6","b6","c6","d6","e6","f6","g6","h6",
"a5","b5","c5","d5","e5","f5","g5","h5",
"a4","b4","c4","d4","e4","f4","g4","h4",
"a3","b3","c3","d3","e3","f3","g3","h3",
"a2","b2","c2","d2","e2","f2","g2","h2",
"a1","b1","c1","d1","e1","f1","g1","h1"
    };

        public static string ChangeLongNameToShortName(string longName)
        {
            var name = "P";
            //Pawn
            //SimplePawn => P
            //Knight => C
            //Bishop => B
            //Rook => T
            //Queen => Q
            //King => K

            //Color
            //Black => B
            //White => W
            switch (longName)
            {
                case "Knight":
                    name = "C";
                    break;
                case "Bishop":
                    name = "B";
                    break;
                case "Rook":
                    name = "T";
                    break;
                case "Queen":
                    name = "Q";
                    break;
                case "King":
                    name = "K";
                    break;
            }
            return name;
        }
        public static int GetIndexFromLocation(string index)
        {
            return _coord.ToList().IndexOf(index);
        }
        public static Board GenerateBoardFormPawnListString(List<string> pawns)
        {
            var board = new Board();

            //InsertPawn(0, "T", "B");
            // InsertPawn(1, "C", "B");
            // InsertPawn(2, "B", "B");

            foreach (var pawn in pawns)
            {
                //Rook;a1;White;False;False;False;False
                var datasString = pawn.Split(';');
                var location = datasString[1];
                if (String.IsNullOrEmpty(location))
                    continue;
                var index = _coord.ToList().IndexOf(location);
                if (index == -1)
                    continue;
                var color = datasString[2].ToString()[0].ToString();
                var longName = datasString[0];
                var name = "P";

                switch (longName)
                {
                    case "Knight":
                        name = "C";
                        break;
                    case "Bishop":
                        name = "B";
                        break;
                    case "Rook":
                        name = "T";
                        break;
                    case "Queen":
                        name = "Q";
                        break;
                    case "King":
                        name = "K";
                        break;
                }



                board.InsertPawn(index, name, color);
            }
            //  board.CalculeScores();
            board.PrintInDebug();
            return board;

        }
        /*
        public static List<NodeChess2> GetMimWeightChild(NodeChess2 parent)
        {
          //var minW = -9999;

          List<NodeChess2> GrandParentNodeChess2 = new List<NodeChess2>();
          GrandParentNodeChess2 = new List<NodeChess2>();
          foreach (var currentP in parent.ChildList)
          {
            GrandParentNodeChess2.Add(currentP);
            if (currentP.ChildList == null)
              return GrandParentNodeChess2;
           GrandParentNodeChess2.AddRange( GetMimWeightChild(currentP));
          }
          return GrandParentNodeChess2;
        }
        */




        public static List<string> MovingList { get; set; }

        // public static Board MainBord { get; set; }
        public static int GetKing(Board board, string color)
        {
            var kingIndex = board.GetCases().ToList().IndexOf($"K|{color}");
            return kingIndex;
        }
        /*tsiry;07-12-2021
         * */
        public static bool KingIsOut(Board board, string color)
        {
            var kingIndex = board.GetCases().ToList().IndexOf($"K|{color}");
            if (kingIndex == -1)
                return true;
            return false;
        }


        /*12-11-2021
         * pour T80
         * */
        public static bool KingIsMenaced(Board board, string kingColor)
        {
            if (board == null)
                return false;
            var kingIndex = board.GetCases().ToList().IndexOf($"K|{kingColor}");
            if (IsMenaced(kingIndex, board, ComputerColor))
                return true;
            return false;
        }

        public static bool ComputerKingIsMenaced(Board board)
        {
            return KingIsMenaced(board, ComputerColor);
        }


        /*tsiry;04-01-2022
         * */


        public static bool IsMenaced(int toIndex, Board inBoard, string menacedColor)
        {
            /*if (toIndex == -1)
              return false;*/
            //if faut cloner si non on affecte le inBoard
            //IL FAUT CLONER inBoard POUR NE PAS L'AFFECTER
            var cloneBoard = Clone(inBoard);



            var opinionColor = "W";
            if (menacedColor == "W")
                opinionColor = "B";
            //dans le cas la case de déstination contien un pion adverse, on l'enleve
            if (cloneBoard.GetCases()[toIndex].Contains("|"))
                cloneBoard.GetCases()[toIndex] = "__";
            var opinionIndexs = cloneBoard.GetCasesIndexForColor(opinionColor);
            foreach (var index in opinionIndexs)
            {

                var pawnName = cloneBoard.GetPawnShortNameInIndex(index);
                if (pawnName != "P")
                {
                    var possibleMoves = cloneBoard.GetPossibleMoves(index, 0).Select(x => x.Index);
                    if (possibleMoves.Contains(toIndex))
                        return true;
                }
                else
                {
                    //POUR LE SIMPLE PION,ON ENLEVER LE HORIZONTAL ET ON AJOUTE LES DIAGONAUX
                    if (pawnName == "P")
                    {
                        var possibleMoves = new List<int>();

                        var caseColor = cloneBoard.GetPawnColorNameInIndex(index);

                        var indexInTab64 = Utils.Tab64[index];
                        //si B
                        var toAddList = new List<int>();
                        var sing = -1;
                        if (caseColor == "B")
                            sing = 1;

                        //Diagonal for opinion
                        var toAddOpinionListList = new List<int>();
                        toAddOpinionListList.Add((10 * sing) + 1);
                        toAddOpinionListList.Add((10 * sing) - 1);




                        foreach (var toAdd in toAddOpinionListList)
                        {
                            var destinationIndexInTab64 = indexInTab64 + toAdd;
                            var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);

                            if (destinationIndex < 0 || destinationIndex > 63)
                                continue;
                            var isContent = cloneBoard.GetIsContent(destinationIndex, caseColor);
                            if (isContent == 0)
                            {
                                //results.Add(destinationIndex);
                                //opinionPossibleMovesIndex.Add(new PossibleMove { FromIndex = opinionIndex, Index = destinationIndex, IsContainOpinion = false });
                                // opinionPossibleMovesIndex.Add(destinationIndex);
                                possibleMoves.Add(destinationIndex);
                            }
                            if (possibleMoves.Contains(toIndex))
                                return true;



                        }


                    }


                }

            }


            return false;
        }

        public static bool IsMenacedOld(int toIndex, Board board)
        {
            var opinionColor = "W";
            if (Utils.ComputerColor == "W")
                opinionColor = "B";

            var opinionIndexs = board.GetCasesIndexForColor(opinionColor);
            foreach (var item in opinionIndexs)
            {
                var possibleMove = board.GetPossibleMoves(item, 0).Select(x => x.Index);
                if (possibleMove.Contains(toIndex))
                    return true;
            }


            return false;
        }

        private static string _computerColor; // field

        public static string ComputerColor   // property
        {
            get { return _computerColor; }   // get method
            set
            {
                _computerColor = value;

                //  var opinionColor = "W";
                if (value == "W")
                    OpinionColor = "B";
                if (value == "B")
                    OpinionColor = "W";
            }  // set method
        }

        public static string OpinionColor { get; set; } = "W";
        /*tsiry;20-05-2022
         * */
        public static Board CloneBoad(Board originalBord)
        {
            return new Board(originalBord);
        }
        // public static string ComputerColor { get; set; }
        public static Board CloneAndMove(Board originalBord, int initialIndex, int destinationIndex, int level)
        {
            var resultBorad = new Board(originalBord);

            resultBorad.Move(initialIndex, destinationIndex, level);

            resultBorad.CalculeScores();


            return resultBorad;
        }

        /*tsiry;24-12-2021
         * */
        public static Board Clone(Board originalBord)
        {
            var resultBorad = new Board(originalBord);
            return resultBorad;
        }

        public static bool IsValideMove(int indexTab120)
        {

            var index120 = Utils.Tab120[indexTab120];
            if (index120 == -1)
                return false;
            return true;
        }

        public static bool IsInFirstTab64(int tab64Content, string color)
        {
            if (color == "W")
            {
                if (SimplePawnFirstWhiteTab64.Contains(tab64Content))
                    return true;
            }
            if (color == "B")
            {
                if (SimplePawnFirstBlackTab64.Contains(tab64Content))
                    return true;
            }
            return false;


        }

        public static int[] SimplePawnFirstBlackTab64 = {
31, 32, 33, 34, 35, 36, 37, 38
    };

        public static int[] SimplePawnFirstWhiteTab64 = {
81, 82, 83, 84, 85, 86, 87, 88
    };

        public static int[] Tab120 = {
-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
-1, 0, 1, 2, 3, 4, 5, 6, 7, -1,
-1, 8, 9, 10, 11, 12, 13, 14, 15, -1,
-1, 16, 17, 18, 19, 20, 21, 22, 23, -1,
-1, 24, 25, 26, 27, 28, 29, 30, 31, -1,
-1, 32, 33, 34, 35, 36, 37, 38, 39, -1,
-1, 40, 41, 42, 43, 44, 45, 46, 47, -1,
-1, 48, 49, 50, 51, 52, 53, 54, 55, -1,
-1, 56, 57, 58, 59, 60, 61, 62, 63, -1,
-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
-1, -1, -1, -1, -1, -1, -1, -1, -1, -1
    };
        public static int[] Tab64 = {
21, 22, 23, 24, 25, 26, 27, 28,
31, 32, 33, 34, 35, 36, 37, 38,
41, 42, 43, 44, 45, 46, 47, 48,
51, 52, 53, 54, 55, 56, 57, 58,
61, 62, 63, 64, 65, 66, 67, 68,
71, 72, 73, 74, 75, 76, 77, 78,
81, 82, 83, 84, 85, 86, 87, 88,
91, 92, 93, 94, 95, 96, 97, 98
    };

    }

}
