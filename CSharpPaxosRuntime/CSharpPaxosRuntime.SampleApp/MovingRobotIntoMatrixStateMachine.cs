using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.SampleApp
{

    class MovingRobotIntoMatrixStateMachine : IStateMachine
    {
        public MovingRobotIntoMatrixStateMachine()
        {
            Matrix = new byte[10][];
            for (int i = 0; i < 10; i++)
            {
                Matrix[i] = new byte[10];
            }
            defineInitialRobotPosition();
        }

        private void defineInitialRobotPosition()
        {
            Matrix[0][0] = 1;
            currentRobotPosition = Point.Empty;
        }

        private void updatePosition(Point point)
        {
            if (point.X < 0 || point.Y < 0 || point.X >= Matrix.Length || point.Y >= Matrix[0].Length)
            {
                return;
            }

            Matrix[currentRobotPosition.X][currentRobotPosition.Y] = 0;
            Matrix[point.X][point.Y] = 1;
            currentRobotPosition = point;
        }

        private Point currentRobotPosition;

        public byte[][] Matrix { get; set; }

        public void Update(ICommand command)
        {
            MoveRobotCommand move = (command as MoveRobotCommand);
            if (move.Move == RobotMove.Left)
            {
                updatePosition(new Point(currentRobotPosition.X, currentRobotPosition.Y - 1));
            }
            else if (move.Move == RobotMove.Down)
            {
                updatePosition(new Point(currentRobotPosition.X + 1, currentRobotPosition.Y));
            }
            else if (move.Move == RobotMove.Up)
            {
                updatePosition(new Point(currentRobotPosition.X - 1, currentRobotPosition.Y));
            }
            else if (move.Move == RobotMove.Right)
            {
                updatePosition(new Point(currentRobotPosition.X, currentRobotPosition.Y + 1));
            }
        }

        public static void DisplayMatrix(string replicaId, MovingRobotIntoMatrixStateMachine stateMachine)
        {
            Console.WriteLine($"Replica ID:{replicaId} has in memory this statemachine:");
            for (int i = 0; i < stateMachine.Matrix.Length; i++)
            {
                for (int n = 0; n < stateMachine.Matrix[i].Length; n++)
                {
                    Console.Write("|");
                    Console.Write(stateMachine.Matrix[i][n]);
                    Console.Write("|");
                }
                Console.WriteLine();
            }
        }
    }
}