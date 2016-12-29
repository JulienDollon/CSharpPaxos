using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.Factories;
using CSharpPaxosRuntime.Roles.Leader;
using CSharpPaxosRuntime.Roles.Replica;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using log4net;

namespace CSharpPaxosRuntime.SampleApp
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));
        private static List<Replica> replicas = new List<Replica>();
        private static List<Acceptor> acceptors = new List<Acceptor>();
        private static List<Leader> leaders = new List<Leader>();
        private static MovingRobotIntoMatrixStateMachine stateMachine;
        private static ObjectsMessageBroker messageBroker;

        static void Main(string[] args)
        {
            initializeStateMachine();
            initializeMessageBroker();
            initializePaxos();
            startPaxos();
            readUserInput();
        }

        /// <summary>
        /// Initialize what the state machine should be
        /// This state machine will be the one replicated accross all replicas
        /// </summary>
        static void initializeStateMachine()
        {
            stateMachine = new MovingRobotIntoMatrixStateMachine();
        }

        /// <summary>
        /// If you want to send message over a real network, you might want to mockup this broker
        /// </summary>
        static void initializeMessageBroker()
        {
            messageBroker = new ObjectsMessageBroker();
        }

        /// <summary>
        /// The goal is to create/instantiate all required roles for Paxos
        /// If N is an even number greater or equal to 2
        /// # of replicas = N + 1
        /// # of leaders = N + 1
        /// # of acceptors = 2 * N + 1
        /// </summary>
        static void initializePaxos()
        {
            AcceptorFactory acceptorFactory = new AcceptorFactory();
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));

            ReplicaFactory replicaFactory = new ReplicaFactory();
            replicas.Add(replicaFactory.BuildInstance(messageBroker, stateMachine));
            replicas.Add(replicaFactory.BuildInstance(messageBroker, stateMachine));
            replicas.Add(replicaFactory.BuildInstance(messageBroker, stateMachine));

            LeaderFactory leaderFactory = new LeaderFactory();
            leaders.Add(leaderFactory.BuildInstance(messageBroker, acceptors, replicas));
            leaders.Add(leaderFactory.BuildInstance(messageBroker, acceptors, replicas));
            leaders.Add(leaderFactory.BuildInstance(messageBroker, acceptors, replicas));
        }

        /// <summary>
        /// Start all instances of Paxos Roles
        /// You might want to run them on separate nodes/hosts
        /// </summary>
        static void startPaxos()
        {
            ThreadPool.SetMaxThreads(20, 20);

            foreach (Acceptor acceptor in acceptors)
            {
                ThreadPool.QueueUserWorkItem(state => { acceptor.Start(); });
            }
            foreach (Leader leader in leaders)
            {
                ThreadPool.QueueUserWorkItem(state => { leader.Start(); });
            }
            ReplicaFactory replicaFactory = new ReplicaFactory();
            foreach (Replica replica in replicas)
            {
                ThreadPool.QueueUserWorkItem(state => { replicaFactory.StartInstance(replica, leaders);  });
            }
        }

        static void readUserInput()
        {
            while (true)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey();

                if (pressedKey.Key == ConsoleKey.Spacebar)
                {
                    Console.Clear();
                    foreach (Replica replica in replicas)
                    {
                        MovingRobotIntoMatrixStateMachine.DisplayMatrix(replica.RoleState.MessageSender.UniqueId,
                            (replica.RoleState as ReplicaState).StateMachine as MovingRobotIntoMatrixStateMachine);
                    }
                }
                //We will use two different client/threads to simulate simultaneous command send
                //use LEFT/RIGHT/UP/DOWN for thread 2, and "1234" for thread 1
                else if (pressedKey.KeyChar >= '1' && pressedKey.KeyChar <= '4')
                {
                    RobotMove move = RobotMove.Down;
                    if (pressedKey.KeyChar == '1')
                    {
                        move = RobotMove.Left;
                    }
                    else if (pressedKey.KeyChar == '2')
                    {
                        move = RobotMove.Up;
                    }
                    else if (pressedKey.KeyChar == '3')
                    {
                        move = RobotMove.Right;
                    }

                    MoveRobotCommand command = new MoveRobotCommand();
                    command.Move = move;
                    Thread thread = new Thread(() =>
                    {
                        sendPaxosRequest(command, 0);
                    });
                    thread.Start();
                }
                else
                {
                    RobotMove move = RobotMove.Down;
                    if (pressedKey.Key == ConsoleKey.LeftArrow)
                    {
                        move = RobotMove.Left;
                    }
                    else if (pressedKey.Key == ConsoleKey.UpArrow)
                    {
                        move = RobotMove.Up;
                    }
                    else if (pressedKey.Key == ConsoleKey.RightArrow)
                    {
                        move = RobotMove.Right;
                    }

                    MoveRobotCommand command = new MoveRobotCommand();
                    command.Move = move;
                    Thread thread = new Thread(() =>
                    {
                        sendPaxosRequest(command, 1);
                    });
                    thread.Start();
                }
            }
        }

        private static void sendPaxosRequest(MoveRobotCommand command, int threadId)
        {
            logger.Info($"Sending move:{command.Move.ToString()}");
            ClientRequest request = new ClientRequest();
            request.Command = command;
            messageBroker.SendMessage(replicas[threadId].RoleState.MessageSender.UniqueId, request);
        }
    }
}