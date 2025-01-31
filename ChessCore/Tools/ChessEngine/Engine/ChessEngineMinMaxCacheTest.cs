using System;
using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{
    // HASH: BORD, MIM MAX ALPA BETA
    public class ChessEngineMinMaxCacheTest : IChessEngine
    {


        // Pool d'objets pour réduire les allocations mémoire
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition pour mettre en cache les états évalués
        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();
        // private readonly ConcurrentDictionary<long, int> _transpositionMinMaxTable = new ConcurrentDictionary<long, int>();
       // private ConcurrentDictionary<long, List<(int depth, int value)>> _transpositionMinMaxTable = new ConcurrentDictionary<long, List<(int, int)>>();


        private static object lockObj = new object();
        private int _depthLevel = 0;

        public void Dispose()
        {

        }

        public string GetName()
        {
            return this.GetName();
        }

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6)
        {
            var cpuColor = colore.First().ToString();
            _depthLevel = depthLevel;
            string opponentColor = boardChess.GetOpponentColor(cpuColor);
            Utils.WritelineAsync($"CHESS ENGINE 4 :");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"cpuColor :  {cpuColor}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");
            var bestOfBest = FindBestMode(boardChess, depthLevel, cpuColor);
            //Utils.WritelineAsync($"bestOfBest :  {bestOfBest}");
            return bestOfBest;
        }

        public NodeCE FindBestMode(BoardCE board, int depthLevel, string cpuColor)
        {
            var startTime = DateTime.UtcNow;
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor, true);
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
                value = MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);
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

                // if (clonedBoard.KingIsMenaced(cpuColor))
                //   currentNode.Weight = -9999;


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
            

            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            // if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
            //         return cachedScore;

            var opponentColor = board.GetOpponentColor(cpuColor);
            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;
            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
                    return cachedScore;

                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTable[boardHash] = score;
                return score;
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }



          

            // if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
            //         return cachedScore;
            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
            List<Move> moves = OrderMoves(board.GetPossibleMovesForColor(currentPlayer), board, currentPlayer);
             ConcurrentDictionary<long, List<(int depth, int value)>> transpositionMinMaxTable = new ConcurrentDictionary<long, List<(int, int)>>();

            if (maximizingPlayer)
            {
                // ConcurrentDictionary<long, int> transpositionMinMaxTable = new ConcurrentDictionary<long, int>();

                int bestValue = int.MinValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    // 1️⃣ 🔍 **Génération du hash unique du plateau**
                    long boardMinMaxHash = ComputeBoardMinMaxHash(board, clonedBoard , depth, alpha, beta, maximizingPlayer, cpuColor);

                    // var boardMinMaxHash = ComputeBoardMinMaxHash( clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    int value = 0;
                    // 2️⃣ ⚡ **Vérification dans la table de transposition**
                    if (transpositionMinMaxTable.TryGetValue(boardMinMaxHash, out var cachedEntries))
                    {
                        var entry = cachedEntries.FirstOrDefault(e => e.depth == depth);
                        if (entry != default)
                        {
                            value = entry.value;

                        }
                        else
                            value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    }
                    else
                        value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);

                    //if (_transpositionMinMaxTable.TryGetValue(boardMinMaxHash, out int cachedboardMinMaxHashScore))
                    //{

                    //    isIntranspositionMinMaxTable = true;
                    //    var valueCache = cachedboardMinMaxHashScore;
                    //    value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    //    if (valueCache != value)
                    //    {
                    //        var t = boardMinMaxHash;
                    //    }
                    //}
                    //else
                    // var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);

                    //ENREGISTREMENT DE value
                    transpositionMinMaxTable.AddOrUpdate(boardMinMaxHash,new List<(int, int)> { (depth, value) },
                    (key, oldList) => { oldList.Add((depth, value)); 
                        return oldList; 
                    });

                    bestValue = Math.Max(bestValue, value);
                    alpha = Math.Max(alpha, bestValue);


                    //ENREGISTREMENT DE value
                    // _transpositionMinMaxTable[boardMinMaxHash] = value;
                    //if (!isIntranspositionMinMaxTable)
                    //{
                    //    var exist = _transpositionMinMaxTable.FirstOrDefault(x => x.Key == boardMinMaxHash);
                    //    if (exist.Key != 0 && exist.Value != 0)//existe déja
                    //    {
                    //        var t = exist;
                    //    }
                    //    _transpositionMinMaxTable[boardMinMaxHash] = value;
                    //}



                    if (beta <= alpha) break;
                }

                var finalvalue = bestValue + currentValue;
      
                return finalvalue;
            }
            else
            {
                // ConcurrentDictionary<long, int> transpositionMinMaxTable = new ConcurrentDictionary<long, int>();

                int bestValue = int.MaxValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    long boardMinMaxHash = ComputeBoardMinMaxHash(board, clonedBoard, depth, alpha, beta, maximizingPlayer, cpuColor);

                    //var boardMinMaxHash = ComputeBoardMinMaxHash(clonedBoard, depth - 1, alpha, beta, true, cpuColor);

                    int value = 0;
                    // 2️⃣ ⚡ **Vérification dans la table de transposition**
                    if (transpositionMinMaxTable.TryGetValue(boardMinMaxHash, out var cachedEntries))
                    {
                        var entry = cachedEntries.FirstOrDefault(e => e.depth == depth);
                        if (entry != default)
                        {
                            value = entry.value;

                        }
                        else
                            value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    }
                    else
                        value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);

                    //ENREGISTREMENT DE value
                    transpositionMinMaxTable.AddOrUpdate(boardMinMaxHash, new List<(int, int)> { (depth, value) },
                   (key, oldList) => { oldList.Add((depth, value)); return oldList; });


                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);

                    //ENREGISTREMENT DE value si il n'y es page déja
                    //et si value != 0
                    //if (!isIntranspositionMinMaxTable)
                    //{
                    //    var exist = _transpositionMinMaxTable.FirstOrDefault(x => x.Key == boardMinMaxHash);
                    //    if (exist.Key != 0 && exist.Value != 0)//existe déja
                    //    {
                    //        var t = exist;
                    //    }
                    //    _transpositionMinMaxTable[boardMinMaxHash] = value;

                    //}



                    if (beta <= alpha) break;
                }
                var finalvalue = bestValue + currentValue;
              
                return finalvalue;
            }
        }


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

        //private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        private long ComputeBoardMinMaxHashCollision(BoardCE cloneBoard, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Implémentation simplifiée, à remplacer par un hachage plus robuste
            //long hash = 0;
            //for (int i = 0; i < board._cases.Length; i++)
            //{
            //    if (board._cases[i] != null)
            //    {
            //        hash ^= (long)board._cases[i].GetHashCode() << (i % 16);
            //    }
            //}
            long hashClone = 0;
            for (int i = 0; i < cloneBoard._cases.Length; i++)
            {
                if (cloneBoard._cases[i] != null)
                {
                    hashClone ^= (long)cloneBoard._cases[i].GetHashCode() << (i % 16);
                }
            }
            int maximizingPlayerInt = 0;
            if (maximizingPlayer)
                maximizingPlayerInt = 1;
            var result =  /*hash ^*/ hashClone ^ (long)depth ^ (long)alpha ^ (long)beta ^ (long)maximizingPlayerInt ^ (long)cpuColor.GetHashCode();
            return result;
        }


        private long ComputeBoardMinMaxHashOLD(BoardCE cloneBoard, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            long hash = 0;



            // Générer un hash basé sur l'état du plateau
            for (int i = 0; i < cloneBoard._cases.Length; i++)
            {
                if (cloneBoard._cases[i] != null)
                {
                    // XOR avec un décalage différent pour éviter les collisions
                    hash ^= (long)cloneBoard._cases[i].GetHashCode() << (i % 16);
                }
            }

            // Ajout de la profondeur (impacte les décisions stratégiques)
            hash ^= ((long)depth << 48);

            // Incorporation des valeurs Alpha-Beta (impacte l'élagage)
            hash ^= ((long)alpha << 32);
            hash ^= ((long)beta << 16);

            // MaximizingPlayer influence le hachage (1 ou 0)
            hash ^= maximizingPlayer ? 1L << 8 : 0;

            // Incorporation de la couleur de l'IA (W = 1, B = 0)
            hash ^= cpuColor == "W" ? 1L << 4 : 0;

            return hash;
        }

        private long ComputeBoardMinMaxHash(BoardCE board, BoardCE cloneBoard, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            long hash = ComputeBoardHash(board); // Utilisation de Zobrist Hashing si possible
            long hashClone = ComputeBoardHash(cloneBoard); // Utilisation de Zobrist Hashing si possible
            hash ^= hashClone;
            // Incorporation des valeurs essentielles dans le hash
            hash ^= (depth & 0xFF) << 48;   // Profondeur (8 bits max)
            hash ^= ((long)alpha & 0xFFFF) << 32;  // Alpha (16 bits)
            hash ^= ((long)beta & 0xFFFF) << 16;   // Beta (16 bits)
            hash ^= maximizingPlayer ? 1L : 0L;    // MaximizingPlayer booléen (1 ou 0)
            hash ^= cpuColor == "W" ? 2L : 4L;     // Différenciation couleur IA (2 ou 4)

            return hash;
        }


    }


}
