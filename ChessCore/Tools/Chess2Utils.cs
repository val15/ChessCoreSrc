using System.Diagnostics;

namespace ChessCore.Tools
{
    public static class Chess2Utils
    {



        public static bool TargetIndexIsProteted(Board inBoard, string targetkingColor, int targetIndex)
        {
            try
            {
                //   if (!TargetKingIsMenaced(inBoard, targetkingColor))
                //     return false;

                //var targetkingindex = inBoard.GetCases().ToList().IndexOf($"K|{targetkingColor}");
                var alliesIndex = inBoard.GetCasesIndexForColor(targetkingColor).ToList();
                alliesIndex.Remove(targetIndex);
                //var possibleMovesAllies = 
                foreach (var alieFromIndex in alliesIndex)
                {
                    var possibleMovesIndex = inBoard.GetPossibleMoves(alieFromIndex, 1, false).Select(x => x.ToIndex);
                    //il faut faire un copi du bord original en déplacent le roi vers le possible move
                    foreach (var alieToIndex in possibleMovesIndex)
                    {
                        var copyBord = CloneAndMove(inBoard, alieFromIndex, alieToIndex, 0);
                        //  var possibleKingMovesCopy = copyBord.GetKingPossiblesMoveIndex(targetkingColor);
                        var isMelaced = TargetIndexIsMenaced(copyBord, targetkingColor, targetIndex);
                        if (!isMelaced)
                            return true;

                    }
                }







                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public static bool TargetKingColorIsProteted(Board inBoard, string targetkingColor)
        {
            try
            {
                //   if (!TargetKingIsMenaced(inBoard, targetkingColor))
                //     return false;

                var targetkingIndex = inBoard.GetCases().ToList().IndexOf($"K|{targetkingColor}");
                return TargetIndexIsProteted(inBoard, targetkingIndex, targetkingColor);




            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// tsiry;20-07-2022
        /// </summary>
        public static bool TargetIndexIsProteted(Board inBoard, int targetIndex, string targetColor)
        {
            try
            {


                //  var targetkingindex = inBoard.GetCases().ToList().IndexOf($"K|{targetkingColor}");
                var alliesIndex = inBoard.GetCasesIndexForColor(targetColor).ToList();
                alliesIndex.Remove(targetIndex);
                //var possibleMovesAllies = 
                foreach (var alieFromIndex in alliesIndex)
                {
                    var possibleMovesIndex = inBoard.GetPossibleMoves(alieFromIndex, 1, false).Select(x => x.ToIndex);
                    //il faut faire un copi du bord original en déplacent le roi vers le possible move
                    foreach (var alieToIndex in possibleMovesIndex)
                    {
                        var copyBord = CloneAndMove(inBoard, alieFromIndex, alieToIndex, 0);
                        //  var possibleKingMovesCopy = copyBord.GetKingPossiblesMoveIndex(targetkingColor);
                        var isMelaced = TargetIndexIsMenaced(copyBord, targetColor, targetIndex);
                        if (!isMelaced)
                            return true;

                    }
                }







                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        /// <summary>
        /// tsiry;02-07-2022
        /// </summary>

        public static bool TargetColorIsInChess(Board inBoard, string targetkingColor)
        {
            try
            {
                if (!TargetKingIsMenaced(inBoard, targetkingColor))
                    return false;


                var targetkingindex = inBoard.GetCases().ToList().IndexOf($"K|{targetkingColor}");
                var possibleKingMoves = inBoard.GetKingPossiblesMoveIndex(targetkingColor);

                //il faut faire un copi du bord original en déplacent le roi vers le possible move
                foreach (var index in possibleKingMoves)
                {
                    var copyBord = CloneAndMove(inBoard, targetkingindex, index, 0);
                    //  var possibleKingMovesCopy = copyBord.GetKingPossiblesMoveIndex(targetkingColor);
                    var isMelaced = TargetIndexIsMenaced(copyBord, targetkingColor, index);
                    if (!isMelaced)
                        return false;

                }

                if (TargetKingColorIsProteted(inBoard, targetkingColor))
                    return false;




                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// tsiry;20-07-2022
        /// </summary>
        public static int GetWeigtOpionionMenacedsByToIndex(Board inBoard, string opinionColorColor, int toIndex)
        {
            try
            {
                var weight = 0;
                var opinionPawnIndex = inBoard.GetCasesIndexForColor(opinionColorColor);
                foreach (var index in opinionPawnIndex)
                {

                    if (TargetIndexIsMenacedByToIndex(inBoard, opinionColorColor, index, toIndex))
                        weight += inBoard.GetWeightInIndex(index);
                }
                return weight;
            }
            catch (Exception ex)
            {
                return 0;

            }
        }

        /// <summary>
        /// tsiry;19-07-2022
        /// </summary>
        public static int GetNumberOpionionMenaceds(Board inBoard, string opinionColorColor)
        {
            try
            {
                var number = 0;
                var opinionPawnIndex = inBoard.GetCasesIndexForColor(opinionColorColor);
                foreach (var index in opinionPawnIndex)
                {
                    if (TargetIndexIsMenaced(inBoard, opinionColorColor, index))
                        inBoard.GetWeightInIndex(index);
                }
                return number;
            }
            catch (Exception ex)
            {
                return 0;

            }
        }

        /* /// <summary>
           /// tsiry;20-07-2022
           /// </summary>
           public static double GetComputerProtectedsWeight(Board inBoard)
           {
             try
             {
               var result = 0;
               var opinionPawnIndex = inBoard.GetCasesIndexForColor(Utils.ComputerColor);
               foreach (var index in opinionPawnIndex)
               {
                 if (TargetIndexIsProteted(inBoard,index, Utils.ComputerColor))
                 {
                   result+=inBoard.GetWeightInIndex(index);
                 }

               }
               return result;
             }
             catch (Exception ex)
             {
               return 0;

             }
           }
       */
        /// <summary>
        /// tsiry;20-07-2022
        /// </summary>
        public static bool TargetIndexIsMenacedByToIndex(Board inBoard, string targetColor, int targetIndex, int toIndex)
        {
            try
            {
                var opinionColor = "W";
                if (targetColor == "W")
                    opinionColor = "B";
                var copyBoard = new Board(inBoard);
                if (targetIndex != -1)
                {
                    if (copyBoard.GetCases()[targetIndex].Contains($"|{opinionColor}"))//si la case contion un pion adverse, on le vide
                        copyBoard.GetCases()[targetIndex] = "__";
                }


                var opinionPossibleMoveIndexList = new List<int>();

                opinionPossibleMoveIndexList.AddRange(copyBoard.GetPossibleMoves(toIndex, 1, false).Select(x => x.ToIndex));

                if (opinionPossibleMoveIndexList.Contains(targetIndex))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                return false;

            }
        }


        /// <summary>
        /// tsiry;02-07-2022
        /// </summary>

        public static bool TargetIndexIsMenaced(Board inBoard, string targetColor, int targetIndex)
        {
            try
            {
                var opinionColor = "W";
                if (targetColor == "W")
                    opinionColor = "B";
                var index = 0;
                var copyBoard = new Board(inBoard);
                if (targetIndex != -1)
                {
                    if (copyBoard.GetCases()[targetIndex].Contains($"|{opinionColor}"))//si la case contion un pion adverse, on le vide
                        copyBoard.GetCases()[targetIndex] = "__";
                }

                var opinionOfTargetColorIndexList = new List<int>();
                foreach (var item in copyBoard.GetCases())
                {
                    if (item.Contains($"|{opinionColor}"))
                    {
                        opinionOfTargetColorIndexList.Add(index);
                    }
                    index++;
                }
                var opinionPossibleMoveIndexList = new List<int>();
                foreach (var opibionIndex in opinionOfTargetColorIndexList)
                {
                    opinionPossibleMoveIndexList.AddRange(copyBoard.GetPossibleMoves(opibionIndex, 1, false).Select(x => x.ToIndex));
                }
                if (opinionPossibleMoveIndexList.Contains(targetIndex))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                return false;

            }
        }





        /// <summary>
        /// tsiry;02-07-2022
        /// </summary>

        public static bool TargetKingIsMenaced(Board inBoard, string targetkingColor)
        {
            try
            {
                var targetkingindex = inBoard.GetCases().ToList().IndexOf($"K|{targetkingColor}");


                return TargetIndexIsMenaced(inBoard, targetkingColor, targetkingindex);


            }
            catch (Exception ex)
            {
                return false;

            }
        }
        public static List<Pawn> LoadFromDirectorie(string dirLocation)
        {
            try
            {
                var pawnListWhite = new List<Pawn>();
                var pawnListBlack = new List<Pawn>();

                var whiteFileLocation = dirLocation + "/WHITEList.txt";
                var blackFileLocation = dirLocation + "/BlackList.txt";






                var readText = File.ReadAllText(whiteFileLocation);

                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {


                        var datas = line.Split(';');
                        var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                        //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                        newPawn.IsFirstMove = bool.Parse(datas[3]);
                        newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                        newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                        newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                        pawnListWhite.Add(newPawn);

                    }
                }

                readText = File.ReadAllText(blackFileLocation);

                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {


                        var datas = line.Split(';');
                        var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                        //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                        newPawn.IsFirstMove = bool.Parse(datas[3]);
                        newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                        newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                        newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                        pawnListBlack.Add(newPawn);

                    }
                }



                var pawnList = new List<Pawn>();
                pawnList.AddRange(pawnListWhite);
                pawnList.AddRange(pawnListBlack);

                return pawnList;
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.Message);
                return null;
            }
        }
        public static List<Node> EmuleAllIndexInParallelForEach(Board boarChess2, List<int> computerPawnsIndex, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            // var cpuColor = ComputerColore[0].ToString();
            var opinionColor = "W";
            if (cpuColor == "W")
                opinionColor = "B";
            List<Node> bestNodList = new List<Node>();
            Parallel.ForEach(computerPawnsIndex, pawnIndex =>
            {
                // GC.Collect();
                // {
                Utils.WritelineAsync($"pawnIndex : {pawnIndex}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");

                var engine = new EngineMultiThreading(level, cpuColor, IsReprise, Utils.IsMenaced(pawnIndex, boarChess2, cpuColor), SpecifiBoardList);


                // var firstInLastMove = GetTreeLastAction();



                var bestNodeChess2 = engine.SearchThreadForOnce(boarChess2, "5", pawnIndex, level);
                if (bestNodeChess2 != null)
                {
                    var node = new Node();
                    node.Location = Chess2Utils.GetLocationFromIndex(bestNodeChess2.FromIndex);
                    node.BestChildPosition = Chess2Utils.GetLocationFromIndex(bestNodeChess2.ToIndex);
                    //DOTO
                    //// node.AssociatePawn = GetPawn(node.Location);
                    //Seulment pour les test
                    node.AsssociateNodeChess2 = bestNodeChess2;

                    //Pour T49
                    // si la destination contien un pion adverse, 
                    //le poid est plus la valeure du pion adverse
                    var destinationColor = boarChess2.GetPawnColorNameInIndex(bestNodeChess2.ToIndex);

                    if (destinationColor == opinionColor)
                    {
                        bestNodeChess2.Weight += 1;
                        // bestNodeChess2.Weight += boarChess2.GetValue(boarChess2.GetCaseInIndex(bestNodeChess2.ToIndex));
                    }
                    node.Weight = bestNodeChess2.Weight;
                    if (level < 4)
                    {
                        //var toMenacedWeight = 0;
                        node.IsMenaced = Utils.IsMenaced(bestNodeChess2.ToIndex, boarChess2, cpuColor);
                        //pour T65
                        //pur bestNodListLevel2 on enleve des point aux menacées 
                        // bestNodList.Where(w => w.IsMenaced).ToList().ForEach(x => x.Weight -= x.AssociatePawn.Value);
                        //if(bestNodeChess2.ToIndex == 16)
                        // {
                        //   var t_ = 00;
                        // }
                        if (node.IsMenaced)
                        {

                            //var t_ddv = bestNodeChess2.GetValue();

                            node.Weight -= bestNodeChess2.GetValue();
                            bestNodeChess2.Weight = node.Weight;
                            // var t_dd = node.Weight;
                        }

                    }


                    //Pour T80 et T81
                    var kingIsMenaced = Utils.ComputerKingIsMenaced(node.AsssociateNodeChess2.Board);

                    if (!kingIsMenaced)
                    {

                        //Pour T88 si il y a -999 au niveau 3 on l'enleve
                        //seulment pour le level 3
                        //on réemule les best de niveau 3
                        /* if(level == 3)
                         {
                           var recheckNode = GetOneNodeLevel2Node(boarChess2, Chess2Utils.GetIndexFromLocation(node.Location), Chess2Utils.GetIndexFromLocation(node.BestChildPosition), Utils.ComputerColor, IsReprise, SpecifiBoardList);
                           if (recheckNode.Weight == -999)
                           {
                             node.Weight = -999;
                           }
                         }*/

                        // }*/

                        Utils.WritelineAsync($"{bestNodeChess2.Weight}  {node.Location} =>  {node.BestChildPosition}");
                        bestNodList.Add(node);
                    }



                }

            });

            return bestNodList;
        }


        /*tsiry;07-01-2022
     * on élule la reinne avant les autres
     * */
        public static Node EmuleOnlyOneIndex(Board boarChess2, int pawnIndex, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            // var boarChess2 = Utils.Clone(boarChessIn);
            //var cpuColor = Utils.ComputerColor;
            var bestNode = new Node();

            // {
            Utils.WritelineAsync($"pawnIndex : {pawnIndex}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");
            var engine = new EngineMultiThreading(level, cpuColor, IsReprise, Utils.IsMenaced(pawnIndex, boarChess2, cpuColor), SpecifiBoardList);


            // var firstInLastMove = GetTreeLastAction();



            var bestNodeChess2 = engine.SearchThreadForOnce(boarChess2, "5", pawnIndex, level);
            if (bestNodeChess2 != null)
            {
                var node = new Node();
                node.Location = Chess2Utils.GetLocationFromIndex(bestNodeChess2.FromIndex);
                node.BestChildPosition = Chess2Utils.GetLocationFromIndex(bestNodeChess2.ToIndex);
                node.AsssociateNodeChess2 = bestNodeChess2;
                node.Weight = bestNodeChess2.Weight;
                if (level < 4)
                {
                    //var toMenacedWeight = 0;
                    node.IsMenaced = Utils.IsMenaced(bestNodeChess2.ToIndex, boarChess2, cpuColor);
                    //pour T65
                    //pur bestNodListLevel2 on enleve des point aux menacées 
                    // bestNodList.Where(w => w.IsMenaced).ToList().ForEach(x => x.Weight -= x.AssociatePawn.Value);
                    //if(bestNodeChess2.ToIndex == 16)
                    // {
                    //   var t_ = 00;
                    // }
                    if (node.IsMenaced)
                    {

                        //var t_ddv = bestNodeChess2.GetValue();

                        node.Weight -= bestNodeChess2.GetValue();
                        bestNodeChess2.Weight = node.Weight;
                        // var t_dd = node.Weight;
                    }

                }


                //Pour T80 et T81
                var kingIsMenaced = Utils.ComputerKingIsMenaced(node.AsssociateNodeChess2.Board);
                //        Console.WriteLine($"kingIsMenaced : {kingIsMenaced}");
                //        Console.WriteLine($"node.Weight : {node.Weight}");
                if (!kingIsMenaced)
                {




                    Utils.WritelineAsync($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    bestNode = node;
                }



            }



            return bestNode;
        }
        /*tsiry;05-01-2022
        * on prend level 2 de noeud sélectionné pour valider le noeud 2
        * */

        public static Node GetOneNodeLevel4Node(Board boarChess2, int fromIndex, int toIndex, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {


            // {
            Utils.WritelineAsync($"pawnIndex : {fromIndex}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");
            //var cpuColor = ComputerColore[0].ToString();
            var engine = new EngineMultiThreading(4, cpuColor, IsReprise, Utils.IsMenaced(fromIndex, boarChess2, cpuColor), SpecifiBoardList);


            // var firstInLastMove = GetTreeLastAction();



            var bestNodeChess2 = engine.SearchThreadForOnceToOne(boarChess2, "5", fromIndex, toIndex);
            if (bestNodeChess2 != null)
            {
                var node = new Node();
                node.Location = Chess2Utils.GetLocationFromIndex(bestNodeChess2.FromIndex);
                node.BestChildPosition = Chess2Utils.GetLocationFromIndex(bestNodeChess2.ToIndex);
                // node.AssociatePawn = GetPawn(node.Location);
                //Seulment pour les test
                node.AsssociateNodeChess2 = bestNodeChess2;
                node.Weight = bestNodeChess2.Weight;
                //pour T65 seulement pour level == 2
                //si la position est menacée, on enleve 1
                /* if (level == 2)
                 {
                   //var toMenacedWeight = 0;
                   if (Utils.IsMenaced(bestNodeChess2.ToIndex, boarChess2, cpuColor))
                   {
                     //toMenacedWeight = -(boarChess2.GetWeightInIndex(bestNodeChess2.FromIndex));
                     //node.Weight += toMenacedWeight;
                     node.IsMenaced = true;
                   }
                 }*/

                //Pour T80 et T81
                var kingIsMenaced = Utils.ComputerKingIsMenaced(node.AsssociateNodeChess2.Board);

                if (!kingIsMenaced)
                {



                    Utils.WritelineAsync($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    //bestNodList.Add(node);
                    return node;
                }



            }



            return null;

        }


        public static List<Node> GetBestNodeListFromLevel(Board boarChess2, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            var bestNodList = new List<Node>();
            // var boarChess2 = Utils.Clone(boarChessIn);



            //computerPawnsIndex.Add(25);


            //var cpuColor = ComputerColore[0].ToString();


            /* if (level < 4)
             {*/
            var computerPawnsIndex = boarChess2.GetCasesIndexForColor(Utils.ComputerColor);//.OrderBy(x=>x);
            bestNodList.AddRange(EmuleAllIndexInParallelForEach(boarChess2, computerPawnsIndex, level, cpuColor, IsReprise, SpecifiBoardList));                                                                                 // var computerPawnsIndex = new List<int>();

            /* }
             else
              {
                //pour T86: FUITE DE MEMOIR
                //Pour level 4 
                var pawnIndexQueen = boarChess2.GetOneIndex(cpuColor,"Q");
                if(pawnIndexQueen!=-1)
                    bestNodList.Add(EmuleOnlyOneIndex(boarChess2, pawnIndexQueen, level, cpuColor, IsReprise, SpecifiBoardList));
               //GC.Collect();
               var computerPawnsIndexExeptQueen = boarChess2.GetCasesAllIndexExcept(cpuColor, "Q").ToList();

                bestNodList.AddRange(EmuleAllIndexInParallelForEach(boarChess2, computerPawnsIndexExeptQueen, level, cpuColor, IsReprise, SpecifiBoardList));
              }
            */


            // if (level == 2)//pour le level 2, on ne fait pas d'inversion pour les menacée
            //  computerPawnsIndex.RemoveAll(x => Utils.IsMenaced(x, boarChess2));
            //   foreach (var pawnIndex in computerPawnsIndex)


            return bestNodList;

        }


        public static string GetLocationFromIndex(int index)
        {
            return Coord[index];
        }
        public static int GetIndexFromLocation(string index)
        {
            return Coord.ToList().IndexOf(index);
        }


        public static Board GenerateBoardFormPawnList(List<Pawn> pawns)
        {

            try
            {
                var board = new Board();
                foreach (var pawn in pawns)
                {
                    if (String.IsNullOrEmpty(pawn.Location))
                        continue;
                    var index = Coord.ToList().IndexOf(pawn.Location);
                    if (index == -1)
                        continue;
                    var color = pawn.Colore[0].ToString();
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
                    switch (pawn.Name)
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
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
                return null;
            }

        }
        public static BoardGPT GenerateBoardFormPawnListGPT(List<Pawn> pawns)
        {

            try
            {
                var board = new BoardGPT();
                foreach (var pawn in pawns)
                {
                    if (String.IsNullOrEmpty(pawn.Location))
                        continue;
                    var index = Coord.ToList().IndexOf(pawn.Location);
                    if (index == -1)
                        continue;
                    var color = pawn.Colore[0].ToString();
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
                    switch (pawn.Name)
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
                board.Print();
                return board;
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
                return null;
            }

        }

        /*tsiry;19-11-2021
         * pour generer PawnList à part des caseList
         * */
        public static List<string> GeneratePawnStringListFromCaseList(List<string> caseList)
        {
            List<string> pawnStringListResult = new List<string>();
            var index = -1;
            foreach (var item in caseList)
            {
                index++;
                var location = GetLocationFromIndex(index);
                if (item.Contains("|"))
                {
                    var datas = item.Split('|');
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


                    var pawnString = $"{pawnName};{location};{caseColor};False;False;False;False";
                    pawnStringListResult.Add(pawnString);

                }
            }
            return pawnStringListResult;
        }


        public static Board CloneAndMove(Board originalBord, int initialIndex, int destinationIndex, int level)
        {
            var resultBorad = new Board(originalBord);

            resultBorad.Move(initialIndex, destinationIndex);

            //évaluation des scores
            //resultBorad.CalculeScores();


            return resultBorad;
        }


        public static bool IsValideMove(int indexTab120)
        {

            var index120 = Chess2Utils.Tab120[indexTab120];
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
        private static string[] Coord = {
"a8","b8","c8","d8","e8","f8","g8","h8",
"a7","b7","c7","d7","e7","f7","g7","h7",
"a6","b6","c6","d6","e6","f6","g6","h6",
"a5","b5","c5","d5","e5","f5","g5","h5",
"a4","b4","c4","d4","e4","f4","g4","h4",
"a3","b3","c3","d3","e3","f3","g3","h3",
"a2","b2","c2","d2","e2","f2","g2","h2",
"a1","b1","c1","d1","e1","f1","g1","h1"
    };

    }

}
