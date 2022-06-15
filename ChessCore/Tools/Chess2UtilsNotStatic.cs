using System.Diagnostics;

namespace ChessCore.Tools
{
    public class Chess2UtilsNotStatic : IDisposable
    {
        //Pour T90
        public List<NodeChess2> Level4BlackList { get; set; }

        public List<Node> EmuleAllIndexInParallelForEach(Board boarChess2, List<int> computerPawnsIndex, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
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
                Debug.WriteLine("pawnIndex : {0}, Thread Id= {1}", pawnIndex, Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("pawnIndex : {0}, Thread Id= {1}", pawnIndex, Thread.CurrentThread.ManagedThreadId);

                using (var engine = new EngineMultiThreading(level, cpuColor, IsReprise, Utils.IsMenaced(pawnIndex, boarChess2, cpuColor), SpecifiBoardList))
                {
                    // var firstInLastMove = GetTreeLastAction();



                    var bestNodeChess2 = engine.SearchThreadForOnce(boarChess2, "5", pawnIndex, level);
                    //Pour T90 et T91
                    if (level == 4 && engine.Level4BlackList != null)
                    {
                        if (Level4BlackList == null)
                            Level4BlackList = new List<NodeChess2>();
                        Level4BlackList.AddRange(engine.Level4BlackList);
                        // var t_minNodeList = engine.Level4MinList;
                    }

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

                            Debug.WriteLine($"{bestNodeChess2.Weight}  {node.Location} =>  {node.BestChildPosition}");
                            Console.WriteLine($"{bestNodeChess2.Weight}  {node.Location} =>  {node.BestChildPosition}");
                            bestNodList.Add(node);
                        }



                    }
                }





            });

            return bestNodList;
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
            Debug.WriteLine("pawnIndex : {0}, Thread Id= {1}", pawnIndex, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("pawnIndex : {0}, Thread Id= {1}", pawnIndex, Thread.CurrentThread.ManagedThreadId);
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




                    Debug.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    Console.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    bestNode = node;
                }



            }



            return bestNode;
        }
        /*tsiry;05-01-2022
        * on prend level 2 de noeud sélectionné pour valider le noeud 2
        * */

        public Node GetOneNodeLevel2Node(Board boarChess2, int fromIndex, int toIndex, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {


            // {
            Debug.WriteLine("pawnIndex : {0}, Thread Id= {1}", fromIndex, Thread.CurrentThread.ManagedThreadId);
            //var cpuColor = ComputerColore[0].ToString();
            var engine = new EngineMultiThreading(2, cpuColor, IsReprise, Utils.IsMenaced(fromIndex, boarChess2, cpuColor), SpecifiBoardList);


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



                    Debug.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    //bestNodList.Add(node);
                    return node;
                }



            }



            return null;

        }


        public List<Node> GetBestNodeListFromLevel(Board boarChess2, int level, string cpuColor, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            var bestNodList = new List<Node>();
            // var boarChess2 = Utils.Clone(boarChessIn);



            //computerPawnsIndex.Add(25);


            //var cpuColor = ComputerColore[0].ToString();


            /* if (level < 4)
             {*/
            var computerPawnsIndex = boarChess2.GetCasesIndex(cpuColor).ToList();//.OrderBy(x=>x);
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


        public Node GetBestPositionLocalUsingMiltiThreading(string colore, Board boarChess, bool IsReprise, List<SpecificBoard> SpecifiBoardList)
        {
            try
            {
                Debug.WriteLine("L2 InChess--------------------");
                //Console.WriteLine("L4--------------------");
                var L2InChessList = GetBestNodeListFromLevel(boarChess, 2, colore[0].ToString(), IsReprise, SpecifiBoardList);
                L2InChessList = L2InChessList.Where(x => x.Weight <= -990).ToList();
              
                //Pour les BestsPosition
                // GC.Collect();
                Utils.ComputerColor = colore[0].ToString();
                Console.WriteLine($"Computer color = {Utils.ComputerColor}");

                /*//TODO A DECOMMENTER SpecificPositionsList
                if (SpecificPositionsList != null)
                {
                  var specifiBoardList = new List<SpecificBoard>();
                  foreach (var item in SpecificPositionsList)
                  {
                    var specificBoard = new SpecificBoard();

                    specificBoard.Color = item.Color[0].ToString();
                    specificBoard.Score = item.Score;
                    specificBoard.SpecificsBoard = item.SpecificsBoard;
                    specifiBoardList.Add(specificBoard);
                  }
                }
                */
                // var boarChess = Chess2Utils.GenerateBoardFormPawnList(this.PawnList);
                var bestNodList = new List<Node>();

                //TODO A DEPLACER
                //Debug.WriteLine("L4------ a1 =>  a6");
                //var fromIndex0 = Utils.GetIndexFromLocation("a1");
                //var toIndex0 = Utils.GetIndexFromLocation("a6");
                //var bestNodeL3One0 = GetOneNodeLevel3Node(boarChess2, fromIndex0, toIndex0);




                   //  var t_blsd = GetBestNodeListFromLevel(boarChess, 2, "B", IsReprise, SpecifiBoardList);
                Debug.WriteLine("L4--------------------");
                Console.WriteLine("L4--------------------");
                var maxBestNodListLevel4 = new List<Node>();
                var maxBestNodListLevel3 = new List<Node>();
                var maxBestNodListLevel2 = new List<Node>();
                var bestNodListLevel4 = GetBestNodeListFromLevel(boarChess, 4, Utils.ComputerColor, IsReprise, SpecifiBoardList);

                // pour T95B on modifie le pour des L2InChessList dans bestNodListLevel4
                Debug.WriteLine($"bestNodListLevel4 after edit for in chess in L2");
                foreach (var node in L2InChessList)
                {
                    bestNodListLevel4.Where(c => c.Location == node.Location && c.BestChildPosition == node.BestChildPosition).Select(c => { c.Weight = node.Weight; return c; }).ToList();

                }
                foreach (var node in bestNodListLevel4)
                {
                    Debug.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    Console.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                }

           
                //Level4MinList.AddRange(Level4MinList);
                //var t_level4MinList = Level4MinList.Distinct();
                Debug.WriteLine("Level4BlackList--------");
                Console.WriteLine("Level4BlackList--------");
                foreach (var node in Level4BlackList)
                {
                    Debug.WriteLine($"fromIndex : {node.FromIndex} => toIndex : {node.ToIndex} = {node.Weight}");
                    Console.WriteLine($"fromIndex : {node.FromIndex} => toIndex : {node.ToIndex} = {node.Weight}");

                }

                
                var maxWeithLevel4 = -1111.0;
                var maxWeithLevel3 = -1111.0;
                if (bestNodListLevel4 != null)
                {
                    if (bestNodListLevel4.Count > 0)
                    {
                       

                        maxWeithLevel4 = bestNodListLevel4.Max(x => x.Weight);
                        maxBestNodListLevel4 = bestNodListLevel4.Where(x => x.Weight == maxWeithLevel4).ToList();
                    }
                }



                //Pour T87 Win
                //si pour L4 il y a un noeud de 999
                //on ne passe plus par L3 et L2
                var isNeedL3L2 = true;
                if (maxBestNodListLevel4.Count > 0)
                {
                    if (maxBestNodListLevel4.Max(x => x.Weight) >= 999)
                    {
                        isNeedL3L2 = false;
                    }
                }

                //Pour T87 protect Win
                //si pour L4 il y plus des 1/2 des noeuds qui ont -999
                //on ne passe plus par L3 et L2
                
                if (bestNodListLevel4.Count > 0)
                {
                   
                    var inChessNumber = bestNodListLevel4.Where(x => x.Weight == -999).ToList().Count;
                    var half = bestNodListLevel4.Count / 2;
                    if (inChessNumber >= half)
                        isNeedL3L2 = false;

                }

                //pour T95B si le L2InChessList n'est pas vide
                //on ne prond plus les autres noeuds
                //Pour T74, on ne que si c'est supperieur à 1
                if (L2InChessList.Count>1)
                       isNeedL3L2 = false;
                if (isNeedL3L2)
                {
                    Debug.WriteLine("L3--------------------");
                    Console.WriteLine("L3--------------------");
                    var bestNodeListLevel3 = new List<Node>();
                    bestNodeListLevel3 = GetBestNodeListFromLevel(boarChess, 3, Utils.ComputerColor, IsReprise, SpecifiBoardList);
                    //Pour T90 et T91
                    foreach (var bestNodeL in bestNodeListLevel3)
                    {
                        var contain = Level4BlackList.FirstOrDefault(x => x.FromIndex == bestNodeL.AsssociateNodeChess2.FromIndex && x.ToIndex == bestNodeL.AsssociateNodeChess2.ToIndex);
                        if (contain != null)
                        {
                            var diff = contain.Weight + bestNodeL.Weight;
                            if (diff < 0)
                                bestNodeL.Weight = contain.Weight;
                        }

                    }
                    



                    if (bestNodeListLevel3.Count > 0)
                    {


                        maxWeithLevel3 = bestNodeListLevel3.Max(x => x.Weight);
                        maxBestNodListLevel3 = bestNodeListLevel3.Where(x => x.Weight == maxWeithLevel3).ToList();


                        //pour T78
                        //si maxBestNodListLevel3 count ==1
                        //si maxBestNodListLevel3 se trouve dans maxBestNodListLevel4
                         if (maxBestNodListLevel3.Count == 1)
                         {
                           var bestNodeInlevel3 = maxBestNodListLevel3.First();
                           var equivalentInLevel4 = bestNodListLevel4.FirstOrDefault(x => x.Location == bestNodeInlevel3.Location && x.BestChildPosition == bestNodeInlevel3.BestChildPosition);
                           if (equivalentInLevel4 != null)
                           {
                             var diff = bestNodeInlevel3.Weight - equivalentInLevel4.Weight;
                             //si diff positif, on ajout bestNodeInlevel3 dans maxBestNodListLevel4
                             if (diff > 0)
                             {
                               maxBestNodListLevel4.Add(bestNodeInlevel3);
                             }
                           }

                         }

                       

                    }




                    Debug.WriteLine("L2--------------------");
                    Console.WriteLine("L2--------------------");
                    var bestNodeListLevel2 = new List<Node>();
                    bestNodeListLevel2 = GetBestNodeListFromLevel(boarChess, 2, Utils.ComputerColor, IsReprise, SpecifiBoardList);
                    //Pour T90 et T91
                    foreach (var bestNodeL in bestNodeListLevel2)
                    {
                        var contain = Level4BlackList.FirstOrDefault(x => x.FromIndex == bestNodeL.AsssociateNodeChess2.FromIndex && x.ToIndex == bestNodeL.AsssociateNodeChess2.ToIndex);

                        if (contain != null)
                        {
                            var diff = contain.Weight + bestNodeL.Weight;
                            if (diff < 0)
                                bestNodeL.Weight = contain.Weight;
                        }
                       
                    }

                    var toDeleteForL3L2Node = bestNodListLevel4.Where(x => x.Weight <= -900);
                    //pour T29 on enleve de bestNodListLevel3 et de bestNodListLevel2
                    //des noeuds -999 de bestNodListLevel4
                    foreach (var toDeleteNode in toDeleteForL3L2Node)
                    {
                        bestNodeListLevel3.RemoveAll(x => x.Location == toDeleteNode.Location && x.BestChildPosition == toDeleteNode.BestChildPosition);
                    }
                   
                    //pour T88
                    //on supprime aussi de l3 les eches en l2
                    //-998  e5 =>  c7
                    /* var toDeleteForL3NodeFromL2 = bestNodListLevel2.Where(x => x.Weight <= -900);
                     foreach (var toDeleteNode in toDeleteForL3NodeFromL2)
                     {
                       bestNodListLevel3.RemoveAll(x => x.Location == toDeleteNode.Location && x.BestChildPosition == toDeleteNode.BestChildPosition);
                     }*/







                    //pour T29 on enleve de bestNodListLevel3 et de bestNodListLevel2
                    //des noeuds -999 de bestNodListLevel4
                    foreach (var toDeleteNode in toDeleteForL3L2Node)
                    {
                        bestNodeListLevel2.RemoveAll(x => x.Location == toDeleteNode.Location && x.BestChildPosition == toDeleteNode.BestChildPosition);
                    }
                    var maxWeithLevel2 = -1111.0;

                    if (bestNodeListLevel2.Count > 0)
                    {
                        maxWeithLevel2 = bestNodeListLevel2.Max(x => x.Weight);
                        maxBestNodListLevel2 = bestNodeListLevel2.Where(x => x.Weight == maxWeithLevel2).ToList();
                    }


                }
                
                var maxWeithList = new List<Node>();
                maxWeithList.AddRange(maxBestNodListLevel4);

                

                //Pour T32 et T33 si maxL4 == maxL2 et positive on ne prend plus L3 et L2
                if (maxBestNodListLevel4.Count == 1 && maxBestNodListLevel2.Count == 1)
                {
                    var firstL4 = maxBestNodListLevel4.First();
                    var firstL2 = maxBestNodListLevel2.First();
                    if (firstL4.Weight > 0 && (firstL4.Location == firstL2.Location && firstL4.BestChildPosition == firstL2.BestChildPosition))
                    {
                        isNeedL3L2 = false;
                    }
                }

                if (isNeedL3L2)
                {
                    //Pour T49 on ajoute L3 si max L3 est suppérieur à max L4
                    if (maxWeithLevel3 > maxWeithLevel4)
                    {
                        maxWeithList.AddRange(maxBestNodListLevel3);

                     
                      
                    }



                    maxWeithList.AddRange(maxBestNodListLevel2);
                }



                //T53 pour tous L3 dans L4, on modifie le poids dans L3
                //T92 on ne modifie que les L3 dans L4 et pas les L2
                //foreach (var node in maxBestNodListLevel3)
                
                for (int i = 0; i < maxWeithList.Count; i++)
                {
                    var node = maxWeithList[i];


                    /* if (node.Location == "e7" && node.BestChildPosition == "e6")
                     {
                       var t_ = 54;
                     }*/
                    var nodeInL4 = bestNodListLevel4.FirstOrDefault(x => x.Location == node.Location && x.BestChildPosition == node.BestChildPosition && node.Level == 3);
                    if (nodeInL4 != null)
                    {


                        Debug.WriteLine($"{node.Location} => {node.BestChildPosition}    {node.Weight} To {nodeInL4.Weight}");
                        Console.WriteLine($"{node.Location} => {node.BestChildPosition}    {node.Weight} To {nodeInL4.Weight}");

                        var diff = nodeInL4.Weight + node.Weight; //-4+1  -4 +3 
                        if (diff < -1)
                        {
                            // -4  e7 => e6
                            // Debug.WriteLine("To l4");
                            // Console.WriteLine("To l4");
                            //Quand best node est aussi présent dans L2 et L3, on ne l'enleve pas
                            node.Weight = nodeInL4.Weight;
                        }
                        //node.Weight = nodeInL4.Weight;
                    }

                }
               
                var maxWeith = maxWeithList.Max(x => x.Weight);
                maxWeithList = maxWeithList.Where(x => x.Weight == maxWeith).ToList();

                Console.WriteLine("--Best for all Levels--");
                Debug.WriteLine("--Best for all Levels--");
                foreach (var node in maxWeithList)
                {
                    Debug.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");
                    Console.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition}");



                }

                var nodeResult = new Node();

                if (maxWeithList.Count() > 1)
                {
                    //pour T94, on ajoute le nombre des pions protégers dans Weight
                    //on cherche ne nombre de protege
                     
                     foreach (var node in maxWeithList)
                     {
                        
                             var protectedNumber = node.AsssociateNodeChess2.GetProtectedNumber();
                             //Debug.WriteLine($"{node.Weight}  {node.Location} =>  {node.BestChildPosition} protectedNumber = {protectedNumber}");
                             node.Weight += protectedNumber;
                        

                     }
                    maxWeith = maxWeithList.Max(x => x.Weight);
                    maxWeithList = maxWeithList.Where(x => x.Weight == maxWeith).ToList();
                    if (maxWeithList.Count() == 1)
                        nodeResult = maxWeithList.First();
                    else
                    {
                        foreach (var node2 in maxWeithList)
                        {
                            nodeResult.AsssociateNodeChess2.RandomEquivalentList.Add(node2.AsssociateNodeChess2);
                        }
                        //rondom
                        var rand = new Random();
                        nodeResult = maxWeithList.ToList()[rand.Next(maxWeithList.Count())];
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


        public string GetLocationFromIndex(int index)
        {
            return Coord[index];
        }
        public int GetIndexFromLocation(string index)
        {
            return Coord.ToList().IndexOf(index);
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
                    var newPawn = new  Pawn(datas[0], datas[1],  datas[2]);
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
                Debug.WriteLine(ex.ToString());
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
                Debug.WriteLine(ex.ToString());
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


        public Board CloneAndMove(Board originalBord, int initialIndex, int destinationIndex, int level)
        {
            var resultBorad = new Board(originalBord);

            resultBorad.Move(initialIndex, destinationIndex, level);

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

        public void Dispose()
        {
            Debug.WriteLine("Memory used before collection:       {0:N0}",
         GC.GetTotalMemory(false));
           /* Console.WriteLine("Memory used before collection:       {0:N0}",
                    GC.GetTotalMemory(false));*/
            Debug.WriteLine("Collect");

            GC.Collect();
            Debug.WriteLine("Memory used before collection:       {0:N0}",
                  GC.GetTotalMemory(false));
         /*   Console.WriteLine("Memory used before collection:       {0:N0}",
                    GC.GetTotalMemory(false));*/
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
