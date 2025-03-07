using ChessCore.Tools.ChessEngine.Engine;
using ChessCore.Tools;

namespace ChessCore.Test
{
    [TestClass]
    public class ChessEngine3Test
    {
        private string testsDirrectory = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.Combine(Environment.CurrentDirectory)).ToString()).ToString()).ToString(), "TESTS");
        private ChessEngine3 _chessEngine;
        public ChessEngine3Test()
        {
             _chessEngine = new ChessEngine3();
        }

        [TestMethod]
        public void T000aLeKnigntBlanchNeDoitPasAttaquer()
        {
            /*La cavalier blanch ne doit pas attaquer*/
            //Positions final du cavalier Blach ne doit pas etre  ni "a7" ni "c7" 


            var computerColore = "White";

            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
      "Queen;d1;White;False;False;False;False" +
      "\nKing;e1;White;False;True;True;True";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
      "Queen;d8;Black;False;False;False;False" +
      "\nKing;e8;Black;False;True;True;True";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Positions final du cavalier Blach ne doit pas etre  ni "a7" ni "c7"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a7", "c7");
            }
        }


        [TestMethod]
        public void T00aLeKnigntBlanchNeDoitPasAttaquer()
        {
            /*La cavalier blanch ne doit pas attaquer*/
            //Positions final du cavalier Blach ne doit pas etre  ni "a7" ni "c7" 


            var computerColore = "White";

            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "Rook;a1;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nKnight;b5;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nSimplePawn;d2;White;True;False;False;False" +
      "\nKing;e1;White;False;True;True;True" +
      "\nSimplePawn;e2;White;True;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
            "SimplePawn;a7;Black;True;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;c7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;d7;Black;True;False;False;False" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nKing;e8;Black;False;True;True;True" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nBishop;b4;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False" +
      "\nRook;h8;Black;False;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Positions final du cavalier Blach ne doit pas etre  ni "a7" ni "c7"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a7", "c7");
            }
        }



        [TestMethod]
        public void T00aMultithreadingLeKnigntBlanchNeDoitPasAttaquer()
        {
            /*La cavalier blanch ne doit pas attaquer*/
            //Positions final du cavalier Blach ne doit pas etre  ni "a7" ni "c7" 


            var computerColore = "White";





            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "Rook;a1;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nKnight;b5;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nSimplePawn;d2;White;True;False;False;False" +
      "\nKing;e1;White;False;True;True;True" +
      "\nSimplePawn;e2;White;True;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
            "SimplePawn;a7;Black;True;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;c7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;d7;Black;True;False;False;False" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nKing;e8;Black;False;True;True;True" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nBishop;b4;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False" +
      "\nRook;h8;Black;False;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);
            
            {
                // var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Positions final du cavalier Blach ne doit pas etre  ni "a7" ni "c7"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a7", "c7");
            }
        }


        [TestMethod]
        public void T00bLeKnigntNoirNeDoitPasAttaquer()
        {
            /*La cavalier noit ne doit pas attaquer*/
            //Positions final du cavalier noir ne doit pas etre  ni "a2" ni "c2" 


            var computerColore = "Black";




            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "Rook;a1;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nKnight;b5;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nSimplePawn;d2;White;True;False;False;False" +
      "\nKing;e1;White;False;True;True;True" +
      "\nSimplePawn;e2;White;True;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
            "SimplePawn;a7;Black;True;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;c7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;d7;Black;True;False;False;False" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nKing;e8;Black;False;True;True;True" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nBishop;b4;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False" +
      "\nRook;h8;Black;False;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {

                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Positions final du cavalier noir ne doit pas etre  ni "a2" ni "c2" 
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a2", "c2");
            }
        }
        [TestMethod]
        public void T00bMultithreadingLeKnigntNoirNeDoitPasAttaquer()
        {
            /*La cavalier noit ne doit pas attaquer*/
            //Positions final du cavalier noir ne doit pas etre  ni "a2" ni "c2" 


            var computerColore = "Black";




            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "Rook;a1;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nKnight;b5;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nSimplePawn;d2;White;True;False;False;False" +
      "\nKing;e1;White;False;True;True;True" +
      "\nSimplePawn;e2;White;True;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
            "SimplePawn;a7;Black;True;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;c7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;d7;Black;True;False;False;False" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nKing;e8;Black;False;True;True;True" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nBishop;b4;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False" +
      "\nRook;h8;Black;False;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Positions final du cavalier noir ne doit pas etre  ni "a2" ni "c2" 
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a2", "c2");
            }
        }



        [TestMethod]
        public void T01QuenLaReineNoirNeDoitPasPrendreLeCavalier()
        {
            /*La reine noir ne doit pas prendre le cavalier*/
            //Position final de la reine Noir ne doit pas etre "g5"


            var computerColore = "Black";


            var testName = "T01QuenLaReineNoirNeDoitPasPrendreLeCavalier";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Position final de la reine Noir ne doit pas etre "g5"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "g5");
            }
        }


        [TestMethod]
        public void T02aLeRookBlanchNeDoitPasPrendresLePion()
        {
            /*Le Rook blanc ne doit pas prendre le pion*/
            //Position final du Rook blanch  ne doit pas etre "a7"


            var computerColore = "White";





            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "King;e1;White;False;True;True;True" +
      "\nRook;a1;White;False;False;False;False" +
      "\nKnight;b1;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c4;White;False;False;False;False" +
      "\nSimplePawn;d3;White;False;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f4;White;False;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nQueen;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
             "King;d8;Black;False;True;True;True" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;f5;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nSimplePawn;c5;Black;False;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nQueen;a8;Black;False;False;False;False" +
      "\nSimplePawn;a7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Position final du rook blanch  ne doit pas etre "a7"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a7");
            }
        }

        [TestMethod]
        public void T02aMultithreadingLeRookBlanchNeDoitPasPrendresLePion()
        {
            /*Le Rook blanc ne doit pas prendre le pion*/
            //Position final du Rook blanch  ne doit pas etre "a7"


            var computerColore = "White";





            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "King;e1;White;False;True;True;True" +
      "\nRook;a1;White;False;False;False;False" +
      "\nKnight;b1;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c4;White;False;False;False;False" +
      "\nSimplePawn;d3;White;False;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f4;White;False;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nQueen;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
             "King;d8;Black;False;True;True;True" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;f5;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nSimplePawn;c5;Black;False;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nQueen;a8;Black;False;False;False;False" +
      "\nSimplePawn;a7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Position final du rook blanch  ne doit pas etre "a7"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a7");
            }
        }

        [TestMethod]
        public void T02bMultithreadingLeRookNoirNeDoitPasPrendresLePion()
        {
            /*Le Rook noir ne doit pas prendre le pion*/
            //Position final du Rook blanch  ne doit pas etre "h2"


            var computerColore = "Black";




            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "" +
              "King;e1;White;False;True;True;True" +
      "\nRook;a1;White;False;False;False;False" +
      "\nKnight;b1;White;False;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nSimplePawn;c4;White;False;False;False;False" +
      "\nSimplePawn;d3;White;False;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nSimplePawn;f4;White;False;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nQueen;h1;White;False;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
             "King;d8;Black;False;True;True;True" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nSimplePawn;f5;Black;False;False;False;False" +
      "\nSimplePawn;e6;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nSimplePawn;c5;Black;False;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nQueen;a8;Black;False;False;False;False" +
      "\nSimplePawn;a7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Position final du rook blanch  ne doit pas etre "a7"
                Assert.AreNotEqual(nodeResult.BestChildPosition, "h2");
            }
        }

        [TestMethod]
        public void T05LeFousBlacheDoitSeSaccrifierPourProtegerLeRook()
        {
            /*Le Fous blanc doit attaquer le pion noir
             * les noirs*/
            //Position final du Fous blanch  doit etre "a7"





            var computerColore = "White";





            var testName = "T05LeFousBlacheDoitSeSaccrifierPourProtegerLeRook";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "c4");
            }
        }



        [TestMethod]
        public void T07aEchecRookBlancDoitAttaquerLeRoiNoir()
        {
            /*Le Rook blanc doit attaquer le roir noir pout  Tout de suite mettre en echec 
             * les noirs*/
            //Position final du rook blanch  doit etre "d8"
            var computerColore = "White";
            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();


            //WHITEList
            var whiteListString = "King;h4;White;False;False;False;True" +
      "\nQueen;e1;White;False;False;False;False" +
      "\nRook;d5;White;False;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;a3;White;False;False;False;False" +
      "\nSimplePawn;c3;White;False;False;False;False" +
      "\nSimplePawn;e3;White;False;False;False;False" +
      "\nSimplePawn;g4;White;False;False;False;False" +
      "\nSimplePawn;h3;White;False;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "King;g8;Black;False;False;False;False" +
              "\nQueen;g6;Black;False;False;False;False" +
      "\nRook;c6;Black;False;False;False;False" +
      "\nKnight;a7;Black;False;False;False;False" +
      "\nSimplePawn;c7;Black;True;False;False;False" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }


            var pawnList = new List<Pawn>();
            pawnList.AddRange(pawnListWhite);
            pawnList.AddRange(pawnListBlack);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreEqual((nodeResult.EquivalentBestNodeCEList, 1););
                //rook blanch  doit etre "d8"
                // Assert.AreEqual(nodeResult.AssociatePawn.Name, "Rook");
                Assert.AreEqual(nodeResult.BestChildPosition, "d8");
            }
        }


        //  [TestMethod]
        //  public void T07bEchecRookOuReineBlancDoitAttaquerLeRoiNoir()
        //  {
        //      /*Le Rook ou la reinne blanc doit attaquer le roir noir pout  Tout de suite mettre en echec 
        //       * les noirs*/
        //      //Position final  blanche  doit etre "d8" ou "e8"





        //      var computerColore = "White";





        //      var pawnListWhite = new List<Pawn>();
        //      var pawnListBlack = new List<Pawn>();


        //      //WHITEList
        //      var whiteListString = "King;h4;White;False;False;False;True" +
        //"\nQueen;e1;White;False;False;False;False" +
        //"\nRook;d5;White;False;False;False;False" +
        //"\nRook;h1;White;False;False;False;False" +
        //"\nKnight;g1;White;False;False;False;False";
        //      var whiteList = whiteListString.Split('\n');
        //      foreach (var line in whiteList)
        //      {
        //          var datas = line.Split(';');
        //          var newPawn = new Pawn(datas[0], datas[1], datas[2]);
        //          //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
        //          newPawn.IsFirstMove = bool.Parse(datas[3]);
        //          newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
        //          newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
        //          newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
        //          pawnListWhite.Add(newPawn);
        //      }

        //      //BLACKList
        //      var blackListString = "King;g8;Black;False;True;False;True" +
        //        "\nQueen;g6;Black;False;False;False;False" +
        //"\nRook;c6;Black;False;False;False;False" +
        //"\nKnight;a7;Black;False;False;False;False" +
        //"\nSimplePawn;c7;Black;True;False;False;False" +
        //"\nSimplePawn;f7;Black;True;False;False;False" +
        //"\nSimplePawn;g7;Black;True;False;False;False" +
        //"\nSimplePawn;h7;Black;True;False;False;False";
        //      var blackList = blackListString.Split('\n');
        //      foreach (var line in blackList)
        //      {
        //          var datas = line.Split(';');
        //          var newPawn = new Pawn(datas[0], datas[1], datas[2]);
        //          //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
        //          newPawn.IsFirstMove = bool.Parse(datas[3]);
        //          newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
        //          newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
        //          newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
        //          pawnListBlack.Add(newPawn);
        //      }


        //      var pawnList = new List<Pawn>();
        //      pawnList.AddRange(pawnListWhite);
        //      pawnList.AddRange(pawnListBlack);

        //      using (var chessEngine = new ChessEngine22())
        //      {
        //          var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
        //          //Position final blanche  doit etre "d8" ou "e8"
        //          //Assert.AreEqual(nodeResult.AssociatePawn.Name, "Rook");
        //          Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 2);

        //          var isSucces = false;
        //          if (nodeResult.BestChildPosition == "d8" || nodeResult.BestChildPosition == "e8")
        //              isSucces = true;
        //          Assert.IsTrue(isSucces);
        //      }
        //  }


        [TestMethod]
        public void T11LaReineBlancNeDoitPasAttaqueLePion()
        {
            /*La reine blanche ne doit pas attaquer le pion noir en g6*/
            //la reine blanche ne doit se mettre sur "g6"





            var computerColore = "White";


            var testName = "T11LaReineBlancNeDoitPasAttaqueLePion";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "g6");
            }
        }


        [TestMethod]
        public void T15LaReineBlanchNeDoitPasPrendreLePion()
        {
            /*La reine blanche ne doit pas attaquer le pion noir en a6*/
            ////la reine blanche ne doit se mettre sur "a6"
            var computerColore = "White";
            var testName = "T15LaReineBlanchNeDoitPasPrendreLePion";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {

                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a6");
            }
        }


        [TestMethod]
        public void T16SeulLePionDoitProtegerLeRoiNoir()
        {
            /*seul le poin doit prot�ger le roi noir*/
            ////le poin noir doit se mettre sur "f6"
            var computerColore = "Black";
            var testName = "T16SeulLePionDoitProtegerLeRoiNoir";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual(nodeResult.Location, "g4");
                Assert.AreEqual(nodeResult.BestChildPosition, "f6");
            }
        }


        [TestMethod]
        public void T17LeRoirNoirNeDoitPasAttaquer()
        {
            /*le roi noir ne doit pas attaquer */
            ////le roi noir ne doit pas se mettre sur "f6"





            var computerColore = "Black";


            var testName = "T17LeRoirNoirNeDoitPasAttaquer";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");




                Assert.AreNotEqual(nodeResult.BestChildPosition, "f6");
            }
        }

        [TestMethod]
        public void T18suiteDe16LeCavalierNoirDoitPrendreLeFouBlanc()//TODO TO VERIFAI
        {
            /*le cavalier noir  doit  attaquer */
            ////le cavalier noir  doit se mettre sur "f6"





            var computerColore = "Black";




            var testName = "T18suiteDe16LeCavalierNoirDoitPrendreLeFouBlanc";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");





                Assert.AreEqual(nodeResult.BestChildPosition, "f6");
            }
        }


        [TestMethod]
        public void T19bLeFouBlanchDoitnenacerLaReineOulePionDoitProtegerLeTour()
        {
            ////////le poin blanch doit se mettre sur ""





            var computerColore = "White";





            var testName = "T19bLeFouBlanchDoitnenacerLaReineOulePionDoitProtegerLeTour";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");






                var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                ////////le poin blanch doit se mettre sur "c3"
                var isSucces = false;
                if (nodeResult.BestChildPosition == "d4" || nodeResult.BestChildPosition == "c3" || nodeResult.BestChildPosition == "a3")
                    isSucces = true;
                Assert.IsTrue(isSucces);
            }
        }



        [TestMethod]
        public void T20LePionDoitPrendreLeCavalier()
        {
            /*le pion blanch doit prendre le cavalier */
            ////le pion blanch doit se mettre sur "d3"





            var computerColore = "White";




            var testName = "T20LePionDoitPrendreLeCavalier";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "d3");
            }
        }


        [TestMethod]
        public void T21LeRoiBlanchDoitSeMettreEnd3()
        {
            /*La roi blanch doit se mettre en d3*/
            //Positions final du roi blanch doit etre d3 


            var computerColore = "White";






            var testName = "T21LaNoirBlancheDoitSeMettreEnD3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.BestChildPosition, "d3");
            }
        }

        [TestMethod]
        public void T22LeBishopOuLeRoiNoirDoitPrendreLePion() //OK IF L3 (-10)
        {
            /*Le Bishop Noir Doit Prendre le pion */
            ////le Bishop noir  doit se mettre sur "e7"

            var computerColore = "Black";

            var testName = "T22LeBishopOuLeRoiNoirDoitPrendreLePion";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                // chessEngine.DeepLevel = 5;
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.AreEqual(nodeResult.BestChildPosition, "e7");
            }
        }


        [TestMethod]
        public void T23LeCavalierNoirNeDoitPasMenacerLeRoiBlanch()
        {
            /*Le Cavalier noir ne doit pas menacer le roi blanc*/
            ////le Cavalier noir ne doit pas se mettre sur "b3"





            var computerColore = "Black";
            var testName = "T23LeCavalierNoirNeDoitPasMenacerLeRoiBlanch";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "b3");
            }
        }


        [TestMethod]
        public void T24LeCavalierNoirNeDoitPasBouger()
        {
            /*Le Cavalier noir ne doit pas bouger*/





            var computerColore = "Black";
            var testName = "T24LeCavalierNoirNeDoitPasBouger";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult.Location, "a1");
            }

        }

        //[TestMethod]
        //public void T25LeCavalierNoirDoitMenacerLeRoiBlanch_IGNOR()//NOT
        //{
        //    /*Le Cavalier noir doit mencer le roi blanc*/



        //    var computerColore = "Black";
        //    var testName = "T25LeCavalierNoirDoitMenacerLeRoiBlanch";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new ChessEngine22())
        //    {
        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

        //        //Assert.AreEqual(nodeResult.Location, "g4");

        //        var isSuccess = false;
        //        string[] accepdedArray = { "a6", "a5", "h6", "d6", "g6", "f6" };

        //        if (accepdedArray.Contains(nodeResult.BestChildPosition))
        //            isSuccess = true;
        //        Assert.IsTrue(isSuccess);
        //          }

        //}

        [TestMethod]
        public void T26LeCavalierNoirDoitBouger()
        {
            /*Le Cavalier noir en f6 doit se deplacer*/





            var computerColore = "Black";

            var testName = "T26LeCavalierNoirDoitBouger";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.AreEqual(nodeResult.Location, "f6");
            }

        }
        [TestMethod]
        public void T01MultiThreadCPUsPawn()
        {

            var computerColore = "White";
            var testName = "T01QuenLaReineNoirNeDoitPasPrendreLeCavalier";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.AreNotEqual(nodeResult.BestChildPosition, "g5");
            }

        }



        [TestMethod]
        public void T27LeBishopBlancDoitSeMettreEnA8()
        {
            /*Le Bishop blanc doit attaque le rook en a8 et non pas le cavalier*/
            // Le Bishop blanc doit se mettre en a8




            var computerColore = "White";






            var testName = "T27LeBishopBlancDoitSeMettreEnA8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.BestChildPosition == "a8" || nodeResult.BestChildPosition == "g7");
            }
        }



        [TestMethod]
        public void T28LePionNoirDoitPrendreLeCavalier()
        {
            /*Le pion noir doit attaque le cavavier en d6*/
            // Le pion noir doit se mettre en d6




            var computerColore = "Black";
            var testName = "T28LePionNoirDoitPrendreLeCavalier";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "d6");
            }
        }

        [TestMethod]
        public void T29_W_PourProtegerDEchec()// TDOD EDIT NOT OK
        {
            //ONLY POSITION D1 OR F1
            /*Le pion blanch doit se mettre en h4 ou le Bishop doit se mettre en g2 ou reine en c2  pour proder de l'check*/
            // Le pion blanc doit se mettre en h4 ou le Bishop doit se mettre en g2 ou ou reine en c2
            var computerColore = "White";
            var testName = "T29PourProtegerDEchec";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.Location == "f1" || nodeResult.Location == "d1" || nodeResult.BestChildPosition == "g5");

            }
        }

        [TestMethod]
        public void T30LaReineNoirNeDoitPasPrendreLeFouEnG4()
        {
            /*La reinne noir ne doit pas se mettre en g4*/
            // La reinne noir ne doit pas se mettre en g4




            var computerColore = "Black";
            var testName = "T30LaReineNoirNeDoitPasPrendreLeFouEnG4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "g4");
            }


        }


        [TestMethod]
        public void T31_B_D8toA8()
        {
            /*La reinne noir ne doit pas se mettre en a8*/




            var computerColore = "Black";

            var testName = "T31LaReineNoirDoitPrendreLaReineBlanch";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "a8");
            }
        }



        [TestMethod]
        public void T32LaReineBlanchDoitAttaquerEnB7()
        {
            var computerColore = "White";
            var testName = "T32LaReineBlanchDoitAttaquerEnB7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "b7");
            }


        }
        [TestMethod]
        public void T33LaReineBlanchDoitAttaquerEnB7()
        {





            var computerColore = "White";
            var testName = "T33LaReineBlanchDoitAttaquerEnB7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "b7");
            }
        }

        [TestMethod]
        public void T35LePoinNoirNeDoitPasSeMettreEnG5()
        {
            var computerColore = "Black";
            var testName = "T35LePoinNoirNeDoitPasSeMettreEnG5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "g5");
            }



        }


        [TestMethod]
        public void T36LePoinNoirNeDoitPasSeMettreEnD6()
        {





            var computerColore = "Black";


            var testName = "T36LePoinNoirNeDoitPasSeMettreEnD6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "d6");
            }


        }


        [TestMethod]
        public void T37LaTourDoitEtreProtegE()
        {
            var computerColore = "Black";
            var testName = "T37LaTourDoitEtreProtegE";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.AreEqual(nodeResult.BestChildPosition, "c6");
            }


        }

        [TestMethod]
        public void T38LePionNoirNeDoitPasSeMettreSurA3()
        {

            var computerColore = "Black";

            var testName = "T38LePionNoirNeDoitPasSeMettreSurA3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult.BestChildPosition, "a3");
            }
        }

        [TestMethod]
        public void T39LePionNoirNeDoitPasSeMettreSurC4()
        {

            var computerColore = "Black";

            var testName = "T39LePionNoirNeDoitPasSeMettreSurC4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult.BestChildPosition, "c3");
            }
        }


        [TestMethod]
        public void T40LePionNoirNeDoitPasSeMettreSurA2()
        {

            var computerColore = "Black";
            var testName = "T40LePionNoirNeDoitPasSeMettreSurA2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult, "a2");
            }
        }


        [TestMethod]
        public void T41LaReineBlancheDoitMenacerLeRoiEnH5OuPrendreLaReineD6()
        {

            var computerColore = "White";
            var testName = "T41LaReineBlancheDoitMenacerLeRoiEnH5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                // var result = nodeResult.NodeRandomList.FirstOrDefault(x => x.BestChildPosition == "a2");

                var isSucces = false;
                if (nodeResult.BestChildPosition == "h5" || nodeResult.BestChildPosition == "d6")
                    isSucces = true;
                Assert.IsTrue(isSucces);
            }
        }


        [TestMethod]
        public void T44LePionNoirPasRamdumeB5()
        {

            var computerColore = "Black";
            var testName = "T44LePionNoirPasRamdumeB5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                // var  T_value = nodeResult.Weight;
                //var result = nodeResult.NodeRandomList.FirstOrDefault(x => x.BestChildPosition == "b5");
                Assert.AreNotEqual(nodeResult.BestChildPosition, "b5");
            }
        }

        #region TODO SpecificPosition

        /*

          [TestMethod]
        public void T51aLeFouBlanchDoiSeMettreSurH5SecificPosition1()
        {
            //Pour les SpecificPositions il faut se connecter � la base

            //Pour les SpecificPositions il faut se connecter � la base
            //mainWindow.SetIsConnectedDB(true);
            var computerColore = "White";





            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();



            //WHITEList
            var whiteListString = "" +
      "King;g1;White;False;True;True;True" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nRook;a1;White;False;False;False;False" +
      "\nRook;f1;White;False;False;False;False" +
      "\nKnight;b1;White;False;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nBishop;e2;White;False;False;False;False" +
      "\nKnight;h4;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nSimplePawn;c4;White;False;False;False;False" +
      "\nSimplePawn;e4;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
      "King;e8;Black;False;True;True;True" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nBishop;b7;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nKnight;g8;Black;False;False;False;False" +
      "\nSimplePawn;a5;Black;False;False;False;False" +
      "\nSimplePawn;b6;Black;False;False;False;False" +
      "\nSimplePawn;c5;Black;False;False;False;False" +
      "\nSimplePawn;d7;Black;True;False;False;False" +
      "\nSimplePawn;d5;Black;False;False;False;False" +
      "\nSimplePawn;f6;Black;False;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nSimplePawn;h5;Black;False;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();pawnList.AddRange(pawnListWhite);pawnList.AddRange(pawnListBlack);

            using (var chessEngine = new chessEngine())
            {
                var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);


                Assert.AreEqual(nodeResult.BestChildPosition, "h5");
            }
        }


        [TestMethod]
        public void T51bLeFouNoirDoitSeMettreSurH4SecificPosition1Symetri()
        {
            //Pour les SpecificPositions il faut se connecter � la base

            //Pour les SpecificPositions il faut se connecter � la base
            //mainWindow.SetIsConnectedDB(true);
            var computerColore = "Black";






            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();



            //WHITEList
            var whiteListString = "" +
      "King;e1;White;False;True;True;True" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nRook;a1;White;False;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nKnight;b1;White;False;False;False;False" +
      "\nBishop;b2;White;False;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nKnight;g1;White;False;False;False;False" +
      "\nSimplePawn;a4;White;False;False;False;False" +
      "\nSimplePawn;b3;White;False;False;False;False" +
      "\nSimplePawn;c4;White;False;False;False;False" +
      "\nSimplePawn;d2;White;True;False;False;False" +
      "\nSimplePawn;d4;White;False;False;False;False" +
      "\nSimplePawn;f3;White;False;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nSimplePawn;h4;White;False;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
      "King;g8;Black;False;True;True;True" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nRook;f8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nBishop;e7;Black;False;False;False;False" +
      "\nKnight;h5;Black;False;False;False;False" +
      "\nSimplePawn;a7;Black;True;False;False;False" +
      "\nSimplePawn;b7;Black;True;False;False;False" +
      "\nSimplePawn;c5;Black;False;False;False;False" +
      "\nSimplePawn;e5;Black;False;False;False;False" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();pawnList.AddRange(pawnListWhite);pawnList.AddRange(pawnListBlack);

            using (var chessEngine = new chessEngine())
            {
                var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);


                Assert.AreEqual(nodeResult.BestChildPosition, "h4");
            }
        }

        [TestMethod]
        public void T45LeChevalierBlacnDoitSeNettreEnG5SpecificPosition0()
        {


            //Pour les SpecificPositions il faut se connecter � la base
           // //mainWindow.SetIsConnectedDB(true);


            var computerColore = "White";






            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();



            //WHITEList
            var whiteListString = "" +
      "King;e1;White;False;True;True;True" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nRook;a1;White;False;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nBishop;c4;White;False;False;False;False" +
      "\nKnight;c3;White;False;False;False;False" +
      "\nKnight;f3;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nSimplePawn;d4;White;False;False;False;False" +
      "\nSimplePawn;e4;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
      "King;e8;Black;False;True;True;True" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nKnight;f6;Black;False;False;False;False" +
      "\nSimplePawn;a5;Black;False;False;False;False" +
      "\nSimplePawn;b6;Black;False;False;False;False" +
      "\nSimplePawn;c6;Black;False;False;False;False" +
      "\nSimplePawn;d6;Black;False;False;False;False" +
      "\nSimplePawn;e7;Black;True;False;False;False" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();pawnList.AddRange(pawnListWhite);pawnList.AddRange(pawnListBlack);

            using (var chessEngine = new chessEngine())
            {
                var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);

                // var  T_value = nodeResult.Weight;
                //var result = nodeResult.NodeRandomList.FirstOrDefault(x => x.BestChildPosition == "b5");
                Assert.AreEqual(nodeResult.BestChildPosition, "g5");
            }
        }



        [TestMethod]
        public void T46LeFouBlacnDoitSeNettreEncC4SpecificPosition0()
        {
            //Pour les SpecificPositions il faut se connecter � la base

            //Pour les SpecificPositions il faut se connecter � la base
            ////mainWindow.SetIsConnectedDB(true);


            var computerColore = "White";





            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();



            //WHITEList
            var whiteListString = "" +
      "King;e1;White;False;True;True;True" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nRook;a1;White;False;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nBishop;f1;White;False;False;False;False" +
      "\nKnight;c3;White;False;False;False;False" +
      "\nKnight;g5;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nSimplePawn;d4;White;False;False;False;False" +
      "\nSimplePawn;e4;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
      "King;e8;Black;False;True;True;True" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nKnight;f6;Black;False;False;False;False" +
      "\nSimplePawn;a5;Black;False;False;False;False" +
      "\nSimplePawn;b6;Black;False;False;False;False" +
      "\nSimplePawn;c6;Black;False;False;False;False" +
      "\nSimplePawn;d6;Black;False;False;False;False" +
      "\nSimplePawn;e7;Black;True;False;False;False" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();pawnList.AddRange(pawnListWhite);pawnList.AddRange(pawnListBlack);

            using (var chessEngine = new chessEngine())
            {
                var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);

                // var  T_value = nodeResult.Weight;
                //var result = nodeResult.NodeRandomList.FirstOrDefault(x => x.BestChildPosition == "b5");
                Assert.AreEqual(nodeResult.BestChildPosition, "c4");
            }
        }


        [TestMethod]
        public void T47LesNoirsDoiventEviterLeSpecificPosition0()
        {
            //Pour les SpecificPositions il faut se connecter � la base

            //Pour les SpecificPositions il faut se connecter � la base
            //mainWindow.SetIsConnectedDB(true);
            var computerColore = "Black";




            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();



            //WHITEList
            var whiteListString = "" +
      "King;e1;White;False;True;True;True" +
      "\nQueen;d1;White;False;False;False;False" +
      "\nRook;a1;White;False;False;False;False" +
      "\nRook;h1;White;False;False;False;False" +
      "\nBishop;c1;White;False;False;False;False" +
      "\nBishop;c4;White;False;False;False;False" +
      "\nKnight;c3;White;False;False;False;False" +
      "\nKnight;f3;White;False;False;False;False" +
      "\nSimplePawn;a2;White;True;False;False;False" +
      "\nSimplePawn;b2;White;True;False;False;False" +
      "\nSimplePawn;c2;White;True;False;False;False" +
      "\nSimplePawn;d4;White;False;False;False;False" +
      "\nSimplePawn;e4;White;False;False;False;False" +
      "\nSimplePawn;f2;White;True;False;False;False" +
      "\nSimplePawn;g2;White;True;False;False;False" +
      "\nSimplePawn;h2;White;True;False;False;False";
            var whiteList = whiteListString.Split('\n');
            foreach (var line in whiteList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListWhite.Add(newPawn);
            }

            //BLACKList
            var blackListString = "" +
      "King;e8;Black;False;True;True;True" +
      "\nQueen;d8;Black;False;False;False;False" +
      "\nRook;a8;Black;False;False;False;False" +
      "\nRook;h8;Black;False;False;False;False" +
      "\nBishop;c8;Black;False;False;False;False" +
      "\nBishop;f8;Black;False;False;False;False" +
      "\nKnight;b8;Black;False;False;False;False" +
      "\nKnight;f6;Black;False;False;False;False" +
      "\nSimplePawn;a5;Black;False;False;False;False" +
      "\nSimplePawn;b6;Black;False;False;False;False" +
      "\nSimplePawn;c6;Black;False;False;False;False" +
      "\nSimplePawn;d6;Black;False;False;False;False" +
      "\nSimplePawn;e7;Black;True;False;False;False" +
      "\nSimplePawn;f7;Black;True;False;False;False" +
      "\nSimplePawn;g7;Black;True;False;False;False" +
      "\nSimplePawn;h7;Black;True;False;False;False";
            var blackList = blackListString.Split('\n');
            foreach (var line in blackList)
            {
                var datas = line.Split(';');
                var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                newPawn.IsFirstMove = bool.Parse(datas[3]);
                newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                pawnListBlack.Add(newPawn);
            }



            var pawnList = new List<Pawn>();pawnList.AddRange(pawnListWhite);pawnList.AddRange(pawnListBlack);

            using (var chessEngine = new chessEngine())
            {
                var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);
                var isSucces = false;
                if (nodeResult.BestChildPosition == "g4" || nodeResult.BestChildPosition == "e6" || nodeResult.BestChildPosition == "h6" || nodeResult.BestChildPosition == "c8" || nodeResult.BestChildPosition == "c7")
                    isSucces = true;
                Assert.IsTrue(isSucces);
            }
        }
        */
        #endregion

        [TestMethod]
        public void T49LesNoirsDoiventprotegerLeRoiMenacE()
        {

            var computerColore = "Black";



            var testName = "T49LesNoirsDoiventprotegerLeRoiMenacE";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                // var randomList = nodeResult.EquivalentBestNodeCEList;
                // Assert.IsNull(randomList);


                Assert.AreEqual(nodeResult.BestChildPosition, "e2");
            }
        }


        [TestMethod]
        public void T50LaToureNoirDoitSeMettreEnA7()
        {

            var computerColore = "Black";
            var testName = "T50LaToureNoirDoitSeMettreEnA7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "a7");
            }
        }



        [TestMethod]
        /*tsiry;26-08-2021*/
        public void T52LaReineNoirNeDoitPasSeMettreEnC2()
        {

            var computerColore = "Black";
            var testName = "T52LaReineNoirNeDoitPasSeMettreEnC2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "c2");
            }
        }


        [TestMethod]
        /*tsiry;26-08-2021*/
        public void T53LaPositionFinalNoirNeDoitEtreE6OuE5()
        {

            var computerColore = "Black";

            var testName = "T53LaPositionFinalNoirNeDoitEtreE6OuE5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                //var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual(nodeResult.BestChildPosition, "h1");
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var isSucces = (nodeResult.BestChildPosition == "e6" || nodeResult.BestChildPosition == "e5");
                Assert.IsFalse(isSucces);

            }



        }


        [TestMethod]
        /*tsiry;27-08-2021*/
        public void T54A_W_A1toB1_PreventTheEvolutionOfThePawnInB2()
        {

            var computerColore = "White";
            var testName = "T54ALesBlanchDoiventEviterLEvolutionDuPion";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                //var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual(nodeResult.BestChildPosition, "h1");
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);

                Assert.AreEqual(nodeResult.BestChildPosition, "b1");
            }
        }


        [TestMethod]
        /*tsiry;27-08-2021*/
        public void T54B_W_D3toG6()//OK IF L3 (70)
        {

            var computerColore = "White";
            var testName = "T54BLaTourBlancDoitDoitSeMettreEnA2CarB1EstDejaMenaC";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                // Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 2);
                Assert.IsTrue("b1" == nodeResult.BestChildPosition || "g6" == nodeResult.BestChildPosition);
            }
        }


        [TestMethod]
        /*tsiry;27-08-2021*/
        public void T54C_B_B2toA1_Evolution()
        {

            var computerColore = "Black";

            var testName = "T54CLePionNoirDoitEvoluerDoitSeMettreEnA1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "a1");
            }
        }


        [TestMethod]
        /*tsiry;27-08-2021*/
        public void T54E_B_B2toA1_Evolution()
        {

            var computerColore = "Black";
            var testName = "T54ELePionNoirDoitEvoluerDoitSeMettreEnA1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.BestChildPosition, "a1");
            }
        }
        [TestMethod]
        /*tsiry;14-10-2021*/
        public void T54F_B_B2toA1_Evolution()
        {

            var computerColore = "Black";
            var testName = "T54FLaReineNoirDoitMenaverLeRoiBlanc";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var randomList = nodeResult.EquivalentBestNodeCEList;
                var isSucces = true;
                if (randomList != null)
                {
                    //  Pour  Tester que  T69
                    foreach (var item in randomList)
                    {
                        if (item.ToIndex == 41)
                        {
                            isSucces = false;
                            break;
                        }
                    }
                }
                Assert.IsTrue(isSucces);




                isSucces = false;
                if (nodeResult.BestChildPosition == "d6" || nodeResult.BestChildPosition == "a1" || nodeResult.BestChildPosition == "b3")
                    isSucces = true;
                Assert.IsTrue(isSucces);
            }
        }





        [TestMethod]
        /*tsiry;27-08-2021*/
        public void T54D_B_B2toB1_Evolution()//  T54DLaReinneNoirDoitSeMettreEnC4()
        {

            var computerColore = "Black";
            var testName = "T54DLaReinneNoirDoitSeMettreEnC4OuB1OuA5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                //var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual(nodeResult.BestChildPosition, "h1");
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue(nodeResult.BestChildPosition == "b1" || nodeResult.BestChildPosition == "c4");
            }
        }


        [TestMethod]
        /*tsiry;27-08-2021*/
        public void T54G_B_B2toB1_Evolution()//  T54DLaReinneNoirDoitSeMettreEnC4()
        {

            var computerColore = "Black";





            var testName = "T54GLaReinneNoirDoitAttaquerEnC4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                //var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual(nodeResult.BestChildPosition, "h1");
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.IsTrue(nodeResult.BestChildPosition == "b1" || nodeResult.BestChildPosition == "c4");


            }

            // 


        }


        /*tsiry;30-08-2021
         * DOTO
         */
        /*  
           [TestMethod]public void T55SecificPositionLeCavalierBlanchDoitSeMettreSurG6()
           {

               var computerColore = "White";





               var pawnListWhite = new List<Pawn>();
               var pawnListBlack = new List<Pawn>();



               //WHITEList
               var whiteListString = "" +
         "King;e1;White;False;True;True;True" +
         "\nQueen;d1;White;False;False;False;False" +
         "\nRook;a1;White;False;False;False;False" +
         "\nRook;h1;White;False;False;False;False" +
         "\nKnight;b5;White;False;False;False;False" +
         "\nBishop;c1;White;False;False;False;False" +
         "\nBishop;c4;White;False;False;False;False" +
         "\nKnight;h4;White;False;False;False;False" +
         "\nSimplePawn;a3;White;False;False;False;False" +
         "\nSimplePawn;b2;White;True;False;False;False" +
         "\nSimplePawn;c2;White;True;False;False;False" +
         "\nSimplePawn;d4;White;False;False;False;False" +
         "\nSimplePawn;e5;White;False;False;False;False" +
         "\nSimplePawn;f2;White;True;False;False;False" +
         "\nSimplePawn;g2;White;True;False;False;False" +
         "\nSimplePawn;h2;White;True;False;False;False";
               var whiteList = whiteListString.Split('\n');
               foreach (var line in whiteList)
               {
                   var datas = line.Split(';');
                   var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                   //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                   newPawn.IsFirstMove = bool.Parse(datas[3]);
                   newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                   newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                   newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                   pawnListWhite.Add(newPawn);
               }

               //BLACKList
               var blackListString = "" +
         "King;e7;Black;False;False;True;True" +
         "\nQueen;d8;Black;False;False;False;False" +
         "\nRook;a8;Black;False;False;False;False" +
         "\nRook;h8;Black;False;False;False;False" +
         "\nKnight;b8;Black;False;False;False;False" +
         "\nBishop;c8;Black;False;False;False;False" +
         "\nBishop;f8;Black;False;False;False;False" +
         "\nKnight;g8;Black;False;False;False;False" +
         "\nSimplePawn;a4;Black;False;False;False;False" +
         "\nSimplePawn;b6;Black;False;False;False;False" +
         "\nSimplePawn;c7;Black;True;False;False;False" +
         "\nSimplePawn;d7;Black;True;False;False;False" +
         "\nSimplePawn;e6;Black;False;False;False;False" +
         "\nSimplePawn;f5;Black;False;False;False;False" +
         "\nSimplePawn;g7;Black;True;False;False;False" +
         "\nSimplePawn;h6;Black;False;False;False;False";
               var blackList = blackListString.Split('\n');
               foreach (var line in blackList)
               {
                   var datas = line.Split(';');
                   var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                   //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                   newPawn.IsFirstMove = bool.Parse(datas[3]);
                   newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                   newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                   newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                   pawnListBlack.Add(newPawn);
               }



               var pawnList = new List<Pawn>();pawnList.AddRange(pawnListWhite);pawnList.AddRange(pawnListBlack);
               using (var chessEngine = new chessEngine())
               {
                   var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);


                   Assert.AreEqual(nodeResult.BestChildPosition, "g6");
               }
           }

       */

        /* [TestMethod]
        //TODO A DECOMMENTER
         public void T56ToNotSecificPositionMesNoirDoiventEmpecherLeCavalierBlanchDeSeMettreSurG6()
         {

             var computerColore = "Black";





             var pawnListWhite = new List<Pawn>();
             var pawnListBlack = new List<Pawn>();



             //WHITEList
             var whiteListString = "" +
       "King;e1;White;False;True;True;True" +
       "\nQueen;d1;White;False;False;False;False" +
       "\nRook;a1;White;False;False;False;False" +
       "\nRook;h1;White;False;False;False;False" +
       "\nKnight;b5;White;False;False;False;False" +
       "\nBishop;c1;White;False;False;False;False" +
       "\nBishop;c4;White;False;False;False;False" +
       "\nKnight;h4;White;False;False;False;False" +
       "\nSimplePawn;a3;White;False;False;False;False" +
       "\nSimplePawn;b2;White;True;False;False;False" +
       "\nSimplePawn;c2;White;True;False;False;False" +
       "\nSimplePawn;d4;White;False;False;False;False" +
       "\nSimplePawn;e5;White;False;False;False;False" +
       "\nSimplePawn;f2;White;True;False;False;False" +
       "\nSimplePawn;g2;White;True;False;False;False" +
       "\nSimplePawn;h2;White;True;False;False;False";
             var whiteList = whiteListString.Split('\n');
             foreach (var line in whiteList)
             {
                 var datas = line.Split(';');
                 var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                 //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                 newPawn.IsFirstMove = bool.Parse(datas[3]);
                 newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                 newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                 newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                 pawnListWhite.Add(newPawn);
             }

             //BLACKList
             var blackListString = "" +
       "King;e7;Black;False;False;True;True" +
       "\nQueen;d8;Black;False;False;False;False" +
       "\nRook;a8;Black;False;False;False;False" +
       "\nRook;h8;Black;False;False;False;False" +
       "\nKnight;b8;Black;False;False;False;False" +
       "\nBishop;c8;Black;False;False;False;False" +
       "\nBishop;f8;Black;False;False;False;False" +
       "\nKnight;g8;Black;False;False;False;False" +
       "\nSimplePawn;a4;Black;False;False;False;False" +
       "\nSimplePawn;b6;Black;False;False;False;False" +
       "\nSimplePawn;c7;Black;True;False;False;False" +
       "\nSimplePawn;d7;Black;True;False;False;False" +
       "\nSimplePawn;e6;Black;False;False;False;False" +
       "\nSimplePawn;f5;Black;False;False;False;False" +
       "\nSimplePawn;g7;Black;True;False;False;False" +
       "\nSimplePawn;h6;Black;False;False;False;False";
             var blackList = blackListString.Split('\n');
             foreach (var line in blackList)
             {
                 var datas = line.Split(';');
                 var newPawn = new Pawn(datas[0], datas[1], datas[2]);
                 //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                 newPawn.IsFirstMove = bool.Parse(datas[3]);
                 newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                 newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                 newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                 pawnListBlack.Add(newPawn);
             }



             var pawnList = new List<Pawn>(); pawnList.AddRange(pawnListWhite); pawnList.AddRange(pawnListBlack);

             using (var chessEngine = new chessEngine())
             {
                 var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                 Assert.AreNotEqual(nodeResult.BestChildPosition, "c6");
                 Assert.AreNotEqual(nodeResult.BestChildPosition, "b7");
             }
         }
    */
        [TestMethod]
        /*tsiry;30-08-2021*/
        public void T57_B_A8toNothing()
        {


            var computerColore = "Black";

            var testName = "T57SeulLeRookDoitBougerNotRandomD5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.Location, "a8");
            }
        }


        /*tsiry;06-10-2021*/
        [TestMethod]
        //TODO TO IMPLEMENTE
        public void T59FinDePartieEviterMortDuRoiNoir__IGNOR_TO_IMPLEMENT()
        {


            var computerColore = "Black";

            var testName = "T59FinDePartieEviterMortDuRoiNoir";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                var randomList = nodeResult.EquivalentBestNodeCEList;

                Assert.AreNotEqual(nodeResult.BestChildPosition, "g1");
            }
        }


        [TestMethod]
        public void T61aPourBestSpecificPosition3White()
        {
            //Pour les SpecificPositions il faut se connecter � la base


            //Pour les SpecificPositions il faut se connecter � la base
            //mainWindow.SetIsConnectedDB(true);
            var computerColore = "White";
            var testName = "T61aPourBestSpecificPosition3White";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                //echec si nodeResult.Location ==  nodeResult.BestChildPosition
                Assert.AreEqual(nodeResult.BestChildPosition, "e6");

            }

        }

        /*tsiry;12-10-2021*/
        [TestMethod]
        public void T60BlackIsInChessInL3()
        {
            //Les noir ont déja pérdue

            var computerColore = "Black";

            var testName = "T60BlackIsInChessInL3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                //echec si nodeResult.Location ==  nodeResult.BestChildPosition
                Assert.IsTrue(nodeResult.Weight <= -900);
            }




        }
        /*tsiry;05-07-2022*/
        [TestMethod]
        public void T60Suite1BlackIsInChessWhiteToF7()
        {
            //la reine blanche doit se mettre en f7
            var computerColore = "White";

            var testName = "T60Suite1BlackIsInChessWhiteToF7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual("e6", nodeResult.Location);
                Assert.AreEqual("f7", nodeResult.BestChildPosition);
            }




        }




        /*tsiry;12-10-2021*/
        [TestMethod]
        public void T62LePionNoirDoitAttaqueLaReineBlancEnH5()
        {


            var computerColore = "Black";
            var testName = "T62LePionNoirDoitAttaqueLaReineBlancEnH5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.BestChildPosition, "h5");
            }
            // var result = nodeResult.NodeRandomList.FirstOrDefault(x => x.BestChildPosition == "d5");
            // Assert.IsNull(result);


        }


        /*tsiry;14-10-2021*/
        [TestMethod]
        public void T65LeRookBlanchNeDoitPasSeMettreEnA6()
        {


            var computerColore = "White";

            var testName = "T65LeRookBlanchNeDoitPasSeMettreEnA6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a6");

            }


        }


        /*tsiry;18-10-2021*/
        [TestMethod]
        public void T67EchecBlancLeRoiDoitSeMettreEnF3()
        {
            var computerColore = "White";
            var testName = "T67EchecBlancLeRoiDoitSeMettreEnF3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual("f3", nodeResult.BestChildPosition);
            }
        }



        /*tsiry;05-07-2022*/

        [TestMethod]
        public void T67Suite_B_D4toC2_TO_IMPLEMENT()
        {
            var computerColore = "Black";

            var testName = "T67SuiteLaReineNoirDoitSeMettreEnE2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual("d4", nodeResult.Location);
                Assert.IsTrue("e2" == nodeResult.BestChildPosition);
            }
        }

        /*tsiry;19-10-2021*/
        [TestMethod]
        public void T68LeRoiBlanchNeDoitPasSeMettreEnG6()
        {


            var computerColore = "White";





            var testName = "T68LeRoiBlanchNeDoitPasSeMettreEnG6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "g6");
            }
        }


        /*tsiry;21-10-2021*/
        [TestMethod]
        public void T69LeRoiBanchNeDoitPasPrendreLePionEnG2() //TODO L5 BEST VALIDED
        {


            var computerColore = "White";
            var testName = "T69LeRoiBanchNeDoitPasPrendreLePionEnG2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var randomList = nodeResult.EquivalentBestNodeCEList;
                var isSuccess = true;
                if (randomList != null)
                {
                    //  Pour  Tester que  T69
                    foreach (var item in randomList)
                    {
                        if (item.ToIndex == 54)
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                }
                Assert.IsTrue(isSuccess);

                Assert.AreNotEqual(nodeResult.BestChildPosition, "g2");
            }
        }





        /*tsiry;25-10-2021*/
        [TestMethod]
        public void T71LeRoisBlantNeDoitPasPrendreLeRookEnF5()
        {


            var computerColore = "White";


            var testName = "T71LeRoisBlantNeDoitPasPrendreLeRookEnF5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual(nodeResult.BestChildPosition, "f5");
            }
        }

        /*tsiry;25-10-2021*/
        [TestMethod]
        public void T72LaReineNoirDoitPrendreLePionEnD5()
        {


            var computerColore = "Black";

            var testName = "T72LaReineNoirDoitPrendreLePionEnD5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var randomList = nodeResult.EquivalentBestNodeCEList;


                var isSuccess = false;
                if (randomList.Count == 1)
                    isSuccess = true;
                Assert.IsTrue(isSuccess);

                Assert.AreEqual(nodeResult.BestChildPosition, "d5");
            }
        }

        /*tsiry;25-10-2021*/
        [TestMethod]
        public void T72BLaReineNoirDoitPrendreLePionEnD5()
        {


            var computerColore = "White";
            var testName = "T72BLaReineBlanchDoitPrendreLePionEnD4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var randomList = nodeResult.EquivalentBestNodeCEList;
                var isSuccess = false;
                if (randomList.Count == 0 || randomList.Count == 1)
                    isSuccess = true;
                Assert.IsTrue(isSuccess);

                Assert.AreEqual(nodeResult.BestChildPosition, "d4");
            }
        }


        /*tsiry;25-10-2021*/
        [TestMethod]
        public void T73LeFouNoirNeDoitPasPrendreLePionEnB2()//TODO L5 BEST
        {


            var computerColore = "Black";

            var testName = "T73LeFouNoirNeDoitPasPrendreLePionEnB2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);

                Assert.AreNotEqual(nodeResult.BestChildPosition, "b2");
            }
        }

        /*tsiry;28-10-2021*/
        [TestMethod]
        public void T74LesBlanchsDoiventPrendreLePionEnD4()
        {


            var computerColore = "White";





            var pawnListWhite = new List<Pawn>();
            var pawnListBlack = new List<Pawn>();

            var testName = "T74LesBlanchsDoiventPrendreLePionEnD4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //var randomList = nodeResult.EquivalentBestNodeCEList ;
                //Assert.AreEqual(0, randomList.Count);

                Assert.AreEqual(nodeResult.BestChildPosition, "d4");
            }
        }

        /*tsiry;28-10-2021*/
        [TestMethod]
        public void T78ProtectionDuRookDesNoirs()//OK IF L3 (50)
        {
            var computerColore = "White";
            var testName = "T78ProtectionDuRookDesNoirs";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // var randomList = nodeResult.EquivalentBestNodeCEList;
                //  Assert.IsNull(randomList);
                //Assert.AreEqual(nodeResult.Location, "c1");
                //Assert.AreEqual(nodeResult.BestChildPosition, "b2");
                // Assert.IsTrue((nodeResult.Location == "a3" && nodeResult.BestChildPosition == "c4") || (nodeResult.Location == "c2" && nodeResult.BestChildPosition == "b1") || (nodeResult.Location == "a2" && nodeResult.BestChildPosition == "b2"));
                //Assert.IsTrue(nodeResult.Location == "a2" && nodeResult.BestChildPosition == "b2");
                Assert.IsTrue(nodeResult.Location == "c1" && nodeResult.BestChildPosition == "b2");
            }
        }


        /*tsiry;28-10-2021*/
        [TestMethod]
        public void T79LeRoisBlanchDoitSeMettreEnG1()
        {
            var computerColore = "White";
            var testName = "T79LeRoisBlanchDoitSeMettreEnG1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                Assert.AreEqual(1, randomList.Count);

                Assert.AreEqual(nodeResult.BestChildPosition, "g1");
            }
        }


        /*tsiry;12-11-2021*/
        [TestMethod]
        public void T80_B_WinToG4()
        {
            var computerColore = "Black";

            var testName = "T80LeRoiNoirDoitBougerEtLaReineNoirNeDoitPasSeMettreEn";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.AreEqual(nodeResult.Location, "d8");

            }
        }


        /*tsiry;12-11-2021*/
        [TestMethod]
        public void T81LeCavalierBlanchNeDoitPasPrendreLaReinEtLesBlanchDoiventEviterLEchec()
        {


            var computerColore = "White";

            var testName = "T81LeCavalierBlanchNeDoitPasPrendreLaReinEtLesBlanchDoiventEviterLEchec";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult.BestChildPosition, "f6");
            }
        }


        //  [TestMethod]
        //  public void T82Null2LesNoirDoiventEviterLeNull__IGNOR()
        //  {


        //      var computerColore = "Black";





        //      var pawnListWhite = new List<Pawn>();
        //      var pawnListBlack = new List<Pawn>();



        //      //WHITEList
        //      var whiteListString = "" +
        //"King;c5;White;False;False;True;True";

        //      var whiteList = whiteListString.Split('\n');
        //      foreach (var line in whiteList)
        //      {
        //          var datas = line.Split(';');
        //          var newPawn = new Pawn(datas[0], datas[1], datas[2]);
        //          //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
        //          newPawn.IsFirstMove = bool.Parse(datas[3]);
        //          newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
        //          newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
        //          newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
        //          pawnListWhite.Add(newPawn);
        //      }

        //      //BLACKList
        //      var blackListString = "" +
        //"King;f7;Black;False;False;True;True" +
        //"\nQueen;f1;Black;False;False;False;False";
        //      var blackList = blackListString.Split('\n');
        //      foreach (var line in blackList)
        //      {
        //          var datas = line.Split(';');
        //          var newPawn = new Pawn(datas[0], datas[1], datas[2]);
        //          //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
        //          newPawn.IsFirstMove = bool.Parse(datas[3]);
        //          newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
        //          newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
        //          newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
        //          pawnListBlack.Add(newPawn);
        //      }



        //      var pawnList = new List<Pawn>(); pawnList.AddRange(pawnListWhite); pawnList.AddRange(pawnListBlack);
        //      //ajout des movements
        //      if (Utils.MovingList == null)
        //          Utils.MovingList = new List<string>();
        //      Utils.MovingList.Add("5(K|B)>13(__)");
        //      Utils.MovingList.Add("33(K|W)>34(__)");

        //      Utils.MovingList.Add("53(Q|B)>61(__)");
        //      Utils.MovingList.Add("34(K|W)>26(__)");
        //      Utils.MovingList.Add("61(Q|B)>53(__)");
        //      Utils.MovingList.Add("26(K|W)>34(__)");
        //      Utils.MovingList.Add("53(Q|B)>61(__)");
        //      Utils.MovingList.Add("34(K|W)>26(__)");
        //      using (var chessEngine = new chessEngineOptimize())
        //      {
        //          var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

        //          var randomList = nodeResult.EquivalentBestNodeCEList;
        //          //var t_movingList = Utils.MovingList;
        //          var notValide = randomList.FirstOrDefault(x => x.FromIndex == 61 && x.ToIndex == 53);
        //          Assert.IsNull(notValide);
        //      }

        //      // Assert.AreNotEqual(nodeResult.BestChildPosition, "f6");
        //  }

        /*tsiry;12-11-2021*/
        [TestMethod]
        public void T84EchecEtMatNoir()
        {


            var computerColore = "Black";





            var testName = "T84EchecEtMatNoir";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue(nodeResult.Weight <= 1000);
            }

        }


        [TestMethod]
        public void T84BEchecEtMatNoir()
        {
            var computerColore = "Black";
            var testName = "T84EchecEtMatNoir";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //  var randomList = nodeResult.EquivalentBestNodeCEList;
                Assert.IsTrue(nodeResult.Weight <= -200);
                //echec si nodeResult.Location ==  nodeResult.BestChildPosition
                // Assert.AreEqual(nodeResult.Location, nodeResult.BestChildPosition);
            }
        }


        /*tsiry;16-12-2021*/
        [TestMethod]
        public void T85LaToureNoirDoitPrendreLePionEnA5()//OK IF L3 (10)
        {
            var computerColore = "Black";
            var testName = "T85LaToureNoirDoitPrendreLePionEnA5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                //echec si nodeResult.Location ==  nodeResult.BestChildPosition
                Assert.IsTrue(nodeResult.BestChildPosition == "a5" || nodeResult.BestChildPosition == "c5");
            }
        }


        /*tsiry;11-01-2022
         * */
        [TestMethod]
        public void T86ApplicationDownOutOfmemories()
        {



            var computerColore = "White";

            var testName = "T86ApplicationDownOutOfmemories";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                //echec si nodeResult.Location ==  nodeResult.BestChildPosition
                Assert.AreEqual("a8", nodeResult.BestChildPosition);
            }
        }
        /*tsiry;11-01-2022
        * */
        [TestMethod]
        public void T87BlackWinL2()
        {



            var computerColore = "Black";
            var testName = "T87BlackInChessInL2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "f2");
            } //var randomList = nodeResult.EquivalentBestNodeCEList;
              //Assert.IsNull(randomList);
              //echec si nodeResult.Location ==  nodeResult.BestChildPosition

            /*    -2  d7 => e6
          - 2  d8 => a5
           - 2  f8 => b4
            - 2  e8 => e7
             - 2  g8 => f6
              - 2  d7 => e6
               - 2  d8 => f6
                - 2  f8 => c5*/

        }
        [TestMethod]
        public void T87WhiteAvoidInChessInL2__TOIMPLEMENT_OK()
        {



            var computerColore = "White";

            var testName = "T87BlackInChessInL2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.IsTrue(nodeResult.BestChildPosition == "e3" || nodeResult.BestChildPosition == "g3");//e3 ou g3 
            }


        }

        [TestMethod]
        public void T88LesBlanchDoiventEviterLEchec()
        {
            var computerColore = "White";
            var testName = "T88LesBlanchDoiventEviterLEchec";
            var testPath = Path.Combine(testsDirrectory, testName);

            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.BestChildPosition == "h3" || nodeResult.BestChildPosition == "e3" || nodeResult.BestChildPosition == "g3" || nodeResult.BestChildPosition == "d3");//e3 ou g3 
            }


        }
        [TestMethod]
        public void T88LesNoirDoiventSeMettreEnF2()
        {



            var computerColore = "Black";
            var testName = "T88LesBlanchDoiventEviterLEchec";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //var randomList = nodeResult.EquivalentBestNodeCEList;
                //Assert.IsNull(randomList);
                //echec si nodeResult.Location ==  nodeResult.BestChildPosition


                Assert.AreEqual(nodeResult.BestChildPosition, "f2");
            }


        }


        /*tsiry;16-03-2022*/
        [TestMethod]
        public void T89LeFouNoirNeDoitPasSeMettreEnB4()
        {



            var computerColore = "Black";
            var testName = "T89LeFouNoirNeDoitPasSeMettreEnB4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult.BestChildPosition, "b4");
            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }

        /*tsiry;16-03-2022*/
        /*  [TestMethod]
          public void T90LePoinNoirDoitSeMettreEnH5()
          {



              var computerColore = "Black";
              var testName = "T90LePoinNoirDoitSeMettreEnH5";
              var testPath = Path.Combine(testsDirrectory, testName);
              var pawnList =  Chess2Utils.LoadFromDirectorie(testPath);
              using (var chessEngine = new chessEngine())
              {
                  var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), true,null);

                  Assert.AreEqual(nodeResult.Location, "h7");
                  Assert.AreEqual(nodeResult.BestChildPosition, "h5");
              }
              //var randomList = nodeResult.EquivalentBestNodeCEList;
              //Assert.IsNull(randomList);
              //echec si nodeResult.Location ==  nodeResult.BestChildPosition





          }
          */
        /*tsiry;17-03-2022*/
        [TestMethod]
        public void T91LaReineNoirDoitSeMettreEnH5()
        {



            var computerColore = "Black";
            var testName = "T91LaReineNoirDoitSeMettreEnH5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.Location, "g4");
                Assert.AreEqual(nodeResult.BestChildPosition, "h5");
            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }
        [TestMethod]
        public void T33bLePionBlancDoitPrendreLeCavalierOuLeFoueEnD5()////OK IF L3(50)
        {
            /*Le pion blanch prendre le cavalier en d4*/
            var computerColore = "White";
            var testName = "T33bLePionBlancDoitPrendreLeCavalierOuLeFoueEnD5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Le poin doit pas se mettre en d4
                //if()
                Assert.IsTrue(nodeResult.EquivalentBestNodeCEList.Count == 1 || nodeResult.EquivalentBestNodeCEList.Count == 2);
                Assert.IsTrue(nodeResult.BestChildPosition == "d4" || nodeResult.BestChildPosition == "d5");
                // var randomList = nodeResult.EquivalentBestNodeCEList;
                // Assert.AreEqual(randomList.Count, 0);
            }



        }



        /*tsiry;16-05-2022*/
        [TestMethod]
        public void T92ABlackG8ToH8()
        {
            var computerColore = "Black";
            var testName = "T92ABlackG8ToH8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");
                //la toure noir doit éviter d'être pris
                Assert.AreEqual(nodeResult.BestChildPosition, "h8");
            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition
        }
        /*tsiry;17-05-2022*/
        [TestMethod]
        public void T92BLeCavalierNoirDoitPartieDeF6()//TODO L5 BEST
        {



            var computerColore = "Black";
            var testName = "T92BLeCavalierNoirDoitPartieDeF6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");
                Assert.AreEqual(nodeResult.Location, "f6");
            }

        }



        /*tsiry;19-05-2022*/
        [TestMethod]
        public void T93ALaReineNoirDoitSeMettreEnG3_echecDesBlancEnL1()
        {

            var computerColore = "Black";
            var testName = "T93ALaReineNoirDoitSeMettreEnG3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.Location, "g4");
                Assert.AreEqual(nodeResult.BestChildPosition, "g3");
            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }

        /*tsiry;19-05-2022*/
        [TestMethod]
        public void T94LeCavalierBlanchDoitSeMettreEnD2_ou_leFouDoiSePettreEnE6()
        {

            var computerColore = "White";
            var testName = "T94LeCavalierNoirDoitPasPartirDeF3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreNotEqual(nodeResult.BestChildPosition, "f3");
            }

            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }

        /*tsiry;23-05-2022*/
        [TestMethod]
        public void T95LaReineNoirDoitSeMettreEnH1PourGagnier()//>L5 BEST<
        {

            var computerColore = "Black";
            var testName = "T95LaReineNoirDoitSeMettreEnH1PourGagnier";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual("h1", nodeResult.BestChildPosition);//h1

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }
        /*tsiry;05-07-2022*/
        [TestMethod]
        public void T95SuiteWhiteE4ToG3()
        {

            var computerColore = "White";
            var testName = "T95SuiteWhiteE4ToG3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.BestChildPosition == "g3" || nodeResult.BestChildPosition == "f2");//h1

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }

        //[TestMethod]
        //public void T95SuiteSuite()
        //{

        //    var computerColore = "Black";
        //    var testName = "T95SuiteSuite";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new chessEngine())
        //    {
        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
        //        Assert.AreEqual("g2", nodeResult.Location);
        //        Assert.AreEqual("f2", nodeResult.BestChildPosition);

        //    }
        //}

        [TestMethod]
        public void T95SuiteSuiteBlackWin()//TODO L5 BEST
        {

            var computerColore = "Black";
            var testName = "T95SuiteSuiteBlackWin";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.BestChildPosition == "h1" || nodeResult.BestChildPosition == "f2");

            }

        }

        /*tsiry;23-05-2022*/
        [TestMethod]
        public void T95BLeCavalierBlanchDoitAttaquerEnF2()//OK IF L3 (100)
        {

            var computerColore = "White";
            var testName = "T95BLeCavalierBlanchDoitAttaquerEnF2";//WHITE MUST LOSE
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList), 3);
                // Assert.AreEqual(nodeResult.BestChildPosition, "f2");
                //var isSuccess = nodeResult.BestChildPosition == "d4" || nodeResult.BestChildPosition == "d3" || nodeResult.BestChildPosition == "c3" || nodeResult.BestChildPosition == "f2";
                Assert.IsTrue(nodeResult.BestChildPosition == "f2");

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }
        /*tsiry;24-06-2022*/
        [TestMethod]
        public void T96LeCavalierBlanchDoitSePettreEnC7()
        {

            var computerColore = "White";
            var testName = "T96LeCavalierBlanchDoitSePettreEnC7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "c7");

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }


        /*tsiry;11-07-2022*/
        [TestMethod]
        public void T97AWhitePourAviterT97WhiteNotF1ToG2()
        {

            var computerColore = "White";
            var testName = "T97AWhitePourAviterT97WhiteNotF1ToG2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var isValide = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.BestChildPosition == "g2" || x.BestChildPosition == "h1");
                Assert.IsNotNull(isValide);

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }

        /*tsiry;11-07-2022*/
        [TestMethod]
        public void T97BBlackWhinG4ToE2()
        {

            var computerColore = "Black";
            var testName = "T97BBlackWhinG4ToE2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.Location, "g4");
                Assert.AreEqual(nodeResult.BestChildPosition, "e2");

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }

        /*tsiry;14-07-2022*/
        [TestMethod]
        public void T98_B_E8ToF7()
        {

            var computerColore = "Black";
            var testName = "T98BlackE8ToF7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //Assert.AreEqual(nodeResult.BestNodeEquivalentList.Count, 1);
                Assert.AreEqual(nodeResult.Location, "e8");
                Assert.AreEqual(nodeResult.BestChildPosition, "f7");

            }

        }

        //[TestMethod]
        //public void T98b_W_F7ToE8()
        //{

        //    var computerColore = "White";
        //    var testName = "T98BlackE8ToF7";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new chessEngineOptimize())
        //    {
        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

        //        //Assert.AreEqual(nodeResult.BestNodeEquivalentList.Count, 1);
        //        Assert.AreEqual(nodeResult.Location, "f7");
        //        Assert.AreEqual(nodeResult.BestChildPosition, "e8");

        //    }

        //}
        /*tsiry;14-07-2022*/
        [TestMethod]
        public void T99WhiteNotC7ToF7__IGNOR()
        {

            var computerColore = "White";
            var testName = "T99WhiteNotC7ToF7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreNotEqual(nodeResult.Location, "c7");
                Assert.IsFalse(nodeResult.BestChildPosition == "f7" || nodeResult.BestChildPosition == "b8");

            }
            //var randomList = nodeResult.EquivalentBestNodeCEList;
            //Assert.IsNull(randomList);
            //echec si nodeResult.Location ==  nodeResult.BestChildPosition





        }


        /*tsiry;19-07-2022*/
        [TestMethod]
        public void T100BlackG6ToF5()
        {

            var computerColore = "Black";
            var testName = "T100BlackG6ToF5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = Utils.RunEngine(_chessEngine, computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreNotEqual(nodeResult.Location, "c7");
                Assert.AreEqual("g6", nodeResult.Location);

            }
        }

        /*tsiry;19-07-2022*/
        [TestMethod]
        public void T105BlackC6ToD4()//>> ONLY BEST L5
        {

            var computerColore = "Black";
            var testName = "T105BlackC6ToD4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreNotEqual(nodeResult.Location, "c7");
                //Assert.IsTrue(nodeResult.li)

                var randomList = nodeResult.EquivalentBestNodeCEList;
                Assert.AreEqual(1, randomList.Count);
                Assert.AreEqual("d4", nodeResult.BestChildPosition);

            }
        }



        [TestMethod]
        public void T106BlackCF6ToD5()
        {

            var computerColore = "Black";
            var testName = "T106LaCavalierNoirDoitPrendreLaReineEnD5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {

                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual("d5", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T107BlackLocationD5()
        {

            var computerColore = "Black";
            var testName = "T107LaReinneNoirDoitPartirDeD5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {

                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual("d5", nodeResult.Location);
            }
        }

        //[TestMethod]
        //public void T108WhiteF2ToF4__IGNOR()
        //{

        //    var computerColore = "White";
        //    var testName = "T108WhiteF2ToF4";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new chessEngineOptimize())
        //    {

        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
        //        Assert.AreEqual("f4", nodeResult.BestChildPosition);
        //    }
        //}

        [TestMethod]
        public void T109BlackNotB6()
        {

            var computerColore = "White";
            var testName = "T109BlackNotB6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {

                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreNotEqual("b6", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T110WhiteKingNoToE1()
        {

            var computerColore = "White";
            var testName = "T110WhiteKingNoToE1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {

                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.AreNotEqual("e1", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T111BlackNotToF5()
        {

            var computerColore = "Black";
            var testName = "T111BlackNotToF5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreEqual((nodeResult.EquivalentBestNodeCEList.Count, 1););
                Assert.AreNotEqual("f5", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T112WhiteLePoinA3NeDoitPasPrendreLePionB4()
        {

            var computerColore = "White";
            var testName = "T112WhiteLePoinA3NeDoitPasPrendreLePionB4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreEqual((nodeResult.EquivalentBestNodeCEList.Count, 1););
                Assert.AreNotEqual("b4", nodeResult.BestChildPosition);
            }
        }
        [TestMethod]
        public void T113_W_D1toD1()
        {

            var computerColore = "White";
            var testName = "T113WhiteD1toMoveD1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.AreEqual(nodeResult.BestNodeEquivalentList.Count, 1);
                Assert.AreEqual("d1", nodeResult.Location);
            }
        }

        [TestMethod]
        public void T114_W_notB3toF7()
        {

            var computerColore = "White";
            var testName = "T114_W_NotB3toF7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.AreNotEqual("F7", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T115_W_F3toG5()//TO DEFINE L3(20) OK
        {

            var computerColore = "White";
            var testName = "T115W_F3toG5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.AreEqual("g5", nodeResult.BestChildPosition);
            }
        }
        [TestMethod]
        public void T116_W_notC3toD5()
        {

            var computerColore = "White";
            var testName = "T116W_notC3toD5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.IsNull(nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.BestChildPosition == "d5"));
                Assert.AreNotEqual("d5", nodeResult.BestChildPosition);
            }
        }
        [TestMethod]
        public void T117_W_toC3()
        {

            var computerColore = "White";
            var testName = "T117_W_toC3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                // Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count,2);
                //Assert.IsTrue(nodeResult.EquivalentBestNodeCEList.Any(x => x.BestChildPosition == "c3"));
                Assert.IsTrue("c3" == nodeResult.BestChildPosition || "a4" == nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T118_W_toB5()
        {

            var computerColore = "White";
            var testName = "T118_W_toB5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                //Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.IsTrue("b5" == nodeResult.BestChildPosition || "a4" == nodeResult.BestChildPosition);
            }
        }


        [TestMethod]
        public void T119_W_notToC4()
        {

            var computerColore = "White";
            var testName = "T119_W_notToC4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                //Assert.AreNotEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.IsNull(nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.BestChildPosition == "c4"));
                Assert.AreNotEqual("c4", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T120_B_notToE3()
        {

            var computerColore = "Black";
            var testName = "T120_B_notToE3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));


                Assert.IsNull(nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.Location == "e3"));
                Assert.AreNotEqual("e3", nodeResult.BestChildPosition);
            }
        }

        [TestMethod]
        public void T121_()//TO DEFINE
        {

            var computerColore = "White";
            var testName = "T121_";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //                5:   H2(55) => G3(46) : -1
                //5:   D1(59) => E2(52) : -1
                //Assert.AreNotEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.IsTrue(nodeResult.BestChildPosition == "h3" || nodeResult.BestChildPosition == "g3");
            }
        }

        [TestMethod]
        public void T122_W_NotD2toE3()//not d2 to e3 car echec et mat au tour suivant, la reinne noir vas en d1
        {

            var computerColore = "White";
            var testName = "T122_W_NotD2toE3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                //                5:   H2(55) => G3(46) : -1
                //5:   D1(59) => E2(52) : -1
                //  Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count, 1);
                Assert.IsFalse(nodeResult.Location == "d2" && nodeResult.BestChildPosition == "e3");

            }
        }


        [TestMethod]
        public void T123_B_G8toG7()//seul solution G8 to G7 car si on la rainne noir se meit en e7 et c'est Échec et mat
        {

            var computerColore = "Black";
            var testName = "T123_B_";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue((nodeResult.Location == "g8" && nodeResult.BestChildPosition == "g7"));

            }
        }

        [TestMethod]
        public void T124_B_H8toF8()
        {

            var computerColore = "Black";
            var testName = "T124_B_H8toF8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue((nodeResult.Location == "h8" && nodeResult.BestChildPosition == "f8")
                    || nodeResult.Location == "b4" && nodeResult.BestChildPosition == "f8");

            }
        }

        [TestMethod]
        public void T125_B_toD2()
        {

            var computerColore = "Black";
            var testName = "T125_B_toD2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.BestChildPosition, "d2");

            }
        }

        [TestMethod]
        public void T126_B_toF1()
        {

            var computerColore = "Black";
            var testName = "T126_B_toF1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.BestChildPosition, "f1");

            }
        }
        //[TestMethod]
        //public void T127_B_toF3()
        //{

        //    var computerColore = "Black";
        //    var testName = "T127_B_toF3";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new ChessEngine22())
        //    {
        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

        //        Assert.AreEqual(nodeResult.BestChildPosition, "f3");

        //    }
        //}

        [TestMethod]
        public void T128_W_NotToC1()
        {

            var computerColore = "White";
            var testName = "T128_W_NotToC1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                var invalideNode = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.BestChildPosition == "c1");
                Assert.IsNull(invalideNode);
                Assert.AreNotEqual(nodeResult.BestChildPosition, "c1");

            }
        }

        [TestMethod]
        public void T128B_B_A6toA1()
        {

            var computerColore = "Black";
            var testName = "T128B_B_A6toA1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.AreEqual(nodeResult.BestChildPosition, "a1");

            }
        }

        [TestMethod]
        public void T129_W_B2toB3()
        {

            var computerColore = "White";
            var testName = "T129_W_B2toB3";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                var notValideNode = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => (x.Location == "b2" && x.BestChildPosition == "b3") || (x.Location == "b2" && x.BestChildPosition == "a3"));
                Assert.IsNotNull(notValideNode);

            }
        }

        [TestMethod]
        public void T130_W()
        {

            var computerColore = "White";
            var testName = "T130_W";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.BestChildPosition == "f2" || nodeResult.BestChildPosition == "e3");

            }
        }


        [TestMethod]
        public void T131_B_E8toD8()
        {

            var computerColore = "Black";
            var testName = "T131_B_E8toD8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.EquivalentBestNodeCEList.Count(), 1);
                Assert.AreEqual(nodeResult.BestChildPosition, "d8");

            }
        }

        [TestMethod]
        public void T131B_W_B8toC8()
        {

            var computerColore = "White";
            var testName = "T131B_W_B8toC8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "c8");

            }
        }


        [TestMethod]
        public void T132_B_toC6()
        {

            var computerColore = "Black";
            var testName = "T132_B_toC6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.AreEqual(nodeResult.BestChildPosition, "c6");

            }
        }

        [TestMethod]
        public void T133_B_NotC5toE5AndNotC5toE7()
        {

            var computerColore = "Black";
            var testName = "T133_B_NotC5toE5AndNotC5toE7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Assert.IsFalse(nodeResult.Location == "c5" && nodeResult.BestChildPosition == "e5");
                Assert.IsTrue(nodeResult.Location == "c5" && nodeResult.BestChildPosition == "e7");
            }
        }

        [TestMethod]
        public void T134_B_ToD2()////OK IF L3(20) BUT TO VERIFIE
        {

            var computerColore = "Black";
            var testName = "T134_B_ToD2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue((nodeResult.Location == "c5" && nodeResult.BestChildPosition == "c6")
                    || (nodeResult.Location == "g5" && nodeResult.BestChildPosition == "d2"));

            }
        }

        /*
                [TestMethod]
                public void T135_W_D5toB6()
                {

                    var computerColore = "White";
                    var testName = "T135_W_D5toB6";
                    var testPath = Path.Combine(testsDirrectory, testName);
                    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
                    using (var chessEngine = new ChessEngine22())
                    {
                        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                        Assert.IsTrue(nodeResult.Location == "d5" && nodeResult.BestChildPosition == "b6");

                    }
                }*/

        [TestMethod]
        public void T136_W_B4toA5()//prend la tour et menace dirrecte le roi noir 
        {

            var computerColore = "White";
            var testName = "T136_W_B4toA5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue((nodeResult.Location == "d5" && nodeResult.BestChildPosition == "f6") ||
                    (nodeResult.Location == "b4" && nodeResult.BestChildPosition == "a5"));

            }
        }

        [TestMethod]
        public void T136B_W_D5toF6()//TO VERIFY
        {

            var computerColore = "White";
            var testName = "T136B_W_D5toF6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.Location == "d5" && nodeResult.BestChildPosition == "f6");

            }
        }

        //[TestMethod]
        //public void T136C_W_D5toF6() //TO VERIFY
        //{

        //    var computerColore = "White";
        //    var testName = "T136C_W_D5toF6";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new ChessEngine22())
        //    {
        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
        //        Assert.IsTrue(nodeResult.Location == "d5" && nodeResult.BestChildPosition == "f6");

        //    }
        //}


        [TestMethod]
        public void T137_W_A3toD6()
        {

            var computerColore = "White";
            var testName = "T137_W_A3toD6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var invalide = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.Location == "a3" && x.BestChildPosition == "d6");
                Assert.IsNotNull(invalide);

            }
        }


        //[TestMethod]
        //public void T138_W_notD1toH5()
        //{

        //    var computerColore = "White";
        //    var testName = "T138_W_notD1toH5";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new ChessEngine22())
        //    {
        //        var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

        //        var invalide = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.Location == "d1" && x.BestChildPosition == "h5");
        //        Assert.IsNull(invalide);

        //    }
        //}

        //[TestMethod]
        //public void T139_W_notD1toD7()
        //{

        //    var computerColore = "White";
        //    var testName = "T139_W_notD1toD7";
        //    var testPath = Path.Combine(testsDirrectory, testName);
        //    var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
        //    using (var chessEngine = new ChessEngine22())
        //    {
        //        var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList),6);

        //        var invalide = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.Location == "d1" && x.BestChildPosition == "d7");
        //        Assert.IsNull(invalide);

        //    }
        //}

        [TestMethod]
        public void T140_W_notE3toC5()
        {

            var computerColore = "White";
            var testName = "T140_W_notE3toC5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var invalide = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.Location == "e3" && x.BestChildPosition == "c5");
                Assert.IsNull(invalide);

            }
        }
        [TestMethod]
        public void T141_W_notD3toD8()
        {

            var computerColore = "White";
            var testName = "T141_W_notD3toD8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                var invalide = nodeResult.EquivalentBestNodeCEList.FirstOrDefault(x => x.Location == "d3" && x.BestChildPosition == "d8");
                Assert.IsNull(invalide);

            }
        }

        [TestMethod]
        public void T142_W_C7toC8_Win()
        {

            var computerColore = "White";
            var testName = "T142_W_C7toC8_Win";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue(nodeResult.Weight >= 9999); 
                Assert.IsTrue(nodeResult.Location == "c7" && nodeResult.BestChildPosition == "c8");

            }
        }


        [TestMethod]
        public void T143_B_B8to_ElseLose()
        {

            var computerColore = "Black";
            var testName = "T143_B_B8to_ElseLose";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue((nodeResult.Location == "b8")
                    || (nodeResult.Location == "f4" && nodeResult.BestChildPosition == "e2")
                    );

            }
        }
        [TestMethod]
        public void T144A_B_E7toD6()
        {

            var computerColore = "Black";
            var testName = "T144A_B_E7toD6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsTrue(nodeResult.EquivalentBestNodeCEList.Count == 1);
                Assert.AreEqual(nodeResult.BestChildPosition, "d6");
                  

            }
        }

        [TestMethod]
        public void T145_B_G8to()
        {

            var computerColore = "Black";
            var testName = "T145_B_G8to";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.IsTrue(nodeResult.EquivalentBestNodeCEList.Count == 1);
                Assert.AreEqual(nodeResult.Location, "g8");
                Assert.AreNotEqual(nodeResult.BestChildPosition, "g5");


            }
        }

        [TestMethod]
        public void T145Suite_B_G5toF3()
        {

            var computerColore = "Black";
            var testName = "T145_SUITE";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                
                Assert.IsTrue(nodeResult.Location == "g5" && nodeResult.BestChildPosition == "f3");


            }
        }

        [TestMethod]
        public void T145Suite_W_H4toG5()
        {

            var computerColore = "White";
            var testName = "T145_SUITE";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));

                Assert.IsTrue(nodeResult.Location == "h4" && nodeResult.BestChildPosition == "g5");


            }
        }


        [TestMethod]
        public void T146a_B_NotD7toB6()
        {
            // NOT D7 to B6 because:
            // the pawn moves to D7:
            // the king and queen will be threatened.
            var computerColore = "Black";
            var testName = "T146a_B_NotD7toB6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsFalse(nodeResult.Location == "d7" && nodeResult.BestChildPosition == "b6");
            }
        }

        [TestMethod]
        public void T146b_B_NotC6toD4()
        {
            // NOT C6 to D4 because:
            // the queen moves to F7:
            // Chess mat.
            var computerColore = "Black";
            var testName = "T146b_B_NotC6toD4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsFalse(nodeResult.Location == "c6" && nodeResult.BestChildPosition == "b4");
            }
        }
        [TestMethod]
        public void T147_B_NotC8toC5()
        {
            var computerColore = "Black";
            var testName = "T147_B_NotC8toC5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                Assert.IsFalse(nodeResult.Location == "c8" && nodeResult.BestChildPosition == "c5");
            }
        }

        /*tsiry;19-07-2022*/
        /*    [TestMethod]
            public void T104BlackF7ToG7()
            {

              var computerColore = "Black";
              var testName = "T104BlackF7ToG7";
              var testPath = Path.Combine(testsDirrectory, testName);
              var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
              using (var chessEngine = new chessEngine())
              {
                var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                // Assert.AreNotEqual(nodeResult.Location, "c7");
                Assert.AreEqual("g7",nodeResult.BestChildPosition);

              }





            }

        */
        /*tsiry;19-07-2022*/
        /*   [TestMethod]
           public void T102WhiteD1ToC1()
           {

             var computerColore = "White";
             var testName = "T102WhiteD1ToC1";
             var testPath = Path.Combine(testsDirrectory, testName);
             var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
             using (var chessEngine = new chessEngine())
             {
               var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
               // Assert.AreNotEqual(nodeResult.Location, "c7");
               Assert.AreEqual("c1",nodeResult.BestChildPosition);

             }
             //var randomList = nodeResult.EquivalentBestNodeCEList;
             //Assert.IsNull(randomList);
             //echec si nodeResult.Location ==  nodeResult.BestChildPosition





           }
       */

        /*tsiry;19-07-2022*/
        /*   [TestMethod]
           public void T101BlackG6ToF5()
           {

             var computerColore = "Black";
             var testName = "T101BlackG6ToF5";
             var testPath = Path.Combine(testsDirrectory, testName);
             var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
             using (var chessEngine = new chessEngine())
             {
               var nodeResult = _chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
               // Assert.AreNotEqual(nodeResult.Location, "c7");
               Assert.AreEqual("f5",nodeResult.BestChildPosition);

             }
             //var randomList = nodeResult.EquivalentBestNodeCEList;
             //Assert.IsNull(randomList);
             //echec si nodeResult.Location ==  nodeResult.BestChildPosition





           }
       */
    }


}
