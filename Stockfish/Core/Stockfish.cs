using Stockfish.Exceptions;
using Stockfish.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Color = Stockfish.Models.Color;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;

namespace Stockfish.Core
{
    public class Stockfish : IStockfish
    {
        #region private variables

        /// <summary>
        /// 
        /// </summary>
        /// //ORIGINAL
      //  private const int MAX_TRIES = 200;
        private const int MAX_TRIES = 400;

        /// <summary>
        /// 
        /// </summary>
        private int _skillLevel;

        #endregion

        # region private properties

        /// <summary>
        /// 
        /// </summary>
        private StockfishProcess _stockfish { get; set; }

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SkillLevel
        {
            get => _skillLevel;
            set
            {
                _skillLevel = value;
                Settings.SkillLevel = SkillLevel;
                setOption("Skill level", SkillLevel.ToString());
            }
        }

        #endregion

        # region constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="depth"></param>
        /// <param name="settings"></param>
        public Stockfish(
            string path,
            int depth = 2,
            Settings settings = null)
        {
            Depth = depth;
            _stockfish = new StockfishProcess(path);
            _stockfish.Start();
            _stockfish.ReadLine();

            if (settings == null)
            {
                Settings = new Settings();
            }
            else
            {
                Settings = settings;
            }

            SkillLevel = Settings.SkillLevel;
            foreach (var property in Settings.GetPropertiesAsDictionary())
            {
                setOption(property.Key, property.Value);
            }

            startNewGame();
        }

        #endregion

        #region private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="estimatedTime"></param>
        private void send(string command, int estimatedTime = 100)
        {
            _stockfish.WriteLine(command);
            _stockfish.Wait(estimatedTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
		private bool isReady()
        {
            send("isready");
            var tries = 0;
            while (tries < MAX_TRIES)
            {
                ++tries;

                if (_stockfish.ReadLine() == "readyok")
                {
                    return true;
                }
            }
            throw new MaxTriesException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ApplicationException"></exception>
        private void setOption(string name, string value)
        {
            send($"setoption name {name} value {value}");
            if (!isReady())
            {
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moves"></param>
        /// <returns></returns>
        private string movesToString(string[] moves)
        {
            return string.Join(" ", moves);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void startNewGame()
        {
            send("ucinewgame");
            if (!isReady())
            {
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void go()
        {
            send($"go depth {Depth}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        private void goTime(int time)
        {
            send($"go movetime {time}", estimatedTime: time + 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<string> readLineAsList()
        {
            try
            {
                var data = _stockfish.ReadLine();
                
                if (data == null)
                   return new List<string>();
                return data.Split(' ').ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in readLineAsList: " + ex.Message);
                return null;
            }
          
        }

        private string readAllLinesText()
        {
            try
            {
                var data =   _stockfish.ReadAllOutputUntilBestmoveAsync().Result;
                return data;
                
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in readLineAsList: " + ex.Message);
                return null;
            }

        }


        private List<string> readAllLines()
        {
            try
            {
                var data = _stockfish.ReadAllOutputUntilBestmoveAsync().Result;
                var allLines = data.Trim().Split('\n').ToList();


                return allLines;

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in readLineAsList: " + ex.Message);
                return null;
            }

        }



        private List<string> readLinesAsList(int lines)
        {
            try
            {
                var data = _stockfish.ReadLastLines(lines);
                return data;
                //if (data == null)
                //    return new List<string>();
                //return data.Split(' ').ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in readLineAsList: " + ex.Message);
                return null;
            }

        }

        #endregion

        #region public

        /// <summary>
        /// Setup current position
        /// </summary>
        /// <param name="moves"></param>
        public void SetPosition(params string[] moves)
        {
            startNewGame();
            send($"position startpos moves {movesToString(moves)}");
        }

        /// <summary>
        /// Get visualisation of current position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetBoardVisual()
        {
            send("d");
            var board = "";
            var lines = 0;
            var tries = 0;
            while (lines < 17)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                
                
                }

                var data = _stockfish.ReadLine();
                if (data.Contains("+") || data.Contains("|"))
                {
                    lines++;
                    board += $"{data}\n";
                }

                tries++;
            }

            return board;
        }

        /// <summary>
        /// Get position in fen format
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetFenPosition()
        {
            send("d");
            var tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                var data = readLineAsList();
                if (data[0] == "Fen:")
                {
                    return string.Join(" ", data.GetRange(1, data.Count - 1));
                }

                tries++;
            }
        }

        /// <summary>
        /// Set position in fen format
        /// </summary>
        /// <param name="fenPosition"></param>
        public void SetFenPosition(string fenPosition)
        {
            startNewGame();
            send($"position fen {fenPosition}");
        }

        /// <summary>
        /// Getting best move of current position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetBestMove()
        {
            try
            {
                go();
                var tries = 0;
                while (true)
                {
                    if (tries > MAX_TRIES)
                    {
                        throw new MaxTriesException();
                    }

                    var bestMove = string.Empty;
                    

                    var alllDataLiens = readAllLines();

                    var finalInfoLine = alllDataLiens[alllDataLiens.Count-2];
                    var bestNodeLine = alllDataLiens.Last();
                   

                    var bestNodeData = bestNodeLine.Split(' ');
                   
                    if (bestNodeData[0] == "bestmove")
                    {
                        //var dd = readLinesAsList(2);
                        if (bestNodeData[1] == "(none)")
                        {
                            return null;
                        }

                        bestMove= bestNodeData[1];
                        return $"{finalInfoLine};{bestMove}";
                    }

                    

                    tries++;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            
        }
        public string GetBestMoveOld()
        {
            try
            {
                go();
                var tries = 0;
                while (true)
                {
                    if (tries > MAX_TRIES)
                    {
                        throw new MaxTriesException();
                    }

                    
                    var data = readLineAsList();
                    //var dd = readLinesAsList(2);
                    if (data[0] == "bestmove")
                    {
                        //var dd = readLinesAsList(2);
                        if (data[1] == "(none)")
                        {
                            return null;
                        }

                        return data[1];
                    }

                    tries++;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetBestMoveTime(int time = 1000)
        {
            goTime(time);
            var tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                var data = readLineAsList();
                if (data[0] == "bestmove")
                {
                    if (data[1] == "(none)")
                    {
                        return null;
                    }

                    return data[1];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveValue"></param>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public bool IsMoveCorrect(string moveValue)
        {
            send($"go depth 1 searchmoves {moveValue}");
            var tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                var data = readLineAsList();
                if (data[0] == "bestmove")
                {
                    if (data[1] == "(none)")
                    {
                        return false;
                    }

                    return true;
                }

                tries++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public Evaluation GetEvaluation()
        {
            Evaluation evaluation = new Evaluation();
            var fen = GetFenPosition();
            Color compare;
            // fen sequence for white always contains w
            if (fen.Contains("w"))
            {
                compare = Color.White;
            }
            else
            {
                compare = Color.Black;
            }

            // I'm not sure this is the good way to handle evaluation of position, but why not?
            // Another way we need to somehow limit engine depth? 
            goTime(10000);
            var tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException("tries:" + tries + ">max-tries:" + MAX_TRIES);
                }

                var data = readLineAsList();
                if (data[0] == "info")
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i] == "score")
                        {
                            //don't use ternary operator here for readability
                            int k;
                            if (compare == Color.White)
                            {
                                k = 1;
                            }
                            else
                            {
                                k = -1;
                            }

                            evaluation = new Evaluation(data[i + 1], Convert.ToInt32(data[i + 2]) * k);
                        }
                    }
                }

                if (data[0] == "bestmove")
                {
                    return evaluation;
                }

                tries++;
            }
        }

        #endregion
    }

}
