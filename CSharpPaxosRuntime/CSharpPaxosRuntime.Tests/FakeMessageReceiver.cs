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
        public event EventHandler OnMessageReceived;

        private IMessage message;
        public void ReceiveMessage(IMessage message)
        {
            this.message = message;
            OnMessageReceived?.Invoke(null, null);
        }

        public IMessage GetLastMessage()
        {
            return message;
        }
    }
}