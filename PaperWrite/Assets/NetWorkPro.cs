using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleNeuralNetwork;
using UnityEngine.UI;

public class NetWorkPro : MonoBehaviour
{
    NumberClassification numberClassification = new NumberClassification();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SaveColor();
    }

    public void SaveColor()//传输用于识别的颜色，因为识别时的颜色分辨率为28*28，需平均化
    {
        float col_edge, row_edge;
        col_edge = Paper._Paper.terrainSize / 28.0f;
        row_edge = Paper._Paper.terrainSize / 28.0f;
        int[] Table = new int[Paper._Paper.vertexs.Count];

        for (int n = 0; n < 784; n++)
        {
            Paper._Paper.SavedColorList[n] = new Color(1, 1, 1);//颜色复位
            
        }

        for (int k = 0; k < Paper._Paper.vertexs.Count; k++)
        {
            int i = k / Paper._Paper.terrainSize;
            int j = k % Paper._Paper.terrainSize;
            //得到第k个顶点的方块索引
            Table[k] = (int)(28 * ((int)((i + 1) / row_edge + 1) - 1) + ((int)((j + 1) / col_edge + 1)) - 1);
            Paper._Paper.SavedColorList[Table[k]] += Paper._Paper.cList[k];//存颜色
            Paper._Paper.SavedColorCount[Table[k]] += 1;//计数，准备平均化
            //print(Table[k]);
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
        texter.GetComponent<Text>().text="猜测为\n"+array[0].ToString()+"\t"+array1[0].ToString()+"%";
        GameObject texter1 = GameObject.Find("Text1");
        texter1.GetComponent<Text>().text = "备选\n" + array[1] + "\t" + array1[1]+"%"+
        "\n" + array[2] + "\t" + array1[2] + "%" +
        "\n" + array[3] + "\t" + array1[3] + "%" +
        "\n" + array[4] + "\t" + array1[4] + "%" +
        "\n" + array[5] + "\t" + array1[5] + "%" +
        "\n" + array[6] + "\t" + array1[6] + "%" +
        "\n" + array[7] + "\t" + array1[7] + "%" +
        "\n" + array[8] + "\t" + array1[8] + "%" +
        "\n" + array[9] + "\t" + array1[9] + "%";

        //print(array[0]);
     

    }
}
