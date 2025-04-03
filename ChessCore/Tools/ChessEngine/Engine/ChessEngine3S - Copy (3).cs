using ChessCore.Tools.ChessEngine.Engine.Interfaces;

namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEngine3S : IChessEngine
    {
        private enum NodeType { Exact, LowerBound, UpperBound }
        private const int TranspositionTableSize = 1 << 20;

        private struct TranspositionEntry
        {
            public ulong Hash;
            public int Depth;
            public int Value;
            public NodeType Type;
            public Move BestMove;
        }

        private readonly TranspositionEntry[] _transpositionTable = new TranspositionEntry[TranspositionTableSize];
        private readonly ulong[,,] _zobristTable = new ulong[8, 8, 16];
        private readonly int[,] _historyHeuristic = new int[64, 64];
        private Move[] _killerMoves = new Move[2];

        private int _searchDepth;
        private DateTime _startTime;
        private int _maxSearchTimeMs = 5000;
        private bool _timeExpired;
        private int _nodesVisited;

        public ChessEngine3S()
        {
            InitializeZobrist();
        }
        public void Dispose() { }
        public string GetName()
        {
            return this.GetType().Name;
        }
        public string GetShortName()
        {
            return Utils.ExtractUppercaseLettersAndDigits(GetName());
        }

        public NodeCE GetBestModeCE(string color, BoardCE board, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {
            _maxSearchTimeMs = maxReflectionTimeInMinute * 60 * 1000;
            _startTime = DateTime.UtcNow;
            _timeExpired = false;
            _nodesVisited = 0;



         
            var cpuColor = color.First().ToString();
            
            string opponentColor = board.GetOpponentColor(cpuColor);

            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"MAX_SEARCH_TIME_S :  {_maxSearchTimeMs * 1000}");
            Utils.WritelineAsync($"cpuColor :  {cpuColor}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");


            NodeCE bestNode = null;
            // Recherche itérative avec gestion du temps
            for (var depth = 1; depth <= depthLevel; depth++)
            {
                if (_timeExpired) break;

                var result = SearchRoot(board, depth, cpuColor);
                if (!_timeExpired) bestNode = result;
            }
            return bestNode;
        }

        private NodeCE SearchRoot(BoardCE board, int depth, string color)
        {
            var allNode = new List<NodeCE>();
            var moves = OrderMoves(board.GetPossibleMovesForColor(color, true), board, color);
            var bestValue = int.MinValue;
            NodeCE bestNode = null;

            foreach (var move in moves)
            {
                if (_timeExpired) break;

                var clonedBoard = board.CloneAndMove(move);
                var value = -NegaMax(clonedBoard, depth - 1, int.MinValue + 1, int.MaxValue - 1, board.GetOpponentColor(color));

                if (value > bestValue)
                {
                    bestValue = value;
                    bestNode = new NodeCE(clonedBoard, move, value, depth, DateTime.UtcNow - _startTime);
                    Utils.WritelineAsync($"{bestNode}");
                    allNode.Add(bestNode);
                    
                }

                // Mise à jour de l'historique
                _historyHeuristic[move.FromIndex, move.ToIndex] += depth * depth;
            }
            bestNode.AllNodeCEList = allNode;
            var equivalentBestNodeCEList = allNode.Where(x => x.Weight == bestValue).ToList();

            if(equivalentBestNodeCEList.Count>1)
            {
                Utils.WritelineAsync($"bestNodeCEList :");
                foreach (var node in equivalentBestNodeCEList)
                {
                    Utils.WritelineAsync($"{node}");
                }
                return equivalentBestNodeCEList[(new Random()).Next(equivalentBestNodeCEList.Count)];

            }
            return bestNode;
        }

        private int NegaMax(BoardCE board, int depth, int alpha, int beta, string color)
        {
            _nodesVisited++;
            if (CheckTimeLimit() || depth <= 0)
                return QuiescenceSearch(board, alpha, beta, color);

            var originalAlpha = alpha;
            var hash = ComputeZobristHash(board);

            // Vérification de la table de transposition
            ref var entry = ref _transpositionTable[hash % TranspositionTableSize];
            if (entry.Hash == hash && entry.Depth >= depth)
            {
                if (entry.Type == NodeType.Exact)
                    return entry.Value;
                if (entry.Type == NodeType.LowerBound)
                    alpha = Math.Max(alpha, entry.Value);
                else if (entry.Type == NodeType.UpperBound)
                    beta = Math.Min(beta, entry.Value);

                if (alpha >= beta)
                    return entry.Value;
            }

            var moves = OrderMoves(board.GetPossibleMovesForColor(color), board, color);
            if (moves.Count == 0)
                return board.IsKingInCheck(color) ? -99999 + _searchDepth : 0;

            var bestValue = int.MinValue;
            var bestMove = new Move();
            var isCheck = board.IsKingInCheck(color);

            // Null Move Pruning
            if (!isCheck && depth >= 3)
            {
                var nullBoard = board.Clone();
               // nullBoard.SwitchPlayer();
               var oppColor = nullBoard.GetOpponentColor(color);
                var nullValue = -NegaMax(nullBoard, depth - 1 - 2, -beta, -beta + 1, oppColor);
                if (nullValue >= beta)
                    return beta;
            }

            for (int i = 0; i < moves.Count; i++)
            {
                var move = moves[i];
                var clonedBoard = board.CloneAndMove(move);
                int value;

                // Late Move Reduction
                if (i > 3 && depth < 6 && !isCheck && !move.IsCapture)
                {
                    value = -NegaMax(clonedBoard, depth - 2, -alpha - 1, -alpha, clonedBoard.GetOpponentColor(color));
                    if (value > alpha)
                        value = -NegaMax(clonedBoard, depth - 1, -beta, -alpha, clonedBoard.GetOpponentColor(color));
                }
                else
                {
                    value = -NegaMax(clonedBoard, depth - 1, -beta, -alpha, clonedBoard.GetOpponentColor(color));
                }

                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = move;
                    if (value > alpha)
                    {
                        alpha = value;
                        if (alpha >= beta)
                        {
                            // Mise à jour des killer moves
                            if (!move.IsCapture)
                            {
                                _killerMoves[1] = _killerMoves[0];
                                _killerMoves[0] = move;
                            }
                            break;
                        }
                    }
                }
            }

            // Mise à jour de la table de transposition
            entry = new TranspositionEntry
            {
                Hash = hash,
                Depth = depth,
                Value = bestValue,
                Type = bestValue <= originalAlpha ? NodeType.UpperBound :
                       bestValue >= beta ? NodeType.LowerBound : NodeType.Exact,
                BestMove = bestMove
            };

            return bestValue;
        }

        private int QuiescenceSearch(BoardCE board, int alpha, int beta, string color)
        {
            var standPat = Evaluate(board, color);
            if (standPat >= beta)
                return beta;
            if (alpha < standPat)
                alpha = standPat;

            var moves = board.GetPossibleMovesForColor(color, true)
                .Where(m => m.IsCapture)
                .OrderByDescending(m => m.CapturedPieceValue)
                .ToList();

            foreach (var move in moves)
            {
                var clonedBoard = board.CloneAndMove(move);
                var value = -QuiescenceSearch(clonedBoard, -beta, -alpha, clonedBoard.GetOpponentColor(color));

                if (value >= beta)
                    return beta;
                if (value > alpha)
                    alpha = value;
            }

            return alpha;
        }

        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            return moves.OrderByDescending(m =>
            {
                int score = 0;

                // Meilleur coup de la table de transposition
                if (m.Equals(_transpositionTable[ComputeZobristHash(board) % TranspositionTableSize].BestMove))
                    score += 10000;

                // Captures
                if (m.IsCapture)
                    score += m.CapturedPieceValue * 10 - m.PieceValue;

                // Killer moves
                if (_killerMoves.Contains(m))
                    score += 9000;

                // Histoire des coups
                score += _historyHeuristic[m.FromIndex, m.ToIndex];

                return score;
            }).ToList();
        }

        private int Evaluate(BoardCE board, string color)
        {
            int score = 0;
            // Implémentez ici une évaluation plus sophistiquée avec :
            // - Tables de position par pièce
            // - Mobilité
            // - Structure de pions
            // - Sécurité du roi
            return board.CalculateBoardCEScore(color, board.GetOpponentColor(color));
        }

        private void InitializeZobrist()
        {
            var rng = new Random();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    for (int k = 0; k < 16; k++)
                        _zobristTable[i, j, k] = (ulong)rng.NextInt64();
        }

        private ulong ComputeZobristHash(BoardCE board)
        {
            ulong hash = 0;
            for (int i = 0; i < 64; i++)
            {
                var piece = board._cases[i];
                if (piece != "__")
                {
                    var parts = piece.Split('|');
                    hash ^= _zobristTable[i % 8, i / 8, GetPieceCode(parts[0], parts[1])];
                }
            }
            return hash;
        }

        private int GetPieceCode(string piece, string color)
        {
            return piece[0] switch
            {
                'P' => 0 + (color == "W" ? 0 : 6),
                'N' => 1 + (color == "W" ? 0 : 6),
                'B' => 2 + (color == "W" ? 0 : 6),
                'R' => 3 + (color == "W" ? 0 : 6),
                'Q' => 4 + (color == "W" ? 0 : 6),
                'K' => 5 + (color == "W" ? 0 : 6),
                _ => 0
            };
        }

        private bool CheckTimeLimit()
        {
            if ((DateTime.UtcNow - _startTime).TotalMilliseconds > _maxSearchTimeMs)
            {
                _timeExpired = true;
                Utils.WritelineAsync($"REFLECTION TIME EXPIRED");
                return true;
            }
            return false;
        }
    }
}