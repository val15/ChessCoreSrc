using ChessCore.Tools;
using ChessCore.Tools.ChessEngine;

namespace ChessCore.Test
{
    [TestClass]
    public class PawnLocationTest
    {
        private string testsDirrectory = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.Combine(Environment.CurrentDirectory)).ToString()).ToString()).ToString(), "TESTS");


        [TestMethod]
        public void T121_W_GetMalus()
        {

            
            var testName = "T121_";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.GetMenacedsPoints("W");
            Assert.AreEqual(result,1);
        }

        [TestMethod]
        public void T122_W_inChess()
        {


            var testName = "T122_W_inChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("W");
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void T123_B_inChess()
        {


            var testName = "T123_B_inChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("B");
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void T124_W_isInChess()
        {


            var testName = "T124_W_isInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("W");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void T127_B_isInNotChess()
        {


            var testName = "T127_B_isInNotChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("B");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void T128_W_isInChesss()
        {
            var testName = "T128_W_isInChesss";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("W");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void T131_B_isInChess()
        {
            var testName = "T131_B_isInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("B");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void T142_W_Win()
        {
            var testName = "T142_W_Win";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var result = boad.IsKingInCheck("B");
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void T93_W_InChess()
        {


            var testName = "T93ALaReineNoirDoitSeMettreEnG3Position";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.IsKingInCheck("W");
            Assert.IsTrue(isInChess);


        }

        [TestMethod]
        public void T95SuiteSuiteB_W_InChess()
        {


            var testName = "T95SuiteSuiteB_W_InChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.IsKingInCheck("W");
            Assert.IsTrue(isInChess);


        }



        [TestMethod]
        public void T67WhiteIsInChess()
        {


            var testName = "T67WhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.IsKingInCheck("W");
            Assert.IsTrue(isInChess);


        }

        [TestMethod]
        public void T18WhiteNoInChess()
        {


            var testName = "T18WhiteNoInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.IsKingInCheck("W");
            Assert.IsFalse(isInChess);


        }

        [TestMethod]
        public void T120_W_NotInChess()
        {


            var testName = "T120_W_NotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.IsKingInCheck("W");
            Assert.IsFalse(isInChess);


        }

        [TestMethod]
        public void T31_B_GetPossibleModeA0()
        {
            /*La reinne noir ne doit pas se mettre en a8*/






            var testName = "T31LaReineNoirDoitPrendreLaReineBlanch";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var possiblesMove = boad.GetPossibleMovesOLD(3);
            Assert.IsNotNull(possiblesMove.FirstOrDefault(x => x.ToIndex == 0));

        }

        [TestMethod]
        public void T124_B_GetPossibleMoveH8()
        {
            /*La reinne noir ne doit pas se mettre en a8*/
            var testName = "T124_B_H8toF8";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);
            var possiblesMove = boad.GetPossibleMovesOLD(7);
            Assert.IsNotNull(possiblesMove.FirstOrDefault(x => x.ToIndex == 5));

        }


        [TestMethod]
        public void T117_B_NotInChess()
        {


            var testName = "T117_W_toC3Position";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.IsKingInCheck("B");
            Assert.IsFalse(isInChess);


        }

        [TestMethod]
        public void T145_B_G5IsProteced()
        {


            var testName = "T145_B_G8to";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var isInChess = boardCE.TargetIndexIsProtected(30, "B");
            Assert.IsTrue(isInChess);


        }

        [TestMethod]
        public void T93_W_InChessGetToOpponentKingPath46to6O()
        {


            var testName = "T93ALaReineNoirDoitSeMettreEnG3Position";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var toOpponentKingPathList = boardCE.GetToOpponentKingPath(46, 60);
            Assert.AreEqual(toOpponentKingPathList.Count, 1);
            Assert.IsTrue(toOpponentKingPathList.Contains(53));


        }

        [TestMethod]
        public void T117_B_NotInChessGetToOpponentKingPath32to4()
        {


            var testName = "T117_B_NotInChessGetToOpponentKingPath32to4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var toOpponentKingPathList = boardCE.GetToOpponentKingPath(32, 4);
            Assert.AreEqual(toOpponentKingPathList.Count, 3);
            Assert.IsTrue(toOpponentKingPathList.Contains(25));
            Assert.IsTrue(toOpponentKingPathList.Contains(18));
            Assert.IsTrue(toOpponentKingPathList.Contains(11));



        }
        [TestMethod]
        public void T117_B_NotInChessGetToOpponentKingPath36to4()
        {


            var testName = "T117_B_NotInChessGetToOpponentKingPath36to4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardCE boardCE = Chess2Utils.GenerateBoardFormPawnListCE(pawnList);

            var toOpponentKingPathList = boardCE.GetToOpponentKingPath(36, 4);
            Assert.AreEqual(toOpponentKingPathList.Count, 3);
            Assert.IsTrue(toOpponentKingPathList.Contains(28));
            Assert.IsTrue(toOpponentKingPathList.Contains(20));
            Assert.IsTrue(toOpponentKingPathList.Contains(12));



        }

    }
}
