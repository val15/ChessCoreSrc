using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Emit;

namespace ChessCore.Tools.ChessEngine.Engine.SS
{
    public class ChessEngine3SS
    {
        // --- Transposition Table Entry ---
        private enum TTBound : byte { Lower, Upper, Exact }

        private struct TTEntry
        {
            // Use packing or smaller types if memory becomes an issue
            public long ZobristHash; // Store hash to detect collisions (Type 1 hash)
            public short Score;      // Score relative to the side to move
            public short Depth;      // Depth searched from this position
            public TTBound Bound;    // Type of score (Exact, Lower, Upper)
            public ushort BestMoveHash; // Hash/identifier of the best move found (optional but good)

            // Simple hash for the move (e.g., (from << 8) | to)
            // Requires Move struct/class to have a way to generate this short hash
            public static ushort MoveToShortHash(Move move)
            {
                if (move == null) return 0;
                // Example: simple hashing, adjust based on your Move implementation
                return (ushort)((move.FromIndex << 8) | move.ToIndex);
            }
            public static Move ShortHashToMove(ushort hash, BoardCE board, string color)
            {
                if (hash == 0) return null;
                int from = hash >> 8;
                int to = hash & 0xFF;
                // This part is tricky - need to reconstruct the full move details
                // It might be better to store the full Move object or more details if memory allows,
                // or require the BoardCE to validate if a move from->to is legal for the color.
                // For now, just return a basic move; validation needed later.
                var piece = board._cases[from];
                if (piece == null || !piece.EndsWith($"|{color}")) return null; // Ensure piece exists and belongs to player
                return new Move(from, to, piece); // Assuming Move constructor
            }
        }

        // Increased TT size, adjust based on available memory (needs power of 2 for masking)
        private const int TranspositionTableSize = 1024 * 1024 * 128; // Example: 128M entries
        private readonly TTEntry[] _transpositionTable = new TTEntry[TranspositionTableSize]; // Use array for performance
        private readonly object _ttLock = new object(); // Simple lock for TT access (can be bottleneck)
                                                        // Consider more advanced concurrent structures if needed

        // --- Killer Moves --- (Store 2 per ply)
        private const int MaxSearchDepth = 64; // Max practical search depth
        private readonly Move[,] _killerMoves = new Move[MaxSearchDepth, 2];

        // --- Search State ---
        private int _targetDepth = 6;
        private long _nodeCount = 0;
        private DateTime _startTime;
        private TimeSpan _maxSearchTime;
        private bool _stopSearch = false;
        private string _engineColor = "W"; // Default or set in GetBestModeCE

        // --- Object Pooling (Keep if BoardCE cloning is expensive) ---
        // private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        public void Dispose() { }

        public string GetName() => "ChessEngineStockfishInspired";
        public string GetShortName() => "CESI"; // Or Utils.ExtractUppercaseLettersAndDigits(GetName());

        public NodeCE GetBestModeCE(string color, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {
            try
            {
                _engineColor = color.First().ToString().ToUpper();
                _targetDepth = depthLevel;
                _maxSearchTime = TimeSpan.FromMinutes(maxReflectionTimeInMinute);
                _startTime = DateTime.UtcNow;
                _stopSearch = false;
                _nodeCount = 0;
                // Clear killers for a new search
                Array.Clear(_killerMoves, 0, _killerMoves.Length);
                // TT clearing is optional - can persist between moves, but clear hash collisions part
                // Consider clearing only entries with ZobristHash = 0 or using generation counters.

                string opponentColor = boardChess.GetOpponentColor(_engineColor);

                Utils.WritelineAsync($"{GetName()}");
                Utils.WritelineAsync($"Target Depth: {_targetDepth}");
                Utils.WritelineAsync($"Max Search Time: {_maxSearchTime}");
                Utils.WritelineAsync($"Engine Color: {_engineColor}");
                Utils.WritelineAsync($"Opponent Color: {opponentColor}");

                NodeCE bestNodeOverall = null;
                var searchTimer = Stopwatch.StartNew();

                // --- Iterative Deepening Loop ---
                for (int currentDepth = 1; currentDepth <= _targetDepth; currentDepth++)
                {
                    if (_stopSearch) break; // Check if time is up before starting iteration

                    var stopwatch = Stopwatch.StartNew();
                    int score = AlphaBetaNegaMax(boardChess, currentDepth, 0, -Constants.CheckmateScore, Constants.CheckmateScore, _engineColor);
                    stopwatch.Stop();

                    // Check time *after* completing a depth
                    if (DateTime.UtcNow - _startTime >= _maxSearchTime)
                    {
                        _stopSearch = true;
                        Utils.WritelineAsync("Time limit reached during search.");
                    }

                    // Retrieve best move from TT for the root position (if available and depth matches)
                    Move bestMoveThisIteration = GetBestMoveFromTT(boardChess);

                    if (bestMoveThisIteration != null)
                    {
                        // Update the overall best node found so far
                        var elapsed = DateTime.UtcNow - _startTime;
                        bestNodeOverall = new NodeCE(boardChess.CloneAndMove(bestMoveThisIteration), // Careful with cloning here if TT move is enough
                                                     bestMoveThisIteration,
                                                     score, // Score is from engine's perspective
                                                     currentDepth,
                                                     elapsed);

                        // Log information about this iteration
                        Utils.WritelineAsync($"Depth {currentDepth} | Score: {FormatScore(score)} | Best Move: {bestMoveThisIteration} | Nodes: {_nodeCount} | Time: {stopwatch.ElapsedMilliseconds}ms");
                    }
                    else
                    {
                        // This shouldn't happen if search completed unless no legal moves
                        Utils.WritelineAsync($"WARN: No best move found at depth {currentDepth}. Score: {FormatScore(score)}");
                        if (bestNodeOverall == null && currentDepth == 1)
                        {
                            // Handle no legal moves case / stalemate / checkmate
                            var possibleMoves = boardChess.GetPossibleMovesForColor(_engineColor, true);
                            if (!possibleMoves.Any())
                            {
                                Utils.WritelineAsync("No legal moves available.");
                                return new NodeCE(boardChess, null, score, currentDepth, DateTime.UtcNow - _startTime); // Return node indicating no move
                            }
                            // else: something unexpected happened, maybe TT issue or search bug
                        }
                    }


                    // Early exit if mate is found
                    if (IsCheckmateScore(score) && currentDepth >= 1)
                    {
                        Utils.WritelineAsync($"Checkmate found at depth {currentDepth}.");
                        break;
                    }
                }
                searchTimer.Stop();

                if (_stopSearch && bestNodeOverall != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Utils.WritelineAsync("SEARCH STOPPED DUE TO TIME LIMIT");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (bestNodeOverall == null)
                {
                    Utils.WritelineAsync("ERROR: No best move could be determined.");
                    // Fallback: pick a random legal move?
                    var moves = boardChess.GetPossibleMovesForColor(_engineColor, true);
                    if (moves.Any())
                    {
                        bestNodeOverall = new NodeCE(boardChess, moves.First(), 0, 0, TimeSpan.Zero); // Placeholder
                    }
                    else
                    {
                        // Truly no moves (mate/stalemate)
                        bestNodeOverall = new NodeCE(boardChess, null, Evaluate(boardChess, _engineColor), 0, TimeSpan.Zero);
                    }
                }

                Utils.WritelineAsync($"Total Reflection Time: {searchTimer.Elapsed}");
                Utils.WritelineAsync($"Total Nodes Searched: {_nodeCount}");
                bestNodeOverall.ReflectionTime = searchTimer.Elapsed;
                // We don't have the full list of root nodes and weights like before easily,
                // but IDDFS provides the principal variation's score and move.
                // bestNodeOverall.AllNodeCEList = ... (Could be populated if needed, but less relevant in IDDFS)
                // bestNodeOverall.EquivalentBestNodeCEList = ... (Could store PV from TT if needed)

                return bestNodeOverall;

            }
            catch (Exception ex)
            {

                return null;
            }
          
        }

        // --- Core Search Function (NegaMax Alpha-Beta) ---
        private int AlphaBetaNegaMax(BoardCE board, int depth, int ply, int alpha, int beta, string playerColor)
        {
            _nodeCount++;

            // Check for time limit periodically (e.g., every 2048 nodes)
            if ((_nodeCount & 2047) == 0 && DateTime.UtcNow - _startTime >= _maxSearchTime)
            {
                _stopSearch = true;
                return 0; // Return neutral score on timeout
            }
            if (_stopSearch) return 0;

            // --- Mate Distance Pruning ---
            // If we found mate earlier, adjust alpha/beta to find shorter mates
            alpha = Math.Max(alpha, -Constants.CheckmateScore + ply);
            beta = Math.Min(beta, Constants.CheckmateScore - ply);
            if (alpha >= beta) return alpha; // Pruned by mate distance

            // --- Transposition Table Lookup ---
            long boardHash = board.GetZobristHash(); // Assuming BoardCE provides this
            int ttIndex = (int)((ulong)boardHash % (ulong)TranspositionTableSize);
            TTEntry ttEntry = _transpositionTable[ttIndex]; // Read might not need lock if writes are careful
            Move ttBestMove = null;

            // Lock is needed for read/write consistency if multi-threading search later
            // For single thread IDDFS, it's mainly for collision detection check
            lock (_ttLock)
            {
                ttEntry = _transpositionTable[ttIndex];
                if (ttEntry.ZobristHash == boardHash && ttEntry.Depth >= depth)
                {
                    // Use TT score if depth is sufficient
                    switch (ttEntry.Bound)
                    {
                        case TTBound.Exact: return ttEntry.Score;
                        case TTBound.Lower: alpha = Math.Max(alpha, ttEntry.Score); break;
                        case TTBound.Upper: beta = Math.Min(beta, ttEntry.Score); break;
                    }
                    if (alpha >= beta) return ttEntry.Score; // Cutoff based on TT info
                }
                // Try to get the best move from TT even if depth isn't sufficient for score cutoff
                if (ttEntry.ZobristHash == boardHash && ttEntry.BestMoveHash != 0)
                {
                    ttBestMove = TTEntry.ShortHashToMove(ttEntry.BestMoveHash, board, playerColor);
                    // TODO: Validate ttBestMove is legal here? Or rely on OrderMoves validation.
                }
            }


            // --- Base Case: Depth Reached or Game Over ---
            if (depth <= 0)
            {
                // return Evaluate(board, playerColor); // Switch to Quiescence Search
                return QuiescenceSearch(board, ply, alpha, beta, playerColor);
            }

            if (board.IsGameOver()) // Check for Checkmate/Stalemate
            {
                string opponentColor = board.GetOpponentColor(playerColor);
                if (board.IsKingInCheck(playerColor))
                    return -Constants.CheckmateScore + ply; // Penalize mate score by ply (shorter mates are better)
                else
                    return Constants.DrawScore; // Stalemate
            }


            // --- Search Moves ---
            int bestScore = -Constants.InfiniteScore; // Start with lowest possible score
            Move bestMove = null;
            TTBound currentBound = TTBound.Upper; // Assume alpha cutoff initially (all moves failed to raise alpha)

            string oppColor = board.GetOpponentColor(playerColor);
            List<Move> moves = OrderMoves(board.GetPossibleMovesForColor(playerColor, true), board, playerColor, ply, ttBestMove);
            int movesSearched = 0;

            foreach (Move move in moves)
            {
                // --- Make Move ---
                // BoardCE nextBoard = _boardPool.Get(); // Get from pool if using
                // nextBoard.CopyFrom(board); // Need efficient copy or clone
                // nextBoard.MakeMove(move);
                BoardCE nextBoard = board.CloneAndMove(move); // Use existing clone method

                // --- Recursive Call (PVS style - simplified here) ---
                int score;
                // Basic NegaMax call:
                score = -AlphaBetaNegaMax(nextBoard, depth - 1, ply + 1, -beta, -alpha, oppColor);

                // --- Unmake Move / Release Board ---
                // nextBoard.UnmakeMove(move); // If using make/unmake
                // _boardPool.Release(nextBoard); // Release back to pool

                movesSearched++;

                // --- Update Best Score & Alpha ---
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move; // Store the best move found so far

                    if (score > alpha)
                    {
                        alpha = score;
                        currentBound = TTBound.Exact; // Found a move that improves alpha, potential PV node

                        // --- Beta Cutoff ---
                        if (alpha >= beta)
                        {
                            // Store Killer Move (only if it's a quiet move)
                            if (!move.IsCaptureOrPromotion(board)) // Assuming Move has this info or board can tell
                                StoreKillerMove(ply, move);

                            currentBound = TTBound.Lower; // This move caused a cutoff (fail-high)
                            bestScore = beta; // Or alpha? Stockfish often returns the bound causing cutoff. Use beta for fail-high.
                            break; // Prune remaining moves
                        }
                    }
                }
            }

            // --- Handle No Legal Moves (Checkmate/Stalemate) ---
            // This check should ideally happen before move generation, but double-check here
            if (movesSearched == 0)
            {
                if (board.IsKingInCheck(playerColor))
                    return -Constants.CheckmateScore + ply; // Checkmate
                else
                    return Constants.DrawScore; // Stalemate
            }

            // --- Store Result in Transposition Table ---
            // Avoid overwriting deeper searches with shallower ones if TT entry exists
            // Simple strategy: always replace for now, or use replacement schemes (e.g., depth-preferred)
            lock (_ttLock)
            {
                // Only write if the new entry is from a deeper search or equally deep exact score
                var existingEntry = _transpositionTable[ttIndex];
                if (existingEntry.ZobristHash != boardHash || depth >= existingEntry.Depth || currentBound == TTBound.Exact)
                {
                    _transpositionTable[ttIndex] = new TTEntry
                    {
                        ZobristHash = boardHash,
                        Score = (short)bestScore, // Use the actual best score found or the bound (beta)
                        Depth = (short)depth,
                        Bound = currentBound,
                        BestMoveHash = TTEntry.MoveToShortHash(bestMove)
                    };
                }
            }


            return bestScore; // Return the best score found for this node
        }

        // --- Quiescence Search ---
        private int QuiescenceSearch(BoardCE board, int ply, int alpha, int beta, string playerColor)
        {
            _nodeCount++;
            if ((_nodeCount & 2047) == 0 && DateTime.UtcNow - _startTime >= _maxSearchTime)
            {
                _stopSearch = true;
                return 0;
            }
            if (_stopSearch) return 0;

            // --- Stand Pat Score ---
            // The score if we don't make any more captures/promotions
            int standPatScore = Evaluate(board, playerColor);

            // --- Delta Pruning (Simple Version) ---
            // If stand-pat score is already high enough, assume no capture will make it worse
            if (standPatScore >= beta)
                return beta; // Fail-high

            // If stand-pat score is significantly worse than alpha, can we even reach alpha with a capture?
            const int BigDelta = 900; // Value of a queen (adjust as needed)
            if (standPatScore < alpha - BigDelta)
            {
                // return alpha; // Fail-low - commented out, less safe than beta cutoff
            }


            if (standPatScore > alpha)
                alpha = standPatScore; // Update alpha with the best score found so far


            // --- Generate & Order Captures/Promotions ---
            string oppColor = board.GetOpponentColor(playerColor);
            // Need a specific move gen function for only noisy moves
            List<Move> noisyMoves = OrderMoves(board.GetNoisyMovesForColor(playerColor), board, playerColor, ply, null); // No TT move needed here usually


            foreach (Move move in noisyMoves)
            {
                // --- Make Move ---
                BoardCE nextBoard = board.CloneAndMove(move);

                // --- Recursive Call ---
                int score = -QuiescenceSearch(nextBoard, ply + 1, -beta, -alpha, oppColor);

                // --- Unmake Move --- (handled by clone)

                // --- Update Alpha ---
                if (score > alpha)
                {
                    alpha = score;
                    // --- Beta Cutoff ---
                    if (alpha >= beta)
                    {
                        return beta; // Fail-high, cutoff
                    }
                }
            }

            // If no captures improved alpha, return the best score found (which could be the stand-pat score)
            return alpha;
        }


        // --- Move Ordering ---
        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color, int ply, Move ttBestMove)
        {
            if (!moves.Any()) return moves;

            var moveScores = new Dictionary<Move, int>();

            foreach (var move in moves)
            {
                int score = 0;

                // 1. TT Move
                if (move.Equals(ttBestMove)) // Requires proper Move equality comparison
                {
                    score = 100000;
                }
                else
                {
                    // 2. Captures (MVV-LVA)
                    string capturedPiece = board._cases[move.ToIndex];
                    if (capturedPiece != null)
                    {
                        int victimValue = board.GetPieceBaseValue(capturedPiece); // Base value (P=100, N=300...)
                        int attackerValue = board.GetPieceBaseValue(board._cases[move.FromIndex]);
                        score = (victimValue * 100) - attackerValue + 10000; // Prioritize captures highly, then by MVV-LVA
                    }
                    else // Quiet Moves
                    {
                        // 3. Killer Moves
                        if (ply < MaxSearchDepth)
                        { // Check bounds
                            if (move.Equals(_killerMoves[ply, 0])) score = 9000;
                            else if (move.Equals(_killerMoves[ply, 1])) score = 8500;
                        }

                        // 4. History Heuristic (Not implemented here - would need a history table)
                        // score += _historyTable[move.FromIndex, move.ToIndex];

                        // 5. Other heuristics (e.g., promotions, checks - less critical if QSearch handles them)
                        // if (move.IsPromotion) score += 9500; // Promotions are usually good
                    }
                }
                moveScores[move] = score;
            }

            // Sort moves descending by score
            // Using LINQ OrderByDescending is simple but might be slow for large move lists
            // A custom sort or bucket sort could be faster
            return moves.OrderByDescending(m => moveScores[m]).ToList();
        }

        // --- Killer Move Storage ---
        private void StoreKillerMove(int ply, Move move)
        {
            if (ply >= MaxSearchDepth) return; // Bounds check

            // Avoid storing the same move twice
            if (!move.Equals(_killerMoves[ply, 0]))
            {
                // Shift existing killer
                _killerMoves[ply, 1] = _killerMoves[ply, 0];
                // Store new killer
                _killerMoves[ply, 0] = move;
            }
        }


        // --- Evaluation Wrapper ---
        private int Evaluate(BoardCE board, string playerColor)
        {
            // Evaluation should be relative to the current player
            string opponentColor = board.GetOpponentColor(playerColor);
            int score = board.CalculateBoardCEScore(playerColor, opponentColor);

            // Simple NegaMax requires score relative to side-to-move.
            // If CalculateBoardCEScore provides absolute score (e.g. White's perspective), adjust it.
            // Assuming CalculateBoardCEScore IS relative to playerColor:
            return score;

            // If CalculateBoardCEScore is always White's score:
            // return (playerColor == "W") ? score : -score;
        }

        // --- Helper Methods ---
        private Move GetBestMoveFromTT(BoardCE board)
        {
            long boardHash = board.GetZobristHash();
            int ttIndex = (int)((ulong)boardHash % (ulong)TranspositionTableSize);
            Move bestMove = null;
            lock (_ttLock)
            { // Lock needed for consistency
                var ttEntry = _transpositionTable[ttIndex];
                if (ttEntry.ZobristHash == boardHash && ttEntry.BestMoveHash != 0)
                {
                    bestMove = TTEntry.ShortHashToMove(ttEntry.BestMoveHash, board, _engineColor); // Use engine color at root
                                                                                                   // TODO: Validate move is legal here?
                    if (bestMove != null && !board.IsMoveLegal(bestMove, _engineColor))
                    {
                        // Log or handle invalid TT move
                        // Utils.WritelineAsync($"Warning: Invalid TT move {bestMove} for hash {boardHash}");
                        bestMove = null;
                    }
                }
            }
            return bestMove;
        }

        private bool IsCheckmateScore(int score)
        {
            return Math.Abs(score) > Constants.CheckmateScore - MaxSearchDepth; // Check if score is near checkmate value
        }

        private string FormatScore(int score)
        {
            if (IsCheckmateScore(score))
            {
                int sign = Math.Sign(score);
                // Calculate moves to mate from the score
                int movesToMate = (Constants.CheckmateScore - Math.Abs(score) + 1) / 2; // +1 because ply starts at 0
                return $"Mate in {movesToMate * sign}"; // Negative if engine is getting mated
            }
            return score.ToString(); // Centipawn score
        }

        // --- Constants --- (Define appropriately)
        private static class Constants
        {
            public const int InfiniteScore = 30000;
            public const int CheckmateScore = 29000; // Must be less than infinite, allow for ply differences
            public const int DrawScore = 0;
        }

        // Assumed methods in BoardCE (Needs implementation details):
        // - long GetZobristHash()
        // - int GetPieceBaseValue(string piece) -> e.g., Pawn=100, Knight=300... (ignores position)
        // - List<Move> GetNoisyMovesForColor(string color) -> Returns only captures/promotions
        // - bool IsMoveLegal(Move move, string color)
        // - bool IsCaptureOrPromotion(Move move) -> Add this helper to Move or BoardCE
        // - bool IsGameOver() -> Checks mate/stalemate
        // - bool IsKingInCheck(string color)
        // - int CalculateBoardCEScore(string myColor, string opponentColor) -> MUST BE FAST!
        // - BoardCE CloneAndMove(Move move) -> Efficiently clones and makes move
        // - (Optional) MakeMove(Move move) / UnmakeMove(Move move) -> For performance if cloning is slow
        // - (Optional) GetPieceValue(string piece) -> Used in old code, might differ from GetPieceBaseValue

        // Assumed methods/properties in Move:
        // - int FromIndex
        // - int ToIndex
        // - string Piece  (optional, can get from board)
        // - bool Equals(object obj) -> Correctly compare moves
        // - int GetHashCode() -> Consistent with Equals
        // - bool IsCaptureOrPromotion(BoardCE board) -> Helper method needed

    }

    // --- Minimal Object Pool (Example - keep if needed) ---
    public class ObjectPool<T> where T : new()
    {
        private readonly ConcurrentBag<T> _objects = new ConcurrentBag<T>();
        private readonly Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        }

       // public T Get() => _objects.TryPop(out T item) ? item : _objectGenerator();

        public void Release(T item) => _objects.Add(item);
    }

    // --- Utils Placeholder ---
    public static class Utils
    {
        public static void WritelineAsync(string message) => Console.WriteLine(message); // Replace with async logging if needed
        public static string ExtractUppercaseLettersAndDigits(string s) => new string(s.Where(c => char.IsUpper(c) || char.IsDigit(c)).ToArray());
    }

    // --- Move Placeholder --- (Ensure this matches your actual Move class/struct)
    public class Move // Or struct? Structs are better for perf if small and passed often
    {
        public int FromIndex { get; }
        public int ToIndex { get; }
        public string Piece { get; } // Piece being moved (e.g., "P|W")
        // Add promotion piece info if applicable

        public Move(int from, int to, string piece)
        {
            FromIndex = from;
            ToIndex = to;
            Piece = piece;
        }

        // Implement Equals and GetHashCode for Dictionary keys and Killer Move comparison
        public override bool Equals(object obj)
        {
            return obj is Move move &&
                   FromIndex == move.FromIndex &&
                   ToIndex == move.ToIndex; // Potentially add PromotionPiece equality if needed
        }

        public override int GetHashCode()
        {
            // Simple hash combining FromIndex and ToIndex
            return HashCode.Combine(FromIndex, ToIndex); // Use System.HashCode for better distribution
                                                         // Or return (FromIndex << 8) | ToIndex; if indices are < 256
        }

        public bool IsCaptureOrPromotion(BoardCE board)
        {
            // Promotion check (e.g., pawn reaching last rank)
            // Simplified check - needs proper logic based on your board representation
            char pieceType = Piece[0];
            if (pieceType == 'P')
            {
                int targetRank = ToIndex / 8;
                if ((Piece.EndsWith("W") && targetRank == 7) || (Piece.EndsWith("B") && targetRank == 0))
                {
                    return true; // Is promotion
                }
            }
            // Capture check
            return board._cases[ToIndex] != null;
        }


        public override string ToString()
        {
            // Convert FromIndex/ToIndex to algebraic notation (e.g., "e2e4")
            return $"{IndexToAlgebraic(FromIndex)}{IndexToAlgebraic(ToIndex)}";
        }

        private static string IndexToAlgebraic(int index)
        {
            if (index < 0 || index > 63) return "??";
            int file = index % 8;
            int rank = index / 8;
            return $"{(char)('a' + file)}{rank + 1}";
        }
    }

    // --- NodeCE Placeholder --- (Ensure this matches your actual NodeCE class)
    public class NodeCE
    {
        public BoardCE Board { get; } // Consider not storing the full board if memory is tight
        public Move Move { get; }
        public int Weight { get; set; }
        public int Depth { get; }
        public TimeSpan Elapsed { get; }
        public TimeSpan ReflectionTime { get; set; }
        public List<NodeCE> EquivalentBestNodeCEList { get; set; } = new List<NodeCE>();
        public List<NodeCE> AllNodeCEList { get; set; } = new List<NodeCE>(); // May not be populated by IDDFS

        public int FromIndex => Move?.FromIndex ?? -1;
        public int ToIndex => Move?.ToIndex ?? -1;
        public string Location => ChessCore.Tools.Utils.GetPositionFromIndex(Move.FromIndex); // Position d'origine en notation échiquier
        public string BestChildPosition => ChessCore.Tools.Utils.GetPositionFromIndex(Move.ToIndex); // Position de destination en notation échiquier



        public NodeCE(BoardCE board, Move move, int weight, int depth, TimeSpan elapsed)
        {
            Board = board; // This might store the board *after* the move
            Move = move;
            Weight = weight;
            Depth = depth;
            Elapsed = elapsed;
        }
        public override string ToString()
        {
            return $"Move: {Move}, Score: {Weight}, Depth: {Depth}, Elapsed: {Elapsed.TotalMilliseconds:F0}ms";
        }
    }

    // --- BoardCE Placeholder --- (Key methods needed by the engine)
    public class BoardCE
    {
        public string[] _cases = new string[64]; // Example representation

        public BoardCE()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
        }
        public void Print()
        {
            Utils.WritelineAsync("_____________________________________________________________________");
            for (int y = 0; y < 8; y++)
            {
                var line = "";
                for (int x = 0; x < 8; x++)
                {
                    var index = x + y * 8;
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Utils.WritelineAsync(line);
            }
            Utils.WritelineAsync("_____________________________________________________________________");
        }


        public void InsertPawn(int index, string pieceType, string color)
        {
            _cases[index] = $"{pieceType}|{color}";
        }
        public string ConvertToFEN()
        {
            // Tableau de correspondance des pièces
            string[] boardRows = this.ToString().Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();

            // Tableau 8x8 pour représenter le plateau
            string[,] chessboard = new string[8, 8];

            // Remplir le tableau avec les pièces ou les cases vides
            for (int i = 0; i < boardRows.Length; i++)
            {
                int row = i / 8;
                int col = i % 8;

                if (boardRows[i] == "__")
                {
                    chessboard[row, col] = ""; // Case vide
                }
                else
                {
                    string[] parts = boardRows[i].Split('|');
                    string piece = parts[0];
                    string color = parts[1];

                    string fenPiece = piece switch
                    {
                        "T" => "R", // Tour
                        "C" => "N", // Cavalier
                        "B" => "B", // Fou
                        "Q" => "Q", // Dame
                        "K" => "K", // Roi
                        "P" => "P", // Pion
                        _ => "?"    // Inconnu (ne devrait pas arriver)
                    };

                    if (color == "B") fenPiece = fenPiece.ToLower(); // Noirs en minuscules
                    chessboard[row, col] = fenPiece;
                }
            }

            // Construire la notation FEN pour les pièces
            string fen = "";
            for (int row = 0; row < 8; row++)
            {
                int emptyCount = 0;
                for (int col = 0; col < 8; col++)
                {
                    string val = chessboard[row, col];

                    if (string.IsNullOrEmpty(val))
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen += emptyCount.ToString();
                            emptyCount = 0;
                        }
                        fen += val;
                    }
                }
                if (emptyCount > 0) fen += emptyCount.ToString();
                if (row < 7) fen += "/";
            }

            // Déterminer les droits de roque
            string castling = "";
            if (chessboard[0, 0] == "R") castling += "Q"; // Roque côté dame pour les blancs
            if (chessboard[0, 7] == "R") castling += "K"; // Roque côté roi pour les blancs
            if (chessboard[7, 0] == "r") castling += "q"; // Roque côté dame pour les noirs
            if (chessboard[7, 7] == "r") castling += "k"; // Roque côté roi pour les noirs
            if (string.IsNullOrEmpty(castling)) castling = "-";

            // Déterminer la prise en passant (à adapter selon votre logique)
            string enPassant = "-"; // Par défaut, pas de prise en passant

            // Déterminer le nombre de demi-coups depuis la dernière prise ou poussée de pion
            int halfmoveClock = 0; // À adapter selon votre logique

            // Déterminer le numéro du coup complet
            int fullmoveNumber = 1; // À adapter selon votre logique

            // Retourner le FEN complet
            return $"{fen} w {castling} {enPassant} {halfmoveClock} {fullmoveNumber}";
        }



        // MUST IMPLEMENT:
        public virtual long GetZobristHash()
        {
            // Implement proper Zobrist hashing here!
            // Example placeholder (BAD HASH):
            long hash = 0;
            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i] != null)
                {
                    hash ^= (long)_cases[i].GetHashCode() << (i % 16);
                }
            }
            // Include castling rights, en passant target, side to move in hash
            return hash;
        }
        public virtual List<Move> GetPossibleMovesForColor(string color, bool includeQuietMoves = true) { /* ... */ return new List<Move>(); }
        public virtual List<Move> GetNoisyMovesForColor(string color)
        {
            // Return only captures and promotions
            return GetPossibleMovesForColor(color, true).Where(m => m.IsCaptureOrPromotion(this)).ToList();
        }
        public virtual BoardCE CloneAndMove(Move move) { /* ... */ return new BoardCE(); }
        public virtual BoardCE CloneAndMove(int fromIndex, int toIndex)
        { // Keep overload if used elsewhere
            var piece = _cases[fromIndex];
            if (piece == null) throw new ArgumentException("No piece at source index");
            return CloneAndMove(new Move(fromIndex, toIndex, piece));
        }
        public virtual int CalculateBoardCEScore(string myColor, string opponentColor) { /* ... VERY IMPORTANT! */ return 0; }
        public virtual bool IsGameOver() { /* ... */ return false; }
        public virtual bool IsKingInCheck(string color) { /* ... */ return false; }
        public virtual string GetOpponentColor(string color) => color == "W" ? "B" : "W";
        public virtual int GetPieceValue(string piece) { /* ... Value including position? */ return GetPieceBaseValue(piece); } // Old method?
        public virtual int GetPieceBaseValue(string piece)
        {
            if (piece == null) return 0;
            switch (piece[0])
            { // Assumes "P|W", "N|B" format
                case 'P': return 100;
                case 'N': return 300;
                case 'B': return 320; // Slight bonus for bishops often used
                case 'R': return 500;
                case 'Q': return 900;
                case 'K': return 20000; // King value for eval purposes (not checkmate)
                default: return 0;
            }
        }
        public virtual bool IsMoveLegal(Move move, string color)
        {
            // Basic check: piece exists and belongs to player, destination is valid
            if (move == null || move.FromIndex < 0 || move.FromIndex > 63 || move.ToIndex < 0 || move.ToIndex > 63) return false;
            var piece = _cases[move.FromIndex];
            if (piece == null || !piece.EndsWith($"|{color}")) return false;
            // IMPORTANT: This needs full legality check (doesn't leave king in check, etc.)
            // For now, we rely on GetPossibleMovesForColor generating only legal moves.
            // If TT can have stale moves, this check becomes crucial.
            return GetPossibleMovesForColor(color, true).Any(legalMove => legalMove.Equals(move));
        }

        public virtual bool TargetIndexIsMenaced(int targetIndex, string attackingColor) { /* ... Check if targetIndex is attacked by attackingColor */ return false; } // From old code

    }
}
