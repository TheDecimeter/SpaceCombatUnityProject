using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AI : MonoBehaviour
{
    private const int North = 1, South = 2, East = 4, West = 8;
    private float StartX, StartY;
    private CharacterMovement_Physics player;
    public LevelRandomizer levelStats;
    private int targetX, targetY;
    Rigidbody rb;
    private float roomCell;

    private HashSet<Collision> EastObstructions = new HashSet<Collision>();
    private HashSet<Collision> WestObstructions = new HashSet<Collision>();

    private const int ignoreLayer= (1 << 14) | (1 << 15) | (1 << 16) | (1 << 17);

    private bool [][] levelPath;
    private int[][] roomPath;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<CharacterMovement_Physics>();
        StartY = levelStats.startY - levelStats.yTileSize/2;
        StartX = levelStats.startX;
        roomCell = levelStats.yTileSize / 2;
        

        levelPath = new bool[levelStats.MapDemensionsY][];
        for (int i = 0; i < levelStats.MapDemensionsY; ++i)
        {
            levelPath[i] = new bool[levelStats.MapDemensionsX];
        }
        roomPath = new int[2][];
        for (int i = 0; i < 2; ++i)
        {
            roomPath[i] = new int[3];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //print(FindNearestExit());
        int x;
        int y;
        //GetRoomGrid(transform.position, out x, out y);
        //MapRoom();
    }

    private void SetLevelPath()
    {
        
    }

    private int MapRoom()
    {
        ResetRoomPath();
        int gridx; int gridy;
        GetGridPos(transform.position,out gridx, out gridy);
        int roomx; int roomy;
        GetRoomGrid(transform.position, out roomx, out roomy);

        Queue<MapNode> n = new Queue<MapNode>();
        Vector3 centerOfRoomGrid = GetCenterOfRoomGrid(transform.position);

        n.Enqueue(new MapNode(roomx, roomy, 1, centerOfRoomGrid));

        RaycastHit h;
        int player, tmpPlayer;
        
        //For some reason ignoring a layer seems to ignore all layers on ray cast.
        // I couldn't find anything about this bug for this verison of unity, but it
        // just isn't working, therefore if a ray intersects a player I just assume the path
        // is clear (going to kill that player anyway)
        while (n.Count > 0)
        {
            MapNode c = n.Dequeue();

            if (c.x < 0 || c.x > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x] != 0)
                continue;
            //print("at " + c.x + "," + c.y);
            roomPath[c.y][c.x] = c.cost;

            //Debug.DrawRay(c.loc, Vector3.left * roomCell * .8f);
            //Debug.DrawRay(c.loc, Vector3.right * roomCell * .8f, Color.red);
            //Debug.DrawRay(c.loc, Vector3.up * roomCell * .8f, Color.gray);
            //Debug.DrawRay(c.loc, Vector3.down * roomCell * .8f, Color.blue);
            //check left
            if (!(c.x - 1 < 0 || c.x - 1 > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x - 1] != 0))
            {
                if (Physics.Raycast(c.loc, Vector3.left, out h, roomCell * .9f))
                {
                    if(isAPlayer(h, out player))
                    {
                        if (Physics.Raycast(new Vector3(c.loc.x - roomCell, c.loc.y, c.loc.z), Vector3.right, out h, roomCell * .9f))
                            if(isAPlayer(h,out tmpPlayer)&&tmpPlayer==player)
                                n.Enqueue(new MapNode(c.x - 1, c.y, c.cost + 1, new Vector3(c.loc.x - roomCell, c.loc.y, c.loc.z)));
                    }
                }
                else
                    n.Enqueue(new MapNode(c.x - 1, c.y, c.cost + 1, new Vector3(c.loc.x - roomCell, c.loc.y, c.loc.z)));
            }
                
            //check right
            if (!(c.x + 1 < 0 || c.x + 1 > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x + 1] != 0))
            {
                if(Physics.Raycast(c.loc, Vector3.right, out h, roomCell * .9f))
                {
                    if (isAPlayer(h, out player))
                    {
                        if (Physics.Raycast(new Vector3(c.loc.x + roomCell, c.loc.y, c.loc.z), Vector3.left, out h, roomCell * .9f))
                            if (isAPlayer(h, out tmpPlayer) && tmpPlayer == player)
                                n.Enqueue(new MapNode(c.x + 1, c.y, c.cost + 1, new Vector3(c.loc.x + roomCell, c.loc.y, c.loc.z)));
                    }
                }
                else
                    n.Enqueue(new MapNode(c.x + 1, c.y, c.cost + 1, new Vector3(c.loc.x + roomCell, c.loc.y, c.loc.z)));
            }
               
                
            //check up
            if (!(c.x < 0 || c.x > 2 || c.y + 1 < 0 || c.y + 1 > 1 || roomPath[c.y + 1][c.x] != 0))
            {
                if(Physics.Raycast(c.loc, Vector3.up, out h, roomCell * .9f))
                {
                    if(isAPlayer(h, out player))
                        n.Enqueue(new MapNode(c.x, c.y + 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y + roomCell, c.loc.z)));
                }
                else
                    n.Enqueue(new MapNode(c.x, c.y + 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y + roomCell, c.loc.z)));
            }
            //check down
            if (!(c.x < 0 || c.x > 2 || c.y - 1 < 0 || c.y - 1 > 1 || roomPath[c.y - 1][c.x] != 0))
            {
                if(Physics.Raycast(c.loc, Vector3.down, out h, roomCell * .9f))
                {
                    if(isAPlayer(h, out player))
                        n.Enqueue(new MapNode(c.x, c.y - 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y - roomCell, c.loc.z)));
                }
                else
                    n.Enqueue(new MapNode(c.x, c.y - 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y - roomCell, c.loc.z)));
            }
        }

        //string s = "\n";

        //for (int j = 0; j < 3; ++j)
        //{
        //    s += roomPath[1][j];
        //}
        //s += "\n";
        //for (int j = 0; j < 3; ++j)
        //{
        //    s += roomPath[0][j];
        //}
        //s += "\ntile(" + gridx + "," + gridy + ") room (" + roomx + "," + roomy + ")" +
        //    "\n"+centerOfRoomGrid;

        //print(s);
        return complete;
    }

    private bool isAPlayer(RaycastHit h, out int player)
    {
        player = h.transform.gameObject.layer - 13;
        return player>=1 && player<=4;
    }
    

    private void ResetRoomPath()
    {
        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                roomPath[i][j] = 0;
            }
        }
    }


    private void GoThroughDoor(DoorBehavior door)
    {
        int exit = FindNearestExit();
        SetTargetFromExit(exit);
    }

    private void GoToTarget()
    {
        int x;
        int y;
        GetRoomGrid(transform.position, out x, out y);
        if (x != targetX)
        {
            if (x < targetX)
            {
                RaycastHit h;
                if(rb.SweepTest(Vector3.right, out h, roomCell / 2))
                {
                    //look up, look right
                }
            }
        }
    }

    private void SetTargetFromExit(int exit)
    {
        switch (exit)
        {
            case North:
                targetX = 1;
                targetY = 1;
                break;

            case South:
                targetX = 1;
                targetY = 0;
                break;

            case East:
                targetX = 2;
                targetY = 0;
                break;

            case West:
                targetX = 0;
                targetY = 0;
                break;

        }
    }

    private void GetRoomGrid(Vector3 loc, out int x, out int y)
    {
        int gridX;
        int gridY;
        GetGridPos(loc, out gridX, out gridY);
        TileInformation room = levelStats.Map[gridY][gridX];
        Vector3 roomLoc = room.transform.position;
        x = (int)((loc.x - roomLoc.x) / 4);
        y = (int)((loc.y - (roomLoc.y+StartY)) / 3);
        x = Mathf.Clamp(x, 0, 2);
        y = Mathf.Clamp(y, 0, 1);

    }

    private Vector3 GetCenterOfRoomGrid(Vector3 loc)
    {
        int gridX;
        int gridY;
        int x;
        int y;
        GetGridPos(loc, out gridX, out gridY);
        TileInformation room = levelStats.Map[gridY][gridX];
        Vector3 roomLoc = room.transform.position;
        x = (int)((loc.x - roomLoc.x) / 4);
        y = (int)((loc.y - (roomLoc.y + StartY)) / 3);
        x = Mathf.Clamp(x, 0, 2);
        y = Mathf.Clamp(y, 0, 1);

        float centx = roomLoc.x + x * roomCell + roomCell / 2;
        float centy = (roomLoc.y + StartY) + y * roomCell + roomCell / 2;
        return new Vector3(centx, centy, loc.z);
    }
    
    private int FindNearestExit()
    {
        //DoorBehavior d = null;
        int exitDir=0;
        float lowestDist = float.MaxValue;
        float dist;
        int x, y;

        GetGridPos(transform.position, out x, out y);

        TileInformation inRoom = levelStats.Map[y][x];
        DoorBehavior South = inRoom.SouthDoors[1];
        DoorBehavior West = inRoom.WestDoors[1];

        print("in Room "+x+","+y+" " + inRoom.gameObject);

        if (IsValid(South))
        {
            dist = Vector3.Distance(South.transform.position, transform.position);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                exitDir = AI.South;
            }
        }
        if (IsValid(West))
        {
            dist = Vector3.Distance(West.transform.position, transform.position);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                exitDir = AI.West;
            }
        }


        if (y < levelStats.MapDemensionsY - 1)
        {
            DoorBehavior East = levelStats.Map[y + 1][x].SouthDoors[1];
            if (IsValid(East))
            {
                dist = Vector3.Distance(East.transform.position, transform.position);
                if (dist < lowestDist)
                {
                    lowestDist = dist;
                    exitDir = AI.East;
                }
            }
        }
        if (x < levelStats.MapDemensionsX - 1)
        {
            DoorBehavior North = levelStats.Map[y][x + 1].WestDoors[1];
            if (IsValid(North))
            {
                dist = Vector3.Distance(North.transform.position, transform.position);
                if (dist < lowestDist)
                {
                    lowestDist = dist;
                    exitDir = AI.North;
                }
            }
        }

        //Debug.DrawLine(transform.position, d.transform.position);

        return exitDir;
    }

    private bool IsValid(DoorBehavior door)
    {
        return door.gameObject.activeInHierarchy&&door.isOpenable;
    }

    private void FindPlayer()
    {
        Transform playerT = FindClosestPlayer();
        //print("closest player is " + playerT.gameObject.name);
        int x, y;
        GetGridPos(playerT.position,out x, out y);
        //print(" they are at " + x + "," + y);
        
    }


    private void GetGridPos(Vector3 loc, out int x, out int y)
    {
        x = (int)((loc.x - StartX) / levelStats.xTileSize);
        y = (int)((loc.y - StartY) / levelStats.yTileSize);
        x = Mathf.Clamp(x, 0, levelStats.MapDemensionsX-1);
        y = Mathf.Clamp(y, 0, levelStats.MapDemensionsY-1);
    }


    private Transform FindClosestPlayer()
    {
        Transform closest=null;
        float closestDist = float.MaxValue;

        foreach (Transform child in levelStats.PlayerArray.transform)
        {
            if (child == this.transform)
                continue;
            PlayerHealth h = child.GetComponent<PlayerHealth>();
            if (h.isDead)
                continue;
            float dist = Vector3.Distance(child.position, transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = child;
            }
        }

        return closest;
    }

    private int Move(float x, float y)
    {
        ControlStruct c = new ControlStruct(ControlStruct.AI);
        if (y > 0)
            c.jump = true;
        else
            c.jump = false;

        if (x != 0)
        {
            x = Mathf.Clamp(x, -1, 1);
            c.moveLeft = x;
        }
        player.ControllerListener(c);
        return inProgress;
    }


    private void OnCollisionStay(Collision col)
    {
        foreach (ContactPoint cp in col.contacts)
        {
            if (isEastObstruction(cp))
            {
                EastObstructions.Add(col);
                WestObstructions.Remove(col);
                return;
            }
        }
        foreach (ContactPoint cp in col.contacts)
        {
            if (isWestObstruction(cp))
            {
                WestObstructions.Add(col);
                EastObstructions.Remove(col);
                return;
            }
        }
        EastObstructions.Remove(col);
        WestObstructions.Remove(col);
    }
    void OnCollisionExit(Collision col)
    {
        EastObstructions.Remove(col);
        WestObstructions.Remove(col);
    }

    private bool isEastObstruction(ContactPoint cp)
    {
        return cp.normal.x > .5f;
    }
    private bool isWestObstruction(ContactPoint cp)
    {
        return cp.normal.x < -.5f;
    }

    private struct MapNode
    {
        public int x;
        public int y;
        public int cost;
        public Vector3 loc;

        public MapNode(int x, int y, int cost, Vector3 loc) {
            //print("created Node (" + x + "," + y + ") " + cost + " " + loc);
            this.x = x;
            this.y = y;
            this.cost = cost;
            this.loc = loc;
        }

    }


    
}
