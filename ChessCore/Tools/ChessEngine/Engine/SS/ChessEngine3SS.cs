using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Emit;

namespace ChessCore.Tools.ChessEngine.Engine.SS
{
    // --- Classe d'ObjectPool (implémentation simplifiée) ---
    public class ObjectPool<T> where T : new()
    {
        private ConcurrentBag<T> _objects = new ConcurrentBag<T>();
        private Func<T> _objectGenerator;
        public ObjectPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator;
        }
        public T GetObject()
        {
            if (_objects.TryTake(out T item))
                return item;
            return _objectGenerator();
        }
        public void PutObject(T item)
        {
            _objects.Add(item);
        }
    }

    // --- Classe représentant un coup ---
    public class Move
    {
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Move other)
                return FromIndex == other.FromIndex && ToIndex == other.ToIndex;
            return false;
        }
        public override int GetHashCode() => (FromIndex, ToIndex).GetHashCode();
    }

    // --- Classe représentant un nœud de la recherche ---
    public class NodeCE
    {
        public BoardCE Board;
        public List<NodeCE> EquivalentBestNodeCEList { get; set; }
        public List<NodeCE> AllNodeCEList { get; set; } = new List<NodeCE>();
        public Move Move;
        public int Weight;
        public int Depth;
        public TimeSpan ReflectionTime;
        public string Location => Utils.GetPositionFromIndex(Move.FromIndex); // Position d'origine en notation échiquier
        public string BestChildPosition => Utils.GetPositionFromIndex(Move.ToIndex); // Position de destination en notation échiquier

        public override string ToString()
        {
            return $"{Depth}:   {Location} ({Move.FromIndex}) => {BestChildPosition} ({Move.ToIndex}) : {Weight} ({ReflectionTime.ToString(@"hh\:mm\:ss")})".ToUpper();
        }
        public NodeCE(BoardCE board, Move move, int weight, int depth, TimeSpan reflectionTime)
        {
            Board = board;
            Move = move;
            Weight = weight;
            Depth = depth;
            ReflectionTime = reflectionTime;
        }
    }

    // --- Implémentation simplifiée et sûre du plateau d'échecs ---
    public class BoardCE
    {
        // _cases représente le plateau (64 cases) ; on utilise ici des chaînes
        public string[] _cases;



        public BoardCE()
        {
            _cases = new string[64];
            // Par exemple, initialiser avec une position vide ou la position de départ
        }


        public BoardCE(string[] cases)
        {
            _cases = cases;
        }



        // Clone crée une copie superficielle sûre du plateau
        public BoardCE Clone()
        {
            var clone = new BoardCE();
            clone._cases = (string[])this._cases.Clone();
            return clone;
        }

        // CloneAndMove crée un clone et effectue le coup
        public BoardCE CloneAndMove(Move move)
        {
            if (move.FromIndex < 0 || move.FromIndex >= _cases.Length ||
                move.ToIndex < 0 || move.ToIndex >= _cases.Length)
                throw new ArgumentOutOfRangeException("Indices invalides pour le mouvement.");

            var clone = Clone();
            clone._cases[move.ToIndex] = clone._cases[move.FromIndex];
            clone._cases[move.FromIndex] = null;
            return clone;
        }

        public string ToString()
        {
            var result = "";
            foreach (var caseContaint in _cases.ToList())
            {
                result += caseContaint + ";";

            }
            return result;
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


        // Renvoie la couleur opposée (suppose "W" ou "B")
        public string GetOpponentColor(string color) => color == "W" ? "B" : "W";

        // Génère une liste de coups possibles pour la couleur donnée (implémentation simplifiée)
        public List<Move> GetPossibleMovesForColorOLD(string color, bool includeSpecial = false)
        {
            var moves = new List<Move>();
            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i] != null && _cases[i].Contains("|" + color))
                {
                    // Pour simplifier, générer des coups vers toutes les autres cases
                    for (int j = 0; j < _cases.Length; j++)
                    {
                        if (i != j)
                            moves.Add(new Move { FromIndex = i, ToIndex = j });
                    }
                }
            }
            return moves;
        }

        public List<Move> GetPossibleMovesForColor(string color, bool includeSpecial = false)
        {
            var moves = new List<Move>();

            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i] != null && _cases[i].EndsWith("|" + color))
                {
                    string pieceType = _cases[i].Split('|')[0];
                    int row = i / 8;
                    int col = i % 8;

                    switch (pieceType)
                    {
                        case "P":
                            // Pour un pion : 
                            // - Si blanc, déplacement vers le haut (row - 1) si la case est vide.
                            // - Si noir, déplacement vers le bas (row + 1).
                            if (color == "W")
                            {
                                int newRow = row - 1;
                                if (newRow >= 0)
                                {
                                    int newIndex = newRow * 8 + col;
                                    if (_cases[newIndex] == null)
                                        moves.Add(new Move { FromIndex = i, ToIndex = newIndex });
                                }
                            }
                            else // noir
                            {
                                int newRow = row + 1;
                                if (newRow < 8)
                                {
                                    int newIndex = newRow * 8 + col;
                                    if (_cases[newIndex] == null)
                                        moves.Add(new Move { FromIndex = i, ToIndex = newIndex });
                                }
                            }
                            break;

                        case "R":
                            // Pour une tour : explorer dans les 4 directions (verticales et horizontales)
                            int[] dRow = { -1, 1, 0, 0 };
                            int[] dCol = { 0, 0, -1, 1 };
                            for (int d = 0; d < 4; d++)
                            {
                                int newRow = row;
                                int newCol = col;
                                while (true)
                                {
                                    newRow += dRow[d];
                                    newCol += dCol[d];
                                    if (newRow < 0 || newRow >= 8 || newCol < 0 || newCol >= 8)
                                        break;
                                    int newIndex = newRow * 8 + newCol;
                                    moves.Add(new Move { FromIndex = i, ToIndex = newIndex });
                                    // Si une pièce est rencontrée, on ne peut pas aller plus loin dans cette direction.
                                    if (_cases[newIndex] != null)
                                        break;
                                }
                            }
                            break;

                        case "C":
                            // Pour un cavalier : 8 mouvements possibles
                            int[] knightMovesRow = { -2, -1, 1, 2, 2, 1, -1, -2 };
                            int[] knightMovesCol = { 1, 2, 2, 1, -1, -2, -2, -1 };
                            for (int m = 0; m < 8; m++)
                            {
                                int newRow = row + knightMovesRow[m];
                                int newCol = col + knightMovesCol[m];
                                if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
                                {
                                    int newIndex = newRow * 8 + newCol;
                                    moves.Add(new Move { FromIndex = i, ToIndex = newIndex });
                                }
                            }
                            break;

                        // Vous pouvez ajouter ici les cas pour "B" (Fou), "Q" (Dame) et "K" (Roi)
                        default:
                            break;
                    }
                }
            }
            return moves;
        }


        // Fonction d'évaluation simplifiée : compte les pièces de chaque camp
        public int CalculateBoardCEScore(string cpuColor, string opponentColor)
        {
            int score = 0;
            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i] != null)
                {
                    if (_cases[i].Contains("|" + cpuColor))
                        score += 1;
                    else if (_cases[i].Contains("|" + opponentColor))
                        score -= 1;
                }
            }
            return score;
        }

        // Méthodes simplifiées pour la vérification d'échec et fin de partie
        public bool IsKingInCheck(string color) => false;
        public bool IsGameOver() => false;

        // Méthode fictive pour vérifier si une case est menacée (toujours false ici)
        public bool TargetIndexIsMenaced(int index, string color) => false;

        // Retourne une valeur (dummy) pour une pièce
        public int GetPieceValue(string piece) => 1;
    }

    // --- Interface du moteur d'échecs ---
    //public interface IChessEngine : IDisposable
    //{
    //    string GetName();
    //    string GetShortName();
    //    NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2);
    //}

    // --- Classe utilitaire ---
    public static class Utils
    {
        public static string GetPositionFromIndex(int index)
        {
            if (index < 0 || index > 63)
                return null;

            char file = (char)('a' + index % 8); // 'a' à 'h'
            int rank = 8 - index / 8; // '8' à '1'

            return $"{file}{rank}";
        }

        public static void WritelineAsync(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public static string ExtractUppercaseLettersAndDigits(string input)
        {
            return new string(input.Where(char.IsUpper).ToArray());
        }
    }

    // --- Moteur d'échecs inspiré de Stockfish (version corrigée) ---
    public class ChessEngine3SS : IChessEngine
    {
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition qui stocke (score, profondeur, type d'entrée)
        private readonly ConcurrentDictionary<long, TranspositionEntry> _transpositionTable = new ConcurrentDictionary<long, TranspositionEntry>();
        private struct TranspositionEntry
        {
            public int Score;
            public int Depth;
            public EntryType Flag;
        }
        private enum EntryType { Exact, LowerBound, UpperBound }

        private int _maxSearchTimeSeconds = 300; // 5 minutes par défaut
        private DateTime _startTime;
        private bool _timeLimitReached = false;
        private int _maxDepth;

        // Killer moves : pour chaque profondeur, stocke les coups ayant entraîné une coupure beta
        private readonly Dictionary<int, List<Move>> _killerMoves = new Dictionary<int, List<Move>>();
        private readonly object _killerLock = new object();

        public void Dispose() { }

        public string GetName() => GetType().Name;
        public string GetShortName() => Utils.ExtractUppercaseLettersAndDigits(GetName());

        public NodeCE GetBestModeCECustum(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
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
            NodeCE bestNode = null;
            List<NodeCE> allNodes = new List<NodeCE>();
            int previousScore = 0;
            int aspirationWindow = 50;
            for (int depth = 1; depth <= _maxDepth; depth++)
            {
                if (_timeLimitReached)
                    break;

                int alpha = previousScore - aspirationWindow;
                int beta = previousScore + aspirationWindow;
                int score = 0;
                NodeCE currentBestNode = null;
                var moves = board.GetPossibleMovesForColor(cpuColor, true);
                moves = OrderMoves(moves, board, cpuColor);
                foreach (var move in moves)
                {
                    if (_timeLimitReached)
                        break;

                    var clonedBoard = SafeCloneAndMove(board, move);
                    if (clonedBoard == null)
                        continue;

                    score = -AlphaBeta(clonedBoard, depth - 1, -beta, -alpha, false, cpuColor);
                    if (_timeLimitReached)
                        break;

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
                if (!_timeLimitReached && currentBestNode != null)
                {
                    bestNode = currentBestNode;
                    previousScore = alpha;
                    Utils.WritelineAsync($"Depth {depth} complétée, meilleur score: {previousScore}");
                    Utils.WritelineAsync($"{currentBestNode}");
                    allNodes.Add(currentBestNode);
                }
            }
            if (bestNode != null)
            {
                bestNode.ReflectionTime = DateTime.UtcNow - _startTime;
                bestNode.AllNodeCEList = allNodes;
            }
             
            Utils.WritelineAsync($"bestNode : {bestNode}");
            return bestNode;
        }

        private NodeCE IterativeDeepeningSearchParallelSlow(BoardCE board, string cpuColor)
        {
            NodeCE bestNode = null;
            int previousScore = 0;
            int aspirationWindow = 50;
            List<NodeCE> allNodes = new List<NodeCE>();

            // Pour chaque profondeur d'itération jusqu'à _maxDepth
            for (int depth = 1; depth <= _maxDepth; depth++)
            {
                if (_timeLimitReached)
                    break;

                int alphaBase = previousScore - aspirationWindow;
                int betaBase = previousScore + aspirationWindow;
                // Une collection thread-safe pour accumuler les nœuds calculés à cette profondeur
                ConcurrentBag<NodeCE> currentDepthNodes = new ConcurrentBag<NodeCE>();

                var moves = board.GetPossibleMovesForColor(cpuColor, true);
                moves = OrderMoves(moves, board, cpuColor);

                // Paralléliser l'évaluation de chaque coup à la racine
                Parallel.ForEach(moves, move =>
                {
                    if (_timeLimitReached)
                        return;
                    var clonedBoard = SafeCloneAndMove(board, move);
                    if (clonedBoard == null)
                        return;

                    int score = -AlphaBeta(clonedBoard, depth - 1, -betaBase, -alphaBase, false, cpuColor);
                    if (_timeLimitReached)
                        return;

                    // Si le score est en dehors de la fenêtre d'aspiration, refaire la recherche avec des bornes larges
                    if (score <= alphaBase || score >= betaBase)
                    {
                        score = -AlphaBeta(clonedBoard, depth - 1, int.MinValue, int.MaxValue, false, cpuColor);
                    }
                    // Créer un nœud pour ce coup
                    var node = new NodeCE(clonedBoard, move, score, depth, DateTime.UtcNow - _startTime);
                    currentDepthNodes.Add(node);
                });

                // Sélectionner le meilleur nœud parmi ceux calculés pour cette profondeur
                NodeCE currentBest = currentDepthNodes.OrderByDescending(n => n.Weight).FirstOrDefault();
                if (currentBest != null)
                {
                    bestNode = currentBest;
                    previousScore = currentBest.Weight;
                    allNodes.Add(currentBest);
                    Utils.WritelineAsync($"Depth {depth} complétée, meilleur score: {previousScore}");
                    Utils.WritelineAsync($"{currentBest}");
                }
            }

            if (bestNode != null)
            {
                bestNode.ReflectionTime = DateTime.UtcNow - _startTime;
                bestNode.AllNodeCEList = allNodes;
            }
            Utils.WritelineAsync($"bestNode : {bestNode}");
            return bestNode;
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

            if (depth >= 3 && !board.IsKingInCheck(cpuColor))
            {
                var nullMoveBoard = SafeClone(board);
                if (nullMoveBoard != null)
                {
                    int R = 2;
                    int nullScore = -AlphaBeta(nullMoveBoard, depth - 1 - R, -beta, -beta + 1, false, cpuColor);
                    if (nullScore >= beta)
                        return nullScore;
                }
            }

            int bestValue = int.MinValue;
            var moves = board.GetPossibleMovesForColor(maximizingPlayer ? cpuColor : opponentColor);
            moves = OrderMoves(moves, board, maximizingPlayer ? cpuColor : opponentColor);
            foreach (var move in moves)
            {
                var clonedBoard = SafeCloneAndMove(board, move);
                if (clonedBoard == null)
                    continue;
                int value = -AlphaBeta(clonedBoard, depth - 1, -beta, -alpha, !maximizingPlayer, cpuColor);
                if (value > bestValue)
                {
                    bestValue = value;
                    if (!IsCaptureMove(move, board))
                    {
                        lock (_killerLock)
                        {
                            if (!_killerMoves.ContainsKey(depth))
                                _killerMoves[depth] = new List<Move>();
                            _killerMoves[depth].Add(move);
                        }
                    }
                }
                alpha = Math.Max(alpha, bestValue);
                if (alpha >= beta)
                    break;
            }

            EntryType flag = EntryType.Exact;
            if (bestValue <= alpha)
                flag = EntryType.UpperBound;
            else if (bestValue >= beta)
                flag = EntryType.LowerBound;
            _transpositionTable[hash] = new TranspositionEntry { Score = bestValue, Depth = depth, Flag = flag };

            return bestValue;
        }

        private int QuiescenceSearchOLD(BoardCE board, int alpha, int beta, string cpuColor)
        {
            int standPat = board.CalculateBoardCEScore(cpuColor, board.GetOpponentColor(cpuColor));
            if (standPat >= beta)
                return beta;
            if (alpha < standPat)
                alpha = standPat;

            var captureMoves = board.GetPossibleMovesForColor(cpuColor)
                .Where(m => IsValidIndex(m.ToIndex, board) && board._cases[m.ToIndex] != null)
                .ToList();
            captureMoves = OrderMoves(captureMoves, board, cpuColor);

            foreach (var move in captureMoves)
            {
                var clonedBoard = SafeCloneAndMove(board, move);
                if (clonedBoard == null)
                    continue;
                int score = -QuiescenceSearch(clonedBoard, -beta, -alpha, cpuColor);
                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
            }
            return alpha;
        }
        private int QuiescenceSearch(BoardCE board, int alpha, int beta, string cpuColor, int quiescenceDepth = 0)
        {
            // Limite pour éviter une récursion infinie en phase de quiétude
            if (quiescenceDepth > _maxDepth)
                return board.CalculateBoardCEScore(cpuColor, board.GetOpponentColor(cpuColor));

            int standPat = board.CalculateBoardCEScore(cpuColor, board.GetOpponentColor(cpuColor));
            if (standPat >= beta)
                return beta;
            if (alpha < standPat)
                alpha = standPat;

            var captureMoves = board.GetPossibleMovesForColor(cpuColor)
                .Where(m => IsValidIndex(m.ToIndex, board) && board._cases[m.ToIndex] != null)
                .ToList();
            captureMoves = OrderMoves(captureMoves, board, cpuColor);

            foreach (var move in captureMoves)
            {
                var clonedBoard = SafeCloneAndMove(board, move);
                if (clonedBoard == null)
                    continue;
                int score = -QuiescenceSearch(clonedBoard, -beta, -alpha, cpuColor, quiescenceDepth + 1);
                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
            }
            return alpha;
        }


        private bool TimeExceeded() => (DateTime.UtcNow - _startTime).TotalSeconds > _maxSearchTimeSeconds;

        private List<Move> OrderMoves(List<Move> moves, BoardCE board, string color)
        {
            var killer = new HashSet<Move>();
            lock (_killerLock)
            {
                foreach (var kvp in _killerMoves)
                {
                    foreach (var km in kvp.Value)
                        killer.Add(km);
                }
            }
            return moves.OrderByDescending(move =>
            {
                int score = 0;
                if (killer.Contains(move))
                    score += 1000;
                if (IsValidIndex(move.ToIndex, board) && board._cases[move.ToIndex] != null)
                    score += board.GetPieceValue(board._cases[move.ToIndex]) * 10;
                return score;
            }).ToList();
        }

        private bool IsValidIndex(int index, BoardCE board) => board._cases != null && index >= 0 && index < board._cases.Length;

        private bool IsCaptureMove(Move move, BoardCE board) => IsValidIndex(move.ToIndex, board) && board._cases[move.ToIndex] != null;

        private BoardCE SafeClone(BoardCE board)
        {
            try
            {
                var clone = board.Clone();
                if (clone == null || clone._cases == null)
                    return null;
                return clone;
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync("Erreur lors du clonage: " + ex.Message);
                return null;
            }
        }

        private BoardCE SafeCloneAndMove(BoardCE board, Move move)
        {
            try
            {
                var clone = board.CloneAndMove(move);
                if (clone == null || clone._cases == null)
                    return null;
                return clone;
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync("Erreur lors du clonage et déplacement: " + ex.Message);
                return null;
            }
        }

        private long ComputeBoardHash(BoardCE board, int depth, string color)
        {
            long hash = 0;
            if (board._cases != null)
            {
                for (int i = 0; i < board._cases.Length; i++)
                {
                    if (board._cases[i] != null)
                        hash ^= (long)board._cases[i].GetHashCode() << (i % 16);
                }
            }
            hash ^= depth << 24;
            hash ^= color.GetHashCode() << 32;
            return hash;
        }

        public ChessEngine.NodeCE GetBestModeCE(string colore, ChessEngine.BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {
           
            
            string opponentColor = boardChess.GetOpponentColor(colore);

            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"MAX_SEARCH_TIME_S :  {maxReflectionTimeInMinute}");
            Utils.WritelineAsync($"cpuColor :  {colore}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");


            var boardCustum = new BoardCE(boardChess._cases);
            var bestNodeTemp = GetBestModeCECustum(colore, boardCustum, depthLevel, maxReflectionTimeInMinute);
            //public NodeCE(BoardCE boardCE, Move move, int weight, int level, TimeSpan reflectionTime= new TimeSpan())

            var bestNode = new ChessEngine.NodeCE(boardChess, bestNodeTemp.Move,depthLevel, bestNodeTemp.Weight, bestNodeTemp.ReflectionTime);
            return bestNode;
            //throw new NotImplementedException();
        }
    }
}
