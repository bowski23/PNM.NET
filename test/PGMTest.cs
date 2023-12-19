using PNM;

namespace test;

[TestClass]
public class PGMTest
{

    [TestMethod]
    public void TestFromFile()
    {
        var pgm = PortableAnyMap.FromFile("sample_640×426.pgm");
        var firstBytes = new byte[]{0x8d, 0x88, 0x85, 0x85, 0x86, 0x82, 0x7d, 0x7a, 0x7b, 0x7b, 0x7a, 0x77, 0x74, 0x72, 0x70, 0x70, 0x6f, 0x70, 0x67, 0x63, 0x6c, 0x65, 0x6f, 0xa9, 0x6c, 0x63, 0x60, 0x64, 0x64, 0x5e, 0x5e, 0x64, 0x61, 0x5f, 0x5e, 0x5e, 0x5f, 0x5e, 0x5c, 0x59, 0x5f, 0x5d, 0x5a, 0x5a, 0x5b, 0x5c, 0x5c, 0x5b, 0x5b};
        var lastBytes = new byte[]{0x17, 0x4e, 0x45, 0x1c, 0x31, 0x33, 0x0f, 0x09, 0x11, 0x1e, 0x36, 0x3e, 0x5b, 0x52, 0x2a, 0x31, 0x73, 0x84, 0x4e, 0x1a, 0x39, 0x88, 0x60, 0x40, 0x36, 0x37, 0x2e, 0x2e, 0x3c, 0x23, 0x28, 0x3d, 0x3a, 0x37, 0x47, 0x4b, 0x60, 0x4e, 0x55, 0x72, 0x71, 0x58, 0x39, 0x35, 0x31, 0x46, 0x5d, 0x2f, 0x77};
        Assert.AreEqual(640, pgm.Width);
        Assert.AreEqual(426, pgm.Height);
        Assert.AreEqual(255, pgm.MaxValue);
        Assert.AreEqual(5, pgm.Number.Id);
        Assert.AreEqual(pgm.Width*pgm.Height, pgm.Bytes.Length);
        CollectionAssert.AreEqual(firstBytes, pgm.Bytes.Take(firstBytes.Length).ToArray());
        CollectionAssert.AreEqual(lastBytes, pgm.Bytes.TakeLast(lastBytes.Length).ToArray());
        Assert.AreEqual(0x8d, pgm[0]);
        Assert.AreEqual(0x77, pgm[pgm.Bytes.Length - 1]);
        Assert.AreEqual(0x8d88, pgm.WideBuffer[0]);
        Assert.AreEqual(0x2f77, pgm.WideBuffer[pgm.Bytes.Length / 2 - 1]);
    }
}