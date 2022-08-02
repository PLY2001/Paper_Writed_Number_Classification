using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toSave : MonoBehaviour
{
    // Start is called before the first frame update
    public int click = 0;
    public static toSave _toSave;
    void Start()
    {
        _toSave = this;
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
