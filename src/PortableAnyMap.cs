using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Buffers.Binary;

namespace PNM
{
    public class PortableAnyMap
    {
        public readonly MagicNumber Number;
        public readonly int Width;
        public readonly int Height;
        public readonly int MaxValue;
        public readonly int BytesPerPixelChannel;
        public byte[] Bytes;

        public ushort[] WideBuffer
        {
            get
            {
                ushort[] wbytes = new ushort[Bytes.Length / 2];
                Buffer.BlockCopy(Bytes, 0, wbytes, 0, Bytes.Length);
                for (int i = 0; i < wbytes.Length; i++)
                {
                    wbytes[i] = BinaryPrimitives.ReverseEndianness(wbytes[i]);
                }
                return wbytes;
            }
        }

        public PortableAnyMap(MagicNumber number, int width, int height, int maxValue, bool splitBitmapToBytes = true)
        {
            if (number == MagicNumber.P3 || number == MagicNumber.P6) throw new NotImplementedException();
            Number = number;
            Width = width;
            Height = height;
            MaxValue = maxValue;
            BytesPerPixelChannel = maxValue > 255 ? 2 : 1;
            if (!splitBitmapToBytes && maxValue == 1)
            {
                throw new NotImplementedException();
                //Bytes = new byte[(int)(float)Width * Height / 8];
            }
            else
            {
                Bytes = new byte[Width * Height * BytesPerPixelChannel];
            }
        }

        public int this[int index]
        {
            get
            {
                if (BytesPerPixelChannel == 2)
                    return Bytes[index * 2] << 8 | Bytes[index * 2 + 1];
                return Bytes[index];
            }
            set
            {
                if (value > MaxValue) throw new Exception("Value exceeds max value");
                if (BytesPerPixelChannel == 2)
                {
                    Bytes[index * 2] = (byte)(value >> 8);
                    Bytes[index * 2 + 1] = (byte)value;
                    return;
                }
                Bytes[index] = (byte)value;
            }
        }

        public int this[int width, int height]
        {
            get
            {
                return this[width + height * Width];
            }
            set
            {
                this[width + height * Width] = value;
            }
        }

        private static void SkipComments(StreamReader reader)
        {
            while (reader.Peek() == '#')
            {
                reader.ReadLine();
            }
        }
        private static (int, int) ReadDims(StreamReader reader)
        {
            var vals = Regex.Split(reader.ReadLine(), "\\s+");
            var dims = vals.Select(s => int.Parse(s)).ToArray();
            return (dims[0], dims[1]);
        }

        private static void ReadBinaryImage(StreamReader reader, PortableAnyMap pnm)
        {
            reader.BaseStream.Position = reader.BaseStream.Length - pnm.Bytes.Length;
            reader.BaseStream.Read(pnm.Bytes, 0, pnm.Bytes.Length);
        }

        private static void ReadAsciiImage(TextReader reader, PortableAnyMap pnm)
        {
            var index = 0;
            while (reader.Peek() != -1)
            {
                IEnumerable<int> vals = Regex.Split(reader.ReadLine(), "\\s+").Select(s => int.Parse(s));
                foreach (var val in vals)
                {
                    pnm[index++] = val;
                }
            }
        }

        public static PortableAnyMap FromFile(string path)
        {
            var sstream = File.OpenText(path);
            return FromStreamReader(sstream);
        }

        public static PortableAnyMap FromStreamReader(StreamReader reader)
        {
            var magicNumber = MagicNumber.FromName(reader.ReadLine());
            if (magicNumber != MagicNumber.P2 && magicNumber != MagicNumber.P5) throw new NotImplementedException();

            SkipComments(reader);

            (int width, int height) = ReadDims(reader);

            int maxValue;
            if (magicNumber != MagicNumber.P1 && magicNumber != MagicNumber.P4)
            {
                SkipComments(reader);
                maxValue = int.Parse(reader.ReadLine());
            }
            else
            {
                maxValue = 1;
            }

            PortableAnyMap pnm = new PortableAnyMap(magicNumber, width, height, maxValue);

            if (magicNumber.IsBinary)
            {
                ReadBinaryImage(reader, pnm);
            }
            else
            {
                ReadAsciiImage(reader, pnm);
            }

            return pnm;
        }

        public void ToFile(string path)
        {
            throw new NotImplementedException();
        }
    }

}