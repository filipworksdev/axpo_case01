using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PowerTradePositions.Logging
{
    public class Logger
    {
        public void LogInformation(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(Exception ex, string message) 
        { 
            Console.WriteLine(ex.Message + "\n" + message);
        }
    }
}
