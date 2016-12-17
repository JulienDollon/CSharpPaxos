using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Models
{
    public class BallotNumber
    {
        protected bool Equals(BallotNumber other)
        {
            return Value == other.Value;
        }

        public int Value { get; set; }

        public static bool operator >(BallotNumber c1, BallotNumber c2)
        {
            return c1.Value > c2.Value;
        }

        public static bool operator <(BallotNumber c1, BallotNumber c2)
        {
            return c1.Value < c2.Value;
        }

        public static bool operator ==(BallotNumber c1, BallotNumber c2)
        {
            return c1.Value == c2.Value;
        }

        public static bool operator !=(BallotNumber c1, BallotNumber c2)
        {
            return c1.Value == c2.Value;
        }

        public static BallotNumber GenerateBallotNumber(int round, int leaderUniqueId)
        {
            return new BallotNumber()
                {
                    Value = int.Parse(($"{round}{leaderUniqueId}"))
                };
        }

        public static BallotNumber Empty()
        {
            return new BallotNumber()
            {
                Value = 0
            };
        }
    }
}