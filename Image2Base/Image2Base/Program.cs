using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Image2Base
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            string OutputFileDir = Path.GetDirectoryName(args[0])
                + Path.DirectorySeparatorChar;
            using (Image<Rgba32> OriginImage = Image.Load<Rgba32>(args[0])) {
                List<byte> ContentInByte = new List<byte>();
                for (int imYindex = 0; imYindex < OriginImage.Height; imYindex++) {
                    Span<Rgba32> pixelRowSpan = OriginImage.GetPixelRowSpan(imYindex);
                    byte[] rgbaBytes = MemoryMarshal.AsBytes(pixelRowSpan).ToArray();
                    ContentInByte.AddRange(rgbaBytes);
                }
                byte[] CIBarray = ContentInByte.ToArray();
                string Outputtext = Encoding.Default.GetString(CIBarray);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(OutputFileDir, Path.GetFileNameWithoutExtension(args[0]) + ".txt")))
                {
                        outputFile.WriteLine(Outputtext);
                }
            }
            return;
        }
    }



}
