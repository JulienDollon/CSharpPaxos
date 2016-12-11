using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Log;

namespace CSharpPaxosRuntime.Roles
{
    public class Acceptor : IPaxosActor, IExecute
    {
        private ILogger logger;
        private IMessageReceiver receiver;

        public Acceptor(ILogger logger, IMessageReceiver receiver)
        {
            this.logger = logger;
            this.receiver = receiver;
        }

        IMessageReceiver IPaxosActor.Receiver
        {
            get
            {
                return this.receiver;
            }
        }

        void IExecute.Execute()
        {
            throw new NotImplementedException();
        }
    }
}