using System.Text.Json.Nodes;
using Xunit;

namespace KCode.BSON.Tests;

public class JsonNodeWriterTest
{
    [Fact]
    public void EmptyDocument()
    {
        var empty = JsonNode.Parse(@"{}");
        var result = JsonNodeWriter.SerializeToArray(empty!);

        var expected = new byte[] {
            // int32 doc size - 5 byte
            0x05, 0x00, 0x00, 0x00,
            // doc end
            0x00,
        };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EmptyDocument_WithCrcPlaceholder()
    {
        var empty = JsonNode.Parse(@"{}");
        var result = JsonNodeWriter.SerializeToArray(empty!, new() { AddCrcFieldPlaceholder = true, });

        var expected = new byte[] {
            // int32 doc size - 5 + 9 = 14
            0x0E, 0x00, 0x00, 0x00,
            0x10,  0x63, 0x72, 0x63, 0x00,  0x00, 0x00, 0x00, 0x00,
            // doc end
            0x00,
        };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void OneDouble()
    {
        var empty = JsonNode.Parse(@"{""a"":1.2}");
        var result = JsonNodeWriter.SerializeToArray(empty!);

        var expected = new byte[] {
            // int32 doc size
            0x10, 0x00, 0x00, 0x00,
            0x01,
            // eName 'a' as cstring
            (byte)'a', 0x00,
            // double 1.2
            0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0x3F,
            // doc end
            0x00,
        };
        Assert.Equal(expected, result);
    }
}
