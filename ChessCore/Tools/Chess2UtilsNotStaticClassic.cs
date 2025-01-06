using System.Collections.Concurrent;

namespace ChessCore.Tools
{
    public class Chess2UtilsNotStaticClassic : IDisposable
    {
        
        
        //Pour T90
        public List<NodeChess2> LevelBlackList { get; set; }
        public int DeepLevel { get; set; } = Utils.DeepLevel;

        public List<Node> EmuleAllIndexInParallelForEach(Board boarChess2, List<int> computerPawnsIndex, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            var opinionColor = cpuColor == "W" ? "B" : "W";
            var bestNodList = new ConcurrentBag<Node>();

            Parallel.ForEach(computerPawnsIndex, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, pawnIndex =>
            {
                Utils.WritelineAsync($"pawnIndex : {pawnIndex}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");

                using (var engine = new EngineMultiThreading(level, cpuColor, IsReprise, Utils.IsMenaced(pawnIndex, boarChess2, cpuColor), SpecifiBoardList))
                {
                    var bestNodeChess2 = engine.SearchThreadForOnce(boarChess2, "5", pawnIndex, level);

                    if (level == DeepLevel && engine.LevelBlackList != null)
                    {
                        if (LevelBlackList == null)
                            LevelBlackList = new List<NodeChess2>();
                        LevelBlackList.AddRange(engine.LevelBlackList);
                    }

                    if (bestNodeChess2 != null)
                    {
                        var node = new Node
                        {
                            Location = Chess2Utils.GetLocationFromIndex(bestNodeChess2.FromIndex),
                            BestChildPosition = Chess2Utils.GetLocationFromIndex(bestNodeChess2.ToIndex),
                            Level = bestNodeChess2.Level,
                            AsssociateNodeChess2 = bestNodeChess2,
                            Weight = bestNodeChess2.Weight
                        };

                        var destinationColor = boarChess2.GetPawnColorNameInIndex(bestNodeChess2.ToIndex);

                        if (destinationColor == opinionColor)
                        {
                            bestNodeChess2.Weight += 1;
                        }

                        node.Weight = bestNodeChess2.Weight;

                        if (level < DeepLevel)
                        {
                            node.IsMenaced = Utils.IsMenaced(bestNodeChess2.ToIndex, boarChess2, cpuColor);

                            if (node.IsMenaced)
                            {
                                node.Weight -= bestNodeChess2.GetValue();
                                bestNodeChess2.Weight = node.Weight;
                            }
                        }

                        var kingIsMenaced = Utils.ComputerKingIsMenaced(node.AsssociateNodeChess2.Board);

                        if (!kingIsMenaced)
                        {
                            Utils.WritelineAsync($"{bestNodeChess2.Weight}  {node.Location} =>  {node.BestChildPosition}");
                            bestNodList.Add(node);
                        }
                    }
                }
            });

            return bestNodList.ToList();
        }


        /*tsiry;07-01-2022
     * on élule la reinne avant les autres
     * */
        public Node EmuleOnlyOneIndex(Board boarChess2, int pawnIndex, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            // var boarChess2 = Utils.Clone(boarChessIn);
            //var cpuColor = Utils.ComputerColor;
            var bestNode = new Node();

            // {
            Utils.WritelineAsync($"pawnIndex : {pawnIndex}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");
            //   EngineMultiThreading engine = null;
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

        public Node GetOneNodeFormSpecificLevel(int level, Board boarChess2, int fromIndex, int toIndex, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {


            // {
            Utils.WritelineAsync($"pawnIndex : {fromIndex}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");
            //var cpuColor = ComputerColore[0].ToString();
            var engine = new EngineMultiThreading(level, cpuColor, IsReprise, Utils.IsMenaced(fromIndex, boarChess2, cpuColor), SpecifiBoardList);


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


        public List<Node> GetBestNodeListFromLevel(Board boarChess2, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            var bestNodList = new List<Node>();



            var computerPawnsIndex = boarChess2.GetCasesIndexForColor(cpuColor).ToList();//.OrderBy(x=>x);

            if (level == 1)
            {
                //T108
                // if (toIndex == 37 && Color == "W" && level == 1)
                //    Weight = 1000;

                foreach (var index in computerPawnsIndex)
                {
                    var possiblesMoves = boarChess2.GetPossibleMoves(index, level).Select(x => x.ToIndex);


                    foreach (var movedIndex in possiblesMoves)
                    {


                        //if (movedIndex == 37)
                        //{
                        //    var t_fd = index;
                        //}

                        var copyAndMovingBord = Utils.CloneAndMove(boarChess2, index, movedIndex, level);
                        //Ajout des neuds
                        var bestNodeChess2 = new NodeChess2(null, copyAndMovingBord, level, Utils.ComputerColor, index, movedIndex, Utils.ComputerColor, 1);

                        if (bestNodeChess2 != null)
                        {
                            var node = new Node
                            {
                                Location = Chess2Utils.GetLocationFromIndex(bestNodeChess2.FromIndex),
                                BestChildPosition = Chess2Utils.GetLocationFromIndex(bestNodeChess2.ToIndex),
                                Level = bestNodeChess2.Level,
                                AsssociateNodeChess2 = bestNodeChess2,
                                Weight = bestNodeChess2.Weight
                            };

                            var destinationColor = boarChess2.GetPawnColorNameInIndex(bestNodeChess2.ToIndex);
                            var opinionColor = "W";
                            if (Utils.ComputerColor == "W")
                                opinionColor = "B";

                            if (destinationColor == opinionColor)
                            {
                                bestNodeChess2.Weight += 1;
                            }

                            node.Weight = bestNodeChess2.Weight;

                            if (level < DeepLevel)
                            {
                                node.IsMenaced = Utils.IsMenaced(bestNodeChess2.ToIndex, boarChess2, cpuColor);

                                if (node.IsMenaced)
                                {
                                    node.Weight -= bestNodeChess2.GetValue();
                                    bestNodeChess2.Weight = node.Weight;
                                }
                            }

                            var kingIsMenaced = Utils.ComputerKingIsMenaced(node.AsssociateNodeChess2.Board);

                            if (!kingIsMenaced)
                            {
                                Utils.WritelineAsync($"{bestNodeChess2.Weight}  {node.Location} =>  {node.BestChildPosition}");
                                bestNodList.Add(node);
                            }
                        }
                    }
                }
            }
            else
                bestNodList.AddRange(EmuleAllIndexInParallelForEach(boarChess2, computerPawnsIndex, level, cpuColor, IsReprise, SpecifiBoardList));
            return bestNodList;

        }

        /// <summary>
        /// tsiry;17-06-2022
        /// pour modificer bestNodListLevel4 en fonction de maxWeight à partir de inList
        /// </summary>
        public void EditBestNodListLevel4FromMaxOtherLevel(in List<Node> bestNodListLevel4, double maxWeight, List<Node> inList)
        {
            var maxInList = inList.Where(x => x.Weight == maxWeight);
            foreach (var node in maxInList)
            {
                //Pour T53 ion ne modifie que les négatives
                bestNodListLevel4.Where(c => c.Location == node.Location && c.BestChildPosition == node.BestChildPosition && c.Weight < 0).Select(c => { c.Weight = node.Weight; return c; }).ToList();

            }
            Utils.WritelineAsync($"bestNodList after edit max");

            foreach (var node in bestNodListLevel4)
            {
                Utils.WritelineAsync($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
            }
        }

        public Node GetBestPositionLocalUsingMiltiThreading(string colore, Board boarChess, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            try
            {
                Utils.WritelineAsync($"DeepLever = {DeepLevel}");
                var startedReflectionTime = DateTime.UtcNow;
                Utils.ComputerColor = colore[0].ToString();
                boarChess.CalculeScores(colore);
                Utils.MainBoard = boarChess;
                Utils.WritelineAsync($"Computer color = {Utils.ComputerColor}");
                Utils.WritelineAsync($"Opinion color = {Utils.OpinionColor}");
                Utils.StartedProcessTime = DateTime.Now;
                //  Utils.Writeline("StartedProcessTime = "+ Utils.StartedProcessTime.ToString("hh:mm:ss tt"));
                Utils.NodeLoseList.Clear();

                var maxBestNodeListLevel4 = new List<Node>();
                //var maxBestNodListLevel3 = new List<Node>();
                //var maxBestNodListLevel2 = new List<Node>();
                var useL2 = false;
                Utils.WritelineAsync($"L{1}--------------------");
                var bestNodeList1 = GetBestNodeListFromLevel(boarChess, 1, Utils.ComputerColor, IsReprise, SpecifiBoardList);
                var maxWeithInLevel1 = bestNodeList1.Max(c => c.Weight);
                bestNodeList1 = bestNodeList1.Where(x => x.Weight == maxWeithInLevel1).ToList();

                /* var bestNodeList3 = new List<Node>();
                 if (DeepLevel > 3)
                 {
                     Utils.WritelineAsync($"L{3}--------------------");
                     bestNodeList3 = GetBestNodeListFromLevel(boarChess, 3, Utils.ComputerColor, IsReprise, SpecifiBoardList);
                     var maxWeithInLevel3 = bestNodeList3.Max(c => c.Weight);
                     bestNodeList3 = bestNodeList3.Where(x => x.Weight == maxWeithInLevel1).ToList();

                 }*/

                Utils.WritelineAsync($"L{DeepLevel}--------------------");
                var bestNodeList = GetBestNodeListFromLevel(boarChess, DeepLevel, Utils.ComputerColor, IsReprise, SpecifiBoardList);

                var loosedNodes = bestNodeList.Where(x => x.Weight <= -999);

                if (bestNodeList1.Count() == 1 && loosedNodes.Count() == 0)
                {
                    var bestInL1 = bestNodeList1.First();
                    var isInBestbestNodeList = bestNodeList.FirstOrDefault(x => x.Location == bestInL1.Location && x.BestChildPosition == bestInL1.BestChildPosition);
                    if (isInBestbestNodeList == null)
                    {
                        bestNodeList.Add(bestInL1);
                    }
                    else// si la difference est > 50
                    {
                        var diff = bestInL1.Weight - bestNodeList.Max(x => x.Weight);
                        if (diff > 50)
                        {
                            bestNodeList.Add(bestInL1);
                        }
                    }

                }

                /*  if (bestNodeList3.Count() == 1 && loosedNodes.Count() == 0 && DeepLevel > 3)
                  {
                      var bestInL3 = bestNodeList3.First();
                      var isInBestbestNodeList = bestNodeList.FirstOrDefault(x => x.Location == bestInL3.Location && x.BestChildPosition == bestInL3.BestChildPosition);
                      if (isInBestbestNodeList == null)
                      {
                          bestNodeList.Add(bestInL3);
                      }
                  }*/



                if (bestNodeList.Count > 0)
                {
                    //pour les Wins T93A,T07a,T07b,T95B
                    if (bestNodeList.All(x => x.Weight >= 999))
                    {
                        var groupedBySameFromLoactionAndSameBestChildPosition = bestNodeList.GroupBy(v => new
                        {
                            FromLoaction = v.Location,
                            BestChildPosition = v.BestChildPosition
                        });
                        var bestNode = groupedBySameFromLoactionAndSameBestChildPosition.OrderByDescending(x => x.Count()).First().First();




                        Utils.WritelineAsync("--WIN NODE DETECTED--");
                        // nodeResult.Weight = t_.Weight;
                        Utils.WritelineAsync($"{bestNode.Weight}  {bestNode.Location} =>  {bestNode.BestChildPosition}");

                        return bestNode;
                    }

                    if (Utils.NodeLoseList.Count > 0)
                    {
                        //T98
                        try
                        {
                            Utils.WritelineAsync("--LOSE NODE DETECTED--");
                            //var firtNode2Win= Utils.NodeWinList.First();
                            foreach (var node2 in Utils.NodeLoseList)
                            {
                                var nodeWinResult = new Node();
                                nodeWinResult.Location = Chess2Utils.GetLocationFromIndex(node2.FromIndex);
                                nodeWinResult.BestChildPosition = Chess2Utils.GetLocationFromIndex(node2.ToIndex);
                                nodeWinResult.Weight = node2.Weight;
                                Utils.WritelineAsync($"{nodeWinResult.Weight}  {nodeWinResult.Location} =>  {nodeWinResult.BestChildPosition}");
                            }

                        }
                        catch (Exception)
                        {
                            Utils.WritelineAsync("DONT PANIC: too mach loseds nodes detected (loseds nodes ignoreds)");
                        }

                    }
                }
                else if (DeepLevel > 3)//si tous les noeud sont des lose, on fait un nouveau recherche sur les L2
                {
                    Utils.WritelineAsync($"all L4 are losing using L2");
                    useL2 = true;

                    bestNodeList = GetBestNodeListFromLevel(boarChess, 2, Utils.ComputerColor, IsReprise, SpecifiBoardList);
                }
                var maxWeight = 0.0;
                //  if(bestNodListLevel4.Count>0)
                maxWeight = bestNodeList.Max(x => x.Weight);



                Utils.WritelineAsync($"bestNodList ");

                foreach (var node in bestNodeList)
                {
                    Utils.WritelineAsync($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                }



                if (!useL2)//si on utilse le L2, on ne prend pas en charge le LevelBlackList
                {
                    if (Utils.NodeLoseList.Count > 0)
                    {
                        if (LevelBlackList == null)
                            LevelBlackList = new List<NodeChess2>();
                        //Pour T29, on ajoute Utils.NodeLoseList
                        LevelBlackList.AddRange(Utils.NodeLoseList);
                    }
                    if (LevelBlackList != null)
                    {
                        Utils.WritelineAsync("LevelBlackList--------");
                        foreach (var node in LevelBlackList)
                        {
                            if (node == null)
                                continue;
                            Utils.WritelineAsync($"fromIndex : {node.FromIndex} => toIndex : {node.ToIndex} = {node.Weight}");
                        }

                    }



                    if (LevelBlackList != null)
                    {
                        if (bestNodeList.Count > 0)
                        {
                            //Pour T88 on modifie les les Level4BlackList
                            foreach (var node in LevelBlackList)
                            {
                                if (node == null)
                                    continue;
                                bestNodeList.Where(x => x.AsssociateNodeChess2.FromIndex == node.FromIndex && x.AsssociateNodeChess2.ToIndex == node.ToIndex && node.Weight < -900).Select(c => { c.Weight = node.Weight; return c; }).ToList();

                            }


                        }
                    }
                }

                var maxWeithLevel4 = -1111.0;
                maxWeithLevel4 = bestNodeList.Max(x => x.Weight);
                maxBestNodeListLevel4 = bestNodeList.Where(x => x.Weight == maxWeithLevel4).ToList();




                var maxWeithList = new List<Node>();
                maxWeithList.AddRange(maxBestNodeListLevel4);







                var maxWeith = maxWeithList.Max(x => x.Weight);
                maxWeithList = maxWeithList.Where(x => x.Weight == maxWeith).ToList();

                Utils.WritelineAsync("--Best for all Levels--");
                foreach (var node in maxWeithList)
                {
                    // nodeResult.Weight = t_.Weight;
                    Utils.WritelineAsync($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");



                }
                var reflextionTime = DateTime.UtcNow - startedReflectionTime;
                Utils.WritelineAsync($"Reflection time : {reflextionTime}");

                var nodeResult = new Node();

                if (maxWeithList.Count() >= 1)
                {
                    //pour T94, on ajoute le nombre des pions protégers dans Weight
                    //on cherche ne nombre de protege

                    //foreach (var node in maxWeithList)
                    //{

                    //    var protectedNumber = node.AsssociateNodeChess2.GetProtectedNumber();
                    //    node.Weight += protectedNumber;


                    //}
                    maxWeith = maxWeithList.Max(x => x.Weight);
                    maxWeithList = maxWeithList.Where(x => x.Weight == maxWeith).ToList();
                    if (maxWeithList.Count() == 1)
                    {
                        nodeResult = maxWeithList.First();
                        nodeResult.AsssociateNodeChess2.RandomEquivalentList.Clear();
                    }

                    else
                    {
                        //random
                        var rand = new Random();
                        nodeResult = maxWeithList.ToList()[rand.Next(maxWeithList.Count())];
                        nodeResult = maxWeithList.First();
                        foreach (var node2 in maxWeithList)
                        {
                            nodeResult.AsssociateNodeChess2.RandomEquivalentList.Add(node2.AsssociateNodeChess2);
                        }
                    }
                }
                else
                    nodeResult = maxWeithList.First();


                return nodeResult;


            }
            catch (Exception ex)
            {
                //WriteInLog(ex.ToString());
                return null;
                //return GetBestPositionLocalNotTaskUsingMiltiThreading(colore);

            }



        }


        

        public List<Pawn> GeneratePawnListFromBoard(Board inBoard)
        {
            try
            {
                var pawnList = new List<Pawn>();
                foreach (var line in inBoard.GetCases())
                {

                    if (!line.Contains("|"))
                        continue;
                    var datas = line.Split(';');
                    var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                    //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                    newPawn.IsFirstMove = true;
                    newPawn.IsFirstMoveKing = true;
                    newPawn.IsLeftRookFirstMove = true;
                    newPawn.IsRightRookFirstMove = true;
                    pawnList.Add(newPawn);

                }

                return pawnList;

            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
                return null;
            }
        }



        public Board GenerateBoardFormPawnList(List<Pawn> pawns)
        {

            try
            {
                var board = new Board();
                foreach (var pawn in pawns)
                {
                    if (String.IsNullOrEmpty(pawn.Location))
                        continue;
                    var index = CoordTools.Coord.ToList().IndexOf(pawn.Location);
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
                //board.PrintInDebug();
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
        public List<string> GeneratePawnStringListFromCaseList(List<string> caseList)
        {
            List<string> pawnStringListResult = new List<string>();
            var index = -1;
            foreach (var item in caseList)
            {
                index++;
                var location = CoordTools.GetLocationFromIndex(index);
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


        public Board CloneAndMove(Board originalBord, int initialIndex, int destinationIndex, int level)
        {
            var resultBorad = new Board(originalBord);

            resultBorad.Move(initialIndex, destinationIndex);

            //évaluation des scores
            //resultBorad.CalculeScores();


            return resultBorad;
        }


        public bool IsValideMove(int indexTab120)
        {

            var index120 = Chess2Utils.Tab120[indexTab120];
            if (index120 == -1)
                return false;
            return true;
        }

        public bool IsInFirstTab64(int tab64Content, string color)
        {
            if (color == "W")
            {
                if (CoordTools.SimplePawnFirstWhiteTab64.Contains(tab64Content))
                    return true;
            }
            if (color == "B")
            {
                if (CoordTools.SimplePawnFirstBlackTab64.Contains(tab64Content))
                    return true;
            }
            return false;


        }

        public void Dispose()
        {
            //Utils.GCColect();
        }

    }
}
