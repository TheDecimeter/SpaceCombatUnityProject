using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStruct
{
    private const float deadZone = .1f;
    public const int Keyboard = 1<<0, Mobile = 1<<1, Controller = 1<<2, None = 0, AI = 1<<3;
    public const int Device1 = 1 << 4, Device2 = 1 << 5, Device3 = 1 << 6, Device4 = 1 << 7, 
        Device5 = 1 << 8, Device6 = 1 << 9, Device7 = 1 << 10, Device8 = 1 << 11, Device9 = 1 << 12, Device10 = 1 << 13;
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
    public void addDevice(int index)
    {
        switch (index)
        {
            case 1:
                addSource(Device1);
                break;
            case 2:
                addSource(Device2);
                break;
            case 3:
                addSource(Device3);
                break;
            case 4:
                addSource(Device4);
                break;
        }
    }

    public static int GetDevice(int index)
    {
        switch (index)
        {
            case 1:
                return Device1;
            case 2:
                return Device2;
            case 3:
                return Device3;
            case 4:
                return Device4;
            case 5:
                return Device5;
            case 6:
                return Device6;
            case 7:
                return Device7;
            case 8:
                return Device8;
            case 9:
                return Device9;
            default:
                return Device10;
        }
    }

    public string Sources()
    {
        string r = "";
        if ((source & Controller) != 0)
            r += "controller ";
        if ((source & Keyboard) != 0)
            r += "keyboard ";
        if ((source & AI) != 0)
            r += "ai ";
        if ((source & Mobile) != 0)
            r += "mobile ";

        for(int i=0; i<10; ++i)
        {
            if ((source & GetDevice(i)) != 0)
                r += "Device"+i+" ";
        }
        if (r.Length == 0)
            return "No Sources";
        return r;
    }
    public string Input()
    {
        string r = "";
        if (A)
            r += "A ";
        if (B)
            r += "B ";
        if (door)
            r += "door ";
        if (attack)
            r += "attack ";
        if (jump)
            r += "jump ";
        if (inGameMenu)
            r += "inGameMenu ";
        if (moveLeft!=0)
            r += "moveLeft: "+moveLeft+" ";

        if (r.Length == 0)
            return "No Input";
        return r;
    }

    public override string ToString()
    {
        return "Controller Devices: " + Sources()+"   Input: "+Input();
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
        else if ((bLeft & other.source)!=0) //if from same source (eg, game pad)
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

    public bool AnyInput()
    {
        return left != 0
            || B || A || inGameMenu || action || jump || door || attack;
    }
}
