using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEngine4 : IChessEngine
    {
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());
        private readonly ConcurrentDictionary<long, TranspositionEntry> _transpositionTable = new ConcurrentDictionary<long, TranspositionEntry>();
        private const int MATE_SCORE = 9999;
       // private const int MAX_SEARCH_TIME_MS = 5000; // 5 secondes maximum de réflexion
        private static readonly object lockObj = new object();
        private int _depthLevel = 0;
        private DateTime _startTime;

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
        // Structure pour stocker plus d'informations dans la table de transposition
        private struct TranspositionEntry
        {
            public int Score { get; set; }
            public int Depth { get; set; }
            public Move BestMove { get; set; }
            public NodeType Type { get; set; }
        }

        private enum NodeType
        {
            Exact,
            UpperBound,
            LowerBound
        }

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 7, int maxReflectionTimeInMinute = 2)
        {
            _startTime = DateTime.UtcNow;
            _depthLevel = depthLevel;
           
            string opponentColor = boardChess.GetOpponentColor(colore);
            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"cpuColor :  {colore}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");

            var cpuColor = colore.First().ToString();

            // Iterative Deepening
            NodeCE bestNode = null;
            //for (int currentDepth = 1; currentDepth <= depthLevel; currentDepth++)
            //{
            //    var newBestNode = FindBestMode(boardChess, currentDepth, cpuColor);
            //    if (DateTime.UtcNow - _startTime > TimeSpan.FromMilliseconds(MAX_SEARCH_TIME_MS))
            //        break;
            //    bestNode = newBestNode;
            //}
            bestNode = FindBestMode(boardChess, depthLevel, cpuColor);
            return bestNode;
        }

        private NodeCE FindBestMode(BoardCE board, int depthLevel, string cpuColor)
        {
            var moves = OrderMoves(board.GetPossibleMovesForColor(cpuColor, true), board, cpuColor);
            var equivalentBestNodeCEList = new ConcurrentBag<NodeCE>();
            int bestValue = int.MinValue;
            var allNodes = new ConcurrentBag<NodeCE>();

            // Utilisation de AsParallel() pour une meilleure parallélisation
            moves.AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForAll(move =>
                {
                   // if (DateTime.UtcNow - _startTime > TimeSpan.FromMilliseconds(MAX_SEARCH_TIME_MS))
                  //      return;

                    var clonedBoard = _boardPool.Get();
                    try
                    {
                        //board.CopyTo(clonedBoard);
                        //clonedBoard.MakeMove(move);
                         clonedBoard = board.CloneAndMove(move);

                        int value = -NegaMax(clonedBoard, depthLevel - 1, -int.MaxValue, -bestValue, cpuColor);
                        var currentNode = new NodeCE(clonedBoard, move, value, depthLevel, DateTime.UtcNow - _startTime);

                        // Ajustements tactiques
                        AdjustNodeValue(currentNode, clonedBoard, cpuColor);

                        allNodes.Add(currentNode);

                        lock (lockObj)
                        {
                            if (value > bestValue)
                            {
                                bestValue = value;
                                equivalentBestNodeCEList.Clear();
                                equivalentBestNodeCEList.Add(currentNode);
                                Utils.WritelineAsync($"{currentNode} *");
                            }
                            else if (value == bestValue)
                            {
                                equivalentBestNodeCEList.Add(currentNode);
                                
                            }
                        }
                    }
                    finally
                    {
                        _boardPool.Return(clonedBoard);
                    }
                });

            var finalNodes = equivalentBestNodeCEList.ToList();
            //ORIGINAL
             //var bestNode = SelectBestNode(finalNodes, board, cpuColor);
            var bestNode = SelectBestNode(finalNodes);
            bestNode.EquivalentBestNodeCEList = finalNodes;
            bestNode.AllNodeCEList = allNodes.ToList();
            bestNode.ReflectionTime = DateTime.UtcNow - _startTime;
            Utils.WritelineAsync($"bestNode : {bestNode}");
            return bestNode;
        }

        private int NegaMax(BoardCE board, int depth, int alpha, int beta, string color)
        {
            var boardHash = ComputeBoardHash(board);
            var alphaOrig = alpha;

            // Vérification de la table de transposition
            if (_transpositionTable.TryGetValue(boardHash, out var ttEntry) && ttEntry.Depth >= depth)
            {
                if (ttEntry.Type == NodeType.Exact)
                    return ttEntry.Score;
                else if (ttEntry.Type == NodeType.LowerBound)
                    alpha = Math.Max(alpha, ttEntry.Score);
                else if (ttEntry.Type == NodeType.UpperBound)
                    beta = Math.Min(beta, ttEntry.Score);

                if (alpha >= beta)
                    return ttEntry.Score;
            }

           // if (DateTime.UtcNow - _startTime > TimeSpan.FromMilliseconds(MAX_SEARCH_TIME_MS))
            //    return board.CalculateBoardCEScore(color, board.GetOpponentColor(color));

            if (depth <= 0)
                return QuiescenceSearch(board, alpha, beta, color);

            var moves = OrderMoves(board.GetPossibleMovesForColor(color), board, color);
            if (moves.Count == 0)
                return board.IsKingInCheck(color) ? -MATE_SCORE : 0;

            Move bestMove = null;
            int bestValue = int.MinValue;

            foreach (var move in moves)
            {
                var clonedBoard = _boardPool.Get();
                try
                {
                    //board.CopyTo(clonedBoard);
                    //clonedBoard.MakeMove(move);
                     clonedBoard = board.CloneAndMove(move);

                    int value = -NegaMax(clonedBoard, depth - 1, -beta, -alpha, board.GetOpponentColor(color));

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = move;

                        if (value > alpha)
                        {
                            alpha = value;
                            if (alpha >= beta)
                                break;
                        }
                    }
                }
                finally
                {
                    _boardPool.Return(clonedBoard);
                }
            }

            // Stockage dans la table de transposition
            var ttType = bestValue <= alphaOrig ? NodeType.UpperBound :
                        bestValue >= beta ? NodeType.LowerBound :
                        NodeType.Exact;

            _transpositionTable.TryAdd(boardHash, new TranspositionEntry
            {
                Score = bestValue,
                Depth = depth,
                BestMove = bestMove,
                Type = ttType
            });

            return bestValue;
        }

        private int QuiescenceSearch(BoardCE board, int alpha, int beta, string color)
        {
            int standPat = board.CalculateBoardCEScore(color, board.GetOpponentColor(color));

            if (standPat >= beta)
                return beta;

            if (alpha < standPat)
                alpha = standPat;

            foreach (var move in GetCaptureMoves(board, color))
            {
                var clonedBoard = _boardPool.Get();
                try
                {
                    //board.CopyTo(clonedBoard);
                    //clonedBoard.MakeMove(move);
                     clonedBoard = board.CloneAndMove(move);

                    int score = -QuiescenceSearch(clonedBoard, -beta, -alpha, board.GetOpponentColor(color));

                    if (score >= beta)
                        return beta;
                    if (score > alpha)
                        alpha = score;
                }
                finally
                {
                    _boardPool.Return(clonedBoard);
                }
            }

            return alpha;
        }

        private List<Move> GetCaptureMoves(BoardCE board, string color)
        {
            return board.GetPossibleMovesForColor(color)
                .Where(move => board._cases[move.ToIndex] != null)
                .OrderByDescending(move => EvaluateMove(board, move))
                .ToList();
        }

        private int EvaluateMove(BoardCE board, Move move)
        {
            var capturedPiece = board._cases[move.ToIndex];
            var movingPiece = board._cases[move.FromIndex];
            return board.GetPieceValue(capturedPiece) - (board.GetPieceValue(movingPiece) / 10);
        }

        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            return moves.OrderByDescending(move =>
            {
                int score = 0;

                // Priorité aux coups de la table de transposition
                var hash = ComputeBoardHash(board);
                if (_transpositionTable.TryGetValue(hash, out var ttEntry) &&
                    ttEntry.BestMove != null && move.Equals(ttEntry.BestMove))
                    score += 10000;

                // Captures
                var capturedPiece = board._cases[move.ToIndex];
                if (capturedPiece != null)
                {
                    score += board.GetPieceValue(capturedPiece) * 100;
                    var movingPiece = board._cases[move.FromIndex];
                    score -= board.GetPieceValue(movingPiece) / 10;
                }

                // Contrôle du centre
                var toRank = move.ToIndex / 8;
                var toFile = move.ToIndex % 8;
                if (Math.Abs(3.5 - toRank) + Math.Abs(3.5 - toFile) < 2)
                    score += 50;

                return score;
            }).ToList();
        }

        private NodeCE SelectBestNode(List<NodeCE> equivalentNodes, BoardCE board, string cpuColor)
        {
            if (equivalentNodes.Count <= 1)
                return equivalentNodes[0];

            return equivalentNodes
                .OrderByDescending(node =>
                {
                    var clonedBoard = _boardPool.Get();
                    try
                    {
                        //board.CopyTo(clonedBoard);
                        //clonedBoard.MakeMove(node.FromIndex, node.ToIndex);
                         clonedBoard = board.CloneAndMove(node.FromIndex, node.ToIndex);
                        return EvaluatePosition(clonedBoard, cpuColor);
                    }
                    finally
                    {
                        _boardPool.Return(clonedBoard);
                    }
                })
                .First();
        }


        private NodeCE SelectBestNode(List<NodeCE> equivalentNodes)
        {
            var maxWeight = equivalentNodes.Max(x => x.Weight);
            Utils.WritelineAsync($"maxWeight = {maxWeight}");

            var equivalentBestNodeCEList = equivalentNodes.Where(node => node.Weight == maxWeight).ToList();
            Utils.WritelineAsync($"bestNodeCEList :");
            foreach (var node in equivalentBestNodeCEList)
            {
                Utils.WritelineAsync($"{node}");
            }
            var bestNodeCE = equivalentBestNodeCEList[(new Random()).Next(equivalentBestNodeCEList.Count)];
            return bestNodeCE;
        }

        private void AdjustNodeValue(NodeCE node, BoardCE board, string cpuColor)
        {
            string opponentColor = board.GetOpponentColor(cpuColor);

            if (board.TargetIndexIsMenaced(node.ToIndex, opponentColor))
                node.Weight -= board.GetPieceValue(board._cases[node.ToIndex]) / 9;

            if (board.IsKingInCheck(opponentColor))
                node.Weight += 50;
        }

        private int EvaluatePosition(BoardCE board, string color)
        {
            string opponentColor = board.GetOpponentColor(color);
            return board.CalculateBoardCEScore(color, opponentColor);
        }

        private long ComputeBoardHash(BoardCE board)
        {
            unchecked
            {
                long hash = 17;
                for (int i = 0; i < board._cases.Length; i++)
                {
                    if (board._cases[i] != null)
                    {
                        hash = hash * 23 + board._cases[i].GetHashCode();
                        hash = hash * 23 + i;
                    }
                }
                return hash;
            }
        }
    }
}