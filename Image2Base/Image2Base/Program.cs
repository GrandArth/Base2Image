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
                    int len = getBytesEnd(rgbaBytes);
                    if (len < rgbaBytes.Length)
                    {
                        // File ended
                        var segment = new ArraySegment<byte>(rgbaBytes, 0, len);
                        ContentInByte.AddRange(segment);
                        break;
                    }
                    ContentInByte.AddRange(rgbaBytes);
                }
                // Known issues: Windows CRLF and *nix LF control
                byte[] CIBarray = ContentInByte.ToArray();
                string Outputtext = Encoding.Default.GetString(CIBarray);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(OutputFileDir, Path.GetFileNameWithoutExtension(args[0]) + ".txt")))
                {
                        outputFile.WriteLine(Outputtext);
                }
            }
            return;
        }

        /// <summary>
        /// Get file end from RGBA Bytes
        /// </summary>
        /// <param name="rgbaBytes">RGBA Bytes</param>
        /// <returns>Rgba byte array length</returns>
        static int getBytesEnd(byte[] rgbaBytes)
        {
            if (rgbaBytes == null)
            {
                return 0;
            }
            int len = rgbaBytes.Length;
            for (int i = 0; i < len - 1; i++)
            {
                // Multiple 255 appears means that the offset should be the end of file.
                if (rgbaBytes[i] == 255 && rgbaBytes[i + 1] == 255)
                {
                    len = i;
                    break;
                }
            }
            return len;
        }
    }



}
