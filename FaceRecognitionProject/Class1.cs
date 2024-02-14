using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionProject
{
    class Data
    {

        string GetName(string name)
        {

            string[] str = name.Split('\\');
            return str[str.Length - 1];

        }

        string repos = "C:\\Users\\Ренат\\Downloads\\facerecognition\\FaceRecognitionProject\\FaceRecognitionProject\\images\\ORL";
        public List<string> trimgPath = new List<string>();
        public List<int> labels1 = new List<int>();
        public List<int> imnum1 = new List<int>();

       public List<string> testimgPath = new List<string>();
        public List<int> labels2 = new List<int>();
       public List<int> imnum2 = new List<int>();


       public List<string> targ = new List<string>();


        int number = 0;
        public string Create(string name)
        {
            if(!Directory.Exists(repos + "\\" + name))
            {
                Directory.CreateDirectory(repos + "\\" + name);

            }

            return repos+"\\"+name;
        }
        public Data(int reqNum)
        {

            foreach (string name in Directory.GetDirectories(repos, "*", SearchOption.TopDirectoryOnly))
            {

                if (Directory.GetFiles(name, "*", SearchOption.TopDirectoryOnly).Length >= reqNum)
                {

                    int i = 0;

                    foreach (string imgName in Directory.GetFiles(name, "*", SearchOption.TopDirectoryOnly))
                    {

                        if (i < reqNum)
                        {
                            trimgPath.Add(imgName);
                            labels1.Add(number);
                            if (imnum1.Count > number)
                            {
                                imnum1[number] += 1;

                            }
                            else
                            {
                                imnum1.Add(1);
                            }

                            if (i == 0)
                            {

                                targ.Add(GetName(name));
                            }
                        }
                        else
                        {
                            testimgPath.Add(imgName);
                            labels2.Add(number);
                            if (imnum2.Count > number)
                            {
                                imnum2[number] += 1;

                            }
                            else
                            {
                                imnum2.Add(1);
                            }



                        }

                        i += 1;
                    }

                    number += 1;

                }
            }

        }
    }
}