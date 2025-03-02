using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{


    //OPTIMIZE USIN TRANSPOSITION TABLE IN 0 LEVEL AND USE TRANSPOSITION IN MINMAX, ENGINE DEFAULT DEPTH LEVEL 6


    public class ChessEngineNotValiteTextCacheMinMaxMulti : IChessEngine
    {
        // Pool d'objets pour réduire les allocations mémoire
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition pour mettre en cache les états évalués
        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();

        // Modifier le type de _transpositionTable pour stocker des TranspositionEntry
        private readonly ConcurrentDictionary<long, TranspositionEntry> _transpositionTEntry = new ConcurrentDictionary<long, TranspositionEntry>();

        private readonly ConcurrentDictionary<long, TranspositionEntry> _transpositionTEntryMax = new ConcurrentDictionary<long, TranspositionEntry>();

        private readonly ConcurrentDictionary<long, TranspositionEntry> _transpositionTEntryMin = new ConcurrentDictionary<long, TranspositionEntry>();


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

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 7, int maxReflectionTimeInSecond = 60 * 2)
        {
            var cpuColor = colore.First().ToString();
            _depthLevel = depthLevel;
            string opponentColor = boardChess.GetOpponentColor(cpuColor);
            Utils.WritelineAsync($"{GetName()}");
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



        private int MinMaxWithAlphaBetaOLD(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
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

            if (maximizingPlayer)
            {
                int bestValue = int.MinValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);


                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);


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


                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);




                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);



                    if (beta <= alpha) break;
                }
                return bestValue + currentValue;
            }
        }


        private int MinMaxWithAlphaBetaNEWBUG(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            var opponentColor = board.GetOpponentColor(cpuColor);


            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                //int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                //_transpositionTable[boardHash] = new TranspositionEntry(score, depth, alpha, beta);
                //return score;

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

            var cachedEntryScore = 0;

            if (_transpositionTEntry.TryGetValue(boardHash, out TranspositionEntry cachedEntry))
            {
                if (cachedEntry.Depth > depth)
                {
                    if (cachedEntry.Score <= alpha || cachedEntry.Score >= beta && (cachedEntry.MaximizingPlayer != maximizingPlayer && cachedEntry.CpuColor == cpuColor))
                    {
                        cachedEntryScore = cachedEntry.Score;
                    }
                }
            }


            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;



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
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    bestValue = Math.Max(bestValue, value);
                    alpha = Math.Max(alpha, bestValue);
                    if (beta <= alpha) break;
                }
                _transpositionTEntry[boardHash] = new TranspositionEntry(bestValue + currentValue, depth, alpha, beta, false, cpuColor);
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
                    if (beta <= alpha) break;
                }
                _transpositionTEntry[boardHash] = new TranspositionEntry(bestValue + currentValue, depth, alpha, beta, true, cpuColor);
                return bestValue + currentValue;
            }
        }


        private int MinMaxWithAlphaBetaNDNNDF(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            var opponentColor = board.GetOpponentColor(cpuColor);

          

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTEntry[boardHash] = new TranspositionEntry(score, depth, alpha, beta, maximizingPlayer, cpuColor);
                return score;
            }



            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            // Vérification de la table de transposition
            if( depth <= _depthLevel-1 && depth >= 3)
            {
                if (_transpositionTEntry.TryGetValue(boardHash, out TranspositionEntry cachedEntry))
                {
                    if (cachedEntry.Depth >= depth && (cachedEntry.CpuColor == cpuColor && !maximizingPlayer))
                    {
                        if (cachedEntry.Score <= alpha || cachedEntry.Score >= beta)
                        {
                            return cachedEntry.Score;
                        }
                        // Si les bornes ne permettent pas une coupe, retournez simplement le score
                        // return cachedEntry.Score;
                    }
                }
            }
            

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, false, cpuColor);
                return finalScore;
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, true, cpuColor);
                return finalScore;
            }
        }

        private int MinMaxWithAlphaBetaDF(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            var opponentColor = board.GetOpponentColor(cpuColor);
            var cachedEntryScore = -999999999;
            // Vérification de la table de transposition
            if (_transpositionTEntry.TryGetValue(boardHash, out TranspositionEntry cachedEntry))
            {
                if (cachedEntry.Depth >= depth && cachedEntry.MaximizingPlayer == maximizingPlayer && cachedEntry.CpuColor == cpuColor)
                {
                    if (cachedEntry.Score <= alpha)
                    {
                        //return cachedEntry.Score;
                        cachedEntryScore = cachedEntry.Score;
                    }
                    if (cachedEntry.Score >= beta)
                    {
                        //return cachedEntry.Score;
                        cachedEntryScore = cachedEntry.Score;
                    }
                    // Si les bornes ne permettent pas une coupe, retournez simplement le score
                    // return cachedEntry.Score;
                    cachedEntryScore = cachedEntry.Score;
                }
            }

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTEntry[boardHash] = new TranspositionEntry(score, depth, alpha, beta, maximizingPlayer, cpuColor);
                return score;
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                if(cachedEntryScore != finalScore && cachedEntryScore != -999999999)
                {
                    var fdg = 0;
                }
                _transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                return finalScore;
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                if (cachedEntryScore != finalScore && cachedEntryScore != -999999999)
                {
                    var fdg = 0;
                }
                _transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                return finalScore;
            }
        }


        private int MinMaxWithAlphaBetaTranslateDoubleArrayOLD(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            var opponentColor = board.GetOpponentColor(cpuColor);

         

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTEntry[boardHash] = new TranspositionEntry(score, depth, alpha, beta, maximizingPlayer, cpuColor);
                return score;
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;


            // Vérification de la table de transposition

           
                 if (_transpositionTEntryMax.TryGetValue(boardHash, out TranspositionEntry cachedEntryMax) && maximizingPlayer)
                 {
                     if (cachedEntryMax.Depth >= depth && cachedEntryMax.MaximizingPlayer == maximizingPlayer && cachedEntryMax.CpuColor == cpuColor)
                     {
                         if (cachedEntryMax.Score <= alpha)
                         {
                             return cachedEntryMax.Score;
                         }
                         //if (cachedEntry.Score >= beta)
                         //{
                         //    return cachedEntry.Score;
                         //}
                         // Si les bornes ne permettent pas une coupe, retournez simplement le score
                         //return cachedEntry.Score;
                     }
                 }
             
             else
             
                 if (_transpositionTEntryMin.TryGetValue(boardHash, out TranspositionEntry cachedEntryMin))
                 {
                     if (cachedEntryMin.Depth >= depth && cachedEntryMin.MaximizingPlayer == maximizingPlayer && cachedEntryMin.CpuColor == cpuColor)
                     {
                         //if (cachedEntry.Score <= alpha)
                         //{
                         //    return cachedEntry.Score;
                         //}
                         if (cachedEntryMin.Score >= beta)
                         {
                             return cachedEntryMin.Score;
                         }
                         // Si les bornes ne permettent pas une coupe, retournez simplement le score
                         //return cachedEntry.Score;
                     }
                 }
             

            ////if (_transpositionTEntry.TryGetValue(boardHash, out TranspositionEntry cachedEntry))
            ////{
            ////    if (cachedEntry.Depth >= depth && cachedEntry.MaximizingPlayer == maximizingPlayer && cachedEntry.CpuColor == cpuColor)
            ////    {
            ////        if (cachedEntry.Score <= alpha && maximizingPlayer)
            ////        {
            ////            return cachedEntry.Score;
            ////        }
            ////        else if (cachedEntry.Score >= beta && !maximizingPlayer)
            ////        {
            ////            return cachedEntry.Score;
            ////        }

            ////    }
            ////}
            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntryMax[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                //_transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                return finalScore;
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntryMin[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                //_transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                return finalScore;
            }
        }

        private int MinMaxWithAlphaBetaTranslateDoubleArray(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            var opponentColor = board.GetOpponentColor(cpuColor);

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                if (maximizingPlayer)
                {
                    _transpositionTEntryMax[boardHash] = new TranspositionEntry(score, depth, alpha, beta, maximizingPlayer, cpuColor);
                }
                else
                {
                    _transpositionTEntryMin[boardHash] = new TranspositionEntry(score, depth, alpha, beta, maximizingPlayer, cpuColor);
                }
                return score;
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            // Vérification de la table de transposition
            if (maximizingPlayer && _transpositionTEntryMax.TryGetValue(boardHash, out TranspositionEntry cachedEntryMax))
            {
                if (cachedEntryMax.Depth >= depth && cachedEntryMax.MaximizingPlayer == maximizingPlayer && cachedEntryMax.CpuColor == cpuColor)
                {
                    if (cachedEntryMax.Score <= alpha)
                    {
                        return cachedEntryMax.Score;
                    }
                }
            }
            else if (_transpositionTEntryMin.TryGetValue(boardHash, out TranspositionEntry cachedEntryMin))
            {
                if (cachedEntryMin.Depth >= depth && cachedEntryMin.MaximizingPlayer == maximizingPlayer && cachedEntryMin.CpuColor == cpuColor)
                {
                    if (cachedEntryMin.Score >= beta)
                    {
                        return cachedEntryMin.Score;
                    }
                }
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
                    var value = MinMaxWithAlphaBetaTranslateDoubleArray(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    bestValue = Math.Max(bestValue, value);
                    alpha = Math.Max(alpha, bestValue);
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntryMax[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                return finalScore;
            }
            else
            {
                int bestValue = int.MaxValue;
                foreach (Move move in moves)
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBetaTranslateDoubleArray(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntryMin[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, maximizingPlayer, cpuColor);
                return finalScore;
            }
        }



        private int MinMaxWithAlphaBetaTranslate(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board);
            var opponentColor = board.GetOpponentColor(cpuColor);

            // Vérification de la table de transposition
          

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                int score = board.CalculateBoardCEScore(cpuColor, opponentColor);
                _transpositionTEntry[boardHash] = new TranspositionEntry(score, depth, alpha, beta, maximizingPlayer, cpuColor);
                return score;
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            if (_transpositionTEntry.TryGetValue(boardHash, out TranspositionEntry cachedEntry))
            {
                if (cachedEntry.Depth == depth && cachedEntry.MaximizingPlayer == maximizingPlayer && cachedEntry.CpuColor == cpuColor)
                {
                    if (cachedEntry.Score <= alpha || cachedEntry.Score >= beta)
                    {
                        return cachedEntry.Score;
                    }
                    // Si les bornes ne permettent pas une coupe, retournez simplement le score
                    // return cachedEntry.Score;
                }
            }

            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, false, cpuColor);
                return finalScore;
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
                    if (beta <= alpha) break;
                }
                int finalScore = bestValue + currentValue;
                _transpositionTEntry[boardHash] = new TranspositionEntry(finalScore, depth, alpha, beta, true, cpuColor);
                return finalScore;
            }
        }

        private int MinMaxWithAlphaBetaNotTranslate(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);
            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                return board.CalculateBoardCEScore(cpuColor, opponentColor);
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
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
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
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);
                    if (beta <= alpha) break;
                }
                return bestValue + currentValue;
            }
        }

        private int MinMaxWithAlphaBetaHeuristicsAndParallelization(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);
            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                return board.CalculateBoardCEScore(cpuColor, opponentColor);
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
                Parallel.ForEach(moves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);

                    lock (lockObj)
                    {
                        bestValue = Math.Max(bestValue, value);
                        alpha = Math.Max(alpha, bestValue);
                    }
                });

                return bestValue + currentValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                Parallel.ForEach(moves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);

                    lock (lockObj)
                    {
                        bestValue = Math.Min(bestValue, value);
                        beta = Math.Min(beta, bestValue);
                    }
                });

                return bestValue + currentValue;
            }
        }

        private int MinMaxWithAlphaBetaHeuristics(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);
            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                return board.CalculateBoardCEScore(cpuColor, opponentColor);
            }

            // Vérification des échecs dans les dernières profondeurs
            if (depth >= _depthLevel - 2)
            {
                if (board.IsKingInCheck(cpuColor)) return -9999;
                if (board.IsKingInCheck(opponentColor)) return 9999;
            }

            // Heuristique : Vérification rapide pour échec et mat imminent
          //  if (board.IsCheckmate(cpuColor)) return 9999;
           // if (board.IsCheckmate(opponentColor)) return -9999;

            string currentPlayer = maximizingPlayer ? cpuColor : opponentColor;

            // Ordonnancement des mouvements pour améliorer l'élagage
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
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);
                    if (beta <= alpha) break;
                }
                return bestValue + currentValue;
            }
        }


        //private static readonly object lockObj = new object();

        private int MinMaxWithAlphaBetaParallelization(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);
            var currentValue = board.CalculateBoardCEScore(cpuColor, opponentColor) / 10;

            // Conditions d'arrêt rapides avec vérification des échecs
            if (depth == 0 || board.IsGameOver())
            {
                return board.CalculateBoardCEScore(cpuColor, opponentColor);
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
                Parallel.ForEach(moves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    lock (lockObj)
                    {
                        bestValue = Math.Max(bestValue, value);
                        alpha = Math.Max(alpha, bestValue);
                    }
                });

                return bestValue + currentValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                Parallel.ForEach(moves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    lock (lockObj)
                    {
                        bestValue = Math.Min(bestValue, value);
                        beta = Math.Min(beta, bestValue);
                    }
                });

                return bestValue + currentValue;
            }
        }



        private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            return MinMaxWithAlphaBetaTranslate( board,  depth,  alpha,  beta,  maximizingPlayer,  cpuColor);
            //return MinMaxWithAlphaBetaTranslateDoubleArray( board,  depth,  alpha,  beta,  maximizingPlayer,  cpuColor);
            //return MinMaxWithAlphaBetaNotTranslate( board,  depth,  alpha,  beta,  maximizingPlayer,  cpuColor);
            //return MinMaxWithAlphaBetaHeuristicsAndParallelization( board,  depth,  alpha,  beta,  maximizingPlayer,  cpuColor);
            //return MinMaxWithAlphaBetaHeuristics( board,  depth,  alpha,  beta,  maximizingPlayer,  cpuColor);
            //return MinMaxWithAlphaBetaHeuristicsAndParallelization( board,  depth,  alpha,  beta,  maximizingPlayer,  cpuColor);
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


    }


    public class TranspositionEntry
    {
        public int Score { get; set; }
        public int Depth { get; set; }
        public int Alpha { get; set; }
        public int Beta { get; set; }
        public bool MaximizingPlayer { get; set; }
        public string CpuColor { get; set; }

        public TranspositionEntry(int score, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            Score = score;
            Depth = depth;
            Alpha = alpha;
            Beta = beta;
            MaximizingPlayer = maximizingPlayer;
            CpuColor = cpuColor;
        }
    }


}
