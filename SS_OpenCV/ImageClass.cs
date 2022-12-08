using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SS_OpenCV
{
    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;
                for (y = 0; y < img.Height; y++)
                {
                    for (x = 0; x < img.Width; x++)
                    {
                        //retrieve 3 colour components
                        blue = dataPtr[0];
                        green = dataPtr[1];
                        red = dataPtr[2];

                        // store in the image
                        dataPtr[0] = (byte)(255.0 - blue);
                        dataPtr[1] = (byte)(255.0 - green);
                        dataPtr[2] = (byte)(255.0 - red);

                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    dataPtr += padding;
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components
                            red = dataPtr[2];


                            dataPtr[0] = red;
                            dataPtr[1] = red;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        private static byte CalcBrightAndLimit(int bright, double contrast, byte channel)
        {
            double value = (contrast * channel) + bright;

            if (value > 255.0) return (byte)255.0;
            if (value < 0.0) return (byte)0.0;

            return (byte)Math.Round(value);
        }
        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];


                            dataPtr[0] = CalcBrightAndLimit(bright, contrast, blue);
                            dataPtr[1] = CalcBrightAndLimit(bright, contrast, green);
                            dataPtr[2] = CalcBrightAndLimit(bright, contrast, red);

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Rotation1(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                MIplImage copy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrCpy = (byte*)copy.ImageData.ToPointer();

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;
                double h2 = height / 2.0;
                double w2 = width / 2.0;
                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);
                double newX;
                double newY;
                byte* newPixel;
                double aux;

                for (y = 0; y < height; y++)
                {
                    aux = (h2 - y);
                    for (x = 0; x < width; x++)
                    {

                        newX = Math.Round((x - w2) * cos - aux * sin + w2);
                        newY = Math.Round(h2 - (x - w2) * sin - aux * cos);

                        if (newX >= width || newY >= height || newX < 0 || newY < 0)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {
                            newPixel = (byte*)(dataPtrCpy + (int)newY * widthstep + (int)newX * nChan);
                            dataPtr[0] = newPixel[0];
                            dataPtr[1] = newPixel[1];
                            dataPtr[2] = newPixel[2];
                        }

                        dataPtr += nChan;
                    }
                    dataPtr += padding;

                }
            }
            Console.WriteLine("BOAS");
        }


        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                MIplImage copy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrCpy = (byte*)copy.ImageData.ToPointer();

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;
                double h2 = height / 2.0;
                double w2 = width / 2.0;
                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);
                double newX;
                double newY;
                byte* newPixel;
                double aux;

                for (y = 0; y < height; y++)
                {
                    aux = (h2 - y);
                    for (x = 0; x < width; x++)
                    {

                        newX = Math.Round((x - w2) * cos - aux * sin + w2);
                        newY = Math.Round(h2 - (x - w2) * sin - aux * cos);

                        if (newX >= width || newY >= height || newX < 0 || newY < 0)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {
                            newPixel = (byte*)(dataPtrCpy + (int)newY * widthstep + (int)newX * nChan);
                            dataPtr[0] = newPixel[0];
                            dataPtr[1] = newPixel[1];
                            dataPtr[2] = newPixel[2];
                        }

                        dataPtr += nChan;
                    }
                    dataPtr += padding;

                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                MIplImage copy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrCpy = (byte*)copy.ImageData.ToPointer();

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;
                int newX, newY;
                byte* newPixel;
                double w2, h2;
                w2 = img.Width / 2.0;
                h2 = img.Height / 2.0;

                for (y = 0; y < height; y++)
                {
                    newY = y - dy;
                    for (x = 0; x < width; x++)
                    {

                        newX = -dx + x;

                        if (newX >= width || newY >= height || newX < 0 || newY < 0)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {
                            newPixel = (byte*)(dataPtrCpy + newY * widthstep + newX * nChan);
                            dataPtr[0] = newPixel[0];
                            dataPtr[1] = newPixel[1];
                            dataPtr[2] = newPixel[2];
                        }

                        dataPtr += nChan;
                    }
                    dataPtr += padding;

                }
            }
        }

        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                MIplImage copy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrCpy = (byte*)copy.ImageData.ToPointer();

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;
                byte* newPixel;
                double w2, h2, newX, newY;
                w2 = img.Width / 2.0;
                h2 = img.Height / 2.0;

                for (y = 0; y < height; y++)
                {
                    newY = height - (h2 - y) / scaleFactor;
                    for (x = 0; x < width; x++)
                    {
                        newX = (x - w2) / scaleFactor + width;

                        if (newX >= width || newY >= height || newX < 0 || newY < 0)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {
                            newPixel = (byte*)(dataPtrCpy + (int)newY * widthstep + (int)newX * nChan);
                            dataPtr[0] = newPixel[0];
                            dataPtr[1] = newPixel[1];
                            dataPtr[2] = newPixel[2];
                        }

                        dataPtr += nChan;
                    }
                    dataPtr += padding;

                }
            }
        }


        public static Image<Bgr, byte> ImageWithPadding(Image<Bgr, byte> img)
        {
            Image<Bgr, Byte> imgBigger = new Image<Bgr, byte>(img.Width + 2, img.Height + 2);
            CvInvoke.CopyMakeBorder(img, imgBigger, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Replicate);

            return imgBigger;
        }

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                MIplImage copy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrCpy = (byte*)copy.ImageData.ToPointer();

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;
                byte* newPixel;
                double w2, h2, newX, newY;
                w2 = img.Width / 2;
                h2 = img.Height / 2;

                for (y = 0; y < height; y++)
                {
                    newY = Math.Round(centerY - (h2 - y) / scaleFactor);
                    for (x = 0; x < width; x++)
                    {
                        newX = Math.Round((x - w2) / scaleFactor + centerX);

                        if (newX >= width || newY >= height || newX < 0 || newY < 0)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {
                            newPixel = (byte*)(dataPtrCpy + (int)newY * widthstep + (int)newX * nChan);
                            dataPtr[0] = newPixel[0];
                            dataPtr[1] = newPixel[1];
                            dataPtr[2] = newPixel[2];
                        }

                        dataPtr += nChan;
                    }
                    dataPtr += padding;

                }
            }
        }


        public static void Mean(Image<Bgr, byte> imgCopy, Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right


                MIplImage m = img.MIplImage;
                MIplImage copy = imgCopy.MIplImage;
                byte* dataPtrOri = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrdest = (byte*)copy.ImageData.ToPointer();

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;

                dataPtrOri += nChan + widthstep;
                dataPtrdest += nChan + widthstep;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {

                        dataPtrdest[0] = (byte)Math.Round((dataPtrOri[0] + (dataPtrOri + nChan)[0] + (dataPtrOri - nChan)[0] + (dataPtrOri + widthstep)[0]
                            + (dataPtrOri + widthstep + nChan)[0] + (dataPtrOri + widthstep - nChan)[0] + (dataPtrOri - widthstep)[0]
                            + (dataPtrOri - widthstep + nChan)[0] + (dataPtrOri - widthstep - nChan)[0]) / 9.0);

                        dataPtrdest[1] = (byte)Math.Round((dataPtrOri[1] + (dataPtrOri + nChan)[1] + (dataPtrOri - nChan)[1] + (dataPtrOri + widthstep)[1]
                            + (dataPtrOri + widthstep + nChan)[1] + (dataPtrOri + widthstep - nChan)[1] + (dataPtrOri - widthstep)[1]
                            + (dataPtrOri - widthstep + nChan)[1] + (dataPtrOri - widthstep - nChan)[1]) / 9.0);

                        dataPtrdest[2] = (byte)Math.Round((dataPtrOri[2] + (dataPtrOri + nChan)[2] + (dataPtrOri - nChan)[2] + (dataPtrOri + widthstep)[2]
                            + (dataPtrOri + widthstep + nChan)[2] + (dataPtrOri + widthstep - nChan)[2] + (dataPtrOri - widthstep)[2]
                            + (dataPtrOri - widthstep + nChan)[2] + (dataPtrOri - widthstep - nChan)[2]) / 9.0);

                        dataPtrdest += nChan;
                        dataPtrOri += nChan;
                    }
                    dataPtrdest += 2 * nChan + padding;
                    dataPtrOri += 2 * nChan + padding;

                }

                dataPtrOri = (byte*)m.ImageData.ToPointer() + nChan;
                dataPtrdest = (byte*)copy.ImageData.ToPointer() + nChan;

                for (x = 1; x < width - 1; x++)
                {
                    dataPtrdest[0] = (byte)Math.Round((2 * dataPtrOri[0] + 2 * (dataPtrOri + nChan)[0] + 2 * (dataPtrOri - nChan)[0] + (dataPtrOri + widthstep)[0]
                            + (dataPtrOri + widthstep + nChan)[0] + (dataPtrOri + widthstep - nChan)[0]) / 9.0);

                    dataPtrdest[1] = (byte)Math.Round((2 * dataPtrOri[1] + 2 * (dataPtrOri + nChan)[1] + 2 * (dataPtrOri - nChan)[1] + (dataPtrOri + widthstep)[1]
                        + (dataPtrOri + widthstep + nChan)[1] + (dataPtrOri + widthstep - nChan)[1]) / 9.0);

                    dataPtrdest[2] = (byte)Math.Round((2 * dataPtrOri[2] + 2 * (dataPtrOri + nChan)[2] + 2 * (dataPtrOri - nChan)[2] + (dataPtrOri + widthstep)[2]
                        + (dataPtrOri + widthstep + nChan)[2] + (dataPtrOri + widthstep - nChan)[2]) / 9.0);

                    dataPtrdest += nChan;
                    dataPtrOri += nChan;
                }

                dataPtrOri = (byte*)m.ImageData.ToPointer() + (height - 1) * widthstep + nChan;
                dataPtrdest = (byte*)copy.ImageData.ToPointer() + (height - 1) * widthstep + nChan;

                for (x = 1; x < width - 1; x++)
                {
                    dataPtrdest[0] = (byte)Math.Round((2 * dataPtrOri[0] + 2 * (dataPtrOri + nChan)[0] + 2 * (dataPtrOri - nChan)[0] + (dataPtrOri - widthstep)[0]
                            + (dataPtrOri - widthstep + nChan)[0] + (dataPtrOri - widthstep - nChan)[0]) / 9.0);

                    dataPtrdest[1] = (byte)Math.Round((2 * dataPtrOri[1] + 2 * (dataPtrOri + nChan)[1] + 2 * (dataPtrOri - nChan)[1] + (dataPtrOri - widthstep)[1]
                        + (dataPtrOri - widthstep + nChan)[1] + (dataPtrOri - widthstep - nChan)[1]) / 9.0);

                    dataPtrdest[2] = (byte)Math.Round((2 * dataPtrOri[2] + 2 * (dataPtrOri + nChan)[2] + 2 * (dataPtrOri - nChan)[2] + (dataPtrOri - widthstep)[2]
                        + (dataPtrOri - widthstep + nChan)[2] + (dataPtrOri - widthstep - nChan)[2]) / 9.0);

                    dataPtrdest += nChan;
                    dataPtrOri += nChan;
                }

                dataPtrOri = (byte*)m.ImageData.ToPointer() + widthstep;
                dataPtrdest = (byte*)copy.ImageData.ToPointer() + widthstep;

                for (y = 1; y < height - 1; y++)
                {
                    dataPtrdest[0] = (byte)Math.Round(((2 * dataPtrOri[0]) + (dataPtrOri + nChan)[0] + (2 * (dataPtrOri + widthstep)[0])
                            + (dataPtrOri + widthstep + nChan)[0] + (2 * (dataPtrOri - widthstep)[0])
                            + (dataPtrOri - widthstep + nChan)[0]) / 9.0);

                    dataPtrdest[1] = (byte)Math.Round(((2 * dataPtrOri[1]) + (dataPtrOri + nChan)[1] + (2 * (dataPtrOri + widthstep)[1])
                            + (dataPtrOri + widthstep + nChan)[1] + (2 * (dataPtrOri - widthstep)[1])
                            + (dataPtrOri - widthstep + nChan)[1]) / 9.0);

                    dataPtrdest[2] = (byte)Math.Round(((2 * dataPtrOri[2]) + (dataPtrOri + nChan)[2] + (2 * (dataPtrOri + widthstep)[2])
                            + (dataPtrOri + widthstep + nChan)[2] + (2 * (dataPtrOri - widthstep)[2])
                            + (dataPtrOri - widthstep + nChan)[2]) / 9.0);

                    dataPtrdest += widthstep;
                    dataPtrOri += widthstep;
                }

                dataPtrOri = (byte*)m.ImageData.ToPointer() + widthstep + (width - 1) * nChan;
                dataPtrdest = (byte*)copy.ImageData.ToPointer() + widthstep + (width - 1) * nChan;


                for (y = 1; y < height - 1; y++)
                {
                    dataPtrdest[0] = (byte)Math.Round(((2 * dataPtrOri[0]) + (dataPtrOri - nChan)[0] + (2 * (dataPtrOri + widthstep)[0])
                            + (dataPtrOri + widthstep - nChan)[0] + (2 * (dataPtrOri - widthstep)[0])
                            + (dataPtrOri - widthstep - nChan)[0]) / 9.0);

                    dataPtrdest[1] = (byte)Math.Round(((2 * dataPtrOri[1]) + (dataPtrOri - nChan)[1] + (2 * (dataPtrOri + widthstep)[1])
                            + (dataPtrOri + widthstep - nChan)[1] + (2 * (dataPtrOri - widthstep)[1])
                            + (dataPtrOri - widthstep - nChan)[1]) / 9.0);

                    dataPtrdest[2] = (byte)Math.Round(((2 * dataPtrOri[2]) + (dataPtrOri - nChan)[2] + (2 * (dataPtrOri + widthstep)[2])
                            + (dataPtrOri + widthstep - nChan)[2] + (2 * (dataPtrOri - widthstep)[2])
                            + (dataPtrOri - widthstep - nChan)[2]) / 9.0);

                    dataPtrdest += widthstep;
                    dataPtrOri += widthstep;
                }

                dataPtrOri = (byte*)m.ImageData.ToPointer();
                dataPtrdest = (byte*)copy.ImageData.ToPointer();

                dataPtrdest[0] = (byte)Math.Round(((4 * dataPtrOri[0]) + 2 * (dataPtrOri + nChan)[0] + 2 * (dataPtrOri + widthstep)[0]
                            + (dataPtrOri + widthstep + nChan)[0]) / 9.0);
                dataPtrdest[1] = (byte)Math.Round(((4 * dataPtrOri[1]) + 2 * (dataPtrOri + nChan)[1] + 2 * (dataPtrOri + widthstep)[1]
                            + (dataPtrOri + widthstep + nChan)[1]) / 9.0);
                dataPtrdest[2] = (byte)Math.Round(((4 * dataPtrOri[2]) + 2 * (dataPtrOri + nChan)[2] + 2 * (dataPtrOri + widthstep)[2]
                            + (dataPtrOri + widthstep + nChan)[2]) / 9.0);

                byte* temp1 = dataPtrOri + (width - 1) * nChan;
                byte* temp2 = dataPtrdest + (width - 1) * nChan;

                temp2[0] = (byte)Math.Round(((4 * temp1[0]) + 2 * (temp1 - nChan)[0] + 2 * (temp1 + widthstep)[0]
                            + (temp1 + widthstep - nChan)[0]) / 9.0);
                temp2[1] = (byte)Math.Round(((4 * temp1[1]) + 2 * (temp1 - nChan)[1] + 2 * (temp1 + widthstep)[1]
                            + (temp1 + widthstep - nChan)[1]) / 9.0);
                temp2[2] = (byte)Math.Round(((4 * temp1[2]) + 2 * (temp1 - nChan)[2] + 2 * (temp1 + widthstep)[2]
                            + (temp1 + widthstep - nChan)[2]) / 9.0);

                temp1 = dataPtrOri + (height - 1) * widthstep;
                temp2 = dataPtrdest + (height - 1) * widthstep;

                temp2[0] = (byte)Math.Round(((4 * temp1[0]) + 2 * (temp1 + nChan)[0] + 2 * (temp1 - widthstep)[0]
                            + (temp1 - widthstep + nChan)[0]) / 9.0);
                temp2[1] = (byte)Math.Round(((4 * temp1[1]) + 2 * (temp1 + nChan)[1] + 2 * (temp1 - widthstep)[1]
                            + (temp1 - widthstep + nChan)[1]) / 9.0);
                temp2[2] = (byte)Math.Round(((4 * temp1[2]) + 2 * (temp1 + nChan)[2] + 2 * (temp1 - widthstep)[2]
                            + (temp1 - widthstep + nChan)[2]) / 9.0);

                temp1 = dataPtrOri + (height - 1) * widthstep + (width - 1) * nChan;
                temp2 = dataPtrdest + (height - 1) * widthstep + (width - 1) * nChan;

                temp2[0] = (byte)Math.Round(((4 * temp1[0]) + 2 * (temp1 - nChan)[0] + 2 * (temp1 - widthstep)[0]
                            + (temp1 - widthstep - nChan)[0]) / 9.0);
                temp2[1] = (byte)Math.Round(((4 * temp1[1]) + 2 * (temp1 - nChan)[1] + 2 * (temp1 - widthstep)[1]
                            + (temp1 - widthstep - nChan)[1]) / 9.0);
                temp2[2] = (byte)Math.Round(((4 * temp1[2]) + 2 * (temp1 - nChan)[2] + 2 * (temp1 - widthstep)[2]
                            + (temp1 - widthstep - nChan)[2]) / 9.0);
            }
        }


        public static void Mean_solutionB(Image<Bgr, byte> imgCopy, Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                Image<Bgr, Byte> imgBigger = new Image<Bgr, byte>(img.Width + 2, img.Height + 2);
                CvInvoke.CopyMakeBorder(img, imgBigger, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Replicate);
                MIplImage origin = imgBigger.MIplImage;
                MIplImage dest = imgCopy.MIplImage;
                byte* dataPtrOrigin = (byte*)origin.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrDest = (byte*)dest.ImageData.ToPointer();

                int width = origin.Width;
                int height = origin.Height;
                int nChan = origin.NChannels; // number of channels - 3
                int widthstep = origin.WidthStep;
                int padding = widthstep - nChan * width;
                int paddingDest = dest.WidthStep - dest.NChannels * dest.Width;
                int x, y, b, g, r;

                dataPtrOrigin += nChan + widthstep;

                for (y = 1; y < height - 1; y++)
                {

                    b = dataPtrOrigin[0] + (dataPtrOrigin + nChan)[0] + (dataPtrOrigin - nChan)[0] + (dataPtrOrigin + widthstep)[0]
                           + (dataPtrOrigin + widthstep + nChan)[0] + (dataPtrOrigin + widthstep - nChan)[0] + (dataPtrOrigin - widthstep)[0]
                           + (dataPtrOrigin - widthstep + nChan)[0] + (dataPtrOrigin - widthstep - nChan)[0];

                    g = dataPtrOrigin[1] + (dataPtrOrigin + nChan)[1] + (dataPtrOrigin - nChan)[1] + (dataPtrOrigin + widthstep)[1]
                         + (dataPtrOrigin + widthstep + nChan)[1] + (dataPtrOrigin + widthstep - nChan)[1] + (dataPtrOrigin - widthstep)[1]
                         + (dataPtrOrigin - widthstep + nChan)[1] + (dataPtrOrigin - widthstep - nChan)[1];

                    r = dataPtrOrigin[2] + (dataPtrOrigin + nChan)[2] + (dataPtrOrigin - nChan)[2] + (dataPtrOrigin + widthstep)[2]
                         + (dataPtrOrigin + widthstep + nChan)[2] + (dataPtrOrigin + widthstep - nChan)[2] + (dataPtrOrigin - widthstep)[2]
                         + (dataPtrOrigin - widthstep + nChan)[2] + (dataPtrOrigin - widthstep - nChan)[2];


                    dataPtrDest[0] = (byte)Math.Round(b / 9.0);
                    dataPtrDest[1] = (byte)Math.Round(g / 9.0);
                    dataPtrDest[2] = (byte)Math.Round(r / 9.0);

                    dataPtrOrigin += nChan;
                    dataPtrDest += nChan;

                    for (x = 2; x < width - 1; x++)
                    {

                        b = b - (dataPtrOrigin - 2 * nChan)[0] - (dataPtrOrigin - 2 * nChan - widthstep)[0] - (dataPtrOrigin - 2 * nChan + widthstep)[0] + (dataPtrOrigin + nChan - widthstep)[0] + (dataPtrOrigin + nChan + widthstep)[0] + (dataPtrOrigin + nChan)[0];
                        g = g - (dataPtrOrigin - 2 * nChan)[1] - (dataPtrOrigin - 2 * nChan - widthstep)[1] - (dataPtrOrigin - 2 * nChan + widthstep)[1] + (dataPtrOrigin + nChan - widthstep)[1] + (dataPtrOrigin + nChan + widthstep)[0] + (dataPtrOrigin + nChan)[1];
                        r = r - (dataPtrOrigin - 2 * nChan)[2] - (dataPtrOrigin - 2 * nChan - widthstep)[2] - (dataPtrOrigin - 2 * nChan + widthstep)[2] + (dataPtrOrigin + nChan - widthstep)[2] + (dataPtrOrigin + nChan + widthstep)[2] + (dataPtrOrigin + nChan)[2];


                        dataPtrDest[0] = (byte)Math.Round(b / 9.0);
                        dataPtrDest[1] = (byte)Math.Round(g / 9.0);
                        dataPtrDest[2] = (byte)Math.Round(r / 9.0);

                        dataPtrOrigin += nChan;
                        dataPtrDest += nChan;
                    }
                    dataPtrDest += paddingDest;
                    dataPtrOrigin += 2 * nChan + padding;
                }
            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight, float offset)
        {
            unsafe
            {
                MIplImage copy = imgCopy.MIplImage;
                MIplImage m = img.MIplImage;
                byte* orig = (byte*)copy.ImageData.ToPointer();
                byte* dest = (byte*)m.ImageData.ToPointer();
                int r, g, b;

                int width = copy.Width;
                int height = copy.Height;
                int nChan = copy.NChannels; // number of channels - 3
                int widthstep = copy.WidthStep;
                int padding = widthstep - nChan * width;
                int x, y;

                orig += widthstep + nChan;
                dest += widthstep + nChan;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        b = (int)Math.Round((((orig - widthstep - nChan)[0] * matrix[0, 0] + (orig - widthstep)[0] * matrix[1, 0] + (orig - widthstep + nChan)[0] * matrix[2, 0] +
                                                 (orig - nChan)[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + (orig + nChan)[0] * matrix[2, 1] +
                                                 (orig + widthstep - nChan)[0] * matrix[0, 2] + (orig + widthstep)[0] * matrix[1, 2] + (orig + widthstep + nChan)[0] * matrix[2, 2]) / matrixWeight) + offset);


                        g = (int)Math.Round((((orig - widthstep - nChan)[1] * matrix[0, 0] + (orig - widthstep)[1] * matrix[1, 0] + (orig - widthstep + nChan)[1] * matrix[2, 0] +
                                                  (orig - nChan)[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + (orig + nChan)[1] * matrix[2, 1] +
                                                  (orig + widthstep - nChan)[1] * matrix[0, 2] + (orig + widthstep)[1] * matrix[1, 2] + (orig + widthstep + nChan)[1] * matrix[2, 2]) / matrixWeight) + offset);


                        r = (int)Math.Round((((orig - widthstep - nChan)[2] * matrix[0, 0] + (orig - widthstep)[2] * matrix[1, 0] + (orig - widthstep + nChan)[2] * matrix[2, 0] +
                                                (orig - nChan)[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + (orig + nChan)[2] * matrix[2, 1] +
                                                (orig + widthstep - nChan)[2] * matrix[0, 2] + (orig + widthstep)[2] * matrix[1, 2] + (orig + widthstep + nChan)[2] * matrix[2, 2]) / matrixWeight) + offset);


                        if (b < 0) b = 0;
                        if (b > 255) b = 255;

                        if (g < 0) g = 0;
                        if (g > 255) g = 255;

                        if (r < 0) r = 0;
                        if (r > 255) r = 255;


                        dest[0] = (byte)b;
                        dest[1] = (byte)g;
                        dest[2] = (byte)r;

                        orig += nChan;
                        dest += nChan;
                    }
                    orig += padding + 2 * nChan;
                    dest += padding + 2 * nChan;
                }

                orig = (byte*)copy.ImageData.ToPointer() + nChan;
                dest = (byte*)m.ImageData.ToPointer() + nChan;

                for (x = 1; x < width - 1; x++)
                {
                    b = (int)Math.Round((((orig - nChan)[0] * matrix[0, 0] + orig[0] * matrix[1, 0] + (orig + nChan)[0] * matrix[2, 0] +
                                             (orig - nChan)[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + (orig + nChan)[0] * matrix[2, 1] +
                                             (orig + widthstep - nChan)[0] * matrix[0, 2] + (orig + widthstep)[0] * matrix[1, 2] + (orig + widthstep + nChan)[0] * matrix[2, 2]) / matrixWeight) + offset);

                    g = (int)Math.Round((((orig - nChan)[1] * matrix[0, 0] + orig[1] * matrix[1, 0] + (orig + nChan)[1] * matrix[2, 0] +
                                              (orig - nChan)[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + (orig + nChan)[1] * matrix[2, 1] +
                                              (orig + widthstep - nChan)[1] * matrix[0, 2] + (orig + widthstep)[1] * matrix[1, 2] + (orig + widthstep + nChan)[1] * matrix[2, 2]) / matrixWeight) + offset);

                    r = (int)Math.Round((((orig - nChan)[2] * matrix[0, 0] + orig[2] * matrix[1, 0] + (orig + nChan)[2] * matrix[2, 0] +
                                            (orig - nChan)[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + (orig + nChan)[2] * matrix[2, 1] +
                                            (orig + widthstep - nChan)[2] * matrix[0, 2] + (orig + widthstep)[2] * matrix[1, 2] + (orig + widthstep + nChan)[2] * matrix[2, 2]) / matrixWeight) + offset);

                    if (b < 0) b = 0;
                    if (b > 255) b = 255;

                    if (g < 0) g = 0;
                    if (g > 255) g = 255;

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;

                    dest[0] = (byte)b;
                    dest[1] = (byte)g;
                    dest[2] = (byte)r;

                    orig += nChan;
                    dest += nChan;
                }


                orig = (byte*)copy.ImageData.ToPointer() + nChan + widthstep * (height - 1);
                dest = (byte*)m.ImageData.ToPointer() + nChan + widthstep * (height - 1);


                for (x = 1; x < width - 1; x++)
                {
                    b = (int)Math.Round((((orig - widthstep - nChan)[0] * matrix[0, 0] + (orig - widthstep)[0] * matrix[1, 0] + (orig - widthstep + nChan)[0] * matrix[2, 0] +
                                                (orig - nChan)[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + (orig + nChan)[0] * matrix[2, 1] +
                                                (orig - nChan)[0] * matrix[0, 2] + orig[0] * matrix[1, 2] + (orig + nChan)[0] * matrix[2, 2]) / matrixWeight) + offset);


                    g = (int)Math.Round((((orig - widthstep - nChan)[1] * matrix[0, 0] + (orig - widthstep)[1] * matrix[1, 0] + (orig - widthstep + nChan)[1] * matrix[2, 0] +
                                                (orig - nChan)[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + (orig + nChan)[1] * matrix[2, 1] +
                                                (orig - nChan)[1] * matrix[0, 2] + orig[1] * matrix[1, 2] + (orig + nChan)[1] * matrix[2, 2]) / matrixWeight) + offset);


                    r = (int)Math.Round((((orig - widthstep - nChan)[2] * matrix[0, 0] + (orig - widthstep)[2] * matrix[1, 0] + (orig - widthstep + nChan)[2] * matrix[2, 0] +
                                            (orig - nChan)[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + (orig + nChan)[2] * matrix[2, 1] +
                                            (orig - nChan)[2] * matrix[0, 2] + orig[2] * matrix[1, 2] + (orig + nChan)[2] * matrix[2, 2]) / matrixWeight) + offset);
                    if (b < 0) b = 0;
                    if (b > 255) b = 255;

                    if (g < 0) g = 0;
                    if (g > 255) g = 255;

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;

                    dest[0] = (byte)b;
                    dest[1] = (byte)g;
                    dest[2] = (byte)r;

                    orig += nChan;
                    dest += nChan;
                }

                orig = (byte*)copy.ImageData.ToPointer() + widthstep + nChan * (width - 1);
                dest = (byte*)m.ImageData.ToPointer() + widthstep + nChan * (width - 1);

                for (y = 1; y < height - 1; y++)
                {
                    b = (int)Math.Round((((orig - widthstep - nChan)[0] * matrix[0, 0] + (orig - widthstep)[0] * matrix[1, 0] + (orig - widthstep)[0] * matrix[2, 0] +
                                                (orig - nChan)[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + orig[0] * matrix[2, 1] +
                                                (orig + widthstep - nChan)[0] * matrix[0, 2] + (orig + widthstep)[0] * matrix[1, 2] + (orig + widthstep)[0] * matrix[2, 2]) / matrixWeight) + offset);


                    g = (int)Math.Round((((orig - widthstep - nChan)[1] * matrix[0, 0] + (orig - widthstep)[1] * matrix[1, 0] + (orig - widthstep)[1] * matrix[2, 0] +
                                                (orig - nChan)[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + orig[1] * matrix[2, 1] +
                                                (orig + widthstep - nChan)[1] * matrix[0, 2] + (orig + widthstep)[1] * matrix[1, 2] + (orig + widthstep)[1] * matrix[2, 2]) / matrixWeight) + offset);


                    r = (int)Math.Round((((orig - widthstep - nChan)[2] * matrix[0, 0] + (orig - widthstep)[2] * matrix[1, 0] + (orig - widthstep)[2] * matrix[2, 0] +
                                            (orig - nChan)[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + orig[2] * matrix[2, 1] +
                                            (orig + widthstep - nChan)[2] * matrix[0, 2] + (orig + widthstep)[2] * matrix[1, 2] + (orig + widthstep)[2] * matrix[2, 2]) / matrixWeight) + offset);
                    if (b < 0) b = 0;
                    if (b > 255) b = 255;

                    if (g < 0) g = 0;
                    if (g > 255) g = 255;

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;


                    dest[0] = (byte)b;
                    dest[1] = (byte)g;
                    dest[2] = (byte)r;

                    orig += widthstep;
                    dest += widthstep;
                }

                orig = (byte*)copy.ImageData.ToPointer() + widthstep;
                dest = (byte*)m.ImageData.ToPointer() + widthstep;

                for (y = 1; y < height - 1; y++)
                {
                    b = (int)Math.Round((((orig - widthstep)[0] * matrix[0, 0] + (orig - widthstep)[0] * matrix[1, 0] + (orig - widthstep + nChan)[0] * matrix[2, 0] + orig[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + (orig + nChan)[0] * matrix[2, 1] +
                                             (orig + widthstep)[0] * matrix[0, 2] + (orig + widthstep)[0] * matrix[1, 2] + (orig + widthstep + nChan)[0] * matrix[2, 2]) / matrixWeight) + offset);

                    g = (int)Math.Round((((orig - widthstep)[1] * matrix[0, 0] + (orig - widthstep)[1] * matrix[1, 0] + (orig - widthstep + nChan)[1] * matrix[2, 0] + orig[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + (orig + nChan)[1] * matrix[2, 1] +
                                              (orig + widthstep)[1] * matrix[0, 2] + (orig + widthstep)[1] * matrix[1, 2] + (orig + widthstep + nChan)[1] * matrix[2, 2]) / matrixWeight) + offset);

                    r = (int)Math.Round((((orig - widthstep)[2] * matrix[0, 0] + (orig - widthstep)[2] * matrix[1, 0] + (orig - widthstep + nChan)[2] * matrix[2, 0] + orig[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + (orig + nChan)[2] * matrix[2, 1] +
                                                   (orig + widthstep)[2] * matrix[0, 2] + (orig + widthstep)[2] * matrix[1, 2] + (orig + widthstep + nChan)[2] * matrix[2, 2]) / matrixWeight) + offset);
                    if (b < 0) b = 0;
                    if (b > 255) b = 255;

                    if (g < 0) g = 0;
                    if (g > 255) g = 255;

                    if (r < 0) r = 0;
                    if (r > 255) r = 255;

                    dest[0] = (byte)b;
                    dest[1] = (byte)g;
                    dest[2] = (byte)r;

                    orig += widthstep;
                    dest += widthstep;
                }

                orig = (byte*)copy.ImageData.ToPointer() + widthstep * (height - 1) + nChan * (width - 1);
                dest = (byte*)m.ImageData.ToPointer() + widthstep * (height - 1) + nChan * (width - 1);


                b = (int)Math.Round((((orig - widthstep - nChan)[0] * matrix[0, 0] + (orig - widthstep)[0] * matrix[1, 0] + (orig - widthstep)[0] * matrix[2, 0] +
                                            (orig - nChan)[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + orig[0] * matrix[2, 1] +
                                            (orig - nChan)[0] * matrix[0, 2] + orig[0] * matrix[1, 2] + orig[0] * matrix[2, 2]) / matrixWeight) + offset);


                g = (int)Math.Round((((orig - widthstep - nChan)[1] * matrix[0, 0] + (orig - widthstep)[1] * matrix[1, 0] + (orig - widthstep)[1] * matrix[2, 0] +
                                            (orig - nChan)[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + orig[1] * matrix[2, 1] +
                                            (orig - nChan)[1] * matrix[0, 2] + orig[1] * matrix[1, 2] + orig[1] * matrix[2, 2]) / matrixWeight) + offset);

                r = (int)Math.Round((((orig - widthstep - nChan)[2] * matrix[0, 0] + (orig - widthstep)[2] * matrix[1, 0] + (orig - widthstep)[2] * matrix[2, 0] +
                                        (orig - nChan)[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + orig[2] * matrix[2, 1] +
                                        (orig - nChan)[2] * matrix[0, 2] + orig[2] * matrix[1, 2] + orig[2] * matrix[2, 2]) / matrixWeight) + offset);
                if (b < 0) b = 0;
                if (b > 255) b = 255;

                if (g < 0) g = 0;
                if (g > 255) g = 255;

                if (r < 0) r = 0;
                if (r > 255) r = 255;


                dest[0] = (byte)b;
                dest[1] = (byte)g;
                dest[2] = (byte)r;

                orig = (byte*)copy.ImageData.ToPointer() + nChan * (width - 1);
                dest = (byte*)m.ImageData.ToPointer() + nChan * (width - 1);


                b = (int)Math.Round((((orig - nChan)[0] * matrix[0, 0] + orig[0] * matrix[1, 0] + orig[0] * matrix[2, 0] +
                                         (orig - nChan)[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + orig[0] * matrix[2, 1] +
                                         (orig + widthstep - nChan)[0] * matrix[0, 2] + (orig + widthstep)[0] * matrix[1, 2] + (orig + widthstep)[0] * matrix[2, 2]) / matrixWeight) + offset);
                if (b < 0) b = 0;
                if (b > 255) b = 255;

                g = (int)Math.Round((((orig - nChan)[1] * matrix[0, 0] + orig[1] * matrix[1, 0] + orig[1] * matrix[2, 0] +
                                          (orig - nChan)[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + orig[1] * matrix[2, 1] +
                                          (orig + widthstep - nChan)[1] * matrix[0, 2] + (orig + widthstep)[1] * matrix[1, 2] + (orig + widthstep)[1] * matrix[2, 2]) / matrixWeight) + offset);
                if (g < 0) g = 0;
                if (g > 255) g = 255;

                r = (int)Math.Round((((orig - nChan)[2] * matrix[0, 0] + orig[2] * matrix[1, 0] + orig[2] * matrix[2, 0] +
                                        (orig - nChan)[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + orig[2] * matrix[2, 1] +
                                        (orig + widthstep - nChan)[2] * matrix[0, 2] + (orig + widthstep)[2] * matrix[1, 2] + (orig + widthstep)[2] * matrix[2, 2]) / matrixWeight) + offset);
                if (r < 0) r = 0;
                if (r > 255) r = 255;

                dest[0] = (byte)b;
                dest[1] = (byte)g;
                dest[2] = (byte)r;

                orig = (byte*)copy.ImageData.ToPointer();
                dest = (byte*)m.ImageData.ToPointer();


                b = (int)Math.Round(((orig[0] * matrix[0, 0] + orig[0] * matrix[1, 0] + (orig + nChan)[0] * matrix[2, 0] + orig[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + (orig + nChan)[0] * matrix[2, 1] +
                                        (orig + widthstep)[0] * matrix[0, 2] + (orig + widthstep)[0] * matrix[1, 2] + (orig + widthstep + nChan)[0] * matrix[2, 2]) / matrixWeight) + offset);
                if (b < 0) b = 0;
                if (b > 255) b = 255;

                g = (int)Math.Round(((orig[1] * matrix[0, 0] + orig[1] * matrix[1, 0] + (orig + nChan)[1] * matrix[2, 0] + orig[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + (orig + nChan)[1] * matrix[2, 1] +
                                         (orig + widthstep)[1] * matrix[0, 2] + (orig + widthstep)[1] * matrix[1, 2] + (orig + widthstep + nChan)[1] * matrix[2, 2]) / matrixWeight) + offset);
                if (g < 0) g = 0;
                if (g > 255) g = 255;

                r = (int)Math.Round(((orig[2] * matrix[0, 0] + orig[2] * matrix[1, 0] + (orig + nChan)[2] * matrix[2, 0] + orig[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + (orig + nChan)[2] * matrix[2, 1] +
                                       (orig + widthstep)[2] * matrix[0, 2] + (orig + widthstep)[2] * matrix[1, 2] + (orig + widthstep + nChan)[2] * matrix[2, 2]) / matrixWeight) + offset);
                if (r < 0) r = 0;
                if (r > 255) r = 255;

                dest[0] = (byte)b;
                dest[1] = (byte)g;
                dest[2] = (byte)r;

                orig = (byte*)copy.ImageData.ToPointer() + widthstep * (height - 1);
                dest = (byte*)m.ImageData.ToPointer() + widthstep * (height - 1);

                b = (int)Math.Round((((orig - widthstep)[0] * matrix[0, 0] + (orig - widthstep)[0] * matrix[1, 0] + (orig - widthstep + nChan)[0] * matrix[2, 0] + orig[0] * matrix[0, 1] + orig[0] * matrix[1, 1] + (orig + nChan)[0] * matrix[2, 1] +
                                          orig[0] * matrix[0, 2] + orig[0] * matrix[1, 2] + (orig + nChan)[0] * matrix[2, 2]) / matrixWeight) + offset);
                if (b < 0) b = 0;
                if (b > 255) b = 255;

                g = (int)Math.Round((((orig - widthstep)[1] * matrix[0, 0] + (orig - widthstep)[1] * matrix[1, 0] + (orig - widthstep + nChan)[1] * matrix[2, 0] + orig[1] * matrix[0, 1] + orig[1] * matrix[1, 1] + (orig + nChan)[1] * matrix[2, 1] +
                                           orig[1] * matrix[0, 2] + orig[1] * matrix[1, 2] + (orig + nChan)[1] * matrix[2, 2]) / matrixWeight) + offset);
                if (g < 0) g = 0;
                if (g > 255) g = 255;

                r = (int)Math.Round((((orig - widthstep)[2] * matrix[0, 0] + (orig - widthstep)[2] * matrix[1, 0] + (orig - widthstep + nChan)[2] * matrix[2, 0] + orig[2] * matrix[0, 1] + orig[2] * matrix[1, 1] + (orig + nChan)[2] * matrix[2, 1] +
                                            orig[2] * matrix[0, 2] + orig[2] * matrix[1, 2] + (orig + nChan)[2] * matrix[2, 2]) / matrixWeight) + offset);
                if (r < 0) r = 0;
                if (r > 255) r = 255;

                dest[0] = (byte)b;
                dest[1] = (byte)g;
                dest[2] = (byte)r;
            }
        }


        public static void Sobel(Image<Bgr, byte> imgCopy, Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                Image<Bgr, Byte> imgBigger = new Image<Bgr, byte>(img.Width + 2, img.Height + 2);
                CvInvoke.CopyMakeBorder(img, imgBigger, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Replicate);
                MIplImage origin = imgBigger.MIplImage;
                MIplImage dest = imgCopy.MIplImage;
                byte* dataPtrOrigin = (byte*)origin.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrDest = (byte*)dest.ImageData.ToPointer();

                int width = origin.Width;
                int height = origin.Height;
                int nChan = origin.NChannels; // number of channels - 3
                int widthstep = origin.WidthStep;
                int padding = widthstep - nChan * width;
                int paddingDest = dest.WidthStep - dest.NChannels * dest.Width;
                int x, y, sx, sy, b, g, r;

                dataPtrOrigin += nChan + widthstep;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        sx = (int)Math.Round((double)(((dataPtrOrigin - widthstep - nChan)[0] + 2 * (dataPtrOrigin - nChan)[0] + (dataPtrOrigin + widthstep - nChan)[0]) -
                                                      ((dataPtrOrigin - widthstep + nChan)[0] + 2 * (dataPtrOrigin + nChan)[0] + (dataPtrOrigin + widthstep + nChan)[0])));

                        sy = (int)Math.Round((double)(((dataPtrOrigin + widthstep - nChan)[0] + 2 * (dataPtrOrigin + widthstep)[0] + (dataPtrOrigin + widthstep + nChan)[0]) -
                                                      ((dataPtrOrigin - widthstep - nChan)[0] + 2 * (dataPtrOrigin - widthstep)[0] + (dataPtrOrigin - widthstep + nChan)[0])));

                        b = Math.Abs(sx) + Math.Abs(sy);

                        sx = (int)Math.Round((double)(((dataPtrOrigin - widthstep - nChan)[1] + 2 * (dataPtrOrigin - nChan)[1] + (dataPtrOrigin + widthstep - nChan)[1]) -
                                                      ((dataPtrOrigin - widthstep + nChan)[1] + 2 * (dataPtrOrigin + nChan)[1] + (dataPtrOrigin + widthstep + nChan)[1])));

                        sy = (int)Math.Round((double)(((dataPtrOrigin + widthstep - nChan)[1] + 2 * (dataPtrOrigin + widthstep)[1] + (dataPtrOrigin + widthstep + nChan)[1]) -
                                                      ((dataPtrOrigin - widthstep - nChan)[1] + 2 * (dataPtrOrigin - widthstep)[1] + (dataPtrOrigin - widthstep + nChan)[1])));

                        g = Math.Abs(sx) + Math.Abs(sy);

                        sx = (int)Math.Round((double)(((dataPtrOrigin - widthstep - nChan)[2] + 2 * (dataPtrOrigin - nChan)[2] + (dataPtrOrigin + widthstep - nChan)[2]) -
                                                      ((dataPtrOrigin - widthstep + nChan)[2] + 2 * (dataPtrOrigin + nChan)[2] + (dataPtrOrigin + widthstep + nChan)[2])));

                        sy = (int)Math.Round((double)(((dataPtrOrigin + widthstep - nChan)[2] + 2 * (dataPtrOrigin + widthstep)[2] + (dataPtrOrigin + widthstep + nChan)[2]) -
                                                      ((dataPtrOrigin - widthstep - nChan)[2] + 2 * (dataPtrOrigin - widthstep)[2] + (dataPtrOrigin - widthstep + nChan)[2])));

                        r = Math.Abs(sx) + Math.Abs(sy);

                        if (b > 255)
                            b = 255;

                        if (g > 255)
                            g = 255;

                        if (r > 255)
                            r = 255;

                        dataPtrDest[0] = (byte)b;
                        dataPtrDest[1] = (byte)g;
                        dataPtrDest[2] = (byte)r;

                        dataPtrOrigin += nChan;
                        dataPtrDest += nChan;
                    }
                    dataPtrDest += paddingDest;
                    dataPtrOrigin += 2 * nChan + padding;
                }

            }

        }


        public static void Diferentiation(Image<Bgr, byte> imgCopy, Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                Image<Bgr, Byte> imgBigger = new Image<Bgr, byte>(img.Width + 2, img.Height + 2);
                CvInvoke.CopyMakeBorder(img, imgBigger, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Replicate);
                MIplImage origin = imgBigger.MIplImage;
                MIplImage dest = imgCopy.MIplImage;
                byte* dataPtrOrigin = (byte*)origin.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrDest = (byte*)dest.ImageData.ToPointer();

                int width = origin.Width;
                int height = origin.Height;
                int nChan = origin.NChannels; // number of channels - 3
                int widthstep = origin.WidthStep;
                int padding = widthstep - nChan * width;
                int paddingDest = dest.WidthStep - dest.NChannels * dest.Width;
                int x, y, b, g, r;

                dataPtrOrigin += nChan + widthstep;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {

                        b = Math.Abs(dataPtrOrigin[0] - (dataPtrOrigin + nChan)[0]) + Math.Abs(dataPtrOrigin[0] - (dataPtrOrigin + widthstep)[0]);

                        g = Math.Abs(dataPtrOrigin[1] - (dataPtrOrigin + nChan)[1]) + Math.Abs(dataPtrOrigin[1] - (dataPtrOrigin + widthstep)[1]);

                        r = Math.Abs(dataPtrOrigin[2] - (dataPtrOrigin + nChan)[2]) + Math.Abs(dataPtrOrigin[2] - (dataPtrOrigin + widthstep)[2]);


                        if (b > 255)
                            b = 255;

                        if (g > 255)
                            g = 255;

                        if (r > 255)
                            r = 255;

                        dataPtrDest[0] = (byte)b;
                        dataPtrDest[1] = (byte)g;
                        dataPtrDest[2] = (byte)r;

                        dataPtrOrigin += nChan;
                        dataPtrDest += nChan;
                    }
                    dataPtrDest += paddingDest;
                    dataPtrOrigin += 2 * nChan + padding;
                }

            }
        }


        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                CvInvoke.MedianBlur(imgCopy, img, 3);
            }
        }

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int widthStep = m.WidthStep;
                int nChan = m.NChannels; // number of channels = 3
                int x, y;

                int[] hist = new int[256];
                int grayValue;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        grayValue = (int)Math.Round(((dataPtr + x * nChan + y * widthStep)[0] + (dataPtr + x * nChan + y * widthStep)[1] + (dataPtr + x * nChan + y * widthStep)[2]) / 3.0);
                        hist[grayValue]++;
                    }
                }

                return hist;
            }
        }

        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int widthStep = m.WidthStep;
                int nChan = m.NChannels; // number of channels = 3
                int x, y;

                int[,] hist = new int[3, 256];
                int r, g, b;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        r = (dataPtr + x * nChan + y * widthStep)[2];
                        g = (dataPtr + x * nChan + y * widthStep)[1];
                        b = (dataPtr + x * nChan + y * widthStep)[0];

                        hist[2, r]++;
                        hist[1, g]++;
                        hist[0, b]++;
                    }
                }

                return hist;
            }
        }

        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int widthStep = m.WidthStep;
                int nChan = m.NChannels; // number of channels = 3
                int x, y;

                int[,] hist = new int[4, 256];
                int r, g, b, gray;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {

                        gray = (int)Math.Round(((dataPtr + x * nChan + y * widthStep)[0] + (dataPtr + x * nChan + y * widthStep)[1] + (dataPtr + x * nChan + y * widthStep)[2]) / 3.0);
                        hist[0, gray]++;

                        r = (dataPtr + x * nChan + y * widthStep)[2];
                        g = (dataPtr + x * nChan + y * widthStep)[1];
                        b = (dataPtr + x * nChan + y * widthStep)[0];

                        hist[3, r]++;
                        hist[2, g]++;
                        hist[1, b]++;
                    }
                }
                return hist;
            }
        }

        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int widthstep = m.WidthStep;
                int nChan = m.NChannels; // number of channels = 3

                int padding = widthstep - nChan * width;
                int x, y, result;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        result = (int)Math.Round((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0);

                        if (result > threshold)
                        {
                            dataPtr[0] = (byte)255;
                            dataPtr[1] = (byte)255;
                            dataPtr[2] = (byte)255;
                        }
                        else
                        {
                            dataPtr[0] = (byte)0;
                            dataPtr[1] = (byte)0;
                            dataPtr[2] = (byte)0;
                        }
                        dataPtr += nChan;
                    }
                    dataPtr += padding;
                }
            }
        }
        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {

                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int widthstep = m.WidthStep;
                int nChan = m.NChannels; // number of channels = 3

                int padding = widthstep - nChan * width;

            }
        }


        public static int Max(int val1, int val2)
        {
            if (val1 > val2)
                return val1;

            return val2;
        }

        public static int Min(int val1, int val2)
        {
            if (val2 == 0 || (val1 < val2 && val1 !=0) )
                return val1;


            return val2;
        }


        public static int[] MinOrdem(int val1, int val2)
        {
            if (val2 == 0 || (val1 < val2 && val1 != 0))
                return new int[] { val1, val2 };


            return new int[] { val2, val1 };
        }


        public static void ComponentesLigadosClassico(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage copy = img.MIplImage;


                byte* dataPtrCopy = (byte*)copy.ImageData.ToPointer(); // imagem de trabalho

                int width = copy.Width;
                int height = copy.Height;
                int widthstep = copy.WidthStep;
                int nChan = copy.NChannels; // number of channels = 3

                int padding = widthstep - nChan * width;


                int x, y;

                int[,] matrix = new int[height + 2, width + 2];
                int objectsNumber = 1, result, threshold = 17;


                int[] mins;

                int parent, son, temp;

                int[] collisions = new int[1000];

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        result = (int)Math.Round((dataPtrCopy[0] + dataPtrCopy[1] + dataPtrCopy[2]) / 3.0);

                        if (result > threshold)
                        {
                            dataPtrCopy[0] = (byte)255;
                            dataPtrCopy[1] = (byte)255;
                            dataPtrCopy[2] = (byte)255;
                        }
                        else
                        {
                            dataPtrCopy[0] = (byte)0;
                            dataPtrCopy[1] = (byte)0;
                            dataPtrCopy[2] = (byte)0;
                        }

                        if (dataPtrCopy[0] == 0)
                        {
                            if (Max(matrix[y, x + 1], matrix[y + 1, x]) == 0)
                            {
                                matrix[y + 1, x + 1] = objectsNumber;
                                collisions[objectsNumber] = objectsNumber;
                                objectsNumber++;

                            }
                            else
                            {
                                mins = MinOrdem(matrix[y, x + 1], matrix[y + 1, x]);
                                matrix[y + 1, x + 1] = mins[0];
                                if (matrix[y, x + 1] != 0 && matrix[y + 1, x] != 0)
                                {
                                    parent = mins[0];
                                    son = mins[1];
                                    while (collisions[son] != son)
                                    {
                                        temp = collisions[son];
                                        collisions[son] = parent;
                                        son = temp;
                                    }
                                    collisions[son] = parent;
                                }
                            }
                        }

                        dataPtrCopy += nChan;
                    }
                    dataPtrCopy += padding;
                }


                for (int i = 1; i < objectsNumber; i++)
                {
                    if (collisions[i] != i)
                    {
                        collisions[i] = collisions[collisions[i]];
                    }
                }

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        matrix[x + 1, y + 1] = collisions[matrix[x + 1, y + 1]];

                        dataPtrCopy += nChan;
                    }
                    dataPtrCopy += padding;
                }

                TableForm.ShowTable(collisions, "colisoes");

                TableForm.ShowTable(matrix, "componentes ligados");


                int[,] objectData = new int[objectsNumber+1,5];
                int obj;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (matrix[y+1,x+1] !=0 )
                        {
                            obj = matrix[y + 1, x + 1];
                            objectData[obj, 0] ++;
                            objectData[obj, 1] += x;
                            objectData[obj, 2] += y;
                        }
                    }
                }

                TableForm.ShowTable(objectData, "informacao objetos");

                int[,] found = new int[1000,2];
                int foundIndex = 0;

                for (x = 1; x < objectsNumber + 1; x++)
                {
                    if (objectData[x, 0] == 0) continue;

                    objectData[x, 3] = (int)Math.Round( (float)objectData[x, 1] / (float)objectData[x, 0]);

                    objectData[x, 4] = (int)Math.Round((float)objectData[x, 2] / (float)objectData[x, 0]);


                    for (y = x-1;y >=0;y--)
                    {
                        if (objectData[x, 3] == objectData[y, 3] && objectData[x, 4] == objectData[y, 4])
                        {
                            found[foundIndex, 0] = x;
                            found[foundIndex, 1] = y;
                            foundIndex++;
                            break;
                        }
                    }
                }

                TableForm.ShowTable(found, "QR");
            }
        }
        //com binarizacao
        public static void ComponentesLigadosIterativo(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage copy = img.MIplImage;


                byte* dataPtrCopy = (byte*)copy.ImageData.ToPointer(); // imagem de trabalho

                int width = copy.Width;
                int height = copy.Height;
                int widthstep = copy.WidthStep;
                int nChan = copy.NChannels; // number of channels = 3

                int padding = widthstep - nChan * width;

                int x, y;
                Boolean isChanged = true;

                int[,] matrix = new int[height + 2, width + 2];
                int objectsNumber = 1, result, iteCounter = 1, threshold = 10, min;


                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        result = (int)Math.Round((dataPtrCopy[0] + dataPtrCopy[1] + dataPtrCopy[2]) / 3.0);

                        if (result > threshold)
                        {
                            dataPtrCopy[0] = (byte)255;
                            dataPtrCopy[1] = (byte)255;
                            dataPtrCopy[2] = (byte)255;
                        }
                        else
                        {
                            dataPtrCopy[0] = (byte)0;
                            dataPtrCopy[1] = (byte)0;
                            dataPtrCopy[2] = (byte)0;
                        }

                        if (dataPtrCopy[0] == 0)
                        {
                            if (Max(matrix[y, x + 1], matrix[y + 1, x]) == 0)
                            {
                                matrix[y + 1, x + 1] = objectsNumber;
                                objectsNumber++;

                            }
                            else
                            {
                                matrix[y + 1, x + 1] = Min(matrix[y, x + 1], matrix[y + 1, x]);

                            }
                        }
                        dataPtrCopy += nChan;
                    }
                    dataPtrCopy += padding;

                }

                while (isChanged)
                {
                    isChanged = false;

                    if (iteCounter % 2 == 0)
                    {

                        for (y = 0; y < height; y++)
                        {
                            for (x = 0; x < width; x++)
                            {

                                if (dataPtrCopy[0] == 0 && Max(matrix[y, x + 1], matrix[y + 1, x]) != 0)
                                {
                                    min = Min(matrix[y, x + 1], matrix[y + 1, x]);
                                    if (min < matrix[y + 1, x + 1])
                                    {
                                        matrix[y + 1, x + 1] = min;
                                        isChanged = true;
                                    }

                                }
                                dataPtrCopy += nChan;
                            }
                            dataPtrCopy += padding;
                        }
                    }
                    else
                    {
                        for (y = width - 1; y >= 0; y--)
                        {
                            for (x = height - 1; x >= 0; x--)
                            {
                                if (dataPtrCopy[0] == 0 && Max(matrix[y + 1, x + 2], matrix[y + 2, x + 1]) != 0)
                                {
                                    min = Min(matrix[y + 1, x + 2], matrix[y + 2, x + 1]);
                                    if (min < matrix[y + 1, x + 1])
                                    {
                                        matrix[y + 1, x + 1] = min;
                                        isChanged = true;
                                    }
                                }

                                dataPtrCopy -= nChan;
                            }
                            dataPtrCopy -= padding;
                        }

                    }
                    iteCounter++;
                }

                TableForm.ShowTable(matrix, "objetos");
            }
        }


        /// QR code reader
        /// </summary>
        /// <param name="img"> imagem de trabalho </param>
        /// <param name="imgCopy">imagem original </param>
        /// <param name="level">nivel de dificuldade</param>
        /// <param name="Center_x">centro x do Qr code</param>
        /// <param name="Center_y">centro x do Qr code</param>
        /// <param name="Width">largura do Qr code</param>
        /// <param name="Height">altura do Qr code</param>
        /// <param name="Rotation">rotação do Qr code</param>
        /// <param name="BinaryOut">String contendo o Qr code lido em binário</param>
        /// <param name="UL_x_out">centro x do posicionador UL</param>
        /// <param name="UL_y_out">centro y do posicionador UL</param>
        /// <param name="UR_x_out">centro x do posicionador UR</param>
        /// <param name="UR_y_out">centro y do posicionador UR</param>
        /// <param name="LL_x_out">centro x do posicionador LL</param>
        /// <param name="LL_y_out">centro y do posicionador LL</param>
        public static void QRCodeReader(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int level,
            out int Center_x, out int Center_y, out int Width, out int Height, out float Rotation, out string BinaryOut,
            out int UL_x_out, out int UL_y_out, out int UR_x_out, out int UR_y_out, out int LL_x_out, out int LL_y_out)
        {
            Center_x = 0;
            Center_y = 0;
            Width = 0;
            Height = 0;
            Rotation = 0;
            BinaryOut = "";

            UL_x_out = 0;
            UL_y_out = 0;
            UR_x_out = 0;
            UR_y_out = 0;
            LL_x_out = 0;
            LL_y_out = 0;

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                MIplImage origin = imgCopy.MIplImage;
                MIplImage copy = img.MIplImage;

                byte* dataPtrOrigin = (byte*)origin.ImageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)origin.ImageData.ToPointer(); // imagem de trabalho

                int width = copy.Width;
                int height = copy.Height;
                int widthstep = copy.WidthStep;
                int nChan = copy.NChannels; // number of channels = 3

                int padding = widthstep - nChan * width;

                int paddingOrigin = origin.WidthStep - origin.NChannels * origin.Width;
                int x, y, result;

                int[,] matrix = new int[height + 2, width + 2];
                int objectsNumber = 1;
                

                
                int[] collisions = new int[1000];

                int min;
                Boolean isChanged = true;
                int threshold = 17;
                int iteCounter = 1;


                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        result = (int)Math.Round((dataPtrCopy[0] + dataPtrCopy[1] + dataPtrCopy[2]) / 3.0);

                        if (result > threshold)
                        {
                            dataPtrCopy[0] = (byte)255;
                            dataPtrCopy[1] = (byte)255;
                            dataPtrCopy[2] = (byte)255;
                        }
                        else
                        {
                            dataPtrCopy[0] = (byte)0;
                            dataPtrCopy[1] = (byte)0;
                            dataPtrCopy[2] = (byte)0;
                        }

                        if (dataPtrCopy[0] == 0)
                        {
                            if (Max(matrix[y, x + 1], matrix[y + 1, x]) == 0)
                            {
                                matrix[y + 1, x + 1] = objectsNumber;
                                objectsNumber++;

                            }
                            else
                            {
                                matrix[y + 1, x + 1] = Min(matrix[y, x + 1], matrix[y + 1, x]);

                            }
                        }
                        dataPtrCopy += nChan;
                    }
                    dataPtrCopy += padding;

                }

                while (isChanged)
                {
                    isChanged = false;

                    if (iteCounter % 2 == 0)
                    {

                        for (y = 0; y < height; y++)
                        {
                            for (x = 0; x < width; x++)
                            {

                                if (dataPtrCopy[0] == 0 && Max(matrix[y, x + 1], matrix[y + 1, x]) != 0)
                                {
                                    min = Min(matrix[y, x + 1], matrix[y + 1, x]);
                                    if (min < matrix[y + 1, x + 1])
                                    {
                                        matrix[y + 1, x + 1] = min;
                                        isChanged = true;
                                    }

                                }
                                dataPtrCopy += nChan;
                            }
                            dataPtrCopy += padding;
                        }
                    }
                    else
                    {
                        for (y = width - 1; y >= 0; y--)
                        {
                            for (x = height - 1; x >= 0; x--)
                            {
                                if (dataPtrCopy[0] == 0 && Max(matrix[y + 1, x + 2], matrix[y + 2, x + 1]) != 0)
                                {
                                    min = Min(matrix[y + 1, x + 2], matrix[y + 2, x + 1]);
                                    if (min < matrix[y + 1, x + 1])
                                    {
                                        matrix[y + 1, x + 1] = min;
                                        isChanged = true;
                                    }
                                }

                                dataPtrCopy -= nChan;
                            }
                            dataPtrCopy -= padding;
                        }

                    }
                    iteCounter++;
                }
                //TableForm.ShowTable(collisions, "colisoes");
                //TableForm.ShowTable(matrix, "componentes ligados");


                int[,] objectData = new int[objectsNumber + 1, 5];
                int obj;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (matrix[y + 1, x + 1] != 0)
                        {
                            obj = matrix[y + 1, x + 1];
                            objectData[obj, 0]++;
                            objectData[obj, 1] += x;
                            objectData[obj, 2] += y;
                        }
                    }
                }

                //TableForm.ShowTable(objectData, "informacao objetos");

                int[,] alignmentBlocks = new int[3, 3];
                int foundIndex = 0;
                

                for (x = 1; x < objectsNumber + 1; x++)
                {
                    if (objectData[x, 0] == 0) continue;

                    objectData[x, 3] = (int)Math.Round((float)objectData[x, 1] / (float)objectData[x, 0]);
                    objectData[x, 4] = (int)Math.Round((float)objectData[x, 2] / (float)objectData[x, 0]);

                    for (y = x - 1; y >= 0; y--)
                    {
                        if (objectData[x, 3] == objectData[y, 3] && objectData[x, 4] == objectData[y, 4])
                        {
                            alignmentBlocks[foundIndex, 0] = objectData[x, 3];
                            alignmentBlocks[foundIndex, 1] = objectData[x, 4];
                            alignmentBlocks[foundIndex++, 2] = y;
                            break;
                        }
                    }
                }


                getHeightAndWidth(alignmentBlocks, matrix, out Height, out Width);


                findCenterAndAlignment(alignmentBlocks, out Center_x, out Center_y, out Rotation, out UL_x_out, 
                    out UL_y_out, out UR_x_out, out UR_y_out, out LL_x_out, out LL_y_out);

                Rotation1(img, imgCopy, Rotation);


            }
        }





        public static void findCenterAndAlignment(int[,] alignmentBlocks, out int Center_x, out int Center_y, out float Rotation,
            out int UL_x_out, out int UL_y_out, out int UR_x_out, out int UR_y_out, out int LL_x_out, out int LL_y_out)
        {
            double dist01 = dist(alignmentBlocks[0, 0], alignmentBlocks[0, 1], alignmentBlocks[1, 0], alignmentBlocks[1, 1]);
            double dist02 = dist(alignmentBlocks[0, 0], alignmentBlocks[0, 1], alignmentBlocks[2, 0], alignmentBlocks[2, 1]);
            double dist12 = dist(alignmentBlocks[1, 0], alignmentBlocks[1, 1], alignmentBlocks[2, 0], alignmentBlocks[2, 1]);

            double maxDist = Math.Max(Math.Max(dist01, dist02), dist12);
            double cX, cY;

            if(maxDist == dist01)
            {
                UL_x_out = alignmentBlocks[2, 0];
                UL_y_out = alignmentBlocks[2, 1];
                cX = Math.Round((alignmentBlocks[0, 0] + alignmentBlocks[1, 0]) / 2.0);
                cY = Math.Round((alignmentBlocks[0, 1] + alignmentBlocks[1, 1]) / 2.0);
            }
            else if (maxDist == dist02)
            {
                UL_x_out = alignmentBlocks[1, 0];
                UL_y_out = alignmentBlocks[1, 1];
                cX = Math.Round((alignmentBlocks[0, 0] + alignmentBlocks[2, 0]) / 2.0);
                cY = Math.Round((alignmentBlocks[0, 1] + alignmentBlocks[2, 1]) / 2.0);
            }
            else
            {
                UL_x_out = alignmentBlocks[0, 0];
                UL_y_out = alignmentBlocks[0, 1];
                cX = Math.Round((alignmentBlocks[1, 0] + alignmentBlocks[2, 0]) / 2.0);
                cY = Math.Round((alignmentBlocks[1, 1] + alignmentBlocks[2, 1]) / 2.0);
            }


            //if angle is in 2nd or 3rd quadrant add offset
            int offset = cX > UL_x_out ? -180 : 0;
            Rotation = 135-(float)(180 / Math.PI) * (float)Math.Atan((cY - UL_y_out) / (UL_x_out - cX)) + offset;


            Center_x = (int)cX;
            Center_y = (int)cY;

            UR_x_out = 0;
            UR_y_out = 0;

            LL_x_out = 0;
            LL_y_out = 0;


        }


        public static void getHeightAndWidth(int[,] alignmentBlocks, int[,] matrix, out int height, out int width)
        {
            int lowestX = 0, highestX = 0, lowestY = 0, highestY = 0;
            int x, y;

            for (y = 0; y < matrix.GetLength(0); y++)
                for (x = 0; x < matrix.GetLength(1); x++)
                    if (matrix[y, x] == alignmentBlocks[0, 2] || matrix[y, x] == alignmentBlocks[1, 2] || matrix[y, x] == alignmentBlocks[2, 2]) {
                        lowestY = y;
                        goto nextLoop1;
                    }

            nextLoop1:

            for (y = matrix.GetLength(0) - 1; y >= 0; y--)
                for (x = 0; x < matrix.GetLength(1); x++)
                    if (matrix[y, x] == alignmentBlocks[0, 2] || matrix[y, x] == alignmentBlocks[1, 2] || matrix[y, x] == alignmentBlocks[2, 2])
                    {
                        highestY = y;
                        goto nextLoop2;

                    }

            nextLoop2:

            for (x = 0; x < matrix.GetLength(1); x++)
                for (y = 0; y < matrix.GetLength(0); y++)
                    if (matrix[y, x] == alignmentBlocks[0, 2] || matrix[y, x] == alignmentBlocks[1, 2] || matrix[y, x] == alignmentBlocks[2, 2]) {
                        lowestX = x;
                        goto nextLoop3;
                    }
            nextLoop3:

            for (x = matrix.GetLength(1)-1; x >= 0; x--) { 
                for (y = 0; y < matrix.GetLength(0); y++)
                {
                    if (matrix[y, x] == alignmentBlocks[0, 2] || matrix[y, x] == alignmentBlocks[1, 2] || matrix[y, x] == alignmentBlocks[2, 2])
                    {
                        highestX = x;
                        goto end;
                    }
                }
            }

        end:

            Console.WriteLine(lowestX);
            Console.WriteLine(highestX);
            Console.WriteLine(lowestY);
            Console.WriteLine(highestY);
            height = highestY - lowestY;
            width = highestX - lowestX;

        }



        public static double dist(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

        }





    }

}
