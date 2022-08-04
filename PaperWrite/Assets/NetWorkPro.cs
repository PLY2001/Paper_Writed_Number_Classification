using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleNeuralNetwork;
using UnityEngine.UI;

public class NetWorkPro : MonoBehaviour
{
    NumberClassification numberClassification = new NumberClassification();
    public float[,] cList2dT2B = new float[250,250];
    public float[,] cList2dL2R = new float[250, 250];
    public float[,] CaughtcList;
    static int indexTop;
    static int indexBottom;
    static int indexLeft;
    static int indexRight;
    public float Value;
    static int toSave = 1;

    // Start is called before the first frame update
    void Start()
    {
        Value = 50;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isClick._instance.click<1)
        {
            if(toSave>0)
            {
                
                toSave = 0;

            }
            

        }
        if(isClick._instance.click >0)
        {
            toSave = 1;
        }
        SaveColor();
    }

    public void SaveColor()//传输用于识别的颜色，因为识别时的颜色分辨率为28*28，需平均化
    {
        

        for (int n = 0; n < 784; n++)
        {
            Paper._Paper.SavedColorList[n] = new Color(1, 1, 1);//颜色复位
            
        }

        indexTop = 0;
        indexBottom = 249;
        indexLeft = 0;
        indexRight = 249;
        int firstCaughtT= 1;
        int firstCaughtL = 1;
        for (int i=0;i< 250;i++)
        {
            for(int j=0;j<250;j++)
            {
                cList2dT2B[i, j] = Paper._Paper.cList[i * 250 + j].r;
                cList2dL2R[j, i] = Paper._Paper.cList[j * 250 + i].r;
                if (cList2dT2B[i,j]<1)
                {
                    if(firstCaughtT>0)
                    {
                        indexTop = i;//行
                        firstCaughtT = 0;
                    }
                    indexBottom = i;  
                }
                if (cList2dL2R[j, i] < 1)
                {
                    if (firstCaughtL > 0)
                    {
                        indexLeft = i;//列
                        firstCaughtL = 0;
                    }
                    indexRight = i;
                }
            }
        }

        int CaughtHeight = indexBottom - indexTop+1;
        int CaughtWidth = indexRight - indexLeft+1;

        //print(CaughtHeight);
        //print(CaughtWidth);

        if(CaughtHeight>CaughtWidth)
        {
            CaughtcList = new float[CaughtHeight,CaughtHeight];
            
            for(int i=0;i<CaughtHeight;i++)
            {
                for (int j = 0; j < CaughtHeight;j++)
                {
                    CaughtcList[i, j] = 1;

                    if (CaughtHeight - 2 * j - CaughtWidth <= 0 && CaughtWidth + CaughtHeight - j * 2 >= 0)
                    {
                        int gg = indexLeft + (j - (CaughtHeight - CaughtWidth) / 2);
                        if (gg>249)
                        {
                            gg = 249;

                        }
                        CaughtcList[i, j] = cList2dT2B[indexTop + i, gg];
                    }
                }

                
            }
        }
        else
        {
            CaughtcList = new float[CaughtWidth, CaughtWidth];

            for (int i = 0; i < CaughtWidth; i++)
            {
                for (int j = 0; j < CaughtWidth; j++)
                {
                    CaughtcList[i, j] = 1;

                    if (CaughtWidth - 2 * i - CaughtHeight <= 0 && CaughtWidth + CaughtHeight - i * 2 >= 0)
                    {
                        int ggg = indexTop + (i - (CaughtWidth - CaughtHeight) / 2);
                        if (ggg > 249)
                        {
                            ggg = 249;

                        }
                        CaughtcList[i, j] = cList2dT2B[ggg, indexLeft + j];
                    }
                }


            }
        }


        /*for (int k = 0; k < Paper._Paper.vertexs.Count; k++)
        {
            int i = k / Paper._Paper.terrainSize;
            int j = k % Paper._Paper.terrainSize;
            //得到第k个顶点的方块索引
            Table[k] = (int)(28 * ((int)((i + 1) / row_edge + 1) - 1) + ((int)((j + 1) / col_edge + 1)) - 1);
            Paper._Paper.SavedColorList[Table[k]] += Paper._Paper.cList[k];//存颜色
            Paper._Paper.SavedColorCount[Table[k]] += 1;//计数，准备平均化
            //print(Table[k]);
        }*/
        int size = CaughtcList.GetLength(0) * CaughtcList.GetLength(1);//0高1宽
        float col_edge, row_edge;
        col_edge = CaughtcList.GetLength(1) / 28.0f;
        row_edge = CaughtcList.GetLength(0) / 28.0f;
        int[] Table = new int[size];
        for (int k = 0; k < size; k++)
        {
            int i = k / CaughtcList.GetLength(1);
            int j = k % CaughtcList.GetLength(1);
            //得到第k个顶点的方块索引
            Table[k] = (int)(28 * ((int)((i + 1) / row_edge + 1) - 1) + ((int)((j + 1) / col_edge + 1)) - 1);
            //print(Table[k]);
            if(Table[k]>783)
            {
                Table[k] = 783;
            }
            Paper._Paper.SavedColorList[Table[k]] += new Color(CaughtcList[i,j],1,1);//存颜色
            Paper._Paper.SavedColorCount[Table[k]] += 1;//计数，准备平均化
            //print(Table[k]);
            //print(CaughtcList.GetLength(1));
            //print(CaughtcList.GetLength(0));
        }

        for (int n = 0; n < 784; n++)
        {
            //平均化
            Paper._Paper.SavedColorList[n] = Paper._Paper.SavedColorList[n] / Paper._Paper.SavedColorCount[n];
            Paper._Paper.SavedColorCount[n] = 0;//计数复位
        }

        //print("Saved Succeed");

        float[,] x = new float[28, 28];
        for (int i = 0; i < 28; i++)
        {
            for (int j = 0; j < 28; j++)
            {
                x[i, j] = 1 - Paper._Paper.SavedColorList[i * 28 + j].r;//将一维数组换为二维数组
                //print(x[i,j]);
            }
        }
        numberClassification.Cal_with_Network(x);//识别
        //print(numberClassification.Get_Num());
        int[] array = new int[10];
        array =numberClassification.Get_Num_Array();//得到识别数字
        float[] array1 = new float[10];
        array1 = numberClassification.Get_Percent_Array();//得到识别准确率百分比

        //显示文字
        GameObject texter = GameObject.Find("Text");
        GameObject texter1 = GameObject.Find("Text1");
        if (array1[0]>Value)
        {
            texter.GetComponent<Text>().text = "猜测为\n" + array[0].ToString() + "\t" + array1[0].ToString() + "%";
            
            texter1.GetComponent<Text>().text = "备选\n" + array[1] + "\t" + array1[1] + "%" +
            "\n" + array[2] + "\t" + array1[2] + "%" +
            "\n" + array[3] + "\t" + array1[3] + "%" +
            "\n" + array[4] + "\t" + array1[4] + "%" +
            "\n" + array[5] + "\t" + array1[5] + "%" +
            "\n" + array[6] + "\t" + array1[6] + "%" +
            "\n" + array[7] + "\t" + array1[7] + "%" +
            "\n" + array[8] + "\t" + array1[8] + "%" +
            "\n" + array[9] + "\t" + array1[9] + "%";
        }
        else
        {
            texter.GetComponent<Text>().text = "不确定";

            texter1.GetComponent<Text>().text = "备选\n" +
                   array[0] + "\t" + array1[0] + "%" +
            "\n" + array[1] + "\t" + array1[1] + "%" +
            "\n" + array[2] + "\t" + array1[2] + "%" +
            "\n" + array[3] + "\t" + array1[3] + "%" +
            "\n" + array[4] + "\t" + array1[4] + "%" +
            "\n" + array[5] + "\t" + array1[5] + "%" +
            "\n" + array[6] + "\t" + array1[6] + "%" +
            "\n" + array[7] + "\t" + array1[7] + "%" +
            "\n" + array[8] + "\t" + array1[8] + "%" +
            "\n" + array[9] + "\t" + array1[9] + "%";
        }
        

        //print(array[0]);
     

    }
}
