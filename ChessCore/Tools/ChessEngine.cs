namespace ChessCore.Tools
{
    public class ChessEngine : IDisposable
    {
        public TimeSpan ReflectionTime { get; set; }
        //EMULER l1 ET L3 ET PRENDRE LE PLUS HAUT
        private bool _checkIsInChessOnEnd;
        private int _depthLevel = 3;

        public NodeCE GetBestPositionLocalUsingMiltiThreading(string colore, BoardCE boardChess, int depthLevel = 3)
        {

            if (depthLevel == 3)
                return GetBestPositionLocalUsingMiltiThreadingObtimize(colore, boardChess, depthLevel);
            else
                return GetBestPositionLocalUsingMiltiThreadingSimple(colore, boardChess, depthLevel);
        }
        public NodeCE GetBestPositionLocalUsingMiltiThreadingSimple(string colore, BoardCE boardChess, int depthLevel = 3)
        {
            if (boardChess.IsGameOver())
                return null;
            var totalStartTime = DateTime.UtcNow;
            Utils.WritelineAsync($"DEPTH LEVEL : {depthLevel}");
            var checkIsInChessOnEnd = true;
            if (depthLevel == 5)
                checkIsInChessOnEnd = false;
            var r = RunEngine(colore, boardChess, depthLevel, false, null,checkIsInChessOnEnd);
            try
            {
                r.ReflectionTime = ReflectionTime;
            }
            catch (Exception)
            {

            }
            Utils.WritelineAsync($"TOTAL REFLECTION TIME : {DateTime.UtcNow - totalStartTime}");

            Utils.WritelineAsync($"r: {r}");
            return r;

        }
        public NodeCE GetBestPositionLocalUsingMiltiThreadingObtimize(string colore, BoardCE boardChess, int depthLevel = 3)
        {
            if (boardChess.IsGameOver())
                return null;

            var l1 = new NodeCE() { Weight = -9999, Colore = colore };
            var l3 = new NodeCE() { Weight = -9999, Colore = colore };
            var l5 = new NodeCE() { Weight = -9999, Colore = colore };

            var totalStartTime = DateTime.UtcNow;
            Utils.WritelineAsync($"DEPTH LEVEL : {depthLevel}");

            NodeCE FinalBest = null;
            var bestList = new List<NodeCE>();

            ///T124
            var maxiDiffToTakeMinimum = 1;
            l1 = RunEngine(colore, boardChess, 1, false);
            try
            {
                l1.ReflectionTime = ReflectionTime;
            }
            catch (Exception)
            {


            }

            var l1p = RunEngine(colore, boardChess, 1, false, null,false);
            try
            {
                l1p.ReflectionTime = ReflectionTime;
            }
            catch (Exception)
            {

            }


            if (l1.Weight == 9999)
            {
                Utils.WritelineAsync($"TOTAL REFLECTION TIME : {DateTime.UtcNow - totalStartTime}");

                Utils.WritelineAsync($"WIN in l1 : {l1}");
                return l1;

            }

            if (depthLevel == 1)
            {
                FinalBest = l1;
                Utils.WritelineAsync($"TOTAL REFLECTION TIME : {DateTime.UtcNow - totalStartTime}");

                Utils.WritelineAsync($"FinalBest: {FinalBest}");

                return FinalBest;
            }

            var blackList = l1.AllNodeCEList.Where(x => x.Weight <= -9999).ToList();
            Utils.WritelineAsync("BLACKLIST: ");
            //foreach (var node in blackList)
            //{
            //    Utils.WritelineAsync($"{node}");
            //}
           
            if (depthLevel == 3)
            {
               


                    l3 = RunEngine(colore, boardChess, 3, false, blackList);
                    try
                    {
                        l3.ReflectionTime = ReflectionTime;
                    }
                    catch (Exception)
                    {


                    }

                    // var l3Odd = RunEngineOdd(colore, boardChess, 3, false);
                    // var l3p = RunEngine(colore, boardChess, isReprise, specificBoardCEList, 3, false,false);

                    //  var l4Even = RunEngineEven(colore, boardChess, 4, false);



                    try
                    {
                        if (l1 != null && l3 != null)
                        {
                            if (l1.Weight > -9000 && l3.Weight < -9000)
                            {
                                FinalBest = l1;
                                Utils.WritelineAsync($"TOTAL REFLECTION TIME : {DateTime.UtcNow - totalStartTime}");

                                Utils.WritelineAsync($"FinalBest: {FinalBest}");
                                return FinalBest;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        Utils.WritelineAsync($"Exeption in l1.Weight > -9000 && l3.Weight < -9000 {ex}"); ;
                    }


                    //recalibre l1 

                    //T128_W_NotToC1
                    try
                    {
                        if (l1 != null && l3 != null)
                        {
                            if (l1.Weight > l3.Weight && l3.Weight > -200 /*T67EchecBlancLeRoiDoitSeMettreEnE1*/) // 14 failds
                            {

                                foreach (var node in l1.EquivalentBestNodeCEList)
                                {

                                    var maxNode = l3.AllNodeCEList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex);
                                    if (maxNode != null)
                                    {
                                        //if(node.Location =="b2" && node.BestChildPosition =="c1")
                                        //{
                                        //    var dsf = node;
                                        //}
                                        var currentDiff = Math.Abs(node.Weight - maxNode.Weight);

                                        if (currentDiff > 10 /*T132_B_toC6 OLD 1000*/ && node.Weight < 9000 && currentDiff < 20000/*T67EchecBlancLeRoiDoitSeMettreEnE1*/)
                                            node.Weight = maxNode.Weight;
                                    }
                                }

                                //réfind best in l1
                                Utils.WritelineAsync($"refind best in l1:");
                                var maxWeight = l1.EquivalentBestNodeCEList.Max(x => x.Weight);

                                var newEquivalentBestNodeCEList = l1.EquivalentBestNodeCEList.Where(x => x.Weight == maxWeight).ToList();
                                var rand = new Random();
                                l1 = newEquivalentBestNodeCEList[rand.Next(newEquivalentBestNodeCEList.Count)];
                                l1.EquivalentBestNodeCEList = newEquivalentBestNodeCEList;

                                Utils.WritelineAsync($"bestNodeCEList after refind :");
                                foreach (var node in l1.EquivalentBestNodeCEList)
                                {
                                    Utils.WritelineAsync($"{node}");
                                }
                                Utils.WritelineAsync($"new best l1 {l1}");
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Utils.WritelineAsync($"Exeption in recalibre l1 {ex}"); ;
                    }


                    //recalibre l1p 
                    try
                    {
                        if (l1p != null && l3 != null)
                        {
                            if (l1p.Weight > l3.Weight && l3.Weight > -200 /*T67EchecBlancLeRoiDoitSeMettreEnE1*/) // 14 failds
                            {

                                foreach (var node in l1p.EquivalentBestNodeCEList)

                                {

                                    var maxNode = l3.AllNodeCEList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex);
                                    if (maxNode != null)
                                    {
                                        //if(node.Location =="b2" && node.BestChildPosition =="c1")
                                        //{
                                        //    var dsf = node;
                                        //}
                                        var currentDiff = Math.Abs(node.Weight - maxNode.Weight);

                                        if (currentDiff > 10/*T132_B_toC6*/ && node.Weight < 9000 && currentDiff < 20000/*T67EchecBlancLeRoiDoitSeMettreEnE1*/)
                                            node.Weight = maxNode.Weight;
                                    }
                                }

                                //réfind best in l1p
                                Utils.WritelineAsync($"refind best in l1p:");
                                var maxWeight = l1p.EquivalentBestNodeCEList.Max(x => x.Weight);

                                var newEquivalentBestNodeCEList = l1p.EquivalentBestNodeCEList.Where(x => x.Weight == maxWeight).ToList();
                                var rand = new Random();
                                l1p = newEquivalentBestNodeCEList[rand.Next(newEquivalentBestNodeCEList.Count)];
                                l1p.EquivalentBestNodeCEList = newEquivalentBestNodeCEList;

                                Utils.WritelineAsync($"bestNodeCEList after refind :");
                                foreach (var node in l1p.EquivalentBestNodeCEList)
                                {
                                    Utils.WritelineAsync($"{node}");
                                }
                                Utils.WritelineAsync($"new best l1 {l1p}");
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Utils.WritelineAsync($"Exeption in recalibre l1 {ex}"); ;
                    }


                    var diff = 0;
                    if (l1 != null && l3 != null)
                        diff = Math.Abs(l1.Weight - l3.Weight);




                    if (l1 != null && l3 != null)
                    {
                        //T29_W_PourProtegerDEchec
                        //if (l1.Weight < 0 && l3.Weight < 0)
                        //    maxiDiffToTakeMinimum = 10;
                        if (l1.Weight > l3.Weight && diff > maxiDiffToTakeMinimum) // 14 failds
                        {
                            FinalBest = l1;
                        }
                        else
                            FinalBest = l3;
                    }



                    if (l1p != null && FinalBest != null)
                    {
                        if (l1p.Weight > FinalBest.Weight && FinalBest.Weight < -200)
                        {
                            Utils.WritelineAsync($"l3 ({l3}) and l1 ({l1}) are in chess, take l1p ({l1p})");
                            FinalBest = l1p;
                        }
                    }
                    Utils.WritelineAsync($"l1 : {l1}");
                    Utils.WritelineAsync($"l1p : {l1p}");
                    Utils.WritelineAsync($"l3 : {l3}");

                    Utils.WritelineAsync($"TOTAL REFLECTION TIME : {DateTime.UtcNow - totalStartTime}");


                    if (FinalBest == null)
                    {
                    Utils.WritelineAsync($"FinalBest IS NULL RE RUN, FinalBest l1");


                    FinalBest = l1;



                    }
                Utils.WritelineAsync($"FinalBest: {FinalBest}");
                return FinalBest;
            }
            


            blackList.AddRange(l3.AllNodeCEList.Where(x => x.Weight <= -9999).ToList());
            Utils.WritelineAsync("BLACKLIST: ");
            //foreach (var node in blackList)
            //{
            //    Utils.WritelineAsync($"{node}");
            //}

            l5 = RunEngine(colore, boardChess, 5, false, blackList, true);
            try
            {
                l5.ReflectionTime = ReflectionTime;
            }
            catch
            {

            }


            Utils.WritelineAsync($"l1 : {l1}");
            Utils.WritelineAsync($"l1p : {l1p}");
            Utils.WritelineAsync($"l3 : {l3}");
            Utils.WritelineAsync($"l5 : {l5}");



            if (l5 != null)
            {
                if (l5.Weight >= FinalBest.Weight)
                {
                    FinalBest = l5;

                }
            }




            //bestList.Add(l1p);
            //bestList.Add(l3p);


            Utils.WritelineAsync($"TOTAL REFLECTION TIME : {DateTime.UtcNow - totalStartTime}");

            Utils.WritelineAsync($"FinalBest: {FinalBest}");




            return FinalBest;
        }
        public NodeCE RunEngine(string colore, BoardCE boardChess, int depthLevel, bool isOppinionTurnInNext,List<NodeCE> blackList= null ,  bool checkIsInChessOnEnd = true, int limitOfReflectionTimeInSecond = 24 * 60 * 60)
        {
            try
            {
                _depthLevel = depthLevel;
                var maxWeight = double.NegativeInfinity;
                colore = colore[0].ToString();
                string opponentColor = colore == "W" ? "B" : "W";
                string iaTurn = (depthLevel % 2 == 1) ? opponentColor : colore;
                _checkIsInChessOnEnd = checkIsInChessOnEnd;
                Utils.LimitOfReflectionTimeInSecond = limitOfReflectionTimeInSecond;
                Utils.WritelineAsync($"DepthLevel :  {_depthLevel}");
                Utils.WritelineAsync($"cpuColor :  {colore}");
                Utils.WritelineAsync($"iaTurn :  {iaTurn}");
                Utils.WritelineAsync($"opponentColor :  {opponentColor}");
                Utils.WritelineAsync($"isOppinionTurnInNext :  {isOppinionTurnInNext}");
                Utils.WritelineAsync($"LimitOfReflectionTimeInSecond :  {Utils.LimitOfReflectionTimeInSecond}");
                Utils.WritelineAsync($"_checkIsInChessOnEnd :  {_checkIsInChessOnEnd}");
                Utils.LimitOfReflectionTimeIsShow = false;
                var bestNodeCEList = new List<NodeCE>();
                var allNodeCEList = new List<NodeCE>();

                var startTime = DateTime.UtcNow;
                Utils.EnginStartTime = startTime;
                var pawnIndices = boardChess.GetCasesIndexForColor(colore);


                // Recherche parallèle des meilleurs mouvements
                Parallel.ForEach(pawnIndices, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, pawnIndex =>
                // foreach (var pawnIndex in pawnIndices)

                {



                    //OLD GetPossibleMoves
                    var possibleMoves = boardChess.GetPossibleMovesOLD(pawnIndex);


                    //OLD siple foreach
                    foreach (var move in possibleMoves)
                    // Parallel.ForEach(possibleMoves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>

                    {
                        var clonedBoardCE = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                        var node = MinMaxWithAlphaBeta(
                            clonedBoardCE,
                            depthLevel,
                            double.NegativeInfinity,
                            double.PositiveInfinity,
                            isOppinionTurnInNext, // Opposant joue ensuite
                            iaTurn
                        );
                        node.FromIndex = move.FromIndex;
                        node.ToIndex = move.ToIndex;
                        node.BoardCE = clonedBoardCE;

                        //T88LesBlanchDoiventEviterLEchec
                        //T136B_W_D5toF6 
                        //T136B_W_D5toF6
                        if (blackList!=null)
                        {
                            if (blackList.Count > 0)
                            {
                                var nodeInBlackList = blackList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex);
                                if(nodeInBlackList!=null)
                                {
                                    Utils.WritelineAsync($"{node} IS IN BLACKLIST");
                                    continue;
                                }
                            }

                        }


                        lock (bestNodeCEList)
                        {
                            var currentClonedBoardCE = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);


                            //TODO METTRE ICI EN PRIORITE LES AMELIORATION
                            var targetIndexIsMenaced = currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor);
                            if (targetIndexIsMenaced)
                            {
                                node.Weight -= currentClonedBoardCE.GetPieceValue(currentClonedBoardCE._cases[node.ToIndex])/*depthLevel*/;//* tomenacedNumber;
                            }
                            //king is menaced
                            if (currentClonedBoardCE.KingIsMenaced(colore))
                            {
                                node.Weight -= 100;  // Malus pour déplacer une pièce menacée
                            }

                            //if (currentClonedBoardCE.KingIsMenaced(opponentColor) && !currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor))
                            //{
                            //    node.Weight += 100;  // Malus pour déplacer une pièce menacée
                            //}
                            //menacedBonus and menacedMalus
                            //if(node.ToIndex == 51)
                            //{
                            //    var fd = node;
                            //    var fromValue = boardChess.GetPieceValue(node.FromIndex);
                            //    var toValue = boardChess.GetPieceValue(node.ToIndex);
                            //}
                            //T134_B_NotToD2 and T133_B_NotC5toE5AndNotC5toE7
                            if (!currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor) 
                            || (currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor) && ( boardChess.GetPieceValue(node.FromIndex) < boardChess.GetPieceValue(node.ToIndex))))
                            {
                                var menacedBonus = currentClonedBoardCE.GetMenacedsPoints(opponentColor);
                                node.Weight += menacedBonus;
                            }

                            
                            var menacedMalus = currentClonedBoardCE.GetMenacedsPoints(colore);
                            node.Weight -= menacedMalus;

                            ///T124
                            ////if initial position is menaced and to position is pprotected
                            if (depthLevel == 3
                            && !currentClonedBoardCE._cases[node.ToIndex].StartsWith('K')
                            && !currentClonedBoardCE._cases[node.ToIndex].StartsWith('P')
                            && boardChess.TargetIndexIsMenaced(node.FromIndex, opponentColor)
                             && currentClonedBoardCE.TargetIndexIsProtected(node.ToIndex, colore)
                            && currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor))
                            {
                                var isAddProtectedBonus = false;
                                var oppinionMenacedMoveList = currentClonedBoardCE.GetMovesOfOpponentsWhoThreaten(node.ToIndex, opponentColor);
                                foreach (var oppinioMove in oppinionMenacedMoveList)
                                {
                                    if (!currentClonedBoardCE.TargetIndexIsMenaced(oppinioMove.FromIndex, colore))
                                        isAddProtectedBonus = true;
                                }
                                // si celui qui menace est menacé
                                if (isAddProtectedBonus)
                                    node.Weight += currentClonedBoardCE.GetPieceValue(currentClonedBoardCE._cases[node.ToIndex]);

                            }





                            //In chess
                            if (currentClonedBoardCE.IsKingInCheck(colore))
                                node.Weight = -9999;
                            if (currentClonedBoardCE.IsKingInCheck(opponentColor)/* && !targetIndexIsMenaced T67WhiteIsInChess*/)
                                node.Weight = 9999;
                            allNodeCEList.Add(node);




                            if (node.Weight > maxWeight)
                            {
                                Utils.WritelineAsync($"{node} *");
                                maxWeight = node.Weight;

                            }
                            bestNodeCEList.Add(node);
                        }
                    }




                });


                // Sélection du meilleur coup
                maxWeight = bestNodeCEList.Max(x => x.Weight);

                bestNodeCEList = bestNodeCEList.Where(x => x.Weight == maxWeight).ToList();
                var rand = new Random();
                var bestNodeCE = bestNodeCEList[rand.Next(bestNodeCEList.Count)];


                Utils.WritelineAsync($"bestNodeCEList :");
                foreach (var node in bestNodeCEList)
                {
                    Utils.WritelineAsync($"{node}");
                }
                bestNodeCE.EquivalentBestNodeCEList = bestNodeCEList;
                bestNodeCE.AllNodeCEList.AddRange(allNodeCEList);
                var elapsed = DateTime.UtcNow - startTime;
                Utils.WritelineAsync($"REFLECTION TIME: {elapsed}");
                // Utils.WritelineAsync($"Utils.PossibleMovesListCount = {Utils.PossibleMovesList.Count()}");
                //Utils.WritelineAsync($"Utils.IsKingInCheckListCount = {Utils.IsKingInCheckList.Count()}");
                Utils.WritelineAsync($"Best node : {bestNodeCE}");
                ReflectionTime = elapsed;
                return bestNodeCE;
            }
            catch (Exception)
            {

                return null; ;
            }
            finally
            {
                //  Utils.GCColect();
            }

        }


        public NodeCE RunEngineOdd(string colore, BoardCE boardChess, int depthLevel, bool isOppinionTurnInNext, bool checkIsInChessOnEnd = true, int limitOfReflectionTimeInSecond = 24 * 60 * 60)
        {
            try
            {

                var maxWeight = double.NegativeInfinity;
                string cpuColor = colore[0].ToString();
                string opponentColor = cpuColor == "W" ? "B" : "W";
                _checkIsInChessOnEnd = checkIsInChessOnEnd;
                Utils.LimitOfReflectionTimeInSecond = limitOfReflectionTimeInSecond;
                Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
                Utils.WritelineAsync($"cpuColor :  {cpuColor}");
                Utils.WritelineAsync($"opponentColor :  {opponentColor}");
                Utils.WritelineAsync($"isOppinionTurnInNext :  {isOppinionTurnInNext}");
                Utils.WritelineAsync($"LimitOfReflectionTimeInSecond :  {Utils.LimitOfReflectionTimeInSecond}");
                Utils.WritelineAsync($"_checkIsInChessOnEnd :  {_checkIsInChessOnEnd}");
                Utils.LimitOfReflectionTimeIsShow = false;
                var bestNodeCEList = new List<NodeCE>();
                var allNodeCEList = new List<NodeCE>();

                var startTime = DateTime.UtcNow;
                Utils.EnginStartTime = startTime;
                var pawnIndices = boardChess.GetCasesIndexForColor(cpuColor);


                // Recherche parallèle des meilleurs mouvements
                Parallel.ForEach(pawnIndices, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, pawnIndex =>
                {



                    //OLD GetPossibleMoves
                    var possibleMoves = boardChess.GetPossibleMovesOLD(pawnIndex);


                    //OLD siple foreach
                    foreach (var move in possibleMoves)
                    {
                        var clonedBoardCE = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                        var node = MinMaxWithAlphaBeta(
                            clonedBoardCE,
                            depthLevel,
                            double.NegativeInfinity,
                            double.PositiveInfinity,
                            isOppinionTurnInNext, // Opposant joue ensuite
                            opponentColor
                        );
                        node.FromIndex = move.FromIndex;
                        node.ToIndex = move.ToIndex;

                        lock (bestNodeCEList)
                        {
                            //TEST
                            if (node.ToIndex == 18)
                            {
                                var hhfd = 0;
                            }

                            var currentClonedBoardCE = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                            var targetIndexIsMenaced = currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor);
                            if (targetIndexIsMenaced)
                            { //Menace
                                if (node.ToIndex == 25)
                                {
                                    var fd = 0;
                                }
                                //T37 
                                node.Weight -= currentClonedBoardCE.GetPieceValue(currentClonedBoardCE._cases[node.ToIndex])/*depthLevel*/;//* tomenacedNumber;
                            }
                            //protection
                            //var currentClonedBoardCE2 = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                            //var targetIndexIsMenaced = currentClonedBoardCE2.TargetIndexIsProtected(node.ToIndex, colore);
                            //if (targetIndexIsMenaced)
                            //{
                            //    node.Weight -= currentClonedBoardCE2.GetPieceValue(currentClonedBoardCE._cases[node.ToIndex]);//* tomenacedNumber;
                            //}

                            //if (currentClonedBoardCE.KingIsMenaced(opponentColor))
                            //{
                            //    node.Weight += 100;  // Malus pour déplacer une pièce menacée
                            //}

                            //king is menaced
                            // T59FinDePartieEviterMortDuRoiNoir
                            if (currentClonedBoardCE.KingIsMenaced(cpuColor))
                            {
                                node.Weight -= 100;  // Malus pour déplacer une pièce menacée
                            }

                            //TODO METTRE ICI EN PRIORITE LES AMELIORATION



                            //menacedBonus
                            var menacedBonus = currentClonedBoardCE.GetMenacedsPoints(opponentColor);
                            node.Weight += menacedBonus;
                            var menacedMalus = currentClonedBoardCE.GetMenacedsPoints(cpuColor);
                            node.Weight -= menacedMalus;


                            //In chess
                            if (currentClonedBoardCE.IsKingInCheck(cpuColor))
                                node.Weight = -9999;
                            if (currentClonedBoardCE.IsKingInCheck(opponentColor)/* && !targetIndexIsMenaced T67WhiteIsInChess*/)
                                node.Weight = 9999;

                            allNodeCEList.Add(node);


                            // if (node.Weight <= -1000)
                            //    blackNodeCEList.Add(node);
                            //Utils.WritelineAsync($"{node}");
                            if (node.Weight > maxWeight)
                            {
                                Utils.WritelineAsync($"{node} *");
                                maxWeight = node.Weight;
                            }
                            bestNodeCEList.Add(node);
                        }
                    }




                });

                // Sélection du meilleur coup
                maxWeight = bestNodeCEList.Max(x => x.Weight);

                bestNodeCEList = bestNodeCEList.Where(x => x.Weight == maxWeight).ToList();
                var rand = new Random();
                var bestNodeCE = bestNodeCEList[rand.Next(bestNodeCEList.Count)];


                Utils.WritelineAsync($"bestNodeCEList :");
                foreach (var node in bestNodeCEList)
                {
                    Utils.WritelineAsync($"{node}");
                }
                bestNodeCE.EquivalentBestNodeCEList = bestNodeCEList;
                bestNodeCE.AllNodeCEList = allNodeCEList;
                var elapsed = DateTime.UtcNow - startTime;
                Utils.WritelineAsync($"Reflection time: {elapsed}");
                Utils.WritelineAsync($"Best node : {bestNodeCE}");
                return bestNodeCE;
            }
            catch (Exception)
            {

                return null; ;
            }
            finally
            {
                //  Utils.GCColect();
            }

        }


        public NodeCE RunEngineEven(string colore, BoardCE boardChess, int depthLevel, bool isOppinionTurnInNext, bool checkIsInChessOnEnd = true, int limitOfReflectionTimeInSecond = 24 * 60 * 60)
        {
            try
            {

                var maxWeight = double.NegativeInfinity;
                string cpuColor = colore[0].ToString();
                string opponentColor = cpuColor == "W" ? "B" : "W";
                _checkIsInChessOnEnd = checkIsInChessOnEnd;
                Utils.LimitOfReflectionTimeInSecond = limitOfReflectionTimeInSecond;
                Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
                Utils.WritelineAsync($"cpuColor :  {cpuColor}");
                Utils.WritelineAsync($"opponentColor :  {opponentColor}");
                Utils.WritelineAsync($"isOppinionTurnInNext :  {isOppinionTurnInNext}");
                Utils.WritelineAsync($"LimitOfReflectionTimeInSecond :  {Utils.LimitOfReflectionTimeInSecond}");
                Utils.WritelineAsync($"_checkIsInChessOnEnd :  {_checkIsInChessOnEnd}");
                Utils.LimitOfReflectionTimeIsShow = false;
                var bestNodeCEList = new List<NodeCE>();
                var allNodeCEList = new List<NodeCE>();

                var startTime = DateTime.UtcNow;
                Utils.EnginStartTime = startTime;
                var pawnIndices = boardChess.GetCasesIndexForColor(cpuColor);


                // Recherche parallèle des meilleurs mouvements
                Parallel.ForEach(pawnIndices, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, pawnIndex =>
                {



                    //OLD GetPossibleMoves
                    var possibleMoves = boardChess.GetPossibleMovesOLD(pawnIndex);


                    //OLD siple foreach
                    foreach (var move in possibleMoves)
                    {
                        var clonedBoardCE = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                        var node = MinMaxWithAlphaBeta(
                            clonedBoardCE,
                            depthLevel,
                            double.NegativeInfinity,
                            double.PositiveInfinity,
                            isOppinionTurnInNext, // Opposant joue ensuite
                            cpuColor
                        );
                        node.FromIndex = move.FromIndex;
                        node.ToIndex = move.ToIndex;

                        lock (bestNodeCEList)
                        {
                            //TEST
                            if (node.ToIndex == 18)
                            {
                                var hhfd = 0;
                            }

                            var currentClonedBoardCE = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                            var targetIndexIsMenaced = currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor);
                            if (targetIndexIsMenaced)
                            {
                                //T37 
                                node.Weight -= currentClonedBoardCE.GetPieceValue(currentClonedBoardCE._cases[node.ToIndex])/*depthLevel*/;//* tomenacedNumber;
                            }
                            //protection
                            //var currentClonedBoardCE2 = boardChess.CloneAndMove(move.FromIndex, move.ToIndex);
                            //var targetIndexIsMenaced = currentClonedBoardCE2.TargetIndexIsProtected(node.ToIndex, colore);
                            //if (targetIndexIsMenaced)
                            //{
                            //    node.Weight -= currentClonedBoardCE2.GetPieceValue(currentClonedBoardCE._cases[node.ToIndex]);//* tomenacedNumber;
                            //}

                            //if (currentClonedBoardCE.KingIsMenaced(opponentColor))
                            //{
                            //    node.Weight += 100;  // Malus pour déplacer une pièce menacée
                            //}

                            //king is menaced
                            // T59FinDePartieEviterMortDuRoiNoir
                            if (currentClonedBoardCE.KingIsMenaced(cpuColor))
                            {
                                node.Weight -= 100;  // Malus pour déplacer une pièce menacée
                            }

                            //TODO METTRE ICI EN PRIORITE LES AMELIORATION
                            //menacedBonus
                            var menacedBonus = currentClonedBoardCE.GetMenacedsPoints(opponentColor);
                            node.Weight += menacedBonus;
                            var menacedMalus = currentClonedBoardCE.GetMenacedsPoints(cpuColor);
                            node.Weight -= menacedMalus;



                            //In chess
                            if (currentClonedBoardCE.IsKingInCheck(cpuColor))
                                node.Weight = -9999;
                            if (currentClonedBoardCE.IsKingInCheck(opponentColor)/* && !targetIndexIsMenaced T67WhiteIsInChess*/)
                                node.Weight = 9999;







                            allNodeCEList.Add(node);


                            // if (node.Weight <= -1000)
                            //    blackNodeCEList.Add(node);
                            //Utils.WritelineAsync($"{node}");
                            if (node.Weight > maxWeight)
                            {
                                Utils.WritelineAsync($"{node} *");
                                maxWeight = node.Weight;
                            }
                            bestNodeCEList.Add(node);
                        }
                    }




                });

                // Sélection du meilleur coup
                maxWeight = bestNodeCEList.Max(x => x.Weight);

                bestNodeCEList = bestNodeCEList.Where(x => x.Weight == maxWeight).ToList();
                var rand = new Random();
                var bestNodeCE = bestNodeCEList[rand.Next(bestNodeCEList.Count)];


                Utils.WritelineAsync($"bestNodeCEList :");
                foreach (var node in bestNodeCEList)
                {
                    Utils.WritelineAsync($"{node}");
                }
                bestNodeCE.EquivalentBestNodeCEList = bestNodeCEList;
                bestNodeCE.AllNodeCEList = allNodeCEList;
                var elapsed = DateTime.UtcNow - startTime;
                Utils.WritelineAsync($"Reflection time: {elapsed}");
                Utils.WritelineAsync($"Best node : {bestNodeCE}");
                return bestNodeCE;
            }
            catch (Exception)
            {

                return null; ;
            }
            finally
            {
                //  Utils.GCColect();
            }

        }




        public NodeCE MinMaxWithAlphaBeta(BoardCE board, int depth, double alpha, double beta, bool maximizingPlayer, string cpuColor)
        {
            var executionEngineTime = DateTime.UtcNow - Utils.EnginStartTime;
            if (executionEngineTime.TotalSeconds > Utils.LimitOfReflectionTimeInSecond)
            {
                if (!Utils.LimitOfReflectionTimeIsShow)
                {
                    Utils.WritelineAsync($"executionEngineTime : {executionEngineTime}");
                    Utils.WritelineAsync($"REFLECTION TIME OVER  {Utils.LimitOfReflectionTimeInSecond} s STOP ENGINE");
                    Utils.LimitOfReflectionTimeIsShow = true;
                }
                return new NodeCE() { Level = -1, Weight = -9999 };
            }


            var currentNodeCE = new NodeCE
            {
                Level = depth,
                Colore = cpuColor
            };
            string opponentColor = cpuColor == "W" ? "B" : "W";

            //T131_B_E8toD8
            if (board.IsKingInCheck(cpuColor))
            {
                currentNodeCE.Weight = -9999;
               //  return currentNodeCE;
            }
            if (board.IsKingInCheck(opponentColor))
            {
                currentNodeCE.Weight = 9999;
                // return currentNodeCE;
            }

            // Vérification de fin de recherche ou de fin de partie
            if (depth == 0 || board.IsGameOver())
            {
                // Évaluation de la position courante
                currentNodeCE.Weight = board.CalculateBoardCEScore(board, cpuColor, opponentColor);


                ////TEST A DECOMMNETER SI NO SUCCES
                ////if (_checkIsInChessOnEnd)
                ////{
                ////    if (board.IsKingInCheck(cpuColor))
                ////        currentNodeCE.Weight = -9999;
                ////    if (board.IsKingInCheck(opponentColor))
                ////        currentNodeCE.Weight = 9999;
                ////}



                //LAST EDIT VERY LONG EXECUTION TIME
                //king is menaced
                //if (board.KingIsMenaced(cpuColor))
                //{
                //    currentNodeCE.Weight -= 100;  // Malus pour déplacer une pièce menacée
                //}
                //menacedBonus
                //var menacedBonus = board.GetMenacedsPoints(opponentColor);
                //currentNodeCE.Weight += menacedBonus;
                //var menacedMalus = board.GetMenacedsPoints(cpuColor);
                //currentNodeCE.Weight -= menacedMalus;




                //In chess
                // T88LesBlanchDoiventEviterLEchec et T95SuiteSuiteBlackWin
                if (_depthLevel == 1)
                {
                    if (board.IsKingInCheck(cpuColor))
                        currentNodeCE.Weight = -9999;
                    if (board.IsKingInCheck(opponentColor)/* && !targetIndexIsMenaced T67WhiteIsInChess*/)
                        currentNodeCE.Weight = 9999;
                }
                   




                return currentNodeCE;
            }

            double bestValue = maximizingPlayer ? double.NegativeInfinity : double.PositiveInfinity;

            // Obtenir les mouvements possibles pour la couleur courante
            var moves = board.GetPossibleMovesForColor(cpuColor);

            // Parcourir les mouvements possibles

            foreach (var move in moves)
            //  Parallel.ForEach(moves, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, move =>
            {
                // Crée un clone du plateau et applique le mouvement
                var clonedBoard = board.CloneAndMove(move.FromIndex, move.ToIndex);


                // Appel récursif pour le sous-nœud
                var childNodeCE = MinMaxWithAlphaBeta(
                    board: clonedBoard,
                    depth: depth - 1,
                    alpha: alpha,
                    beta: beta,
                    maximizingPlayer: !maximizingPlayer,
                    cpuColor: opponentColor
                );

                // Ajouter la valeur de la pièce capturée, si elle existe
                //string capturedPiece = board.GetCaseInIndex(move.ToIndex);
                //if (capturedPiece != "__")
                //{
                //    string capturedPieceType = capturedPiece.Split('|')[0];
                //    int capturedValue = clonedBoard.GetPieceValue(capturedPieceType);
                //    childNodeCE.Weight += maximizingPlayer ? capturedValue : -capturedValue;
                //}

                // Ajouter un bonus pour mettre l'adversaire en échec
                //if (clonedBoard.IsKingInCheck(opponentColor))
                //{
                //    childNodeCE.Weight += 50;
                //}

                //// Vérifier les menaces sur les pièces alliées
                //if (TargetIndexIsMenaced(clonedBoard, cpuColor, opponentColor, move.ToIndex) > 0)
                //{
                //    childNodeCE.Weight -= 20;  // Malus pour déplacer une pièce menacée
                //}
                // malus si le roi est menacé 
                //T59FinDePartieEviterMortDuRoiNoir
                //if (clonedBoard.KingIsMenaced(cpuColor))
                //{
                //    childNodeCE.Weight -= 100;  // Malus pour déplacer une pièce menacée
                //}

                // T59FinDePartieEviterMortDuRoiNoir OK BUT LON TIME
                //if (board.KingIsMenaced(cpuColor))
                //{
                //    currentNodeCE.Weight -= 100;  // Malus pour déplacer une pièce menacée
                //}
                //if (board.KingIsMenaced(opponentColor))
                //{
                //    currentNodeCE.Weight += 100;  // Malus pour déplacer une pièce menacée
                //}


                //menacedBonus
                //T129_W_notB2toB3
                //LONG TIME IN L3
                //If l3 faild T129_W_notB2toB3
                //
                if (_depthLevel == 1)
                {
                    var menacedBonus = clonedBoard.GetMenacedsPoints(opponentColor);
                    childNodeCE.Weight += menacedBonus;
                    var menacedMalus = clonedBoard.GetMenacedsPoints(cpuColor);
                    childNodeCE.Weight -= menacedMalus;
                }



                // Maximizing Player (CPU)
                if (maximizingPlayer)
                {
                    if (childNodeCE.Weight > bestValue)
                    {
                        bestValue = childNodeCE.Weight;
                        currentNodeCE.ToIndex = move.ToIndex;
                        currentNodeCE.FromIndex = move.FromIndex;

                    }
                    alpha = Math.Max(alpha, bestValue);
                }
                // Minimizing Player (Adversaire)
                else
                {
                    if (childNodeCE.Weight < bestValue)
                    {
                        bestValue = childNodeCE.Weight;
                        currentNodeCE.ToIndex = move.ToIndex;
                        currentNodeCE.FromIndex = move.FromIndex;

                    }
                    beta = Math.Min(beta, bestValue);
                }

                // Élagage Alpha-Beta
                if (beta <= alpha)
                {
                    break;
                    // return;
                }
                //});
            }



            //if(currentNodeCE.FromIndex == 27 && currentNodeCE.ToIndex == 21)
            //{
            //    var dfd = currentNodeCE;
            //}

            //if (currentNodeCE.FromIndex == 6 && currentNodeCE.ToIndex == 2)
            //{
            //    var dfd = currentNodeCE;
            //}

            currentNodeCE.Weight = (int)bestValue;
           


            //Utils.WritelineAsync($"{depth}, {currentNodeCE}");
            return currentNodeCE;
        }



        public void Dispose()
        {
            GC.Collect();
        }
    }

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
                    var index = x + (y * 8);
                    var data = _cases[index];
                    line += $"{data}\t";
                }
                Utils.WritelineAsync(line);
            }
            Utils.WritelineAsync("_____________________________________________________________________");
        }

        public BoardCE()
        {
            for (int i = 0; i < 64; i++)
            {
                _cases[i] = $"__";
            }
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
                    AddInIsKingInCheckList(new IsKingInCheck(_cases, color, true));
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
                            AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
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
                            {
                                //SAVE IN CHESS 
                                AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
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
                            AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
                            return false;
                        }
                        //SAVE IN CHESS
                        AddInIsKingInCheckList(new IsKingInCheck(_cases, color, true));
                        return true; // Le roi est en échec
                    }
                }

                //SAVE IN CHESS
                AddInIsKingInCheckList(new IsKingInCheck(_cases, color, false));
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
                "T" => 60, // Tour // Tour 50 mais pour T126 et T125
                "Q" => 90, // Reine
                "K" => 100, // Roi 10000 to 100 for T136C_W_D5toF6 et T136B_W_D5toF6 Roi
                _ => 0,
            };
        }

        public int GetPieceValue(int index)
        {
           return GetPieceValue(_cases[index].First().ToString());
        }
        public void AddInIsKingInCheckList(IsKingInCheck isKingInCheck)
        {
            //TODO A DECOMMENTER

            //   Générer la clé
            //var key = Utils.GenerateKeyForIsKingInCheck(isKingInCheck.CasesToString, isKingInCheck.KingColor);

            //// Ajouter uniquement si la clé n'existe pas déjà
            //if (!Utils.IsKingInCheckList.ContainsKey(key))
            //{
            //    Utils.IsKingInCheckList.TryAdd(key, isKingInCheck);
            //}
        }

        private static readonly int[] centralSquares = { 28, 29, 36, 37, 44, 45, 52, 53 };

        public int CalculateBoardCEScore(BoardCE board, string color, string opponentColor)
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
        { "K", 100 } // 10000 to 100 for T136C_W_D5toF6 et T136B_W_D5toF6 Roi (valeur arbitraire très élevée pour éviter sa capture)
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
           
            if (index == -1)
                return result;
            var color = GetOpponentColor(opponentColor);
            //T67WhiteIsInChess
            //on vide la case de l'index
            var oldContaine = _cases[index];
            if (_cases[index].EndsWith(opponentColor))
                _cases[index] = _cases[index].Replace($"|{_cases[index].Last()}", $"|{color}");


            // Récupère les indices des pièces adverses
            var opponentPawnIndexList = this.GetCasesIndexForColor(opponentColor);


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
            var allierPawnIndexList = this.GetCasesIndexForColor(allierColor);
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

        public BoardCE CloneAndMove(int fromIndex, int toIndex)
        {
            var clone = new BoardCE(this);
            clone.Move(fromIndex, toIndex);
            return clone;
        }
        public BoardCE CloneAndMove(Move move)
        {
            
            return CloneAndMove(move.FromIndex, move.ToIndex);
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

    public class NodeCE
    {
        public List<NodeCE> EquivalentBestNodeCEList { get; set; }
        public List<NodeCE> AllNodeCEList { get; set; } = new List<NodeCE>();
        public int Weight { get; set; }
        public int Level { get; set; }
        public string Colore { get; set; }
        public int FromIndex { get; set; } // Index de départ (0-63)
        public int ToIndex { get; set; } // Index d'arrivée (0-63)

        public string Location => GetPositionFromIndex(FromIndex); // Position d'origine en notation échiquier
        public string BestChildPosition => GetPositionFromIndex(ToIndex); // Position de destination en notation échiquier

        public BoardCE BoardCE { get; set; }
        public NodeCE MaxNode { get; set; }

        public TimeSpan ReflectionTime { get; set; }
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
            return $"{Level}:   {Location} ({FromIndex}) => {BestChildPosition} ({ToIndex}) : {Weight} ({ReflectionTime})".ToUpper();
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


    public class PossibleMoves
    {
        public string CasesToString { get; set; } = string.Empty;
        // public string[] Cases { get; set; }
        public int FromIndex { get; set; }
        public List<Move> PossibleMovesResult { get; set; }

        public PossibleMoves(string[] cases, int fromIndex, List<Move> possibleMovesResult)
        {
            CasesToString = Utils.CasesToCasesString(cases);
            // Cases = cases;

            FromIndex = fromIndex;
            PossibleMovesResult = possibleMovesResult;

        }
    }


    public class IsKingInCheck
    {
        public string CasesToString { get; set; } = string.Empty;
        // public string[] Cases { get; set; }
        public string KingColor { get; set; }
        public bool IsInChess { get; set; }

        public IsKingInCheck(string[] cases, string kingColor, bool isInChess)
        {
            CasesToString = Utils.CasesToCasesString(cases);
            // Cases = cases;

            KingColor = kingColor;
            IsInChess = isInChess;

        }
    }

    public class SpecificBoardCE { /* Utilisé pour une logique avancée */ }


}
