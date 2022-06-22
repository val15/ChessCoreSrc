using System.Diagnostics;

namespace ChessCore.Tools
{
    public class EngineMultiThreading : IDisposable
    {

        public List<NodeChess2> level1NodeList { get; set; }
        public bool IsSecond { get; set; } = true;

        public bool FromIndexIsNenaced { get; set; } = false;

        public string ComputerColore { get; set; }
        public string OpinionrColore { get; set; }
        public Board MainBord { get; set; }


        public List<SpecificBoard> SpecifiBoardList { get; set; }

        public List<NodeChess2> Level4BlackList { get; set; }


        public int DeepLevel { get; set; }

        //private int _oldW;
        private bool _isReprise = false;




        public List<NodeChess2> LastNodes { get; set; }

        public NodeChess2 BestNode { get; set; }
        public NodeChess2 BestNodeSecond { get; set; }

        public NodeChess2 NotBestNode { get; set; }

        public bool IsParallelMode { get; set; }




        public EngineMultiThreading(int deepLevel, string computerColore, bool isReprise, bool fromIndexIsNenaced, List<SpecificBoard> specifiBoardList = null, bool isInTest = false)
        {
            FromIndexIsNenaced = fromIndexIsNenaced;
            _isReprise = isReprise;
            DeepLevel = deepLevel;
            LastNodes = new List<NodeChess2>();
            ComputerColore = computerColore;
            OpinionrColore = "W";
            if (ComputerColore == "W")
                OpinionrColore = "B";
            SpecifiBoardList = specifiBoardList;
            Utils.ComputerColor = computerColore;
        }

        /*tsiry;27-09-2021
         * */
        public bool IsEquals(Board board1, Board board2)
        {
            var caseList1 = board1.GetCases();
            var caseList2 = board2.GetCases();
            for (int i = 0; i < caseList1.Count(); i++)
            {
                if (caseList1[i] != caseList2[i])
                    return false;
            }



            return true;
        }

        /*tsiry;21-12-2021
            * Search pour le multi treading
            * on ne simule que l'actuel index
            * */
        public NodeChess2 SearchThreadForOnce(Board mainBord, string tunNumber, int pawnIndex, int level)
        {
            MainBord = mainBord;
            // Utils.MainBord = MainBord;
            //TEST print board
            //  MainBord.Print();

            //Si reprise
            // IsReprise = true;

            //Pour les sécifiques positions
            //Specifiques positions
            if (SpecifiBoardList != null && level == DeepLevel)//on ne prend de le level le plus haut
            {
                EmuleLevel1ForSpecifiquePosition(MainBord, pawnIndex);

            }



            if (BestNode == null)
            {
                if ((tunNumber == "1" || tunNumber == "0") && !_isReprise)
                    EmulAllPossibleMovesForFirstTurn(null, mainBord, ComputerColore, 0, -1, -1);
                else
                    EmulAllPossibleMovesForOnce(null, mainBord, ComputerColore, 0, pawnIndex);

                // if (BestNode == null)
                MinMaxByRoot();
            }

            /*if (BestNode != null)
            {
              Console.WriteLine($"L ZEZO : {BestNode.Level};{BestNode.Weight};{BestNode.Color}\t\t{BestNode.FromIndex}=>{BestNode.ToIndex}");

            }*/

            return BestNode;
        }

        /*tsiry;05-01-2022
         * SearchThreadForOnce pour une seule destination
         * */
        public NodeChess2 SearchThreadForOnceToOne(Board mainBord, string tunNumber, int pawnIndex, int toIndex)
        {
            MainBord = mainBord;
            // Utils.MainBord = MainBord;

            //TEST print board
            // MainBord.Print();

            //Si reprise
            // IsReprise = true;

            if ((tunNumber == "1" || tunNumber == "0") && !_isReprise)
                EmulAllPossibleMovesForFirstTurn(null, mainBord, ComputerColore, 0, -1, -1);
            else
                EmulAllPossibleMovesForOnceToOne(null, mainBord, ComputerColore, 0, pawnIndex, toIndex);

            if (BestNode == null)
                MinMaxByRoot();
            if (BestNode != null)
            {
                Console.WriteLine($"L ZEZO : {BestNode.Level};{BestNode.Weight};{BestNode.Color}\t\t{BestNode.FromIndex}=>{BestNode.ToIndex}");

            }

            return BestNode;
        }


        /*tsiry;11-01-2022
       * pour cherche les best positions
       * 
       * */
        public void EmuleLevel1ForSpecifiquePosition(Board inBord, int pawnIndex)
        {


            var possiblesMoves = inBord.GetPossibleMoves(pawnIndex, 1).Select(x => x.Index);
            foreach (var movedIndex in possiblesMoves)
            {

                ////Console.WriteLine($"{index} => {movedIndex}");
                ////Console.WriteLine($"\t L : {level}");
                var copyAndMovingBord = Utils.CloneAndMove(inBord, pawnIndex, movedIndex, 1);





                foreach (var specific in SpecifiBoardList)
                {

                    var result = IsEquals(specific.SpecificsBoard, copyAndMovingBord);
                    //var nodeColor = "W";
                    //if (color == "W")
                    //  nodeColor = "B";
                    /* var localComputeurColor = "W";
                     if (ComputerColore == "Black")
                       localComputeurColor = "B";*/


                    if (result)
                    {
                        if (((ComputerColore == specific.Color[0].ToString())))
                        {
                            /*var resultArray0 = new int[2] { specifics.Score.Value, makeCheckmateLevel };
                            return null;*/

                            //si on trouve une best position
                            BestNode = new NodeChess2();
                            BestNode.FromIndex = pawnIndex;
                            BestNode.ToIndex = movedIndex;
                            BestNode.Weight = specific.Weight;
                            return;
                        }
                        else
                        {
                            /*var resultArray0 = new int[2] { -specifics.Score.Value, makeCheckmateLevel };
                            return null;*/
                            //var score = specifics.Score.Value;
                            NotBestNode = new NodeChess2();
                            NotBestNode.FromIndex = pawnIndex;
                            NotBestNode.ToIndex = movedIndex;
                            NotBestNode.Weight = -specific.Weight;
                            return;
                        }
                    }
                }




            }






        }

        /*tsiry;21-12-2021
         * pour SearchThreadForOnce
         * emulation des pawns du cpu
         * */
        public void EmulAllPossibleMovesForOnce(NodeChess2 parentNode, Board board, string color, int level, int currentPawnIndex)
        {




            if (level == 0)
            {
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, DeepLevel);
                node.Weight = 0;
                //LastNodes.Add(node);
                parentNode = node;
            }






            level++;





            var index = currentPawnIndex;
            // _oldW = 9000;


            var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.Index);
            foreach (var movedIndex in possiblesMoves)
            {

                ////Console.WriteLine($"{index} => {movedIndex}");
                ////Console.WriteLine($"\t L : {level}");
                var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);










                //Ajout des neuds
                var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, DeepLevel);
                parentNode.ChildList.Add(newNode);


                //if (newNode.Level == DeepLevel)
                //{

                //  //LastNodes.Add(newNode);

                //  //if (_oldW < newNode.Weight)
                //  //{
                //    LastNodes.Add(newNode);
                //  //  _oldW = newNode.Weight;
                // // }



                //}





                var newColor = "B";
                if (color == "B")
                    newColor = "W";

                /* if (Chess2Console.Utils.KingIsInChess2(copyAndMovingBord, Chess2Console.Utils.OpinionColor))
                 {
                   var t_ = 1;
                 }

                 if (Chess2Console.Utils.KingIsInChess2(copyAndMovingBord, Chess2Console.Utils.ComputerColor))
                 {
                   var t_ = 1;
                 }*/
                EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
            }





        }

        /*tsiry;05-01-2022
         * SearchThreadForOnce pour une seule destination
         * */
        public void EmulAllPossibleMovesForOnceToOne(NodeChess2 parentNode, Board board, string color, int level, int currentPawnIndex, int toIndex)
        {




            if (level == 0)
            {
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, DeepLevel);

                node.Weight = 0;
                //LastNodes.Add(node);
                parentNode = node;
            }






            level++;





            var index = currentPawnIndex;
            // _oldW = 9000;


            var possiblesMoves = new List<int>();//board.GetPossibleMoves(index, level).Select(x => x.Index);

            possiblesMoves.Add(toIndex);
            foreach (var movedIndex in possiblesMoves)
            {

                ////Console.WriteLine($"{index} => {movedIndex}");
                ////Console.WriteLine($"\t L : {level}");
                var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);










                //Ajout des neuds
                var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, DeepLevel);
                /*if (newNode.Weight == 999999)
                {
                    var t_ = "ds";
                }*/
                parentNode.ChildList.Add(newNode);


                //if (newNode.Level == DeepLevel)
                //{

                //  //LastNodes.Add(newNode);

                //  //if (_oldW < newNode.Weight)
                //  //{
                //    LastNodes.Add(newNode);
                //  //  _oldW = newNode.Weight;
                // // }



                //}





                var newColor = "B";
                if (color == "B")
                    newColor = "W";

                /* if (Chess2Console.Utils.KingIsInChess2(copyAndMovingBord, Chess2Console.Utils.OpinionColor))
                 {
                   var t_ = 1;
                 }

                 if (Chess2Console.Utils.KingIsInChess2(copyAndMovingBord, Chess2Console.Utils.ComputerColor))
                 {
                   var t_ = 1;
                 }*/
                EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
            }





        }


        public void EmulAllPossibleMovesFor(NodeChess2 parentNode, Board board, string color, int level, int fromIndex, int toIndex, int fromIndexNotValide = -1, int toIndexNotValide = -1)
        {
            //if (Chess2Console.Utils.KingIsInChess2(board, Chess2Console.Utils.ComputerColor))
            //  return;
            /* if (Chess2Console.Utils.KingIsInChess2(board, Chess2Console.Utils.OpinionColor))
             {
               return;
             }

             if (Chess2Console.Utils.KingIsInChess2(board, Chess2Console.Utils.ComputerColor))
             {
               return;
             }*/
            if (level == 0)
            {
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, DeepLevel);
                node.Weight = 0;
                //LastNodes.Add(node);
                parentNode = node;
            }

            if (NotBestNode != null)
            {
                if (fromIndex == NotBestNode.FromIndex && toIndex == NotBestNode.ToIndex)
                {
                    return;
                }
            }






            if (level == DeepLevel)
                return;
            level++;





            var pawns = board.GetCasesIndex(color);
            // var opinionIsInChess = board.IsInChess(opinionColor);
            foreach (var index in pawns)
            {
                // _oldW = -9000;

                var elementInIndex = board.GetCases().ElementAt(index);


                //ByPass pour DeepLevel == 4
                /* if (DeepLevel == 4)
                 {
                   if (elementInIndex.Contains("Q") && level == 4)
                     continue;
                 }*/

                var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.Index);
                foreach (var movedIndex in possiblesMoves)
                {
                    ////Console.WriteLine($"{index} => {movedIndex}");
                    ////Console.WriteLine($"\t L : {level}");
                    var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);


                    /////copyAndMovingBord.Print();
                    //si Board est menacer et copyAndMovingBord aussi mencer, on ne l'éxpoite plus

                    /*  if (!_isChessInNextLevel)
                      {
                        if (isInChess && !opinionIsInChess)
                        {
                          if (copyAndMovingBord.IsInChess(color))
                          {
                            continue;
                          }
                        }
                      }*/






                    if (fromIndexNotValide != -1 && toIndexNotValide != -1)
                    {
                        if (index == fromIndexNotValide && movedIndex == toIndexNotValide)
                            continue;
                    }

                    //Ajout des neuds
                    var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, DeepLevel);
                    parentNode.ChildList.Add(newNode);


                    if (newNode.Level == DeepLevel)
                    {

                        //LastNodes.Add(newNode);

                        //if (_oldW < newNode.Weight)
                        // {

                        // if()
                        LastNodes.Add(newNode);
                        // _oldW = newNode.Weight;
                        // }



                    }





                    var newColor = "B";
                    if (color == "B")
                        newColor = "W";
                    EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
                }
            }
            // }




        }

        public void EmulAllPossibleMovesForFirstTurn(NodeChess2 parentNode, Board board, string color, int level, int fromIndex, int toIndex)
        {

            var randomNodes = new List<NodeChess2>();
            if (level == 0)
            {
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, DeepLevel);
                node.Weight = 0;
                //LastNodes.Add(node);
                parentNode = node;
            }


            if (level == DeepLevel)
                return;
            level++;
            var pawns = board.GetCasesIndex(color);

            foreach (var index in pawns)
            {

                // _oldW = -9000;
                var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.Index);
                foreach (var movedIndex in possiblesMoves)
                {
                    ////Console.WriteLine($"{index} => {movedIndex}");
                    ////Console.WriteLine($"\t L : {level}");
                    var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);
                    /////copyAndMovingBord.Print();

                    //Ajout des neuds
                    var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, DeepLevel);
                    randomNodes.Add(newNode);
                    // Nodes.Add(newNode);




                }





            }

            var rand = new Random();
            BestNode = randomNodes[rand.Next(randomNodes.Count())];
            
        }


        public void PrintAllNode()
        {
            foreach (var node in LastNodes)
            {
                //if(node.Level == 3)
                Console.WriteLine($"l : {node.Level}\tcolor: {node.Color}\tWeight:{node.Weight}\t\t{node.FromIndex}=>{node.ToIndex}");
            }
        }





        //retour le position du roi
        public int GetKingAlierIndex(Board board)
        {






            var kingIndex = board.GetKingColorIndex(ComputerColore);

            return kingIndex;








        }

        public List<NodeChess2> GetParentListAndMinMax(List<NodeChess2> nodesIn, int level)
        {
            var oldParent = new NodeChess2();
            List<NodeChess2> parentLevelList = new List<NodeChess2>();

            foreach (var node in nodesIn)//Remplissage des parent
            {
                var currentParent = node.Parent;
                if (oldParent == currentParent)
                {
                    parentLevelList.Add(currentParent);

                }
                else
                    oldParent = currentParent;
            }

            //Pour T07

            //Pour tous les parentLevel5List
            //MAX




            foreach (var parent in parentLevelList)
            // Parallel.ForEach(parentLevelList, parent =>
            {
                if (level == 1 || level == 3)//MAX
                {

                    var maxWeight = parent.ChildList.Max(x => x.Weight);
                    parent.Weight = maxWeight;
                   





                    if (level == 1)
                    {
                      

                        //Pour 82
                        //pour eviter le nulle on enleve evite de refaire les memes actions
                        //on rend les 6 dérnier action
                        var sixLastAction = new List<string>();
                        if (Utils.MovingList != null)
                        {
                            if (Utils.MovingList.Count >= 6)
                            {
                                sixLastAction = Utils.MovingList.Skip(Math.Max(0, Utils.MovingList.Count() - 6)).ToList();
                                var opinionRepetitionCount = 0;
                                var repetionOpinionMove = "";
                                var cumputerRepetitionCount = 0;
                                var repetionCumputerMove = "";

                                foreach (var item in sixLastAction)
                                {

                                    if (item.Contains($"{ComputerColore})"))
                                    {
                                        cumputerRepetitionCount = sixLastAction.Where(x => x == item).Count();
                                        if (cumputerRepetitionCount > 1)
                                            repetionCumputerMove = item;

                                    }

                                    if (item.Contains($"{GetOpinionColor()})"))
                                    {
                                        opinionRepetitionCount = sixLastAction.Where(x => x == item).Count();
                                        if (opinionRepetitionCount > 1)
                                            repetionOpinionMove = item;

                                    }
                                }

                                //si il y a repetition
                                if (opinionRepetitionCount == 2 && cumputerRepetitionCount == 2)
                                {
                                    //on prend l'élement à ne pas répeter
                                    sixLastAction.RemoveAll(x => x.Contains($"{GetOpinionColor()})") || x == repetionCumputerMove || x == repetionOpinionMove);
                                    if (sixLastAction.Count == 1)
                                    {
                                        var toRemoveCumputerMove = sixLastAction.First();


                                        //on doit enlever toRemovecumputerMove de parent.ChildList pour éviter la répétition
                                        //  "61(Q|B)>53(__)"
                                        var data = toRemoveCumputerMove.Split('>');

                                        var fromIndexToRemove = data[0].Substring(0, data[0].IndexOf("("));
                                        var toIndexToRemove = data[1].Substring(0, data[1].IndexOf("("));

                                        parent.ChildList.RemoveAll(x => x.FromIndex == Int32.Parse(fromIndexToRemove) && x.ToIndex == Int32.Parse(toIndexToRemove));

                                    }



                                }
                            }

                        }


                        //si ChildList.count ==0 => on l'ia a perdu
                        if (parent.ChildList.Count == 0)
                        {
                            //var t_ = "echec";
                            var loseNode = new NodeChess2();
                            loseNode.Weight = -999;
                            parent.BestNode = loseNode;
                            break;
                        }




                        var maxWeith = parent.ChildList.Max(x => x.Weight);


                        //var minW = parent.ChildList.Min(x=>x.Weight);
                        // if (maxWeith == 999)
                        // maxWeith = parent.ChildList.Where(x => !Utils.KingIsMenaced(x.Board, Utils.ComputerColor)).Max(x => x.Weight);

                        var maxNodeList = parent.ChildList.Where(x => x.Weight == maxWeith).ToList();
                        //pour T90 et T91,seulement pour les L4, on ajoute les minNodes pour ne plus les prendres dans L3 et L2
                        if (DeepLevel == 4)
                        {
                            var minWeith = parent.ChildList.Min(x => x.Weight);
                            var minNodeList = parent.ChildList.Where(x => x.Weight == minWeith).ToList();
                            if (Level4BlackList == null)
                                Level4BlackList = new List<NodeChess2>();
                            foreach (var node in minNodeList)
                            {
                                //si le node n'est pas deja dans Level4MinList, on l'ajoute
                                var containt = Level4BlackList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex && x.Weight == node.Weight);
                                if (containt == null)
                                {
                                    //si deja dans maxNodeList, on ne l'ajoute pas
                                    var containtInMaxNode = maxNodeList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex);
                                    if (containtInMaxNode == null)
                                        Level4BlackList.Add(node);
                                }

                            }
                        }

                        // var t_maxNode = maxNodeList.First();
                        // Utils.ComputerKingIsMenaced( t_maxNode.Board);
                        if (maxNodeList.Count != 0)
                        {



                            var maxNode = new NodeChess2();
                            if (maxNodeList.Count() == 1)
                                maxNode = maxNodeList.First();






                            else
                            {
                                //Pour T95B si il y a plus de deux au hazard, on privilegie 
                                //celui qui est protégé
                                //if(maxWeith<=-900)
                                if (maxWeith >-900)
                                {
                                    foreach (var node in maxNodeList)
                                    {
                                       /// if (node.GetIsLocationIsProtected(node.ToIndex, "B", "W"))
                                        if (DeepLevel == 4 && node.GetIsLocationIsProtected(node.ToIndex, Utils.ComputerColor, Utils.OpinionColor))
                                        {
                                            node.Weight += 0.5;
                                        }
                                            
                                    }
                                }
                               
                                maxWeith = maxNodeList.Max(x => x.Weight);
                                maxNodeList = maxNodeList.Where(x => x.Weight == maxWeith).ToList();
                                if (maxNodeList.Count() == 1)
                                    maxNode = maxNodeList.First();
                                else
                                {
                                    //if(List)
                                    //maxNodeList = maxNodeList.Where(x => x.Weight == maxWeith).ToList();
                                    var rand = new Random();
                                    maxNode = maxNodeList[rand.Next(maxNodeList.Count())];
                                    maxNode.RandomEquivalentList = maxNodeList;
                                }

                                  
                                
                               
                                



                            }



                            parent.BestNode = maxNode;
                            //parent.MinNodeList = minNodeList;
                            break;
                        }

                    }



                }//);

                else//MIN
                {

                    var minWeight = parent.ChildList.Min(x => x.Weight);
                    parent.Weight = minWeight;



                }

            }



            level--;


            if (level == 0)
                return parentLevelList;
            else
                return GetParentListAndMinMax(parentLevelList, level);




        }

        public string GetOpinionColor()
        {
            if (ComputerColore == "W")
                return "B";
            else
                return "W";
        }
        /*tsiry;21-12-2021*/
        public void MinMaxByRoot()
        {


            //var maxW = LastNodes.Max(x => x.Weight);
            var parentLevel1List = GetParentListAndMinMax(LastNodes, DeepLevel);



            if (parentLevel1List != null)
            {
                if (parentLevel1List.Count != 0)
                {
                    var nodeZero = parentLevel1List.First();
                    BestNode = nodeZero.BestNode;
                }

            }





        }






        public void PrintAllPossiblePossiblesPositionFromColor(Board board, string color)
        {
            var pawns = board.GetCasesIndex(color);
            foreach (var index in pawns)
            {
                PrintAllPossiblePossiblesPositionFromOncePosition(board, index);
            }
        }

        public void PrintAllPossiblePossiblesPositionFromOncePosition(Board board, int initalPosition)
        {
            var possiblesIndex = board.GetPossibleMoves(initalPosition, 1);
            Console.WriteLine($"POSSIBLES POSITION FOR {initalPosition} :");
            foreach (var index in possiblesIndex)
            {
                Console.WriteLine($"\t{index}");
            }

        }

        public void Dispose()
        {
            Debug.WriteLine("Memory used before collection:       {0:N0}",
       GC.GetTotalMemory(false));
          /*  Console.WriteLine("Memory used before collection:       {0:N0}",
                    GC.GetTotalMemory(false));*/
            Debug.WriteLine("Collect");

            GC.Collect();
            Debug.WriteLine("Memory used before collection:       {0:N0}",
                  GC.GetTotalMemory(false));
          /*  Console.WriteLine("Memory used before collection:       {0:N0}",
                    GC.GetTotalMemory(false));*/

        }
    }

}
