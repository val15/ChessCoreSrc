namespace ChessCore.Tools
{
    public class NodeChess2
    {
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


        public NodeChess2(NodeChess2 parent, Board board, int level, string color, int formIndex, int toIndex, string computeurColor, int maxDeepLevel)
        {
            FromIndex = formIndex;
            ToIndex = toIndex;
            Level = level;
            Board = board;
            Parent = parent;
            Color = color;
            ChildList = new List<NodeChess2>();

            //Pour T69
            /* var kingIndex = board.GetCases().ToList().IndexOf($"K|{computeurColor}");
               if (kingIndex == -1)
                 Weight =  -999;

               */
            /*if(parent != null)
            {
              if (Utils.KingIsInChess(parent.Board, Utils.OpinionColor))
              {
                var t_in = 0;
              }
            }
         */






            if (Level == maxDeepLevel)
            {

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





                /* if(Level < 4)
                 {
                   // pour T87
                   if (Utils.KingIsInChess(board, Utils.ComputerColor))
                   {
                     Weight = -999;
                     return;
                   }

                   //if (Utils.KingIsInChess2(Board, Utils.OpinionColor))
                   //{
                   //  Weight = 999;
                   //  return;
                   //}
                 }*/








                Board.CalculeScores();
                if (computeurColor == "B")
                    Weight = Board.BlackScore - Board.WhiteScore;
                else
                    Weight = Board.WhiteScore - Board.BlackScore;











            }










        }

        public void CalculeScores()
        {

            Board.CalculeScores();

            /* if(Utils.KingIsOut(Board, Utils.ComputerColor))
             {
               Weight = -999;
               return;
             }

             if (Utils.KingIsOut(Board, Utils.OpinionColor))
             {
               Weight = 999;
               return;
             }*/

            /*if (Utils.KingIsInChess(Board, Utils.ComputerColor))
            {
              Weight = -999;
              return; 
            }

            if (Utils.KingIsInChess(Board, Utils.OpinionColor))
            {
              Weight = 999;
              return;
            }*/

            if (Utils.ComputerColor == "B")
                Weight = Board.BlackScore - Board.WhiteScore;
            else
                Weight = Board.WhiteScore - Board.BlackScore;

        }
    }

}
