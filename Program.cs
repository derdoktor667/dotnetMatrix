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
        private static readonly string HookLine = "Follow the white rabbit...";
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
            // Stellen Sie sicher, dass die Konsole eine geeignete Schriftart verwendet
            Console.OutputEncoding = Encoding.UTF8;

            Console.Clear();
            InitializeConsole();

            while (true)
            {
                // Erzeuge neue Matrix-Strings bis das Maximum erreicht ist
                if (MatrixStrings.Count < MaxStreams)
                    CreateMatrixString();

                // Aktualisiere die Position und Status der Matrix-Strings
                UpdateMatrixStrings();
                
                // Render die Matrix-Strings auf der Konsole
                RenderMatrixStrings();

                // Warte 50ms
                Thread.Sleep(50);

                // Beende das Programm, wenn eine Taste gedrückt wird
                if (Console.KeyAvailable)
                    break;
            }

            // Zeige die Nachricht im Typewriter-Stil an
            TypeWriterOutput(HookLine);

            // Stelle die ursprünglichen Konsoleneinstellungen wieder her
            RestoreConsoleDefaults();

            // Eine Leerzeile für das Auge
            Console.WriteLine(" ");

            // Verschiebe den Cursor nach links und lösche das Zeichen
            Console.Write("\x1B[1D"); // Move the cursor one unit to the left
            Console.Write("\x1B[1P"); // Delete the character
        }

        // Initialisiert die Konsoleinstellungen für den Matrix-Effekt
        static void InitializeConsole()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
        }

        // Stellt die ursprünglichen Konsoleneinstellungen wieder her
        static void RestoreConsoleDefaults()
        {
            Console.BackgroundColor = InitialBackgroundColor;
            Console.ForegroundColor = InitialForegroundColor;
            Console.CursorVisible = InitialCursorVisible;
        }

        // Zeigt eine Nachricht im Typewriter-Stil an
        static void TypeWriterOutput(string message)
        {
            Console.Clear();
            InitializeConsole();
            Console.WriteLine(" ");

            // Schreibe Zeichen für Zeichen mit einer zufälligen Verzögerung
            foreach (char c in message)
            {
                Thread.Sleep(Random.Next(50, 500));
                Console.Write(c);
            }

            // Kurze Pause für dramatische Wirkung
            Thread.Sleep(250);
            Console.WriteLine(" ");

            RestoreConsoleDefaults();
        }

        // Erstellt einen neuen Matrix-String
        static void CreateMatrixString()
        {
            int column = Random.Next(Console.WindowWidth);
            int length = Random.Next(MinStreamLength, MaxStreamLength);
            int speed = Random.Next(MinStreamSpeed, MaxStreamSpeed);
            ConsoleColor color = Colors[Random.Next(Colors.Length)];

            MatrixStrings.Add(new MatrixString(column, -length, length, speed, color));
        }

        // Aktualisiert die Matrix-Strings
        static void UpdateMatrixStrings()
        {
            for (int i = 0; i < MatrixStrings.Count; i++)
            {
                MatrixString matrixString = MatrixStrings[i];

                // Überprüfe, ob es Zeit ist, den Matrix-String zu aktualisieren
                if (Environment.TickCount - matrixString.LastUpdate > matrixString.Speed)
                {
                    matrixString.DropDown();

                    // Beende den Matrix-String zufällig
                    if (Random.NextDouble() < MatrixStringEndProbability)
                        matrixString.KillMatrixString();

                    matrixString.LastUpdate = (uint)Environment.TickCount;
                }

                // Entferne beendete Matrix-Strings
                if (matrixString.IsEnded)
                {
                    MatrixStrings.RemoveAt(i);
                    i--;
                }
            }
        }

        // Rendert die Matrix-Strings auf der Konsole
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

        // Bewegt den Matrix-String um eine Zeile nach unten
        public void DropDown()
        {
            Row++;
        }

        // Markiert den Matrix-String als beendet
        public void KillMatrixString()
        {
            IsEnded = true;
        }
    }
}
