namespace ChessCore.Tools.ChessEngine.Engine
{



    //SIMPLE ENGINE DEFAULT DEPTH LEVEL 3
    public class ChessEngine1 :  IChessEngine
    {
        public TimeSpan ReflectionTime { get; set; }
        //EMULER l1 ET L3 ET PRENDRE LE PLUS HAUT
        private bool _checkIsInChessOnEnd;
        private int _depthLevel = 3;
        private static object lockObj = new object();

        public string GetName()
        {
            return this.GetType().Name;
        }

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 3)
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
            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DEPTH LEVEL : {depthLevel}");
            var checkIsInChessOnEnd = true;
            if (depthLevel == 5)
                checkIsInChessOnEnd = false;
            var r = RunEngine(colore, boardChess, depthLevel, false, null, checkIsInChessOnEnd);
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
            Utils.WritelineAsync($"{GetName()}");

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

            var l1p = RunEngine(colore, boardChess, 1, false, null, false);
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




        public NodeCE RunEngine(string colore, BoardCE boardChess, int depthLevel, bool isOppinionTurnInNext, List<NodeCE> blackList = null, bool checkIsInChessOnEnd = true, int limitOfReflectionTimeInSecond = 24 * 60 * 60)
        {
            try
            {
                _depthLevel = depthLevel;
                var maxWeight = double.NegativeInfinity;
                colore = colore[0].ToString();
                string opponentColor = colore == "W" ? "B" : "W";
                string iaTurn = depthLevel % 2 == 1 ? opponentColor : colore;
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
                        if (blackList != null)
                        {
                            if (blackList.Count > 0)
                            {
                                var nodeInBlackList = blackList.FirstOrDefault(x => x.FromIndex == node.FromIndex && x.ToIndex == node.ToIndex);
                                if (nodeInBlackList != null)
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
                            || currentClonedBoardCE.TargetIndexIsMenaced(node.ToIndex, opponentColor) && boardChess.GetPieceValue(node.FromIndex) < boardChess.GetPieceValue(node.ToIndex))
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
                currentNodeCE.Weight = board.CalculateBoardCEScore(cpuColor, opponentColor);


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

}
