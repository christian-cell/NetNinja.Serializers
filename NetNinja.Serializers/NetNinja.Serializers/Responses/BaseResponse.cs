namespace NetNinja.Serializers.Responses
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public List<Message> Messages { get; set; }
        public Guid? ItemId { get; set; }
    }
};