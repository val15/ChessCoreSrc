﻿using System.Collections.Concurrent;
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

        public List<NodeChess2> LevelBlackList { get; set; }


        private int _deepLevel { get; set; }

        //private int _oldW;
        private bool _isReprise = false;
        private int level;
        private string cpuColor;
        private bool isReprise;
        private bool v;
        private List<Node> specifiBoardList;

        public List<NodeChess2> LastNodes { get; set; }

        public NodeChess2 BestNode { get; set; }
        public NodeChess2 BestNodeSecond { get; set; }

        public NodeChess2 NotBestNode { get; set; }

        public bool IsParallelMode { get; set; }




        public EngineMultiThreading(int deepLevel, string computerColore, bool isReprise, bool fromIndexIsNenaced, List<SpecificBoard> specifiBoardList = null, bool isInTest = false)
        {
            FromIndexIsNenaced = fromIndexIsNenaced;
            _isReprise = isReprise;
            _deepLevel = deepLevel;
            LastNodes = new List<NodeChess2>();
            ComputerColore = computerColore;
            OpinionrColore = "W";
            if (ComputerColore == "W")
                OpinionrColore = "B";
            SpecifiBoardList = specifiBoardList;
            Utils.ComputerColor = computerColore;
        }

        public EngineMultiThreading(int level, string cpuColor, bool isReprise, bool v, List<Node> specifiBoardList)
        {
            this.level = level;
            this.cpuColor = cpuColor;
            this.isReprise = isReprise;
            this.v = v;
            this.specifiBoardList = specifiBoardList;
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
            try
            {
                MainBord = mainBord;
                // Utils.MainBord = MainBord;
                //TEST print board
                //  MainBord.Print();

                //Si reprise
                // IsReprise = true;

                //Pour les sécifiques positions
                //Specifiques positions
                if (SpecifiBoardList != null && level == _deepLevel)//on ne prend de le level le plus haut
                {
                    EmuleLevel1ForSpecifiquePosition(MainBord, pawnIndex);

                }
                /* if (BestNode == null)
                 {*/
                if ((tunNumber == "1" || tunNumber == "0") && !_isReprise)
                    EmulAllPossibleMovesForFirstTurn(null, mainBord, ComputerColore, 0, -1, -1);
                else
                    EmulAllPossibleMovesForOnce(null, mainBord, ComputerColore, 0, pawnIndex);
                MinMaxByRoot();
                ClearALL();

                return BestNode;
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.Message);
                return null;
            }


        }

        public void ClearALL()
        {
            try
            {
                if (LastNodes.Count == 0)
                    return;

                foreach (var lastNode in LastNodes)
                {
                    //Utils.WritelineAsync(lastNode.Level.ToString());
                    if(lastNode == null) continue;
                    ClearNode(lastNode.Parent);
                }
                LastNodes.Clear();
            }
            catch (Exception ex)
            {

                Utils.WritelineAsync(ex.Message);
            }


        }
        public void ClearNode(NodeChess2 node)
        {
            try
            {
                if (node == null)
                    return;
                if (node.ChildList.Count == 0)
                    return;
                // Utils.WritelineAsync($"Clear node.ChildList.Count Count = {node.ChildList.Count()}");
                node.ChildList.Clear();
                if (node.Parent == null)
                    return;
                ClearNode(node.Parent);
            }
            catch (Exception ex)
            {

                Utils.WritelineAsync(ex.Message);
            }
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


            var possiblesMoves = inBord.GetPossibleMoves(pawnIndex, 1).Select(x => x.ToIndex);
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
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, _deepLevel);
                node.Weight = 0;
                //LastNodes.Add(node);
                parentNode = node;
            }
            level++;
            var index = currentPawnIndex;

            // _oldW = 9000;
            var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.ToIndex);

            foreach (var movedIndex in possiblesMoves)
            {
                var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);
                //Ajout des neuds
                var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, _deepLevel);
                parentNode.ChildList.Add(newNode);
                var newColor = "B";
                if (color == "B")
                    newColor = "W";
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
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, _deepLevel);

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
                var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, _deepLevel);

                parentNode.ChildList.Add(newNode);






                var newColor = "B";
                if (color == "B")
                    newColor = "W";


                EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
            }





        }


        public void EmulAllPossibleMovesForSP(NodeChess2 parentNode, Board board, string color, int level, int fromIndex, int toIndex, int fromIndexNotValide = -1, int toIndexNotValide = -1)
        {
            try
            {
                if (BestNode != null)
                    return;
                if (level == 0)
                {
                    var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, _deepLevel);
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

                if (level == _deepLevel)
                    return;
                level++;
                var pawns = board.GetCasesIndexForColor(color);
                foreach (var index in pawns)
                {
                    // _oldW = -9000;

                    var elementInIndex = board.GetCases().ElementAt(index);


                    var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.ToIndex);
                    foreach (var movedIndex in possiblesMoves)
                    {
                        var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);
                        if (fromIndexNotValide != -1 && toIndexNotValide != -1)
                        {
                            if (index == fromIndexNotValide && movedIndex == toIndexNotValide)
                                continue;
                        }

                        //Ajout des neuds
                        var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, _deepLevel);
                        parentNode.ChildList.Add(newNode);


                        if (newNode.Level == _deepLevel)
                        {

                            LastNodes.Add(newNode);

                        }

                        //pour T80, o regarde les lose et les win seulment pour les DeepLevel == 4
                        if (_deepLevel == Utils.DeepLevel)
                        {
                            if (level <= 3)
                            {
                                if (Chess2Utils.TargetColorIsInChess(board, Utils.ComputerColor))
                                {
                                    //LOSE node
                                    Console.WriteLine("lose");
                                    Debug.WriteLine("lose");
                                    newNode.Parent.Parent.Weight = -999;
                                    Utils.NodeLoseList.Add(newNode.Parent.Parent);
                                    return;
                                }



                            }
                            if (level <= 3)
                            {
                                if (Chess2Utils.TargetColorIsInChess(board, Utils.OpinionColor))
                                {
                                    if (Utils.MainBoard.GetCases()[newNode.Parent.FromIndex].Contains($"|{Utils.ComputerColor}"))//si from index n' est pas vide dans MainBoard 
                                    {
                                        var possibleMovesofFromIndex = Utils.MainBoard.GetPossibleMoves(newNode.Parent.FromIndex, 1, false).Select(x => x.ToIndex);
                                        //si newNode.Parent.Toindex est dans possibleMovesofFromIndex
                                        if (possibleMovesofFromIndex.Contains(newNode.Parent.ToIndex))
                                        {

                                            //WIN Node
                                            Console.WriteLine("win");
                                            Debug.WriteLine("win");
                                            // newNode.Board.PrintInDebug();
                                            //Chess2Utils.ma
                                            //newNode.Parent.
                                            BestNode = newNode.Parent;
                                            BestNode.Weight = 999;
                                            return;


                                        }

                                    }
                                }
                            }

                        }
                        var newColor = "B";
                        if (color == "B")
                            newColor = "W";
                        EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
                    }
                }
            }

            catch (Exception ex)
            {

                Utils.WritelineAsync(ex.ToString());
            }

        }
        public void EmulAllPossibleMovesForN(NodeChess2 parentNode, Board board, string color, int level, int fromIndex, int toIndex, int fromIndexNotValide = -1, int toIndexNotValide = -1)
        {
            try
            {
                if (BestNode != null)
                    return;

                if (level == 0)
                {
                    var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, _deepLevel);
                    node.Weight = 0;
                    parentNode = node;
                }

                if (NotBestNode != null)
                {
                    if (fromIndex == NotBestNode.FromIndex && toIndex == NotBestNode.ToIndex)
                    {
                        return;
                    }
                }

                if (level == _deepLevel)
                    return;

                level++;
                var pawns = board.GetCasesIndexForColor(color);

                // Utiliser Parallel.ForEach pour exécuter les boucles en parallèle
                Parallel.ForEach(pawns, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, index =>
                {
                    var elementInIndex = board.GetCases().ElementAt(index);
                    var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.ToIndex);

                    foreach (var movedIndex in possiblesMoves)
                    {
                        var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);

                        if (fromIndexNotValide != -1 && toIndexNotValide != -1 && index == fromIndexNotValide && movedIndex == toIndexNotValide)
                            continue;

                        var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, _deepLevel);
                        parentNode.ChildList.Add(newNode);

                        if (newNode.Level == _deepLevel)
                        {
                            LastNodes.Add(newNode);
                        }

                        if (_deepLevel == Utils.DeepLevel && level <= Utils.DeepLevel - 1)
                        {
                            if (Chess2Utils.TargetColorIsInChess(board, Utils.ComputerColor))
                            {
                                //LOSE node
                                Console.WriteLine("lose");
                                Debug.WriteLine("lose");
                                newNode.Parent.Parent.Weight = -999;
                                Utils.NodeLoseList.Add(newNode.Parent.Parent);
                                return;
                            }

                            if (Chess2Utils.TargetColorIsInChess(board, Utils.OpinionColor))
                            {
                                if (Utils.MainBoard.GetCases()[newNode.Parent.FromIndex].Contains($"|{Utils.ComputerColor}"))
                                {
                                    var possibleMovesofFromIndex = Utils.MainBoard.GetPossibleMoves(newNode.Parent.FromIndex, 1, false).Select(x => x.ToIndex);
                                    if (possibleMovesofFromIndex.Contains(newNode.Parent.ToIndex))
                                    {
                                        //WIN Node
                                        Console.WriteLine("win");
                                        Debug.WriteLine("win");
                                        BestNode = newNode.Parent;
                                        BestNode.Weight = 999;
                                        return;
                                    }
                                }
                            }
                        }

                        var newColor = color == "B" ? "W" : "B";
                        EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
                    }
                });
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
            }
        }




        public void EmulAllPossibleMovesFor(NodeChess2 parentNode, Board board, string color, int level, int fromIndex, int toIndex, int fromIndexNotValide = -1, int toIndexNotValide = -1)
        {
            try
            {
                if (BestNode != null)
                    return;

                if (level == 0)
                {
                    var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, _deepLevel);
                    node.Weight = 0;
                    parentNode = node;
                }

                if (NotBestNode != null && fromIndex == NotBestNode.FromIndex && toIndex == NotBestNode.ToIndex)
                {
                    return;
                }

              


                if (level == _deepLevel)
                    return;

                level++;
                var pawns = board.GetCasesIndexForColor(color);

                // Use ConcurrentBag to store the results in a thread-safe manner
                ConcurrentBag<NodeChess2> newLastNodes = new ConcurrentBag<NodeChess2>();

                var localPawns = pawns.ToList(); // Créez une copie locale de la collection pawns

                object childListLock = new object(); // Déclaration d'un objet verrou

                Parallel.ForEach(localPawns, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, index =>
                {
                    var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.ToIndex);
                    foreach (var movedIndex in possiblesMoves)
                    {
                        var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);
                        if (fromIndexNotValide != -1 && toIndexNotValide != -1 && index == fromIndexNotValide && movedIndex == toIndexNotValide)
                            continue;

                        var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, _deepLevel);

                        lock (childListLock) // Verrouillage de la liste parentNode.ChildList
                        {
                            parentNode.ChildList.Add(newNode);
                        }

                        if (newNode.Level == _deepLevel)
                        {
                            lock (childListLock) // Verrouillage de la liste newLastNodes
                            {
                                newLastNodes.Add(newNode);
                            }
                        }

                        if (_deepLevel == Utils.DeepLevel && level <= Utils.DeepLevel - 1)
                        {
                            if (Chess2Utils.TargetColorIsInChess(board, Utils.ComputerColor))
                            {
                                newNode.Parent.Parent.Weight = -999;
                                lock (childListLock) // Verrouillage de la liste newLastNodes
                                {
                                    newLastNodes.Add(newNode.Parent.Parent);
                                }
                                return;
                            }

                            if (Chess2Utils.TargetColorIsInChess(board, Utils.OpinionColor))
                            {
                                if (Utils.MainBoard.GetCases()[newNode.Parent.FromIndex].Contains($"|{Utils.ComputerColor}"))
                                {
                                    var possibleMovesofFromIndex = Utils.MainBoard.GetPossibleMoves(newNode.Parent.FromIndex, 1, false).Select(x => x.ToIndex);
                                    if (possibleMovesofFromIndex.Contains(newNode.Parent.ToIndex))
                                    {
                                        BestNode = newNode.Parent;
                                        BestNode.Weight = 999;
                                        return;
                                    }
                                }
                            }
                        }

                        var newColor = color == "B" ? "W" : "B";
                        EmulAllPossibleMovesFor(newNode, copyAndMovingBord, newColor, level, index, movedIndex);
                    }
                });

                foreach (var newNode in newLastNodes)
                {
                    LastNodes.Add(newNode);
                }
            }
            catch (Exception ex)
            {

                Utils.WritelineAsync(ex.ToString());
            }
        }


        public void EmulAllPossibleMovesForFirstTurn(NodeChess2 parentNode, Board board, string color, int level, int fromIndex, int toIndex)
        {

            var randomNodes = new List<NodeChess2>();
            if (level == 0)
            {
                var node = new NodeChess2(null, board, level, color, -1, -1, ComputerColore, _deepLevel);
                node.Weight = 0;
                //LastNodes.Add(node);
                parentNode = node;
            }


            if (level == _deepLevel)
                return;
            level++;
            var pawns = board.GetCasesIndexForColor(color);

            foreach (var index in pawns)
            {

                // _oldW = -9000;
                var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.ToIndex);
                foreach (var movedIndex in possiblesMoves)
                {
                    ////Console.WriteLine($"{index} => {movedIndex}");
                    ////Console.WriteLine($"\t L : {level}");
                    var copyAndMovingBord = Utils.CloneAndMove(board, index, movedIndex, level);
                    /////copyAndMovingBord.Print();

                    //Ajout des neuds
                    var newNode = new NodeChess2(parentNode, copyAndMovingBord, level, color, index, movedIndex, ComputerColore, _deepLevel);
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

            try
            {
                //if(Utils.BestNodeIsFind)
                //     return null;
                var oldParent = new NodeChess2();
                List<NodeChess2> parentLevelList = new List<NodeChess2>();

                foreach (var node in nodesIn)//Remplissage des parent
                {
                    //Pour tous les parentLevel5List
                    //TODO
                    if (node == null)
                        continue;


                    var currentParent = node.Parent;
                    if (oldParent == currentParent)
                    {
                        parentLevelList.Add(currentParent);

                    }
                    else
                        oldParent = currentParent;
                }

                foreach (var parent in parentLevelList)
                // Parallel.ForEach(parentLevelList, parent =>
                {
                    

                    if (parent == null)
                        continue;
                    // if (level == 1 || level == 3 || level == 1 || level == 5)//MAX
                    if ((level % 2 != 0))//MAX
                    {

                        var maxWeight = parent.ChildList.Max(x => x.Weight);
                        if (parent.Weight < maxWeight)
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
                                            var data = toRemoveCumputerMove.Split('→');

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

                            //T99 et T78
                            foreach (var node in parent.ChildList)
                            {
                                if (!Chess2Utils.TargetIndexIsMenaced(parent.Board, Utils.ComputerColor, node.FromIndex) && Chess2Utils.TargetIndexIsMenaced(node.Board, Utils.ComputerColor, node.ToIndex))
                                {
                                    node.Weight -= node.Board.GetWeightInIndex(node.ToIndex);
                                }
                            }



                            var maxWeith = parent.ChildList.Max(x => x.Weight);


                            //var minW = parent.ChildList.Min(x=>x.Weight);
                            // if (maxWeith == 999)
                            // maxWeith = parent.ChildList.Where(x => !Utils.KingIsMenaced(x.Board, Utils.ComputerColor)).Max(x => x.Weight);

                            var maxNodeList = parent.ChildList.Where(x => x.Weight == maxWeith).ToList();
                            //pour T90 et T91,seulement pour les L4, on ajoute les minNodes pour ne plus les prendres dans L3 et L2
                            if (/*DeepLevel == 4*/true)
                            {
                                var minWeith = parent.ChildList.Min(x => x.Weight);
                                var minNodeList = parent.ChildList.Where(x => x.Weight == minWeith).ToList();
                                if (LevelBlackList == null)
                                    LevelBlackList = new List<NodeChess2>();
                                foreach (var node in minNodeList)
                                {
                                    //si le node n'est pas deja dans Level4MinList, on l'ajoute
                                    var containt = LevelBlackList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex && x.Weight == node.Weight);
                                    if (containt == null)
                                    {
                                        //si deja dans maxNodeList, on ne l'ajoute pas
                                        var containtInMaxNode = maxNodeList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex);
                                        if (containtInMaxNode == null)
                                            LevelBlackList.Add(node);
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
                                    //Pour T94 si il y a plus de deux au hazard, on privilegie 
                                    //celui qui est protégé
                                    //if(maxWeith<=-900)
                                  /*  if (maxWeith > -900)
                                    {
                                        foreach (var node in maxNodeList)
                                        {
                                            /// if (node.GetIsLocationIsProtected(node.ToIndex, "B", "W"))
                                            if (node.GetIsLocationIsProtected(node.ToIndex, Utils.ComputerColor, Utils.OpinionColor))
                                            {
                                                node.Weight += 0.5;
                                            }
                                            //T100 et T105 mais pour T33b ,il faut que toIndexne soit pas nenacé
                                            if (!Chess2Utils.TargetIndexIsMenaced(node.Board, Utils.ComputerColor, node.ToIndex))
                                                node.Weight += (Chess2Utils.GetWeigtOpionionMenacedsByToIndex(node.Board, Utils.OpinionColor, node.ToIndex)) * 0.1;
                                        }
                                    }*/

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
                        if (parent.ChildList == null)
                            continue;
                        if (parent.ChildList.Count == 0)
                            continue;
                        parent.ChildList.RemoveAll(x => x == null);
                        var minWeight = parent.ChildList.Min(x => x.Weight);
                        //T96 
                        if (parent.Weight > minWeight)
                            parent.Weight = minWeight;




                    }

                }



                level--;

                //  Utils.IsFirstTurn=false;
                if (level == 0)
                    return parentLevelList;
                else
                    return GetParentListAndMinMax(parentLevelList, level);




            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.Message);
                return null;
            }

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
            var parentLevel1List = GetParentListAndMinMax(LastNodes, _deepLevel);

            //Pour T07a
            if (BestNode != null)
                return;
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
            var pawns = board.GetCasesIndexForColor(color);
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
            //Utils.GCColect();

        }
    }

}
