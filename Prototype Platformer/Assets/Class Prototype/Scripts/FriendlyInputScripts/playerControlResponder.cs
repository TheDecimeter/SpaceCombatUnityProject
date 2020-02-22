using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControlResponder : MonoBehaviour
{
    public int playerNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ControllerListener(ControlStruct controls)
    {
        if (controls.jump)
            print("player " + playerNum + " received the jump instruction");
        if (controls.action)
            print("player " + playerNum + " received the action instruction");
        if (controls.attack)
            print("player " + playerNum + " received the attack instruction");
        if (controls.moveLeft!=0)
            print("player " + playerNum + " received the move instruction "+controls.moveLeft);
    }
}
