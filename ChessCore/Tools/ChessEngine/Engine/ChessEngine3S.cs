using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEngine3S : IChessEngine
    {
        // Pool d'objets pour réduire les allocations mémoire
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition pour mettre en cache les états évalués
        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();

        private static readonly object lockObj = new object();
        private int _depthLevel = 0;
        private DateTime _startTime;
        private int MAX_SEARCH_TIME_S = 60 * 5;
        private bool _isExperedReflectionTime = false;

        public void Dispose() { }

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
            MAX_SEARCH_TIME_S = maxReflectionTimeInMinute * 60;
            _startTime = DateTime.UtcNow;
            _isExperedReflectionTime = false;
            var cpuColor = colore.First().ToString();
            _depthLevel = depthLevel;
            string opponentColor = boardChess.GetOpponentColor(cpuColor);

            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"MAX_SEARCH_TIME_S :  {MAX_SEARCH_TIME_S}");
            Utils.WritelineAsync($"cpuColor :  {cpuColor}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");

            NodeCE bestOfBest = null;
            for (int currentDepth = 1; currentDepth <= depthLevel; currentDepth++)
            {
                if (isReflextionLimitExpered()) break;
                bestOfBest = FindBestMode(boardChess, currentDepth, cpuColor);
            }
            return bestOfBest;
        }

        public NodeCE FindBestMode(BoardCE board, int depthLevel, string cpuColor)
        {
            var startTime = DateTime.UtcNow;
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor, true);
            Move bestMove = null;
            int bestValue = int.MinValue;
            NodeCE bestNode = null;
            var allNode = new List<NodeCE>();
            var equivalentBestNodeCEList = new List<NodeCE>();

            Parallel.ForEach(possibleMoves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
            {
                if (isReflextionLimitExpered())
                {
                    return;
                }

                int value = 0;
                var clonedBoard = board.CloneAndMove(move);
                var opponentColor = board.GetOpponentColor(cpuColor);
                value = MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);

                var elapsed = DateTime.UtcNow - startTime;
                var currentNode = new NodeCE(clonedBoard, move, value, depthLevel, elapsed);

                var isMenaced = clonedBoard.TargetIndexIsMenaced(currentNode.ToIndex, opponentColor);
                if (isMenaced)
                {
                    value -= clonedBoard.GetPieceValue(clonedBoard._cases[currentNode.ToIndex]) / 9;
                    currentNode.Weight = value;
                }

                lock (lockObj)
                {
                    allNode.Add(currentNode);
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = move;
                        bestNode = currentNode;
                        Utils.WritelineAsync($"{currentNode} *");
                    }
                }
            });

            equivalentBestNodeCEList = allNode.Where(x => x.Weight == bestValue).ToList();

            if (equivalentBestNodeCEList.Count > 1)
            {
                Utils.WritelineAsync($"bestNodeCEList calcul immediatelyWeight:");

                for (int i = 0; i < equivalentBestNodeCEList.Count; i++)
                {
                    var node = equivalentBestNodeCEList[i];
                    var cloanBoard = board.CloneAndMove(node.FromIndex, node.ToIndex);
                    var opponentColor = board.GetOpponentColor(cpuColor);
                    var immediatelyWeight = cloanBoard.CalculateBoardCEScore(cpuColor, opponentColor);
                    equivalentBestNodeCEList[i].Weight += immediatelyWeight / 10;
                }
                bestValue = equivalentBestNodeCEList.Max(x => x.Weight);
                equivalentBestNodeCEList = equivalentBestNodeCEList.Where(x => x.Weight == bestValue).ToList();
            }

            Utils.WritelineAsync($"bestNodeCEList :");
            foreach (var node in equivalentBestNodeCEList)
            {
                Utils.WritelineAsync($"{node}");
            }

            var bestNodeCE = equivalentBestNodeCEList[(new Random()).Next(equivalentBestNodeCEList.Count)];

            bestNodeCE.EquivalentBestNodeCEList = equivalentBestNodeCEList;
            bestNodeCE.AllNodeCEList = allNode;
            var elapsed = DateTime.UtcNow - startTime;

            if (_isExperedReflectionTime)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Utils.WritelineAsync("REFLECTION TIME LIMIT EXPERED");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Utils.WritelineAsync($"REFLECTION TIME: {elapsed}");
            bestNodeCE.ReflectionTime = elapsed;

            return bestNodeCE;
        }

        private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);

            if (isReflextionLimitExpered())
                return board.CalculateBoardCEScore(cpuColor, opponentColor);

            var boardHash = ComputeBoardHash(board, depth, cpuColor);

            if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
                return cachedScore;

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            if (depth == 0 || board.IsGameOver())
            {
                if (_transpositionTable.TryGetValue(boardHash, out cachedScore))
                    return cachedScore;

                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTable[boardHash] = score;
                return score;
            }

            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor))
                {
                    return -9999;
                }

                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;
            List<Move> moves = OrderMoves(board.GetPossibleMovesForColor(currentPlayer), board, currentPlayer);

            if (maximizingPlayer)
            {
                int bestValue = int.MinValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);

                    bestValue = Math.Max(bestValue, value);
                    alpha = Math.Max(alpha, bestValue);

                    if (beta <= alpha) break; // Élagage alpha-bêta
                }
                return bestValue + currentValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);

                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);

                    if (beta <= alpha) break; // Élagage alpha-bêta
                }
                return bestValue + currentValue;
            }
        }

        private bool isReflextionLimitExpered()
        {
            if (DateTime.UtcNow - _startTime > TimeSpan.FromSeconds(MAX_SEARCH_TIME_S))
            {
                _isExperedReflectionTime = true;
                return true;
            }
            return false;
        }

        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            return moves.OrderByDescending(move =>
            {
                var capturedPiece = board._cases[move.ToIndex];
                if (capturedPiece != null)
                    return board.GetPieceValue(capturedPiece) * 10; // Prioriser les captures

                // Prioriser les coups qui mettent le roi en échec
                var clonedBoard = board.CloneAndMove(move);
                if (clonedBoard.IsKingInCheck(board.GetOpponentColor(color)))
                    return 1000;

                return 0;
            }).ToList();
        }

        private long ComputeBoardHash(BoardCE board, int depth, string color)
        {
            long hash = 0;
            for (int i = 0; i < board._cases.Length; i++)
            {
                if (board._cases[i] != null)
                {
                    hash ^= (long)board._cases[i].GetHashCode() << (i % 16);
                }
            }
            hash ^= depth << 24;
            hash ^= color.GetHashCode() << 32;
            return hash;
        }
    }
}