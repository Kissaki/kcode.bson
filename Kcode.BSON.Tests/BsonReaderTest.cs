using Xunit;

namespace KCode.BSON.Tests;

public class BsonReaderTest
{
    [Fact]
    public void Test_CString()
    {
        Assert.Equal("", BsonReader.ReadCString(BinR("00")));
        Assert.Equal("a", BsonReader.ReadCString(BinR("6100")));
        Assert.Equal("A", BsonReader.ReadCString(BinR("4100")));
        Assert.Equal("1259", BsonReader.ReadCString(BinR("3132353900")));
    }

    private static BinaryReader BinR(string hex)
    {
        return new BinaryReader(new MemoryStream(Bin(hex)));
    }

    private static byte[] Bin(string hex)
    {
        var b = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2)
        {
            b[i / 2] = Convert.ToByte(hex.Substring(i, 2), fromBase: 16);
        }
        return b;
    }
}
