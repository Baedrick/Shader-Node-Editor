// See https://aka.ms/new-console-template for more information

namespace ShaderNodeEditor
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var game = new Game(800, 600, "LearnOpenTK");
            game.Run();
        }
    }
}