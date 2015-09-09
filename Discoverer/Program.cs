using System;

namespace Discoverer
{
    class Program
    {
        static void Main(string[] args)
        {
            InteractionHandler interactionHandler = new InteractionHandler();
            interactionHandler.Start();

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
