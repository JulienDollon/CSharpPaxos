using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Roles.Acceptor
{
    public class AcceptorState : IPaxosRoleState, IBallotNumberProperty
    {
        public BallotNumber BallotNumber { get; set; }

        public MessageSender MessageSender { get; set; }

        public IDecision AcceptedDecision { get; set; }
    }
}