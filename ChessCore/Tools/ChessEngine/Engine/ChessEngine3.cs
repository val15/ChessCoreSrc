using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{
    //OPTIMIZE USIN TRANSPOSITION TABLE IN 0 LEVEL ENGINE DEFAULT DEPTH LEVEL 6
    public class ChessEngine3 : IChessEngine
    {
        // Pool d'objets pour réduire les allocations mémoire
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition pour mettre en cache les états évalués
        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();


        private static object lockObj = new object();
        private int _depthLevel = 0;
        private DateTime _startTime;
        private  int MAX_SEARCH_TIME_S = 60*5;
        private bool _isExperedReflectionTime = false;

        public void Dispose()
        {

        }
        public string GetName()
        {
            return this.GetType().Name;
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
               // var isProtected = clonedBoard.TargetIndexIsProtected(currentNode.ToIndex, cpuColor);
                if (isMenaced /*&& isProtected*/)
                {
                    //T141_W_notD3toD8
                    value -= clonedBoard.GetPieceValue(clonedBoard._cases[currentNode.ToIndex]) / 9;
                    //OLD
                    //value -= clonedBoard.GetPieceValue(clonedBoard._cases[currentNode.ToIndex]) / 10;
                    currentNode.Weight= value;
                }

                // if (clonedBoard.KingIsMenaced(cpuColor))
                //   currentNode.Weight = -9999;


                allNode.Add(currentNode);
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

            equivalentBestNodeCEList = allNode.Where(x => x.Weight == bestValue).ToList();





            //T144A_B_E7toD6
            //get local next turn score for all equivalentBestNodeCEList
            if (equivalentBestNodeCEList.Count > 1)
            {
                Utils.WritelineAsync($"bestNodeCEList calcul immediatelyWeight:");

                //foreach (var node in equivalentBestNodeCEList)
                for (int i = 0; i < equivalentBestNodeCEList.Count; i++)
                {

                    var node = equivalentBestNodeCEList[i];
                    var cloanBoard = board.CloneAndMove(node.FromIndex, node.ToIndex);
                    var opponentColor = board.GetOpponentColor(cpuColor);
                    var immediatelyWeight = cloanBoard.CalculateBoardCEScore(cpuColor, opponentColor);
                    //Utils.WritelineAsync($"{node} ; score : {score}");
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
                Utils.WritelineAsync("REFTECTION TIME LIMIT EXPERED");
                Console.ForegroundColor = ConsoleColor.White;
            }
                
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
            var opponentColor = board.GetOpponentColor(cpuColor);
            if (isReflextionLimitExpered())
               return board.CalculateBoardCEScore(cpuColor, opponentColor);

            // Mise en cache rapide
            var boardHash = ComputeBoardHash(board,depth, cpuColor);
            // if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
            //         return cachedScore;

            
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

                    // currentValue += GetEvolutionBonusOrMalus(board, move, maximizingPlayer);




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


                    // currentValue += GetEvolutionBonusOrMalus(board, move, maximizingPlayer);



                    var clonedBoard = board.CloneAndMove(move);


                    var value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);




                    bestValue = Math.Min(bestValue, value);
                    beta = Math.Min(beta, bestValue);



                    if (beta <= alpha) break;
                }


                return bestValue + currentValue;

            }
        }
       
        private bool isReflextionLimitExpered()
        {
            if(DateTime.UtcNow - _startTime > TimeSpan.FromSeconds(MAX_SEARCH_TIME_S))
            {
                _isExperedReflectionTime = true;
                return true;
            }
            return false;
        }


        //public int GetEvolutionBonusOrMalus(BoardCE boardCE,Move move,bool maximizingPlayer)
        //{
        //    string piece = boardCE._cases[move.FromIndex];

        //    string[] parts = piece.Split('|');
        //    string pieceType = parts[0]; // Type de pièce (P, C, B, etc.)
        //    string pieceColor = parts[1]; // Couleur de la pièce (W ou B)
        //    if(pieceType != "P")
        //        return 0;

        //    if (pieceType == "P")
        //    {
        //        // Bonus pour les pions avancés (encourager les promotions)
        //        // positionalBonus = pieceColor == "W" ? i / 8 * 2 : (7 - i / 8) * 2;

        //        // Bonus supplémentaire pour les pions proches de la promotion
        //        if (pieceColor == "B")
        //        {
        //            if (move.ToIndex >= 56 && move.ToIndex <= 63)
        //            {
        //                if(maximizingPlayer)
        //                    return +10;
        //                else
        //                    return -10;

        //            }
        //        }
        //        else
        //        {
        //            if (move.ToIndex >= 0 && move.ToIndex <= 7)
        //            {

        //                if (maximizingPlayer)
        //                    return +10;
        //                else
        //                    return -10;

        //            }
        //        }
        //    }
        //    return 0;
        //}


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
            // Inclure la profondeur et la couleur dans le hachage
            hash ^= depth << 24;
            hash ^= color.GetHashCode() << 32;
            return hash;
        }

    }



}
