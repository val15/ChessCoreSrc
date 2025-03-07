

using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEngineTestOptimClaudeIA : IChessEngine
    {
        // Pool d'objets pour réduire les allocations mémoire
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition pour mettre en cache les états évalués
        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();

        private int _depthLevel = 0;
        private const int MATE_SCORE = 10000;
        private const int DRAW_SCORE = 0;

        public void Dispose() { }

        public string GetName()
        {
            return this.GetName();
        }

        public string GetShortName()
        {
            return Utils.ExtractUppercaseLettersAndDigits(GetName());
        }
        public NodeCE GetBestModeCE(string color, BoardCE boardChess, int depthLevel = 5, int maxReflectionTimeInSecond = 60 * 2)
        {
            var cpuColor = color.First().ToString();
            _depthLevel = depthLevel;
            string opponentColor = boardChess.GetOpponentColor(cpuColor);

            // Utilisation de Stopwatch pour un suivi de temps plus léger
            var stopwatch = Stopwatch.StartNew();

            var bestNode = FindBestMove(boardChess, depthLevel, cpuColor);

            stopwatch.Stop();
            bestNode.ReflectionTime = stopwatch.Elapsed;

            return bestNode;
        }

        public NodeCE FindBestMove(BoardCE board, int depthLevel, string cpuColor)
        {
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor);
            var bestNodes = new ConcurrentBag<NodeCE>();
            var startTime = DateTime.UtcNow;
            // Ordonnancement des mouvements pour améliorer l'élagage
            var orderedMoves = OrderMoves(possibleMoves, board, cpuColor);

            Parallel.ForEach(orderedMoves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
            {
                // Utilisation du pool d'objets pour réduire les allocations
                var clonedBoard = _boardPool.Get();
                clonedBoard = clonedBoard.CopyFrom(board);
                clonedBoard.Move(move);

                var opponentColor = board.GetOpponentColor(cpuColor);
                //  int value = EvaluateMove(clonedBoard, depthLevel - 1, cpuColor, move);
                var value = MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);


                var node = new NodeCE(clonedBoard, move, value, depthLevel);
                //AdjustNodeValueForSpecialConditions(node, clonedBoard, opponentColor, cpuColor);
                var elapsed = DateTime.UtcNow - startTime;
                node.ReflectionTime = elapsed;
                bestNodes.Add(node);
                Utils.WritelineAsync($"{node}");
                // Retour au pool après utilisation
                _boardPool.Return(clonedBoard);
            });

            // Sélection du meilleur mouvement
            var bestNodesSeconde = bestNodes.OrderByDescending(n => n.Weight).ToList();
            var equivalentBestNodes = bestNodesSeconde.Where(n => n.Weight == bestNodesSeconde[0].Weight).ToList();
            Utils.WritelineAsync("equivalentBestNodes : ");
            foreach (var node in equivalentBestNodes)
            {
                Utils.WritelineAsync($"{node}");
            }

            var randomBestNode = equivalentBestNodes[new Random().Next(equivalentBestNodes.Count)];
            randomBestNode.EquivalentBestNodeCEList = equivalentBestNodes;
            var elapsed = DateTime.UtcNow - startTime;
            Utils.WritelineAsync($"REFLECTION TIME: {elapsed}");
            randomBestNode.ReflectionTime = elapsed;
            Utils.WritelineAsync($"bestNode : {randomBestNode}");
            return randomBestNode;
        }

        private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
                return cachedScore;

            var opponentColor = board.GetOpponentColor(cpuColor);
            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;
            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTable[boardHash] = score + currentValue;
                return score;
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
            List<Move> moves = OrderMoves(board.GetPossibleMovesForColor(currentPlayer), board, currentPlayer);

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


        private int MinMaxWithAlphaBeta3333(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide des valeurs connues
            var boardHash = ComputeBoardHash(board);
            if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
                return cachedScore;

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

            // Conditions d'arrêt rapides
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTable[boardHash] = score;
                return score;
            }

            // Optimisation : pré-calculer les informations communes
            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;
            List<Move> moves = OrderMoves(board.GetPossibleMovesForColor(currentPlayer), board, currentPlayer);

            if (maximizingPlayer)
            {
                int bestValue = int.MinValue;
                foreach (Move move in moves)
                {
                    // Utiliser une méthode de clonage plus légère si possible
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);

                    bestValue = Math.Max(bestValue, value);
                    alpha = Math.Max(alpha, bestValue);

                    // Élagage alpha-beta précoce
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

                    // Élagage alpha-beta précoce
                    if (beta <= alpha) break;
                }
                return bestValue + currentValue;
            }
        }


        // Méthode pour ordonner les mouvements et améliorer l'élagage
        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            return moves.OrderByDescending(move =>
            {
                // Prioriser les captures
                var capturedPiece = board._cases[move.ToIndex];
                if (capturedPiece != null)
                    return board.GetPieceValue(capturedPiece) * 10;

                // Autres critères de priorité
                return 0;
            }).ToList();
        }

        // Ajustement du poids du nœud pour des conditions spéciales
        private void AdjustNodeValueForSpecialConditions(NodeCE node, BoardCE board, string opponentColor, string cpuColor)
        {
            var isMenaced = board.TargetIndexIsMenaced(node.ToIndex, opponentColor);
            var isProtected = board.TargetIndexIsProtected(node.ToIndex, cpuColor);

            if (isMenaced && !isProtected)
            {
                node.Weight -= board.GetPieceValue(board._cases[node.ToIndex]) / 10;
            }
        }

        // Méthode simple de hachage pour la table de transposition
        private long ComputeBoardHash(BoardCE board)
        {
            // Implémentation simplifiée, à remplacer par un hachage plus robuste
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

    // Implémentation simple d'un pool d'objets
    public class ObjectPool<T> where T : new()
    {
        private ConcurrentBag<T> _objects = new ConcurrentBag<T>();
        private Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator = null)
        {
            _objectGenerator = objectGenerator ?? (() => new T());
        }

        public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();

        public void Return(T item) => _objects.Add(item);
    }
}