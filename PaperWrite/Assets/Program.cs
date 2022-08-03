using System;
using UnityEngine;

namespace SimpleNeuralNetwork
{
    public class NumberClassification
    {
        float[,] image28_28 = new float[28, 28];
        float[] image28_28_flatten = new float[28 * 28];
        float[] percent = new float[10];
        int[] index = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        Flatten fla1 = new Flatten(28, 28);
        Linear fc1 = new Linear(28 * 28, 32);
        ReLU relu1 = new ReLU();
        Linear fc2 = new Linear(32, 32);
        ReLU relu2 = new ReLU();
        Linear fc3 = new Linear(32, 10);

        public NumberClassification()
        {
            fc1.Read_Weight(Application.streamingAssetsPath+"/fc1_weight.csv");
            fc1.Read_Bias(Application.streamingAssetsPath + "/fc1_bias.csv");
            fc2.Read_Weight(Application.streamingAssetsPath + "/fc2_weight.csv");
            fc2.Read_Bias(Application.streamingAssetsPath + "/fc2_bias.csv");
            fc3.Read_Weight(Application.streamingAssetsPath + "/fc3_weight.csv");
            fc3.Read_Bias(Application.streamingAssetsPath + "/fc3_bias.csv");            
        }

        public void Cal_with_Network(float[,] image)
        {
            //必须是28x28图像
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    image28_28[i, j] = image[i, j];
                }
            }

            image28_28_flatten = new float[28 * 28];

            image28_28_flatten = fla1.Cal(image28_28);

            image28_28_flatten = fc1.Cal(image28_28_flatten);

            image28_28_flatten = relu1.Cal(image28_28_flatten);

            image28_28_flatten = fc2.Cal(image28_28_flatten);

            image28_28_flatten = relu2.Cal(image28_28_flatten);

            percent = fc3.Cal(image28_28_flatten);


            //换算为百分比
            float sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (float)Math.Exp(percent[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                percent[i] = (float)Math.Exp(percent[i])/sum*100.0f;
            }

            //冒泡排序
            index = new int[10]{0,1,2,3,4,5,6,7,8,9};
            for (int i = 0; i < 10; i++)
            {
                for (int j = i; j < 10; j++)
                {
                    if (percent[i] < percent[j])
                    {
                        float temp = percent[i];
                        percent[i] = percent[j];
                        percent[j] = temp;
                        int temp_index = index[i];
                        index[i] = index[j];
                        index[j] = temp_index;
                    }
                }
            }
        }

        public int Get_Num()
        {
            return index[0];
        }

        public float Get_Percent()
        {
            return percent[0];
        }

        public int[] Get_Num_Array()
        {
            return index;
        }

        public float[] Get_Percent_Array()
        {
            return percent;
        }

        public void Print_Num_Array()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("{0:G}", index[i]);
            }
            Console.Write("\n");
        }

        public void Print_Percent_Array()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("{0:G}", percent[i]);
            }
            Console.Write("\n");
        }
    }

    class Linear
    {
        public int input_Num;
        public int output_Num;
        public float[] input = new float[1024];
        public float[] output = new float[1024];
        public float[,] weight = new float[1024, 1024];
        public float[] bias = new float[1024];

        public Linear(int input_Number,int output_Number)
        {
            input_Num = input_Number;
            output_Num = output_Number;
            System.Random r = new System.Random();
            for (int i = 0; i < output_Num; i++)
            {
                for (int j = 0; j < input_Num; j++)
                {
                    weight[i, j] = 2 * (float)r.NextDouble() - 1;
                }
                bias[i] = input_Num * (2 * (float)r.NextDouble() - 1);
            }
        }

        public void Read_Weight(string path)
        {
            //文件流读取
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("utf-8"));

            string tempText = "";
            int i = 0;
            while ((tempText = sr.ReadLine()) != null)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //从第二行开始添加到datatable数据行
                for (int j = 0; j < arr.Length; j++)
                {
                    weight[i,j] = float.Parse(arr[j]);
                }
                i++;                
            }
            //关闭流
            sr.Close(); 
            fs.Close();
        }

        public void Read_Bias(string path)
        {
            //文件流读取
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("utf-8"));

            string tempText = "";
            int i = 0;
            while ((tempText = sr.ReadLine()) != null)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //从第二行开始添加到datatable数据行
                bias[i] = float.Parse(arr[0]);
                i++;
            }
            //关闭流
            sr.Close();
            fs.Close();
        }

        public void Print_Weight()
        {
            for (int i = 0; i < output_Num; i++)
            {
                for (int j = 0; j < input_Num; j++)
                {
                    Console.Write("{0:G} ",weight[i,j]);
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Bias()
        {
            for (int i = 0; i < output_Num; i++)
            {
                Console.WriteLine("{0:G}", bias[i]);
            }
            Console.Write("\n");
        }

        public void Print_Input()
        {
            for (int i = 0; i < input_Num; i++)
            {
                Console.WriteLine("{0:G}", input[i]);
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int i = 0; i < output_Num; i++)
            {
                Console.WriteLine("{0:G}", output[i]);
            }
            Console.Write("\n");
        }

        public float[] Cal(float[] x)
        {
            Array.Resize(ref input, input_Num);
            Array.Resize(ref output, output_Num);
            for (int i = 0; i < output_Num; i++)
            {
                output[i] = 0;
                for (int j = 0; j < input_Num; j++)
                {
                    input[j] = x[j];
                    output[i] += weight[i, j] * input[j];
                }
                output[i] += bias[i];
            }
            return output;
        }

    }

    class ReLU
    {
        public int num;
        public float[] input = new float[1024];
        public float[] output = new float[1024];

        public ReLU()
        {
            ;
        }

        public void Print_Input()
        {
            for (int i = 0; i < num; i++)
            {
                Console.WriteLine("{0:G}", input[i]);
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int i = 0; i < num; i++)
            {
                Console.WriteLine("{0:G}", output[i]);
            }
            Console.Write("\n");
        }

        public float[] Cal(float[] x)
        {
            num = x.Length;
            Array.Resize(ref input, num);
            Array.Resize(ref output, num);
            for (int i = 0; i < num; i++)
            {
                input[i] = x[i];
                output[i] = input[i] >= 0 ? input[i] : 0;
            }
            return output;
        }

    }

    class Flatten
    {
        public int input_i_Num;
        public int input_j_Num;
        public float[,] input = new float[1024,1024];
        public float[] output = new float[1024];

        public Flatten(int input_i_Number, int input_j_Number)
        {
            input_i_Num = input_i_Number;
            input_j_Num = input_j_Number;
        }

        public void Print_Input()
        {
            for (int i = 0; i < input_i_Num; i++)
            {
                for (int j = 0; j < input_j_Num; j++)
                {
                    Console.Write("{0:G}", input[i,j]);
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int i = 0; i < input_i_Num*input_j_Num; i++)
            {
                Console.WriteLine("{0:G}", output[i]);
            }
            Console.Write("\n");
        }

        public float[] Cal(float[,] x)
        {
            Array.Resize(ref output, input_i_Num*input_j_Num);
            for (int i = 0; i < input_i_Num; i++)
            {
                for (int j = 0; j < input_j_Num; j++)
                {
                    input[i, j] = x[i, j];
                    output[i * input_j_Num + j] = input[i,j];
                }
            }
            return output;
        }

    }


   /* class Program
    {
        static void Main(string[] args)
        {
            int width = 28;
            int height = 28;
            float[,] x = new float[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j==14)
                    {
                        x[i, j] = 1;
                    }
                    else
                    {
                        x[i, j] = 0;
                    }
                }
            }

            NumberClassification numberClassification = new NumberClassification();
            numberClassification.Cal_with_Network(x);
            numberClassification.Print_Num_Array();
            numberClassification.Print_Percent_Array();
        }
    }*/
}
