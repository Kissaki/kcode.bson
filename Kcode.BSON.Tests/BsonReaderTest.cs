using Xunit;

namespace KCode.BSON.Tests;

public class BsonReaderTest
{
    [Fact]
    public void Test_CString()
    {
        Assert.Equal("", BsonReader.ReadCString(BinR("00")));
    }

    private BinaryReader BinR(string hex)
    {
        return new BinaryReader(new MemoryStream(Bin(hex)));
    }

    private byte[] Bin(string hex)
    {
        var b = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2)
        {
            b[i / 2] = byte.Parse(hex[i..(i + 2)]);
        }
        return b;
    }
}
