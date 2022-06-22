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


        [TestMethod]
        public void WhiteIsInChess()
        {

            var testName = "T95ZWhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            Utils.OpinionColor = "B";
            Utils.ComputerColor = "W";
           // Utils.MainBord = board;
            var result = nodeChess2.GetIsInChess(Utils.OpinionColor, Utils.ComputerColor);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void WhiteIsNotInChess()
        {

            var testName = "T95ZWhiteIsNotInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            Utils.OpinionColor = "B";
            Utils.ComputerColor = "W";
            //Utils.MainBord = board;
            var result = nodeChess2.GetIsInChess(Utils.OpinionColor, Utils.ComputerColor);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void BlackIsNotInChess()
        {

            var testName = "T62ZLePionNoirDoitAttaqueLaReineBlancEnH5";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "B", 0, 0, "W", 0);
            Utils.ComputerColor = "B";
            Utils.OpinionColor = "W";
           // Utils.MainBord = board;

            var result = nodeChess2.GetIsInChess(Utils.OpinionColor, Utils.ComputerColor);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BlackIsInChess()
        {

            var testName = "T62Z213IsMenaced";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "B", 0, 0, "W", 0);
            Utils.ComputerColor = "B";
            Utils.OpinionColor = "W";
            // Utils.MainBord = board;

            // var result = nodeChess2.GetIsInChess(Utils.OpinionColor, Utils.ComputerColor);
            var result = nodeChess2.GetKingIsInChess(board, Utils.ComputerColor, Utils.OpinionColor);
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
            var result = nodeChess2.TargetIndexIsMenaced(board, color, pinionColor, 51);
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
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 53);
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
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 61);
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
            var result = nodeChess2.TargetIndexIsMenaced(board, color, opinionColor, 13);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void BlackWin_WhiteIsInChess()
        {

            var testName = "T93ZWhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            Utils.OpinionColor = "B";
            Utils.ComputerColor = "W";
           // Utils.MainBord = board;
            var result = nodeChess2.GetIsInChess(Utils.OpinionColor, Utils.ComputerColor);
            Assert.IsTrue(result);
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
            var result = nodeChess2.GetIsLocationIsProtected(52,"W", "B");
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
        public void WhiteIsInChess1()
        {

            var testName = "T95Z1WhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);
            Utils.OpinionColor = "B";
            Utils.ComputerColor = "W";
            //Utils.MainBord = board;
            var result = nodeChess2.GetIsInChess(Utils.OpinionColor, Utils.ComputerColor);
            Assert.IsTrue(result);
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
            var computerPawnsIndex = boad.GetCasesIndex("W").ToList();
            var opinionPawnsIndex = boad.GetCasesIndex("B").ToList();
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
