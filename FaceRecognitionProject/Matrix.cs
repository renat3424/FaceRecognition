using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace FaceRecognitionProject
{
    class M
    {
        List<string> imgPath;
        int imgWidth;
        int imgHeight;
        int imgSize;
        public M(List<string> imgPath, int imgWidth, int imgHeight)
        {
            this.imgPath = imgPath;
            this.imgHeight = imgHeight;
            this.imgWidth = imgWidth;
            this.imgSize = imgWidth * imgSize;

        }

        byte[,] Transpose(byte[][] array)
        {
            byte[,] arrayT = new byte[array[0].Length, array.Length];

            for (int i = 0; i < arrayT.GetLength(0); i++)
            {

                for (int j = 0; j < arrayT.GetLength(1); j++)
                {

                    arrayT[i, j] = array[j][i];
                }
            }
            return arrayT;

        }
        public byte[] ImageToArray(Image<Gray, byte> img)
        {
            byte[] array = new byte[img.Height * img.Width];
            for (int i = 0; i < array.Length; i++)
            {

                array[i] = img.Data[i / img.Height, i % img.Width, 0];

            }
            return array;

        }
        public byte[,] GetM()
        {


            byte[][] matrix = new byte[imgPath.Count][];


            int i = 0;
            foreach (string path in imgPath)
            {
                Mat img = CvInvoke.Imread(path, 0);
                CvInvoke.Resize(img, img, new Size(imgWidth, imgHeight));



                matrix[i] = ImageToArray(img.ToImage<Gray, byte>());
                i++;


            }

            return Transpose(matrix);
        }




    }
}
