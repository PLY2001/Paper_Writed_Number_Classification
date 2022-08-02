using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Paper : MonoBehaviour
{


    [Range(0, 250)]
    public int terrainSize = 250;    //����ֽ�ŵĳ���

    [Range(50, 3000)]
    public int pencilWidth = 1000;  //�ʵĴ�ϸ

    public Material terrainMat;     //����������ֽ�ŵĲ���

    
    public Vector3 mousePos;    //�����Ļ�ռ�����

    public Color[] cList;    //�洢���ж�����ɫ
    public List<Vector3> vertexs = new List<Vector3>();//�������ж�������
        
    private List<int> tris = new List<int>();//��������������

    public Color[] SavedColorList=new Color[784];//������������ʶ�����ɫ
    public int[] SavedColorCount = new int[784];//��������ʶ�����ɫ�ļ���
    

    public static Paper _Paper;//Paperʵ��

    //private Vector3[] plaScreenPos;//ֽ����Ļ����

   

    void Start()
    {
        _Paper = this;
        GenerateTerrain();//����ֽ��
    }
    void Update()
    {
        mousePos = Input.mousePosition;//�����Ļ����
       
        if (isClick._instance.click > 0)//��������滭
            ChangeTerrain();
        
        if (Input.GetKeyDown(KeyCode.C))//���c��ջ���
            ClearColor();
    }

    

    private void GenerateTerrain()
    {
        //����ѭ��������һ��������ƽ��
        for (int i = 0; i < terrainSize; i++)
            for (int j = 0; j < terrainSize; j++)
            {
                
                vertexs.Add(new Vector3(j - terrainSize / 2, terrainSize / 2-i, 0));//������һ��һ����������

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

        //ֽ�ų�ʼ��Ϊ��ɫ
        for (int j = 0; j < vertexs.Count; j++)
        {
            cList[j] = new Color(1, 1, 1);

        }
        plane.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);

        





    }

    private void ChangeTerrain()
    {
        
        GameObject pla = GameObject.Find("Terrain");//�ҵ�Terrain����
        for (int j = 0; j < vertexs.Count; j++)
        {
            //��������ľ���
            Vector3 plaScreenPos = Camera.main.WorldToScreenPoint(vertexs[j]);//��ֽ�Ŷ������������ϵת��Ϊ��Ļ����ϵ
            float dis = (plaScreenPos.x - mousePos.x) * (plaScreenPos.x - mousePos.x) + (plaScreenPos.y - mousePos.y) * (plaScreenPos.y - mousePos.y);
            if (dis < pencilWidth)//�ھ���������ʾ��ɫ
            {
                cList[j] = new Color(0, 0, 0);
                //print(dis);
            }
            

        }


        pla.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//������ɫ

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
            cList[j] = new Color(1, 1, 1);//��ɫȫ��Ϊ��ɫ
        }
        GameObject plan = GameObject.Find("Terrain");
        plan.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//������ɫ
    }
}





    
