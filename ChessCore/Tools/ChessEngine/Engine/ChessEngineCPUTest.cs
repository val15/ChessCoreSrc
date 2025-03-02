using ComputeSharp;
using ComputeSharp.Descriptors;
using ComputeSharp.Interop;


namespace ChessCore.Tools.ChessEngine.Engine
{
    public class ChessEngineCPUTest : IChessEngine
    {
        private static object lockObj = new object();
        private int _depthLevel = 0;

        public void Dispose() { }

        public string GetName()
        {
            return this.GetName();
        }

        public string GetShortName()
        {
            return Utils.ExtractUppercaseLettersAndDigits(GetName());
        }
        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 5, int maxReflectionTimeInSecond = 60 * 2)
        {
            var cpuColor = colore.First().ToString();
            _depthLevel = depthLevel;
            string opponentColor = boardChess.GetOpponentColor(cpuColor);

            Utils.WritelineAsync($"CHESS ENGINE GPU :");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");

            var bestOfBest = FindBestMode(boardChess, depthLevel, cpuColor);
            return bestOfBest;
        }

        public NodeCE FindBestModeOLD(BoardCE board, int depthLevel, string cpuColor)
        {
            var startTime = DateTime.UtcNow;
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor);
            var bestNodeList = new List<NodeCE>();

            Parallel.ForEach(possibleMoves, move =>
            {
                var clonedBoard = board.CloneAndMove(move);
                int value = MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);
                var elapsed = DateTime.UtcNow - startTime;
                var currentNode = new NodeCE(clonedBoard, move, value, depthLevel, elapsed);

                lock (lockObj)
                {
                    bestNodeList.Add(currentNode);
                }
            });

            return bestNodeList.OrderByDescending(x => x.Weight).FirstOrDefault();
        }


        public NodeCE FindBestMode(BoardCE board, int depthLevel, string cpuColor)
        {

            //var device = GraphicsDevice.GetDefault();
            //Console.WriteLine($"ComputeSharp utilise le GPU : {device.Name}");

            // Sélectionner le GPU sur lequel exécuter (exemple : 2e GPU)
            var possibleMoves = board.GetPossibleMovesForColor(cpuColor);
            int[] scores = new int[possibleMoves.Count];
            List<Move> movesList = possibleMoves.ToList();

            using (var buffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(scores))
            {
                Parallel.ForEach(possibleMoves, (move, state, index) =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    scores[index] = MinMaxWithAlphaBeta(clonedBoard, depthLevel - 1, int.MinValue, int.MaxValue, false, cpuColor);
                });

                buffer.CopyTo(scores);
            }

            int bestScore = scores.Max();
            int bestIndex = Array.IndexOf(scores, bestScore);
            Move bestMove = movesList[bestIndex];

            return new NodeCE(board.CloneAndMove(bestMove), bestMove, bestScore, depthLevel, TimeSpan.Zero);
        }

        private int MinMaxWithAlphaBetaOLD(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);

            if (depth == 0 || board.IsGameOver())
            {
                return board.CalculateBoardCEScore(cpuColor, opponentColor);
            }

            var moves = board.GetPossibleMovesForColor(maximizingPlayer ? cpuColor : opponentColor);
            if (maximizingPlayer)
            {
                int bestValue = int.MinValue;
                Parallel.ForEach(moves, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, false, cpuColor);
                    lock (lockObj) { bestValue = Math.Max(bestValue, value); }
                });
                return bestValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                Parallel.ForEach(moves, move =>
                {
                    var clonedBoard = board.CloneAndMove(move);
                    int value = MinMaxWithAlphaBeta(clonedBoard, depth - 1, alpha, beta, true, cpuColor);
                    lock (lockObj) { bestValue = Math.Min(bestValue, value); }
                });
                return bestValue;
            }
        }



        private int MinMaxWithAlphaBeta(BoardCE board, int depth, int alpha, int beta, bool maximizingPlayer, string cpuColor)
        {
            var opponentColor = board.GetOpponentColor(cpuColor);
            int[] scores = new int[board.GetPossibleMovesForColor(cpuColor).Count];

            using (var buffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(scores))
            {
                // GraphicsDevice.GetDefault().For(scores.Length, new MinMaxShader(buffer, depth, alpha, beta, maximizingPlayer));
                //GraphicsDevice.GetDefault().For(scores.Length, new MinMaxShader(buffer, alpha, beta, depth, maximizingPlayer));

                //GraphicsDevice.GetDefault().ForEach(scores.Length, new MinMaxShader(buffer, alpha, beta, depth, maximizingPlayer));
                //GraphicsDevice.GetDefault().For<MinMaxShader>(scores.Length, new MinMaxShader(buffer, alpha, beta, depth, maximizingPlayer));


                buffer.CopyTo(scores);
            }

            return maximizingPlayer ? scores.Max() : scores.Min();
        }







        //private int CalculateBoardCEScoreGPU(BoardCE board, string cpuColor, string opponentColor)
        //{
        //    int[] boardData = board._cases.ToIntArray(); // Convertir en tableau d'entiers
        //    int[] result = new int[1];

        //    using ReadWriteBuffer<int> boardBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(boardData);
        //    using ReadWriteBuffer<int> resultBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(result);

        //    GraphicsDevice.GetDefault().For(1, new EvaluateBoardShader(boardBuffer, resultBuffer, cpuColor == "W" ? 1 : -1));

        //    resultBuffer.CopyTo(result);
        //    return result[0];
        //}
    }

    //public readonly struct EvaluateBoardShader : IComputeShader
    //{
    //    private readonly ReadWriteBuffer<int> board;
    //    private readonly ReadWriteBuffer<int> result;
    //    private readonly int colorMultiplier;

    //    public EvaluateBoardShader(ReadWriteBuffer<int> board, ReadWriteBuffer<int> result, int colorMultiplier)
    //    {
    //        this.board = board;
    //        this.result = result;
    //        this.colorMultiplier = colorMultiplier;
    //    }

    //    public void Execute()
    //    {
    //        int sum = 0;
    //        for (int i = 0; i < board.Length; i++)
    //        {
    //            sum += board[i] * colorMultiplier;
    //        }
    //        result[0] = sum;
    //    }
    //}


}
