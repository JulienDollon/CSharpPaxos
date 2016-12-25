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
            if (other == null)
            {
                return false;
            }
            return Value == other.Value;
        }

        public int Value => int.Parse(($"{round}{leaderUniqueId}"));

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
            if (object.ReferenceEquals(c1, null))
            {
                return object.ReferenceEquals(c2, null);
            }

            return c1.Equals(c2);
        }

        public static bool operator !=(BallotNumber c1, BallotNumber c2)
        {
            return !(c1 == c2);
        }

        public static BallotNumber GenerateBallotNumber(int round, int leaderUniqueId)
        {
            return new BallotNumber()
                {
                    round = round,
                    leaderUniqueId = leaderUniqueId
                };
        }

        private int round;
        private int leaderUniqueId;

        public static BallotNumber Empty()
        {
            return new BallotNumber()
            {
                round = 0,
                leaderUniqueId = 0
            };
        }

        public BallotNumber Increment()
        {
            this.round++;
            return this;
        }

        public BallotNumber Decrement()
        {
            this.round--;
            return this;
        }
    }
}