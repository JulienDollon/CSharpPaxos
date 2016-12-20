namespace CSharpPaxosRuntime.Messaging.Bus
{
    public class MessageSender
    {
        public override bool Equals(object obj)
        {
            return UniqueId == (obj as MessageSender).UniqueId;
        }

        public string UniqueId { get; set; }
    }
}