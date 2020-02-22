using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBobMenu : MonoBehaviour
{
    private const float d = 5f;
    private float X=.2f, Y=.2f, Xcur, Ycur;
    private bool _bob = false;
    private float oldX, oldY;
    private bool which = false;
    public float Xrate=5, Yrate=5;


    // Start is called before the first frame update
    void Start()
    {
        oldX = transform.localPosition.x;
        oldY = transform.localPosition.y;
        Xcur = 0; Ycur = 0;
    }

    // Update is called once per frame
    void Update()
    {
            if (which)
                Xrate *= -1;
            else
                Yrate *= -1;
            which = !which;


            Xcur += Xrate;
            Ycur += Yrate;

            transform.localPosition = new Vector3(oldX + Xcur, oldY + Ycur, transform.localPosition.z);
        
    }
    
}
