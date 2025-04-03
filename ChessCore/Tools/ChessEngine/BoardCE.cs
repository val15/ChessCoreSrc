using Newtonsoft.Json.Linq;
using System;

namespace ChessCore.Tools.ChessEngine
{
    public class BoardCE
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
                    var index = x + y * 8;
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Utils.WritelineAsync(line);
            }
            Utils.WritelineAsync("_____________________________________________________________________");
        }
        public string ToString()
        {
            var result = "";
            foreach (var caseContaint in _cases.ToList())
            {
                result += caseContaint+";";

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


        public BoardCE()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
        }

        public BoardCE CopyFrom(BoardCE boardCE)
        {
            return new BoardCE(boardCE);
        }
        public BoardCE(string[] cases)
        {
            _cases = cases;
        }

        public BoardCE(BoardCE other)
        {
            Array.Copy(other._cases, _cases, 64);
        }
        public int GetKingIndex(string color)
        {
            for (int i = 0; i < _cases.Count(); i++)
            {
                var c = _cases[i];
                if (c.StartsWith("K") && c.EndsWith(color))
                    return i;

            }
            return -1;
        }

        public void InsertPawn(int index, string pieceType, string color)
        {
            _cases[index] = $"{pieceType}|{color}";
        }


        public void Move(Move move)
        {
            Move(move.FromIndex, move.ToIndex);
        }
        public void Move(int fromIndex, int toIndex)
        {
            _cases[toIndex] = _cases[fromIndex];
            _cases[fromIndex] = "__";
        }
        public void MakeMove(Move move)
        {
            MakeMove(move.FromIndex, move.ToIndex);
        }

        public void MakeMove(int fromIndex, int toIndex)
        {
            // Déplacer la pièce de fromIndex à toIndex
            _cases[toIndex] = _cases[fromIndex];
            _cases[fromIndex] = "__";

            // Gérer la promotion des pions
            if (_cases[toIndex].StartsWith("P"))
            {
                // Promotion pour les pions blancs
                if (toIndex >= 0 && toIndex <= 7)
                {
                    _cases[toIndex] = _cases[toIndex].Replace("P", "Q"); // Promotion en reine
                }
                // Promotion pour les pions noirs
                else if (toIndex >= 56 && toIndex <= 63)
                {
                    _cases[toIndex] = _cases[toIndex].Replace("P", "Q"); // Promotion en reine
                }
            }
        }
        public int GetMenacedsPoints(string color)
        {
            var result = 0;
            var opponentColor = GetOpponentColor(color);

            var indexList = GetCasesIndexForColor(color);

            Parallel.ForEach(indexList, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, index =>
            //foreach (var index in indexList)
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
            try
            {
                //GET IN IsKingInCheckList
                ////var key = Utils.GenerateKeyForIsKingInCheck(Utils.CasesToCasesString(_cases), color);
                ////if (Utils.IsKingInCheckList.TryGetValue(key, out var inChessFromList))
                ////{

                ////    return inChessFromList.IsInChess;
                ////}

                // Identifier l'opposant
                string opponentColor = GetOpponentColor(color);

                // Trouver l'index du roi de la couleur donnée
                int kingIndex = Array.FindIndex(_cases, piece => piece == $"K|{color}");

                if (kingIndex == -1)
                {

                    //SAVE IN CHESS 
                    //  AddInIsKingInCheckList(new IsKingInCheck(_cases, color, true));
                    return true;
                }


                //si le roi adverse peux encore bouger ou pas
                //si ces dirrections son menacé ou non
                //TODO TO DELATE IF ALL OK  var kingPosibleMoves = GetPossibleMoves(kingIndex);
                var kingPosibleMoves = GetPossibleMovesOLD(kingIndex);
                if (kingPosibleMoves.Count > 0)
                {
                    foreach (var kingMove in kingPosibleMoves)
                    {
                        if (kingMove.ToIndex == 18)
                        {
                            var fdf = 0;
                        }
                        var copyBoard = CloneAndMove(kingMove.FromIndex, kingMove.ToIndex);
                        if (!copyBoard.TargetIndexIsMenaced(kingMove.ToIndex, opponentColor))
                        {
                            //SAVE IN CHESS 
                            // AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
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

                       // if(move.ToIndex == )
                        
                        //si un de ses mouvement est dans tokingIndexPath
                        foreach (var toOpponentKingPathIndex in toOpponentKingPathIndexList)
                        {
                            if (alierPossibleMoves.Select(x => x.ToIndex).Contains(toOpponentKingPathIndex))
                            {
                                //SAVE IN CHESS 
                                //AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
                                return false;
                            }

                        }

                        //si celui qui menace est menacée en non pas par le roi
                        var indexOfOpponentsWhoThreatenList = GetMovesOfOpponentsWhoThreaten(move.FromIndex, color);
                        //on enleve l'index du roir nenacé
                        indexOfOpponentsWhoThreatenList.RemoveAll(x => x.FromIndex == kingIndex);
                        if (indexOfOpponentsWhoThreatenList.Count > 0)
                        {
                            //SAVE IN CHESS 
                            //AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
                            return false;
                        }
                        //SAVE IN CHESS
                        //AddInIsKingInCheckList(new IsKingInCheck(_cases, color, true));
                        return true; // Le roi est en échec
                    }
                }

                //SAVE IN CHESS
                // AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
                return false; // Le roi n'est pas en échec
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                // Utils.GCColect();
            }

        }

        public List<Move> GetPossibleMovesForColor(string color, bool chessIsInChess = false)
        {
            var moves = new List<Move>();
            var indicesForColor = GetCasesIndexForColor(color);

            foreach (var fromIndex in indicesForColor)
            {
                moves.AddRange(GetPossibleMovesOLD(fromIndex));
            }

            //si pour un mouvement, le roi est menacé, ce mouvement n'est pas possible
            if (chessIsInChess)
            {
                for (int i = 0; i < moves.Count; i++)
                {


                    var move = moves[i];
                    var cloanBord = this.CloneAndMove(move);
                    if (cloanBord.KingIsMenaced(color))
                        moves.Remove(move);
                }
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
                "T" => 60, // Tour // Tour 50 mais pour T126 et T125
                "Q" => 90, // Reine
                "K" => 999, // Roi 10000 to 100 for T136C_W_D5toF6 et T136B_W_D5toF6 Roi
                _ => 0,
            };
        }

        public int GetPieceValue(int index)
        {
            return GetPieceValue(_cases[index].First().ToString());
        }
        //public void AddInIsKingInCheckList(IsKingInCheck isKingInCheck)
        //{
        //    //TODO A DECOMMENTER

        //    //   Générer la clé
        //    //var key = Utils.GenerateKeyForIsKingInCheck(isKingInCheck.CasesToString, isKingInCheck.KingColor);

        //    //// Ajouter uniquement si la clé n'existe pas déjà
        //    //if (!Utils.IsKingInCheckList.ContainsKey(key))
        //    //{
        //    //    Utils.IsKingInCheckList.TryAdd(key, isKingInCheck);
        //    //}
        //}

        private static readonly int[] centralSquares = { 28, 29, 36, 37, 44, 45, 52, 53 };

        public int CalculateBoardCEScore(string color, string opponentColor)
        {

            int whiteScore = 0;
            int blackScore = 0;

            // Définir les valeurs des pièces
            Dictionary<string, int> pieceValues = new Dictionary<string, int>
    {
        { "P", 10 },  // Pion
        { "C", 30 },  // Cavalier
        { "B", 30 },  // Fou
        { "T", 60 },  // Tour 50 mais pour T126 et T125
        { "Q", 90 },  // Reine
        { "K", 999 } // 10000 to 100 for T136C_W_D5toF6 et T136B_W_D5toF6 Roi (valeur arbitraire très élevée pour éviter sa capture)
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
                    {
                        whiteScore += pieceValues[pieceType] + positionalBonus;

                    }

                    else if (pieceColor == "B")
                    {
                        blackScore += pieceValues[pieceType] + positionalBonus;

                    }

                }
            }

            ////T110WhiteKingNoToE1
            //value -= board.GetMenacedsPoints(opponentColor);
            //currentNode.Weight = value;

            // Calcul du score en fonction de la couleur donnée
            return color == "W" ? whiteScore - blackScore : blackScore - whiteScore;

        }

        public BoardCE Evolution(BoardCE boardCE, int index)
        {
            if (boardCE._cases[index].First() != 'P')
                return boardCE;

            if ((index >= 56 && index <= 63)
                ||
                (index >= 0 && index <= 7))
            {
                boardCE._cases[index] = boardCE._cases[index].Replace("P", "Q");//EVOLUTION
            }


            return boardCE;

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

            if (index == -1)
                return result;
            var color = GetOpponentColor(opponentColor);
            //T67WhiteIsInChess
            //on vide la case de l'index
            var oldContaine = _cases[index];
            if (_cases[index].EndsWith(opponentColor))
                _cases[index] = _cases[index].Replace($"|{_cases[index].Last()}", $"|{color}");


            // Récupère les indices des pièces adverses
            var opponentPawnIndexList = GetCasesIndexForColor(opponentColor);


            // Utilisation de Parallel.ForEach pour paralléliser les itérations
            Parallel.ForEach(opponentPawnIndexList, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (opponentPawnIndex, state) =>
            {
                // Récupère les mouvements possibles pour chaque pièce adverse

                var opponentPossiblesMoves = GetPossibleMovesOLD(opponentPawnIndex);

                // Vérifie si un mouvement menace la case cible

                foreach (var enemyMove in opponentPossiblesMoves)
                //Parallel.ForEach(opponentPossiblesMoves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, enemyMove =>
                {
                    if (enemyMove.ToIndex == index)
                    {
                        //T95SuiteSuiteB_W_InChess
                        var cloneBoard = CloneAndMove(enemyMove);
                        if (!cloneBoard.KingIsMenaced(opponentColor))
                            result.Add(enemyMove);
                        //isMenaced = true;
                        //state.Stop(); // Arrête toutes les autres itérations
                        //  _cases[index] = oldContaine;
                        //break;
                    }
                }//);
            });
            _cases[index] = oldContaine;
            //foreach (var move in result)
            //{
            //    var cloneBoard = CloneAndMove(move);
            //    if (cloneBoard.KingIsMenaced(opponentColor))
            //    {
            //        var f = move;
            //        result = result.Remove(move);
            //    }
            //}
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
            var allierPawnIndexList = GetCasesIndexForColor(allierColor);
            foreach (var opponentPawnIndex in allierPawnIndexList)
            {


                //TODO TO DELATE IF ALL OK foreach (var enemyMove in GetPossibleMoves(opponentPawnIndex))
                foreach (var enemyMove in GetPossibleMovesOLD(opponentPawnIndex))
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
        public bool IsGameOverTow()
        {
            var kCount = _cases.Where(c => c.StartsWith("K|")).Count();
            return ! (kCount != 2) ;
        }

        public BoardCE CloneAndMove(int fromIndex, int toIndex)
        {
            var clone = new BoardCE(this);
            clone.Move(fromIndex, toIndex);

            //evoution
            clone = clone.Evolution(clone, toIndex);
            return clone;
        }
        public BoardCE CloneAndMove(Move move)
        {

            return CloneAndMove(move.FromIndex, move.ToIndex);
        }
        public BoardCE Clone()
        {
            return new BoardCE(this);
        }

        public List<int> GetCasesIndexForColor(string color)
        {
            var indices = new List<int>();
            for (int i = 0; i < _cases.Length; i++)
                if (_cases[i].EndsWith($"|{color}"))
                    indices.Add(i);
            return indices;
        }
        public List<Move> GetPossibleMovesTODELETE(int fromIndex)
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
            int forwardIndex = fromIndex + 8 * direction;
            if (IsWithinBounds(forwardIndex) && _cases[forwardIndex] == "__")
            {
                moves.Add(new Move(fromIndex, forwardIndex, this));
            }

            // Mouvement double depuis la position initiale
            if (pieceColor == "W" && fromRow == 6 || pieceColor == "B" && fromRow == 1)
            {
                int doubleForwardIndex = fromIndex + 16 * direction;
                if (IsWithinBounds(doubleForwardIndex) && _cases[forwardIndex] == "__" && _cases[doubleForwardIndex] == "__")
                {
                    moves.Add(new Move(fromIndex, doubleForwardIndex, this));
                }
            }

            // Captures diagonales
            foreach (int diagOffset in new[] { -1, 1 })
            {
                int diagCol = fromCol + diagOffset;
                int diagIndex = forwardIndex + diagOffset;

                if (IsWithinBounds(diagIndex) && diagCol >= 0 && diagCol < 8 && _cases[diagIndex] != "__" && !_cases[diagIndex].EndsWith(pieceColor))
                {
                    moves.Add(new Move(fromIndex, diagIndex,this));
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
                        moves.Add(new Move(fromIndex, currentIndex,this));
                    }
                    else
                    {
                        if (!_cases[currentIndex].EndsWith(pieceColor)) // Capture
                        {
                            moves.Add(new Move(fromIndex, currentIndex, this));
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
                if(direction == -1)
                {
                    var fdf = 0;
                }
                List<int> pathToOpponentKingIndex = new List<int>();
                int currentIndex = fromIndex;
                while (true)
                {
                    currentIndex += direction;

                    if (currentIndex == opponentKingIndex)
                        return pathToOpponentKingIndex;

                    if (!IsWithinBounds(currentIndex) || IsEdgeCase(fromIndex, currentIndex, direction))
                    {
                        var df = pathToOpponentKingIndex;
                     //   return pathToOpponentKingIndex;
                        break;
                    }
                       

                    if (_cases[currentIndex] == "__")
                    {
                        pathToOpponentKingIndex.Add(currentIndex);

                    }
                    else
                    {
                        if (!_cases[currentIndex].EndsWith(pieceColor)) // Capture
                        {
                            pathToOpponentKingIndex.Add(currentIndex);
                          
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
                    moves.Add(new Move(fromIndex, newIndex, this));
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
                        moves.Add(new Move(fromIndex, currentIndex, this));
                    }
                    else
                    {
                        // Si la case contient une pièce adverse
                        if (!_cases[currentIndex].EndsWith(pieceColor))
                        {
                            moves.Add(new Move(fromIndex, currentIndex, this));
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

                if (currentIndex == opponentKingIndex)
                    return pathToOpponentKingIndex;

                while (true)
                {
                    int currentRow = currentIndex / 8;
                    int currentCol = currentIndex % 8;

                    currentIndex += direction;

                    if (currentIndex == opponentKingIndex)
                        return pathToOpponentKingIndex;

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
                            //if (currentIndex == opponentKingIndex)
                            //    return pathToOpponentKingIndex;
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
                    moves.Add(new Move(fromIndex, targetIndex, this));
                }
            }

            return moves;
        }


        public List<Move> GetPossibleMovesOLD(int fromIndex)
        {
            //GET POSSIBLE MOVE IN LIST
            ////var key = Utils.GenerateKeyForPossibleMoves(Utils.CasesToCasesString(_cases), fromIndex);
            ////if (Utils.PossibleMovesList.TryGetValue(key, out var possibleMoveRAMPossibleMovesList))
            ////{
            ////    return possibleMoveRAMPossibleMovesList.PossibleMovesResult;
            ////}


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
            int direction = color == "W" ? -1 : 1; // Les pions blancs avancent vers le haut, les noirs vers le bas
            int row = fromIndex / 8;
            int col = fromIndex % 8;

            // Mouvement de base (un pas en avant)
            int forwardIndex = fromIndex + direction * 8;
            if (IsValidIndex(forwardIndex) && _cases[forwardIndex] == "__")
            {
                moves.Add(new Move(fromIndex, forwardIndex, this));
            }

            // Mouvement initial (deux pas en avant)
            if (color == "W" && row == 6 || color == "B" && row == 1)
            {
                int doubleForwardIndex = fromIndex + direction * 16;
                if (IsValidIndex(doubleForwardIndex) && _cases[doubleForwardIndex] == "__" && _cases[forwardIndex] == "__")
                {
                    moves.Add(new Move(fromIndex, doubleForwardIndex, this));
                }
            }

            // Capture en diagonale gauche
            if (col > 0)
            {
                int captureLeftIndex = fromIndex + direction * 8 - 1;
                if (IsValidIndex(captureLeftIndex) && _cases[captureLeftIndex] != "__")
                {
                    string targetColor = _cases[captureLeftIndex].Split('|')[1];
                    if (targetColor != color) // Vérifie que la cible appartient à l'adversaire
                    {
                        moves.Add(new Move(fromIndex, captureLeftIndex, this));
                    }
                }
            }

            // Capture en diagonale droite
            if (col < 7)
            {
                int captureRightIndex = fromIndex + direction * 8 + 1;
                if (IsValidIndex(captureRightIndex) && _cases[captureRightIndex] != "__")
                {
                    string targetColor = _cases[captureRightIndex].Split('|')[1];
                    if (targetColor != color) // Vérifie que la cible appartient à l'adversaire
                    {
                        moves.Add(new Move(fromIndex, captureRightIndex, this));
                    }
                }
            }
            //SAVE POSSIBLE MOVE IN LIST
            // Utils.PossibleMovesList.Add(new PossibleMoves(_cases, fromIndex, moves));
            // Générer la clé
            ////var key = Utils.GenerateKeyForPossibleMoves(Utils.CasesToCasesString(_cases), fromIndex);
            ////// Ajouter uniquement si la clé n'existe pas déjà
            ////if (!Utils.PossibleMovesList.ContainsKey(key))
            ////{
            ////    Utils.PossibleMovesList.TryAdd(key, new PossibleMoves(_cases, fromIndex, moves));
            ////}
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
                    moves.Add(new Move(fromIndex, toIndex, this));
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
                    moves.Add(new Move(fromIndex, toIndex, this));
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
            int rowDiff = Math.Abs(fromIndex / 8 - toIndex / 8);
            int colDiff = Math.Abs(fromIndex % 8 - toIndex % 8);
            return rowDiff == 2 && colDiff == 1 || rowDiff == 1 && colDiff == 2;
        }

        private bool IsValidKingMove(int fromIndex, int toIndex)
        {
            int rowDiff = Math.Abs(fromIndex / 8 - toIndex / 8);
            int colDiff = Math.Abs(fromIndex % 8 - toIndex % 8);
            return rowDiff <= 1 && colDiff <= 1;
        }
    }

}
