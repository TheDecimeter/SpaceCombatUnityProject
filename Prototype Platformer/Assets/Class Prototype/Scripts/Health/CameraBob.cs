using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    private const int d = 16;
    private float X=.5f, Y=.5f, Xrate, Yrate, Xcur, Ycur;
    private bool _bob = false;
    private float oldX, oldY;

    public bool Bob { get { return _bob; } set {
            if (value == true)
            {
                print("bob = true)");
                if (!_bob)
                {
                    Xcur = 0; Ycur = 0;
                    Xrate = Random.Range(-X/d, X/d);
                    Yrate = Random.Range(-Y/d, Y/d);
                }
            }
            else
            {
                print("bob = false)");
                if(_bob)    
                    transform.localPosition = new Vector3(oldX, oldY, transform.localPosition.z);
            }
            _bob = value;
        } }
    // Start is called before the first frame update
    void Start()
    {
        oldX = transform.localPosition.x;
        oldY = transform.localPosition.y;
        print("x " + oldX + " y " + oldY);
        Bob = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Bob)
        {
            if (Xcur > X || Xcur < -X || Xrate == 0)
            {
                if (Xrate < 0) Xrate = Random.Range(0, X / d);
                else Xrate = Random.Range(-X / d, 0);
            }
            if (Ycur > Y || Ycur < -Y || Yrate == 0)
            {
                if (Yrate < 0) Yrate = Random.Range(0, Y / d);
                else Yrate = Random.Range(-Y / d, 0);
            }

            Xcur += Xrate;
            Ycur += Yrate;

            transform.localPosition = new Vector3(oldX + Xcur, oldY + Ycur, transform.localPosition.z);
        }
    }
    
}
