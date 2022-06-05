using Xunit;

namespace KCode.BSON.Tests;

public class BSONSerializerTest
{
    [Fact]
    public void TestEmpty()
    {
        Assert.Equal("{}", BsonSerializer.DeserializeDocument(new byte[] { 0x05, 0x00, 0x00, 0x00, 0x00, }).ToJsonString());
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations", Justification = "Uniform test case code")]
    public void Test_SmallerMinBSON_Throws()
    {
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x00, 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x00, 0x00, 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x00, 0x00, 0x00, 0x00, }));
    }

    [Fact]
    public void Test_SmallerMinLen_Throws()
    {
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, }));
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00, }));
    }

    [Fact]
    public void Test_NegLen_Throws()
    {
        Assert.Throws<BsonException>(() => BsonSerializer.DeserializeDocument(new byte[] { 0b100, 0x00, 0x00, 0x00, 0x00, }));
    }

    private static IEnumerable<byte> ParseHexBytes(string hexBytes)
    {
        for (var i = 0; i < hexBytes.Length; i += 2)
        {
            yield return Convert.ToByte(hexBytes.Substring(i, 2), 16);
        }
    }
}
