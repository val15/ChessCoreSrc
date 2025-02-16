

using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEnginTestCupurMinMax : IChessEngine
    {
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();
        private readonly ConcurrentDictionary<long, int> _transpositionMinMaxTable = new ConcurrentDictionary<long, int>();

        private static object lockObj = new object();
        private int _depthLevel = 0;

        public void Dispose() { }

        public string GetName() => this.GetType().Name;

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6)
        {
            _depthLevel = depthLevel;
            string cpuColor = colore.First().ToString();
            string opponentColor = boardChess.GetOpponentColor(cpuColor);

            Utils.WritelineAsync($"[{GetName()}] DepthLevel: {depthLevel}, CPUColor: {cpuColor}, OpponentColor: {opponentColor}");
            return FindBestMode(boardChess, depthLevel, cpuColor);
        }

        public NodeCE FindBestMode(BoardCE board, int depthLevel, string cpuColor)
        {
            var startTime = DateTime.UtcNow;
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor, true);

            Move bestMove = null;
            int bestValue = int.MinValue;
            NodeCE bestNode = null;
            var allNodes = new ConcurrentBag<NodeCE>();

            Parallel.ForEach(possibleMoves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
            {
                var clonedBoard = board.CloneAndMove(move);
                string opponentColor = board.GetOpponentColor(cpuColor);

                int value = MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);

                var elapsed = DateTime.UtcNow - startTime;
                var currentNode = new NodeCE(clonedBoard, move, value, depthLevel, elapsed);

                // Vérification des menaces sur la pièce jouée
                bool isMenaced = clonedBoard.TargetIndexIsMenaced(currentNode.ToIndex, opponentColor);
                bool isProtected = clonedBoard.TargetIndexIsProtected(currentNode.ToIndex, cpuColor);
                if (isMenaced && !isProtected)
                {
                    value -= clonedBoard.GetPieceValue(clonedBoard._cases[currentNode.ToIndex]) / 10;
                    currentNode.Weight = value;
                }

                allNodes.Add(currentNode);

                lock (lockObj)
                {
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = move;
                        bestNode = currentNode;
                        Utils.WritelineAsync($"{currentNode} *");
                    }
                }
            });

            var bestNodeCEList = allNodes.Where(x => x.Weight == bestValue).ToList();
            var bestNodeCE = bestNodeCEList[new Random().Next(bestNodeCEList.Count)];
            bestNodeCE.EquivalentBestNodeCEList = bestNodeCEList;
            bestNodeCE.ReflectionTime = DateTime.UtcNow - startTime;

            Utils.WritelineAsync($"BestNode: {bestNodeCE}");
            return bestNodeCE;
        }

        private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var boardHash = ComputeBoardHash(board);
            if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
                return cachedScore;

            string opponentColor = board.GetOpponentColor(cpuColor);
            int currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTable[boardHash] = score;
                return score;
            }

            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;
            List<Move> moves = OrderMoves(board.GetPossibleMovesForColor(currentPlayer), board, currentPlayer);

            if (maximizingPlayer)
            {
                int bestValue = int.MinValue;
                Parallel.ForEach(moves, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);

                    lock (lockObj)
                    {
                        bestValue = Math.Max(bestValue, value);
                        alpha = Math.Max(alpha, bestValue);
                    }

                    if (beta <= alpha) return;
                });

                return bestValue + currentValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                Parallel.ForEach(moves, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);

                    lock (lockObj)
                    {
                        bestValue = Math.Min(bestValue, value);
                        beta = Math.Min(beta, bestValue);
                    }

                    if (beta <= alpha) return;
                });

                return bestValue + currentValue;
            }
        }

        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            return moves.OrderByDescending(move =>
            {
                var capturedPiece = board._cases[move.ToIndex];
                if (capturedPiece != null)
                    return board.GetPieceValue(capturedPiece) * 10;
                return 0;
            }).ToList();
        }

        private long ComputeBoardHash(BoardCE board)
        {
            long hash = 0;
            for (int i = 0; i < board._cases.Length; i++)
            {
                if (board._cases[i] != null)
                {
                    hash ^= (long)board._cases[i].GetHashCode() << (i % 16);
                }
            }
            return hash;
        }
    }
}

