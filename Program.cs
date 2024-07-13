using System.Text;

namespace DotnetMatrix
{
    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly ConsoleColor InitialBackgroundColor = Console.BackgroundColor;
        private static readonly ConsoleColor InitialForegroundColor = Console.ForegroundColor;
        private static readonly bool InitialCursorVisible = true;
        private static readonly ConsoleColor[] Colors = [ConsoleColor.Green, ConsoleColor.DarkGreen];
        private static readonly char[] Glyphs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+-*/%=!?#$&@()[]{}<>,;:_^~`".ToCharArray();
        private static readonly string HookLine = " Follow the white rabbit...";
        private static readonly List<MatrixString> MatrixStrings = new List<MatrixString>();
        private const int MaxStreams = 250;
        private const int MinStreamLength = 16;
        private const int MaxStreamLength = 64;
        private const int MinStreamSpeed = 10;
        private const int MaxStreamSpeed = 200;
        private const double ColorChangeProbability = 0.1;
        private const double WhiteTipProbability = 0.8;
        private const double MatrixStringEndProbability = 0.01;

        static void Main(string[] args)
        {
            // Ensure the console uses an appropriate font
            Console.OutputEncoding = Encoding.UTF8;

            Console.Clear();
            InitializeConsole();

            while (true)
            {
                // Create new Matrix strings until the maximum is reached
                if (MatrixStrings.Count < MaxStreams)
                    CreateMatrixString();

                // Update the position and status of the Matrix strings
                UpdateMatrixStrings();

                // Render the Matrix strings on the console
                RenderMatrixStrings();

                // Wait for 50ms
                Thread.Sleep(50);

                // End the program if a key is pressed
                if (Console.KeyAvailable)
                    break;
            }

            // Display the message in typewriter style
            TypeWriterOutput(HookLine);

            // Restore the original console settings
            RestoreConsoleDefaults();

            // A blank line for aesthetics
            Console.WriteLine(" ");
        }

        // Initialize console settings for the Matrix effect
        static void InitializeConsole()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;

            Console.Clear();
        }

        // Restore the original console settings
        static void RestoreConsoleDefaults()
        {
            Console.BackgroundColor = InitialBackgroundColor;
            Console.ForegroundColor = InitialForegroundColor;
            Console.CursorVisible = InitialCursorVisible;
        }

        // Display a message in typewriter style
        static void TypeWriterOutput(string message)
        {
            Console.Clear();
            InitializeConsole();
            Console.WriteLine(" ");

            // Write character by character with a random delay
            foreach (char c in message)
            {
                Thread.Sleep(Random.Next(50, 500));
                Console.Write(c);
            }

            // Short pause for dramatic effect
            Thread.Sleep(250);
            Console.WriteLine(" ");

            RestoreConsoleDefaults();
        }

        // Create a new Matrix string
        static void CreateMatrixString()
        {
            int column = Random.Next(Console.WindowWidth);
            int length = Random.Next(MinStreamLength, MaxStreamLength);
            int speed = Random.Next(MinStreamSpeed, MaxStreamSpeed);
            ConsoleColor color = Colors[Random.Next(Colors.Length)];

            MatrixStrings.Add(new MatrixString(column, -length, length, speed, color));
        }

        // Update the Matrix strings
        static void UpdateMatrixStrings()
        {
            for (int i = 0; i < MatrixStrings.Count; i++)
            {
                MatrixString matrixString = MatrixStrings[i];

                // Check if it's time to update the Matrix string
                if (Environment.TickCount - matrixString.LastUpdate > matrixString.Speed)
                {
                    matrixString.DropDown();

                    // End the Matrix string randomly
                    if (Random.NextDouble() < MatrixStringEndProbability)
                        matrixString.KillMatrixString();

                    matrixString.LastUpdate = (uint)Environment.TickCount;
                }

                // Remove ended Matrix strings
                if (matrixString.IsEnded)
                {
                    MatrixStrings.RemoveAt(i);
                    i--;
                }
            }
        }

        // Render the Matrix strings on the console
        static void RenderMatrixStrings()
        {
            foreach (MatrixString matrixString in MatrixStrings)
            {
                for (int i = 0; i < matrixString.Length; i++)
                {
                    int x = matrixString.Column;
                    int y = matrixString.Row - i;

                    if (y >= 0 && y < Console.WindowHeight)
                    {
                        char symbol = Glyphs[Random.Next(Glyphs.Length)];
                        Console.ForegroundColor = i == 0 && Random.NextDouble() < WhiteTipProbability ? ConsoleColor.White : matrixString.Color;

                        Console.SetCursorPosition(x, y);

                        if (i == matrixString.Length - 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }

                        Console.Write(symbol);
                    }
                }
            }
        }
    }

    class MatrixString
    {
        public int Column { get; }
        public int Row { get; private set; }
        public int Length { get; }
        public int Speed { get; }
        public ConsoleColor Color { get; }
        public uint LastUpdate { get; set; }
        public bool IsEnded { get; private set; }

        public MatrixString(int column, int row, int length, int speed, ConsoleColor color)
        {
            Column = column;
            Row = row;
            Length = length;
            Speed = speed;
            Color = color;
            LastUpdate = (uint)Environment.TickCount;
            IsEnded = false;
        }

        // Move the Matrix string down by one row
        public void DropDown()
        {
            Row++;
        }

        // Mark the Matrix string as ended
        public void KillMatrixString()
        {
            IsEnded = true;
        }
    }
}
