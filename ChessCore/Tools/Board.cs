using System.Diagnostics;

namespace ChessCore.Tools
{
    public class Board
    {

        #region Attribus
        private string[] _cases = new string[64];//index:coord|pawnName(sp)|colore

        //T86 A DECOMMENTER SI ON VEUT VOIR UNE FUITE DE MEMOIR
        /* private int[] _evolutionPawnIndexBlack =
        {
          56,57,58,59,60,61,62,63
        };
        private int[] _evolutionPawnIndexWhite =
        {
          0,1,2,3,4,5,6,7
        };

        private string[] _coord = {
    "a8","b8","c8","d8","e8","f8","g8","h8",
    "a7","b7","c7","d7","e7","f7","g7","h7",
    "a6","b6","c6","d6","e6","f6","g6","h6",
    "a5","b5","c5","d5","e5","f5","g5","h5",
    "a4","b4","c4","d4","e4","f4","g4","h4",
    "a3","b3","c3","d3","e3","f3","g3","h3",
    "a2","b2","c2","d2","e2","f2","g2","h2",
    "a1","b1","c1","d1","e1","f1","g1","h1"
        };*/
        #endregion
        #region Properties

        public int Weight { get; set; }
        public List<string> MovingList { get; set; }//pour l'historique de déplacement
        public List<string> HuntingBoardWhiteImageList { get; set; }//pour la version web
        public List<string> HuntingBoardBlackImageList { get; set; }//pour la version web
        public string[] GetCases()
        {
            return _cases;
        }
        public void SetCases(int index, string containt)
        {
            _cases[index] = containt;
        }
        public string GetPawnShortNameInIndex(int index)
        {
            var currentCase = _cases[index];
            if (!currentCase.Contains("|"))
                return "empty";
            return currentCase.Split('|')[0].ToString();
        }
        public string GetPawnColorNameInIndex(int index)
        {
            var currentCase = _cases[index];
            if (!currentCase.Contains("|"))
                return "empty";
            return currentCase.Split('|')[1].ToString();
        }

        public bool DestinationIndexIsMenaced { get; set; } = false;
        public int Diff { get; set; }
        public int WhiteScore { get; set; }
        public int BlackScore { get; set; }
        public int Level { get; set; }


        //retourne tous les index du couleur
        public int[] GetCasesIndex(string colore)
        {
            List<int> results = new List<int>();
            //foreach (var item in _cases)
            for (int i = 0; i < _cases.Count(); i++)
            {
                var item = _cases[i];
                if (item.Contains("|"))
                {
                    var caseDatas = item.Split('|');
                    var caseColor = caseDatas[1];

                    if (caseColor == colore)
                        results.Add(i);


                }
            }

            return results.ToArray();
        }

        /*tsiry;07-01-2022
        * returne tout les index sauf 
        * */
        public int[] GetCasesAllIndexExcept(string colore, string exceptPawnName)
        {
            List<int> results = new List<int>();
            //foreach (var item in _cases)
            for (int i = 0; i < _cases.Count(); i++)
            {
                var item = _cases[i];
                if (item.Contains("|"))
                {
                    var caseDatas = item.Split('|');
                    var caseColor = caseDatas[1];

                    var caseName = caseDatas[0];
                    if (caseColor == colore && caseName != exceptPawnName)
                        results.Add(i);
                }


            }

            return results.ToArray();
        }

        /*tsiry;07-01-2022
         * pour ne prendre d'une index
         * */
        public int GetOneIndex(string colore, string pawnName)
        {

            //foreach (var item in _cases)
            for (int i = 0; i < _cases.Count(); i++)
            {
                var item = _cases[i];
                if (item.Contains("|"))
                {
                    var caseDatas = item.Split('|');
                    var caseColor = caseDatas[1];

                    var caseName = caseDatas[0];
                    if (caseColor == colore && caseName == pawnName)
                        return i;


                }
            }
            return -1;


        }
        #endregion

        #region Methodes

        /*tsiry;03-01-2022
         * */
        public void LoadFromDirectorie(string dirLocation)
        {
            try
            {



                var whiteFileLocation = dirLocation + "/WHITEList.txt";
                var blackFileLocation = dirLocation + "/BlackList.txt";






                var readText = File.ReadAllText(whiteFileLocation);

                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Debug.WriteLine(line);
                        //  public Pawn(string name, string location, Button associateButton, string colore, MainWindow mainWindowParent)


                        var datas = line.Split(';');
                        // var bt = (Button)this.FindName(datas[1]);
                        var index = Utils.GetIndexFromLocation(datas[1]);//datas[1] = location
                        this.InsertPawn(index, Utils.ChangeLongNameToShortName(datas[0]), "W");


                    }
                }

                readText = File.ReadAllText(blackFileLocation);

                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var datas = line.Split(';');
                        // var bt = (Button)this.FindName(datas[1]);
                        var index = Utils.GetIndexFromLocation(datas[1]);//datas[1] = location
                        this.InsertPawn(index, Utils.ChangeLongNameToShortName(datas[0]), "B");

                    }
                }





            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }

        }

        public void CalculeScores()
        {
            var blackBonus = 0;
            var whiteBonus = 0;
            foreach (var evolutionPawnIndex in Utils.GetEvolutionPawnIndexWhite())
            {
                var contains = _cases[evolutionPawnIndex];
                if (contains.Contains("P"))
                {
                    whiteBonus = 9;
                    break;
                }
            }

            foreach (var evolutionPawnIndex in Utils.GetEvolutionPawnIndexBlack())
            {
                var contains = _cases[evolutionPawnIndex];
                if (contains.Contains("P"))
                {
                    blackBonus = 9;
                    break;
                }
            }

            var whitePawnNumber = _cases.Count(x => x == "P|W");
            var blackPawnNumber = _cases.Count(x => x == "P|B");
            var whiteBishopNumber = _cases.Count(x => x == "B|W");
            var blackBishopNumber = _cases.Count(x => x == "B|B");
            var whiteKnightNumber = _cases.Count(x => x == "C|W");
            var blackKnightNumber = _cases.Count(x => x == "C|B");
            var whiteRookNumber = _cases.Count(x => x == "T|W");
            var blackRooktNumber = _cases.Count(x => x == "T|B");
            var whiteQueenNumber = _cases.Count(x => x == "Q|W");
            var blackQueenNumber = _cases.Count(x => x == "Q|B");
            var whiteKingNumber = _cases.Count(x => x == "K|W");
            var blackKingNumber = _cases.Count(x => x == "K|B");



            WhiteScore =
                 whitePawnNumber
                 + (whiteBishopNumber + whiteKnightNumber) * 3
                 + whiteRookNumber * 5
                 + whiteQueenNumber * 9
             + whiteKingNumber * 100
            + whiteBonus;
            BlackScore =
              blackPawnNumber
              + (blackBishopNumber + blackKnightNumber) * 3
              + blackRooktNumber * 5
              + blackQueenNumber * 9
              + blackKingNumber * 100
              + blackBonus;


            Diff = Math.Abs(BlackScore - WhiteScore);
            if (Utils.ComputerColor == "B")
                Weight = BlackScore - WhiteScore;
            else
                Weight = WhiteScore - BlackScore;
        }

        public int GetValue(string caseContaint)
        {
            if (caseContaint.Contains("P|"))
                return 1;
            if (caseContaint.Contains("B|") || caseContaint.Contains("C|"))
                return 3;
            if (caseContaint.Contains("T|"))
                return 5;
            if (caseContaint.Contains("Q|"))
                return 9;
            if (caseContaint.Contains("K|"))
                return 100;
            return 0;

        }
        public string GetCaseInIndex(int index)
        {
            return _cases[index];
        }
        public int GetWeightInIndex(int index)
        {
            var caseContaint = GetCaseInIndex(index);



            return GetValue(caseContaint);
        }


        public Board()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
        }



        public int GetKingColorIndex(string color)
        {
            // var caseListColor = _cases.Where(x => x.Contains(color));
            var contain = $"K|{color}";
            return _cases.ToList().IndexOf(contain);
        }
        public Board(Board originalBord)
        {
            var originalCases = originalBord.GetCases();
            for (int i = 0; i < originalCases.Count(); i++)
            {
                _cases[i] = originalCases[i];
            }


        }

        public void Init()
        {
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


            //Black
            InsertPawn(0, "T", "B");
            InsertPawn(1, "C", "B");
            InsertPawn(2, "B", "B");
            InsertPawn(3, "Q", "B");
            InsertPawn(4, "K", "B");
            InsertPawn(7, "T", "B");
            InsertPawn(6, "C", "B");
            InsertPawn(5, "B", "B");
            // InsertPawn(49, "P", "B");
            for (int i = 8; i <= 15; i++)
            {
                InsertPawn(i, "P", "B");
            }


            // White
            InsertPawn(56, "T", "W");
            InsertPawn(57, "C", "W");
            InsertPawn(58, "B", "W");
            InsertPawn(59, "Q", "W");
            InsertPawn(60, "K", "W");
            //InsertPawn(14, "P", "W");
            InsertPawn(62, "C", "W");
            InsertPawn(61, "B", "W");
            InsertPawn(63, "T", "W");
            for (int i = 48; i <= 55; i++)
            {
                InsertPawn(i, "P", "W");
            }
        }

        public void Print()
        {
            Console.WriteLine("_____________________________________________________________________");
            for (int y = 0; y < 8; y++)
            {
                var line = "";
                for (int x = 0; x < 8; x++)
                {
                    var index = x + (y * 8);
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("_____________________________________________________________________");
        }
        public void PrintInDebug()
        {
            Debug.WriteLine("_____________________________________________________________________");
            for (int y = 0; y < 8; y++)
            {
                var line = "";
                for (int x = 0; x < 8; x++)
                {
                    var index = x + (y * 8);
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Debug.WriteLine(line);
            }
            Debug.WriteLine("_____________________________________________________________________");
        }

        public void InsertPawn(int index, string pawnName, string color)
        {
            try
            {
                _cases[index] = $"{pawnName}|{color}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return;
            }
        }





        public void Move(int initialIndex, int destinationIndex, int level)
        {
            var initialCase = _cases[initialIndex];
            var destinationCase = _cases[destinationIndex];

            _cases[destinationIndex] = initialCase;


            if (destinationCase.Contains("|"))
            {
                var datas = destinationCase.Split('|');
                var pawnName = datas[0];
                var caseColor = datas[1];
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




                if (caseColor == "B")
                {
                    caseColor = "Black";
                }
                else
                {

                    caseColor = "White";
                }

                //  / BishopBlack.png

              //  var imageSrc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", $"{pawnName}{caseColor}.png");

                var imageSrc = $"../../Images/{pawnName}{caseColor}.png";

                if (HuntingBoardWhiteImageList == null)
                    HuntingBoardWhiteImageList = new List<string>();
                if (HuntingBoardBlackImageList == null)
                    HuntingBoardBlackImageList = new List<string>();
                if (caseColor == "White")
                    HuntingBoardWhiteImageList.Add(imageSrc);
                else
                    HuntingBoardBlackImageList.Add(imageSrc);
            }




            //if(destinationCase.Contains("K"))
            //{
            //  var t_ok = "OK";
            //}
            _cases[initialIndex] = "__";

            Level = level;
            //pour T35
            if (Level == 1)
                if (Utils.IsMenaced(destinationIndex, this, Utils.ComputerColor))
                {
                    //item.Weight -= 1;
                    this.DestinationIndexIsMenaced = true;
                }

            if (MovingList == null)
                MovingList = new List<string>();

            MovingList.Add($"{initialIndex.ToString()}({initialCase})>{destinationIndex.ToString()}({destinationCase})");

            //GestionDes roc
            // si le point de depar et le rois : 60 et point d'arriver est 62 
            //=>roc court pour les blancs
            //on depace aussi le rook en 63 ver 61
            if (initialIndex == 60 && destinationIndex == 62)
            {
                Move(63, 61, 0);
            }
            // si le point de depar et le rois : 60 et point d'arriver est 58 
            //=>roc court pour les blancs
            //on depace aussi le rook en 56 ver 59
            if (initialIndex == 60 && destinationIndex == 58)
            {
                Move(56, 59, 0);
            }


            if (initialIndex == 4 && destinationIndex == 6)
            {
                Move(7, 5, 0);
            }
            if (initialIndex == 4 && destinationIndex == 2)
            {
                Move(0, 3, 0);
            }

            //Evolution
            if (initialCase.Contains("|"))
            {
                var initialPawnName = initialCase.Split('|')[0];
                var initialPawnColor = initialCase.Split('|')[1];
                if (initialPawnName == "P")
                {
                    var evolutionPawnIndex = Utils.GetEvolutionPawnIndexBlack();
                    if (initialPawnColor == "W")
                    {
                        evolutionPawnIndex = Utils.GetEvolutionPawnIndexWhite();
                    }

                    if (evolutionPawnIndex.Contains(destinationIndex))
                    {
                        //évolution
                        _cases[destinationIndex] = _cases[destinationIndex].Replace("P", "Q");
                    }

                }
            }



        }

        //verifie si la case contien un pion
        //retourne 0 si libre
        //retourne 1 si alier
        //retourne -1 si advers

        public int GetIsContent(int index, string color)
        {
            if (index > _cases.Count())
                return 0;
            if (index < 0)
                return 0;
            var destinationCase = _cases[index];

            if (!destinationCase.Contains("|"))
            {
                //la case est vide
                return 0;
            }
            var caseDatas = destinationCase.Split('|');
            var pawnName = caseDatas[0];
            var caseColor = caseDatas[1];

            if (caseColor != color)

                return -1;
            return 1;
        }


        /*tsiry;18-10-2021
        * verifie si en echec en fonction du couleur
        * */
        /*  public bool IsInChessInNextLevel(string color)
          {

            try
            {
              var kingIndex = _cases.ToList().IndexOf($"K|{color}");

              var opinionColor = "W";
              if (color == "W")
                opinionColor = "B";
              var opinionPawns = GetCasesIndex(opinionColor);

              var possiblesMovesOpinionIndex = new List<int>();
              foreach (var index in opinionPawns)
              {

                var possiblesMoves = GetPossibleMoves(index);
                foreach (var movedIndex in possiblesMoves)
                {
                  ////Console.WriteLine($"{index} => {movedIndex}");
                  ////Console.WriteLine($"\t L : {level}");
                  var copyAndMovingBord = Utils.CloneAndMove(this, index, movedIndex);
                  /////copyAndMovingBord.Print();
                  if (copyAndMovingBord.IsInChess(color))
                    return true;
                  //si Board est menacer et copyAndMovingBord aussi mencer, on ne l'éxpoite plus

                }
              }
              return false;
            }
            catch (Exception ex)
            {

              return false;
            }

          }
      */
        /*tsiry;18-10-2021
         * verifie si en echec en fonction du couleur
         * */
        public bool IsInChess(string color)
        {

            try
            {
                var kingIndex = _cases.ToList().IndexOf($"K|{color}");

                var opinionColor = "W";
                if (color == "W")
                    opinionColor = "B";
                var opinionPawns = GetCasesIndex(opinionColor);

                var possiblesMovesOpinionIndex = new List<int>();
                foreach (var index in opinionPawns)
                {
                    //var t_ = GetPossibleMoves(index);
                    possiblesMovesOpinionIndex.AddRange(GetPossibleMoves(index, 1).Select(x => x.Index));
                }


                if (possiblesMovesOpinionIndex.Contains(kingIndex))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

        }

        public List<PossibleMove> GetPossibleMoves(int index, int level, bool isForKing = true)
        {
      /*if(Utils(this,Utils.OpinionColor))
      {
        var t_hh = this;
      }*/
            if (index == -1)
                return null;
            //var t_index64 = Utils.Tab64[50];

            //// var t_toAdd = ((9 / 9) * 10) + 1;
            //var t_index120 = Utils.Tab120[t_index64-14];
            var indexInTab64 = Utils.Tab64[index];
            var results = new List<PossibleMove>();



            var currentCase = _cases[index];
            if (!currentCase.Contains("|"))
            {
                //la case est vide
                return results;
            }
            var caseDatas = currentCase.Split('|');
            var pawnName = caseDatas[0];
            var caseColor = caseDatas[1];
            if (pawnName == "T" || pawnName == "Q" || pawnName == "K")//Rook ou Reine ou Roi
            {

                //Horizontal +
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = i;
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;

                }
                //Horizontal -
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = -(i);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }

                //vertical --
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = -(i * 10);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }

                //vertical ++
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = (i * 10);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }




            }




            if (pawnName == "B" || pawnName == "Q" || pawnName == "K")//Bishop ou Reine ou Roi
            {

                //Horizontal + //vertical +
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = (y * 10) + x;
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);

                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });

                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }


                //Horizontal + //vertical -
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = -((y * 10) - x);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        //if (pawnName != "K")
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                       // else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                        //    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }

                //Horizontal - //vertical +
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = ((y * 10) - x);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }

                //Horizontal - //vertical -
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = -((y * 10) + x);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }




            }


            //gestion du roc
            if (pawnName == "K")
            {
                // si le roi ne s'a pas encore bouger
                if (this.MovingList != null)
                {
                    foreach (var move in this.MovingList)
                    {

                        if (move.Contains($"K|{caseColor}"))
                            return results;
                    }
                }


                //si blanch
                if (caseColor == "W")
                {
                    if (index == 60)//si le roi n'a ba bouger de 60
                    {
                        //A droite
                        var rightIsFree = true;
                        //si il n'y a rien entre 60 et 63
                        for (int i = 61; i < 63; i++)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                rightIsFree = false;
                                break;
                            }
                        }
                        if (rightIsFree)
                        {
                            if (_cases[63] == $"T|{caseColor}")
                            {
                                var rookRightIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("63"))
                                            rookRightIsAsBeenMoved = false;
                                    }
                                }
                                if (rookRightIsAsBeenMoved)
                                    //results.Add(62);
                                    results.Add(new PossibleMove { FromIndex = index, Index = 62, IsContainOpinion = false });
                            }
                        }

                        //a gauche
                        var leftIsFree = true;
                        //si il n'y a rien entre 60 et 63
                        for (int i = 59; i > 56; i--)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                leftIsFree = false;
                                break;
                            }
                        }
                        if (leftIsFree)
                        {
                            if (_cases[56] == $"T|{caseColor}")
                            {
                                var rookLeftIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("56"))
                                            rookLeftIsAsBeenMoved = false;
                                    }
                                }
                                if (rookLeftIsAsBeenMoved)
                                    //results.Add(58);
                                    results.Add(new PossibleMove { FromIndex = index, Index = 58, IsContainOpinion = false });
                            }
                        }


                        //si il n'y a rien entre 60 et 56
                    }


                }
                else//si noir
                {
                    if (index == 4)//si le roi n'a ba bouger de 4
                    {
                        //A droite
                        var rightIsFree = true;

                        //si il n'y a rien entre 5 et 7
                        for (int i = 5; i < 7; i++)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                rightIsFree = false;
                                break;
                            }
                        }
                        if (rightIsFree)
                        {
                            if (_cases[7] == $"T|{caseColor}")
                            {
                                var rookRightIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("7"))
                                            rookRightIsAsBeenMoved = false;
                                    }
                                }
                                if (rookRightIsAsBeenMoved)
                                    //results.Add(6);
                                    results.Add(new PossibleMove { FromIndex = index, Index = 6, IsContainOpinion = false });
                            }
                        }

                        //a gauche
                        var leftIsFree = true;
                        //si il n'y a rien entre 60 et 63
                        for (int i = 3; i > 0; i--)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                leftIsFree = false;
                                break;
                            }
                        }
                        if (leftIsFree)
                        {
                            if (_cases[0] == $"T|{caseColor}")
                            {
                                var rookLeftIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("0"))
                                            rookLeftIsAsBeenMoved = false;
                                    }
                                }
                                if (rookLeftIsAsBeenMoved)
                                    //results.Add(2);
                                    results.Add(new PossibleMove { FromIndex = index, Index = 2, IsContainOpinion = false });
                            }
                        }

                    }

                }



            }


            if (pawnName == "C")//Knight
            {
                var toAddList = new List<int>();
                toAddList.Add(-12);
                toAddList.Add(-21);
                toAddList.Add(-19);
                toAddList.Add(-8);
                toAddList.Add(12);
                toAddList.Add(21);
                toAddList.Add(19);
                toAddList.Add(8);



                foreach (var toAdd in toAddList)
                {

                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == -1)
                        results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                    if (isContent == 0)
                    {
                        //results.Add(destinationIndex);
                        results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });

                    }


                }



            }


            if (pawnName == "P")//SimplePawn
            {
                var toAddList = new List<int>();
                var sing = -1;
                if (caseColor == "B")
                    sing = 1;
                //Diagonal for opinion
                var toAddOpinionListList = new List<int>();
                toAddOpinionListList.Add((10 * sing) + 1);
                toAddOpinionListList.Add((10 * sing) - 1);

                //Normal move
                toAddList.Add(10 * sing);
                //pour les premérs mouvements
                if (Utils.IsInFirstTab64(indexInTab64, caseColor))
                    toAddList.Add(20 * sing);


                foreach (var toAdd in toAddList)
                {
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 0)
                    {
                        // results.Add(destinationIndex);
                        results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = false });
                    }
                    else
                        break;


                }


                foreach (var toAdd in toAddOpinionListList)
                {
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == -1)
                    {
                        //results.Add(destinationIndex);
                        results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                    }



                }



            }




            return results;

        }

    /// <summary>
    /// tsiry;02-07-2022
    /// pour determiner les mouvement possibles du rois si ce dérnier est menacé
    /// </summary>
    public List<int> GetKingPossiblesMoveIndex(string targetkingColor)
    {
      try
      {
        
        var targetkingindex = this.GetCases().ToList().IndexOf($"K|{targetkingColor}");
        var indexInTab64 = Utils.Tab64[targetkingindex];
        var results = new List<int>();
        var toAddList = new List<int>();
        toAddList.Add(-11);
        toAddList.Add(-10);
        toAddList.Add(-9);
        toAddList.Add(+1);
        toAddList.Add(+11);
        toAddList.Add(10);
        toAddList.Add(9);
        toAddList.Add(-1);

        

        foreach (var toAdd in toAddList)
                {

                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, targetkingColor);
                    if (isContent == -1)
                        results.Add(destinationIndex);
                    if (isContent == 0)
                    {
                        //results.Add(destinationIndex);
                        results.Add(destinationIndex);

                    }


                }
     
        return results;

      }
      catch (Exception ex)
      {

        return null;
      }
    }

    #endregion
  }
}
