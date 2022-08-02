using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{

    //public Texture2D heightMap;     //����ָ���ĸ߶�ͼ

    [Range(0, 70f)]     //Range��Ϊһ��Attribute�����Է���������Inspector����Ͻ���
                        //��ק���ı�����ֵ���������С��һ������
    public float terrainHeight = 10;

    [Range(0, 250)]
    public int terrainSize = 250;    //���������ɵĵ��εĳ���

    [Range(50, 3000)]
    public int pencilWidth = 1000;

    public Material terrainMat;     //���������ɵ��εĲ���

    public static int toWrite = 1;
    public Vector3 mousePos;

    private static Color[] cList;
    private List<Vector3> vertexs = new List<Vector3>();
        
    private List<int> tris = new List<int>();

    private static int[] SavedColorList=new int[784];


    //public int grassRowCount = 300;
    //public int grassCountPerPatch = 5;
    //public Material grassMat;	//֮���ָ�������ƵĲ���
    //private List<Vector3> verts;
    //public gameManager Player;


    void Start()
    {
        //verts = new List<Vector3>();
        //FirstGenerateTerrain();      //���ɵ���

        //GenerateGrassArea(grassRowCount, grassCountPerPatch);
        
        
        GenerateTerrain();
    }
    void Update()
    {
        mousePos = Input.mousePosition;//�����Ļ����
        
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
        //Ҫ����һ��ƽ�棬������Ҫ�Զ����䶥�����������
        //List<Vector3> vertexs = new List<Vector3>();
        //List<int> tris = new List<int>();


        //����ѭ��������һ��������ƽ��
        for (int i = 0; i < terrainSize; i++)
            for (int j = 0; j < terrainSize; j++)
            {
                //ʹ��GetPixel��ȡ�߶�ͼ�ĻҶȣ����������ɵ�ĸ߶�
                vertexs.Add(new Vector3(i - terrainSize / 2, j - terrainSize / 2, terrainHeight));

                //��������Ķ���
                if (i == 0 || j == 0)
                    continue;

                //��tris���vertex���������������Ϊ��ÿ�������㡰�໥��������������������
                tris.Add(terrainSize * i + j);
                tris.Add(terrainSize * i + j - 1);
                tris.Add(terrainSize * (i - 1) + j - 1);
                tris.Add(terrainSize * (i - 1) + j - 1);
                tris.Add(terrainSize * (i - 1) + j);
                tris.Add(terrainSize * i + j);
            }
        cList = new Color[vertexs.Count];//��ɫ
        //����uv
        Vector2[] uvs = new Vector2[vertexs.Count];
        for (var i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(vertexs[i].x, vertexs[i].z);

        //����һ����ΪTerrain��GameObject�������������
        GameObject plane = new GameObject("Terrain");
        plane.AddComponent<MeshFilter>();
        MeshRenderer renderer = plane.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = terrainMat;

        //����һ��mesh���������ǵ��������ݣ�������mesh�������ɵ�Terrain
        Mesh groundMesh = new Mesh();
        groundMesh.vertices = vertexs.ToArray();
        groundMesh.uv = uvs;
        groundMesh.triangles = tris.ToArray();
        //���¼��㷨��
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





    
