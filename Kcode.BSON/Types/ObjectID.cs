namespace KCode.BSON.Types
{
    /// <param name="TimestampS"></param>
    /// <param name="RandID">5 byte</param>
    /// <param name="Counter">3 byte</param>
    /// <remarks>BSON is little endian. ObjectID is big endian.</remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/"/>
    public record ObjectID(uint TimestampS, byte[] RandID, uint Counter)
    {
        public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeSeconds(TimestampS);
    }
}
