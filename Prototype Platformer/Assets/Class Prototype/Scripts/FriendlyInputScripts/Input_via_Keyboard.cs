using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_Keyboard : MonoBehaviour
{

    public HUD hud;
    public ScrollManager MainMenu;
    public ControlEvents ExitItem;


    [System.Serializable]
    public class keysEvent : UnityEvent<ControlStruct> { }



    public keysEvent Arrow_and_Rshift_keys;
    public keysEvent WASD_and_Space_keys;
    public keysEvent IJKL_and_H_keys;
    public keysEvent NumericPad_and_NumericEnter_keys;
    

    // Update is called once per frame
    void Update()
    {
        sendArrowcontrols();
        sendWASDcontrols();
        sendIJKLcontrols();
        sendNumPadcontrols();

        sendMenu();
    }

    void sendMenu()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (hud.Link == null)
            {
                if(UndestroyableData.IsMainMenuOpened()&&MainMenu.gameObject.activeInHierarchy)
                {
                    MainMenu.ScrollTo(ExitItem);
                    return;
                }
            }
            if (hud.Link.Menu[0].IsGlobalMenuActive())
                hud.Link.Menu[0].SetGlobalMenuActive(false, null);
            else
            {
                hud.Link.Menu[0].SetGlobalMenuActive(true, null);
            }
        }
    }

    private void sendArrowcontrols()
    {
        ControlStruct c = new ControlStruct(ControlStruct.Keyboard);
        c.A = Input.GetKey(KeyCode.Return);
        c.door = Input.GetKey(KeyCode.RightControl) || c.A;

        c.A |= c.jump = Input.GetKey(KeyCode.UpArrow);
        c.attack = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Quote);
        c.B = c.action = Input.GetKey(KeyCode.DownArrow);

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
        ControlStruct c = new ControlStruct(ControlStruct.Keyboard);

        c.A = c.jump = Input.GetKey(KeyCode.W);
        
        c.attack = Input.GetKey(KeyCode.Space);
        c.door = Input.GetKey(KeyCode.B);
        c.B = c.action = Input.GetKey(KeyCode.S);

        c.inGameMenu = Input.GetKey(KeyCode.Tab);

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
        ControlStruct c = new ControlStruct(ControlStruct.Keyboard);

        c.A = c.jump = Input.GetKey(KeyCode.I);
        c.attack = Input.GetKey(KeyCode.H);
        c.door = Input.GetKey(KeyCode.Y);
        c.B = c.action = Input.GetKey(KeyCode.K);

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
        ControlStruct c = new ControlStruct(ControlStruct.Keyboard);

        c.A = c.jump = Input.GetKey(KeyCode.Keypad8);
        c.A |= c.attack = Input.GetKey(KeyCode.KeypadEnter);
        c.door = Input.GetKey(KeyCode.Keypad0);
        c.B = c.action = Input.GetKey(KeyCode.Keypad5);

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
