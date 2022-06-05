namespace KCode.KCode.BSON.Types;

/// <seealso href="https://bsonspec.org/spec.html"/>
public enum ElementID
{
    Double = 0x01,
    StringInt32UTF8 = 0x02,
    Document = 0x03,
    Array = 0x04,
    Binary = 0x05,
    DeprecatedUndefined = 0x06,
    ObjectID12B = 0x07,
    Boolean = 0x08,
    DateTimeUTCInt64 = 0x09,
    Null = 0x0A,
    Regex = 0x0B,
    DeprecatedDBPointer = 0x0C,
    JSCode = 0x0D,
    DeprecatedSymbol = 0x0E,
    DeprecatedJSCodeWS = 0x0F,
    Int32 = 0x10,
    TimestampUInt64 = 0x11,
    Int64 = 0x12,
    decimal128 = 0x13,
    MinKey = 0xFF,
    MaxKey = 0x7F,
}
