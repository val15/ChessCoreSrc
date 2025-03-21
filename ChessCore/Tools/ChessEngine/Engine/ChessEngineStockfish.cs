
using ChessCore.Tools.ChessEngine.Engine.Interfaces;
using Stockfish.Core;
using System;
using System.Xml.Linq;



namespace ChessCore.Tools.ChessEngine.Engine
{

    public class ChessEngineStockfish : IChessEngine
    {



        private IStockfish _stockfish { get; set; }
        private readonly int _depthFactor = 5;

        private readonly string _path;

        public void Dispose() { }

        public string GetName()
        {
            return this.GetType().Name;
        }

        public string GetShortName()
        {
            return Utils.ExtractUppercaseLettersAndDigits(GetName());
        }

        public ChessEngineStockfish()
        {
            Console.WriteLine("Chess Stockfish");
            Console.WriteLine("===============");
            //  var startDate = DateTime.Now;



             _path = Utils.GetStockfishDir();
            if (!File.Exists(_path))
            {
                throw new Exception("Tockfish exe not exist");
            }

           
        }

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {

            var cpuColor = colore.First().ToString();

            string opponentColor = boardChess.GetOpponentColor(cpuColor);
            var realDepthLevel = _depthFactor * depthLevel;
            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"DepthLevel :  {depthLevel}");
            Utils.WritelineAsync($"DepthFactor :  {_depthFactor}");
            Utils.WritelineAsync($"Real depth level :  {realDepthLevel}");

            Utils.WritelineAsync($"cpuColor :  {cpuColor}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");


            _stockfish =  new Stockfish.Core.Stockfish(_path, depth: realDepthLevel);
            //_stockfish =  new Stockfish.NET.Stockfish(_path,depthLevel);

            var bestOfBest = FindBestMode(boardChess, cpuColor);
            return bestOfBest;
        }

        public NodeCE FindBestMode(BoardCE board, string cpuColor)
        {
            try
            {
               
                var startTime = DateTime.Now;

                NodeCE bestNodeCE = null;


                var finalFENWithColore = board.ConvertToFEN();
                if (cpuColor == "B")
                    finalFENWithColore = finalFENWithColore.Replace("w", "b");
                _stockfish.SetFenPosition(finalFENWithColore);

                var bestMoveAndInfoString = _stockfish.GetBestMove();
               
                var infoData = bestMoveAndInfoString.Split(";");
                
               



                var info = infoData[0];
                Utils.WritelineAsync($"Stockfish : {info}");

                var depth = string.Empty;
                var weight = string.Empty;

                var finalInfoLineData = info.Split(' ');
                depth = finalInfoLineData[2];
                weight = finalInfoLineData[9];

                var bestMoveString = infoData[1];


                bestNodeCE = new NodeCE(bestMoveString.Substring(0, 2), bestMoveString.Substring(2, 2),Int32.Parse(weight),Int32.Parse(depth));


                var elapsed = DateTime.Now - startTime;
                Utils.WritelineAsync($"REFLECTION TIME: {elapsed}");
                bestNodeCE.ReflectionTime = elapsed;

                Utils.WritelineAsync($"bestNodeCEList :");
                Utils.WritelineAsync($"{bestNodeCE}");

                return bestNodeCE;
            }
            catch (Exception ex)
            {

                return null;
            }


        }


       


    }
}
