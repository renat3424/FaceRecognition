using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;

using Emgu.CV.Structure;
using Emgu.Util;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Emgu.CV.CvEnum;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Diagnostics;
using System.IO;

namespace FaceRecognitionProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private Capture _capture = null;

        public static string path = "C:\\Users\\Ренат\\Downloads\\facerecognition\\FaceRecognitionProject\\FaceRecognitionProject\\faces1.xml";
        CascadeClassifier cascadeClassifier = new CascadeClassifier(path);
        public MainWindow()
        {
            InitializeComponent();
          
        }

        Data data = new Data(8);
        private void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
        {
            
            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (imageFrame != null)
                {
                    if (faceDetectionEnabled)
                    {
                        Mat grayImage = new Mat();
                        CvInvoke.CvtColor(imageFrame, grayImage, ColorConversion.Bgr2Gray);
                        CvInvoke.EqualizeHist(grayImage, grayImage);
                        if (grayImage.IsEmpty)
                        {

                        }
                        System.Drawing.Rectangle[] faces = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, System.Drawing.Size.Empty, System.Drawing.Size.Empty);
                        if (faces.Length > 0)
                        {
                            foreach (var f in faces)
                            {
                                CvInvoke.Rectangle(imageFrame, f, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                                Image<Bgr, Byte> resImage = imageFrame.Convert<Bgr, Byte>();
                                resImage.ROI = f;
                                

                                if (enableSaveImage)
                                {
                                    pictureBox1.Source = BitmapSourceConvert.ToBitmapSource(imageFrame);
                                    string path = data.Create(Name.Text);
                                    string str = Name.Text;
                                    Task.Factory.StartNew(() =>
                                    {
                                        for (int i = 0; i < 10; i++)
                                        {
                                            Image<Gray, Byte> grayIm = resImage.Convert<Gray, Byte>();
                                            CvInvoke.EqualizeHist(grayIm, grayIm);
                                            grayIm.Resize(100, 100, Inter.Cubic).Save(path + @"\" + str + "_" + DateTime.Now.ToString("dd-mm-yyyy-hh-mm-ss") + ".jpg");
                                            Thread.Sleep(1000);
                                        }
                                    });
                                    

                                }
                                enableSaveImage = false;

                                if (isTrained && Res)
                                {
                                    
                                        Mat mage = new Mat();
                                        CvInvoke.CvtColor(resImage, mage, ColorConversion.Bgr2Gray);
                                        CvInvoke.EqualizeHist(mage, mage);
                                        Stopwatch stopwatch = new Stopwatch();
                                        CvInvoke.Resize(mage, mage, new System.Drawing.Size(100, 100), 0, 0, Inter.Linear);
                                        stopwatch.Start();
                                        string name = algorithm.Recognize(algorithm.NewCoordinates(mage));
                                        stopwatch.Stop();
                                        Console.WriteLine("Time of recognition is {0} ms", stopwatch.ElapsedMilliseconds);
                                        CvInvoke.PutText(imageFrame, name, new System.Drawing.Point(f.X - 2, f.Y - 2), FontFace.HersheyComplex, 1.0, new Bgr(System.Drawing.Color.Blue).MCvScalar);
                                        
                                   
                                }
                            }
                        }
                    }
                    
                    pictureBox.Source = BitmapSourceConvert.ToBitmapSource(imageFrame);
                    
                }
            }


            
        }
        bool Res = false;

        private bool isTrained = false;

        List<string> trimgPath;
        List<int> labels1;
        List<int> imnum1;

        List<string> testimgPath;
        List<int> labels2;
        List<int> imnum2;


        List<string> targ;
        Algorithm algorithm;
        public bool Train()
        {
            int threshold = 8;
            Data date = new Data(threshold);

            List<string> trimgPath = date.trimgPath;
             List<int> labels1 = date.labels1;
              List<int> imnum1 = date.imnum1;

                   List<string> testimgPath = date.testimgPath;
                List<int> labels2 = date.labels2;
                List<int> imnum2 = date.imnum2;


       List<string> targ = date.targ;

            int imgWidth = 100;
            int imgHeight =100;


            try
            {
                M imMatr = new M(trimgPath, imgWidth, imgHeight);
                byte[,] m = imMatr.GetM();
                algorithm = new Algorithm(m, labels1, targ, imnum1, imgWidth, imgHeight, 90);

                MathNet.Numerics.LinearAlgebra.Matrix<double> coordinates = algorithm.DimensionReduction();
                isTrained = true;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
                
            }

        return true;

    }
        public static class BitmapSourceConvert
        {
            [DllImport("gdi32")]
            private static extern int DeleteObject(IntPtr o);

            public static BitmapSource ToBitmapSource(IImage image)
            {
                using (System.Drawing.Bitmap source = image.Bitmap)
                {
                    IntPtr ptr = source.GetHbitmap();

                    BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        ptr,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    DeleteObject(ptr);
                    return bs;
                }
            }
        }
        bool faceDetectionEnabled=false;
        private void Capture_Click(object sender, RoutedEventArgs e)
        {

            if (_capture == null)
            {
                try
                {
                    _capture = new Capture();
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                    return;
                }

            }
            
            
            ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;

        }
       
        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            if (faceDetectionEnabled)
            {
                faceDetectionEnabled = false;
            }
            else
            {
                faceDetectionEnabled = true;
            }
            
        }
        bool enableSaveImage=false;
        private void AddPerson_Click(object sender, RoutedEventArgs e)
        {
            SavePerson.IsEnabled = true;
            AddPerson.IsEnabled = false;
            enableSaveImage = false;
        }

        private void SavePerson_Click(object sender, RoutedEventArgs e)
        {
            SavePerson.IsEnabled = false;
            AddPerson.IsEnabled = true;
            enableSaveImage = true;
        }

        private void TrainImages_Click(object sender, RoutedEventArgs e)
        {
            if (Train())
            {

                Console.WriteLine("True");
            }
        }

        private void Recognize_Click(object sender, RoutedEventArgs e)
        {

            if (Res)
            {
                Res = false;
            }
            else
            {
                Res= true;
            }

        }
    }
}

