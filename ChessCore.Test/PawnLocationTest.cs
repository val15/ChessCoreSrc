using ChessCore.Tools;

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

            var boad = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);
            var result = boad.GetMenacedsPoints("W");
            Assert.AreEqual(result,1);
        }

        [TestMethod]
        public void T122_W_inChess()
        {


            var testName = "T122_W_inChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);
            var result = boad.IsKingInCheck("W");
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void T123_B_inChess()
        {


            var testName = "T123_B_inChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);
            var result = boad.IsKingInCheck("B");
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void T124_W_isInChess()
        {


            var testName = "T124_W_isInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            var boad = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);
            var result = boad.IsKingInCheck("W");
            Assert.IsTrue(result);
        }



        [TestMethod]
        public void T93_W_InChess()
        {


            var testName = "T93ALaReineNoirDoitSeMettreEnG3Position";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var isInChess = boardGPT.IsKingInCheck("W");
            Assert.IsTrue(isInChess);


        }


        [TestMethod]
        public void T67WhiteIsInChess()
        {


            var testName = "T67WhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var isInChess = boardGPT.IsKingInCheck("W");
            Assert.IsTrue(isInChess);


        }

        [TestMethod]
        public void T18WhiteNoInChess()
        {


            var testName = "T18WhiteNoInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var isInChess = boardGPT.IsKingInCheck("W");
            Assert.IsFalse(isInChess);


        }

        [TestMethod]
        public void T120_W_NotInChess()
        {


            var testName = "T120_W_NotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var isInChess = boardGPT.IsKingInCheck("W");
            Assert.IsFalse(isInChess);


        }

        [TestMethod]
        public void T31_B_GetPossibleModeA0()
        {
            /*La reinne noir ne doit pas se mettre en a8*/






            var testName = "T31LaReineNoirDoitPrendreLaReineBlanch";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);
            var possiblesMove = boad.GetPossibleMovesOLD(3);
            Assert.IsNotNull(possiblesMove.FirstOrDefault(x => x.ToIndex == 0));

        }


        [TestMethod]
        public void T117_B_NotInChess()
        {


            var testName = "T117_W_toC3Position";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var isInChess = boardGPT.IsKingInCheck("B");
            Assert.IsFalse(isInChess);


        }

        [TestMethod]
        public void T93_W_InChessGetToOpponentKingPath46to6O()
        {


            var testName = "T93ALaReineNoirDoitSeMettreEnG3Position";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var toOpponentKingPathList = boardGPT.GetToOpponentKingPath(46, 60);
            Assert.AreEqual(toOpponentKingPathList.Count, 1);
            Assert.IsTrue(toOpponentKingPathList.Contains(53));


        }

        [TestMethod]
        public void T117_B_NotInChessGetToOpponentKingPath32to4()
        {


            var testName = "T117_B_NotInChessGetToOpponentKingPath32to4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var toOpponentKingPathList = boardGPT.GetToOpponentKingPath(32, 4);
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

            BoardGPT boardGPT = Chess2Utils.GenerateBoardFormPawnListGPT(pawnList);

            var toOpponentKingPathList = boardGPT.GetToOpponentKingPath(36, 4);
            Assert.AreEqual(toOpponentKingPathList.Count, 3);
            Assert.IsTrue(toOpponentKingPathList.Contains(28));
            Assert.IsTrue(toOpponentKingPathList.Contains(20));
            Assert.IsTrue(toOpponentKingPathList.Contains(12));



        }

    }
}
