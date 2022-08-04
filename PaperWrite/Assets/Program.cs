using System;
using UnityEngine;

namespace SimpleNeuralNetwork
{
    class NumberClassification
    {
        float[,] image28_28 = new float[28, 28];
        float[] percent = new float[10];
        int[] index = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        NeuralNetwork NeuralNetwork = new NeuralNetwork();

        public NumberClassification()
        {
            ;
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

            percent = NeuralNetwork.Cal_with_Network(image28_28);

            //换算为百分比
            float sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (float)Math.Exp(percent[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                percent[i] = (float)Math.Exp(percent[i]) / sum * 100.0f;
            }

            //冒泡排序
            index = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
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

    class NeuralNetwork
    {
        float[,,] input3d = new float[32, 256, 256];
        float[,] input2d = new float[256, 256];
        float[] input1d = new float[1024];

        float[,] input = new float[256, 256];
        float[] output = new float[10];

        DimensionChange from2dto3d = new DimensionChange();
        Conv2d conv2d1 = new Conv2d(28, 28, 1, 16, 3, 3, 1, 1, 1, 1, 1, 1);
        ReLU2d relu2d1 = new ReLU2d(28, 28);
        Pool2d pool2d1 = new Pool2d(28, 28, 14, 14);
        Conv2d conv2d2 = new Conv2d(14, 14, 16, 16, 3, 3, 1, 1, 1, 1, 1, 1);
        ReLU2d relu2d2 = new ReLU2d(14, 14);
        Pool2d pool2d2 = new Pool2d(14, 14, 2, 2);
        FlattenFrom3Dto1D Flatten1 = new FlattenFrom3Dto1D(16, 2, 2);
        Linear fc1 = new Linear(2 * 2 * 16, 16);
        ReLU relu1 = new ReLU(16);
        Linear fc2 = new Linear(16, 10);

        public NeuralNetwork()
        {
            conv2d1.Read_Weight(1);
            conv2d1.Read_Bias(1);
            conv2d2.Read_Weight(2);
            conv2d2.Read_Bias(2);
            fc1.Read_Weight(3);
            fc1.Read_Bias(3);
            fc2.Read_Weight(4);
            fc2.Read_Bias(4);
        }

        public float[] Cal_with_Network(float[,] INPUT)
        {
            //必须是28x28图像
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    input[i, j] = INPUT[i, j];
                }
            }

            input3d = from2dto3d.from2Dto3D(input);

            input3d = conv2d1.Cal(input3d);
            //conv2d1.Print_Input();


            input3d = relu2d1.Cal(input3d);

            input3d = pool2d1.Cal(input3d);
            //pool2d1.Print_Output();

            input3d = conv2d2.Cal(input3d);

            input3d = relu2d2.Cal(input3d);

            input3d = pool2d2.Cal(input3d);

            input1d = Flatten1.Cal(input3d);

            input1d = fc1.Cal(input1d);

            input1d = relu1.Cal(input1d);

            output = fc2.Cal(input1d);

            return output;

        }

        public void Print_Input()
        {
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    Console.Write("{0:G} ", input[i, j]);
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("{0:G}", output[i]);
            }
            Console.Write("\n");
        }

    }

    class DimensionChange
    {
        public DimensionChange()
        {
            ;
        }

        public float[] from3Dto1D(float[,,] input)
        {
            float[] output = new float[input.GetLength(0)];
            for (int i = 0; i < input.GetLength(0); i++)
            {
                output[i] = input[i, 0, 0];
            }
            return output;
        }

        public float[,,] from2Dto3D(float[,] input)
        {
            float[,,] output = new float[1, input.GetLength(0), input.GetLength(1)];
            for (int i = 0; i < input.GetLength(0); i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    output[0, i, j] = input[i, j];
                }
            }
            return output;
        }




    }

    class Read_Para
    {
        string path = Application.streamingAssetsPath + "/para.csv";

        public Read_Para()
        {
            ;
        }

        public float[,] read_weight(int index)
        {
            float[,] weight = new float[1024, 1024];
            int cnt = 0;
            bool isFind = false;
            //文件流读取
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("utf-8"));

            string tempText = "";
            while ((tempText = sr.ReadLine()) != null && isFind == false)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length >= 1 && arr[0].IndexOf("weight") >= 0)
                {
                    cnt++;
                    if (cnt == index)
                    {
                        tempText = sr.ReadLine();
                        string[] arr1 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr1.Length == 2)
                        {
                            isFind = true;
                            int i = 0;
                            while ((tempText = sr.ReadLine()) != null)
                            {
                                string[] arr2 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr2.Length < 1)
                                {
                                    break;
                                }
                                for (int j = 0; j < arr2.Length; j++)
                                {
                                    weight[i, j] = float.Parse(arr2[j]);
                                }
                                i++;
                            }
                        }
                    }
                }
            }
            //关闭流
            sr.Close();
            fs.Close();
            return weight;
        }

        public float[,,,] read_kernel(int index)
        {
            float[,,,] weight = new float[32, 32, 256, 256];
            int cnt = 0;
            bool isFind = false;
            //文件流读取
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("utf-8"));

            string tempText = "";
            while ((tempText = sr.ReadLine()) != null && isFind == false)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length >= 1 && arr[0].IndexOf("weight") >= 0)
                {
                    cnt++;
                    if (cnt == index)
                    {
                        tempText = sr.ReadLine();
                        string[] arr1 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr1.Length == 4)
                        {
                            isFind = true;
                            while ((tempText = sr.ReadLine()) != null)
                            {
                                string[] arr2 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr2.Length < 1)
                                {
                                    tempText = sr.ReadLine();
                                    arr2 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (arr2.Length < 1)
                                    {
                                        break;
                                    }
                                }
                                int in_channel = int.Parse(arr2[1]);
                                int out_channel = int.Parse(arr2[0]);
                                int i = 0;
                                while ((tempText = sr.ReadLine()) != null)
                                {
                                    string[] arr3 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (arr3.Length < 1)
                                    {
                                        break;
                                    }
                                    for (int j = 0; j < arr3.Length; j++)
                                    {
                                        weight[out_channel, in_channel, i, j] = float.Parse(arr3[j]);
                                    }
                                    i++;
                                }
                                //                                tempText = sr.ReadLine();
                            }
                        }
                    }
                }
            }
            //关闭流
            sr.Close();
            fs.Close();
            return weight;
        }

        public float[] read_bias(int index)
        {
            float[] bias = new float[1024];
            int cnt = 0;
            bool isFind = false;
            //文件流读取
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("utf-8"));

            string tempText = "";
            while ((tempText = sr.ReadLine()) != null && isFind == false)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length >= 1 && arr[0].IndexOf("bias") >= 0)
                {
                    cnt++;
                    if (cnt == index)
                    {
                        tempText = sr.ReadLine();
                        string[] arr1 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr1.Length == 1)
                        {
                            isFind = true;
                            int i = 0;
                            while ((tempText = sr.ReadLine()) != null)
                            {
                                string[] arr2 = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr2.Length < 1)
                                {
                                    break;
                                }
                                bias[i] = float.Parse(arr2[0]);
                                i++;
                            }
                        }
                    }
                }
            }
            //关闭流
            sr.Close();
            fs.Close();
            return bias;
        }

    }

    class Linear : Read_Para
    {
        public int input_Num;
        public int output_Num;
        public float[] input = new float[1024];
        public float[] output = new float[1024];
        public float[,] weight = new float[1024, 1024];
        public float[] bias = new float[1024];

        public Linear(int input_Number, int output_Number)
        {
            input_Num = input_Number;
            output_Num = output_Number;
        }

        public void Read_Weight(int index)
        {
            weight = read_weight(index);
        }

        public void Read_Bias(int index)
        {
            bias = read_bias(index);
        }

        public void Print_Weight()
        {
            for (int i = 0; i < output_Num; i++)
            {
                for (int j = 0; j < input_Num; j++)
                {
                    Console.Write("{0:G} ", weight[i, j]);
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

    class Conv2d : Read_Para
    {
        public int input_Width;//max:256
        public int input_Height;//max:256
        public int input_Depth;//max:32
        public int output_Width;
        public int output_Height;
        public int output_Depth;
        public int[] kernel_Size = new int[2];//max:256,256
        public int[] stride = new int[2];
        public int[] padding = new int[2];
        public int[] dilation = new int[2];

        public float[,,] input = new float[32, 256, 256];
        public float[,,] output = new float[32, 256, 256];

        public float[,,,] weight = new float[32, 32, 256, 256];
        public float[] bias = new float[32];

        public Conv2d(int input_width, int input_height, int input_depth, int output_depth, int kernel_size0, int kernel_size1, int stride0, int stride1, int padding0, int padding1, int dilation0, int dilation1)
        {
            input_Width = input_width;
            input_Height = input_height;
            input_Depth = input_depth;
            output_Depth = output_depth;
            kernel_Size[0] = kernel_size0;
            stride[0] = stride0;
            padding[0] = padding0;
            dilation[0] = dilation0;
            kernel_Size[1] = kernel_size1;
            stride[1] = stride1;
            padding[1] = padding1;
            dilation[1] = dilation1;
        }

        public void Read_Weight(int index)
        {
            weight = read_kernel(index);
        }

        public void Read_Bias(int index)
        {
            bias = read_bias(index);
        }

        public void Print_Weight()
        {
            for (int out_channal = 0; out_channal < output_Depth; out_channal++)
            {
                for (int in_channal = 0; in_channal < input_Depth; in_channal++)
                {
                    for (int i = 0; i < kernel_Size[0]; i++)
                    {
                        for (int j = 0; j < kernel_Size[1]; j++)
                        {
                            Console.Write("{0:G} ", weight[out_channal, in_channal, i, j]);
                        }
                        Console.Write("\n");
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Bias()
        {
            for (int i = 0; i < output_Depth; i++)
            {
                Console.WriteLine("{0:G}", bias[i]);
            }
            Console.Write("\n");
        }

        public void Print_Input()
        {
            for (int in_channal = 0; in_channal < input_Depth; in_channal++)
            {
                for (int i = 0; i < input_Height; i++)
                {
                    for (int j = 0; j < input_Width; j++)
                    {
                        Console.Write("{0:G} ", input[in_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int out_channal = 0; out_channal < output_Depth; out_channal++)
            {
                for (int i = 0; i < output_Height; i++)
                {
                    for (int j = 0; j < output_Width; j++)
                    {
                        Console.Write("{0:G} ", output[out_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public float[,,] Cal(float[,,] x)
        {
            output_Width = (input_Width + 2 * padding[1] - dilation[1] * (kernel_Size[1] - 1) - 1) / stride[1] + 1;
            output_Height = (input_Height + 2 * padding[0] - dilation[0] * (kernel_Size[0] - 1) - 1) / stride[0] + 1;

            for (int in_channal = 0; in_channal < input_Depth; in_channal++)
            {
                for (int i = 0; i < input_Height; i++)
                {
                    for (int j = 0; j < input_Width; j++)
                    {
                        input[in_channal, i, j] = x[in_channal, i, j];
                    }
                }
            }


            for (int out_channel = 0; out_channel < output_Depth; out_channel++)
            {
                //初始化一个输出层的结果
                float[,] output_one_channel = new float[output_Height, output_Width];
                /*for (int i = 0; i < output_Height; i++)
                {
                    for (int j = 0; j < output_Width; j++)
                    {
                        output_one_channel[i, j] = 0;
                    }
                }*/

                for (int in_channel = 0; in_channel < input_Depth; in_channel++)
                {
                    float[,] conv2d_Result = Get_Conv2d(Get_2d_from3d(input, in_channel),
                                                        Get_2d_from4d(weight, in_channel, out_channel),
                                                        stride,
                                                        padding,
                                                        dilation);
                    output_one_channel = Get_Plus2d(conv2d_Result, output_one_channel);
                }
                output_one_channel = Get_Plus2d(Get_2d_from1d(bias[out_channel]), output_one_channel);
                output = Add_to_output(output, output_one_channel, out_channel);
            }
            return output;
        }

        public float[,] Get_Conv2d(float[,] ConvObject,//被卷积的对象
                                    float[,] kernel,//卷积核
                                    int[] stride,//步长
                                    int[] padding,//补偿
                                    int[] dilation)//卷积核的空位
        {
            float[,] ConvObject_padding = new float[input_Height + padding[0] * 2, input_Width + padding[1] * 2];
            float[,] kernel_dilation = new float[(kernel_Size[0] - 1) * dilation[0] + 1, (kernel_Size[1] - 1) * dilation[1] + 1];
            float[,] result = new float[output_Height, output_Width];

            //补偿
            for (int i = 0; i < input_Height + padding[0] * 2; i++)
            {
                for (int j = 0; j < input_Width + padding[1] * 2; j++)
                {
                    if (i >= padding[0] && i <= input_Height - 1 + padding[0] && j >= padding[1] && j <= input_Width - 1 + padding[1])
                    {
                        ConvObject_padding[i, j] = ConvObject[i - padding[0], j - padding[1]];
                    }
                    else
                    {
                        ConvObject_padding[i, j] = 0;
                    }
                }
            }

            //卷积核空洞
            for (int i = 0; i < (kernel_Size[0] - 1) * dilation[0] + 1; i++)
            {
                for (int j = 0; j < (kernel_Size[1] - 1) * dilation[1] + 1; j++)
                {
                    if ((i == 0 || i % (dilation[0]) == 0) && (j == 0 || j % (dilation[1]) == 0))
                    {
                        kernel_dilation[i, j] = kernel[i / (dilation[0]), j / (dilation[1])];
                    }
                    else
                    {
                        kernel_dilation[i, j] = 0;
                    }
                }
            }

            //卷积结果初始化为0
            for (int i = 0; i < output_Height; i++)
            {
                for (int j = 0; j < output_Width; j++)
                {
                    result[i, j] = 0;
                }
            }

            //开始卷积
            int out_i = 0;
            for (int in_i = ((kernel_Size[0] - 1) * dilation[0]) / 2; in_i < input_Height + padding[0] * 2 - ((kernel_Size[0] - 1) * dilation[0]) / 2; in_i += stride[0])
            {
                int out_j = 0;
                for (int in_j = ((kernel_Size[1] - 1) * dilation[1]) / 2; in_j < input_Width + padding[1] * 2 - ((kernel_Size[1] - 1) * dilation[1]) / 2; in_j += stride[1])
                {
                    for (int m = 0; m < (kernel_Size[0] - 1) * dilation[0] + 1; m++)
                    {
                        for (int n = 0; n < (kernel_Size[1] - 1) * dilation[1] + 1; n++)
                        {
                            result[out_i, out_j] += ConvObject_padding[in_i - ((kernel_Size[0] - 1) * dilation[0]) / 2 + m, in_j - ((kernel_Size[1] - 1) * dilation[1]) / 2 + n] * kernel_dilation[m, n];
                        }
                    }
                    out_j++;
                }
                out_i++;
            }

            return result;
        }

        public float[,] Get_2d_from4d(float[,,,] weight, int in_channel, int out_channel)
        {
            float[,] result = new float[kernel_Size[0], kernel_Size[1]];
            for (int i = 0; i < kernel_Size[0]; i++)
            {
                for (int j = 0; j < kernel_Size[1]; j++)
                {
                    result[i, j] = weight[out_channel, in_channel, i, j];
                }
            }
            return result;
        }

        public float[,] Get_2d_from3d(float[,,] input, int in_channel)
        {
            float[,] result = new float[input_Height, input_Width];
            for (int i = 0; i < input_Height; i++)
            {
                for (int j = 0; j < input_Width; j++)
                {
                    result[i, j] = input[in_channel, i, j];
                }
            }
            return result;
        }

        public float[,] Get_2d_from1d(float num)
        {
            float[,] result = new float[output_Height, output_Width];
            for (int i = 0; i < output_Height; i++)
            {
                for (int j = 0; j < output_Width; j++)
                {
                    result[i, j] = num;
                }
            }
            return result;
        }

        public float[,] Get_Plus2d(float[,] a, float[,] b)
        {
            float[,] result = new float[output_Height, output_Width];
            for (int i = 0; i < output_Height; i++)
            {
                for (int j = 0; j < output_Width; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }
            return result;
        }

        public float[,,] Add_to_output(float[,,] output, float[,] output_one_channel, int out_channel)
        {
            for (int i = 0; i < output_Height; i++)
            {
                for (int j = 0; j < output_Width; j++)
                {
                    output[out_channel, i, j] = output_one_channel[i, j];
                }
            }
            return output;
        }

    }

    class ReLU
    {
        public int num;
        public float[] input = new float[1024];
        public float[] output = new float[1024];

        public ReLU(int NUM)
        {
            num = NUM;
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

    class ReLU2d
    {
        public int Depth;
        public int Width;
        public int Height;
        public float[,,] input = new float[32, 256, 256];
        public float[,,] output = new float[32, 256, 256];

        public ReLU2d(int height, int width)
        {
            Height = height;
            Width = width;
        }

        public void Print_Input()
        {
            for (int in_channal = 0; in_channal < Depth; in_channal++)
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Console.Write("{0:G} ", input[in_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int out_channal = 0; out_channal < Depth; out_channal++)
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Console.Write("{0:G} ", output[out_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public float[,,] Cal(float[,,] x)
        {
            Depth = x.GetLength(0);
            for (int channal = 0; channal < Depth; channal++)
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        input[channal, i, j] = x[channal, i, j];
                        output[channal, i, j] = input[channal, i, j] >= 0 ? input[channal, i, j] : 0;
                    }
                }
            }
            return output;
        }

    }

    class Pool2d
    {
        public int input_Height;
        public int input_Width;
        public int Depth;
        public int output_Height;
        public int output_Width;
        public float[,,] input = new float[32, 256, 256];
        public float[,,] output = new float[32, 256, 256];
        public int[,,] cnt = new int[32, 256, 256];

        public Pool2d(int input_height, int input_width, int output_height, int output_width)
        {
            input_Height = input_height;
            input_Width = input_width;
            output_Height = output_height;
            output_Width = output_width;
        }

        public void Print_Input()
        {
            for (int in_channal = 0; in_channal < Depth; in_channal++)
            {
                for (int i = 0; i < input_Height; i++)
                {
                    for (int j = 0; j < input_Width; j++)
                    {
                        Console.Write("{0:G} ", input[in_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int out_channal = 0; out_channal < Depth; out_channal++)
            {
                for (int i = 0; i < output_Height; i++)
                {
                    for (int j = 0; j < output_Width; j++)
                    {
                        Console.Write("{0:G} ", output[out_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public float[,,] Cal(float[,,] x)
        {
            Depth = x.GetLength(0);
            float col_edge, row_edge;
            col_edge = input_Width / output_Width;
            row_edge = input_Height / output_Height;
            int[,] table = new int[input_Height, input_Width];
            for (int i = 0; i < input_Height; i++)
            {
                for (int j = 0; j < input_Width; j++)
                {
                    table[i, j] = (int)(output_Width * ((int)((i + 1) / row_edge + 1) - 1) + ((int)((j + 1) / col_edge + 1)) - 1);
                }
            }
            for (int channal = 0; channal < Depth; channal++)
            {
                for (int i = 0; i < output_Height; i++)
                {
                    for (int j = 0; j < output_Width; j++)
                    {
                        output[channal, i, j] = 0;
                        cnt[channal, i, j] = 0;
                    }
                }
            }
            for (int channal = 0; channal < Depth; channal++)
            {
                for (int i = 0; i < input_Height; i++)
                {
                    for (int j = 0; j < input_Width; j++)
                    {
                        input[channal, i, j] = x[channal, i, j];
                        output[channal, table[i, j] / output_Width, table[i, j] % output_Width] += input[channal, i, j];
                        cnt[channal, table[i, j] / output_Width, table[i, j] % output_Width] += 1;
                    }
                }
                for (int i = 0; i < output_Height; i++)
                {
                    for (int j = 0; j < output_Width; j++)
                    {
                        output[channal, i, j] = output[channal, i, j] / cnt[channal, i, j];
                    }
                }
            }
            return output;
        }

    }

    class FlattenFrom2Dto1D
    {
        public int input_i_Num;
        public int input_j_Num;
        public float[,] input = new float[256, 256];
        public float[] output = new float[1024];

        public FlattenFrom2Dto1D(int input_i_Number, int input_j_Number)
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
                    Console.Write("{0:G}", input[i, j]);
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int i = 0; i < input_i_Num * input_j_Num; i++)
            {
                Console.WriteLine("{0:G}", output[i]);
            }
            Console.Write("\n");
        }

        public float[] Cal(float[,] x)
        {
            Array.Resize(ref output, input_i_Num * input_j_Num);
            for (int i = 0; i < input_i_Num; i++)
            {
                for (int j = 0; j < input_j_Num; j++)
                {
                    input[i, j] = x[i, j];
                    output[i * input_j_Num + j] = input[i, j];
                }
            }
            return output;
        }

    }

    class FlattenFrom3Dto1D
    {
        public int input_Depth;
        public int input_Width;
        public int input_Height;
        public float[,,] input = new float[32, 256, 256];
        public float[] output = new float[1024];

        public FlattenFrom3Dto1D(int input_depth, int input_height, int input_width)
        {
            input_Depth = input_depth;
            input_Height = input_height;
            input_Width = input_width;
        }

        public void Print_Input()
        {
            for (int in_channal = 0; in_channal < input_Depth; in_channal++)
            {
                for (int i = 0; i < input_Height; i++)
                {
                    for (int j = 0; j < input_Width; j++)
                    {
                        Console.Write("{0:G} ", input[in_channal, i, j]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        public void Print_Output()
        {
            for (int i = 0; i < input_Depth * input_Height * input_Width; i++)
            {
                Console.WriteLine("{0:G}", output[i]);
            }
            Console.Write("\n");
        }

        public float[] Cal(float[,,] x)
        {
            Array.Resize(ref output, input_Depth * input_Height * input_Width);
            int cnt = 0;
            for (int in_channal = 0; in_channal < input_Depth; in_channal++)
            {
                for (int i = 0; i < input_Height; i++)
                {
                    for (int j = 0; j < input_Width; j++)
                    {
                        input[in_channal, i, j] = x[in_channal, i, j];
                        output[cnt] = input[in_channal, i, j];
                        cnt++;
                    }
                }
            }
            return output;
        }

    }




    
}
