using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Board board = new Board(10, 10, 1);
            board.SaveToFile("test.txt");
            board.LoadFromFile("test.txt");
            Assert.AreEqual(10, board.Columns);
            Assert.AreEqual(10, board.Rows);

        }
        [TestMethod]
        public void TestMethod2()
        {
            var board = new Board(10, 10, 1);
            board.Cells[2, 2].IsAlive = true;
            board.Cells[3, 2].IsAlive = true;
            board.Cells[2, 3].IsAlive = true;
            board.Cells[3, 3].IsAlive = true;
            var figure = new List<List<bool>>
            {
                new List<bool> { true, true },
                new List<bool> { true, true }
            };
            int matchCount = FigureCounter.CountFigureMatches(board, figure);
            Assert.AreEqual(1, matchCount);
        }
        [TestMethod]
        public void TestMethod3()
        {
            var board = new Board(6, 6, 1);
            board.Cells[2, 1].IsAlive = true;
            board.Cells[3, 1].IsAlive = true;
            board.Cells[1, 2].IsAlive = true;
            board.Cells[2, 2].IsAlive = true;
            var figure = new List<List<bool>>
    {
        new List<bool> { true, true },
        new List<bool> { true, false }
    };
            int matches = FigureCounter.CountFigureMatches(board, figure);
            Assert.AreEqual(1, matches);
        }
        [TestMethod]
        public void TestMethod4()
        {
            var board = new Board(10, 10, 1);
            board.Cells[2, 2].IsAlive = true;
            board.Cells[3, 2].IsAlive = true;
            board.Cells[2, 3].IsAlive = true;
            board.Cells[3, 3].IsAlive = true;
            var originalBoardState = board.Cells.Cast<Cell>().Select(cell => cell.IsAlive).ToList();
            var figure = new List<List<bool>>
            {
                new List<bool> { false, true, false },
                new List<bool> { true, false, true },
                new List<bool> { false, true, false }
            };
            FigureCounter.CountFigureMatches(board, figure);
            var currentBoardState = board.Cells.Cast<Cell>().Select(cell => cell.IsAlive).ToList();
            CollectionAssert.AreEqual(originalBoardState, currentBoardState);
        }
        [TestMethod]
        public void TestMethod5()
        {
            var board = new Board(10, 10, 1);
            var figure = new List<List<bool>>
            {
                new List<bool> { true, true },
                new List<bool> { true, true }
            };
            int matchCount = FigureCounter.CountFigureMatches(board, figure);
            Assert.AreEqual(0, matchCount);
        }
    }
}
