using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace KCode.BSON;

public static class BsonSerializer
{
    /// <summary>Deserialize one document.</summary>
    /// <remarks>A BSON file or stream may contain more than one document.</remarks>
    /// <exception cref="BsonException"></exception>
    public static JsonObject DeserializeDocument(byte[] bson) => DeserializeDocument(new MemoryStream(bson));

    /// <exception cref="BsonException"></exception>
    public static JsonObject DeserializeDocument(Stream bson)
    {
        if (!BitConverter.IsLittleEndian) throw new NotImplementedException("BSON is little endian. Runtime arch is not. Conversion necessary.");

        using var r = new BinaryReader(bson);

        return ReadDocument(r);
    }

    public static ImmutableArray<JsonObject> DeserializeDocuments(byte[] bson) => DeserializeDocuments(new MemoryStream(bson));

    public static ImmutableArray<JsonObject> DeserializeDocuments(Stream bson)
    {
        if (!BitConverter.IsLittleEndian) throw new NotImplementedException("BSON is little endian. Runtime arch is not. Conversion necessary.");

        using var r = new BinaryReader(bson);

        return ReadDocuments(r);
    }

    /// <exception cref="BsonException"></exception>
    public static JsonObject ReadDocument(BinaryReader reader)
    {
        return BsonReader.ReadDocument(reader);
    }

    /// <exception cref="BsonException"></exception>
    public static ImmutableArray<JsonObject> ReadDocuments(BinaryReader reader)
    {
        var l = new List<JsonObject>();

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            l.Add(ReadDocument(reader));
        }

        return l.ToImmutableArray();
    }
}
