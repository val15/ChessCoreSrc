using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using System;

namespace ChessCore.Tools.ChessEngine.Engine
{

    //OPTIMIZE ENGINE DEFAULT DEPTH LEVEL 5

    //SIMPLE : NOT CHAS
    public class ChessEngine2 : IChessEngine
    {


        private static object lockObj = new object();
        private int _depthLevel = 0;

        public void Dispose()
        {

        }
        public string GetName()
        {
            return this.GetType().Name;
        }

        public string GetShortName()
        {
            return Utils.ExtractUppercaseLettersAndDigits(GetName());
        }
        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {
            var cpuColor = colore.First().ToString();
            _depthLevel = depthLevel;
            string opponentColor = boardChess.GetOpponentColor(cpuColor);
            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"cpuColor :  {cpuColor}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");

            //var bestList = new List<NodeCE>();
            //bestList.Add(FindBestMode(boardChess, 3, cpuColor));
            //bestList.Add(FindBestMode(boardChess, 5, cpuColor));
            //for (int l = 2; l <= depthLevel; l++)
            //{
            //    bestList.Add(FindBestMode(boardChess, l, cpuColor));
            //}
            //Utils.WritelineAsync($"bestList :");
            //foreach (var node in bestList)
            //{
            //    Utils.WritelineAsync($"{node}");
            //}
            //var maxWWeight = bestList.Max(x => x.Weight);
            //Utils.WritelineAsync($"maxWWeight :  {maxWWeight}");
            //var bestOfBestList = bestList.Where(x=>x.Weight == maxWWeight);
            //Utils.WritelineAsync($"bestOfBestList :");
            //foreach (var node in bestOfBestList)
            //{
            //    Utils.WritelineAsync($"{node}");
            //}
            //var bestOfBest = bestOfBestList.OrderByDescending(x=>x.Weight).OrderBy(x=>x.Level).First();
            var bestOfBest = FindBestMode(boardChess, depthLevel, cpuColor);
            //Utils.WritelineAsync($"bestOfBest :  {bestOfBest}");
            return bestOfBest;
        }

        public NodeCE FindBestMode(BoardCE board, int depthLevel, string cpuColor)
        {
            var startTime = DateTime.UtcNow;
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor);
            Move bestMove = null;
            int bestValue = int.MinValue;
            NodeCE bestNode = null;
            var allNomde = new List<NodeCE>();
            var equivalentBestNodeCEList = new List<NodeCE>();
            Parallel.ForEach(possibleMoves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
            {
                int value = 0;
                var clonedBoard = board.CloneAndMove(move);


                var opponentColor = board.GetOpponentColor(cpuColor);
                var currentW = 0;
                value = currentW + MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);
                var elapsed = DateTime.UtcNow - startTime;
                var currentNode = new NodeCE(clonedBoard, move, value, depthLevel, elapsed);


                //T140_W_notE3toC5 and T78ProtectionDuRookDesNoirs
                var isMenaced = clonedBoard.TargetIndexIsMenaced(currentNode.ToIndex, opponentColor);
                var isProtected = clonedBoard.TargetIndexIsProtected(currentNode.ToIndex, cpuColor);
                if (isMenaced && !isProtected)
                {
                    value -= clonedBoard.GetPieceValue(clonedBoard._cases[currentNode.ToIndex]) / 10;
                    currentNode.Weight = value;
                }



                allNomde.Add(currentNode);
                // Utils.WritelineAsync($"value = {value}");
                lock (lockObj)
                {
                    if (value > bestValue)
                    {
                        //  Utils.WritelineAsync($"bestValue = {currentNode}");
                        bestValue = value;
                        bestMove = move;
                        Utils.WritelineAsync($"{currentNode} *");
                        bestNode = currentNode;

                    }
                }

            });

            equivalentBestNodeCEList = allNomde.Where(x => x.Weight == bestValue).ToList();
            var rand = new Random();
            var bestNodeCE = equivalentBestNodeCEList[rand.Next(equivalentBestNodeCEList.Count)];


            Utils.WritelineAsync($"bestNodeCEList :");
            foreach (var node in equivalentBestNodeCEList)
            {
                Utils.WritelineAsync($"{node}");
            }
            bestNodeCE.EquivalentBestNodeCEList = equivalentBestNodeCEList;
            var elapsed = DateTime.UtcNow - startTime;
            Utils.WritelineAsync($"REFLECTION TIME: {elapsed}");
            // Utils.WritelineAsync($"Utils.PossibleMovesListCount = {Utils.PossibleMovesList.Count()}");
            //Utils.WritelineAsync($"Utils.IsKingInCheckListCount = {Utils.IsKingInCheckList.Count()}");
            //Utils.WritelineAsync($"Best node : {bestNodeCE}");

            bestNodeCE.ReflectionTime = elapsed;


            Utils.WritelineAsync($"bestNode : {bestNodeCE}");
            return bestNodeCE;
        }

        private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            //check in first depth
            //T07aEchecRookBlancDoitAttaquerLeRoiNoir
            if (depth >= _depthLevel - 2)
            {

                if (board.IsKingInCheck(cpuColor))
                    return -9999;
                if (board.IsKingInCheck(opponentColor))
                    return 9999;

            }


            if (depth == 0 || board.IsGameOver())
            {
                var score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                return score;
            }


            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;
            List<Move> moves = board.GetPossibleMovesForColor(currentPlayer);

            if (maximizingPlayer)
            {
                int bestValue = int.MinValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    bestValue = Math.Max(bestValue, value);
                    alpha = Math.Max(alpha, bestValue);
                    if (beta <= alpha) break;
                }
                return bestValue + currentValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);
                    if (beta <= alpha) break;
                }
                return bestValue + currentValue;
            }
        }
    }

}
