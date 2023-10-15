namespace KCode.BSON;

public static class BsonConstants
{
    public const byte BooleanFalse = 0x00;
    public const byte BooleanTrue = 0x01;

    public const byte DocEnd = 0x00;
    public const byte StringEnd = 0x00;
    public const byte CStringEnd = 0x00;

    public enum BinarySubtypeId : byte
    {
        GenericBinary = 0x00,
        Function = 0x01,
        BinaryOld = 0x02,
        UUIDOld = 0x03,
        UUID = 0x04,
        MD5 = 0x05,
        Encrypted = 0x06,
        Compressed = 0x07,

        UserDefined = 0x80,
    }
}
