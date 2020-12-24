namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new ThreeDMaze();
            instance.SetupConsole("Three Dee Maze", 180, 60);
            instance.Start();
        }
    }
}