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
    private int currentMapX, currentMapY;

    private HashSet<GameObject> EastObstructions = new HashSet<GameObject>();
    private HashSet<GameObject> WestObstructions = new HashSet<GameObject>();

    //Stuck checking
    private Vector3 lastPosition;
    private float stagnateTimer = 0, freedomTimer = 0, roomStagnateTimer = 0;
    private const float stagnateTime = .5f, freedomTime = stagnateTime / 2, roomStagnateTime = 2;
    private bool PickUpItem = false;

    //Tasks and planning
    private Stack<Task> TaskList = new Stack<Task>();
    private Task currentTask;
    private int priority;
    private List<IChecker> PeriodicGoalCheckers = new List<IChecker>();
    private List<IChecker> ConstantGoalCheckers = new List<IChecker>();
    private byte[][] roomPath;
    private bool specialDirections = false, up, down, left, right, failed = false;


    private const int ignoreLayer = ~((1 << 14) | (1 << 15) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 25) | (1<<26));

    //debugging
    private string asyncname = "";

    //thread stuff
    private static int AIcount;
    private Doer doer;


    public Vector3 AsyncPosition { get; protected set; }
    public void Init()
    {
        AsyncPosition = transform.position;
    }

    void Awake()
    {
        if (AIcount == 4)
            AIcount = 0;
    }


    void Start()
    {
        asyncname = name;

        lastPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        player = GetComponent<CharacterMovement_Physics>();

        StartY = levelStats.startY - levelStats.yTileSize / 2;
        StartX = levelStats.startX;
        roomCell = levelStats.yTileSize / 2;



        roomPath = new byte[2][];
        for (int i = 0; i < 2; ++i)
        {
            roomPath[i] = new byte[3];
        }


        ConstantGoalCheckers.Add(new CheckerGrabItem(this, 10));


        priority = 0;
        PeriodicGoalCheckers.Add(new CheckerHarm(this, 1, 3));

        PeriodicGoalCheckers.Add(new CheckerGoToNearestPlayer(this, 5));

        PeriodicGoalCheckers.Add(new CheckerKillPlayers(this, 6));

        PeriodicGoalCheckers.Add(new CheckerAvoidAsteroids(this, 15));

        PeriodicGoalCheckers.Add(new CheckerEscapePod(this, 20));

        currentTask = TaskComplete;

        if (player.PlayerNumber + 1 <= GetPlayers())
            enabled = false;

        AIcount++;
        if (AIcount == 4) StartThread();
    }

    private void StartThread()
    {
        if (doer == null)
        {
            doer = new Doer();
            foreach (Transform child in levelStats.PlayerArray.transform)
            {
                AI ai = child.GetComponent<AI>();
                if (ai && ai.enabled)
                    doer.AddRunner(ai.AsyncUpdate);
            }
            doer.Start();

        }
        //if (RunThread == null)
        //{
        //    RunThread = new Thread(AsyncRun);
        //    Runners = new List<Runner>();
        //    KeepRunning = true;

        //    foreach(Transform child in levelStats.PlayerArray.transform)
        //    {
        //        AI ai = child.GetComponent<AI>();
        //        if (ai && ai.enabled)
        //            Runners.Add(ai.AsyncUpdate);
        //    }


        //    RunThread.Start();
        //}
    }

    private void StopThread()
    {
        if (doer != null)
            doer.Stop();
        //if (RunThread != null)
        //{
        //    KeepRunning = false;
        //    Runners = null;
        //    RunThread = null;
        //}
    }

    //private void AsyncRun()
    //{
    //    float deltaTime = 0.1f;
    //    while (KeepRunning&&Runners.Count>0)
    //    {
    //        for (int i = Runners.Count-1; i >= 0; --i)
    //        {
    //            if (Runners[i](deltaTime))
    //                Runners.RemoveAt(i);
    //        }
    //        Thread.Sleep(100);
    //    }
    //}

    bool AsyncUpdate(float deltaTime)
    {
        if (!player._canMove)//player.GetComponent<PlayerHealth>().isDead)
        {
            return true;
        }

        foreach (IChecker checker in PeriodicGoalCheckers)
            priority = checker.Do(priority, deltaTime, failed);
        failed = false;

        return false;
    }

    void OnDestroy()
    {
        StopThread();
    }

    //void FixedUpdate()
    //{
    //    AsyncUpdate(Time.fixedDeltaTime);
    //}

    // Update is called once per frame
    void Update()
    {


        AsyncPosition = transform.position;
        if (!player._canMove)//player.GetComponent<PlayerHealth>().isDead)
        {
            enabled = false;
            return;
        }

        controls.reset();

        foreach (IChecker checker in ConstantGoalCheckers)
            checker.Do(priority, Time.deltaTime, false);

        int count = 100;
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
            {

                Debug.Log(asyncname + " popping next task");
                currentTask = TaskList.Pop();
            }
            else
            {
                print(gameObject.name + " end of task list " + TaskList.Count);
                priority = 0;
                break;
            }

        }
        }

        if (status == impossible)
        {
            failed = true;
            //print(gameObject.name + "      IMPOSSIBLE !");
        }

        player.ControllerListener(controls);
        //print(FindNearestExit());
        //int x;
        //int y;
        //GetRoomGrid(transform.position, out x, out y);
        //MapRoom();
    }

    public void SetDirections(bool up, bool down, bool left, bool right)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        specialDirections = true;
    }

    public void RemoveDirections()
    {
        specialDirections = false;
    }


    private int TaskMapRoom()
    {
        ResetRoomPath();
        int gridx; int gridy;
        GetMapGridPos(transform.position, out gridx, out gridy);
        int roomx; int roomy;
        GetRoomGrid(transform.position, out roomx, out roomy);

        Queue<MapNode> n = new Queue<MapNode>();
        Vector3 centerOfRoomGrid = GetCenterOfRoomGrid(transform.position);

        if (specialDirections)
        {
            //print("special directions ");
            roomPath[roomy][roomx] = 1;
            if (up)
                n.Enqueue(new MapNode(roomx, roomy + 1, 2, new Vector3(centerOfRoomGrid.x, centerOfRoomGrid.y + roomCell, centerOfRoomGrid.z)));
            if (down)
                n.Enqueue(new MapNode(roomx, roomy - 1, 2, new Vector3(centerOfRoomGrid.x, centerOfRoomGrid.y - roomCell, centerOfRoomGrid.z)));
            if (left)
                n.Enqueue(new MapNode(roomx - 1, roomy, 2, new Vector3(centerOfRoomGrid.x - roomCell, centerOfRoomGrid.y, centerOfRoomGrid.z)));
            if (right)
                n.Enqueue(new MapNode(roomx + 1, roomy, 2, new Vector3(centerOfRoomGrid.x + roomCell, centerOfRoomGrid.y, centerOfRoomGrid.z)));
        }
        else
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

            //Debug.DrawRay(c.loc, Vector3.left * roomCell * .9f);
            //Debug.DrawRay(c.loc, Vector3.right * roomCell * .9f, Color.red);
            //Debug.DrawRay(c.loc, Vector3.up * roomCell * .9f, Color.gray);
            //Debug.DrawRay(c.loc, Vector3.down * roomCell * .9f, Color.blue);


            //check left
            if (!(c.x - 1 < 0 || c.x - 1 > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x - 1] != 0))
            {
                if (!Physics.Raycast(c.loc, Vector3.left, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x - 1, c.y, c.cost + 1, new Vector3(c.loc.x - roomCell, c.loc.y, c.loc.z)));
                //else
                //    print("hit " + h.collider.gameObject.name);
            }

            //check right
            if (!(c.x + 1 < 0 || c.x + 1 > 2 || c.y < 0 || c.y > 1 || roomPath[c.y][c.x + 1] != 0))
            {
                if (!Physics.Raycast(c.loc, Vector3.right, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x + 1, c.y, c.cost + 1, new Vector3(c.loc.x + roomCell, c.loc.y, c.loc.z)));
                //else
                //    print("hit " + h.collider.gameObject.name);
            }


            //check up
            if (!(c.x < 0 || c.x > 2 || c.y + 1 < 0 || c.y + 1 > 1 || roomPath[c.y + 1][c.x] != 0))
            {
                if (!Physics.Raycast(c.loc, Vector3.up, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x, c.y + 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y + roomCell, c.loc.z)));
                //else
                //    print("hit " + h.collider.gameObject.name);
            }
            //check down
            if (!(c.x < 0 || c.x > 2 || c.y - 1 < 0 || c.y - 1 > 1 || roomPath[c.y - 1][c.x] != 0))
            {
                if (!Physics.Raycast(c.loc, Vector3.down, out h, roomCell * .9f, ignoreLayer, QueryTriggerInteraction.Ignore))
                    n.Enqueue(new MapNode(c.x, c.y - 1, c.cost + 1, new Vector3(c.loc.x, c.loc.y - roomCell, c.loc.z)));
                //else
                //    print("hit " + h.collider.gameObject.name);
            }
        }



        print(asyncname+" roomGrid\n"+roomGridString());
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
        return player >= 1 && player <= 4;
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
        y = (int)((loc.y - (roomLoc.y + StartY)) / 3);
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

        GetMapGridPos(AsyncPosition, out x, out y);

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
        GetMapGridPos(AsyncPosition, out x, out y);

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
        GetMapGridPos(AsyncPosition, out x, out y);

        if (x < levelStats.MapDemensionsX - 1)
        {
            if (!levelStats.Map[y][x + 1])
            {
                door = null;
                return false;
            }
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
        GetMapGridPos(AsyncPosition, out x, out y);

        if (y < levelStats.MapDemensionsY - 1)
        {
            if (!levelStats.Map[y + 1][x])
            {
                door = null;
                return false;
            }
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

    private int FindNeare33stExit()
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

        ////Debug.DrawLine(transform.position, d.transform.position);

        return exitDir;
    }

    private bool IsValid(DoorBehavior door)
    {
        return door.isOpenable;
    }

    private void FindPlayer()
    {
        Transform playerT = FindClosestPlayer();
        //print("closest player is " + playerT.gameObject.name);
        int x, y;
        GetMapGridPos(playerT.position, out x, out y);
        //print(" they are at " + x + "," + y);

    }


    private void GetMapGridPos(Vector3 loc, out int x, out int y)
    {
        x = (int)((loc.x - StartX) / levelStats.xTileSize);
        y = (int)((loc.y - StartY) / levelStats.yTileSize);
        x = Mathf.Clamp(x, 0, levelStats.MapDemensionsX - 1);
        y = Mathf.Clamp(y, 0, levelStats.MapDemensionsY - 1);
    }


    private Transform FindClosestPlayer()
    {
        Transform closest = null;
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

    /// <summary>
    /// Normally this happens if the AI gets stuck in a door
    /// This should only be called if the AI is trying to move.
    /// </summary>
    /// <returns>true if the ai has been in one place for a while</returns>
    private bool stagnate()
    {
        if ((lastPosition - transform.position).magnitude < .1f)
        {
            if (stagnateTimer > stagnateTime)
                return true;
            stagnateTimer += Time.deltaTime;
            freedomTimer = 0;
            //print("not stagnate time " + stagnateTimer);
            return false;
        }
        //print("not stagnate " + lastPosition + " " + transform.position);
        lastPosition = transform.position;
        stagnateTimer = 0;
        return false;
    }

    /// <summary>
    /// wiggle wiggle wiggle
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int freeYourself(float x, float y)
    {
        if (freedomTimer > freedomTime)
        {
            if (x > 0)
                controls.moveLeft = -1;
            else
                controls.moveLeft = 1;
            controls.jump = controls.door = ButtonPresser();
        }
        else
        {
            controls.door = ButtonPresser();
        }
        freedomTimer += Time.deltaTime;
        if (freedomTimer > stagnateTime)
            freedomTimer = 0;

        return inProgress;
    }

    private int Move(float x, float y)
    {
        if (stagnate())
        {
            //print("    FREEDOM");
            return freeYourself(x, y);
        }
        //print("Move (" + x + "," + y + ")\n"+roomGridString());
        if (y > 0)
        {
            controls.jump = ButtonPresser();
            //print("Move jump " + controls.jump + " " + x);
        }
        else
            controls.jump = false;

        controls.action = PickUpItem;

        if (x != 0)
        {
            x = Mathf.Clamp(x, -1, 1);
            if ((x < 0 && WestObstructions.Count > 0) || (x > 0 && EastObstructions.Count > 0))
            {
                if (!controls.jump)
                {
                    //print("\n");
                    //if (x > 0)
                    //    foreach (GameObject c in EastObstructions)
                    //        print("obsticle east " + c.name);
                    //if (x < 0)
                    //    foreach (GameObject c in WestObstructions)
                    //        print("obsticle west " + c.name);
                    controls.jump = ButtonPresser();
                    //print("Move obsticle jump " + controls.jump + " " + x);
                }
            }
            //else
            //    print("Move " + x + " E:" + EastObstructions.Count + " W:" + WestObstructions.Count + "\n for " + gameObject.name);
            controls.moveLeft = x;
        }
        return inProgress;
    }

    private void TriggerPickup(bool press)
    {
        PickUpItem = press;
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




    private void OnCollisionStay(Collision col)
    {
        foreach (ContactPoint cp in col.contacts)
        {
            if (isEastObstruction(cp))
            {
                //print("   adding east " + cp.otherCollider.gameObject + " " + cp.normal.x + "\n for " + gameObject.name);
                EastObstructions.Add(col.gameObject);
                WestObstructions.Remove(col.gameObject);
                return;
            }
        }
        foreach (ContactPoint cp in col.contacts)
        {
            if (isWestObstruction(cp))
            {
                //print("   adding west " + cp.otherCollider.gameObject + " " + cp.normal.x + "\n for " + gameObject.name);
                WestObstructions.Add(col.gameObject);
                EastObstructions.Remove(col.gameObject);
                return;
            }
        }
        EastObstructions.Remove(col.gameObject);
        WestObstructions.Remove(col.gameObject);
    }
    void OnCollisionExit(Collision col)
    {
        EastObstructions.Remove(col.gameObject);
        WestObstructions.Remove(col.gameObject);
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
        float otherHeight = cp.otherCollider.bounds.center.y + cp.otherCollider.bounds.extents.y / 2;
        float thisHeight = cp.thisCollider.bounds.center.y + cp.thisCollider.bounds.extents.y / 2;
        //print("heights this: "+thisHeight + " other " + otherHeight + "\n for " + gameObject.name);
        //print("this collider " + cp.thisCollider.gameObject.name + " other: " + cp.otherCollider.gameObject.name);
        if (thisHeight < otherHeight)
            return false;
        return true;
    }

    private struct MapNode
    {
        public int x;
        public int y;
        public byte cost;
        public Vector3 loc;

        public MapNode(int x, int y, int cost, Vector3 loc)
        {
            //print("created Node (" + x + "," + y + ") " + cost + " " + loc);
            this.x = x;
            this.y = y;
            this.cost = (byte)cost;
            this.loc = loc;
        }

    }

    private int GetPlayers()
    {
        int r = 0;
        FindObjectOfType<UndestroyableData>().GetPlayers((x) => { r = x; });
        return r;
    }

}
