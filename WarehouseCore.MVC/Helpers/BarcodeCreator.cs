using System.Drawing;
using ZXing;
using ZXing.Common;

namespace WarehouseCore.MVC.Helpers
{
    public class BarcodeCreator
    {
        public Bitmap GenerateBarcode(string content, BarcodeFormat format, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 0
                }
            };
            var bitmap = writer.Write(content);
            return bitmap;
        }
    }
}