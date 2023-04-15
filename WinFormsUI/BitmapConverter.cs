using System.Drawing;

namespace WinFormsUI
{
    static class BitmapConverter
    {
        #region ------------- Methods -------------------------------------------------------------
        public static Bitmap ByteToBitmap(byte[] image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            int pixelCount = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(255, image[pixelCount], image[pixelCount], image[pixelCount]));
                    pixelCount++;
                }
            }

            return bmp;
        }
        #endregion
    }
}
