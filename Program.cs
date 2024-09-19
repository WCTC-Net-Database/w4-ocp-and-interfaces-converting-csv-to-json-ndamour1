using W4_assignment_template.Interfaces;
using W4_assignment_template.Models;
using W4_assignment_template.Services;
using CharacterConsole;

namespace W4_assignment_template;

class Program
    {
        static void Main()
        {
            var input = new ConsoleInput();
            var output = new ConsoleOutput();

            CharacterManager manager = new CharacterManager(input, output);
            manager.Run();
        }
    }

    class ConsoleInput : IInput
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }

    class ConsoleOutput : IOutput
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Write(string message)
        {
            Console.Write(message);
        }
    }
