using System.Diagnostics;

namespace ChessCore.Tools
{
    public class NodeChess2
    {
        public string PawnName { get; set; }
        public NodeChess2 BestNode { get; set; }

        public int MinW { get; set; }

        public int Level { get; set; }
        public List<NodeChess2> ChildList { get; set; }
        public List<NodeChess2> GrandChildList { get; set; }
        public List<NodeChess2> RandomEquivalentList { get; set; } = new List<NodeChess2>();
        public string Color { get; set; }
        public Board Board { get; set; }
        public NodeChess2 Parent { get; set; }

        // public List<NodeChess2> MinNodeList { get; set; }

        //  public bool IsChess { get; set; }






        public int FromIndex { get; set; }
        public int ToIndex { get; set; }

        public int Weight { get; set; }
        public NodeChess2()
        {

        }


        public int GetValue()//en fonction de la case où il se trouve
        {
            if (this.Board == null)
                return 0;
            var fromCase = this.Board.GetCases()[ToIndex];
            if (!fromCase.Contains("|"))
                return 0;
            var pawnName = fromCase.Split('|')[0];
            switch (pawnName)
            {
                case "P":
                    return 1;
                case "Q":
                    return 9;
                case "T":
                    return 5;
                case "B":
                    return 3;
                case "C":
                    return 3;
                case "K":
                    return 100;
            }
            return 0;
        }

        public bool TargetIndexIsMenaced(Board board, string curentColor, string opinionColor, int targetIndex)
        {

            var opinionListIndex = board.GetCasesIndexForColor(opinionColor);


            foreach (var index in opinionListIndex)
            {
                if (targetIndex == -1)
                    return false;
                var possiblesMoves = board.GetPossibleMoves(index, 1).Select(x => x.Index);
                if (possiblesMoves.Contains(targetIndex))
                {
                    return true;
                }
                else
                {
                    var cloneBoad = Utils.CloneBoad(board);

                    cloneBoad.SetCases(targetIndex, cloneBoad.GetCases()[targetIndex].Replace($"{opinionColor}", $"{curentColor}"));
                    // cloneBoad = cloneBoad.GetCases()[targetIndex].Replace($"{opinionColor}", $"{curentColor}");

                    possiblesMoves = cloneBoad.GetPossibleMoves(index, 1).Select(x => x.Index);

                    if (possiblesMoves.Contains(targetIndex))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TargetIndexIsMenacedMT(Board board, string curentColor, string opinionColor, int targetIndex)
        {
            var opinionListIndex = board.GetCasesIndexForColor(opinionColor);

            bool isTargetMenaced = false;

            Parallel.ForEach(opinionListIndex, (index, loopState) =>
            {
                if (targetIndex == -1)
                {
                    loopState.Break(); // Exit the loop early
                    return;
                }

                var possiblesMoves = board.GetPossibleMoves(index, 1).Select(x => x.Index);
                if (possiblesMoves.Contains(targetIndex))
                {
                    isTargetMenaced = true;
                    loopState.Break(); // Exit the loop early
                    return;
                }
                else
                {
                    var cloneBoad = Utils.CloneBoad(board);

                    cloneBoad.SetCases(targetIndex, cloneBoad.GetCases()[targetIndex].Replace($"{opinionColor}", $"{curentColor}"));

                    possiblesMoves = cloneBoad.GetPossibleMoves(index, 1).Select(x => x.Index);

                    if (possiblesMoves.Contains(targetIndex))
                    {
                        isTargetMenaced = true;
                        loopState.Break(); // Exit the loop early
                        return;
                    }
                }
            });
            return isTargetMenaced;
        }


        public bool TargetIndexIsMenacedOld(Board board, string curentColor, string opinionColor, int targetIndex)
        {

            //var opinionListIndex = new List<int>();


            // for (int i = 0; i < board.GetCases().Count(); i++)
            // {
            //     var caseBoard = board.GetCases()[i];
            //     if (caseBoard.Contains($"|{opinionColor}"))
            //         opinionListIndex.Add(i);
            // }
            var opinionListIndex = board.GetCasesIndexForColor(opinionColor);


            foreach (var index in opinionListIndex)
            {

                if (targetIndex == -1)
                    return false;
                var cloneBoad = Utils.CloneBoad(board);

                cloneBoad.SetCases(targetIndex, cloneBoad.GetCases()[targetIndex].Replace($"{opinionColor}", $"{curentColor}"));
                // cloneBoad = cloneBoad.GetCases()[targetIndex].Replace($"{opinionColor}", $"{curentColor}");

                var possiblesMoves = cloneBoad.GetPossibleMoves(index, 1).Select(x => x.Index);

                if (possiblesMoves.Contains(targetIndex))
                {
                    return true;
                }


                /*foreach (var movedIndex in possiblesMoves)
                {
                    if (movedIndex == targetIndex)
                    {

                        return true;


                    }

                }*/
            }


            /* var possiblesMoves = board.GetPossibleMoves(index, level).Select(x => x.Index);
             foreach (var movedIndex in possiblesMoves)
             {
             }*/
            return false;
        }
        public bool KingIsMenaced(Board board, string curentColor, string opinionColor)
        {
            var kingIndex = -1;
            for (int i = 0; i < board.GetCases().Count(); i++)
            {
                var caseBoard = board.GetCases()[i];
                if (caseBoard.Contains($"K|{curentColor}"))
                {
                    kingIndex = i;
                    break;
                }


            }

            return TargetIndexIsMenaced(board, curentColor, opinionColor, kingIndex);

        }




        public bool GetIsLocationIsProtected(int locationIndex, string currentColor, string opinionColor)
        {
            try
            {
                //if (!TargetIndexIsMenaced(Board, currentColor, opinionColor, locationIndex))
                //    return false;

                //On creer une copy du board
                var copyBoard = Utils.CloneBoad(Board);
                var currentCase = copyBoard.GetCases()[locationIndex];
                if (!currentCase.Contains("|"))
                    return false;

                //on change la couleur 
                currentCase = currentCase.Replace($"|{currentColor}", $"|{opinionColor}");
                copyBoard.GetCases()[locationIndex] = currentCase;
                //si apres changement de couleur, la position est menacer
                //=> c'est que la position est protégée
                if (TargetIndexIsMenaced(copyBoard, $"{opinionColor}", $"{currentColor}", locationIndex))
                    return true;

                return false;
            }
            catch(Exception ex) 
            {
                return false;
            }

          
        }


        public int GetProtectedNumber()
        {
            var opinionColor = "W";
            if (Color == "W")
                opinionColor = "B";
            var protectedNumber = 0;
            var alierIndexList = new List<int>();
            var protectedList = new List<int>();
            ///Board.GetCases().Where(x => x.Contains($"|{Color}"));
            var i = 0;
            foreach (var currentCase in Board.GetCases())
            {
                if (currentCase.Contains($"|{Color}"))
                    alierIndexList.Add(i);
                i++;
            }
            foreach (var index in alierIndexList)
            {
                if (GetIsLocationIsProtected(index, Color, opinionColor))
                {
                    protectedNumber++;
                    protectedList.Add(index);
                }

            }
            return protectedNumber;
        }
        public NodeChess2(NodeChess2 parent, Board board, int level, string color, int formIndex, int toIndex, string computeurColor, int maxDeepLevel)
        {
            if (toIndex != -1)
                PawnName = board.GetCases()[toIndex][0].ToString();
            FromIndex = formIndex;
            ToIndex = toIndex;
            Level = level;
            Board = Utils.CloneBoad(board);
            Parent = parent;
            Color = color;
            ChildList = new List<NodeChess2>();



            //Pour T97A
            /*  if (Level == 4 )
              {

                  if (Parent.Parent.Weight>-900)
                  {
                    if (Chess2Utils.TargetColorIsInChess(Board, Utils.ComputerColor))
                    {
                      //LOSE node
                      Console.WriteLine("lose");
                      Utils.Writeline("lose");
                      Parent.Parent.Weight = -999;
                      Parent.Parent.Weight = -999;
                      Utils.NodeLoseList.Add(Parent.Parent);
                      return;
                    }
                  }
                }*/







            var opinionKingIndex = board.GetCases().ToList().IndexOf($"K|{Utils.OpinionColor}");
            if (opinionKingIndex == -1)
            {
                Weight = 999;
                return;
            }
            var kingIndex = board.GetCases().ToList().IndexOf($"K|{computeurColor}");
            if (kingIndex == -1)
            {
                Weight = -999;
                return;
            }
            if (level == Utils.DeepLevel)
            {

                //T41
                var diffTime = (DateTime.Now - Utils.StartedProcessTime).TotalMinutes;

                if (diffTime < Utils.LimitationForT41InMn)
                {
                    //T97A
                    if (Chess2Utils.TargetColorIsInChess(Board, Utils.ComputerColor))
                    {
                        if(Parent!=null)
                            Parent.Parent.Weight--;//+= -99;//Poure que T97A marche avec T37
                        return;
                    }
                }

            }


            //if(level==4)
            //if (level == Utils.DeepLevel)//TODO POUR L5
                CalculeScores();
            // if(Utils.DeepLevel<=4 || Utils.DeepLevel == level)
            //        CalculeScores();

            

            if (toIndex == -1)
                return;
           //T108
           // test
           /* if (toIndex == 37 /*&& level == 1)*/
           try
            {
               if(level % 2 !=0)
                {
                    // var t_fd = index;
                    var menaceds = new List<string>();
                    var totalValue = 0;
                    var panwValue = board.GetValue(board.GetCaseInIndex(toIndex));
                    // on prend le total des poids des poins menacéer par toIndex
                    var opinionColor = "W";
                    if (Color == "W")
                        opinionColor = "B";
                    if (TargetIndexIsMenaced(Board, Color,opinionColor, toIndex) && !GetIsLocationIsProtected(toIndex, Color, opinionColor))
                    {
                        Weight -= panwValue;
                        return;
                    }
                       
                    if (GetIsLocationIsProtected(toIndex,Color, opinionColor))
                    {
                        // board.PrintInDebug();
                       
                        var possiblesMovesIndex = board.GetPossibleMoves(toIndex, 1).Select(x => x.Index);
                        foreach (var targetIndex in possiblesMovesIndex)
                        {
                            var caseInIndex = board.GetCaseInIndex(targetIndex);
                            if (caseInIndex != "__")
                            {
                                menaceds.Add(caseInIndex);
                                if (TargetIndexIsMenaced(board, Color, computeurColor, targetIndex))
                                {
                                    var targetValue = board.GetValue(caseInIndex);
                                    totalValue += targetValue;
                                }
                            }
                        }
                    }
                    if (totalValue > 0 && menaceds.Count > 1)
                    {

                        //   si la valeur du poin qui menace * 2 est inférieur au total des poins nenaceé / 2
                        //  on ajoute le bonnus
                        if ((panwValue * 2) < totalValue / 2)
                        {
                            var t_ = menaceds;
                            var t_df = totalValue;
                            Weight += totalValue / 10;
                        }


                    }
               }
               

            }
            catch(Exception ex)  
            { 
                Console.WriteLine(ex.ToString());
            }


            

        }


        public void CalculeScores()
        {

            Board.CalculeScores(this);



            if (Utils.ComputerColor == "B")
                Weight = Board.BlackScore - Board.WhiteScore;
            else
                Weight = Board.WhiteScore - Board.BlackScore;

        }

        public NodeChess2 GetRootParent(NodeChess2 inNode)
        {
            if (inNode.Parent.Level == 1)
                return inNode.Parent;
            else
                return GetRootParent(inNode.Parent);
        }
    }

}
