namespace KCode.BSON;

public readonly struct JsonNodeWriterOptions
{
    public bool Validate { get; init; } = true;

    /// <summary>Include a top document int32 'crc' field with 0-value which can be replaced by custom checksum generation (e.g. with a CRC32 value)</summary>
    /// <remarks>The impl is not included in this library to have minimal dependencies</remarks>
    public bool AddCrcFieldPlaceholder { get; init; }

    public JsonNodeWriterOptions()
    {
    }
}
