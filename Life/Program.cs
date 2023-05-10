using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
 
namespace cli_life
{
    public static class FigureCounter
    {
        public static int CountFigureMatches(Board board, List<List<bool>> figure)
        {
            int matches = 0;
            int figureWidth = figure[0].Count;
            int figureHeight = figure.Count;
 
            for (int row = 0; row <= board.Rows - figureHeight; row++)
            {
                for (int col = 0; col <= board.Columns - figureWidth; col++)
                {
                    if (IsFigureMatch(board, figure, row, col))
                    {
                        matches++;
                        HighlightFigure(board, figure, row, col);
                    }
                }
            }
 
            return matches;
        }
 
        private static bool IsFigureMatch(Board board, List<List<bool>> figure, int startRow, int startCol)
        {
            for (int row = 0; row < figure.Count; row++)
            {
                for (int col = 0; col < figure[row].Count; col++)
                {
                    bool figureCell = figure[row][col];
                    bool boardCell = board.Cells[startCol + col, startRow + row].IsAlive;
 
                    if (figureCell != boardCell)
                        return false;
                }
            }
 
            return true;
        }
 
        private static void HighlightFigure(Board board, List<List<bool>> figure, int startRow, int startCol)
        {
            for (int row = 0; row < figure.Count; row++)
            {
                for (int col = 0; col < figure[row].Count; col++)
                {
                    if (figure[row][col])
                        board.Cells[startCol + col, startRow + row].IsAlive = false; // Change cell state for highlighting
                }
            }
        }
    }
    public class GameSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
 
        public static GameSettings FromJson(string json)
        {
            return JsonConvert.DeserializeObject<GameSettings>(json);
        }
 
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
        public int CountLiveNeighbors()
        {
            return neighbors.Count(x => x.IsAlive);
        }
        public bool IsSymmetric()
        {
            foreach (var neighbor in neighbors)
            {
                if (neighbor.IsAlive != IsAlive)
                    return false;
            }
            return true;
        }
 
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
 
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
 
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
 
            ConnectNeighbors();
            Randomize(liveDensity);
        }
        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }
        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            if (IsStable())
            {
                Console.WriteLine("Стабильная фаза достигнута.");
            }
            foreach (var cell in Cells)
                cell.Advance();
 
            
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;
 
                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;
 
                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
        public void SaveToFile(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        var cell = Cells[col, row];
                        writer.Write(cell.IsAlive ? '*' : ' ');
                    }
                    writer.WriteLine();
                }
            }
        }
        public void LoadFromFile(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                int row = 0;
                while ((line = reader.ReadLine()) != null && row < Rows)
                {
                    for (int col = 0; col < Columns && col < line.Length; col++)
                    {
                        char cellState = line[col];
                        Cells[col, row].IsAlive = (cellState == '*');
                    }
                    row++;
                }
            }
        }
        public void LoadFromFile1(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            int startY = (Rows - lines.Length) / 2;
            int startX = (Columns - lines[0].Length) / 2;
 
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '*')
                    {
                        Cells[startX + x, startY + y].IsAlive = true;
                    }
                }
            }
        }
        public bool IsStable()
        {
            foreach (var cell in Cells)
            {
                if (cell.IsAlive != cell.IsAliveNext)
                {
                    return false;
                }
            }
            return true;
        }
        public int CountSymmetricCells()
        {
            int count = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive && cell.IsSymmetric())
                    count++;
            }
            return count;
        }
    }
    class Program
    {
 
        static Board board;
        static GameSettings settings;
        static private void Reset()
        {
            string json = File.ReadAllText("settings.json");
            settings = GameSettings.FromJson(json);
 
            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);
        }
        static void Render()
        {
            int liveCells = 0;
            int cellsWithLiveNeighbors = 0;
 
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                        liveCells++;
                    }
                    else
                    {
                        Console.Write(' ');
                    }
 
                    if (cell.IsAlive && cell.CountLiveNeighbors() > 0)
                    {
                        cellsWithLiveNeighbors++;
                    }
                }
                Console.Write('\n');
            }
 
            Console.WriteLine("Количество живых клеток: " + liveCells);
            Console.WriteLine("Количество клеток с живыми соседями: " + cellsWithLiveNeighbors);
 
        }
 
        static void Main(string[] args)
        {
            Reset();
            board.LoadFromFile1("glider.txt");
            while (true)
            {
                Console.Clear();
                Render();
                board.Advance();
                int symmetricCellCount = board.CountSymmetricCells();
                Console.WriteLine("Симметричные клетки: " + symmetricCellCount);
                List<List<bool>> figure = new List<List<bool>>
                {
                    new List<bool> { false, true, false },
                    new List<bool> { true, false, true },
                    new List<bool> { false, true, false }
                };
                int matchCount = FigureCounter.CountFigureMatches(board, figure);
                Console.WriteLine("Количество совпадений с коробкой: " + matchCount);
 
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.S)
                    {
                        Console.WriteLine("Введите имя файла для сохранения: ");
                        string filename = Console.ReadLine();
                        board.SaveToFile(filename);
                    }
                    else if (key == ConsoleKey.L)
                    {
                        Console.WriteLine("Введите имя файла для загрузки: ");
                        string filename = Console.ReadLine();
                        board.LoadFromFile(filename);
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}