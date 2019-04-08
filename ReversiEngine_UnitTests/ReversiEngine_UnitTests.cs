using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi;

namespace ReversiEngine_UnitTests
{
    [TestClass]
    public class ReversiEngine_UnitTests
    {
        const int boardWidth = 8;
        const int boardHeight = 8;
        const int playerNumberStarting = 1;

        private ReversiEngine CreateDefaultEngine() => new ReversiEngine(playerNumberStarting, boardWidth, boardHeight);

        [TestMethod]
        public void ConstructorTest()
        {
            ReversiEngine engine = CreateDefaultEngine();

            Assert.AreEqual(boardWidth, engine.BoardWidth);
            Assert.AreEqual(boardHeight, engine.BoardHeight);
            Assert.AreEqual(playerNumberStarting, engine.PlayerNumberMakingNextMove);
        }

        [TestMethod]
        public void NumberFieldsTest()
        {
            ReversiEngine engine = CreateDefaultEngine();

            int totalNumberFields = boardWidth * boardHeight;
            Assert.AreEqual(totalNumberFields - 4, engine.NumberOfEmptyFields);
            Assert.AreEqual(2, engine.NumberOfPlayer1Fields);
            Assert.AreEqual(2, engine.NumberOfPlayer2Fields);
        }

        [TestMethod]
        public void DownloadFieldConditionTest()
        {
            ReversiEngine engine = CreateDefaultEngine();

            int conditionField = engine.DownloadFieldCondition(0, 0);
            Assert.AreEqual(0, conditionField);

            conditionField = engine.DownloadFieldCondition(boardWidth - 1, 0);
            Assert.AreEqual(0, conditionField);

            conditionField = engine.DownloadFieldCondition(0, boardHeight - 1);
            Assert.AreEqual(0, conditionField);

            conditionField = engine.DownloadFieldCondition(boardWidth - 1, boardHeight - 1);
            Assert.AreEqual(0, conditionField);

            conditionField = engine.DownloadFieldCondition(boardWidth / 2 - 1, boardHeight / 2 - 1);
            Assert.AreEqual(1, conditionField);

            conditionField = engine.DownloadFieldCondition(boardWidth / 2, boardHeight / 2);
            Assert.AreEqual(1, conditionField);

            conditionField = engine.DownloadFieldCondition(boardWidth / 2 - 1, boardHeight / 2);
            Assert.AreEqual(2, conditionField);

            conditionField = engine.DownloadFieldCondition(boardWidth / 2, boardHeight / 2 - 1);
            Assert.AreEqual(2, conditionField);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DownloadFieldConditionTest_OutOfBoard()
        {
            ReversiEngine engine = CreateDefaultEngine();
            int conditionField = engine.DownloadFieldCondition(-1, -1);
        }

        [TestMethod]
        public void PutStoneTest()
        {
            ReversiEngine engine = CreateDefaultEngine();

            //before move
            int horizontal = 5; int vertical = 3;
            Assert.AreEqual(0, engine.DownloadFieldCondition(horizontal, vertical));
            Assert.AreEqual(2, engine.DownloadFieldCondition(horizontal - 1, vertical));

            //correct move player 1
            bool result = engine.PutStone(horizontal, vertical);
            Assert.IsTrue(result);
            Assert.AreEqual(1, engine.DownloadFieldCondition(horizontal, vertical));
            Assert.AreEqual(1, engine.DownloadFieldCondition(horizontal - 1, vertical));

            int totalNumberFields = boardWidth * boardHeight;
            Assert.AreEqual(totalNumberFields - 5, engine.NumberOfEmptyFields);
            Assert.AreEqual(4, engine.NumberOfPlayer1Fields);
            Assert.AreEqual(1, engine.NumberOfPlayer2Fields);

            //incorrect move player 2
            result = engine.PutStone(horizontal, vertical);
            Assert.IsFalse(result);

            Assert.AreEqual(totalNumberFields - 5, engine.NumberOfEmptyFields);
            Assert.AreEqual(4, engine.NumberOfPlayer1Fields);
            Assert.AreEqual(1, engine.NumberOfPlayer2Fields);

            //correct move player 2
            result = engine.PutStone(horizontal, vertical + 1);
            Assert.IsTrue(result);
            Assert.AreEqual(2, engine.DownloadFieldCondition(horizontal, vertical + 1));
            Assert.AreEqual(2, engine.DownloadFieldCondition(horizontal - 1, vertical + 1));

            Assert.AreEqual(totalNumberFields - 6, engine.NumberOfEmptyFields);
            Assert.AreEqual(3, engine.NumberOfPlayer1Fields);
            Assert.AreEqual(3, engine.NumberOfPlayer2Fields);
        }
    }
}
