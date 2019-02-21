using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_Keyboard : MonoBehaviour
{

    [System.Serializable]
    public class keysEvent : UnityEvent<ControlStruct> { }



    public keysEvent Arrow_and_Rshift_keys;
    public keysEvent WASD_and_Space_keys;
    public keysEvent IJKL_and_H_keys;
    public keysEvent NumericPad_and_NumericEnter_keys;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sendArrowcontrols();
        sendWASDcontrols();
        sendIJKLcontrols();
        sendNumPadcontrols();
    }

    private void sendArrowcontrols()
    {
        ControlStruct c = new ControlStruct();

        c.jump = Input.GetKey(KeyCode.UpArrow);
        c.attack = Input.GetKey(KeyCode.RightShift);
        c.action = Input.GetKey(KeyCode.DownArrow);

        if (Input.GetKey(KeyCode.LeftArrow))
            c.moveLeft = -1;
        else if (Input.GetKey(KeyCode.RightArrow))
            c.moveLeft = 1;
        else
            c.moveLeft = 0;

        Arrow_and_Rshift_keys.Invoke(c);
    }

    private void sendWASDcontrols()
    {
        ControlStruct c = new ControlStruct();

        c.jump = Input.GetKey(KeyCode.W);
        c.attack = Input.GetKey(KeyCode.Space);
        c.action = Input.GetKey(KeyCode.S);

        if (Input.GetKey(KeyCode.A))
            c.moveLeft = -1;
        else if (Input.GetKey(KeyCode.D))
            c.moveLeft = 1;
        else
            c.moveLeft = 0;

        WASD_and_Space_keys.Invoke(c);
    }

    private void sendIJKLcontrols()
    {
        ControlStruct c = new ControlStruct();

        c.jump = Input.GetKey(KeyCode.I);
        c.attack = Input.GetKey(KeyCode.H);
        c.action = Input.GetKey(KeyCode.K);

        if (Input.GetKey(KeyCode.J))
            c.moveLeft = -1;
        else if (Input.GetKey(KeyCode.L))
            c.moveLeft = 1;
        else
            c.moveLeft = 0;

        IJKL_and_H_keys.Invoke(c);
    }

    private void sendNumPadcontrols()
    {
        ControlStruct c = new ControlStruct();

        c.jump = Input.GetKey(KeyCode.Keypad8);
        c.attack = Input.GetKey(KeyCode.KeypadEnter);
        c.action = Input.GetKey(KeyCode.Keypad5);

        if (Input.GetKey(KeyCode.Keypad4))
            c.moveLeft = -1;
        else if (Input.GetKey(KeyCode.Keypad6))
            c.moveLeft = 1;
        else
            c.moveLeft = 0;

        NumericPad_and_NumericEnter_keys.Invoke(c);
    }
}
