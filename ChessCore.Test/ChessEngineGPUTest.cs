using ChessCore.Tools.ChessEngine.Engine;
using ChessCore.Tools;

namespace ChessCore.Test
{

    [TestClass]
    public class ChessEngineGPUTest
    {
        private string testsDirrectory = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.Combine(Environment.CurrentDirectory)).ToString()).ToString()).ToString(), "TESTS");

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

            using (var chessEngine = new ChessEngineCPU())
            {
                var nodeResult = chessEngine.GetBestModeCE(computerColore, Chess2Utils.GenerateBoardFormPawnListCE(pawnList));
                //Positions final du cavalier noir ne doit pas etre  ni "a2" ni "c2" 
                Assert.AreNotEqual(nodeResult.BestChildPosition, "a2", "c2");
            }
        }




    }
}
