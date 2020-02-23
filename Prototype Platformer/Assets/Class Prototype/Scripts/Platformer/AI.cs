using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AI : MonoBehaviour
{

    public LevelRandomizer levelStats;

    private CharacterMovement_Physics player;
    private ControlStruct controls = new ControlStruct(ControlStruct.AI);

    //metrics
    private float StartX, StartY, roomCell;

    private float buttonPressTimer = 0;
    private const float buttonPressTime = .3f;

    //status checking
    private Rigidbody rb;
    private int currentMapX,currentMapY;

    private HashSet<Collision> EastObstructions = new HashSet<Collision>();
    private HashSet<Collision> WestObstructions = new HashSet<Collision>();

    //Tasks and planning
    private Stack<Task> TaskList = new Stack<Task>();
    private Task currentTask;
    private int priority;
    private List<IChecker> GoalCheckers = new List<IChecker>();
    private byte[][] roomPath;

    private const int ignoreLayer= ~((1 << 14) | (1 << 15) | (1 << 16) | (1 << 17));

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<CharacterMovement_Physics>();

        StartY = levelStats.startY - levelStats.yTileSize/2;
        StartX = levelStats.startX;
        roomCell = levelStats.yTileSize / 2;
        

        roomPath = new byte[2][];
        for (int i = 0; i < 2; ++i)
        {
            roomPath[i] = new byte[3];
        }
        priority = 5;
        GoalCheckers.Add(new CheckerKillNearestPlayer(this,priority));

        //TaskList.Push(TaskAssignGoThroughNorthDoor);
        //currentTask = TaskList.Pop();
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (IChecker checker in GoalCheckers)
            priority=checker.Do(priority);

        int count = 100;
        controls.reset();
        int status = complete;
        while (status == complete)
        {
            count -= 1;
            if (count < 0)
            {
                Debug.LogError("Had to use loop counter task update");
                break;
            }
            status = currentTask();
            if (status == complete)
            {
                if (TaskList.Count > 0)
                    currentTask = TaskList.Pop();
                else
                {
                    print(gameObject.name+ " end of task list " + TaskList.Count);
                    break;
                }
                
            }
        }

        if (status == impossible)
            print(gameObject.name + "      IMPOSSIBLE !");

        player.ControllerListener(controls);
        //print(FindNearestExit());
        //int x;
        //int y;
        //GetRoomGrid(transform.position, out x, out y);
        //MapRoom();
    }
    

    private void SetLevelPath()
    {
        
    }

    private int TaskMapRoom()
    {
        ResetRoomPath();
        int gridx; int gridy;
        GetMapGridPos(transform.position,out gridx, out gridy);
        int roomx; int roomy;
        GetRoomGrid(transform.position, out roomx, out roomy);

        Queue<MapNode> n = new Queue<MapNode>();
        Vector3 centerOfRoomGrid = GetCenterOfRoomGrid(transform.position);

        n.Enqueue(new MapNode(roomx, roomy, 1, centerOfRoomGrid));

        RaycastHit h;
        
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

            Debug.DrawRay(c.loc, Vector3.left * roomCell * .9f);
            Debug.DrawRay(c.loc, Vector3.right * roomCell * .9f, Color.red);
            Debug.DrawRay(c.loc, Vector3.up * roomCell * .9f, Color.gray);
            Debug.DrawRay(c.loc, Vector3.down * roomCell * .9f, Color.blue);
            //check left
            if (!(c.x - 1 < 0 || c.x - 1 > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x - 1] != 0))
            {
                if (!Physics.Raycast(c.loc, Vector3.left, out h, roomCell * .9f,ignoreLayer,QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x - 1, c.y, c.cost + 1, new Vector3(c.loc.x - roomCell, c.loc.y, c.loc.z)));
                else
                    print("hit " + h.collider.gameObject.name);
            }
                
            //check right
            if (!(c.x + 1 < 0 || c.x + 1 > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x + 1] != 0))
            {
                if(!Physics.Raycast(c.loc, Vector3.right, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x + 1, c.y, c.cost + 1, new Vector3(c.loc.x + roomCell, c.loc.y, c.loc.z)));
                else
                    print("hit " + h.collider.gameObject.name);
            }
               
                
            //check up
            if (!(c.x < 0 || c.x > 2 || c.y + 1 < 0 || c.y + 1 > 1 || roomPath[c.y + 1][c.x] != 0))
            {
                if(!Physics.Raycast(c.loc, Vector3.up, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x, c.y + 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y + roomCell, c.loc.z)));
                else
                    print("hit " + h.collider.gameObject.name);
            }
            //check down
            if (!(c.x < 0 || c.x > 2 || c.y - 1 < 0 || c.y - 1 > 1 || roomPath[c.y - 1][c.x] != 0))
            {
                if (!Physics.Raycast(c.loc, Vector3.down, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x, c.y - 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y - roomCell, c.loc.z)));
                else
                    print("hit " + h.collider.gameObject.name);
            }
        }

        

        print(roomGridString());
        return complete;
    }

    private string roomGridString()
    {
        string s = "\n";

        for (int j = 0; j < 3; ++j)
        {
            s += roomPath[1][j];
        }
        s += "\n";
        for (int j = 0; j < 3; ++j)
        {
            s += roomPath[0][j];
        }
        s += "\n";
        return s;
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

    
    

    

    private void GetRoomGrid(Vector3 loc, out int x, out int y)
    {
        int gridX;
        int gridY;
        GetMapGridPos(loc, out gridX, out gridY);
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
        GetMapGridPos(loc, out gridX, out gridY);
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

    private bool TryFindSouthDoor(out DoorBehavior door)
    {
        int x, y;

        GetMapGridPos(transform.position, out x, out y);

        TileInformation inRoom = levelStats.Map[y][x];
        DoorBehavior South = inRoom.SouthDoors[1];


        if (IsValid(South))
        {
            door = South;
            return true;
        }
        door = null;
        return false;
    }
    private bool TryFindWestDoor(out DoorBehavior door)
    {
        int x, y;
        GetMapGridPos(transform.position, out x, out y);

        TileInformation inRoom = levelStats.Map[y][x];
        DoorBehavior West = inRoom.WestDoors[1];


        if (IsValid(West))
        {
            door = West;
            return true;
        }
        door = null;
        return false;
    }

    private bool TryFindEastDoor(out DoorBehavior door)
    {
        int x, y;
        GetMapGridPos(transform.position, out x, out y);
        
        if (x < levelStats.MapDemensionsX - 1)
        {
            DoorBehavior North = levelStats.Map[y][x + 1].WestDoors[1];
            if (IsValid(North))
            {
                door = North;
                return true;
            }
        }
        door = null;
        return false;
    }
    private bool TryFindNorthDoor(out DoorBehavior door)
    {
        int x, y;
        GetMapGridPos(transform.position, out x, out y);

        if (y < levelStats.MapDemensionsY - 1)
        {
            DoorBehavior East = levelStats.Map[y + 1][x].SouthDoors[1];
            if (IsValid(East))
            {
                door = East;
                return true;
            }
        }
        door = null;
        return false;
    }

    private int FindNearestExit()
    {
        //DoorBehavior d = null;
        int exitDir = 0;
        float lowestDist = float.MaxValue;
        float dist;
        int x, y;

        GetMapGridPos(transform.position, out x, out y);

        TileInformation inRoom = levelStats.Map[y][x];
        DoorBehavior South = inRoom.SouthDoors[1];
        DoorBehavior West = inRoom.WestDoors[1];

        print("in Room " + x + "," + y + " " + inRoom.gameObject);

        if (IsValid(South))
        {
            dist = Vector3.Distance(South.transform.position, transform.position);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                exitDir = -1;
            }
        }
        if (IsValid(West))
        {
            dist = Vector3.Distance(West.transform.position, transform.position);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                exitDir = -2;
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
                    exitDir = 2;
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
                    exitDir = 1;
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
        GetMapGridPos(playerT.position,out x, out y);
        //print(" they are at " + x + "," + y);
        
    }


    private void GetMapGridPos(Vector3 loc, out int x, out int y)
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
        //print("Move (" + x + "," + y + ")\n"+roomGridString());
        if (y > 0)
        {
            controls.jump = ButtonPresser();
            print("Move jump " + controls.jump + " " + x);
        }
        else
            controls.jump = false;

        if (x != 0)
        {
            x = Mathf.Clamp(x, -1, 1);
            if ((x < 0 && WestObstructions.Count > 0) || (x > 0 && EastObstructions.Count > 0))
            {
                if (!controls.jump)
                {
                    if (x > 0)
                        foreach (Collision c in EastObstructions)
                            print("obsticle east " + c.gameObject.name);
                    if(x < 0)
                        foreach (Collision c in WestObstructions)
                            print("obsticle west " + c.gameObject.name);
                    controls.jump = ButtonPresser();
                    //print("Move obsticle jump " + controls.jump + " " + x);
                }
            }
            //else
            //    print("Move " + x);
            controls.moveLeft = x;
        }
        return inProgress;
    }


    private bool ButtonPresser()
    {
        buttonPressTimer += Time.deltaTime;
        if (buttonPressTimer > buttonPressTime)
        {
            if (buttonPressTimer > buttonPressTime * 2)
                buttonPressTimer = 0;
            return false;
        }
        else
            return true;
    }

    


    private void OnCollisionEnter(Collision col)
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

        if (cp.normal.x > -.1f)
            return false;
        return isObsticle(cp); 

    }
    private bool isWestObstruction(ContactPoint cp)
    {

        if (cp.normal.x < .1f)
            return false;
        return isObsticle(cp);
    }
    private bool isObsticle(ContactPoint cp)
    {
        if (cp.otherCollider.gameObject.GetComponent<DoorBehavior>())
            return false;
        return true;
    }

    private struct MapNode
    {
        public int x;
        public int y;
        public byte cost;
        public Vector3 loc;

        public MapNode(int x, int y, int cost, Vector3 loc) {
            //print("created Node (" + x + "," + y + ") " + cost + " " + loc);
            this.x = x;
            this.y = y;
            this.cost = (byte)cost;
            this.loc = loc;
        }

    }


    
}
