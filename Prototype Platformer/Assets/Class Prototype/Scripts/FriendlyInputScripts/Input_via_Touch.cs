using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_Touch : MonoBehaviour
{
    private bool Player_jump,Player_Attack,Player_Pickup,Player_Door,Player_Menu;
    private float Player_Right = 0;
    public void PlayerJump(bool val) { Player_jump = val; }
    public void PlayerAttack(bool val) { Player_Attack = val; }
    public void PlayerPickup(bool val) { Player_Pickup = val; }
    public void PlayerDoor(bool val) { Player_Door = val; }
    public void PlayerMenu(bool val) { Player_Menu = val; }
    public void PlayerLeft(bool val) { if (val) Player_Right = -1; else Player_Right = 0; }
    public void PlayerRight(bool val) { if (val) Player_Right = 1; else Player_Right = 0; }




    [System.Serializable]
    public class keysEvent : UnityEvent<ControlStruct> { }


    public keysEvent Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerCheck();
    }

    private void PlayerCheck()
    {
        ControlStruct c = new ControlStruct(ControlStruct.Mobile);

        c.jump = Player_jump;
        c.attack = Player_Attack;
        c.door = Player_Door;
        c.B = Player_Door;
        c.action = Player_Pickup;

        c.inGameMenu = Player_Menu;

        c.moveLeft = Player_Right;

        Player.Invoke(c);
    }
}
