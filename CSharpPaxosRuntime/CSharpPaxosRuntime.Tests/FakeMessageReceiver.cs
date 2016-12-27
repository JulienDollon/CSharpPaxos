using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Tests
{
    public class FakeMessageReceiver : IMessageReceiver
    {
        public event EventHandler<IMessage> OnMessageReceived;

        private IMessage message;
        public void ReceiveMessage(IMessage msg)
        {
            this.message = msg;
            OnMessageReceived?.Invoke(null, msg);
        }

        public IMessage GetLastMessage()
        {
            return message;
        }
    }
}