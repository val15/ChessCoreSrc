#pragma warning disable SKEXP0001  // Désactive l'avertissement SKEXP0001
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using System.Drawing;

namespace ChessCore.Tools.ChessEngine.Engine
{

    public class ChessEngineLLM : IChessEngine
    {
        private readonly string _modelName = "llama3";
        private IChatCompletionService _chatService;
        private ChatHistory _history;

        public void Dispose() { }

        public string GetName()
        {
            return this.GetType().Name;
        }

        public string GetShortName()
        {
            return Utils.ExtractUppercaseLettersAndDigits(GetName());
        }

        public ChessEngineLLM()
        {
            Console.WriteLine("Chess IA");
            Console.WriteLine("===============");
            //  var startDate = DateTime.Now;
            




            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434"),
                Timeout = TimeSpan.FromMinutes(5),
            };

            _chatService = new OllamaChatCompletionService(_modelName, httpClient);
            _history = new();
            //var systemMessage = "Vous êtes un assistant d'échecs compétent. Vous pouvez analyser des positions d'échecs, proposer des coups et discuter de stratégies. Vous utiliserez le moteur Stockfish pour valider vos analyses et suggestions.";
           // _history.AddSystemMessage(modelFileContent);


        }

        public NodeCE GetBestModeCE(string colore, BoardCE boardChess, int depthLevel = 6, int maxReflectionTimeInMinute = 2)
        {

            var cpuColor = colore.First().ToString();

            string opponentColor = boardChess.GetOpponentColor(cpuColor);

            Utils.WritelineAsync($"{GetName()}");
            Utils.WritelineAsync($"cpuColor :  {cpuColor}");
            Utils.WritelineAsync($"opponentColor :  {opponentColor}");

            var bestOfBest = FindBestMode(boardChess, cpuColor);
            return bestOfBest;
        }

        public NodeCE FindBestMode(BoardCE board, string cpuColor)
        {
            try
            {
                var possibleMovesForColor = board.GetPossibleMovesForColor(cpuColor, true);

                var startTime = DateTime.Now;

                var color = "White";
                if (cpuColor == "B")
                    color = "Black";

                Console.WriteLine(_modelName);
                NodeCE bestNodeCE = null;
                while (true)
                {
                    // var initialBoardString = board.ToString();
                    var initialBoardString = board.ToString();
                    var FEN = board.ConvertToFEN();


                    var prompt = $"Position FEN: {FEN} Side to move: {color} Analyze this chess position and provide the best move for {color} in UCI format. Remember:" +
      $"1.Only output a single UCI move(e.g. 'e2e4')" +
      $"2.Ensure the move is legal for {color}" +
      $"3.Use proper UCI format(e.g: 'e2e4', 'g1f3', 'e1g1')" +
      $"Your move:";


                    Utils.WritelineAsync($"prompt : {prompt}");

                    _history.AddUserMessage(prompt);




                    var assistant = _chatService.GetChatMessageContentAsync(_history).Result;
                    _history.Add(assistant);
                    var llmResult = assistant.ToString();
                    Utils.WritelineAsync(llmResult);







                    var uciMove = Utils.ExtractUCIMove(llmResult);
                    Utils.WritelineAsync($"uciMove: {uciMove}");
                   

                    
                    bestNodeCE = new NodeCE(uciMove.Substring(0, 2), uciMove.Substring(2, 2));

                    // Check if possibleMovesForColor contains a move with the same ToIndex as bestNodeCE.ToIndex
                    if (possibleMovesForColor.Any(move => move.ToIndex == bestNodeCE.ToIndex))
                    {
                        break;
                    }
                    Utils.WritelineAsync("incorrect proposition");
                    _history.AddUserMessage($"Your proposal is incorrect because {uciMove} is not a valid move according to the chessboard that I sent you!");
                }





                var elapsed = DateTime.Now - startTime;
                Utils.WritelineAsync($"REFLECTION TIME: {elapsed}");
                bestNodeCE.ReflectionTime = elapsed;

                return bestNodeCE;
            }
            catch (Exception ex)
            {

                return null;
            }


        }


        [Obsolete]
        public async Task TestChess()
        {
            Console.WriteLine("Chess IA");
            Console.WriteLine("===============");

            // IChatCompletionService chatService = new AzureOpenAIChatCompletionService("gpt-4o", Environment.GetEnvironmentVariable("AI:AzureOpenAI:Endpoint"), Environment.GetEnvironmentVariable("AI:AzureOpenAI:Key"));
            // var modelName = "deepseek-r1";
            //var modelName = "deepseek-r1:1.5b";
            //var modelName = "llama3.2";//1
            //var modelName = "mistral";
            //  var modelName = "llama3";
            //var modelName = "gemma:2b";

            //var modelName = "deepseek-r1";
            //var modelName = "deepseek-r1:1.5b";
            //var modelName = "llama3";
            //var modelName = "gemma:2b";
            // Créer une instance du Kernel
            //var kernel = Kernel.CreateBuilder()
            //    .AddOllamaChatCompletion(modelName, new Uri("http://localhost:11434"))
            //    .Build();



            IChatCompletionService chatService = new OllamaChatCompletionService(_modelName, new Uri("http://localhost:11434"));


            // (modelName, new Uri("http://localhost:11434"));


            ChatHistory history = new();

            // history.AddSystemMessage("Bonjour, tu es une IA ayant un problème d'Alcool, tu es là pour rappeler que tu as eu un accident en étant bourré, et tu t'appelles Cédric. A chaque reponse, tu dois demander qu'est-ce que l'on boit et proposer une bière belge.");
            history.AddSystemMessage("Hello, you are an AI specializing in chess games." +
                "each time I will send you the chess board like this:" +
                "T|B;C|B;B|B;Q|B;K|B;B|B;C|B;T|B;P|B;P|B;P|B;P|B;P|B;P|B;P|B;P|B;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__; __;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;P|W;P|W;P|W;P|W;P|W;P|W;P|W;P|W;T|W;C|W;B|W;Q|W;K|W;B|W;C|W;T|W;" +
                ". I give you a color W or B, and you MUST GIVE me the best shots like this:" +
                "G6 (22) => F6 (21): -90." +
                "G6 (22) starting position and index and F6 (21) position and index of the best move and -90 the score of the best move" +
                "If there are several moves with the same score, you must give me the list of the best moves and choose one at random.");

            //while (true)
            //{
            //Console.Write("Question ? : ");
            //var initialBoardString = "T|B;C|B;B|B;Q|B;K|B;B|B;C|B;T|B;P|B;P|B;P|B;P|B;P|B;P|B;P|B;P|B;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;__;P|W;P|W;P|W;P|W;P|W;P|W;P|W;P|W;T|W;C|W;B|W;Q|W;K|W;B|W;C|W;T|W;";
            //Console.WriteLine(modelName);
            var initialBoardString = "T|B;C|B;B|B;Q|B;K|B;B|B;__;T|B;P|B;P|B;P|B;P|B;P|B;P|B;P|B;P|B;__;__;__;__;__;C|B;__;__;__;__;__;Q|W;__;__;__;__;__;__;P|W;__;__;__;__;__;__;__;__;__;__;__;__;__;P|W;P|W;__;P|W;P|W;P|W;P|W;P|W;T|W;C|W;B|W;__;K|W;B|W;C|W;T|W;";
            history.AddUserMessage(initialBoardString);
            history.AddUserMessage("Color : B");
            var startDate = DateTime.Now;
            var assistant = await chatService.GetChatMessageContentAsync(history);
            history.Add(assistant);
            Console.WriteLine(assistant);
            Console.WriteLine($"REFLECTION TIME :{DateTime.Now - startDate}");

            // }

        }



    }
}
