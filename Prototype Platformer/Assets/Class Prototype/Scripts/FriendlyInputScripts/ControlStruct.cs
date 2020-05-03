using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStruct
{
    private const float deadZone = .1f;
    public const int Keyboard = 1, Mobile = 2, Controller = 4, None = 0, AI = 16;
    public bool jump { get { return bJump != None; } set { if (value) bJump = source; else bJump = None; } }
    public bool attack { get { return bAttack != None; } set { if (value) bAttack = source; else bAttack = None; } }
    public bool action { get { return bAction != None; } set { if (value) bAction = source; else bAction = None; } }
    public bool door { get { return bDoor != None; } set { if (value) bDoor = source; else bDoor = None; } }
    public bool A { get { return bA != None; } set { if (value) bA = source; else bA = None; } }
    public bool B { get { return bB != None; } set { if (value) bB = source; else bB = None; } }
    public bool inGameMenu { get { return bMenu != None; } set { if (value) bMenu = source; else bMenu = None; } }

    private int bJump, bAttack, bAction, bDoor, bMenu, bLeft, bA, bB;

    public float moveLeft { get { return left; } set { if (Mathf.Abs(value) > deadZone) { bLeft = source; left = value; } else{ bLeft = None; left = 0; } } }
    private float left = 0;

    public int source { get; protected set; }
    public int menuSource { get { return bMenu; } protected set { } }

    public ControlStruct(int source)
    {
        this.source = source;
    }

    public bool fromSource(int source)
    {
        return (this.source & source) != 0;
    }

    public void addSource(int source)
    {
        this.source = (this.source | source);
    }

    public void reset()
    {
        moveLeft = 0;
        jump = false;
        action = false;
        door = false;
        B = false;
        A = false;
        attack = false;
        inGameMenu = false;
    }

    public void combine(ControlStruct other)
    {
        if(other.jump) bJump |=  other.source;
        else           bJump &= ~other.source;

        if(other.attack) bAttack |=  other.source;
        else             bAttack &= ~other.source;

        if(other.action) bAction |=  other.source;
        else             bAction &= ~other.source;
        
        if(other.door)   bDoor |=  other.source;
        else             bDoor &= ~other.source;
        
        if(other.B)      bB |=  other.source;
        else             bB &= ~other.source;
        
        if(other.A)      bA |=  other.source;
        else             bA &= ~other.source;

        if(other.inGameMenu)      bMenu |=  other.source;
        else                      bMenu &= ~other.source;

        if (bLeft == None)
        {
            bLeft = other.bLeft;
            left = other.left;
        }
        else if ((bLeft & other.source)!=0)
        {
            left = other.left;
            bLeft = other.bLeft;
        }
        

        if (left==0)
        {
            if (bLeft == other.bLeft || bLeft == None)
            {
                bLeft = other.bLeft;
                left = other.left;
            }
        }

        //if (Mathf.Abs(moveLeft) < Mathf.Abs(other.moveLeft))
        //{
        //    moveLeft = other.moveLeft;
        //}
        addSource(other.source);

        //if (other.moveLeft > .01 || other.moveLeft < -.01)
        //{
        //    moveLeft = other.moveLeft;
        //}

        //Debug.Log("combined " + bJump);
    }
}
