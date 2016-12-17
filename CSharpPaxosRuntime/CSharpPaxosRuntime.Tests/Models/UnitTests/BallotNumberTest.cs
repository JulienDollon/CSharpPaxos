using System;
using CSharpPaxosRuntime.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests.Models.UnitTests
{
    [TestClass]
    public class BallotNumberTest
    {
        [TestMethod]
        public void ValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 500);
            Assert.AreEqual(number.Value, 10500);
        }

        [TestMethod]
        public void UnValidBallot()
        {
            try
            {
                BallotNumber number = BallotNumber.GenerateBallotNumber(10, Int32.MaxValue);
                Assert.Fail();
            }
            catch (OverflowException e)
            {
            }
        }

        [TestMethod]
        public void Operators()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 500);
            BallotNumber number2 = BallotNumber.GenerateBallotNumber(10, 500);
            BallotNumber number3 = BallotNumber.GenerateBallotNumber(10, 501);

            Assert.IsTrue(number == number2);
            Assert.IsTrue(number3 > number2);
            Assert.IsTrue(number2 < number3);
        }
    }
}