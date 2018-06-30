using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using FreeImageAPI;

// THIS SOFTWARE IS LICENSED UNDER THE LGPL 3.0. SEE COPYING and COPYING.LESSER FOR DETAILS.
// SOME COMPONENTS USED UNDER OTHER LICENSES.
// PLEASE RETAIN ALL LICENSING INFORMATION IN YOUR SOURCE CODE IF YOU MODIFY THIS SOFTWARE.

namespace IMP2DEC
{
    // Utilities class, from TargaImage.cs
    // ==========================================================
    // TargaImage
    //
    // Design and implementation by
    // - David Polomis (paloma_sw@cox.net)
    //
    //
    // This source code, along with any associated files, is licensed under
    // The Code Project Open License (CPOL) 1.02
    // A copy of this license can be found in the CPOL.html file 
    // which was downloaded with this source code
    // or at http://www.codeproject.com/info/cpol10.aspx
    //
    // 
    // COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS,
    // WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,
    // INCLUDING, WITHOUT LIMITATION, WARRANTIES THAT THE COVERED CODE IS
    // FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE OR
    // NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE
    // OF THE COVERED CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE
    // DEFECTIVE IN ANY RESPECT, YOU (NOT THE INITIAL DEVELOPER OR ANY
    // OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY SERVICING,
    // REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN
    // ESSENTIAL PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS
    // AUTHORIZED HEREUNDER EXCEPT UNDER THIS DISCLAIMER.
    //
    // Use at your own risk!
    //
    // ==========================================================


    static class Utilities
    {
        /// <summary>
        /// Gets an int value representing the subset of bits from a single Byte.
        /// </summary>
        /// <param name="b">The Byte used to get the subset of bits from.</param>
        /// <param name="offset">The offset of bits starting from the right.</param>
        /// <param name="count">The number of bits to read.</param>
        /// <returns>
        /// An int value representing the subset of bits.
        /// </returns>
        /// <remarks>
        /// Given -> b = 00110101 
        /// A call to GetBits(b, 2, 4)
        /// GetBits looks at the following bits in the byte -> 00{1101}00
        /// Returns 1101 as an int (13)
        /// </remarks>
        internal static int GetBits(byte b, int offset, int count)
        {
            return (b >> offset) & ((1 << count) - 1);
        }

        /// <summary>
        /// Reads ARGB values from the 16 bits of two given Bytes in a 1555 format.
        /// </summary>
        /// <param name="one">The first Byte.</param>
        /// <param name="two">The Second Byte.</param>
        /// <returns>A System.Drawing.Color with a ARGB values read from the two given Bytes</returns>
        /// <remarks>
        /// Gets the ARGB values from the 16 bits in the two bytes based on the below diagram
        /// |   BYTE 1   |  BYTE 2   |
        /// | A RRRRR GG | GGG BBBBB |
        /// </remarks>
        internal static Color GetColorFrom2Bytes(byte one, byte two)
        {
            // get the 5 bits used for the RED value from the first byte
            int r1 = Utilities.GetBits(one, 2, 5);
            int r = r1 << 3;

            // get the two high order bits for GREEN from the from the first byte
            int bit = Utilities.GetBits(one, 0, 2);
            // shift bits to the high order
            int g1 = bit << 6;

            // get the 3 low order bits for GREEN from the from the second byte
            bit = Utilities.GetBits(two, 5, 3);
            // shift the low order bits
            int g2 = bit << 3;
            // add the shifted values together to get the full GREEN value
            int g = g1 + g2;

            // get the 5 bits used for the BLUE value from the second byte
            int b1 = Utilities.GetBits(two, 0, 5);
            int b = b1 << 3;

            // get the 1 bit used for the ALPHA value from the first byte
            int a1 = Utilities.GetBits(one, 7, 1);
            int a = a1 * 255;

            //treat 16-bit numbers within error range of 255 as white
            if ((r >= 248) && (g >= 248) && (b >= 248))
            {
               // r = 255;
               // g = 255;
               // b = 255;
                //Console.WriteLine(a);
                //Console.ReadKey();
            }

            
            // return the resulting Color
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Gets a 32 character binary string of the specified Int32 value.
        /// </summary>
        /// <param name="n">The value to get a binary string for.</param>
        /// <returns>A string with the resulting binary for the supplied value.</returns>
        /// <remarks>
        /// This method was used during debugging and is left here just for fun.
        /// </remarks>
        internal static string GetIntBinaryString(Int32 n)
        {
            char[] b = new char[32];
            int pos = 31;
            int i = 0;

            while (i < 32)
            {
                if ((n & (1 << i)) != 0)
                {
                    b[pos] = '1';
                }
                else
                {
                    b[pos] = '0';
                }
                pos--;
                i++;
            }
            return new string(b);
        }

        /// <summary>
        /// Gets a 16 character binary string of the specified Int16 value.
        /// </summary>
        /// <param name="n">The value to get a binary string for.</param>
        /// <returns>A string with the resulting binary for the supplied value.</returns>
        /// <remarks>
        /// This method was used during debugging and is left here just for fun.
        /// </remarks>
        internal static string GetInt16BinaryString(Int16 n)
        {
            char[] b = new char[16];
            int pos = 15;
            int i = 0;

            while (i < 16)
            {
                if ((n & (1 << i)) != 0)
                {
                    b[pos] = '1';
                }
                else
                {
                    b[pos] = '0';
                }
                pos--;
                i++;
            }
            return new string(b);
        }

        internal static void printIntBits(int value)
        {

            string s = Convert.ToString(value, 2).PadLeft(8, '0');
            Console.WriteLine(s);
            return;
        }

        internal static void printUShortBits(ushort value)
        {

            string s = Convert.ToString(value, 2).PadLeft(16, '0');
            Console.WriteLine(s);
            return;
        }

        internal static int ReverseBytes(long val)
        {
            byte[] intAsBytes = BitConverter.GetBytes(val);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }

        internal static string IntToBinaryString(long v)
        {
            string s = Convert.ToString(v, 2);
            string t = s.PadLeft(32, '0');
            string res = "";
            for (int i = 0; i < t.Length; ++i)
            {
                if (i > 0 && i % 8 == 0)
                    res += " ";
                res += t[i];
            }
            return res;
        }
    }

    public struct RecordContentHeaderStruct
    {
        public Int32 contentHeaderLength;
        public Int32 imageWidth;
        public Int32 imageHeight;
        public Int16 frameCount; //not sure on this one; tends to be 1
        public Int16 colorDepthInBits; //not sure on this one
        public Int32 lengthOfImageDataFor8BitImages; //not sure on this one
        public Int32 paletteSizeFor8BitImages;
        public Int32 mysteriousNumber1; //tends to be 2835
        public Int32 mysteriousNumber2;//tends to be 2835
        public Int32 endDataHeader;
    }

    public struct RecordHeaderStruct
    {
        public string blockType;
        public Int32 recordID;
        public Int32 startOffset;
        public Int32 dataSize;
        public Int32 originalPosition;
        public RecordContentHeaderStruct recordContentHeader;
    }


    class Program
    {
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        unsafe static void Main(string[] args)
        {
            byte version1;
            byte version2;
            byte numRecordsAsByte;
            int numRecords;
            string filename;
            string outputFolderName;
            RecordHeaderStruct[] recHeads;


            Console.WriteLine();
            Console.WriteLine("IMPERIALISM II PC .RSRC FILE UNPACKER");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Version 1.1 released in July 2017 by n64gamer");
            Console.WriteLine("");
            Console.WriteLine("This software is released under the LGPL.");
            Console.WriteLine("Portions of this software use code licensed under the CPOL.");
            Console.WriteLine("Please include acknowledgements below if you make changes to the code.");
            Console.WriteLine("");
            Console.WriteLine("Made with Utilities class from TargaImage.cs by David Polomis");
            Console.WriteLine("(paloma_sw@cox.net) which is licensed under Code Project Open License");
            Console.WriteLine("(CPOL) 1.02.");
            Console.WriteLine("");
            Console.WriteLine("Parts of this software use modified code portions from Zachtronics.com");
            Console.WriteLine("Yoda Stories reverse engineering tutorial:");
            Console.WriteLine("http://www.zachtronics.com/yoda-stories/");
            Console.WriteLine("");
            Console.WriteLine("Other portions obtained from various programming forums discussing C++ and C#.");
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            if (args.GetLength(0) == 0)
            {
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadKey();
                Console.WriteLine("------------------------------------");
            }
            Console.WriteLine();
            Console.WriteLine("Note: This tool won't unpack Imperialism II for Mac .RSRC files. Use a Resource");
            Console.WriteLine("Fork Editor on Mac to access the contents of Mac .RSRC files for Imperialism II");
            Console.WriteLine();
            
            Console.WriteLine("------------------------------------");
            Console.WriteLine();
            if (args.GetLength(0) == 0) {
                Console.WriteLine("Usage Instructions:");
                Console.WriteLine("IMP2DEC.EXE [RSRCFILENAME]");
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Error: No RSRCFILENAME provided!");
                Console.WriteLine("Please provide the filename of the Imperialism II .RSRC file you wish to unpack.");
            }
            else 
            {
                if (File.Exists(args[0]))
                {
                    filename = args[0];
                    outputFolderName = Path.GetFileNameWithoutExtension(filename);

                    /* create output directory if it doesn't exist */
                    if (!Directory.Exists(outputFolderName))
                    Directory.CreateDirectory(@""+outputFolderName);

                    using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(args[0])))
                    {
                        string headerCode = new string(binaryReader.ReadChars(4));
                        if (headerCode.Equals("rsrc"))
                        {
                            version1 = binaryReader.ReadByte();
                            /* empty bytes */
                            binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            Console.WriteLine("RSRC Format Version1: " + version1);
                            version2 = binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            Console.WriteLine("RSRC Format Version2: " + version2);
                            numRecordsAsByte = binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            binaryReader.ReadByte();
                            numRecords = Convert.ToInt16(numRecordsAsByte);
                            Console.WriteLine("Number of Records in RSRC File: " + numRecords);
                            recHeads = new RecordHeaderStruct[numRecords];

                            /* read data headers */
                            for (int i = 0; i < numRecords; i++) 
                            {
                                string chunktype = new string(binaryReader.ReadChars(4));
                                RecordHeaderStruct recHead = default(RecordHeaderStruct);
                                recHead.blockType = chunktype;
                                recHead.originalPosition = (i + 1);
                                recHead.recordContentHeader = default(RecordContentHeaderStruct);

                                if ((chunktype.Equals("TCIP")) || (chunktype.Equals(" dns")))
                                {
                                    //we have a picture 
                                    Int32 recordID = binaryReader.ReadInt32();
                                    Int32 startOffset = binaryReader.ReadInt32();
                                    Int32 dataSize = binaryReader.ReadInt32();
                                    recHead.recordID = recordID;
                                    recHead.startOffset = startOffset;
                                    recHead.dataSize = dataSize;
                                    recHeads[i] = recHead;
                                    Console.WriteLine("SLOT: " + recHeads[i].originalPosition + " REC#: " + recordID + " StartAt: " + startOffset + "  DataSize: " + dataSize);
                                }
                                else
                                {
                                    Console.WriteLine("Unknown BLOCKTYPE in BLOCK HEADER for ENTRY# " + (i + 1) + " : "+chunktype);
                                }
                            }

                            Int32 tileSectionLength;

                            /* read data blocks */
                            for (int j = 0; j < numRecords; j++)
                            {
                                if (recHeads[j].blockType == "TCIP") /* PICTURE RESOURCE */
                                {
                                    var stream = binaryReader.BaseStream;
                                    /* 40 byte header */
                                    recHeads[j].recordContentHeader.contentHeaderLength = binaryReader.ReadInt32();
                                    recHeads[j].recordContentHeader.imageWidth = binaryReader.ReadInt32();
                                    recHeads[j].recordContentHeader.imageHeight = binaryReader.ReadInt32();
                                    recHeads[j].recordContentHeader.frameCount = binaryReader.ReadInt16();
                                    recHeads[j].recordContentHeader.colorDepthInBits = binaryReader.ReadInt16();
                                    binaryReader.ReadInt32(); //32-bit gap (zeroes)
                                    recHeads[j].recordContentHeader.lengthOfImageDataFor8BitImages = binaryReader.ReadInt32();
                                    recHeads[j].recordContentHeader.paletteSizeFor8BitImages = recHeads[j].dataSize - recHeads[j].recordContentHeader.contentHeaderLength - recHeads[j].recordContentHeader.lengthOfImageDataFor8BitImages;
                                    recHeads[j].recordContentHeader.mysteriousNumber1 = binaryReader.ReadInt32();
                                    recHeads[j].recordContentHeader.mysteriousNumber2 = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32(); //32-bit gap (zeroes)
                                    recHeads[j].recordContentHeader.endDataHeader = binaryReader.ReadInt32(); //32-bit gap (zeroes)
                                    /* end of 40 byte header */

                                    tileSectionLength = recHeads[j].dataSize - 40; //40 for the header length

                                    uint imWidthEven = (uint)recHeads[j].recordContentHeader.imageWidth;
                                    uint imWidthActual = (uint)recHeads[j].recordContentHeader.imageWidth;
                                    Boolean widthHackApplied = false;

                                    /* handle binary file padding quirks */
                                    if (imWidthEven % 2 == 1)
                                    {
                                        bool divisbleBy2 = false;
                                        do
                                        {
                                            widthHackApplied = true;
                                            imWidthEven++;

                                            if (recHeads[j].recordContentHeader.colorDepthInBits == 16)
                                            {
                                                if ((imWidthEven % 2) == 0) { divisbleBy2 = true; }
                                            }
                                            else if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                            {
                                                if ((imWidthEven % 4) == 0) { divisbleBy2 = true; }
                                            }
                                        }
                                        while (divisbleBy2 == false);
                                    } //round up to nearest divisor of 2; quirk of the file format
                                    else if ((imWidthEven % 4 != 0) && (recHeads[j].recordContentHeader.colorDepthInBits == 8))
                                    {
                                        bool divisbleBy4 = false;
                                        do
                                        {
                                            widthHackApplied = true;
                                            uint difference = imWidthEven % 4;
                                            imWidthEven = imWidthEven + difference;

                                            if (recHeads[j].recordContentHeader.colorDepthInBits == 16)
                                            {
                                                if ((imWidthEven % 4) == 0) { divisbleBy4 = true; }
                                            }
                                            else if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                            {
                                                if ((imWidthEven % 4) == 0) { divisbleBy4 = true; }
                                            }
                                            else if (recHeads[j].recordContentHeader.colorDepthInBits == 24)
                                            {
                                                if ((imWidthEven % 4) == 0) { divisbleBy4 = true; }
                                            }
                                        }
                                        while (divisbleBy4 == false);
                                    } //round up to nearest divisor of 4; quirk of the file format

                                    uint totalPixels;

                                    if (widthHackApplied == true)
                                    {
                                        totalPixels = imWidthEven * (uint)recHeads[j].recordContentHeader.imageHeight;
                                    }
                                    else
                                    {
                                        totalPixels = (uint)recHeads[j].recordContentHeader.imageWidth * (uint)recHeads[j].recordContentHeader.imageHeight;
                                    }


                                    //Bitmap theImage = null;
                                    FIBITMAP dib = new FIBITMAP();
                                    Color bg = Color.FromArgb(255, 255, 255, 255);
                                    //ImageCodecInfo myImageCodecInfo = null;
                                    if (recHeads[j].recordContentHeader.colorDepthInBits == 16)
                                    {
                                        if (widthHackApplied == true)
                                        {
                                            
                                            //Console.WriteLine("color depth 16");
                                            //dib = FreeImage.Allocate((int)imWidthEven, recHeads[j].recordContentHeader.imageHeight, 16, FreeImage.FI16_555_RED_MASK, FreeImage.FI16_555_GREEN_MASK, FreeImage.FI16_555_BLUE_MASK);
                                            //dib = FreeImage.Allocate((int)imWidthEven, recHeads[j].recordContentHeader.imageHeight, 32);
                                            dib = FreeImage.AllocateEx((int)imWidthEven, recHeads[j].recordContentHeader.imageHeight, 32, bg, FREE_IMAGE_COLOR_OPTIONS.FICO_RGBA, null);
                                        }
                                        else
                                        {
                                            //dib = FreeImage.Allocate(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, 16, FreeImage.FI16_555_RED_MASK, FreeImage.FI16_555_GREEN_MASK, FreeImage.FI16_555_BLUE_MASK);
                                            //dib = FreeImage.Allocate(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, 32);
                                            dib = FreeImage.AllocateEx(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, 32, bg, FREE_IMAGE_COLOR_OPTIONS.FICO_RGBA, null);
                                        }
                                        //theImage = new Bitmap(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, System.Drawing.Imaging.PixelFormat.Format16bppArgb1555);
                                        //myImageCodecInfo = GetEncoderInfo("image/bmp");
                                    }
                                    else if (recHeads[j].recordContentHeader.colorDepthInBits == 24)
                                    {
                                        if (widthHackApplied == true)
                                        {
                                            //Console.WriteLine("color depth 24");
                                            dib = FreeImage.Allocate((int)imWidthEven, recHeads[j].recordContentHeader.imageHeight, 24);
                                        }
                                        else
                                        {
                                            dib = FreeImage.Allocate(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, 24);
                                        }
                                        //theImage = new Bitmap(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                                        //myImageCodecInfo = GetEncoderInfo("image/bmp");
                                    }
                                    else if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                    {
                                        //Console.WriteLine("color depth 8");
                                        if (widthHackApplied == true)
                                        {
                                            dib = FreeImage.Allocate((int)imWidthEven, recHeads[j].recordContentHeader.imageHeight, 8, 0, 0, 0);
                                        }
                                        else
                                        {
                                            dib = FreeImage.Allocate(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, 8, 0, 0, 0);
                                        }

                                        //theImage = new Bitmap(recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                                        //myImageCodecInfo = GetEncoderInfo("image/bmp");
                                    }
                                    

                                    uint x = 0;
                                    uint y = 0;
                                    byte[] palette8 = new byte[1024];
                                    byte[] color16;
                                    byte[] color24;
                                    byte color8;
                                    byte palR;
                                    byte palG;
                                    byte palB;
                                    byte palX;
                                    int intR;
                                    int intG;
                                    int intB;
                                    
                                    Color paletteColor8;
                                    Color pixelColor16;
                                    
                                    Color pixelColor = Color.FromArgb(255, 255, 255, 255); //defaults to white
                                    //Console.WriteLine("total Pixels:" + totalPixels);

                                    if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                    {
                                        //Console.WriteLine("cd8");
                                        //read the first 1024 bytes containing the palette for 8-bit files
                                        //palette8 = binaryReader.ReadBytes(recHeads[j].recordContentHeader.paletteSizeFor8BitImages);
                                        //string palFilename = string.Format(@"" + outputFolderName + "\\" + "{0}_{1}_{2}bit.pal", recHeads[j].originalPosition, recHeads[j].recordID, recHeads[j].recordContentHeader.colorDepthInBits);
                                        //File.WriteAllBytes(palFilename, palette8);
                                        //Console.WriteLine("Saved palette to " + palFilename + "");

                                        Palette palette = FreeImage.GetPaletteEx(dib);
                                        //int palSize = palette.Length;
                                        // Loading the palette
                                        for (int i = 0; i < 256; i++)
                                        {
                                            palR = binaryReader.ReadByte();
                                            palG = binaryReader.ReadByte();
                                            palB = binaryReader.ReadByte();
                                            palX = binaryReader.ReadByte();
                                            intR = (int)palR;
                                            intG = (int)palG;
                                            intB = (int)palB;
                                            //Console.WriteLine(intR + "r, " + intG + "g, " + intB + "b");
                                            //Console.ReadKey();
                                            //RGBQUAD temp = palette[i];
                                            paletteColor8 = Color.FromArgb(255, palB, palG, palR);
                                            RGBQUAD indexColor = new RGBQUAD(paletteColor8);
                                            palette[i] = indexColor;
                                        }
                                        //Console.WriteLine("end cd8");

                                    }
                                    //Console.WriteLine("read pix");
                                    //Console.WriteLine("streampos: " + binaryReader.BaseStream.Position);
                                    //Console.ReadKey();

                                    
                                    //read the pixel data
                                    for (uint k = 0; k < (totalPixels); k++)
                                    {
                                        if (recHeads[j].recordContentHeader.colorDepthInBits == 16)
                                        {
                                            color16 = binaryReader.ReadBytes(2);
                                            //ushort = binaryReader.ReadUInt16();
                                            pixelColor16 = Utilities.GetColorFrom2Bytes(color16[1], color16[0]);
                                            if ((color16[1] == 255) && (color16[0] == 255))
                                            {
                                                Console.WriteLine("FF FF");
                                                Console.WriteLine(pixelColor16.R);
                                                Console.WriteLine(pixelColor16.G);
                                                Console.WriteLine(pixelColor16.B);
                                                Console.WriteLine(pixelColor16.A);
                                                pixelColor16 = Color.FromArgb(255, 255, 255, 255);
                                                //Console.ReadKey();
                                            }
                                            else if ((color16[1] == 127) && (color16[0] == 255)) {
                                                Console.WriteLine("7F FF");
                                                Console.WriteLine(pixelColor16.R);
                                                Console.WriteLine(pixelColor16.G);
                                                Console.WriteLine(pixelColor16.B); 
                                                Console.WriteLine(pixelColor16.A);
                                                pixelColor16 = Color.FromArgb(127, 255, 255, 255);
                                                //Console.ReadKey();
                                            }

                                                //Utilities.printIntBits(color16[0]);
                                                //Console.WriteLine(color16[0]);
                                                //Utilities.printIntBits(color16[1]);
                                                //Console.WriteLine(color16[1]);
                                                //Console.ReadKey();
                                                //not sure how transparency in 16-bit files is handled - have set all pixels to 100% opaque with 255 alpha
                                                pixelColor = pixelColor16; // Color.FromArgb(255, pixelColor16);
                                        }
                                        else if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                        {
                                            //thanks to Zachtronics.com Yoda Stories tutorial for this! (and other code)
                                            //color8 = binaryReader.ReadByte();
                                            //pixelIndex = (int)color8;
                                            //byte r = palette8[color8 * 4 + 2];
                                            //byte g = palette8[color8 * 4 + 1];
                                            //byte b = palette8[color8 * 4 + 0];
                                            //pixelColor = Color.FromArgb(255, r, g, b);
                                        }
                                        else if (recHeads[j].recordContentHeader.colorDepthInBits == 24)
                                        {
                                            //no support for transparency in 24-bit files, sorry - I'm clueless with alpha stuff
                                            color24 = binaryReader.ReadBytes(3);
                                            pixelColor = Color.FromArgb(color24[2], color24[1], color24[0]);
                                        }

                                        

                                        /* take into account binary file padding quirks */
                                        if (widthHackApplied == true)
                                        {
                                           // Console.WriteLine("width hack applied");
                                            x = k % (imWidthEven);
                                           // Console.WriteLine("x = "+x);
                                            
                                            if ((k > (imWidthEven - 1)) && (k % (imWidthEven) == 0))
                                            {
                                                y = y + 1;
                                            }
                                            //Console.WriteLine("y = " + y);
                                            
                                        }
                                        else
                                        {
                                            //Console.WriteLine("no width hack");
                                            x = k % ((uint)recHeads[j].recordContentHeader.imageWidth);
                                            if ((k > (recHeads[j].recordContentHeader.imageWidth - 1)) && (k % (recHeads[j].recordContentHeader.imageWidth) == 0))
                                            {
                                                y = y + 1;
                                            }
                                        }

                                        //Console.WriteLine("IWA = " + imWidthActual);
                                        //Console.WriteLine("IWE = " + imWidthEven);
                                        //Console.ReadKey();
                                        //uint newx = (uint)recHeads[j].recordContentHeader.imageWidth - x - 1;

                                        uint newx = 0;
                                        if (widthHackApplied == true)
                                        {
                                            newx = imWidthEven - x - 1;
                                            //Console.WriteLine("WH newx = " + newx);
                                            //Console.ReadKey();
                                        }
                                        else { 
                                            newx = (uint)recHeads[j].recordContentHeader.imageWidth - x - 1;
                                        }

                                        /* more binary padding quirks, this one known as a width hack */
                                        if (widthHackApplied == true)
                                        {
                                            if (x >= imWidthActual - 1)
                                            {
                                                //do nothing
                                                if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                                {
                                                    //Console.WriteLine("reading byte");
                                                    color8 = binaryReader.ReadByte();
                                                    
                                                    FreeImage.SetPixelIndex(dib, newx, y, ref color8);
                                                }
                                                else
                                                {
                                                    RGBQUAD rgbq_a;
                                                    rgbq_a = new RGBQUAD(pixelColor);
                                                    FreeImage.SetPixelColor(dib, newx, y, ref rgbq_a);
                                                    //theImage.SetPixel(x, y, pixelColor);
                                                }
                                            }
                                            else
                                            {
                                                if (recHeads[j].recordContentHeader.colorDepthInBits == 8) {
                                                    //Console.WriteLine("reading byte");
                                                    color8 = binaryReader.ReadByte();
                                                    FreeImage.SetPixelIndex(dib, newx, y, ref color8);
                                                }
                                                else
                                                {
                                                    RGBQUAD rgbq_a;
                                                    rgbq_a = new RGBQUAD(pixelColor);
                                                    FreeImage.SetPixelColor(dib, newx, y, ref rgbq_a);
                                                    //theImage.SetPixel(x, y, pixelColor);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (recHeads[j].recordContentHeader.colorDepthInBits == 8) {
                                                //Console.WriteLine("reading more byte");
                                                color8 = binaryReader.ReadByte();
                                                FreeImage.SetPixelIndex(dib, newx, y, ref color8);
                                                
                                            }
                                            else
                                            {
                                                RGBQUAD rgbq_b;
                                                rgbq_b = new RGBQUAD(pixelColor);
                                                FreeImage.SetPixelColor(dib, newx, y, ref rgbq_b);
                                                //theImage.SetPixel(x, y, pixelColor);
                                            }
                                        }
                                        //Console.WriteLine("cwh: " + k);
                                        //Console.WriteLine("streampos: " + binaryReader.BaseStream.Position);
                                    }

                                    

                                    //Console.WriteLine("tasks");

                                    //rotate image by 180 deg and flip on x axis
                                    //theImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);
                                    //FreeImage.Rotate(dib, 180); IMPORTANT
                                    FreeImage.FlipHorizontal(dib);

                                    //write the data to the file
                                    if (((recHeads[j].recordContentHeader.colorDepthInBits == 16) || (recHeads[j].recordContentHeader.colorDepthInBits == 8)) || (recHeads[j].recordContentHeader.colorDepthInBits == 24))
                                    {
                                        string outputFilename = "";
                                        if (recHeads[j].recordContentHeader.colorDepthInBits == 16)
                                        {
                                            outputFilename = string.Format(@"" + outputFolderName + "\\" + "{0}_{1}_{2}bit_{3}_x_{4}.bmp", recHeads[j].originalPosition, recHeads[j].recordID, recHeads[j].recordContentHeader.colorDepthInBits, recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight);
                                            //dib = FreeImage.ConvertTo24Bits(dib);
                                            //System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.ColorDepth;
                                            //System.Drawing.Imaging.EncoderParameters myEncoderParameters = new EncoderParameters(1);

                                            // Save the image with a color depth of 16 bits per pixel.
                                            //System.Drawing.Imaging.EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 16L);
                                            //myEncoderParameters.Param[0] = myEncoderParameter;

                                            //theImage.Save(outputFilename, myImageCodecInfo, myEncoderParameters);

                                            //FreeImage.SaveEx(ref dib, outputFilename, FREE_IMAGE_FORMAT.FIF_BMP, FREE_IMAGE_SAVE_FLAGS.DEFAULT, FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP, true);
                                            //FreeImage.SaveEx(ref dib, outputFilename, FREE_IMAGE_FORMAT.FIF_TARGA, FREE_IMAGE_SAVE_FLAGS.DEFAULT, FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP_555, true);
                                            Console.Write("Saving to " + outputFilename + "...");
                                            FreeImage.SaveEx(ref dib, outputFilename, FREE_IMAGE_FORMAT.FIF_BMP, FREE_IMAGE_SAVE_FLAGS.DEFAULT, FREE_IMAGE_COLOR_DEPTH.FICD_32_BPP, true);
                                            //FreeImage.SaveEx(ref dib, outputFilename, FREE_IMAGE_FORMAT.FIF_BMP, FREE_IMAGE_SAVE_FLAGS.DEFAULT, FREE_IMAGE_COLOR_DEPTH.FICD_16_BPP, true);
                                        }
                                        else if (recHeads[j].recordContentHeader.colorDepthInBits == 24)
                                        {
                                            outputFilename = string.Format(@"" + outputFolderName + "\\" + "{0}_{1}_{2}bit_{3}_x_{4}.bmp", recHeads[j].originalPosition, recHeads[j].recordID, recHeads[j].recordContentHeader.colorDepthInBits, recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight);
                                            //System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.ColorDepth;
                                            //System.Drawing.Imaging.EncoderParameters myEncoderParameters = new EncoderParameters(1);

                                            // Save the image with a color depth of 16 bits per pixel.
                                            //System.Drawing.Imaging.EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 24L);
                                            //myEncoderParameters.Param[0] = myEncoderParameter;

                                            //theImage.Save(outputFilename, myImageCodecInfo, myEncoderParameters);
                                            //FreeImage.SaveEx(ref dib, outputFilename, true);
                                            Console.Write("Saving to " + outputFilename + "...");
                                            FreeImage.SaveEx(ref dib, outputFilename, FREE_IMAGE_FORMAT.FIF_BMP, FREE_IMAGE_SAVE_FLAGS.DEFAULT, FREE_IMAGE_COLOR_DEPTH.FICD_24_BPP, true);
                                        }
                                        else if (recHeads[j].recordContentHeader.colorDepthInBits == 8)
                                        {
                                            outputFilename = string.Format(@"" + outputFolderName + "\\" + "{0}_{1}_{2}bit_{3}_x_{4}.png", recHeads[j].originalPosition, recHeads[j].recordID, recHeads[j].recordContentHeader.colorDepthInBits, recHeads[j].recordContentHeader.imageWidth, recHeads[j].recordContentHeader.imageHeight);
                                            Console.Write("Saving to " + outputFilename + "...");
                                            FreeImage.SaveEx(ref dib, outputFilename, FREE_IMAGE_FORMAT.FIF_PNG, FREE_IMAGE_SAVE_FLAGS.DEFAULT, FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP, true);
                                            
                                        }
                                        Console.WriteLine("done!");
                                    }
                                    else
                                    {
                                        Console.WriteLine(recHeads[j].recordContentHeader.colorDepthInBits + "-bit Image Extraction not supported yet!");
                                        
                                    }

                                    //empty the image object to save memory - not sure if this does anything useful!
                                    //theImage = null;
                                    FreeImage.UnloadEx(ref dib);
                                    //Console.WriteLine("unload!");
                                    GC.Collect();
                                    //Console.WriteLine("gc!");
                                }
                                else if (recHeads[j].blockType == " dns") /* SOUND RESOURCE */
                                {
                                    //sound file header
                                    byte[] dataHeader = binaryReader.ReadBytes(4); //LOAF header
                                    Int32 numChannels = binaryReader.ReadInt32();
                                    Int32 sampleRate = binaryReader.ReadInt32();
                                    Int16 sampRate16 = (Int16)sampleRate;
                                    Int32 bitsPerSample = binaryReader.ReadInt32();
                                    Int32 numSamples = binaryReader.ReadInt32();
                                    int dataLength = recHeads[j].dataSize - 20; //header size is 20 bytes for " dns" entries in the RSRC file

                                    //read sound data
                                    byte[] soundData = binaryReader.ReadBytes(dataLength);
                                    
                                    //write RAW file to disk
                                    string rawFilename = string.Format(@"" + outputFolderName + "\\" + "{0}_{1}.raw", recHeads[j].originalPosition, recHeads[j].recordID);
                                    string wavFilename = string.Format(@"" + outputFolderName + "\\" + "{0}_{1}.wav", recHeads[j].originalPosition, recHeads[j].recordID);
                                    File.WriteAllBytes(rawFilename, soundData);
                                    Console.WriteLine("Saved raw sound data to " + rawFilename + "");
                                    
                                    /* write WAV file to disk */
                                    FileStream fs = new FileStream(wavFilename, FileMode.Create, FileAccess.Write);
                                    BinaryWriter bw = new BinaryWriter(fs);
                                    Int32 chunkSize = dataLength + 16; //dont include the first 8 bytes used by the RIFF header
                                    bw.Write(new char[4] { 'R', 'I', 'F', 'F' });
                                    bw.Write(chunkSize);
                                    bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                                    bw.Write((int)16); //size of fmt section = 16 bytes
                                    bw.Write((short)1); //AudioFormat
                                    bw.Write((short)numChannels); //NumChannels
                                    bw.Write(sampleRate);
                                    Int32 avgBytesPerSec = (sampleRate * (bitsPerSample * numChannels) / 8);
                                    bw.Write(avgBytesPerSec);
                                    short blockAlign = 4;
                                    bw.Write((short)blockAlign);
                                    bw.Write((short)bitsPerSample);
                                    bw.Write(new char[4] { 'd', 'a', 't', 'a' });
                                    bw.Write(dataLength);
                                    bw.Write(soundData);
                                    bw.Close();
                                    fs.Close();
                                    Console.WriteLine("Saved wav sound data to " + wavFilename + "");
                                }
                                else
                                {
                                    Console.WriteLine("Resources of type " + recHeads[j].blockType + " are not supported!");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Not an Imperialism II .RSRC file: " + headerCode);
                        }
                    }
                    Console.WriteLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
