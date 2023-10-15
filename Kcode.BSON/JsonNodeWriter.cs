using KCode.KCode.BSON.Types;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;

namespace KCode.BSON;

public class JsonNodeWriter
{
    public static byte[] SerializeToArray(JsonNode node, JsonNodeWriterOptions? options = null)
    {
        using var ms = new MemoryStream();
        var writer = new JsonNodeWriter(ms, options);
        writer.WriteNode(node);
        return ms.ToArray();
    }

    public static readonly UTF8Encoding _utf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private readonly Stream _stream;
    private readonly JsonNodeWriterOptions _options;

    private JsonNodeWriter(Stream stream, JsonNodeWriterOptions? options)
    {
        _stream = stream;
        _options = options ?? new();
    }

    private void WriteNode(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            WriteObject(obj, _options.AddCrcFieldPlaceholder);
        }
        else
        {
            throw new InvalidOperationException("A top level element in BSON MUST be a document (JsonObject)");
        }
    }

    private void WriteObject(JsonObject obj, bool addCrcFieldPlaceholder = false)
    {
        var beginPosition = _stream.Position;

        const int sizePlaceholder = 0;
        _stream.Write(BitConverter.GetBytes(sizePlaceholder));

        foreach ((string eName, JsonNode? node) in obj)
        {
            Write(eName, node);
        }
        if (addCrcFieldPlaceholder) WriteCrcPlaceholder();
        _stream.WriteByte(BsonConstants.DocEnd);

        var endPosition = _stream.Position;
        _stream.Position = beginPosition;
        int actualSize = (int)(endPosition - beginPosition);
        // Overwrite reserved size value
        _stream.Write(BitConverter.GetBytes(actualSize));
        // Restore stream position
        _stream.Position = endPosition;
    }

    private void WriteCrcPlaceholder()
    {
        _stream.WriteByte((byte)ElementID.Int32);
        WriteAsCString("crc");
        int crcPlaceholder = 0x00;
        _stream.Write(BitConverter.GetBytes(crcPlaceholder));
    }

    private void Write(string eName, JsonNode? node)
    {
        if (node is JsonObject obj)
        {
            _stream.WriteByte((byte)ElementID.Document);
            WriteAsCString(eName);
            WriteObject(obj);
        }
        else if (node is JsonArray arr)
        {
            _stream.WriteByte((byte)ElementID.Array);
            WriteAsCString(eName);
            WriteArray(arr);
        }
        else if (node is JsonValue val)
        {
            WriteValue(eName, val);
        }
        else if (node is null)
        {
            WriteNull(eName);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private void WriteArray(JsonArray arr)
    {
        var beginPosition = _stream.Position;

        const int sizePlaceholder = 0;
        _stream.Write(BitConverter.GetBytes(sizePlaceholder));

        var i = 0;
        foreach (JsonNode? node in arr)
        {
            Write(eName: i.ToString(), node);
            i++;
        }
        _stream.WriteByte(BsonConstants.DocEnd);

        var endPosition = _stream.Position;
        _stream.Position = beginPosition;
        int actualSize = (int)(endPosition - beginPosition);
        // Overwrite reserved size value
        _stream.Write(BitConverter.GetBytes(actualSize));
        // Restore stream position
        _stream.Position = endPosition;
    }

    private void WriteNull(string eName)
    {
        _stream.WriteByte((byte)ElementID.Null);
        WriteAsCString(eName);
    }

    private void WriteValue(string eName, JsonValue val)
    {
        if (val.TryGetValue(out double dValue))
        {
            _stream.WriteByte((byte)ElementID.Double);
            WriteAsCString(eName);
            _stream.Write(BitConverter.GetBytes(dValue));
            return;
        }
        if (val.TryGetValue(out int intValue))
        {
            _stream.WriteByte((byte)ElementID.Int32);
            WriteAsCString(eName);
            _stream.Write(BitConverter.GetBytes(intValue));
            return;
        }

        throw new NotImplementedException();
    }

    private void SerializeElement(string eName, JsonElement el)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Null:
                WriteNull(eName);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void WriteAsCString(string value)
    {
        var valueBytes = _utf8.GetBytes(value);
        if (_options.Validate && valueBytes.Contains(BsonConstants.CStringEnd)) throw new ArgumentException("BSON CString must not contain 0x00 bytes");
        _stream.Write(valueBytes);
        _stream.WriteByte(BsonConstants.CStringEnd);
    }
}
