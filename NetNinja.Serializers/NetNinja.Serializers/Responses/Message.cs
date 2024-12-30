namespace NetNinja.Serializers.Responses
{
    public class Message
    {
        public enum MessageLevel
        {
            Info,
            Warning,
            Error,
        }

        public MessageLevel Level { get; set; }
        public string Body { get; set; }

        public Message(string body, MessageLevel level = MessageLevel.Info)
        {
            Body = body;
            Level = level;
        }
    }
};