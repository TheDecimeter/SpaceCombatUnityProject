using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStruct
{
    public const int Keyboard = 1, Mobile = 2, Controller = 4, None=8, AI=16;
    public bool jump=false, attack=false, action=false, door=false;
    public bool inGameMenu = false, B=false;
    public float moveLeft=0;

    public int source { get; protected set; }

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

    public void combine(ControlStruct other)
    {
        addSource(other.source);

        jump |= other.jump;
        attack |= other.attack;
        action |= other.action;
        door |= other.door;
        inGameMenu |= other.inGameMenu;
        B |= other.B;

        if (other.moveLeft > .01 || other.moveLeft < -.01)
        {
            moveLeft = other.moveLeft;
        }
    }
}
