﻿using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
{
    public class VoteRequest : IRequestMessage, IBallotNumberProperty, ISlotNumberProperty
    {
        public BallotNumber BallotNumber
        {
            get;
            set;
        }

        public ICommand Command
        {
            get;
            set;
        }

        public MessageSender MessageSender
        {
            get;
            set;
        }

        public int SlotNumber
        {
            get;
            set;
        }
    }
}