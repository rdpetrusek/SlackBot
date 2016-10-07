using StructureMap;
using System.Configuration;
namespace SlackBot
{
    class Program
    {
        static void Main(string[] args)
        {
            StructureMapConfiguration.Initialize();

            var token = ConfigurationManager.AppSettings["SlackRoundRobinBotToken"];

            var squattyBotty = new RoundRobinSlackBot(token);
            while (true)
            {
                
            }
        }
    }
}
