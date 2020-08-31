using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace Base2Image
{
    class Program
	{
		static void Main(string[] args)	{
			if (args.Length == 0) return;
			//if no input, end.
			string OutputFileName = Path.GetDirectoryName(args[0])
				+ Path.DirectorySeparatorChar
				+ Path.GetFileNameWithoutExtension(args[0]);
			//compute output path, without extension
			string FileContent = File.ReadAllText(args[0]);
			//read all files
			byte[] FileContentByte = StringEncodeByte(FileContent);
			//compute a byte representation
			//modify the bytes array so that it can be divided by 4
			List<ImagePixelRep> ImageMatrix = new List<ImagePixelRep>();
			//create the final data set for image
			ImageMatrix = Bytes2GroupOfPixels(FileContentByte);
			//compute the final data set
			int OutputImageWidth = ComputeImageWidth(ImageMatrix);
			//Image<Argb32>(OutputImageWidth, OutputImageWidth);

			using (Image<Rgba32> ResultImage = new Image<Rgba32>(OutputImageWidth, OutputImageWidth)) {
				for (int imYindex = 0; imYindex < OutputImageWidth; imYindex++){
					for (int imXindex = 0; imXindex < OutputImageWidth; imXindex++) {
						if (imYindex * OutputImageWidth + imXindex + 1 > ImageMatrix.Count)
						{
							ResultImage[imXindex, imYindex] = new Rgba32(255, 255, 255, 255);
						}
						else
						{
							ResultImage[imXindex, imYindex] = new Rgba32(
								ImageMatrix[imYindex * OutputImageWidth + imXindex].Rchannel,
								ImageMatrix[imYindex * OutputImageWidth + imXindex].Gchannel,
								ImageMatrix[imYindex * OutputImageWidth + imXindex].Bchannel,
								ImageMatrix[imYindex * OutputImageWidth + imXindex].Achannel
								);
						}
					}
				}
				ResultImage.SaveAsPng(OutputFileName+".png");
			}
			//Console.ReadKey();
			return;
		}



        static byte[] StringEncodeByte(string Source)
		{
			//encode source sting into bytes array
			byte[] FileContentByte = Encoding.Default.GetBytes(Source);
			return FileContentByte;
		}
		static byte[] ByteModifer(byte[] OldBytes)
		{
			int resid = OldBytes.Length % 4;
			byte[] buffer = new byte[] { 0, 0, 0, 0 };
			if (resid != 0)
			{
				int OldLength = OldBytes.Length;
				Array.Resize<byte>(ref OldBytes, OldBytes.Length + 4 - resid);
				Array.Copy(buffer, 0, OldBytes, OldLength, 4 - resid);
			}
			return OldBytes;
		}

		static List<ImagePixelRep> Bytes2GroupOfPixels(byte[] ContentInBytes)
		{
			ContentInBytes=ByteModifer(ContentInBytes);
			Console.WriteLine(ContentInBytes.Length);
			//copy the array of bytes into a list of pixels
			//note the bytes should be divideable by 4
			byte[] buffer;
			List<ImagePixelRep> ImagePixelChain = new List<ImagePixelRep>();
			for (int i = 0; i < ContentInBytes.Length; i = i + 4)
			{	
				//Console.WriteLine(ContentInBytes.Length);
				buffer = new byte[4];
				Array.Copy(ContentInBytes, i, buffer, 0, 4);
				ImagePixelChain.Add(Bytes2ImagePixel(buffer[0], buffer[1], buffer[2], buffer[3]));
			}
			return ImagePixelChain;
		}

		static ImagePixelRep Bytes2ImagePixel(byte b1, byte b2, byte b3, byte b4) {
			ImagePixelRep ThePixel = new ImagePixelRep(b1,b2,b3,b4);
			return ThePixel;
		}

		static int ComputeImageWidth(List<ImagePixelRep> ImageRepresent){
			//compute the width of image
			double ImageWidth = System.Math.Sqrt(ImageRepresent.Count);
			int ImageWidthInter = (int)ImageWidth + 1;
			return ImageWidthInter;
		}
	}
	class ImagePixelRep {
		public byte Rchannel;
		public byte Gchannel;
		public byte Bchannel;
		public byte Achannel;
		//we use rgba32 format
		public ImagePixelRep( byte ByteR, byte ByteG, byte ByteB, byte ByteA)
        {
			
			this.Rchannel = ByteR;
            this.Gchannel = ByteG;
            this.Bchannel = ByteB;
			this.Achannel = ByteA;

		}
    }
}
