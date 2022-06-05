namespace KCode.BSON;

public class BsonException : Exception
{
    public BsonException(string message, Exception? exception = null) : base(message, exception)
    {
    }
}
