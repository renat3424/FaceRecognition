using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FaceRecognitionProject
{
    class Algorithm
    {

        double[,] img;
        List<int> labels;
        List<string> targets;
        List<int> number;
        int width;
        int height;
        double quality;
        MathNet.Numerics.LinearAlgebra.Matrix<double> a;
        MathNet.Numerics.LinearAlgebra.Matrix<double> bases;
        MathNet.Numerics.LinearAlgebra.Matrix<double> u;
        MathNet.Numerics.LinearAlgebra.Matrix<double> eValues;
        MathNet.Numerics.LinearAlgebra.Matrix<double> v;
        Vector<double> vectorS;
        MathNet.Numerics.LinearAlgebra.Matrix<double> newCoord;
        double[] meanArr;
        double[,] Convert(byte[,] mat)
        {

            double[,] mat1 = new double[mat.GetLength(0), mat.GetLength(1)];
            for (int i = 0; i < mat.GetLength(0); i++)
            {

                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    mat1[i, j] = mat[i, j];
                }

            }
            return mat1;
        }

        double[,] Transpose(double[][] array)
        {
            double[,] arrayT = new double[array[0].Length, array.Length];

            for (int i = 0; i < arrayT.GetLength(0); i++)
            {

                for (int j = 0; j < arrayT.GetLength(1); j++)
                {

                    arrayT[i, j] = array[j][i];
                }
            }
            return arrayT;

        }

        double[] getMean(double[,] array)
        {
            double[] mean = new double[array.GetLength(0)];
            double S;

            for (int i = 0; i < array.GetLength(0); i++)
            {
                S = 0;

                for (int j = 0; j < array.GetLength(1); j++)
                {


                    S += array[i, j];

                }

                mean[i] = S / array.GetLength(1);

            }
            return mean;


        }

        double[] Convert1(byte[] array)
        {

            double[] array1 = new double[array.Length];

            for (int j = 0; j < array.Length; j++)
            {


                array1[j] = array[j];

            }

            return array1;
        }
        public Algorithm(byte[,] img, List<int> labels, List<string> targets, List<int> number, int width, int height, double quality)
        {
            this.img = Convert(img);
            this.labels = labels;
            this.targets = targets;
            this.number = number;
            this.width = width;
            this.height = height;
            this.quality = quality;
            double[][] meanMatr;

            meanMatr = new double[this.img.GetLength(1)][];

            meanArr = getMean(this.img);

            for (int i = 0; i < img.GetLength(1); i++)
            {
                meanMatr[i] = meanArr;

            }

            MathNet.Numerics.LinearAlgebra.Matrix<double> b = DenseMatrix.OfArray(Transpose(meanMatr));

            a = DenseMatrix.OfArray(this.img);
            a = a.Subtract(b);






        }


        public MathNet.Numerics.LinearAlgebra.Matrix<double> DimensionReduction()
        {

            var svd = a.Svd(true);
            u = svd.U;

            v = svd.VT;
            eValues = svd.W;
            vectorS = svd.S;
            int val = (int)Val();
            double[,] bases = new double[u.RowCount, val];

            for (int i = 0; i < bases.GetLength(0); i++)
            {


                for (int j = 0; j < bases.GetLength(1); j++)
                {


                    bases[i, j] = u[i, j];

                }



            }

            this.bases = DenseMatrix.OfArray(bases);
            newCoord = this.bases.Transpose().Multiply(a);

            return newCoord;

        }
        public Vector<double> NewCoordinates(Emgu.CV.Mat img)
        {

            Vector<double> imgVec = Vector<double>.Build.DenseOfArray(Convert1(ImageToArray(img.ToImage<Emgu.CV.Structure.Gray, byte>())));

            Vector<double> meanVec = Vector<double>.Build.DenseOfArray(meanArr);

            Vector<double> newMean = meanVec.Multiply(labels.Count).Add(imgVec).Divide(labels.Count + 1);


            imgVec = imgVec.Subtract(newMean);


            return bases.Transpose().Multiply(imgVec);


        }
        byte[] ImageToArray(Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> img)
        {
            byte[] array = new byte[img.Height * img.Width];
            for (int i = 0; i < array.Length; i++)
            {

                array[i] = img.Data[i / img.Height, i % img.Width, 0];

            }
            return array;

        }
        double[,] getSubMatrix(double[,] _a, int i_start, int i_end, int j_start, int j_end)
        {
            double[,] _nex = new double[i_end - i_start + 1, j_end - j_start + 1];
            for (int i = i_start, i_sub = 0; i <= i_end; i++, i_sub++)
            {
                for (int j = j_start, j_sub = 0; j <= j_end; j++, j_sub++)
                {
                    _nex[i_sub, j_sub] = _a[i, j];
                }
            }
            return _nex;
        }
        public string Recognize(Vector<double> newCoordinates)
        {

            int len = number.Count;

            int j = 0;
            List<double> distances = new List<double>();
            for (int i = 0; i < len; i++)
            {
                MathNet.Numerics.LinearAlgebra.Matrix<double> temporary = DenseMatrix.OfArray(getSubMatrix(newCoord.ToArray(), 0, newCoord.RowCount - 1, j, (j + number[i] - 1)));



                Vector<double> temporaryMean = temporary.RowSums().Divide(temporary.RowCount);

                j = j + number[i];
                double distance = newCoordinates.Subtract(temporaryMean).Norm(2);

                distances.Add(distance);



            }

            double position = distances.IndexOf(distances.Min());
            if (position < 10000)
            {
                return targets[(int)position];

            }
            else
            {
                return "Unknown";
            }

        }

        public double Val()
        {
            double sum = vectorS.Sum();
            double threshold = sum * quality / 100;

            double sumT = 0;
            double val = 0;
            foreach (double value in vectorS)
            {

                if (sumT < threshold)
                {
                    sumT += value;
                    val += 1;

                }
                else
                {
                    return val;
                }
            }
            return val;
        }


        public Emgu.CV.Mat GetImage(string path)
        {
            Emgu.CV.Mat img = Emgu.CV.CvInvoke.Imread(path, 0);

            Emgu.CV.CvInvoke.Resize(img, img, new System.Drawing.Size(width, height), 0, 0, Emgu.CV.CvEnum.Inter.Linear);

            return img;
        }




    }
}
