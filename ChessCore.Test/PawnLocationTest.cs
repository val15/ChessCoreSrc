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
        public void BlackWin_WhiteIsInChess()
        {
           
            var testName = "T93ZWhiteIsInChess";
            var testPath = Path.Combine(testsDirrectory, testName);
            var pawnList = Chess2Utils.LoadFromDirectorie(testPath);
            var board = Chess2Utils.GenerateBoardFormPawnList(pawnList);
            NodeChess2 nodeChess2 = new NodeChess2(null, board, 0, "W", 0, 0, "B", 0);

            var result = nodeChess2.GetIsInChess("B");
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

            var result = nodeChess2.GetIsLocationIsProtected(13,"W","B");
            Assert.IsTrue(result);
        }

    }
}
