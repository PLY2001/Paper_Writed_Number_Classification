using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change : MonoBehaviour
{
    public Material material;
    public static int state=1;

    

    
    
    void Start()
    {
        //yRotation=0;
        if (material == null)
        {
            GameObject plan = GameObject.Find("Terrian");//ûд

            Renderer renderer = plan.GetComponent<Renderer>();
            if (renderer != null)
            {

                material = renderer.material;//renderer���ж��materialʱ
                
            }
        }

        if (material == null)
        {
            this.enabled = false;
        }
        else
        {


            material.SetFloat("_State", 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            
            ChangeMat();
        }
        

    }

    private void ChangeMat()
    {
        state = -state;
        material.SetFloat("_State", state);
    }
}

