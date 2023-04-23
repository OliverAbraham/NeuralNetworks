using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace UI
{
    internal class BitmapLibrary
    {
        public static BitmapSource CreateImageFromBytes(byte[] bytes)
        {
            PixelFormat pf = PixelFormats.Gray8; // means one byte per pixel
            int width = 28;
            int height = 28;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            var bitmapSource = BitmapSource.Create(width, height, 96, 96, pf, null, bytes, rawStride);
            return bitmapSource;
        }

        public static byte[] CreateBytesFromImage(BitmapSource image)
        {
            var bytes = new byte[28 * 28];
            image.CopyPixels(bytes, 28, 0);
            return bytes;
        }

        public static byte[] ConvertBitmapPointsToListOfBytes(RenderTargetBitmap bitmap)
        {
            var width  = Convert.ToInt32(bitmap.Width);
            var height = Convert.ToInt32(bitmap.Height);

            var result = new byte[width*height];
            bitmap.CopyPixels(result, width, 0);
            return result;
        }

        public static RenderTargetBitmap GetBitmapFromCanvas(Canvas canvas, int width, int height)
        {
            var renderBitmap = new RenderTargetBitmap(width, height, 1/300, 1/300, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(canvas);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(canvas.Width, canvas.Height)));
            }
            visual.Transform = new ScaleTransform(width / canvas.ActualWidth, height / canvas.ActualHeight);
            renderBitmap.Render(visual);
            return renderBitmap;
        }

        public static System.Drawing.Bitmap ConvertColorBitmapToGrayscale(System.Drawing.Bitmap bitmap) 
        {
            var result = new System.Drawing.Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++) 
                {
                    var grayColor = ConvertRgbValueToGrayscaleValue(bitmap.GetPixel(x, y));
                    result.SetPixel(x, y, grayColor);
                }
            }
            return result;
        }

        public static byte[] ConvertColorBitmapTo1dimensionalBytesArray(System.Drawing.Bitmap bitmap) 
        {
            var result = new byte[bitmap.Width * bitmap.Height];
            int i=0;

            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++) 
                    result[i++] = (byte)ConvertRgbValueToGrayscaleByte(bitmap.GetPixel(x, y));

            return result;
        }

        public static System.Drawing.Color ConvertRgbValueToGrayscaleValue(System.Drawing.Color color) 
        {
            var level = (byte)((color.R + color.G + color.B) / 3);
            var result = System.Drawing.Color.FromArgb(level, level, level);
            return result;
        }

        public static byte ConvertRgbValueToGrayscaleByte(System.Drawing.Color color) 
        {
            var level = (byte)((color.R + color.G + color.B) / 3);
            return level;
        }

        public static byte[] ConvertGrayscaleBitmapTo1dimensionalBytesArray(System.Drawing.Bitmap bitmap) 
        {
            var result = new byte[bitmap.Width * bitmap.Height];
            var i = 0;

            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    result[i++] = (byte)(bitmap.GetPixel(x, y).R / 255);
            
            return result;
        }

        public static byte[,] ConvertGrayscaleBitmapTo2dimensionalBytesArray(System.Drawing.Bitmap bitmap) 
        {
            var result = new byte[bitmap.Width, bitmap.Height];

            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    result[x, y] = (byte)(bitmap.GetPixel(x, y).R / 255);
            
            return result;
        }

        public static double[,] ConvertGrayscaleBitmapTo2dimensionalDoublesArray(System.Drawing.Bitmap bitmap) 
        {
            var result = new double[bitmap.Width, bitmap.Height];
            
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    result[x, y] = (double)bitmap.GetPixel(x, y).R / 255;
            
            return result;
        }

        public static System.Drawing.Bitmap LoadBitmapFromFile(string filename)
        {
            return new System.Drawing.Bitmap(filename);
        }

        public static System.Drawing.Bitmap ConvertRenderTargetBitmapToSystemDrawingBitmap(RenderTargetBitmap renderTargetBitmap)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                encoder.Save(fs);
                fs.Flush();
                fs.Position = 0;

                var bitmap = new System.Drawing.Bitmap(fs);
                return bitmap;
            }
        }

        public static System.Drawing.Bitmap ConvertRenderTargetBitmapToSystemDrawingBitmapOld(RenderTargetBitmap renderTargetBitmap)
        {
            SaveBitmapToFile(renderTargetBitmap, @"handwriting.bmp");
            var drawingBitmap = LoadBitmapFromFile(@"handwriting.bmp");
            return drawingBitmap;
        }

        public static void SaveBitmapToMemoryStream(RenderTargetBitmap renderBitmap, string path)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                //BitmapEncoder encoder = new JpegBitmapEncoder();
                //BitmapEncoder encoder = new TiffBitmapEncoder();
                //BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(fs);
                fs.Flush();
                fs.Close();
            }
        }    

        public static void SaveBitmapToFile(RenderTargetBitmap renderBitmap, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                //BitmapEncoder encoder = new JpegBitmapEncoder();
                //BitmapEncoder encoder = new TiffBitmapEncoder();
                //BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(fs);
                fs.Flush();
                fs.Close();
            }
        }
    }
}
