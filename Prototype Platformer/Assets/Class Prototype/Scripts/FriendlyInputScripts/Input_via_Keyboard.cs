﻿using System.Collections;
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
        c.door = Input.GetKey(KeyCode.RightControl);
        c.B = Input.GetKey(KeyCode.RightControl);
        c.action = Input.GetKey(KeyCode.DownArrow);

        c.inGameMenu = Input.GetKey(KeyCode.Slash);

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
        c.door = Input.GetKey(KeyCode.B);
        c.B = Input.GetKey(KeyCode.B);
        c.action = Input.GetKey(KeyCode.S);

        c.inGameMenu = Input.GetKey(KeyCode.Escape);

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
        c.door = Input.GetKey(KeyCode.Y);
        c.B = Input.GetKey(KeyCode.Y);
        c.action = Input.GetKey(KeyCode.K);

        c.inGameMenu = Input.GetKey(KeyCode.Backslash);

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
        c.door = Input.GetKey(KeyCode.Keypad0);
        c.B = Input.GetKey(KeyCode.Keypad0);
        c.action = Input.GetKey(KeyCode.Keypad5);

        c.inGameMenu = Input.GetKey(KeyCode.KeypadDivide);

        if (Input.GetKey(KeyCode.Keypad4))
            c.moveLeft = -1;
        else if (Input.GetKey(KeyCode.Keypad6))
            c.moveLeft = 1;
        else
            c.moveLeft = 0;

        NumericPad_and_NumericEnter_keys.Invoke(c);
    }
}
