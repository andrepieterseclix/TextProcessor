
namespace TextProcessor.Processors.Data
{
    class EnclosingCharacters
    {
        public EnclosingCharacters(string description, string front, string back = null)
        {
            Front = front;
            Back = back == null ? front : back;
            Description = description;
        }

        public string Front { get; private set; }

        public string Back { get; private set; }

        public string Description { get; private set; }
    }
}
