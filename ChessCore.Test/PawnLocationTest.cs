using ChessCore.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore.Test
{
    [TestClass]
    public class PawnLocationTest
    {
        private string testsDirrectory = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.Combine(Environment.CurrentDirectory)).ToString()).ToString()).ToString(), "TESTS");


        /*tsiry;03-07-2022*/
        [TestMethod]
        public void BlackGetWeigtOpionionMenacedsByToIndex29()
        {
            var testName = "T100GetWeigtOpionionMenacedsByToIndexBlackG6ToF5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.GetWeigtOpionionMenacedsByToIndex(board, "W", 29);



            Assert.AreEqual(60, result);
        }


        /*tsiry;03-07-2022*/
        [TestMethod]
        public void BlackGetWeigtOpionionMenacedsByToIndex35()
        {
            var testName = "T105GetWeigtOpionionMenacedsByToIndexBlackC6ToD4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.GetWeigtOpionionMenacedsByToIndex(board, "W", 35);



            Assert.AreEqual(150, result);
        }
        /*tsiry;03-07-2022*/
        [TestMethod]
        public void WhiteIsInChessT95ZWhiteIsInChess()
        {
            var testName = "T95ZWhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "W");



            Assert.IsTrue(result);
        }

        /*tsiry;03-07-2022*/
        [TestMethod]
        public void WhiteIsInChessT95Z1WhiteIsNotInChess()
        {
            var testName = "T95Z1WhiteIsNotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "W");



            Assert.IsFalse(result);
        }


        /*tsiry;02-07-2022*/
        [TestMethod]
        public void WhiteIsInChessT29WhiteIsInChess()
        {
            var testName = "T29WhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "W");



            Assert.IsTrue(result);
        }

        /*tsiry;03-07-2022*/
        [TestMethod]
        public void WhiteIsInChessT87WhiteIsInChess0()
        {
            var testName = "T87WhiteIsInChess0";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "W");



            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhiteIsNotInChessT87WhiteIsNotInChess1()
        {
            var testName = "T87WhiteIsNotInChess1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "W");



            Assert.IsFalse(result);
        }

        /*tsiry;02-07-2022*/
        [TestMethod]
        public void BlackIsInChessT07bInChess()
        {
            var testName = "T07bInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsTrue(result);
        }


        /*tsiry;02-07-2022*/
        [TestMethod]
        public void BlackIsInChessT07aInChess0()
        {
            var testName = "T07aInChess0";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BlackIsInChessT07aInChess1()
        {
            var testName = "T07aInChess0";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BlackIsNotInChesT27BlackIsNotInChess()
        {
            var testName = "T27BlackIsNotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsFalse(result);
        }
        [TestMethod]
        public void BlackkingIsNotProtecctedT07aInChess0()
        {
            var testName = "T07aInChess0";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetKingColorIsProteted(board, "B");



            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BlackkingIsProtecctedT27BlackIsNotInChess()
        {
            var testName = "T27BlackIsNotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetKingColorIsProteted(board, "B");



            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BlackIsnotInChessT07aNotInChess0()
        {
            var testName = "T07aNotInChess0";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsFalse(result);
        }
        [TestMethod]
        public void BlackIsnotInChessT07aNotInChess1()
        {
            var testName = "T07aNotInChess1";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsFalse(result);
        }
        [TestMethod]
        public void BlackIsnotInChessT60BlackIsNotInChess()
        {
            var testName = "T60BlackIsNotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetColorIsInChess(board, "B");



            Assert.IsFalse(result);
        }

        /*tsiry;02-07-2022*/
        [TestMethod]
        public void BlackkingIsManced()
        {
            var testName = "T07aInChess0";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetKingIsMenaced(board, "B");



            Assert.IsTrue(result);
        }





        [TestMethod]
        public void White51IsNotMenaced()
        {

            var testName = "T95ZWhiteIsNotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var pinionColor = "B";
            var color = "W";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, pinionColor, 51) > 0;
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void White53IsMenaced()
        {

            var testName = "T95ZWhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var opinionColor = "B";
            var color = "W";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 53) > 0;
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void White61IsMenaced()
        {

            var testName = "T95ZWhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var opinionColor = "B";
            var color = "W";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 61) > 0;
            Assert.IsTrue(result);
        }

        /*tsiry;14-07-2022*/
        [TestMethod]
        public void WhiteB8IsMenaced()
        {
            var testName = "T99WhiteNotC7ToF7";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var result = Chess2Utils.TargetIndexIsMenaced(board, "W", 1);




            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Black13IsMenaced()
        {

            var testName = "T62Z213IsMenaced";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var opinionColor = "W";
            var color = "B";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 13) > 0;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void T62Z213IsMenacedBykingAndQueenIsMenacedReturn90()//9 est le poid minimum des menacants
        {

            var testName = "T62Z213IsMenacedBykingAndQueen";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var opinionColor = "B";
            var color = "W";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 13);
            Assert.AreEqual(result,90);
        }
        [TestMethod]
        public void T108White37IsMenacedBykingAndQueenIsMenacedReturn30()//9 est le poid minimum des menacants
        {

            var testName = "T108WhiteF2ToF4";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var opinionColor = "B";
            var color = "W";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 37);
            Assert.AreEqual(result, 30);
        }

        [TestMethod]
        public void T109Black17IsMenacedBykingAndQueenIsMenacedReturn10()//9 est le poid minimum des menacants
        {

            var testName = "T109BlackNotB6";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            var opinionColor = "B";
            var color = "W";
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 17);
            Assert.AreEqual(result, 10);
        }




        [TestMethod]
        public void WhiteF2IsProtected_IsProtected()
        {

            var testName = "T95ZWhiteF2IsProtected";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            Utils.OpinionColor = "B";
            Utils.ComputerColor = "W";
            var result = nodeChess2.GetIsLocationIsProtected(52, "W", "B");
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void WhiteG3IsNotProtected_IsProtected()
        {

            var testName = "T95ZWhiteG3IsNotProtected";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            //Utils.OpinionColor = "B";
            //Utils.Color = "W";
            var result = nodeChess2.GetIsLocationIsProtected(46, "W", "B");
            Assert.IsFalse(result);
        }



        [TestMethod]
        public void WhiteF7IsProtected()
        {

            var testName = "TPositionWhiteF7IsProtected";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);

            var result = nodeChess2.GetIsLocationIsProtected(13, "W", "B");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FillePossiblesMoves_TowTimes()
        {
            var computerColore = "White";
            var testName = "T95BLeCavalierBlanchDoitAttaquerEnF2";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var boad = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            var computerPawnsIndex = boad.GetCasesIndexForColor("W").ToList();
            var opinionPawnsIndex = boad.GetCasesIndexForColor("B").ToList();
            var possibleMoveList = new List<PossibleMove>();
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }

            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            //Tow
            foreach (var index in computerPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }
            foreach (var index in opinionPawnsIndex)
            {
                possibleMoveList.AddRange(boad.GetPossibleMoves(index, 1));
            }

            /* using (var chess2UtilsNotStatic = new Chess2UtilsNotStatic())
             {
                 var nodeResult = chess2UtilsNotStatic.GetBestPositionLocalUsingMiltiThreading(computerColore, Chess2Utils.GenerateBoardFormPawnList(pawnList), true, null);
                 Assert.AreEqual(nodeResult.BestChildPosition, "f2");

             }*/
            Assert.AreEqual(possibleMoveList.Count, 65 * 10);
        }
    }
}
