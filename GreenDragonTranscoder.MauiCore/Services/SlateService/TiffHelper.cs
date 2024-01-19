using BitMiracle.LibTiff.Classic;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace GreenDragonTranscoder.MauiCore.Services.SlateService
{
    internal class TiffHelper
    {
        public static SKBitmap? DecodeTiff(string filePath)
        {
            // open a TIFF stored in the stream
            using var tifImg = Tiff.Open(filePath, "r");
            return _DecodeTiff(tifImg);
        }

        public static SKBitmap? DecodeTiff(Stream tiffStream)
        {
            // open a TIFF stored in the stream
            using Tiff tifImg = Tiff.ClientOpen("in-memory", "r", tiffStream, new TiffStream());
            return _DecodeTiff(tifImg);
        }

        // Convert a TIFF stream to a SKBitmap
        private static SKBitmap? _DecodeTiff(Tiff? tifImg)
        { 
            if (tifImg == null)
            {
                // Handle error: Unable to open the TIFF file
                return null;
            }

            // read the dimensions
            var width = tifImg.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            var height = tifImg.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

            var info = new SKImageInfo(width, height);

            // create the buffer that will hold the pixels
            var raster = new int[width * height];

            // get a pointer to the buffer, and give it to the bitmap
            var ptr = GCHandle.Alloc(raster, GCHandleType.Pinned);

            var bitmap = new SkiaSharp.SKBitmap();
            bitmap.InstallPixels(info, ptr.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) => ptr.Free(), null);
            
            // read the image into the memory buffer
            if (!tifImg.ReadRGBAImageOriented(width, height, raster, Orientation.TOPLEFT))
            {
                // not a valid TIF image.
                return null;
            }

            // swap the red and blue because SkiaSharp may differ from the tiff
            if (SKImageInfo.PlatformColorType == SKColorType.Bgra8888)
            {
                SKSwizzle.SwapRedBlue(ptr.AddrOfPinnedObject(), raster.Length);
            }

            return bitmap;
        }


    }
}
