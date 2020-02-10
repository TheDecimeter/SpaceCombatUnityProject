using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_Touch : MonoBehaviour
{
    private bool Player1_jump;
    public void Player1Jump(bool val) { print("jump input"); Player1_jump = val ; }




    [System.Serializable]
    public class keysEvent : UnityEvent<ControlStruct> { }


    public keysEvent player1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player1Check();
    }

    private void player1Check()
    {
        ControlStruct c = new ControlStruct(ControlStruct.Mobile);

        if (Player1_jump)
        {
            c.jump = Player1_jump;
            print("jump button");
        }
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

        player1.Invoke(c);
    }
}
