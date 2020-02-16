﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AI : MonoBehaviour
{
    private const int inProgress = 0, impossible = -1, complete = 1;
    private delegate int Task();

    private int TaskGoToEastDoor()
    {
        int Tx = 2;
        int Ty = 0;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);
        //print("going east");
        return ret;
    }
    private int TaskGoToWestDoor()
    {
        int Tx = 0;
        int Ty = 0;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);

        //print("going west");
        return ret;
    }

    private int TaskGoThroughEastDoor()
    {
        int x; int y;
        GetGridPos(transform.position, out x, out y);
        if (x != currentMapX || y != currentMapY)
            return complete;
        Move(1, 0);

        controls.door = !controls.door;
        //print(gameObject.name + " move (" + controls.door + ")");
        return inProgress;
    }

    private int TaskSetMapXY()
    {
        GetGridPos(transform.position, out currentMapX, out currentMapY);
        return complete;
    }


    private int GoToTarget(int x, int y, int Tx, int Ty)
    {
        if (x == Tx && y == Ty)
            return complete;

        //print("  GO TO TARGET at(" + x + "," + y + ") targ(" + Tx + "," + Ty + ")");
        //if you are to the west of target
        if (x < Tx)
        {
            int response = MoveEast(x, y);
            if (response != impossible)
                return response;
        }
        //if you are to the right of the target
        if (x > Tx)
        {
            int response = MoveWest(x, y);
            if (response != impossible)
                return response;
        }
        //if you are below the target
        if (y < Ty)
        {
            int response = MoveNorth(x, y, Tx, Ty);
            if (response != impossible)
                return response;
        }
        else
        {
            int response = MoveSouth(x, y, Tx, Ty);
            if (response != impossible)
                return response;
        }
        return impossible;
    }

    private int MoveEast(int x, int y)
    {
        //print("   MOVEeast");
        float ox = XoffsetToCenter(.2f);
        int current = path(x, y);
        if (path(x + 1, y) == current + 1)
            //move right
            return Move(1, 0);
        if (path(x, y + 1) == current + 1)
            //move up
            return Move(ox, 1);
        if (path(x, y - 1) == current + 1)
            //move down
            return Move(ox, -1);
        if (path(x - 1, y) == current + 1)
            //move left
            return Move(-1, 0);
        //You should have moved, you probably got stuck
        ErrorCorrect();
        return inProgress;
    }

    private int MoveWest(int x, int y)
    {
        //print("   MOVEwest");
        float ox = XoffsetToCenter(.2f);
        int current = path(x, y);
        if (path(x - 1, y) == current + 1)
            //move left
            return Move(-1, 0);
        if (path(x, y + 1) == current + 1)
            //move up
            return Move(ox, 1);
        if (path(x, y - 1) == current + 1)
            //move down
            return Move(ox, -1);
        if (path(x + 1, y) == current + 1)
            //move right
            return Move(1, 0);
        //You should have moved, you probably got stuck
        ErrorCorrect();
        return inProgress;
    }

    private int MoveNorth(int x, int y, int Tx, int Ty)
    {
        //print("   MOVEnorth");
        float ox = XoffsetToCenter(.2f);
        int current = path(x, y);
        if (path(x, y + 1) == current + 1)
            //move up
            return Move(ox, 1);

        if (x < Tx)
        {
            if (path(x - 1, y) == current + 1)
                //move left
                return Move(-1, 0);
            if (path(x + 1, y) == current + 1)
                //move right
                return Move(1, 0);
        }
        else
        {
            if (path(x + 1, y) == current + 1)
                //move right
                return Move(1, 0);
            if (path(x - 1, y) == current + 1)
                //move left
                return Move(-1, 0);
        }

        //this should really never happen
        if (path(x, y - 1) == current + 1)
            //move down
            return Move(ox, -1);

        //You should have moved, you probably got stuck
        ErrorCorrect();
        return inProgress;
    }

    private int MoveSouth(int x, int y, int Tx, int Ty)
    {
        //print("   MOVEsouth");
        float ox = XoffsetToCenter(.2f);
        int current = path(x, y);
        if (path(x, y - 1) == current + 1)
            //move down
            return Move(ox, -1);

        if (x < Tx)
        {
            if (path(x - 1, y) == current + 1)
                //move left
                return Move(-1, 0);
            if (path(x + 1, y) == current + 1)
                //move right
                return Move(1, 0);
        }
        else
        {
            if (path(x + 1, y) == current + 1)
                //move right
                return Move(1, 0);
            if (path(x - 1, y) == current + 1)
                //move left
                return Move(-1, 0);
        }

        //this should really never happen
        if (path(x, y + 1) == current + 1)
            //move up
            return Move(ox, 1);

        //You should have moved, you probably got stuck
        ErrorCorrect();
        return inProgress;
    }


    private void ErrorCorrect()
    {
        TaskMapRoom();
    }

    private float XoffsetToCenter(float clamp)
    {
        Vector3 center = GetCenterOfRoomGrid(transform.position);
        return Mathf.Clamp(center.x - transform.position.x, -clamp, clamp);
    }

    private int path(int x, int y)
    {
        if (x > 2||x<0||y>1||y<0)
            return 0;
        return roomPath[y][x];
    }
}
