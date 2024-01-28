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
        private static readonly String HookLine = "Follow the white rabbit...";

        private static readonly List<MatrixString> MatrixStrings = new List<MatrixString>();
        private const int MaxStreams = 500;

        private const int MinStreamLength = 16;
        private const int MaxStreamLength = 64;

        private const int MinStreamSpeed = 10;
        private const int MaxStreamSpeed = 100;

        private const double ColorChangeProbability = 0.1;
        private const double WhiteTipProbability = 0.8;
        private const double MatrixStringEndProbability = 0.01;

        static void Main(string[] args)
        {
            InitializeConsole();

            while (true)
            {
                if (MatrixStrings.Count < MaxStreams)
                {
                    CreateMatrixString();
                }

                UpdateMatrixStrings();
                RenderMatrixStrings();

                Thread.Sleep(50);

                if (Console.KeyAvailable)
                {
                    Console.Clear();

                    Console.WriteLine(" ");
                    TypeWriterOutput(HookLine);
                    Console.WriteLine(" ");
                    break;
                }
            }

            RestoreConsoleDefaults();

        }

        static void InitializeConsole()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
        }

        static void RestoreConsoleDefaults()
        {
            Console.BackgroundColor = InitialBackgroundColor;
            Console.ForegroundColor = InitialForegroundColor;
            Console.CursorVisible = InitialCursorVisible;
        }

        static void TypeWriterOutput(string message)
        {
            Console.CursorVisible = true;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Encoding asciiEncoding = Encoding.ASCII;

            // ...convert the message to a byte array
            byte[] bytes = asciiEncoding.GetBytes(message);

            foreach (byte b in bytes)
            {
                Thread.Sleep(new Random().Next(50, 500));
                Console.Write((char)b);
            }

            // ...give the console back
            Console.BackgroundColor = InitialBackgroundColor;
            Console.ForegroundColor = InitialForegroundColor;
            Console.CursorVisible = InitialCursorVisible;

            // ...short pause for dramatic effect
            Thread.Sleep(250);

            Console.WriteLine();
        }

        static void CreateMatrixString()
        {
            int column = Random.Next(0, Console.WindowWidth);
            int length = Random.Next(MinStreamLength, MaxStreamLength);
            int speed = Random.Next(MinStreamSpeed, MaxStreamSpeed);
            ConsoleColor color = Colors[Random.Next(0, Colors.Length)];

            MatrixString matrixString = new MatrixString(column, -length, length, speed, color);
            MatrixStrings.Add(matrixString);
        }

        static void UpdateMatrixStrings()
        {
            for (int i = 0; i < MatrixStrings.Count; i++)
            {
                MatrixString matrixString = MatrixStrings[i];

                if (Environment.TickCount - matrixString.LastUpdate > matrixString.Speed)
                {
                    matrixString.DropDown();

                    if (Random.NextDouble() < MatrixStringEndProbability)
                    {
                        matrixString.KillMatrixString();
                    }

                    matrixString.LastUpdate = Environment.TickCount;
                }

                if (matrixString.IsEnded)
                {
                    MatrixStrings.RemoveAt(i);
                    i--;
                }
            }
        }

        static void RenderMatrixStrings()
        {
            foreach (MatrixString matrixString in MatrixStrings)
            {
                Console.ForegroundColor = matrixString.Color;

                for (int i = 0; i < matrixString.Length; i++)
                {
                    int x = matrixString.Column;
                    int y = matrixString.Row - i;

                    if (y >= 0 && y < Console.WindowHeight)
                    {
                        char symbol = Glyphs[Random.Next(0, Glyphs.Length)];

                        if (i == 0 && Random.NextDouble() < WhiteTipProbability)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        Console.SetCursorPosition(x, y);

                        if (i == matrixString.Length - 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }

                        Console.Write(symbol);

                        if (i == 0 && Console.ForegroundColor == ConsoleColor.White)
                        {
                            Console.ForegroundColor = matrixString.Color;
                        }
                    }
                }
            }
        }
    }

    class MatrixString
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Length { get; set; }
        public int Speed { get; set; }
        public ConsoleColor Color { get; set; }
        public int LastUpdate { get; set; }
        public bool IsEnded { get; set; }

        public MatrixString(int column, int row, int length, int speed, ConsoleColor color)
        {
            Column = column;
            Row = row;
            Length = length;
            Speed = speed;
            Color = color;
            LastUpdate = Environment.TickCount;
            IsEnded = false;
        }

        public void DropDown()
        {
            Row++;
        }

        public void KillMatrixString()
        {
            IsEnded = true;
        }
    }
}
