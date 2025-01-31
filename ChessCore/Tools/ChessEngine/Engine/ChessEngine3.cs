﻿using System.Collections.Concurrent;

namespace ChessCore.Tools.ChessEngine.Engine
{
    // HASH: BORD
    public class ChessEngine3 : IChessEngine
    {
        // Pool d'objets pour réduire les allocations mémoire
        private static readonly ObjectPool<BoardCE> _boardPool = new ObjectPool<BoardCE>(() => new BoardCE());

        // Table de transposition pour mettre en cache les états évalués
        private readonly ConcurrentDictionary<long, int> _transpositionTable = new ConcurrentDictionary<long, int>();
        
        private static object lockObj = new object();
        private int _depthLevel = 0;

        public void Dispose()
        {

        }
        public string GetName()
        {
            return this.GetType().Name;
        }

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6)
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

}
