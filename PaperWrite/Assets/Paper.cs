using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Paper : MonoBehaviour
{


    [Range(0, 250)]
    public int terrainSize = 250;    //����ֽ�ŵĳ���

    [Range(50, 3000)]
    public int pencilWidth = 1000;  //�ʵĴ�ϸ

    public Material terrainMat;     //����������ֽ�ŵĲ���

    
    public Vector3 mousePos;    //�����Ļ�ռ�����
    public Vector3 lastMousePos;//�����һ��λ����Ļ�ռ�����

    public Color[] cList;    //�洢���ж�����ɫ
    public List<Vector3> vertexs = new List<Vector3>();//�������ж�������,�·����еڶ��ֶ�������ķ�ʽ
        
    private List<int> tris = new List<int>();//��������������

    public Color[] SavedColorList=new Color[784];//������������ʶ�����ɫ
    public int[] SavedColorCount = new int[784];//��������ʶ�����ɫ�ļ���
    

    public static Paper _Paper;//Paperʵ��

    private List<Vector3> plaScreenPos=new List<Vector3>();//ֽ����Ļ����

    public int donot = 1;//1��ʾ��Ҫ����һ�����λ��������

    void Start()
    {
        _Paper = this;
        GenerateTerrain();//����ֽ��
        donot = 1;
        mousePos = new Vector3(0, 0, 0);
        lastMousePos = new Vector3(0, 0, 0);
        GameObject slider = GameObject.Find("Slider");
        slider.GetComponent<Slider>().value = pencilWidth;
    }
    void Update()
    {

        
        
        mousePos = Input.mousePosition;//�����Ļ����
        
        if (isClick._instance.click > 0)//��������滭
            ChangeTerrain();

        if (isClick._instance.click < 1)//���̧������һ�λ滭������̧�𴦲�ֵ
            donot = 1;
        
        if (Input.GetKeyDown(KeyCode.C))//���c��ջ���
            ClearColor();
        
        lastMousePos = mousePos;
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
            plaScreenPos.Add(Camera.main.WorldToScreenPoint(vertexs[j]));//��ֽ�Ŷ������������ϵת��Ϊ��Ļ����ϵ


        }
        plane.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);

        





    }

    private void ChangeTerrain()
    {
        
        GameObject pla = GameObject.Find("Terrain");//�ҵ�Terrain����
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
            //��������ľ���
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
                if (dis1 < pencilWidth || dis3 < pencilWidth || dis4 < pencilWidth || dis5 < pencilWidth || dis6 < pencilWidth || dis7 < pencilWidth || dis8 < pencilWidth || dis9 < pencilWidth)//�ھ���������ʾ��ɫ
                {
                    cList[j] = new Color(0, 0, 0);
                    //print(dis);
                }
            }
            else
            {
                if (dis1 < pencilWidth)//�ھ���������ʾ��ɫ
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


        pla.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//������ɫ

    }


    private void ClearColor()
    {
        for (int j = 0; j < cList.Length; j++)
        {
            cList[j] = new Color(1, 1, 1);//��ɫȫ��Ϊ��ɫ
        }
        GameObject plan = GameObject.Find("Terrain");
        plan.GetComponent<MeshFilter>().sharedMesh.SetColors(cList);//������ɫ
    }

    

    private float dis(Vector3 mouse,Vector3 pla)
    {
        float dis1 = (pla.x - mouse.x) * (pla.x - mouse.x) + (pla.y - mouse.y) * (pla.y - mouse.y);

        return dis1;
    }
}





    
