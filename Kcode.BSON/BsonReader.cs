using System.Text;
using System.Text.Json.Nodes;
using KCode.KCode.BSON.Types;

namespace KCode.BSON;

public static class BsonReader
{
    public static JsonObject ReadDocument(BinaryReader r)
    {
        if (r.BaseStream.Length < 5) throw new BsonException("Smaller than a minimum BSON document");

        var docLenB = r.ReadInt32();
        if (docLenB < 5) throw new BsonException($"Invalid BSON data. Len must be bigger than minimum 5 but is {docLenB}");

        var payloadLenB = docLenB - 5;
        var payload = r.ReadBytes(payloadLenB);
        if (r.ReadByte() != 0x00) throw new BsonException("Missing document end 0x00");

        var s2 = new MemoryStream(payload);
        var r2 = new BinaryReader(s2);

        var j = new JsonObject();
        foreach (var el in ReadElements(r2))
        {
            j.Add(el.Key, el.Value);
        }

        return j;
    }

    public static JsonArray ReadArray(BinaryReader r)
    {
        if (r.BaseStream.Length < 5) throw new BsonException("Smaller than a minimum BSON document");

        var docLenB = r.ReadInt32();
        if (docLenB < 5) throw new BsonException($"Invalid BSON data. Len must be bigger than minimum 5 but is {docLenB}");

        var payloadLenB = docLenB - 5;
        var payload = r.ReadBytes(payloadLenB);
        if (r.ReadByte() != 0x00) throw new BsonException("Missing document end 0x00");

        var s2 = new MemoryStream(payload);
        var r2 = new BinaryReader(s2);

        var j = new JsonArray();
        foreach (var el in ReadElements(r2))
        {
            j.Add(el.Value);
        }

        return j;
    }

    public static IEnumerable<KeyValuePair<string, JsonNode?>> ReadElements(BinaryReader r2)
    {
        while (true)
        {
            if (r2.BaseStream.Position == r2.BaseStream.Length) break;

            var el = ParseElement(r2);
            if (el == null)
            {
                if (r2.BaseStream.Position != r2.BaseStream.Length) throw new InvalidOperationException($"{r2.BaseStream.Position} != {r2.BaseStream.Length}");
                continue;
            }

            yield return el.Value;
        }
    }

    public static KeyValuePair<string, JsonNode?>? ParseElement(BinaryReader r)
    {
        var typeValue = r.ReadByte();
        if (typeValue == 0x00) return null;

        var type = (ElementID)typeValue;

        var key = ReadCString(r);

        JsonNode? value = type switch
        {
            ElementID.Double => JsonValue.Create(r.ReadDouble()),
            ElementID.StringInt32UTF8 => JsonValue.Create(ReadString(r)),
            ElementID.Document => ReadDocument(r),
            ElementID.Array => ReadArray(r),
            ElementID.Binary => throw new NotImplementedException("bin"),
            ElementID.DeprecatedUndefined => throw new NotImplementedException("undefined"),
            ElementID.ObjectID12B => JsonValue.Create(ReadObjectID(r)),
            ElementID.Boolean => JsonValue.Create(r.ReadByte() switch { 0x00 => false, 0x01 => true, _ => throw new FormatException("Invalid bool value") }),
            ElementID.DateTimeUTCInt64 => throw new NotImplementedException("dt"),
            ElementID.Null => null,
            ElementID.Regex => throw new NotImplementedException("regex"),
            ElementID.DeprecatedDBPointer => throw new NotImplementedException(),
            ElementID.JSCode => throw new NotImplementedException("js code"),
            ElementID.DeprecatedSymbol => throw new NotImplementedException("symbol"),
            ElementID.DeprecatedJSCodeWS => throw new NotImplementedException("js code WS"),
            ElementID.Int32 => JsonValue.Create(r.ReadInt32()),
            ElementID.TimestampUInt64 => throw new NotImplementedException("ts"),
            ElementID.Int64 => JsonValue.Create(r.ReadInt64()),
            ElementID.decimal128 => throw new NotImplementedException("dec 128"),
            ElementID.MinKey => throw new NotImplementedException("min key"),
            ElementID.MaxKey => throw new NotImplementedException("max key"),
            _ => throw new NotImplementedException(),
        };
        return new(key, value);
    }

    /// <remarks>12 bytes. 4 byte uint timestampS since epoch, 5 byte rand ID, 3 byte counter.</remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/"/>
    public static string? ReadObjectID(BinaryReader r)
    {
        var values = string.Join("", r.ReadBytes(12).Select(x => x.ToString("X")));
        return $@"ObjectId('{values}')";
    }

    public static string ReadCString(BinaryReader r)
    {
        var bytes = new List<byte>();
        byte b;
        while ((b = r.ReadByte()) != 0x00)
        {
            bytes.Add(b);
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    public static string ReadString(BinaryReader r)
    {
        var lenB = r.ReadInt32();
        var payload = r.ReadBytes(lenB - 1);
        if (r.ReadByte() != 0x00) throw new FormatException("Missing string end");

        return Encoding.UTF8.GetString(payload);
    }
}
