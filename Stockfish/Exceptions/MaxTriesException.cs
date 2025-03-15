using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockfish.Exceptions
{
    public class MaxTriesException : Exception
    {
        public MaxTriesException(string msg = "") : base(msg) 
        { 
            Console.WriteLine("MaxTriesException");
        }
    }
}
