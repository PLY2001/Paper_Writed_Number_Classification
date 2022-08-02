using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{

    //public Texture2D heightMap;     //我们指定的高度图

    [Range(0, 70f)]     //Range作为一个Attribute，可以方便我们在Inspector面板上进行
                        //拖拽更改变量的值，并对其大小做一个限制
    public float terrainHeight = 10;

    [Range(0, 250)]
    public int terrainSize = 250;    //我们所生成的地形的长宽

    [Range(50, 3000)]
    public int pencilWidth = 1000;

    public Material terrainMat;     //赋予所生成地形的材质

    public static int toWrite = 1;
    public Vector3 mousePos;

    private static Color[] cList;
    private List<Vector3> vertexs = new List<Vector3>();
        
    private List<int> tris = new List<int>();

    private static int[] SavedColorList=new int[784];


    //public int grassRowCount = 300;
    //public int grassCountPerPatch = 5;
    //public Material grassMat;	//之后会指定给点云的材质
    //private List<Vector3> verts;
    //public gameManager Player;


    void Start()
    {
        //verts = new List<Vector3>();
        //FirstGenerateTerrain();      //生成地形

        //GenerateGrassArea(grassRowCount, grassCountPerPatch);
        
        
        GenerateTerrain();
    }
    void Update()
    {
        mousePos = Input.mousePosition;//鼠标屏幕坐标
        
        if (isClick._instance.click > 0)
            ChangeTerrain();
        
        if(Input.GetKeyDown(KeyCode.S))
            SaveColor();
        if (Input.GetKeyDown(KeyCode.C))
            ClearColor();
        //grassMat.SetVector("_PlayerPosition", new Vector4(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z, 1.0f));
    }

    

    private void GenerateTerrain()
    {
        //要生成一个平面，我们需要自定义其顶点和网格数据
        //List<Vector3> vertexs = new List<Vector3>();
        //List<int> tris = new List<int>();


        //进行循环，生成一个基本的平面
        for (int i = 0; i < terrainSize; i++)
            for (int j = 0; j < terrainSize; j++)
            {
                //使用GetPixel读取高度图的灰度，计算所生成点的高度
                vertexs.Add(new Vector3(i - terrainSize / 2, j - terrainSize / 2, terrainHeight));

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

        //verts.Clear();
        for (int j = 0; j < vertexs.Count; j++)
        {
            cList[j] = new Color(1, 1, 1);
        }
            plane.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);








    }

    private void ChangeTerrain()
    {
        
        GameObject pla = GameObject.Find("Terrain");
        for (int j = 0; j < vertexs.Count; j++)
        {
            Vector3 plaScreenPos = Camera.main.WorldToScreenPoint(vertexs[j]);
            float dis = (plaScreenPos.x - mousePos.x) * (plaScreenPos.x - mousePos.x) + (plaScreenPos.y - mousePos.y) * (plaScreenPos.y - mousePos.y);
            if (dis < pencilWidth)
            {
                cList[j] = new Color(0, 0, 0);
                //print(dis);
            }
            

        }


        pla.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);

    }


    private void SaveColor()
    {
        
        for(int j = 0; j < SavedColorList.Length; j++)
        {
            int i = j * cList.Length / SavedColorList.Length;
            SavedColorList[j] = (int)cList[i].r;
        }
        print("Saved Succeed");
    }

    private void ClearColor()
    {
        for (int j = 0; j < cList.Length; j++)
        {
            cList[j] = new Color(1, 1, 1);
        }
    }
}





    
