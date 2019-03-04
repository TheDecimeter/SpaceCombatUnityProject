using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [Header("Vertical door=true, horizontal=false")]
    public bool Vertical=false;
    [Header("Which way does the door slide")]
    public bool OpenLeftorDown;
    // Start is called before the first frame update
    public bool isOpenable = true;
    void Start()
    {
        if(isOpenable)
            this.transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setOpenable(bool value)
    {
        isOpenable = value;
        if (!isOpenable)
            this.transform.localScale = new Vector3(1, 1, 1);
        else
            this.transform.localScale = new Vector3(0, 0, 0);
    }
}
