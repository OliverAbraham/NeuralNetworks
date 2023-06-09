﻿namespace NeuralNetwork
{
    /// <summary>
    /// Loads MNIST training data files. 
    public static class MnistTrainingDataLoader
    {
        #region ------------- Methods -------------------------------------------------------------

        /// <summary>
        /// Load a training data file, containing images.
        /// Every entry is a grayscale image of 28x28 pixels.
        /// </summary>
        public static byte[][] LoadImageFile(string filePath, int amount)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);

                int magicNum = binaryReader.ReadInt32();
                int imageCount = binaryReader.ReadInt32();
                int rowCount = binaryReader.ReadInt32();
                int columnsCount = binaryReader.ReadInt32();

                byte[][] images = new byte[amount][];

                for (int iter = 0; iter < images.Length; iter++)
                {
                    byte[] image = new byte[28 * 28];

                    for (int i = 0; i < image.Length; i++)
                    {
                        image[i] = binaryReader.ReadByte();
                    }

                    images[iter] = image;
                }

                return images;
            }
        }

        /// <summary>
        /// Loads a label file.
        /// Every entry is a byte, containing the digital value 0-9 for the digital representation of an image
        /// </summary>
        public static byte[] LoadLabelFile(string filePath, int amount)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);

                int magicNum = binaryReader.ReadInt32();
                int labelCount = binaryReader.ReadInt32();

                byte[] labels = new byte[amount];

                for (int i = 0; i < labels.Length; i++)
                {
                    byte label = binaryReader.ReadByte();
                    labels[i] = label;
                }

                return labels;
            }
        }

        //public static Bitmap ByteToBitmap(byte[] image, int width, int height)
        //{
        //    Bitmap bmp = new Bitmap(width, height);
        //    int pixelCount = 0;
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            bmp.SetPixel(x, y, Color.FromArgb(255, image[pixelCount], image[pixelCount], image[pixelCount]));
        //            pixelCount++;
        //        }
        //    }
        //
        //    return bmp;
        //}

        public static float[] ByteToFloat(byte[] image)
        {
            float[] img = new float[image.Length];

            for (int i = 0; i < image.Length; i++)
            {
                float prevVal = image[i];
                float newVal = prevVal * 1.0F / 255.0F;

                img[i] = (float)Math.Round(newVal, 2);
            }

            return img;
        }

        public static byte[] FloatToByte(float[] image)
        {
            byte[] img = new byte[image.Length];

            for (int i = 0; i < image.Length; i++)
            {
                float prevVal = image[i];
                float newVal = prevVal * 255.0F;

                img[i] = (byte)Math.Round(newVal, 2);
            }

            return img;
        }
        #endregion
    }
}
