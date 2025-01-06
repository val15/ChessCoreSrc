using System.Collections.Concurrent;

namespace ChessCore.Tools
{




    public class Chess2UtilsNotStaticOptimize : IDisposable
    {
        //public List<NodeGPTChess2> LevelBlackList { get; private set; }
        public int DeepLevel { get; set; } = Utils.DeepLevel;

        public NodeGPT GetBestPositionLocalUsingMiltiThreading(string colore, BoardGPT boardChess, bool isReprise, List<SpecificBoardGPT> specificBoardGPTList)
        {
            colore = colore.First().ToString();
            Utils.WritelineAsync($"DeepLever = {DeepLevel}");
            var startedReflectionTime = DateTime.UtcNow;
            var bestNodeGPTList = new List<NodeGPT>();
            var opponentColor = colore == "W" ? "B" : "W";
            var pawnIndices = boardChess.GetCasesIndexForColor(colore);
            NodeGPT bestNodeGPT = null;

            double alpha = double.NegativeInfinity;
            double beta = double.PositiveInfinity;

            Parallel.ForEach(pawnIndices, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, pawnIndex =>
            {

                var possibleMoves = boardChess.GetPossibleMoves(pawnIndex, 1);

                foreach (var move in possibleMoves)
                {
                    var clonedBoardGPT = boardChess.CloneAndMove(move.FromIndex, move.ToIndex, 1);
                    var node = MinMaxWithAlphaBeta(
                        board: clonedBoardGPT,
                        depth: DeepLevel,
                        alpha: alpha,
                        beta: beta,
                        maximizingPlayer: false,
                        cpuColor: opponentColor
                    );

                    node.FromIndex = move.FromIndex;
                    node.ToIndex = move.ToIndex;


                    lock (this)
                    {
                        Utils.WritelineAsync($"{node}");

                        bestNodeGPTList.Add(node);
                        if (bestNodeGPT == null || node.Weight > bestNodeGPT.Weight)
                        {
                            bestNodeGPT = node;
                            Utils.WritelineAsync($"{node} *");
                        }

                        alpha = Math.Max(alpha, node.Weight);
                    }
                }
            });

            bestNodeGPTList = bestNodeGPTList.Where(n => n?.Weight == bestNodeGPT.Weight).ToList();
            Utils.WritelineAsync($"bestNodeGPTList:");
            foreach (var node in bestNodeGPTList)
            {
                Utils.WritelineAsync($"{node}");
            }


            bestNodeGPT = bestNodeGPTList.Count == 1 ? bestNodeGPT : bestNodeGPTList.OrderBy(n => Guid.NewGuid()).First();
            bestNodeGPT.EquivalentBestNodeGPTList = bestNodeGPTList;
            var reflextionTime = DateTime.UtcNow - startedReflectionTime;
            Utils.WritelineAsync($"Reflection time : {reflextionTime}");

            Utils.WritelineAsync($"FinalBestNode =>  {bestNodeGPT}");


            return bestNodeGPT;
        }

        public NodeGPT MinMaxWithAlphaBeta(BoardGPT board, int depth, double alpha, double beta, bool maximizingPlayer, string cpuColor)
        {
            var currentNodeGPT = new NodeGPT { Level = depth, Colore = cpuColor };

            // Vérification de fin de recherche ou de fin de partie
            if (depth == 0 || board.IsGameOver())
            {
                currentNodeGPT.Weight = board.CalculateBoardGPTScore(cpuColor);

                // Si le roi est en échec dans la position actuelle
                if (board.IsKingInCheck(cpuColor))
                {
                    currentNodeGPT.Weight -= 10000; // Pénalité massive pour un roi en échec
                }

                return currentNodeGPT;
            }

            double bestValue = maximizingPlayer ? double.NegativeInfinity : double.PositiveInfinity;
            var moves = board.GetPossibleMovesForColor(cpuColor);

            // Boucle sur chaque coup possible
            foreach (var move in moves)
            {
                // Clone du plateau et application du coup
                var clonedBoardGPT = board.CloneAndMove(move.FromIndex, move.ToIndex, depth);

                // Vérification de la menace sur la pièce déplacée
                string movedPiece = board.GetCaseInIndex(move.FromIndex);
                string opponentColor = cpuColor == "W" ? "B" : "W";

                // Si la pièce déplacée devient menacée après le mouvement
                if (clonedBoardGPT.TargetIndexIsMenaced(clonedBoardGPT, cpuColor, opponentColor, move.ToIndex) > 0)
                {
                    int movedPieceValue = clonedBoardGPT.GetPieceValue(movedPiece.Split('|')[0]);
                    bestValue -= movedPieceValue; // Pénalité basée sur la valeur de la pièce
                }

                // Calcul du score du sous-nœud
                var childNodeGPT = MinMaxWithAlphaBeta(
                    board: clonedBoardGPT,
                    depth: depth - 1,
                    alpha: alpha,
                    beta: beta,
                    maximizingPlayer: !maximizingPlayer,
                    cpuColor: opponentColor);

                // Maximizing Player (CPU)
                if (maximizingPlayer)
                {
                    if (childNodeGPT.Weight > bestValue)
                    {
                        bestValue = childNodeGPT.Weight;
                        currentNodeGPT.ToIndex = move.ToIndex;
                        currentNodeGPT.FromIndex = move.FromIndex;
                    }

                    alpha = Math.Max(alpha, bestValue);
                }
                // Minimizing Player (Adversaire)
                else
                {
                    if (childNodeGPT.Weight < bestValue)
                    {
                        bestValue = childNodeGPT.Weight;
                        currentNodeGPT.ToIndex = move.ToIndex;
                        currentNodeGPT.FromIndex = move.FromIndex;
                    }

                    beta = Math.Min(beta, bestValue);
                }

                // Élagage Alpha-Beta
                if (beta <= alpha)
                {
                    break;
                }
            }

            currentNodeGPT.Weight = (int)bestValue;

            return currentNodeGPT;
        }



        public string GetOpponentColor(string color)
        {
            return color == "W" ? "B" : "W";
        }

        public NodeGPT MinMaxWithAlphaBetaNext(BoardGPT board, int depth, double alpha, double beta, bool maximizingPlayer, string cpuColor)
        {
            var currentNodeGPT = new NodeGPT { Level = depth, Colore = cpuColor };

            // Fin de recherche ou jeu terminé
            if (depth == 0 || board.IsGameOver())
            {
                currentNodeGPT.Weight = board.CalculateBoardGPTScore(cpuColor);

                // Si le roi ou la reine est en danger immédiat, appliquer une pénalité importante
                if (board.IsKingInCheck(cpuColor) /*|| board.IsQueenInDanger(cpuColor)*/)
                {
                    currentNodeGPT.Weight -= 10000;
                }

                return currentNodeGPT;
            }

            double bestValue = maximizingPlayer ? double.NegativeInfinity : double.PositiveInfinity;
            var moves = board.GetPossibleMovesForColor(cpuColor);

            foreach (var move in moves)
            {
                var clonedBoardGPT = board.CloneAndMove(move.FromIndex, move.ToIndex, depth);
                var opponentColor = cpuColor == "W" ? "B" : "W";

                // Vérification des menaces après le déplacement
                if (clonedBoardGPT.IsKingInCheck(cpuColor) /*|| clonedBoardGPT.IsQueenInDanger(cpuColor)*/)
                {
                    continue; // Ignorer ce mouvement
                }

                var childNodeGPT = MinMaxWithAlphaBeta(
                    board: clonedBoardGPT,
                    depth: depth - 1,
                    alpha: alpha,
                    beta: beta,
                    maximizingPlayer: !maximizingPlayer,
                    cpuColor: opponentColor);

                // Ajouter le bonus pour une capture
                var capturedPiece = board.GetCaseInIndex(move.ToIndex);
                if (capturedPiece != "__")
                {
                    string capturedPieceType = capturedPiece.Split('|')[0];
                    int capturedValue = clonedBoardGPT.GetPieceValue(capturedPieceType);
                    childNodeGPT.Weight += capturedValue * 3; // Bonus ajusté pour une capture
                }

                if (maximizingPlayer)
                {
                    if (childNodeGPT.Weight > bestValue)
                    {
                        bestValue = childNodeGPT.Weight;
                        currentNodeGPT.ToIndex = move.ToIndex;
                        currentNodeGPT.FromIndex = move.FromIndex;
                    }

                    alpha = Math.Max(alpha, bestValue);
                }
                else
                {
                    if (childNodeGPT.Weight < bestValue)
                    {
                        bestValue = childNodeGPT.Weight;
                        currentNodeGPT.ToIndex = move.ToIndex;
                        currentNodeGPT.FromIndex = move.FromIndex;
                    }

                    beta = Math.Min(beta, bestValue);
                }

                if (beta <= alpha)
                {
                    break;
                }
            }

            currentNodeGPT.Weight = (int)bestValue;
            return currentNodeGPT;
        }

   
        // Fonction pour vérifier si une case est protégée par une pièce amie
        private bool IsProtectedByFriendlyPiece(BoardGPT board, int index, string color)
        {
            var friendlyMoves = board.GetPossibleMovesForColor(color);
            foreach (var move in friendlyMoves)
            {
                if (move.ToIndex == index) // Si une pièce amie peut aller à cette case
                {
                    return true; // La case est protégée
                }
            }
            return false; // La case n'est pas protégée
        }

        // Fonction pour vérifier si une case est menacée par une pièce ennemie
        private bool IsThreatenedByOpponent(BoardGPT board, int index, string opponentColor)
        {
            var opponentMoves = board.GetPossibleMovesForColor(opponentColor);
            foreach (var move in opponentMoves)
            {
                if (move.ToIndex == index) // Si une pièce ennemie peut aller à cette case
                {
                    return true; // La case est menacée
                }
            }
            return false; // La case n'est pas menacée
        }


    
        public void Dispose()
        {
            GC.Collect();
        }
    }

    public class BoardGPT
    {

        public string[] _cases = new string[64];

        public BoardGPT(string[] cases)
        {
            _cases = cases;
        }
        public BoardGPT()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
        }

        public bool IsKingInCheck(string kingColor)
        {
            // Trouver l'index du roi
            int kingIndex = Array.FindIndex(_cases, piece => piece == $"K|{kingColor}");

            if (kingIndex == -1)
            {
                return true;
            }

            // Couleur opposée (ennemie)
            string opponentColor = kingColor == "W" ? "B" : "W";

            // Obtenir tous les mouvements possibles des pièces ennemies
            var opponentMoves = GetPossibleMovesForColor(opponentColor);

            // Vérifier si l'une des pièces ennemies menace le roi
            return opponentMoves.Any(move => move.ToIndex == kingIndex);
        }
        public string GetCaseInIndex(int index)
        {
            if (index < 0 || index >= _cases.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "L'index est hors des limites du plateau.");
            }

            return _cases[index]; // Retourne la chaîne décrivant la pièce sur cette case (par exemple, "P|W" ou "__").
        }
        public int GetPieceValue(string pieceType)
        {
            // Définir les valeurs des pièces
            Dictionary<string, int> pieceValues = new Dictionary<string, int>
    {
        { "P", 10 },  // Pion
        { "C", 30 },  // Cavalier
        { "B", 30 },  // Fou
        { "T", 50 },  // Tour
        { "Q", 90 },  // Reine
        { "K", 1000 } // Roi (valeur très élevée pour éviter sa capture)
    };

            // Retourner la valeur de la pièce
            return pieceValues.ContainsKey(pieceType) ? pieceValues[pieceType] : 0;
        }

        public void InsertPawn(int index, string pawnName, string color)
        {
            try
            {
                _cases[index] = $"{pawnName}|{color}";
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
                return;
            }
        }

        public void PrintInDebug()
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

        public List<int> GetCasesIndexForColor(string color)
        {
            var result = new List<int>();
            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i].EndsWith($"|{color}"))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        //public List<Move> GetPossibleMoves(int fromIndex, int depth)
        //{
        //    // Exemple simplifié, à adapter pour un vrai jeu d'échecs
        //    return new List<Move>
        //{
        //    new Move { FromIndex = fromIndex, ToIndex = (fromIndex + 1) % _cases.Length }
        //};
        //}

        public List<Move> GetPossibleMoves(int fromIndex, int depth)
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
                    moves.AddRange(GetRookMoves(fromIndex, pieceColor));
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


        private List<Move> GetQueenMoves(int fromIndex, string pieceColor)
        {
            List<Move> moves = new List<Move>();
            moves.AddRange(GetRookMoves(fromIndex, pieceColor)); // Combine les mouvements de la tour
            moves.AddRange(GetBishopMoves(fromIndex, pieceColor)); // et ceux du fou
            return moves;
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
        private bool IsWithinBounds(int index)
        {
            return index >= 0 && index < 64;
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







        private string GetOpponentColor(string color)
        {
            return color == "W" ? "B" : "W";
        }


        public List<Move> GetPossibleMovesForColor(string color)
        {
            var indices = GetCasesIndexForColor(color);
            var moves = new List<Move>();
            foreach (var index in indices)
            {
                moves.AddRange(GetPossibleMoves(index, 1));
            }
            return moves;
        }

        public BoardGPT CloneAndMove(int fromIndex, int toIndex, int depth)
        {
            var clonedCases = (string[])_cases.Clone();
            clonedCases[toIndex] = clonedCases[fromIndex];
            clonedCases[fromIndex] = "__";
            return new BoardGPT(clonedCases);
        }

        public bool IsGameOver()
        {
            // Simplifié : partie terminée si un roi est capturé
            return !_cases.Any(c => c.StartsWith("K|"));
        }


        public int CalculateBoardGPTScore(string color)
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
        { "K", 1000 } // Roi
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

                if (pieceValues.ContainsKey(pieceType))
                {
                    int baseValue = pieceValues[pieceType];
                    int positionalBonus = 0;
                    int threatPenalty = 0;

                    // Vérifier si la pièce est menacée par une pièce adverse
                    string opponentColor = GetOpponentColor(pieceColor);
                    if (TargetIndexIsMenaced(this, pieceColor, opponentColor, i) > 0)
                    {
                        // Appliquer un malus basé sur la valeur de la pièce menacée
                        threatPenalty -= baseValue;
                    }

                    // Bonus pour les pièces protégées
                    if (TargetIndexIsProtected(this, pieceColor, i) > 0)
                    {
                        positionalBonus += 5;
                    }

                    // Ajouter les scores au total pour chaque camp
                    if (pieceColor == "W")
                        whiteScore += baseValue + positionalBonus + threatPenalty;
                    else if (pieceColor == "B")
                        blackScore += baseValue + positionalBonus + threatPenalty;
                }
            }

            // Calcul du score final en fonction de la couleur donnée
            return color == "W" ? whiteScore - blackScore : blackScore - whiteScore;
        }


        //Ok pour T113
        public int CalculateBoardGPTScoreT113(string color)
        {
            int whiteScore = 0;
            int blackScore = 0;

            // Définir les valeurs des pièces
            Dictionary<string, int> pieceValues = new()
    {
        { "P", 10 },  // Pion
        { "C", 30 },  // Cavalier
        { "B", 30 },  // Fou
        { "T", 50 },  // Tour
        { "Q", 90 },  // Reine
        { "K", 1000 } // Roi
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

                if (pieceValues.ContainsKey(pieceType))
                {
                    int baseValue = pieceValues[pieceType];
                    int positionalBonus = 0;
                    int threatPenalty = 0;

                    // Vérifier les menaces pour cette pièce
                    int threats = TargetIndexIsMenaced(this, pieceColor, GetOpponentColor(pieceColor), i);

                    if (threats > 0)
                    {
                        // Appliquer un malus basé sur la valeur de la pièce
                        threatPenalty -= (int)(baseValue * 1.5); // Malus pour pièce menacée
                        if (pieceType == "K")
                        {
                            threatPenalty -= 500; // Malus additionnel pour le roi en danger
                        }
                        else if (pieceType == "Q")
                        {
                            threatPenalty -= 100; // Malus additionnel pour la reine en danger
                        }
                    }

                    // Vérifier si la pièce est protégée
                    int protections = TargetIndexIsProtected(this, pieceColor, i);
                    if (protections > 0)
                    {
                        positionalBonus += 10; // Bonus pour une pièce protégée
                    }

                    // Ajouter le score de la pièce
                    if (pieceColor == "W")
                        whiteScore += baseValue + positionalBonus + threatPenalty;
                    else
                        blackScore += baseValue + positionalBonus + threatPenalty;
                }
            }

            // Calcul du score final en fonction de la couleur donnée
            return color == "W" ? whiteScore - blackScore : blackScore - whiteScore;
        }


        public int CalculateBoardGPTScore115(string color)
        {
            int whiteScore = 0;
            int blackScore = 0;

            Dictionary<string, int> pieceValues = new()
    {
        { "P", 10 },
        { "C", 30 },
        { "B", 30 },
        { "T", 50 },
        { "Q", 90 },
        { "K", 1000 }
    };

            for (int i = 0; i < _cases.Length; i++)
            {
                string piece = _cases[i];

                if (piece == "__")
                    continue;

                string[] parts = piece.Split('|');
                string pieceType = parts[0];
                string pieceColor = parts[1];

                if (pieceValues.ContainsKey(pieceType))
                {
                    int baseValue = pieceValues[pieceType];
                    int positionalBonus = 0;
                    int threatPenalty = 0;

                    // Pénalités pour les pièces menacées
                    int threats = TargetIndexIsMenaced(this, pieceColor, GetOpponentColor(pieceColor), i);
                    if (threats > 0)
                    {
                        threatPenalty -= baseValue / 2;
                    }

                    // Bonus pour les pièces protégées
                    int protections = TargetIndexIsProtected(this, pieceColor, i);
                    if (protections > 0)
                    {
                        positionalBonus += 20;
                    }

                    // Bonus pour les pions proches de la promotion
                    if (pieceType == "P")
                    {
                        positionalBonus += pieceColor == "W" ? i / 8 * 2 : (7 - i / 8) * 2;
                        if ((pieceColor == "W" && i / 8 == 6) || (pieceColor == "B" && i / 8 == 1))
                        {
                            positionalBonus += 20;
                        }
                    }

                    if (pieceColor == "W")
                    {
                        whiteScore += baseValue + positionalBonus + threatPenalty;
                    }
                    else
                    {
                        blackScore += baseValue + positionalBonus + threatPenalty;
                    }
                }
            }

            return color == "W" ? whiteScore - blackScore : blackScore - whiteScore;
        }



        public int TargetIndexIsMenaced(BoardGPT board, string pieceColor, string opponentColor, int index)
        {
            var attackers = board.GetPossibleMovesForColor(opponentColor)
                                 .Count(move => move.ToIndex == index);
            return attackers;
        }

        public int TargetIndexIsProtected(BoardGPT board, string color, int index)
        {
            var allyColor = color;
            var defenders = board.GetPossibleMovesForColor(allyColor)
                                 .Count(move => move.ToIndex == index);
            return defenders;
        }


      
    }

    public class NodeGPT
    {

        public List<NodeGPT> EquivalentBestNodeGPTList { get; set; }
    public NodeChess2 AsssociateNodeChess2 { get; set; } = new NodeChess2();

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
            return $"{Location} ({FromIndex}) => {BestChildPosition} ({ToIndex}) : {Weight}".ToUpper();
        }

    }



    public class Move
    {
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public Move(int fromIndex, int toIndex)
        {
            FromIndex = fromIndex; ToIndex = toIndex;
        }
    }


    public class SpecificBoardGPT
    {
        // Classe vide pour adapter au besoin
    }

}
