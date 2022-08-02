using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Paper : MonoBehaviour
{


    [Range(0, 250)]
    public int terrainSize = 250;    //生成纸张的长宽

    [Range(50, 3000)]
    public int pencilWidth = 1000;  //笔的粗细

    public Material terrainMat;     //赋予所生成纸张的材质

    
    public Vector3 mousePos;    //鼠标屏幕空间坐标

    public Color[] cList;    //存储所有顶点颜色
    public List<Vector3> vertexs = new List<Vector3>();//储存所有顶点坐标
        
    private List<int> tris = new List<int>();//储存所有三角形

    public Color[] SavedColorList=new Color[784];//储存所有用于识别的颜色
    public int[] SavedColorCount = new int[784];//储存用于识别的颜色的计数
    

    public static Paper _Paper;//Paper实例

    //private Vector3[] plaScreenPos;//纸张屏幕坐标

   

    void Start()
    {
        _Paper = this;
        GenerateTerrain();//生成纸张
    }
    void Update()
    {
        mousePos = Input.mousePosition;//鼠标屏幕坐标
       
        if (isClick._instance.click > 0)//鼠标点击，绘画
            ChangeTerrain();
        
        if (Input.GetKeyDown(KeyCode.C))//点击c清空画布
            ClearColor();
    }

    

    private void GenerateTerrain()
    {
        //进行循环，生成一个基本的平面
        for (int i = 0; i < terrainSize; i++)
            for (int j = 0; j < terrainSize; j++)
            {
                
                vertexs.Add(new Vector3(j - terrainSize / 2, terrainSize / 2-i, 0));//从左上一行一行向下生成

                //非坐标轴的顶点
                if (i == 0 || j == 0)
                    continue;

                //给tris添加vertex的索引，可以理解为把每三个顶点“相互连起来”，生成三角形
                tris.Add(terrainSize * i + j);
                tris.Add(terrainSize * i + j - 1);
                tris.Add(terrainSize * (i - 1) + j - 1);
                tris.Add(terrainSize * (i - 1) + j - 1);
                tris.Add(terrainSize * (i - 1) + j);
                tris.Add(terrainSize * i + j);
            }
        cList = new Color[vertexs.Count];//颜色
        //计算uv
        Vector2[] uvs = new Vector2[vertexs.Count];
        for (var i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(vertexs[i].x, vertexs[i].z);

        //创建一个名为Terrain的GameObject，并赋予其材质
        GameObject plane = new GameObject("Terrain");
        plane.AddComponent<MeshFilter>();
        MeshRenderer renderer = plane.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = terrainMat;

        //创建一个mesh来承载我们的网格数据，并将该mesh赋予生成的Terrain
        Mesh groundMesh = new Mesh();
        groundMesh.vertices = vertexs.ToArray();
        groundMesh.uv = uvs;
        groundMesh.triangles = tris.ToArray();
        //重新计算法线
        groundMesh.RecalculateNormals();
        plane.GetComponent<MeshFilter>().mesh = groundMesh;

        plane.AddComponent<MeshCollider>();

        //纸张初始化为白色
        for (int j = 0; j < vertexs.Count; j++)
        {
            cList[j] = new Color(1, 1, 1);

        }
        plane.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);

        





    }

    private void ChangeTerrain()
    {
        
        GameObject pla = GameObject.Find("Terrain");//找到Terrain对象
        for (int j = 0; j < vertexs.Count; j++)
        {
            //计算和鼠标的距离
            Vector3 plaScreenPos = Camera.main.WorldToScreenPoint(vertexs[j]);//将纸张顶点从世界坐标系转换为屏幕坐标系
            float dis = (plaScreenPos.x - mousePos.x) * (plaScreenPos.x - mousePos.x) + (plaScreenPos.y - mousePos.y) * (plaScreenPos.y - mousePos.y);
            if (dis < pencilWidth)//在距离内则显示黑色
            {
                cList[j] = new Color(0, 0, 0);
                //print(dis);
            }
            

        }


        pla.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//设置颜色

    }


    /*private void SaveColor()
    {
        float col_edge, row_edge;
        col_edge = terrainSize / 28.0f;
        row_edge = terrainSize / 28.0f;
        int[] Table = new int[vertexs.Count];
        for (int k = 0; k < vertexs.Count; k++)
        {
            int i = k / terrainSize;
            int j = k % terrainSize;
            Table[k] = (int)(28 * ((int)((i + 1) / row_edge + 1) - 1) + ((int)((j + 1) / col_edge + 1)) - 1);
            SavedColorList[Table[k]] += cList[k];
            SavedColorCount[Table[k]] += 1;
            //print(Table[k]);
        }
        for(int n=0;n<784;n++)
        {
            SavedColorList[n] = SavedColorList[n] / SavedColorCount[n];
        }

        print("Saved Succeed");

        float[,] x = new float[28, 28];
        for(int i = 0; i < 28; i++)
        {
            for (int j = 0; j < 28; j++)
            {
                x[i, j] = 1-SavedColorList[i * 28 + j].r;
            }
        }
        numberClassification.Cal_with_Network(x);
        //print(numberClassification.Get_Num());
        int[] array = new int[10];
        array = numberClassification.Get_Num_Array();
        for (int i = 0; i < 10; i++)
        {
            print(array[i]);
        }

    }*/

    private void ClearColor()
    {
        for (int j = 0; j < cList.Length; j++)
        {
            cList[j] = new Color(1, 1, 1);//颜色全部为白色
        }
        GameObject plan = GameObject.Find("Terrain");
        plan.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//设置颜色
    }
}





    
