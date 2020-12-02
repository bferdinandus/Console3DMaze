using System;
using System.Threading;

namespace ConsoleUI
{
    class Program
    {



        static void Main(string[] args)
        {
            var instance =new ThreeDMaze();
            instance.SetupConsole(128, 40);
            instance.Start();


        }
    }
}