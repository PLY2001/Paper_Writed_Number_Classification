using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isClick : MonoBehaviour
{
    public int click = 0;
    public static isClick _instance;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        click = 1;

    }

    private void OnMouseUp()
    {
        click = 0;

    }
}

