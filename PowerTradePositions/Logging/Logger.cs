// simple logging implementation 
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
            Console.WriteLine(message + " Exception: " + ex.Message);
        }
    }
}
