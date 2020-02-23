﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This portion of AI uses smaller methods found in the AI file
/// to make broader movements and actions.
/// </summary>
public partial class AI : MonoBehaviour
{
    private const int inProgress = 0, impossible = -1, complete = 1;
    private delegate int Task();

    private int TaskAssignGoThroughEastDoor()
    {
        //order is backwards because the task list is a stack
        TaskList.Push(TaskGoThroughEastDoor);
        TaskList.Push(TaskGoToEastDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskSetMapXY);
        return complete;
    }

    private int TaskAssignGoThroughWestDoor()
    {
        //order is backwards because the task list is a stack
        TaskList.Push(TaskGoThroughWestDoor);
        TaskList.Push(TaskGoToWestDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskSetMapXY);
        return complete;
    }

    private int TaskAssignGoThroughSouthDoor()
    {
        //order is backwards because the task list is a stack
        TaskList.Push(TaskGoThroughSouthDoor);
        TaskList.Push(TaskGoToSouthDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskSetMapXY);
        return complete;
    }

    private int TaskAssignGoThroughNorthDoor()
    {
        //order is backwards because the task list is a stack
        TaskList.Push(TaskGoThroughNorthDoor);
        TaskList.Push(TaskGoToNorthDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskSetMapXY);
        return complete;
    }



    private int TaskGoToSouthDoor()
    {
        DoorBehavior S;
        if (!TryFindSouthDoor(out S))
            return impossible;

        int doorX, doorY;
        GetRoomGrid(S.GetComponent<Collider>().bounds.center, out doorX, out doorY);

        int Tx = doorX;
        int Ty = 0;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);
        //print(gameObject.name + " going south " + Tx);
        return ret;
    }
    private int TaskGoToNorthDoor()
    {
        DoorBehavior N;
        if (!TryFindNorthDoor(out N))
            return impossible;

        int doorX, doorY;
        GetRoomGrid(N.GetComponent<Collider>().bounds.center, out doorX, out doorY);
        Debug.DrawLine(transform.position, N.GetComponent<Collider>().bounds.center);
        int Tx = doorX;
        int Ty = 1;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);
        //print(gameObject.name + " going north " + Tx);
        return ret;
    }

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

    private int TaskGoThroughEastDoor()
    {
        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through east door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;
        Move(1, 0);

        controls.door = ButtonPresser();
        ////print(gameObject.name + " move (" + controls.door + ")");
        return inProgress;
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

    private int TaskGoThroughWestDoor()
    {
        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through West door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;
        Move(-1, 0);

        controls.door = ButtonPresser();
        ////print(gameObject.name + " move (" + controls.door + ")");
        return inProgress;
    }
    private int TaskGoThroughSouthDoor()
    {
        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through south door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;
        float ox = XoffsetToCenter(.2f);

        if (rb.velocity.y > -.01)//don't press the door button too soon or you might get sucked back up
        {
            controls.door = ButtonPresser();
            if (!controls.door)
                Move(ox, -1);

        }
        else
        {
            Move(ox, -1);
        }
        ////print(gameObject.name + " move throgh south door (" + controls.door + ")");
        return inProgress;
    }
    private int TaskGoThroughNorthDoor()
    {
        DoorBehavior N;
        if (!TryFindNorthDoor(out N))
            return impossible;

        int doorX, doorY;
        GetRoomGrid(N.GetComponent<Collider>().bounds.center, out doorX, out doorY);
        Debug.DrawLine(transform.position, N.GetComponent<Collider>().bounds.center);


        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through north door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;
        float ox = XoffsetToCenter(.2f);
        Move(ox, 1);

        controls.door = ButtonPresser();
        ////print(gameObject.name + " move throgh north door (" + controls.door + ")");
        return inProgress;
    }

    private int TaskSetMapXY()
    {
        GetMapGridPos(transform.position, out currentMapX, out currentMapY);
        ////print("  Set Map XY (" + currentMapX + "," + currentMapY + ")");
        return complete;
    }

    private int TaskAttackPlayerInRoom(PlayerHealth player)
    {

        //print("obsticle count " + (EastObstructions.Count + WestObstructions.Count) + "\n for " + gameObject.name);
        return inProgress;
    }


    private int GoToTarget(int x, int y, int Tx, int Ty)
    {
        if (x == Tx && y == Ty)
            return complete;

        ////print("  GO TO TARGET at(" + x + "," + y + ") targ(" + Tx + "," + Ty + ")");
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
        ////print("   MOVEeast");
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
        ////print("   MOVEwest");
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
       // print("   MOVEnorth");
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
        {
            //print("moving south, but north is done?");
            //move up
            return Move(ox, 1);
        }

        //You should have moved, you probably got stuck
        ErrorCorrect();
        return inProgress;
    }


    private void ErrorCorrect()
    {
        //print("error correct")
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
