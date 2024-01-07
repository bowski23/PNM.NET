using PNM;
using System.IO;
using System.Security.Cryptography;
using System.Text;


[TestClass]
public class PGMTest
{
    SHA1 sha = SHA1.Create();

    public string GetFileHash(string filename)
    {
        var clearBytes = File.ReadAllBytes(filename);
        var hashedBytes = sha.ComputeHash(clearBytes);
        return ConvertBytesToHex(hashedBytes);
    }

    public string ConvertBytesToHex(byte[] bytes)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x"));
        }
        return sb.ToString();
    }

    public bool AreFilesEqual(string filename1, string filename2)
    {
        var hash1 = GetFileHash(filename1);
        var hash2 = GetFileHash(filename2);
        return hash1 == hash2;
    }

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

    [TestMethod]
    public void TestBinaryToFile(){
        var testBytes = new byte[]{
            0x01, 0x00, 0x20, 0x00, 0x01,
            0x00, 0x00, 0x40, 0x00, 0x00,
            0xFF, 0xAA, 0x60, 0xAA, 0xFF,
            0x00, 0x00, 0x40, 0x00, 0x00,
            0x01, 0x00, 0x20, 0x00, 0x01
        };

        PortableAnyMap pgm = new(MagicNumber.P5, 5, 5, 255);
        pgm.Bytes = testBytes;
        Assert.AreEqual(0x01, pgm[0]);
        Assert.AreEqual(0x20, pgm[2]);
        Assert.AreEqual(0x60, pgm[2,2]);
        pgm.ToFile("testBinaryTemp.pgm", "test comment");


        AreFilesEqual("testBinaryTemp.pgm", "testBinary.pgm");

        pgm = new(MagicNumber.P2, 5, 5, 255);
        pgm.Bytes = testBytes;
        pgm.ToFile("testAsciiTemp.pgm", "test comment");
        AreFilesEqual("testAsciiTemp.pgm", "testAscii.pgm");
    }
    
}