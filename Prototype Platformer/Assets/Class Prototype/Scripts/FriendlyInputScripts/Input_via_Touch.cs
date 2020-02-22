using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_Touch : MonoBehaviour
{
    private bool Player1_jump,Player1_Attack,Player1_Pickup,Player1_Door,Player1_Menu;
    private float Player1_Right = 0;
    public void Player1Jump(bool val) { Player1_jump = val; }
    public void Player1Attack(bool val) { Player1_Attack = val; }
    public void Player1Pickup(bool val) { Player1_Pickup = val; }
    public void Player1Door(bool val) { Player1_Door = val; }
    public void Player1Menu(bool val) { Player1_Menu = val; }
    public void Player1Left(bool val) { if (val) Player1_Right = -1; else Player1_Right = 0; }
    public void Player1Right(bool val) { if (val) Player1_Right = 1; else Player1_Right = 0; }




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

        c.jump = Player1_jump;
        c.attack = Player1_Attack;
        c.door = Player1_Door;
        c.B = Player1_Door;
        c.action = Player1_Pickup;

        c.inGameMenu = Player1_Menu;

        c.moveLeft = Player1_Right;

        player1.Invoke(c);
    }
}
