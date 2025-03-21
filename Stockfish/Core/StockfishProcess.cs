using System.Diagnostics;
using System.Text;

namespace Stockfish.Core
{
    internal class StockfishProcess
    {
        /// <summary>
        /// Default process info for Stockfish process
        /// </summary>
        private ProcessStartInfo _processStartInfo { get; set; }

        /// <summary>
        /// Stockfish process
        /// </summary>
        private Process _process { get; set; }

        /// <summary>
        /// Stockfish process constructor
        /// </summary>
        /// <param name="path">Path to usable binary file from stockfish site</param>
        public StockfishProcess(string path)
        {
            //TODO: need add method which should be depended on os version
            _processStartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            _process = new Process { StartInfo = _processStartInfo };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecond"></param>
        public void Wait(int millisecond)
        {
            this._process.WaitForExit(millisecond);
        }

        /// <summary>
        /// This method is writing in stdin of Stockfish process
        /// </summary>
        /// <param name="command"></param>
        public void WriteLine(string command)
        {
            if (_process.StandardInput == null)
            {
                throw new NullReferenceException();
            }
            _process.StandardInput.WriteLine(command);
            _process.StandardInput.Flush();
        }

        /// <summary>
        /// This method is allowing to read stdout of Stockfish process
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            if (_process.StandardOutput == null)
            {
                throw new NullReferenceException();
            }


            //var tall = ReadAllLines(2);
            return _process.StandardOutput.ReadLine();
        }

        public string ReadAllOutputOLD()
        {
            if (_process.StandardOutput == null)
            {
                throw new NullReferenceException("Le StandardOutput est null.");
            }
            var allLines = _process.StandardOutput.ReadToEnd();
            return allLines;
        }


        public async Task<string> ReadAllOutputAsync(int timeoutMilliseconds = 500)
        {
            if (_process.StandardOutput == null)
            {
                throw new NullReferenceException("StandardOutput is null.");
            }

            var output = new StringBuilder();
            // Crée un token de cancellation pour arrêter après timeoutMilliseconds sans nouvelle donnée
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(timeoutMilliseconds);

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    // Vérifie s'il y a des données disponibles dans le buffer
                    if (_process.StandardOutput.Peek() > -1)
                    {
                        string line = await _process.StandardOutput.ReadLineAsync();
                        if (line == null)
                        {
                            break; // fin du flux
                        }
                        output.AppendLine(line);
                        // Réinitialise le timer dès qu'on lit une ligne
                        cts.CancelAfter(timeoutMilliseconds);
                    }
                    else
                    {
                        // Petite pause pour éviter de boucler en continu
                        await Task.Delay(50, cts.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Le délai d'attente est écoulé, on quitte la boucle et on retourne ce qui a été lu
            }

            return output.ToString();
        }
        public async Task<string> ReadAllOutputUntilBestmoveAsync()
        {
            if (_process.StandardOutput == null)
            {
                throw new NullReferenceException("StandardOutput is null.");
            }

            var output = new StringBuilder();
            while (true)
            {
                string line = await _process.StandardOutput.ReadLineAsync();
                if (line == null)
                {
                    break; // Fin du flux
                }
                output.AppendLine(line);
                // Si la ligne contient "bestmove", on arrête la lecture
                if (line.Contains("bestmove"))
                {
                    break;
                }
            }
            return output.ToString();
        }


        public List<string> ReadLastLines(int x)
        {
            if (_process.StandardOutput == null)
            {
                throw new NullReferenceException("La sortie standard du processus est null.");
            }

            // Utiliser une file pour stocker les X dernières lignes
            Queue<string> lastLines = new Queue<string>(x);

            // Lire les lignes jusqu'à la fin de la sortie standard
            while (lastLines.Count < x)
            {
                string line = _process.StandardOutput.ReadLine();

                // Ajouter la ligne à la file
                if (lastLines.Count >= x)
                {
                    lastLines.Dequeue(); // Retirer la ligne la plus ancienne si la file est pleine
                }
                lastLines.Enqueue(line); // Ajouter la nouvelle ligne
            }

            // Retourner les X dernières lignes sous forme de chaîne concaténée
            //return string.Join(Environment.NewLine, lastLines);
            return lastLines.ToList();
        }

        /// <summary>
        /// Start stockfish process
        /// </summary>
        public void Start()
        {
            _process.Start();
        }
        /// <summary>
        /// This method is allowing to close Stockfish process
        /// </summary>
        ~StockfishProcess()
        {
            //When process is going to be destructed => we are going to close stockfish process
            _process.Close();
        }
    }

}
