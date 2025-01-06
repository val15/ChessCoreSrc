using System.Collections.Concurrent;

namespace ChessCore.Tools
{
    public class ChessEngine : IDisposable
    {
        private bool _checkIsInChessOnEnd;


        public NodeGPT GetBestPositionLocalUsingMiltiThreading(string colore, BoardGPT boardChess, bool isReprise, List<SpecificBoardGPT> specificBoardGPTList)
        {

            var activeL5 = true;
            NodeGPT FinalBest = null;
            var bestList = new List<NodeGPT>();
            var l5 = new NodeGPT();
            l5.Weight = -999999;
            var maxiDiffToTakeMinimum = 0;
            var l1 = RunEngine(colore, boardChess, isReprise, specificBoardGPTList, 1, false);
            var l1p = RunEngine(colore, boardChess, isReprise, specificBoardGPTList, 1, false, false);

            if (l1.Weight == 9999)
            {
                Utils.WritelineAsync($"WIN in l1 : {l1}");
                return l1;

            }
            var l3 = RunEngine(colore, boardChess, isReprise, specificBoardGPTList, 3, false);
            // var l3p = RunEngine(colore, boardChess, isReprise, specificBoardGPTList, 3, false,false);

            if (activeL5)
                l5 = RunEngine(colore, boardChess, isReprise, specificBoardGPTList, 5, false, true, 60 * 60 * 24);

            Utils.WritelineAsync($"l1 : {l1}");
            Utils.WritelineAsync($"l1p : {l1p}");
            Utils.WritelineAsync($"l3 : {l3}");
            //Utils.WritelineAsync($"l3p : {l3p}");
            Utils.WritelineAsync($"l5 : {l5}");

            //var l5BestNode = RunEngine(colore, boardChess, isReprise, specificBoardGPTList, 5, false);
            //Utils.WritelineAsync($"L5 : {l5BestNode}");
            //if (l5BestNode.Weight > l5.Weight && l5BestNode.Location != l5.Location && l5BestNode.BestChildPosition != l5.BestChildPosition)
            //{
            //    Utils.WritelineAsync($"BEST L5: {l5BestNode}");
            //}

            //T60BlackIsInChessInL3
            if (l1.Weight > -9000 && l3.Weight < -9000 && l5.Weight < -9000)
            {
                FinalBest = l1;
                Utils.WritelineAsync($"FinalBest: {FinalBest}");
                return FinalBest;
            }


            //recalibre l1 
            if (l1.Weight > l3.Weight) // 14 failds
            {

                foreach (var node in l1.EquivalentBestNodeGPTList)
                {

                    var maxNode = l3.AllNodeGPTList.FirstOrDefault(x => x.FromIndex == node.FromIndex);
                    if (maxNode != null)
                    {

                        var currentDiff = Math.Abs(node.Weight - maxNode.Weight);

                        if (currentDiff > 1000 && node.Weight < 9000 && currentDiff < 20000/*T67EchecBlancLeRoiDoitSeMettreEnE1*/)
                            node.Weight = maxNode.Weight;
                    }
                }

                //réfind best in l1
                Utils.WritelineAsync($"refind best in l1:");
                var maxWeight = l1.EquivalentBestNodeGPTList.Max(x => x.Weight);

                var newEquivalentBestNodeGPTList = l1.EquivalentBestNodeGPTList.Where(x => x.Weight == maxWeight).ToList();
                var rand = new Random();
                l1 = newEquivalentBestNodeGPTList[rand.Next(newEquivalentBestNodeGPTList.Count)];
                l1.EquivalentBestNodeGPTList = newEquivalentBestNodeGPTList;

                Utils.WritelineAsync($"bestNodeGPTList after refind :");
                foreach (var node in l1.EquivalentBestNodeGPTList)
                {
                    Utils.WritelineAsync($"{node}");
                }
                Utils.WritelineAsync($"new best l1 {l1}");
            }


            //recalibre l1p 

            //if (l1p.Weight > l3.Weight) // 14 failds
            //{

            //    foreach (var node in l1p.EquivalentBestNodeGPTList)
            //    {

            //        var maxNode = l3.AllNodeGPTList.FirstOrDefault(x => x.FromIndex == node.FromIndex);
            //        if (maxNode != null)
            //        {

            //            var currentDiff = Math.Abs(node.Weight - maxNode.Weight);

            //            if (currentDiff > 1000 && node.Weight < 9000 && currentDiff < 20000/*T67EchecBlancLeRoiDoitSeMettreEnE1*/)
            //                node.Weight = maxNode.Weight;
            //        }
            //    }

            //    //réfind best in l1p
            //    Utils.WritelineAsync($"refind best in l1:");
            //    var maxWeight = l1p.EquivalentBestNodeGPTList.Max(x => x.Weight);

            //    var newEquivalentBestNodeGPTList = l1p.EquivalentBestNodeGPTList.Where(x => x.Weight == maxWeight).ToList();
            //    var rand = new Random();
            //    l1p = newEquivalentBestNodeGPTList[rand.Next(newEquivalentBestNodeGPTList.Count)];
            //    l1p.EquivalentBestNodeGPTList = newEquivalentBestNodeGPTList;

            //    Utils.WritelineAsync($"bestNodeGPTList after refind :");
            //    foreach (var node in l1p.EquivalentBestNodeGPTList)
            //    {
            //        Utils.WritelineAsync($"{node}");
            //    }
            //    Utils.WritelineAsync($"new best l1p {l1p}");
            //}


            var diff = Math.Abs(l1.Weight - l3.Weight);



            //T29_W_PourProtegerDEchec
            if (l1.Weight < 0 && l3.Weight < 0)
                maxiDiffToTakeMinimum = 10;
            if (l1.Weight > l3.Weight && diff > maxiDiffToTakeMinimum) // 14 failds
            {
                FinalBest = l1;
            }
            else
                FinalBest = l3;



            if (l1p.Weight > FinalBest.Weight && FinalBest.Weight < -200)
            {
                Utils.WritelineAsync($"l3 ({l3}) and l1 ({l1}) are in chess, take l1p ({l1p})");
                FinalBest = l1p;
            }

            if (l5.Weight >= FinalBest.Weight)
            {
                FinalBest = l5;

            }


            //bestList.Add(l1p);
            //bestList.Add(l3p);



            Utils.WritelineAsync($"FinalBest: {FinalBest}");




            return FinalBest;
        }


        public NodeGPT RunEngine(string colore, BoardGPT boardChess, bool isReprise, List<SpecificBoardGPT> specificBoardGPTList, int depthLevel, bool isOppinionTurnInNext, bool checkIsInChessOnEnd = true, int limitOfReflectionTimeInSecond = 24 * 60 * 60)
        {
            try
            {
                var maxWeight = double.NegativeInfinity;
                string cpuColor = colore[0].ToString();
                string opponentColor = cpuColor == "W" ? "B" : "W";
                _checkIsInChessOnEnd = checkIsInChessOnEnd;

                var bestNodeGPTList = new ConcurrentBag<NodeGPT>();

                var pawnIndices = boardChess.GetCasesIndexForColor(cpuColor);

                // Parallélisation des calculs
                Parallel.ForEach(pawnIndices, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, pawnIndex =>
                {
                    var possibleMoves = boardChess.GetPossibleMoves(pawnIndex);

                    foreach (var move in possibleMoves)
                    {
                        var clonedBoard = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                        var node = MinMaxWithAlphaBeta(clonedBoard, depthLevel, double.NegativeInfinity, double.PositiveInfinity, isOppinionTurnInNext, opponentColor);
                        node.FromIndex = move.FromIndex;
                        node.ToIndex = move.ToIndex;

                        lock (bestNodeGPTList)
                        {
                            Utils.WritelineAsync(node.ToString());
                            bestNodeGPTList.Add(node);
                        }
                    }
                });

                // Sélection du meilleur coup
                var bestNode = bestNodeGPTList.OrderByDescending(x => x.Weight).FirstOrDefault();
                return bestNode;
            }
            finally
            {
                Utils.GCColect();
            }
        }

        public NodeGPT MinMaxWithAlphaBeta(BoardGPT board, int depth, double alpha, double beta, bool maximizingPlayer, string cpuColor)
        {
            var executionEngineTime = DateTime.UtcNow - Utils.EnginStartTime;
         //   if (executionEngineTime.TotalSeconds > Utils.LimitOfReflectionTimeInSecond)
         //       return new NodeGPT() { Level = -1, Weight = -9999 };

            var currentNodeGPT = new NodeGPT { Level = depth, Colore = cpuColor };
            string opponentColor = cpuColor == "W" ? "B" : "W";

            // Vérification de fin de partie ou de profondeur
            if (depth == 0 || board.IsGameOver())
            {
                currentNodeGPT.Weight = board.CalculateBoardGPTScore(board, cpuColor, opponentColor);
                if (_checkIsInChessOnEnd)
                {
                    if (board.IsKingInCheck(cpuColor)) currentNodeGPT.Weight = -9999;
                    if (board.IsKingInCheck(opponentColor)) currentNodeGPT.Weight = 9999;
                }
                return currentNodeGPT;
            }

            double bestValue = maximizingPlayer ? double.NegativeInfinity : double.PositiveInfinity;

            var moves = board.GetPossibleMovesForColor(cpuColor);
            Parallel.ForEach(moves, move =>
            {
                var clonedBoard = board.CloneAndMove(move.FromIndex, move.ToIndex);
                var childNodeGPT = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, !maximizingPlayer, opponentColor);

                lock (currentNodeGPT)
                {
                    if (maximizingPlayer)
                    {
                        if (childNodeGPT.Weight > bestValue)
                        {
                            bestValue = childNodeGPT.Weight;
                            currentNodeGPT.FromIndex = move.FromIndex;
                            currentNodeGPT.ToIndex = move.ToIndex;
                        }
                        alpha = Math.Max(alpha, bestValue);
                    }
                    else
                    {
                        if (childNodeGPT.Weight < bestValue)
                        {
                            bestValue = childNodeGPT.Weight;
                            currentNodeGPT.FromIndex = move.FromIndex;
                            currentNodeGPT.ToIndex = move.ToIndex;
                        }
                        beta = Math.Min(beta, bestValue);
                    }

                    if (beta <= alpha)
                        return; // Élagage Alpha-Bêta
                }
            });

            currentNodeGPT.Weight = (int)bestValue;
            return currentNodeGPT;
        }

        public void Dispose() => GC.Collect();
    }


    public class BoardGPT
    {
        public string[] _cases = new string[64];
        public void Print()
        {
            Utils.WritelineAsync("_____________________________________________________________________");
            for (int y = 0; y < 8; y++)
            {
                var line = "";
                for (int x = 0; x < 8; x++)
                {
                    var index = x + (y * 8);
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Utils.WritelineAsync(line);
            }
            Utils.WritelineAsync("_____________________________________________________________________");
        }

        public BoardGPT()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
        }
        public BoardGPT(string[] cases)
        {
            _cases = cases;
        }

        public BoardGPT(BoardGPT other)
        {
            Array.Copy(other._cases, _cases, 64);
        }

        public void InsertPawn(int index, string pieceType, string color)
        {
            _cases[index] = $"{pieceType}|{color}";
        }

        public void Move(int fromIndex, int toIndex)
        {
            _cases[toIndex] = _cases[fromIndex];
            _cases[fromIndex] = "__";
        }

        public int GetMenacedsPoints(string color)
        {
            var result = 0;
            var opponentColor = GetOpponentColor(color);

            var indexList = GetCasesIndexForColor(color);

            Parallel.ForEach(indexList, index =>
            {


                if (TargetIndexIsMenaced(index, opponentColor))
                {
                    //on ne prend pas en compte les rois
                    if (!_cases[index].StartsWith("K"))
                        result += GetPieceValue(_cases[index]) / 10;
                }

            });
            return result;
        }

        public List<int> GetToOpponentKingPath(int threateningIndex, int kingIndex)
        {
            var toOpponentKingPath = new List<int>();
            if (_cases[threateningIndex] == "__")
                return toOpponentKingPath;

            var pieceType = _cases[threateningIndex].First();
            var pieceColor = _cases[threateningIndex].Last();

            if (pieceType != 'Q' && pieceType != 'B' && pieceType != 'T')
                return toOpponentKingPath;

            if (pieceType == 'B')
                return GetBishopMovesPathToOpponentKing(threateningIndex, pieceColor.ToString(), kingIndex);
            if (pieceType == 'T')
                return GetRookMovesPathToOpponentKing(threateningIndex, pieceColor.ToString(), kingIndex);


            if (pieceType == 'Q')
                return GetQueenMovesPathToOpponentKing(threateningIndex, pieceColor.ToString(), kingIndex);


            return toOpponentKingPath;
        }


        public bool IsKingInCheck(string color)
        {
            // Identifier l'opposant
            string opponentColor = GetOpponentColor(color);

            // Trouver l'index du roi de la couleur donnée
            int kingIndex = Array.FindIndex(_cases, piece => piece == $"K|{color}");

            if (kingIndex == -1)
            {
                return true;
            }


            //si le roi adverse peux encore bouger ou pas
            //si ces dirrections son menacé ou non
            var kingPosibleMoves = GetPossibleMoves(kingIndex);
            if (kingPosibleMoves.Count > 0)
            {
                foreach (var kingMove in kingPosibleMoves)
                {
                    if (kingMove.ToIndex == 18)
                    {
                        var fdf = 0;
                    }
                    if (!TargetIndexIsMenaced(kingMove.ToIndex, opponentColor))
                    {

                        return false;

                    }

                }
            }

            //


            // Obtenir tous les mouvements possibles de l'opposant
            var opponentMoves = GetPossibleMovesForColor(opponentColor);

            // Vérifier si l'un des mouvements peut atteindre le roi
            foreach (var move in opponentMoves)
            {

                if (move.ToIndex == kingIndex)
                {



                    //Pour ce move.ToIndex to kingIndex
                    //on cherche tokingIndexPath
                    var toOpponentKingPathIndexList = GetToOpponentKingPath(move.FromIndex, kingIndex);

                    //On cherche le mouvements possible des alier du roi menacé 
                    var alierPossibleMoves = GetPossibleMovesForColor(color);
                    foreach (var kingPosible in kingPosibleMoves)
                    {
                        //on enleve les mouvement du roi
                        alierPossibleMoves.RemoveAll(x => x.FromIndex == kingPosible.FromIndex && x.ToIndex == kingPosible.ToIndex);
                    }


                    //si un de ses mouvement est dans tokingIndexPath
                    foreach (var toOpponentKingPathIndex in toOpponentKingPathIndexList)
                    {
                        if (alierPossibleMoves.Select(x => x.ToIndex).Contains(toOpponentKingPathIndex))
                            return false;
                    }

                    //si celui qui menace est menacée en non pas par le roi
                    var indexOfOpponentsWhoThreatenList = GetMovesOfOpponentsWhoThreaten(move.FromIndex, color);
                    //on enleve l'index du roir nenacé
                    indexOfOpponentsWhoThreatenList.RemoveAll(x => x.FromIndex == kingIndex);
                    if (indexOfOpponentsWhoThreatenList.Count > 0)
                        return false;

                    return true; // Le roi est en échec
                }
            }

            return false; // Le roi n'est pas en échec
        }

        public List<Move> GetPossibleMovesForColor(string color)
        {
            var moves = new List<Move>();
            var indicesForColor = GetCasesIndexForColor(color);

            foreach (var fromIndex in indicesForColor)
            {
                moves.AddRange(GetPossibleMovesOLD(fromIndex));
            }

            return moves;
        }
        public int GetPieceValue(string pieceType)
        {
            return pieceType.First().ToString() switch
            {
                "P" => 10, // Pion
                "C" => 30, // Cavalier
                "B" => 30, // Fou
                "T" => 50, // Tour
                "Q" => 90, // Reine
                "K" => 10000, // Roi
                _ => 0,
            };
        }

        private static readonly int[] centralSquares = { 28, 29, 36, 37, 44, 45, 52, 53 };

        public int CalculateBoardGPTScore(BoardGPT board, string color, string opponentColor)
        {

            int whiteScore = 0;
            int blackScore = 0;

            // Définir les valeurs des pièces
            Dictionary<string, int> pieceValues = new Dictionary<string, int>
    {
        { "P", 10 },  // Pion
        { "C", 30 },  // Cavalier
        { "B", 30 },  // Fou
        { "T", 50 },  // Tour
        { "Q", 90 },  // Reine
        { "K", 10000 } // Roi (valeur arbitraire très élevée pour éviter sa capture)
    };

            // Parcourir le plateau pour calculer les scores
            for (int i = 0; i < _cases.Length; i++)
            {
                string piece = _cases[i];

                if (piece == "__") // Case vide
                    continue;

                string[] parts = piece.Split('|');
                string pieceType = parts[0]; // Type de pièce (P, C, B, etc.)
                string pieceColor = parts[1]; // Couleur de la pièce (W ou B)
                int positionalBonus = 0;
                if (pieceValues.ContainsKey(pieceType))
                {

                    // Calcul des bonus/malus pour les pions
                    if (pieceType == "P")
                    {
                        // Bonus pour les pions avancés (encourager les promotions)
                        // positionalBonus = pieceColor == "W" ? i / 8 * 2 : (7 - i / 8) * 2;

                        // Bonus supplémentaire pour les pions proches de la promotion
                        if (pieceColor == "B")
                        {
                            if (i >= 56 && i <= 63)
                            {
                                positionalBonus += 90;
                            }
                        }
                        else
                        {
                            if (i >= 0 && i <= 7)
                            {
                                positionalBonus += 90;
                            }
                        }
                    }


                    if (pieceColor == "W")
                        whiteScore += pieceValues[pieceType] + positionalBonus;
                    else if (pieceColor == "B")
                        blackScore += pieceValues[pieceType] + positionalBonus;
                }
            }

            // Calcul du score en fonction de la couleur donnée
            return color == "W" ? whiteScore - blackScore : blackScore - whiteScore;

        }



        public bool TargetIndexIsMenaced(int index, string opponentColor)
        {
            var indexOfOpponentsWhoThreatenList = GetMovesOfOpponentsWhoThreaten(index, opponentColor);
            if (indexOfOpponentsWhoThreatenList.Count() > 0)
                return true;
            return false;
        }




        public List<Move> GetMovesOfOpponentsWhoThreaten(int index, string opponentColor)
        {
            var result = new List<Move>();
            if (index == 12)
            {
                var dfdf = index;
            }
            var color = GetOpponentColor(opponentColor);
            //T67WhiteIsInChess
            //on vide la case de l'index
            var oldContaine = _cases[index];
            if (_cases[index].EndsWith(opponentColor))
                _cases[index] = _cases[index].Replace($"|{_cases[index].Last()}", $"|{color}");


            // Récupère les indices des pièces adverses
            var opponentPawnIndexList = this.GetCasesIndexForColor(opponentColor);


            // Utilisation de Parallel.ForEach pour paralléliser les itérations
            Parallel.ForEach(opponentPawnIndexList, (opponentPawnIndex, state) =>
            {
                // Récupère les mouvements possibles pour chaque pièce adverse
                if (opponentPawnIndex == 18)
                {
                    var dsd = 9;
                }
                var opponentPossiblesMoves = GetPossibleMovesOLD(opponentPawnIndex);

                // Vérifie si un mouvement menace la case cible
                foreach (var enemyMove in opponentPossiblesMoves)
                {
                    if (enemyMove.ToIndex == index)
                    {
                        result.Add(enemyMove);
                        //isMenaced = true;
                        //state.Stop(); // Arrête toutes les autres itérations
                        //  _cases[index] = oldContaine;
                        //break;
                    }
                }
            });
            _cases[index] = oldContaine;
            return result;

        }

        public bool KingIsMenaced(string kingColor)
        {
            string opponentColor = GetOpponentColor(kingColor);

            // Trouver l'index du roi de la couleur donnée
            int kingIndex = Array.FindIndex(_cases, piece => piece == $"K|{kingColor}");

            return TargetIndexIsMenaced(kingIndex, opponentColor);


        }


        public bool TargetIndexIsProtected(int index, string allierColor)
        {
            //on change la couleur de la cse
            var opponentColor = "W";
            if (allierColor == "W")
                opponentColor = "B";
            _cases[index] = _cases[index].Replace($"|{allierColor}", $"|{opponentColor}");
            var allierPawnIndexList = this.GetCasesIndexForColor(allierColor);
            foreach (var opponentPawnIndex in allierPawnIndexList)
            {
                foreach (var enemyMove in GetPossibleMoves(opponentPawnIndex))
                {
                    if (enemyMove.ToIndex == index)
                    {
                        return true;
                    }
                }


            }
            return false;

        }


        public string GetOpponentColor(string color)
        {
            return color == "W" ? "B" : "W";
        }

        public bool IsGameOver()
        {
            return !_cases.Any(c => c.StartsWith("K|"));
        }

        public BoardGPT CloneAndMove(int fromIndex, int toIndex)
        {
            var clone = new BoardGPT(this);
            clone.Move(fromIndex, toIndex);
            return clone;
        }

        public List<int> GetCasesIndexForColor(string color)
        {
            var indices = new List<int>();
            for (int i = 0; i < _cases.Length; i++)
                if (_cases[i].EndsWith($"|{color}"))
                    indices.Add(i);
            return indices;
        }
        public List<Move> GetPossibleMoves(int fromIndex)
        {
            List<Move> moves = new List<Move>();
            string piece = _cases[fromIndex];

            if (piece == "__") // Case vide, pas de mouvement possible
                return moves;

            string pieceType = piece.Split('|')[0];
            string pieceColor = piece.Split('|')[1];

            switch (pieceType)
            {
                case "P": // Pion
                    moves.AddRange(GetPawnMoves(fromIndex, pieceColor));
                    break;

                case "T": // Tour
                    moves.AddRange(GetRookMovesOLD(fromIndex, pieceColor));
                    break;

                case "C": // Cavalier
                    moves.AddRange(GetKnightMoves(fromIndex, pieceColor));
                    break;

                case "B": // Fou
                    moves.AddRange(GetBishopMoves(fromIndex, pieceColor));
                    break;

                case "Q": // Reine
                    moves.AddRange(GetQueenMoves(fromIndex, pieceColor));
                    break;

                case "K": // Roi
                    moves.AddRange(GetKingMoves(fromIndex, pieceColor));
                    break;
            }

            return moves;
        }


        private List<Move> GetPawnMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            int direction = pieceColor == "W" ? -1 : 1;
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;

            // Mouvement simple
            int forwardIndex = fromIndex + (8 * direction);
            if (IsWithinBounds(forwardIndex) && _cases[forwardIndex] == "__")
            {
                moves.Add(new Move(fromIndex, forwardIndex));
            }

            // Mouvement double depuis la position initiale
            if ((pieceColor == "W" && fromRow == 6) || (pieceColor == "B" && fromRow == 1))
            {
                int doubleForwardIndex = fromIndex + (16 * direction);
                if (IsWithinBounds(doubleForwardIndex) && _cases[forwardIndex] == "__" && _cases[doubleForwardIndex] == "__")
                {
                    moves.Add(new Move(fromIndex, doubleForwardIndex));
                }
            }

            // Captures diagonales
            foreach (int diagOffset in new[] { -1, 1 })
            {
                int diagCol = fromCol + diagOffset;
                int diagIndex = forwardIndex + diagOffset;

                if (IsWithinBounds(diagIndex) && diagCol >= 0 && diagCol < 8 && _cases[diagIndex] != "__" && !_cases[diagIndex].EndsWith(pieceColor))
                {
                    moves.Add(new Move(fromIndex, diagIndex));
                }
            }

            return moves;
        }
        private List<Move> GetRookMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            int[] directions = { -8, 8, -1, 1 }; // Haut, bas, gauche, droite

            foreach (int direction in directions)
            {
                int currentIndex = fromIndex;

                while (true)
                {
                    currentIndex += direction;

                    if (!IsWithinBounds(currentIndex) || IsEdgeCase(fromIndex, currentIndex, direction))
                        break;

                    if (_cases[currentIndex] == "__")
                    {
                        moves.Add(new Move(fromIndex, currentIndex));
                    }
                    else
                    {
                        if (!_cases[currentIndex].EndsWith(pieceColor)) // Capture
                        {
                            moves.Add(new Move(fromIndex, currentIndex));
                        }
                        break; // Bloqué par une pièce
                    }
                }
            }

            return moves;
        }

        private List<int> GetRookMovesPathToOpponentKing(int fromIndex, string pieceColor, int opponentKingIndex)
        {

            int[] directions = { -8, 8, -1, 1 }; // Haut, bas, gauche, droite

            foreach (int direction in directions)
            {
                List<int> pathToOpponentKingIndex = new List<int>();
                int currentIndex = fromIndex;
                while (true)
                {
                    currentIndex += direction;

                    if (!IsWithinBounds(currentIndex) || IsEdgeCase(fromIndex, currentIndex, direction))
                        break;

                    if (_cases[currentIndex] == "__")
                    {
                        pathToOpponentKingIndex.Add(currentIndex);

                    }
                    else
                    {
                        if (!_cases[currentIndex].EndsWith(pieceColor)) // Capture
                        {
                            pathToOpponentKingIndex.Add(currentIndex);
                            if (currentIndex == opponentKingIndex)
                                return pathToOpponentKingIndex;
                        }
                        break; // Bloqué par une pièce
                    }
                }
            }

            return new List<int>();
        }

        private bool IsEdgeCase(int fromIndex, int currentIndex, int direction)
        {
            // Gérer les transitions entre les colonnes
            int fromCol = fromIndex % 8;
            int currentCol = currentIndex % 8;

            if ((direction == -1 || direction == 1) && Math.Abs(fromCol - currentCol) > 1)
                return true;

            if ((direction == -9 || direction == 7) && fromCol == 0)
                return true;

            if ((direction == -7 || direction == 9) && fromCol == 7)
                return true;

            return false;
        }
        private List<Move> GetKnightMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            int[] rowOffsets = { -2, -2, -1, 1, 2, 2, 1, -1 };
            int[] colOffsets = { -1, 1, 2, 2, 1, -1, -2, -2 };

            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int newRow = fromRow + rowOffsets[i];
                int newCol = fromCol + colOffsets[i];
                int newIndex = newRow * 8 + newCol;

                if (IsWithinBounds(newIndex) && newCol >= 0 && newCol < 8 && (_cases[newIndex] == "__" || !_cases[newIndex].EndsWith(pieceColor)))
                {
                    moves.Add(new Move(fromIndex, newIndex));
                }
            }

            return moves;
        }

        private List<Move> GetBishopMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            int[] directions = { -9, -7, 7, 9 }; // Diagonales : haut-gauche, haut-droite, bas-gauche, bas-droite
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;

            foreach (int direction in directions)
            {
                int currentIndex = fromIndex;

                while (true)
                {
                    int currentRow = currentIndex / 8;
                    int currentCol = currentIndex % 8;

                    currentIndex += direction;

                    if (!IsWithinBounds(currentIndex))
                        break;

                    int newRow = currentIndex / 8;
                    int newCol = currentIndex % 8;

                    // Si le mouvement traverse une bordure de colonne
                    if (Math.Abs(newCol - currentCol) > 1)
                        break;

                    if (_cases[currentIndex] == "__")
                    {
                        moves.Add(new Move(fromIndex, currentIndex));
                    }
                    else
                    {
                        // Si la case contient une pièce adverse
                        if (!_cases[currentIndex].EndsWith(pieceColor))
                        {
                            moves.Add(new Move(fromIndex, currentIndex));
                        }
                        break; // Arrêt si une pièce bloque la route
                    }
                }
            }

            return moves;
        }

        private List<int> GetBishopMovesPathToOpponentKing(int fromIndex, string pieceColor, int opponentKingIndex)
        {
            int[] directions = { -9, -7, 7, 9 }; // Diagonales : haut-gauche, haut-droite, bas-gauche, bas-droite
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;

            foreach (int direction in directions)
            {
                List<int> pathToOpponentKingIndex = new List<int>();
                int currentIndex = fromIndex;

                while (true)
                {
                    int currentRow = currentIndex / 8;
                    int currentCol = currentIndex % 8;

                    currentIndex += direction;

                    if (!IsWithinBounds(currentIndex))
                        break;

                    int newRow = currentIndex / 8;
                    int newCol = currentIndex % 8;

                    // Si le mouvement traverse une bordure de colonne
                    if (Math.Abs(newCol - currentCol) > 1)
                        break;

                    if (_cases[currentIndex] == "__")
                    {
                        pathToOpponentKingIndex.Add(currentIndex);
                    }
                    else
                    {
                        // Si la case contient une pièce adverse
                        if (!_cases[currentIndex].EndsWith(pieceColor))
                        {
                            pathToOpponentKingIndex.Add(currentIndex);
                            if (currentIndex == opponentKingIndex)
                                return pathToOpponentKingIndex;
                        }
                        break; // Arrêt si une pièce bloque la route
                    }
                }
            }

            return new List<int>();
        }


        private bool IsWithinBounds(int index)
        {
            return index >= 0 && index < 64;
        }

        private List<Move> GetQueenMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            moves.AddRange(GetRookMoves(fromIndex, pieceColor)); // Combine les mouvements de la tour
            moves.AddRange(GetBishopMoves(fromIndex, pieceColor)); // et ceux du fou
            return moves;
        }

        private List<int> GetQueenMovesPathToOpponentKing(int fromIndex, string pieceColor, int opponentKingIndex)
        {
            List<int> pathToOpponentKingIndex = new List<int>();
            pathToOpponentKingIndex.AddRange(GetRookMovesPathToOpponentKing(fromIndex, pieceColor, opponentKingIndex)); // Combine les mouvements de la tour
            pathToOpponentKingIndex.AddRange(GetBishopMovesPathToOpponentKing(fromIndex, pieceColor, opponentKingIndex)); // et ceux du fou

            pathToOpponentKingIndex.RemoveAll(x => x == opponentKingIndex);
            return pathToOpponentKingIndex;
        }

        private List<Move> GetKingMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            int[] directions = { -9, -8, -7, -1, 1, 7, 8, 9 }; // Toutes les directions

            foreach (int direction in directions)
            {
                int targetIndex = fromIndex + direction;
                if (IsWithinBounds(targetIndex) && (_cases[targetIndex] == "__" || !_cases[targetIndex].EndsWith(pieceColor)))
                {
                    moves.Add(new Move(fromIndex, targetIndex));
                }
            }

            return moves;
        }


        public List<Move> GetPossibleMovesOLD(int fromIndex)
        {
            var moves = new List<Move>();
            string piece = _cases[fromIndex];
            if (piece == "__") return moves;

            string pieceType = piece.Split('|')[0];
            string pieceColor = piece.Split('|')[1];

            switch (pieceType)
            {
                case "P": moves.AddRange(GetPawnMovesOLD(fromIndex, pieceColor)); break;
                case "C": moves.AddRange(GetKnightMovesOLD(fromIndex)); break;
                case "B": moves.AddRange(GetBishopMovesOLD(fromIndex, pieceColor)); break;
                case "T": moves.AddRange(GetRookMovesOLD(fromIndex, pieceColor)); break;
                case "Q": moves.AddRange(GetQueenMovesOLD(fromIndex, pieceColor)); break;
                case "K": moves.AddRange(GetKingMovesOLD(fromIndex, pieceColor)); break;
            }
            return moves;
        }
        // Méthode pour obtenir les déplacements possibles d'une pièce donnée

        private List<Move> GetPawnMovesOLD(int fromIndex, string color)
        {
            List<Move> moves = new List<Move>();
            int direction = (color == "W") ? -1 : 1; // Les pions blancs avancent vers le haut, les noirs vers le bas
            int row = fromIndex / 8;
            int col = fromIndex % 8;

            // Mouvement de base (un pas en avant)
            int forwardIndex = fromIndex + (direction * 8);
            if (IsValidIndex(forwardIndex) && _cases[forwardIndex] == "__")
            {
                moves.Add(new Move(fromIndex, forwardIndex));
            }

            // Mouvement initial (deux pas en avant)
            if ((color == "W" && row == 6) || (color == "B" && row == 1))
            {
                int doubleForwardIndex = fromIndex + (direction * 16);
                if (IsValidIndex(doubleForwardIndex) && _cases[doubleForwardIndex] == "__" && _cases[forwardIndex] == "__")
                {
                    moves.Add(new Move(fromIndex, doubleForwardIndex));
                }
            }

            // Capture en diagonale gauche
            if (col > 0)
            {
                int captureLeftIndex = fromIndex + (direction * 8) - 1;
                if (IsValidIndex(captureLeftIndex) && _cases[captureLeftIndex] != "__")
                {
                    string targetColor = _cases[captureLeftIndex].Split('|')[1];
                    if (targetColor != color) // Vérifie que la cible appartient à l'adversaire
                    {
                        moves.Add(new Move(fromIndex, captureLeftIndex));
                    }
                }
            }

            // Capture en diagonale droite
            if (col < 7)
            {
                int captureRightIndex = fromIndex + (direction * 8) + 1;
                if (IsValidIndex(captureRightIndex) && _cases[captureRightIndex] != "__")
                {
                    string targetColor = _cases[captureRightIndex].Split('|')[1];
                    if (targetColor != color) // Vérifie que la cible appartient à l'adversaire
                    {
                        moves.Add(new Move(fromIndex, captureRightIndex));
                    }
                }
            }

            return moves;
        }
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < 64;
        }

        private List<Move> GetKnightMovesOLD(int fromIndex)
        {
            var moves = new List<Move>();
            int[] knightOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };
            string pieceColor = _cases[fromIndex].Split('|')[1];

            foreach (var offset in knightOffsets)
            {
                int toIndex = fromIndex + offset;
                if (IsInsideBoard(toIndex) && !IsSameColor(toIndex, pieceColor) && IsValidKnightMove(fromIndex, toIndex))
                {
                    moves.Add(new Move(fromIndex, toIndex));
                }
            }

            return moves;
        }

        private List<Move> GetBishopMovesOLD(int fromIndex, string color)
        {
            return GetSlidingMovesOLD(fromIndex, color, new int[] { -9, -7, 7, 9 });
        }

        private List<Move> GetRookMovesOLD(int fromIndex, string color)
        {
            return GetSlidingMovesOLD(fromIndex, color, new int[] { -8, -1, 1, 8 });
        }

        private List<Move> GetQueenMovesOLD(int fromIndex, string color)
        {
            return GetSlidingMovesOLD(fromIndex, color, new int[] { -9, -7, -8, -1, 1, 7, 8, 9 });
        }

        private List<Move> GetKingMovesOLD(int fromIndex, string color)
        {
            var moves = new List<Move>();
            int[] kingOffsets = { -9, -8, -7, -1, 1, 7, 8, 9 };
            string pieceColor = _cases[fromIndex].Split('|')[1];

            foreach (var offset in kingOffsets)
            {
                int toIndex = fromIndex + offset;
                if (IsInsideBoard(toIndex) && !IsSameColor(toIndex, pieceColor) && IsValidKingMove(fromIndex, toIndex))
                {
                    moves.Add(new Move(fromIndex, toIndex));
                }
            }

            return moves;
        }

        private List<Move> GetSlidingMovesOLD(int fromIndex, string color, int[] directions)
        {
            var moves = new List<Move>();

            foreach (var direction in directions)
            {
                int currentIndex = fromIndex;

                while (true)
                {
                    currentIndex += direction;

                    // Vérifiez si l'indice est hors limites
                    if (!IsValidIndex(currentIndex) || IsOutOfBounds(fromIndex, currentIndex, direction))
                        break;

                    // Vérifiez si la case est vide
                    if (_cases[currentIndex] == "__")
                    {
                        moves.Add(new Move { FromIndex = fromIndex, ToIndex = currentIndex });
                    }
                    // Si une pièce adverse est présente
                    else if (_cases[currentIndex].EndsWith(GetOpponentColor(color)))
                    {
                        moves.Add(new Move { FromIndex = fromIndex, ToIndex = currentIndex });
                        break; // Une pièce adverse bloque le chemin après capture
                    }
                    else
                    {
                        break; // Une pièce alliée bloque le chemin
                    }
                }
            }

            return moves;
        }

        private bool IsOutOfBounds(int fromIndex, int toIndex, int direction)
        {
            // Vérifier si l'indice cible est hors des limites de l'échiquier
            if (toIndex < 0 || toIndex >= 64)
                return true;

            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;

            int toRow = toIndex / 8;
            int toCol = toIndex % 8;

            // Vérifier les déplacements horizontaux (gauche/droite) - direction ±1
            if (Math.Abs(direction) == 1 && fromRow != toRow)
                return true;

            // Vérifier les déplacements diagonaux et verticaux/horizontaux
            if (Math.Abs(direction) == 7 || Math.Abs(direction) == 9)
            {
                // Une diagonale doit correspondre à un changement simultané de rangée et de colonne
                if (Math.Abs(fromRow - toRow) != Math.Abs(fromCol - toCol))
                    return true;
            }

            // Aucun dépassement détecté
            return false;
        }

        private bool IsInsideBoard(int index)
        {
            return index >= 0 && index < 64;
        }

        private bool IsSameColor(int index, string color)
        {
            return _cases[index] != "__" && _cases[index].Contains($"|{color}");
        }

        private bool IsValidKnightMove(int fromIndex, int toIndex)
        {
            int rowDiff = Math.Abs((fromIndex / 8) - (toIndex / 8));
            int colDiff = Math.Abs((fromIndex % 8) - (toIndex % 8));
            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }

        private bool IsValidKingMove(int fromIndex, int toIndex)
        {
            int rowDiff = Math.Abs((fromIndex / 8) - (toIndex / 8));
            int colDiff = Math.Abs((fromIndex % 8) - (toIndex % 8));
            return rowDiff <= 1 && colDiff <= 1;
        }
    }

    public class NodeGPT
    {
        public List<NodeGPT> EquivalentBestNodeGPTList { get; set; }
        public List<NodeGPT> AllNodeGPTList { get; set; }
        public int Weight { get; set; }
        public int Level { get; set; }
        public string Colore { get; set; }
        public int FromIndex { get; set; } // Index de départ (0-63)
        public int ToIndex { get; set; } // Index d'arrivée (0-63)

        public string Location => GetPositionFromIndex(FromIndex); // Position d'origine en notation échiquier
        public string BestChildPosition => GetPositionFromIndex(ToIndex); // Position de destination en notation échiquier

        /// <summary>
        /// Convertit un index de l'échiquier (0-63) en position échiquier (ex : 0 => a8).
        /// </summary>
        public static string GetPositionFromIndex(int index)
        {
            if (index < 0 || index > 63)
                return null;

            char file = (char)('a' + (index % 8)); // 'a' à 'h'
            int rank = 8 - (index / 8); // '8' à '1'

            return $"{file}{rank}";
        }

        public override string ToString()
        {
            return $"{Level}:   {Location} ({FromIndex}) => {BestChildPosition} ({ToIndex}) : {Weight}".ToUpper();
        }

    }

    public class Move
    {
        public Move() { }
        public Move(int fromIndex, int toIndex)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }

    public class SpecificBoardGPT { /* Utilisé pour une logique avancée */ }
}
