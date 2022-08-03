using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Paper : MonoBehaviour
{


    [Range(0, 250)]
    public int terrainSize = 250;    //生成纸张的长宽

    [Range(50, 3000)]
    public int pencilWidth = 1000;  //笔的粗细

    public Material terrainMat;     //赋予所生成纸张的材质

    
    public Vector3 mousePos;    //鼠标屏幕空间坐标
    public Vector3 lastMousePos;//鼠标上一个位置屏幕空间坐标

    public Color[] cList;    //存储所有顶点颜色
    public List<Vector3> vertexs = new List<Vector3>();//储存所有顶点坐标,下方还有第二种定义数组的方式
        
    private List<int> tris = new List<int>();//储存所有三角形

    public Color[] SavedColorList=new Color[784];//储存所有用于识别的颜色
    public int[] SavedColorCount = new int[784];//储存用于识别的颜色的计数
    

    public static Paper _Paper;//Paper实例

    private List<Vector3> plaScreenPos=new List<Vector3>();//纸张屏幕坐标

    public int donot = 1;//1表示不要与上一个鼠标位置连起来

    void Start()
    {
        _Paper = this;
        GenerateTerrain();//生成纸张
        donot = 1;
        mousePos = new Vector3(0, 0, 0);
        lastMousePos = new Vector3(0, 0, 0);
        GameObject slider = GameObject.Find("Slider");
        slider.GetComponent<Slider>().value = pencilWidth;
    }
    void Update()
    {

        
        
        mousePos = Input.mousePosition;//鼠标屏幕坐标
        
        if (isClick._instance.click > 0)//鼠标点击，绘画
            ChangeTerrain();

        if (isClick._instance.click < 1)//鼠标抬起，则下一次绘画不会与抬起处插值
            donot = 1;
        
        if (Input.GetKeyDown(KeyCode.C))//点击c清空画布
            ClearColor();
        
        lastMousePos = mousePos;
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
            plaScreenPos.Add(Camera.main.WorldToScreenPoint(vertexs[j]));//将纸张顶点从世界坐标系转换为屏幕坐标系


        }
        plane.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);

        





    }

    private void ChangeTerrain()
    {
        
        GameObject pla = GameObject.Find("Terrain");//找到Terrain对象
        //float a = 1 / (mousePos.x - lastMousePos.x);
        //float b = -1 / (mousePos.x - lastMousePos.x);
        //float c = lastMousePos.y / (mousePos.y - lastMousePos.y) - lastMousePos.x / (mousePos.x - lastMousePos.x);
        Vector3 mP1 = (lastMousePos + mousePos) / 2;
        
        Vector3 mP2 = (lastMousePos + mP1) / 2;
        Vector3 mP3 = (mousePos + mP1) / 2;
        Vector3 mP4 = (lastMousePos + mP2) / 2;
        Vector3 mP5 = (mP2 + mP1) / 2;
        Vector3 mP6 = (mP1 + mP3) / 2;
        Vector3 mP7 = (mP3 + mousePos) / 2;

        GameObject slider = GameObject.Find("Slider");
        pencilWidth=(int)slider.GetComponent<Slider>().value;


        for (int j = 0; j < vertexs.Count; j++)
        {
            //计算和鼠标的距离
            //float dis = (plaScreenPos[j].x - mousePos.x) * (plaScreenPos[j].x - mousePos.x) + (plaScreenPos[j].y - mousePos.y) * (plaScreenPos[j].y - mousePos.y);
            float dis1 = dis(mousePos, plaScreenPos[j]);
            //float dis2 = dis(lastMousePos, plaScreenPos[j]);
            if(donot<1)
            {
                float dis3 = dis(mP1, plaScreenPos[j]);
                float dis4 = dis(mP2, plaScreenPos[j]);
                float dis5 = dis(mP3, plaScreenPos[j]);
                float dis6 = dis(mP4, plaScreenPos[j]);
                float dis7 = dis(mP5, plaScreenPos[j]);
                float dis8 = dis(mP6, plaScreenPos[j]);
                float dis9 = dis(mP7, plaScreenPos[j]);
                if (dis1 < pencilWidth || dis3 < pencilWidth || dis4 < pencilWidth || dis5 < pencilWidth || dis6 < pencilWidth || dis7 < pencilWidth || dis8 < pencilWidth || dis9 < pencilWidth)//在距离内则显示黑色
                {
                    cList[j] = new Color(0, 0, 0);
                    //print(dis);
                }
            }
            else
            {
                if (dis1 < pencilWidth)//在距离内则显示黑色
                {
                    cList[j] = new Color(0, 0, 0);
                    //print(dis);
                }
                
            }
            if (donot > 0)
                donot = 0;
            

            //float lastDis = (plaScreenPos[j].x - lastMousePos.x) * (plaScreenPos[j].x - lastMousePos.x) + (plaScreenPos[j].y - lastMousePos.y) * (plaScreenPos[j].y - lastMousePos.y);
            //double dis = abs(a * plaScreenPos[j].x + b * plaScreenPos[j].y + c) / (a * a + b * b);
            //float dis10 = dis1 + dis2 + dis3 + dis4 + dis5 + dis6 + dis7 + dis8 + dis9;
            
            

            

        }


        pla.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//设置颜色

    }


    private void ClearColor()
    {
        for (int j = 0; j < cList.Length; j++)
        {
            cList[j] = new Color(1, 1, 1);//颜色全部为白色
        }
        GameObject plan = GameObject.Find("Terrain");
        plan.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//设置颜色
    }

    

    private float dis(Vector3 mouse,Vector3 pla)
    {
        float dis1 = (pla.x - mouse.x) * (pla.x - mouse.x) + (pla.y - mouse.y) * (pla.y - mouse.y);

        return dis1;
    }
}





    
