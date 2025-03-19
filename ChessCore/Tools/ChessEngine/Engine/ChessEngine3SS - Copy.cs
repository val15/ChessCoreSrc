using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEngine3SS : IChessEngine
    {
        // Pool d'objets pour limiter les allocations
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition qui stocke (score, profondeur, type de score)
        private readonly ConcurrentDictionary<long, TranspositionEntry> _transpositionTable = new ConcurrentDictionary<long, TranspositionEntry>();

        private struct TranspositionEntry
        {
            public int Score;
            public int Depth;
            public EntryType Flag;
        }
        private enum EntryType
        {
            Exact,
            LowerBound,
            UpperBound
        }

        private int _maxSearchTimeSeconds = 300; // par défaut 5 minutes
        private DateTime _startTime;
        private bool _timeLimitReached = false;
        private int _maxDepth;

        // Killer moves : stocke pour chaque profondeur les coups ayant provoqué un cutoff
        private readonly Dictionary<int, List<Move>> _killerMoves = new Dictionary<int, List<Move>>();

        public void Dispose() { }

        public string GetName() => GetType().Name;
        public string GetShortName() => Utils.ExtractUppercaseLettersAndDigits(GetName());

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {
            _maxSearchTimeSeconds = maxReflectionTimeInMinute * 60;
            _startTime = DateTime.UtcNow;
            _timeLimitReached = false;
            _maxDepth = depthLevel;
            var cpuColor = colore.First().ToString();
            var bestNode = IterativeDeepeningSearch(boardChess, cpuColor);
            return bestNode;
        }

        private NodeCE IterativeDeepeningSearch(BoardCE board, string cpuColor)
        {
            try
            {
                NodeCE bestNode = null;
                int previousScore = 0;
                int aspirationWindow = 50; // largeur initiale de la fenêtre d'aspiration
                                           // On itère de 1 jusqu'à la profondeur maximale souhaitée
                for (int depth = 1; depth <= _maxDepth; depth++)
                {
                    if (_timeLimitReached) break;
                    int alpha = previousScore - aspirationWindow;
                    int beta = previousScore + aspirationWindow;
                    int score = 0;
                    NodeCE currentBestNode = null;
                    var moves = board.GetPossibleMovesForColor(cpuColor, true);
                    // Tri des coups (en privilégiant les killer moves et captures)
                    moves = OrderMoves(moves, board, cpuColor);
                    foreach (var move in moves)
                    {
                        if (_timeLimitReached) break;
                        var clonedBoard = board.CloneAndMove(move);
                        score = -AlphaBeta(clonedBoard, depth - 1, -beta, -alpha, false, cpuColor);
                        if (_timeLimitReached) break;
                        // Si le score se situe en dehors de la fenêtre, on refait une recherche avec des bornes larges
                        if (score <= alpha || score >= beta)
                        {
                            score = -AlphaBeta(clonedBoard, depth - 1, int.MinValue, int.MaxValue, false, cpuColor);
                        }
                        if (score > alpha)
                        {
                            alpha = score;
                            currentBestNode = new NodeCE(clonedBoard, move, score, depth, DateTime.UtcNow - _startTime);
                        }
                    }
                    if (!_timeLimitReached)
                    {
                        bestNode = currentBestNode;
                        previousScore = alpha;
                        Utils.WritelineAsync($"Depth {depth} complétée, meilleur score: {previousScore}");
                    }
                }
                if (bestNode != null)
                {
                    bestNode.ReflectionTime = DateTime.UtcNow - _startTime;
                }
                return bestNode;
            }
            catch (Exception ex)
            {

                Utils.WritelineAsync($"Error : {ex}");
                return null;
            }
            
        }

        private int AlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            if (_timeLimitReached || TimeExceeded())
            {
                _timeLimitReached = true;
                return board.CalculateBoardCEScore(cpuColor, board.GetOpponentColor(cpuColor));
            }

            var opponentColor = board.GetOpponentColor(cpuColor);
            long hash = ComputeBoardHash(board, depth, cpuColor);
            if (_transpositionTable.TryGetValue(hash, out var entry) && entry.Depth >= depth)
            {
                if (entry.Flag == EntryType.Exact)
                    return entry.Score;
                if (entry.Flag == EntryType.LowerBound)
                    alpha = Math.Max(alpha, entry.Score);
                else if (entry.Flag == EntryType.UpperBound)
                    beta = Math.Min(beta, entry.Score);
                if (alpha >= beta)
                    return entry.Score;
            }

            if (depth == 0 || board.IsGameOver())
            {
                int eval = QuiescenceSearch(board, alpha, beta, cpuColor);
                _transpositionTable[hash] = new TranspositionEntry { Score = eval, Depth = depth, Flag = EntryType.Exact };
                return eval;
            }

            // Null move pruning : si le roi n'est pas en échec et la profondeur est suffisante
            if (depth >= 3 && !board.IsKingInCheck(cpuColor))
            {
                var nullMoveBoard = board.Clone(); // pas de mouvement effectué
                int R = 2; // réduction
                int nullScore = -AlphaBeta(nullMoveBoard, depth - 1 - R, -beta, -beta + 1, false, cpuColor);
                if (nullScore >= beta)
                {
                    return nullScore;
                }
            }

            int bestValue = int.MinValue;
            var moves = board.GetPossibleMovesForColor(maximizingPlayer ? cpuColor : opponentColor);
            moves = OrderMoves(moves, board, maximizingPlayer ? cpuColor : opponentColor);
            foreach (var move in moves)
            {
                var clonedBoard = board.CloneAndMove(move);
                int value = -AlphaBeta(clonedBoard, depth - 1, -beta, -alpha, !maximizingPlayer, cpuColor);
                if (value > bestValue)
                {
                    bestValue = value;
                    // Si le coup n'est pas une capture, on enregistre en tant que killer move
                    if (!IsCaptureMove(move, board))
                    {
                        if (!_killerMoves.ContainsKey(depth))
                            _killerMoves[depth] = new List<Move>();
                        _killerMoves[depth].Add(move);
                    }
                }
                alpha = Math.Max(alpha, bestValue);
                if (alpha >= beta)
                {
                    break; // coupure beta
                }
            }

            // Stockage dans la table de transposition avec le flag approprié
            EntryType flag = EntryType.Exact;
            if (bestValue <= alpha)
                flag = EntryType.UpperBound;
            else if (bestValue >= beta)
                flag = EntryType.LowerBound;
            _transpositionTable[hash] = new TranspositionEntry { Score = bestValue, Depth = depth, Flag = flag };

            return bestValue;
        }

        private int QuiescenceSearch(BoardCE board, int alpha, int beta, string cpuColor)
        {
            int standPat = board.CalculateBoardCEScore(cpuColor, board.GetOpponentColor(cpuColor));
            if (standPat >= beta)
                return beta;
            if (alpha < standPat)
                alpha = standPat;

            // On ne considère que les coups de capture en recherche de quiétude
            var captureMoves = board.GetPossibleMovesForColor(cpuColor)
                .Where(m => board._cases[m.ToIndex] != null)
                .ToList();
            captureMoves = OrderMoves(captureMoves, board, cpuColor);

            foreach (var move in captureMoves)
            {
                var clonedBoard = board.CloneAndMove(move);
                int score = -QuiescenceSearch(clonedBoard, -beta, -alpha, cpuColor);
                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
            }
            return alpha;
        }

        private bool TimeExceeded()
        {
            return (DateTime.UtcNow - _startTime).TotalSeconds > _maxSearchTimeSeconds;
        }

        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            // Les coups killer, les captures, puis les autres sont privilégiés
            var killer = _killerMoves.SelectMany(kvp => kvp.Value).Distinct();
            return moves.OrderByDescending(move =>
            {
                int score = 0;
                if (killer.Contains(move)) score += 1000;
                if (board._cases[move.ToIndex] != null)
                    score += board.GetPieceValue(board._cases[move.ToIndex]) * 10;
                return score;
            }).ToList();
        }

        private bool IsCaptureMove(Move move, BoardCE board)
        {
            return board._cases[move.ToIndex] != null;
        }

        // Pour simplifier, on utilise ici une fonction de hachage basique.
        // Pour une implémentation plus robuste, il est recommandé d'utiliser le Zobrist hashing.
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
