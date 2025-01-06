using System;
using System.Diagnostics;

namespace ChessCore.Tools
{
    public class Board
    {

        #region Attribus
        private string[] _cases = new string[64];//index:coord|pawnName(sp)|colore

        //T86 A DECOMMENTER SI ON VEUT VOIR UNE FUITE DE MEMOIR
        /* private int[] _evolutionPawnIndexBlack =
        {
          56,57,58,59,60,61,62,63
        };
        private int[] _evolutionPawnIndexWhite =
        {
          0,1,2,3,4,5,6,7
        };

        private string[] _coord = {
    "a8","b8","c8","d8","e8","f8","g8","h8",
    "a7","b7","c7","d7","e7","f7","g7","h7",
    "a6","b6","c6","d6","e6","f6","g6","h6",
    "a5","b5","c5","d5","e5","f5","g5","h5",
    "a4","b4","c4","d4","e4","f4","g4","h4",
    "a3","b3","c3","d3","e3","f3","g3","h3",
    "a2","b2","c2","d2","e2","f2","g2","h2",
    "a1","b1","c1","d1","e1","f1","g1","h1"
        };*/
        #endregion
        #region Properties

        public int Weight { get; set; }
        public List<string> MovingList { get; set; }//pour l'historique de déplacement
        public List<string> HuntingBoardWhiteImageList { get; set; }//pour la version web
        public List<string> HuntingBoardBlackImageList { get; set; }//pour la version web
        public string[] GetCases()
        {
            return _cases;
        }
        public void SetCases(int index, string containt)
        {
            _cases[index] = containt;
        }
        public string GetPawnShortNameInIndex(int index)
        {
            var currentCase = _cases[index];
            if (!currentCase.Contains("|"))
                return "empty";
            return currentCase.Split('|')[0].ToString();
        }
        public string GetPawnColorNameInIndex(int index)
        {
            var currentCase = _cases[index];
            if (!currentCase.Contains("|"))
                return "empty";
            return currentCase.Split('|')[1].ToString();
        }

        public bool DestinationIndexIsMenaced { get; set; } = false;
        public int Diff { get; set; }
        public int WhiteScore { get; set; }
        public int BlackScore { get; set; }
        public int Level { get; set; }

        /// <summary>
        /// retourne tous les index de la couleur
        /// </summary>
        public List<int> GetCasesIndexForColor(string colore)
        {
            colore = colore.First().ToString();
            var indexAndCaseList = new List<IndexAndCase>();

            for (int i = 0; i < _cases.Count(); i++)
            {

                indexAndCaseList.Add(new IndexAndCase { Index = i, CaseContain = _cases[i] });
            }

            //on ordone les pieces celont leur rang
            var caseInOrder = indexAndCaseList.Where(x => x.CaseContain.Contains($"|{colore}")).ToList().OrderByDescending(x => this.GetValue(x.CaseContain));
            //foreach (var item in _cases)

            return caseInOrder.Select(x => x.Index).ToList();

        }
        /// <summary>
        /// tsiry;16-07-2022
        /// cette classe n'est util que dans la methode GetCasesIndex
        /// pour ordoner les cases selont les rang des pieces
        /// </summary>
        class IndexAndCase
        {
            public int Index { get; set; }
            public string CaseContain { get; set; }
        }

        /*tsiry;07-01-2022
        * returne tout les index sauf 
        * */
        public int[] GetCasesAllIndexExcept(string colore, string exceptPawnName)
        {
            List<int> results = new List<int>();
            //foreach (var item in _cases)
            for (int i = 0; i < _cases.Count(); i++)
            {
                var item = _cases[i];
                if (item.Contains("|"))
                {
                    var caseDatas = item.Split('|');
                    var caseColor = caseDatas[1];

                    var caseName = caseDatas[0];
                    if (caseColor == colore && caseName != exceptPawnName)
                        results.Add(i);
                }


            }

            return results.ToArray();
        }

        /*tsiry;07-01-2022
         * pour ne prendre d'une index
         * */
        public int GetOneIndex(string colore, string pawnName)
        {

            //foreach (var item in _cases)
            for (int i = 0; i < _cases.Count(); i++)
            {
                var item = _cases[i];
                if (item.Contains("|"))
                {
                    var caseDatas = item.Split('|');
                    var caseColor = caseDatas[1];

                    var caseName = caseDatas[0];
                    if (caseColor == colore && caseName == pawnName)
                        return i;


                }
            }
            return -1;


        }
        #endregion

        #region Methodes

        /// <summary>
        /// Vérifie si la partie est terminée (par exemple, un roi est capturé ou aucun coup n'est possible).
        /// </summary>
        public bool IsGameOver()
        {
            // Exemple simplifié : la partie est terminée si un des rois n'est plus sur le plateau.
            var whiteKingIndex = Array.IndexOf(_cases, "K|W");
            var blackKingIndex = Array.IndexOf(_cases, "K|B");

            return whiteKingIndex == -1 || blackKingIndex == -1;
        }

        /// <summary>
        /// Retourne tous les mouvements possibles pour une couleur donnée.
        /// </summary>
        public List<PossibleMove> GetPossibleMovesForColor(string color)
        {
            var moves = new List<PossibleMove>();
            var indices = GetCasesIndexForColor(color);

            foreach (var index in indices)
            {
                moves.AddRange(GetPossibleMoves(index, level: 1)); // Utilise la méthode existante pour obtenir les mouvements possibles.
            }

            return moves;
        }

        /// <summary>
        /// Clone le plateau et effectue un mouvement donné.
        /// </summary>
        public Board CloneAndMove(int fromIndex, int toIndex, int depth)
        {
            var clonedBoard = new Board(this); // Clone le plateau.
            clonedBoard.Move(fromIndex, toIndex); // Applique le mouvement.
            return clonedBoard;
        }

        /// <summary>
        /// Calcule le score du plateau pour une couleur donnée.
        /// </summary>
        public int CalculateBoardScore(string color, NodeChess2 node = null)
        {
            // Convertir la couleur en format court ("W" ou "B")
            color = color.First().ToString();

            // Appeler la méthode CalculeScores avec le nœud actuel
            CalculeScores(color,node);

            // Retourner la différence des scores en fonction de la couleur
            return color == "W" ? WhiteScore - BlackScore : BlackScore - WhiteScore;
        }


        /*tsiry;03-01-2022
         * */
        public void LoadFromDirectorie(string dirLocation)
        {
            try
            {



                var whiteFileLocation = dirLocation + "/WHITEList.txt";
                var blackFileLocation = dirLocation + "/BlackList.txt";






                var readText = File.ReadAllText(whiteFileLocation);

                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        //  public Pawn(string name, string location, Button associateButton, string colore, MainWindow mainWindowParent)


                        var datas = line.Split(';');
                        // var bt = (Button)this.FindName(datas[1]);
                        var index = Utils.GetIndexFromLocation(datas[1]);//datas[1] = location
                        this.InsertPawn(index, Utils.ChangeLongNameToShortName(datas[0]), "W");


                    }
                }

                readText = File.ReadAllText(blackFileLocation);

                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var datas = line.Split(';');
                        // var bt = (Button)this.FindName(datas[1]);
                        var index = Utils.GetIndexFromLocation(datas[1]);//datas[1] = location
                        this.InsertPawn(index, Utils.ChangeLongNameToShortName(datas[0]), "B");

                    }
                }





            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.Message);
                return;
            }

        }

        public void CalculeScoresERROR(string color,NodeChess2 node = null)
        {

            color = color.First().ToString();
            // Initialisation des scores
            int whiteBonus = 0;
            int blackBonus = 0;

            // Détection des pions évolutifs
            if (Utils.GetEvolutionPawnIndexWhite().Any(index => _cases[index].Contains("P")))
                whiteBonus += 90;

            if (Utils.GetEvolutionPawnIndexBlack().Any(index => _cases[index].Contains("P")))
                blackBonus += 90;

            // Compte des pièces et calcul des scores
            WhiteScore = CalculateScore("W") + whiteBonus;
            BlackScore = CalculateScore("B") + blackBonus;

            // Calcul des menaces si le nœud est fourni
            if (node != null && node.Level < Utils.DeepLevel && Utils.DeepLevelPrime)
            {
                EvaluateThreats(node);
            }

            // Calcul du poids final
            if (color == "B")
                Weight = BlackScore - WhiteScore;
            else
                Weight = WhiteScore - BlackScore;

            // Différence absolue pour information
            Diff = Math.Abs(WhiteScore - BlackScore);
        }
        public void CalculeScores(string color,NodeChess2 node = null)
        {
            int whiteBonus = 0;
            int blackBonus = 0;

            // Vérifie la présence de pions blancs évolutifs
            bool hasWhiteEvolutionPawn = Utils.GetEvolutionPawnIndexWhite().Any(evolutionPawnIndex => _cases[evolutionPawnIndex].Contains("P"));
            if (hasWhiteEvolutionPawn)
                whiteBonus = 90;

            // Vérifie la présence de pions noirs évolutifs
            bool hasBlackEvolutionPawn = Utils.GetEvolutionPawnIndexBlack().Any(evolutionPawnIndex => _cases[evolutionPawnIndex].Contains("P"));
            if (hasBlackEvolutionPawn)
                blackBonus = 90;

            // Compte des pièces
            int whitePawnNumber = _cases.Count(x => x == "P|W");
            int blackPawnNumber = _cases.Count(x => x == "P|B");
            int whiteBishopNumber = _cases.Count(x => x == "B|W");
            int blackBishopNumber = _cases.Count(x => x == "B|B");
            int whiteKnightNumber = _cases.Count(x => x == "C|W");
            int blackKnightNumber = _cases.Count(x => x == "C|B");
            int whiteRookNumber = _cases.Count(x => x == "T|W");
            int blackRookNumber = _cases.Count(x => x == "T|B");
            int whiteQueenNumber = _cases.Count(x => x == "Q|W");
            int blackQueenNumber = _cases.Count(x => x == "Q|B");
            int whiteKingNumber = _cases.Count(x => x == "K|W");
            int blackKingNumber = _cases.Count(x => x == "K|B");

            // Calcul des scores basiques
            WhiteScore = whitePawnNumber * 10
                         + whiteBishopNumber * 30
                         + whiteKnightNumber * 30
                         + whiteRookNumber * 50
                         + whiteQueenNumber * 90
                         + whiteKingNumber * 100
                         + whiteBonus;

            BlackScore = blackPawnNumber * 10
                         + blackBishopNumber * 30
                         + blackKnightNumber * 30
                         + blackRookNumber * 50
                         + blackQueenNumber * 90
                         + blackKingNumber * 100
                         + blackBonus;

            // Analyse des menaces (nœud fourni ou plateau actuel)
            var allWhitePieces = GetCasesIndexForColor("W");
            var allBlackPieces = GetCasesIndexForColor("B");

            foreach (var whiteIndex in allWhitePieces)
            {
                if (TargetIndexIsMenaced(this, "W", "B", whiteIndex) > 0)
                {
                    int pieceValue = GetValue(GetCaseInIndex(whiteIndex));
                    BlackScore += pieceValue / 2; // Les noirs profitent de la menace
                    WhiteScore -= pieceValue / 2; // Les blancs sont pénalisés
                }
            }

            foreach (var blackIndex in allBlackPieces)
            {
                if (TargetIndexIsMenaced(this, "B", "W", blackIndex) > 0)
                {
                    int pieceValue = GetValue(GetCaseInIndex(blackIndex));
                    WhiteScore += pieceValue / 2; // Les blancs profitent de la menace
                    BlackScore -= pieceValue / 2; // Les noirs sont pénalisés
                }
            }

            // Calcul du poids final
            if (color == "B")
                Weight = BlackScore - WhiteScore;
            else
                Weight = WhiteScore - BlackScore;
        }
        public int TargetIndexIsMenaced(Board board, string targetColor, string opponentColor, int targetIndex)
        {
            // Récupérer toutes les positions des pièces de l'adversaire
            var opponentPieces = board.GetCasesIndexForColor(opponentColor);

            int menaceScore = 0;

            foreach (var opponentIndex in opponentPieces)
            {
                // Récupérer les mouvements possibles pour la pièce adverse
                var possibleMoves = board.GetPossibleMoves(opponentIndex, 1);

                foreach (var move in possibleMoves)
                {
                    // Vérifie si le mouvement menace la position cible
                    if (move.ToIndex == targetIndex)
                    {
                        menaceScore += board.GetValue(board.GetCaseInIndex(opponentIndex));
                        break; // Une seule menace par pièce suffit
                    }
                }
            }

            return menaceScore; // Retourne la somme des valeurs des pièces menaçant la cible
        }


        /// <summary>
        /// Calcule le score d'une couleur donnée en fonction des pièces présentes sur le plateau.
        /// </summary>
        private int CalculateScore(string color)
        {
            int score = 0;
            foreach (var piece in _cases)
            {
                if (!piece.Contains($"|{color}")) continue;

                string pieceType = piece.Split('|')[0]; // Extrait le type de la pièce (P, T, B, etc.)
                score += GetValue(pieceType);
            }
            return score;
        }

        /// <summary>
        /// Évalue les menaces sur les pièces pour les deux couleurs.
        /// </summary>
        private void EvaluateThreats(NodeChess2 node)
        {
            var opponentColor = Utils.ComputerColor == "W" ? "B" : "W";

            // Menaces sur les pièces blanches
            foreach (var index in GetCasesIndexForColor("W"))
            {
                int threatLevel = node.TargetIndexIsMenaced(this, "W", "B", index);
                if (threatLevel > 0)
                {
                    int pieceValue = GetValue(GetCaseInIndex(index));
                    BlackScore += pieceValue / 10; // Ajustez le poids des menaces ici
                }
            }

            // Menaces sur les pièces noires
            foreach (var index in GetCasesIndexForColor("B"))
            {
                int threatLevel = node.TargetIndexIsMenaced(this, "B", "W", index);
                if (threatLevel > 0)
                {
                    int pieceValue = GetValue(GetCaseInIndex(index));
                    WhiteScore += pieceValue / 10; // Ajustez le poids des menaces ici
                }
            }
        }

        /// <summary>
        /// Retourne la valeur d'une pièce donnée.
        /// </summary>
        private int GetValue(string piece)
        {
            return piece switch
            {
                "P" => 10, // Pion
                "T" => 50, // Tour
                "C" => 30, // Cavalier
                "B" => 30, // Fou
                "Q" => 90, // Reine
                "K" => 100, // Roi
                _ => 0
            };
        }









        public void CalculeScoresOLD(NodeChess2 node = null)
        {
            int whiteBonus = 0;
            int blackBonus = 0;

            // Vérifie la présence de pions blancs évolutifs
            bool hasWhiteEvolutionPawn = Utils.GetEvolutionPawnIndexWhite().Any(evolutionPawnIndex => _cases[evolutionPawnIndex].Contains("P"));
            if (hasWhiteEvolutionPawn)
                whiteBonus = 90;

            // Vérifie la présence de pions noirs évolutifs
            bool hasBlackEvolutionPawn = Utils.GetEvolutionPawnIndexBlack().Any(evolutionPawnIndex => _cases[evolutionPawnIndex].Contains("P"));
            if (hasBlackEvolutionPawn)
                blackBonus = 90;

            // Compte des pièces
            int whitePawnNumber = _cases.Count(x => x == "P|W");
            int blackPawnNumber = _cases.Count(x => x == "P|B");
            int whiteBishopNumber = _cases.Count(x => x == "B|W");
            int blackBishopNumber = _cases.Count(x => x == "B|B");
            int whiteKnightNumber = _cases.Count(x => x == "C|W");
            int blackKnightNumber = _cases.Count(x => x == "C|B");
            int whiteRookNumber = _cases.Count(x => x == "T|W");
            int blackRookNumber = _cases.Count(x => x == "T|B");
            int whiteQueenNumber = _cases.Count(x => x == "Q|W");
            int blackQueenNumber = _cases.Count(x => x == "Q|B");
            int whiteKingNumber = _cases.Count(x => x == "K|W");
            int blackKingNumber = _cases.Count(x => x == "K|B");

            // Calcule les scores

            WhiteScore = whitePawnNumber * 10
                         + whiteBishopNumber * 30
                         + whiteKnightNumber * 30
                         + whiteRookNumber * 50
                         + whiteQueenNumber * 90
                         + whiteKingNumber * 100
                         + whiteBonus;

            BlackScore = blackPawnNumber * 10
                         + blackBishopNumber * 30
                         + blackKnightNumber * 30
                         + blackRookNumber * 50
                         + blackQueenNumber * 90
                         + blackKingNumber * 100
                         + blackBonus;

            Diff = Math.Abs(WhiteScore - BlackScore);

            //les manaces 
            //on prend les nenacées blancs
            if (node != null)
            {
                if (node.Level < Utils.DeepLevel && Utils.DeepLevelPrime)
                {
                    var opinionColor = "W";
                    if (node.Color == "W")
                        opinionColor = "B";
                    /*if (node.GetIsLocationIsProtected(node.ToIndex, node.Color, opinionColor))
                    {*/

                        var whiteIndexList = GetCasesIndexForColor("W");
                        foreach (var index in whiteIndexList)
                        {
                            if (node.TargetIndexIsMenaced(this, "W", "B", index) > 0)
                            {
                                var panwValue = GetValue(GetCaseInIndex(index));
                                BlackScore += panwValue / 10;
                            }
                        }
                        //on prend les nenacées blancs
                        var blackIndexList = GetCasesIndexForColor("B");
                        foreach (var index in blackIndexList)
                        {
                            if (node.TargetIndexIsMenaced(this, "B", "W", index) > 0)
                            {
                                var panwValue = GetValue(GetCaseInIndex(index));
                                WhiteScore += panwValue / 10;
                            }
                        }
                   // }
                }

            }



            if (Utils.ComputerColor == "B")
                Weight = BlackScore - WhiteScore;
            else
                Weight = WhiteScore - BlackScore;
        }


        public int GetValueOld(string caseContaint)
        {
            if (caseContaint.Contains("P|"))
                return 10;
            if (caseContaint.Contains("B|") || caseContaint.Contains("C|"))
                return 30;
            if (caseContaint.Contains("T|"))
                return 50;
            if (caseContaint.Contains("Q|"))
                return 90;
            if (caseContaint.Contains("K|"))
                return 100;
            return 0;

        }
        public string GetCaseInIndex(int index)
        {
            return _cases[index];
        }
        public int GetWeightInIndex(int index)
        {
            var caseContaint = GetCaseInIndex(index);



            return GetValue(caseContaint);
        }


        public Board()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
        }



        public int GetKingColorIndex(string color)
        {
            // var caseListColor = _cases.Where(x => x.Contains(color));
            var contain = $"K|{color}";
            return _cases.ToList().IndexOf(contain);
        }
        public Board(Board originalBord)
        {
            var originalCases = originalBord.GetCases();
            for (int i = 0; i < originalCases.Count(); i++)
            {
                _cases[i] = originalCases[i];
            }


        }

        public void Init()
        {
            //Pawn
            //SimplePawn => P
            //Knight => C
            //Bishop => B
            //Rook => T
            //Queen => Q
            //King => K

            //Color
            //Black => B
            //White => W


            //Black
            InsertPawn(0, "T", "B");
            InsertPawn(1, "C", "B");
            InsertPawn(2, "B", "B");
            InsertPawn(3, "Q", "B");
            InsertPawn(4, "K", "B");
            InsertPawn(7, "T", "B");
            InsertPawn(6, "C", "B");
            InsertPawn(5, "B", "B");
            // InsertPawn(49, "P", "B");
            for (int i = 8; i <= 15; i++)
            {
                InsertPawn(i, "P", "B");
            }


            // White
            InsertPawn(56, "T", "W");
            InsertPawn(57, "C", "W");
            InsertPawn(58, "B", "W");
            InsertPawn(59, "Q", "W");
            InsertPawn(60, "K", "W");
            //InsertPawn(14, "P", "W");
            InsertPawn(62, "C", "W");
            InsertPawn(61, "B", "W");
            InsertPawn(63, "T", "W");
            for (int i = 48; i <= 55; i++)
            {
                InsertPawn(i, "P", "W");
            }
        }

        public void Print()
        {
            Console.WriteLine("_____________________________________________________________________");
            for (int y = 0; y < 8; y++)
            {
                var line = "";
                for (int x = 0; x < 8; x++)
                {
                    var index = x + (y * 8);
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("_____________________________________________________________________");
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


        /// <summary>
        /// tsiry;07-07-2022
        /// pour les mouvements preview et next
        /// on revien en arrier
        /// </summary>
        public void NavigationMove(string initialStr, string destinationStr, bool isNext = false)
        {
            try
            {

                var fromSrtData = initialStr.Split("(");
                var fromIndex = Int32.Parse(fromSrtData[0]);

                var toSrtData = destinationStr.Split("(");
                var toIndex = Int32.Parse(toSrtData[0]);
                var fromContain = fromSrtData[1].Substring(0, fromSrtData[1].Count() - 1);
                var toContain = toSrtData[1].Substring(0, toSrtData[1].Count() - 1);
                if (!isNext)
                {
                    _cases[fromIndex] = fromContain;
                    _cases[toIndex] = toContain;
                }
                else
                {
                    _cases[fromIndex] = "__";
                    _cases[toIndex] = fromContain;
                }


                PrintInDebug();

            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
            }
        }

        public void Move(int initialIndex, int destinationIndex)
        {
            var initialCase = _cases[initialIndex];
            var destinationCase = _cases[destinationIndex];

            _cases[destinationIndex] = initialCase;


            if (destinationCase.Contains("|"))
            {
                var datas = destinationCase.Split('|');
                var pawnName = datas[0];
                var caseColor = datas[1];
                if (pawnName == "B")
                {
                    pawnName = "Bishop";

                }
                if (pawnName == "K")
                {
                    pawnName = "King";
                }
                if (pawnName == "Q")
                {
                    pawnName = "Queen";
                }
                if (pawnName == "T")
                {
                    pawnName = "Rook";
                }
                if (pawnName == "C")
                {
                    pawnName = "Knight";
                }
                if (pawnName == "P")
                {
                    pawnName = "SimplePawn";
                }




                if (caseColor == "B")
                {
                    caseColor = "Black";
                }
                else
                {

                    caseColor = "White";
                }

                //  / BishopBlack.png

                //  var imageSrc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", $"{pawnName}{caseColor}.png");

                var imageSrc = $"../../Images/{pawnName}{caseColor}.png";

                if (HuntingBoardWhiteImageList == null)
                    HuntingBoardWhiteImageList = new List<string>();
                if (HuntingBoardBlackImageList == null)
                    HuntingBoardBlackImageList = new List<string>();
                if (caseColor == "White")
                    HuntingBoardWhiteImageList.Add(imageSrc);
                else
                    HuntingBoardBlackImageList.Add(imageSrc);
            }




            //if(destinationCase.Contains("K"))
            //{
            //  var t_ok = "OK";
            //}
            _cases[initialIndex] = "__";

           // Level = level;
            //pour T35
            if (Level == 1)
                if (Utils.IsMenaced(destinationIndex, this, Utils.ComputerColor))
                {
                    //item.Weight -= 1;
                    this.DestinationIndexIsMenaced = true;
                }

            if (MovingList == null)
                MovingList = new List<string>();

            // MovingList.Add($"{Utils.ToSympbol(initialCase)}({initialIndex.ToString()}) > {Utils.ToSympbol(destinationCase)}({destinationIndex})");//pour l'affichage
            MovingList.Add($"{initialIndex.ToString()}({initialCase})>{destinationIndex.ToString()}({destinationCase})");


            //GestionDes roc
            // si le point de depar et le rois : 60 et point d'arriver est 62 
            //=>roc court pour les blancs
            //on depace aussi le rook en 63 ver 61
            if (initialIndex == 60 && destinationIndex == 62)
            {
                Move(63, 61);
            }
            // si le point de depar et le rois : 60 et point d'arriver est 58 
            //=>roc court pour les blancs
            //on depace aussi le rook en 56 ver 59
            if (initialIndex == 60 && destinationIndex == 58)
            {
                Move(56, 59);
            }


            if (initialIndex == 4 && destinationIndex == 6)
            {
                Move(7, 5);
            }
            if (initialIndex == 4 && destinationIndex == 2)
            {
                Move(0, 3);
            }

            //Evolution
            if (initialCase.Contains("|"))
            {
                var initialPawnName = initialCase.Split('|')[0];
                var initialPawnColor = initialCase.Split('|')[1];
                if (initialPawnName == "P")
                {
                    var evolutionPawnIndex = Utils.GetEvolutionPawnIndexBlack();
                    if (initialPawnColor == "W")
                    {
                        evolutionPawnIndex = Utils.GetEvolutionPawnIndexWhite();
                    }

                    if (evolutionPawnIndex.Contains(destinationIndex))
                    {
                        //évolution
                        _cases[destinationIndex] = _cases[destinationIndex].Replace("P", "Q");
                    }

                }
            }
        }

        //verifie si la case contien un pion
        //retourne 0 si libre
        //retourne 1 si alier
        //retourne -1 si advers

        public int GetIsContent(int index, string color)
        {
            if (index > _cases.Count())
                return 0;
            if (index < 0)
                return 0;
            var destinationCase = _cases[index];

            if (!destinationCase.Contains("|"))
            {
                //la case est vide
                return 0;
            }
            var caseDatas = destinationCase.Split('|');
            var pawnName = caseDatas[0];
            var caseColor = caseDatas[1];

            if (caseColor != color)

                return -1;
            return 1;
        }



        /*tsiry;18-10-2021
         * verifie si en echec en fonction du couleur
         * */
        public bool IsInChess(string color)
        {

            try
            {
                var kingIndex = _cases.ToList().IndexOf($"K|{color}");

                var opinionColor = "W";
                if (color == "W")
                    opinionColor = "B";
                var opinionPawns = GetCasesIndexForColor(opinionColor);

                var possiblesMovesOpinionIndex = new List<int>();
                foreach (var index in opinionPawns)
                {
                    //var t_ = GetPossibleMoves(index);
                    possiblesMovesOpinionIndex.AddRange(GetPossibleMoves(index, 1).Select(x => x.ToIndex));
                }


                if (possiblesMovesOpinionIndex.Contains(kingIndex))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                Utils.WritelineAsync(ex.ToString());
                return false;
            }

        }

        public List<PossibleMove> GetPossibleMoves(int index, int level, bool isForKing = true)
        {
            if (index == -1)
                return null;

            var indexInTab64 = Utils.Tab64[index];
            var results = new List<PossibleMove>();
            var currentCase = _cases[index];
            if (!currentCase.Contains("|"))
            {
                //la case est vide
                return results;
            }
            var caseDatas = currentCase.Split('|');
            var pawnName = caseDatas[0];
            var caseColor = caseDatas[1];
            var opinionColorcaseColor = "W";
            if (caseColor == "W")
                opinionColorcaseColor = "B";
            if (pawnName == "T" || pawnName == "Q" || pawnName == "K")//Rook ou Reine ou Roi
            {

                //Horizontal +
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = i;
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;

                }
                //Horizontal -
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = -(i);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }

                //vertical --
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = -(i * 10);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }

                //vertical ++
                for (int i = 1; i < 8; i++)
                {
                    var toAdd = (i * 10);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }
            }
            if (pawnName == "B" || pawnName == "Q" || pawnName == "K")//Bishop ou Reine ou Roi
            {

                //Horizontal + //vertical +
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = (y * 10) + x;
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);

                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });

                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;
                }


                //Horizontal + //vertical -
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = -((y * 10) - x);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        //if (pawnName != "K")
                        //results.Add(destinationIndex);
                        results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        // else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                        // results.Add(destinationIndex);
                        //    results.Add(new PossibleMove { FromIndex = index, Index = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }

                //Horizontal - //vertical +
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = ((y * 10) - x);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }

                //Horizontal - //vertical -
                for (int y = 1, x = 1; y < 8 && x < 8; y++, x++)
                {
                    var toAdd = -((y * 10) + x);
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    if (!Utils.IsValideMove(indexInTab64 + toAdd))
                        break;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        break;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 1)
                        break;
                    if (isContent == -1)
                    {
                        //Pour T71
                        if (pawnName != "K")
                            // results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        else if (!(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)))
                            //results.Add(destinationIndex);
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                        break;
                    }
                    //results.Add(destinationIndex);
                    results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    //si  roi, une seule déplacement
                    if (pawnName == "K")
                        break;


                }




            }

            //gestion du roc
            if (pawnName == "K")
            {
                // si le roi ne s'a pas encore bouger
                if (this.MovingList != null)
                {
                    foreach (var move in this.MovingList)
                    {

                        if (move.Contains($"K|{caseColor}"))
                            return results;
                    }
                }


                //si blanch
                if (caseColor == "W")
                {
                    if (index == 60)//si le roi n'a ba bouger de 60
                    {
                        //A droite
                        var rightIsFree = true;
                        //si il n'y a rien entre 60 et 63
                        for (int i = 61; i < 63; i++)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                rightIsFree = false;
                                break;
                            }
                        }
                        if (rightIsFree)
                        {
                            if (_cases[63] == $"T|{caseColor}")
                            {
                                var rookRightIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("63"))
                                            rookRightIsAsBeenMoved = false;
                                    }
                                }
                                if (rookRightIsAsBeenMoved)
                                    //results.Add(62);
                                    results.Add(new PossibleMove { FromIndex = index, ToIndex = 62, IsContainOpinion = false });
                            }
                        }

                        //a gauche
                        var leftIsFree = true;
                        //si il n'y a rien entre 60 et 63
                        for (int i = 59; i > 56; i--)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                leftIsFree = false;
                                break;
                            }
                        }
                        if (leftIsFree)
                        {
                            if (_cases[56] == $"T|{caseColor}")
                            {
                                var rookLeftIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("56"))
                                            rookLeftIsAsBeenMoved = false;
                                    }
                                }
                                if (rookLeftIsAsBeenMoved)
                                    //results.Add(58);
                                    results.Add(new PossibleMove { FromIndex = index, ToIndex = 58, IsContainOpinion = false });
                            }
                        }


                        //si il n'y a rien entre 60 et 56
                    }


                }
                else//si noir
                {
                    if (index == 4)//si le roi n'a ba bouger de 4
                    {
                        //A droite
                        var rightIsFree = true;

                        //si il n'y a rien entre 5 et 7
                        for (int i = 5; i < 7; i++)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                rightIsFree = false;
                                break;
                            }
                        }
                        if (rightIsFree)
                        {
                            if (_cases[7] == $"T|{caseColor}")
                            {
                                var rookRightIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("7"))
                                            rookRightIsAsBeenMoved = false;
                                    }
                                }
                                if (rookRightIsAsBeenMoved)
                                    //results.Add(6);
                                    results.Add(new PossibleMove { FromIndex = index, ToIndex = 6, IsContainOpinion = false });
                            }
                        }

                        //a gauche
                        var leftIsFree = true;
                        //si il n'y a rien entre 60 et 63
                        for (int i = 3; i > 0; i--)
                        {
                            if (_cases[i].Contains("|"))
                            {
                                leftIsFree = false;
                                break;
                            }
                        }
                        if (leftIsFree)
                        {
                            if (_cases[0] == $"T|{caseColor}")
                            {
                                var rookLeftIsAsBeenMoved = true;
                                //si le rook n'as pas encore bouger
                                if (this.MovingList != null)
                                {
                                    foreach (var move in this.MovingList)
                                    {

                                        if (move.Contains($"T|{caseColor}") && move.Contains("0"))
                                            rookLeftIsAsBeenMoved = false;
                                    }
                                }
                                if (rookLeftIsAsBeenMoved)
                                    //results.Add(2);
                                    results.Add(new PossibleMove { FromIndex = index, ToIndex = 2, IsContainOpinion = false });
                            }
                        }

                    }

                }



            }


            if (pawnName == "C")//Knight
            {
                var toAddList = new List<int>();
                toAddList.Add(-12);
                toAddList.Add(-21);
                toAddList.Add(-19);
                toAddList.Add(-8);
                toAddList.Add(12);
                toAddList.Add(21);
                toAddList.Add(19);
                toAddList.Add(8);



                foreach (var toAdd in toAddList)
                {

                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == -1 /*&& !(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)*/)
                    {
                       // f(!Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(caseColor) && Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(opinionColorcaseColor))
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });

                    }
                    if (isContent == 0 /*&& !(Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(Utils.ComputerColor)*/)
                    {
                        //results.Add(destinationIndex);
                       // if(!Utils.CloneAndMove(this, index, destinationIndex, level).IsInChess(caseColor))
                            results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });

                    }


                }



            }


            if (pawnName == "P")//SimplePawn
            {
                var toAddList = new List<int>();
                var sing = -1;
                if (caseColor == "B")
                    sing = 1;
                //Diagonal for opinion
                var toAddOpinionListList = new List<int>();
                toAddOpinionListList.Add((10 * sing) + 1);
                toAddOpinionListList.Add((10 * sing) - 1);

                //Normal move
                toAddList.Add(10 * sing);
                //pour les premérs mouvements
                if (Utils.IsInFirstTab64(indexInTab64, caseColor))
                    toAddList.Add(20 * sing);


                foreach (var toAdd in toAddList)
                {
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == 0)
                    {
                        // results.Add(destinationIndex);
                        results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = false });
                    }
                    else
                        break;


                }


                foreach (var toAdd in toAddOpinionListList)
                {
                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, caseColor);
                    if (isContent == -1)
                    {
                        //results.Add(destinationIndex);
                        results.Add(new PossibleMove { FromIndex = index, ToIndex = destinationIndex, IsContainOpinion = true });
                    }



                }



            }

           
            return results;

        }

        /// <summary>
        /// tsiry;02-07-2022
        /// pour determiner les mouvement possibles du rois si ce dérnier est menacé
        /// </summary>
        public List<int> GetKingPossiblesMoveIndex(string targetkingColor)
        {
            try
            {

                var targetkingindex = this.GetCases().ToList().IndexOf($"K|{targetkingColor}");
                var indexInTab64 = Utils.Tab64[targetkingindex];
                var results = new List<int>();
                var toAddList = new List<int>();
                toAddList.Add(-11);
                toAddList.Add(-10);
                toAddList.Add(-9);
                toAddList.Add(+1);
                toAddList.Add(+11);
                toAddList.Add(10);
                toAddList.Add(9);
                toAddList.Add(-1);



                foreach (var toAdd in toAddList)
                {

                    var destinationIndexInTab64 = indexInTab64 + toAdd;
                    var destinationIndex = Utils.Tab64.ToList().IndexOf(destinationIndexInTab64);
                    if (destinationIndex < 0 || destinationIndex > 63)
                        continue;
                    var isContent = GetIsContent(destinationIndex, targetkingColor);
                    if (isContent == -1)
                        results.Add(destinationIndex);
                    if (isContent == 0)
                    {
                        //results.Add(destinationIndex);
                        results.Add(destinationIndex);

                    }


                }

                return results;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        #endregion
    }
}
