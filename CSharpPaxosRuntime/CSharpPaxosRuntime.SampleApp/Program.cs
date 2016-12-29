using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.Factories;
using CSharpPaxosRuntime.Roles.Leader;
using CSharpPaxosRuntime.Roles.Replica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.SampleApp
{
    class Program
    {
        private static List<Replica> replicas;
        private static List<Acceptor> acceptors;
        private static List<Leader> leaders;
        private static UniqueIntegerStateMachine stateMachine;
        private static ObjectsMessageBroker messageBroker;

        static void Main(string[] args)
        {
            initializeStateMachine();
            initializeMessageBroker();
            initializePaxos();
            startPaxos();
            readUserInput();
        }

        static void initializeStateMachine()
        {
            stateMachine = new UniqueIntegerStateMachine();
        }

        static void initializeMessageBroker()
        {
            messageBroker = new ObjectsMessageBroker();
        }


        static void initializePaxos()
        {
            AcceptorFactory acceptorFactory = new AcceptorFactory();
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            acceptors.Add(acceptorFactory.BuildInstance(messageBroker));
            //3 replica, 3 leaders, 5 acceptors

        }

        static void startPaxos()
        {

        }

        static void readUserInput()
        {

        }
    }

    class AdditionCommand : ICommand
    {
        public int ValueToAddToStateMachine { get; set; }
    }

    class UniqueIntegerStateMachine : IStateMachine
    {
        public UniqueIntegerStateMachine()
        {
            Value = 0;
        }

        public int Value { get; set; }

        public void Update(ICommand command)
        {
            Value += (command as AdditionCommand).ValueToAddToStateMachine;
        }
    }
}