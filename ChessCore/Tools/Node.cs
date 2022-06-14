namespace ChessCore.Tools
{
    public class Node
    {

        public NodeChess2 AsssociateNodeChess2 { get; set; } = new NodeChess2();

        public List<Pawn> CurrentLocalPawnList { get; set; }
        public int Level { get; set; }
        public int MakeCheckmateLevel { get; set; }

        public int IsProtected { get; set; }
        public bool IsMenaced { get; set; }

        public double Weight { get; set; }
        public string Location { get; set; }
        public string OldPositionName { get; set; }

        public string Colore { get; set; }




        public string BestChildPosition { get; set; }
        public Pawn AssociatePawn { get; set; }

        //public TreeNode<CircleNode> ParentTreeNode { get; set; }

        public List<Pawn> GetCurrentLocalPawnList()
        {
            return CurrentLocalPawnList;
        }

        public List<Pawn> GetCurrentLocalPawnListAllier()
        {
            return CurrentLocalPawnList.Where(x => x.Colore == Colore).ToList();
        }
        public Node()
        {
        }
        public int GetValue()
        {
            return AssociatePawn.Value;
        }

        public Node(int level, int makeCheckmateLevel, int weight, string location, string oldPositionName, string colore, Node parent, List<Node> childList, string bestChildPosition, Pawn associatePawn, List<Pawn> currentLocalPawnList)
        {
            Level = level;
            MakeCheckmateLevel = makeCheckmateLevel;
            Weight = weight;
            Location = location;
            OldPositionName = oldPositionName;
            Colore = colore;

            BestChildPosition = bestChildPosition;
            AssociatePawn = associatePawn;
            CurrentLocalPawnList = currentLocalPawnList;
        }


        public Node(List<Pawn> currentLocalPawnList)
        {
            BestChildPosition = "";
            CurrentLocalPawnList = null;
            CurrentLocalPawnList = new List<Pawn>();
            CurrentLocalPawnList.AddRange(currentLocalPawnList);
        }

    }

}
