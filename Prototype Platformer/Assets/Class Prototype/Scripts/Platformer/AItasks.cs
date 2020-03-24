using System.Collections;
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
        return ErrorChecking(ret);
    }
    private int TaskGoToNorthDoor()
    {
        DoorBehavior N;
        if (!TryFindNorthDoor(out N))
            return impossible;

        int doorX, doorY;
        GetRoomGrid(N.GetComponent<Collider>().bounds.center, out doorX, out doorY);
        //Debug.DrawLine(transform.position, N.GetComponent<Collider>().bounds.center,Color.white);
        int Tx = doorX;
        int Ty = 1;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);
        //print(gameObject.name + " going TO nDoor " + Tx+"\n"+roomGridString());
        return ErrorChecking(ret);
    }

    private int TaskGoToEastDoor()
    {
        int Tx = 2;
        int Ty = 0;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);
        //print("going east");
        return ErrorChecking(ret);
    }

    private int TaskGoThroughEastDoor()
    {
        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through east door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;

        if (rb.velocity.x < .1)//don't press the door button too soon or you might get sucked back up
        {
            controls.door = ButtonPresser();
            Move(1, 0);

        }
        else
        {
            Move(1, 0);
        }
        ////print(gameObject.name + " move (" + controls.door + ")");
        return ErrorChecking();
    }

    private int TaskGoToWestDoor()
    {
        int Tx = 0;
        int Ty = 0;

        int x; int y;
        GetRoomGrid(transform.position, out x, out y);
        int ret = GoToTarget(x, y, Tx, Ty);

        //print("going west");
        return ErrorChecking(ret);
    }

    private int TaskGoThroughWestDoor()
    {
        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through West door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;



        if (rb.velocity.x > -.1)//don't press the door button too soon or you might get sucked back up
        {
            controls.door = ButtonPresser();
            Move(-1, 0);

        }
        else
        {
            Move(-1, 0);
        }

        
        ////print(gameObject.name + " move (" + controls.door + ")");
        return ErrorChecking();
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
        return ErrorChecking();
    }
    private int TaskGoThroughNorthDoor()
    {
        DoorBehavior N;
        if (!TryFindNorthDoor(out N))
            return impossible;

        int doorX, doorY;
        GetRoomGrid(N.GetComponent<Collider>().bounds.center, out doorX, out doorY);
        //Debug.DrawLine(transform.position, N.GetComponent<Collider>().bounds.center,Color.yellow);


        int x; int y;
        GetMapGridPos(transform.position, out x, out y);

        //print("  go through north door at(" + x + "," + y + ") old(" + currentMapX + "," + currentMapY + ")");
        if (x != currentMapX || y != currentMapY)
            return complete;

        //GetRoomGrid(transform.position, out x, out y);

        //if (x != doorX || y != doorY)
        //{
        //    print(gameObject.name + " redirect " + new Vector2(x, y) + new Vector2(doorX, doorY));
        //    return TaskGoToNorthDoor();
        //}

        float ox = XoffsetToCenter(.2f);
        //Move(ox, 1);


        if (rb.velocity.y < .01)//don't press the door button too soon or you might get sucked back up
        {
            controls.door = ButtonPresser();
            if (!controls.door)
                Move(ox, 1);

        }
        else
        {
            Move(ox, 1);
        }


        //controls.door = ButtonPresser();
        //print(gameObject.name + " going Through nDoor " + doorX + "\n" + roomGridString());
        ////print(gameObject.name + " move throgh north door (" + controls.door + ")");
        return ErrorChecking();
    }

    private int ErrorChecking(int passThroughStatus=inProgress)
    {
        roomStagnateTimer -= Time.deltaTime;
        if (roomStagnateTimer < 0)
            return impossible;
        return passThroughStatus;
    }

    private int TaskSetMapXY()
    {
        roomStagnateTimer = roomStagnateTime;
        GetMapGridPos(transform.position, out currentMapX, out currentMapY);
        ////print("  Set Map XY (" + currentMapX + "," + currentMapY + ")");
        return complete;
    }

    private int TaskAttackPlayerInRoom(PlayerHealth player)
    {
        //print("\n");
        //print(this.player.gameObject.name+" attacking player " + player.gameObject.name);
        //print("obsticle count " + (EastObstructions.Count + WestObstructions.Count) + "\n for " + gameObject.name);
        if (player.isDead)
            return ErrorChecking(complete);
        


        int pX, pY, mX, mY;

        GetMapGridPos(player.transform.position, out pX, out pY);
        GetMapGridPos(transform.position, out mX, out mY);
        if (mX != pX || mY != pY)//if you aren't in the room as the target
            return impossible;

        GetRoomGrid(player.transform.position, out pX, out pY);
        GetRoomGrid(transform.position, out mX, out mY);

        if (canAttackTarget(player))
        {
            //print(this.player.gameObject.name + " Can Attack player " + player.gameObject.name);
            roomStagnateTimer = .5f;
            //print(gameObject.name + " attacking " + player.gameObject);
            controls.attack = true;
        }
        //else
        //    print(gameObject.name + " not attacking " + player.gameObject);
        //else
        //    controls.attack = false;


        int ret = 0;
        if (mX != pX || mY != pY)
        {

            //print(this.player.gameObject.name + " Going to attack player " + player.gameObject.name);
            ret = GoToTarget(mX, mY, pX, pY);
            return ErrorChecking(ret);
        }

        float ox = XtoTarget(player.transform.position);
        float pow = Random.Range(.25f,.5f);
        if (ox > 0)
            controls.moveLeft = pow;
        else
            controls.moveLeft = -pow;

        ret= ErrorChecking();
        if (ret != impossible)
        {
            //print(this.player.gameObject.name + " Successful Attack/Move " + player.gameObject.name);
            return ret;
        }
        //print(this.player.gameObject.name + " Error correction attack/move " + player.gameObject.name);

        Move(controls.moveLeft, 1);
        return impossible;
    }

    private bool canAttackTarget(PlayerHealth p)
    {
        return Vector2.Distance(p.transform.position,transform.position)<2;
    }


    private int TaskAssignAttackPlayerInRoom(PlayerHealth p)
    {
        TaskList.Push(() => TaskAttackPlayerInRoom(p));
        TaskList.Push(TaskMapRoom);
        roomStagnateTimer = roomStagnateTime;
        return complete;
    }

    private int TaskComplete()
    {
        print("task complete " + player.gameObject.name);
        return complete;
    }

    private int TaskTraverseRoom()
    {
        TaskList.Push(TaskGoToWestDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskGoToEastDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskGoToWestDoor);
        TaskList.Push(TaskMapRoom);
        TaskList.Push(TaskGoToEastDoor);
        TaskList.Push(TaskMapRoom);
        return complete;
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
            {
                //Debug.DrawRay(transform.position, new Vector2(Tx-x,Ty-y)*3, Color.green);
                return response;
            }
        }
        //if you are to the right of the target
        if (x > Tx)
        {
            int response = MoveWest(x, y);
            if (response != impossible)
            {
                //Debug.DrawRay(transform.position, new Vector2(Tx - x, Ty - y) * 3, Color.red);
                return response;
            }
        }
        //if you are below the target
        if (y < Ty)
        {
            int response = MoveNorth(x, y, Tx, Ty);
            if (response != impossible)
            {
                //Debug.DrawRay(transform.position, new Vector2(Tx - x, Ty - y) * 3, Color.white);
                return response;
            }
        }
        else
        {
            int response = MoveSouth(x, y, Tx, Ty);
            if (response != impossible)
            {
                //Debug.DrawRay(transform.position, new Vector2(Tx - x, Ty - y) * 3, Color.black);
                return response;
            }
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
        //print("error correct");
        Move(0, 0);//perform a "nothing" move so that if the player is really stuck they will try to free themselves
        TaskMapRoom();
    }

    private float XoffsetToCenter(float clamp)
    {
        Vector3 center = GetCenterOfRoomGrid(transform.position);
        return Mathf.Clamp(center.x - transform.position.x, -clamp, clamp);
    }

    private float XtoTarget(Vector2 t)
    {
        return (t-(Vector2)transform.position).x;
    }

    private int path(int x, int y)
    {
        if (x > 2||x<0||y>1||y<0)
            return 0;
        return roomPath[y][x];
    }
}
