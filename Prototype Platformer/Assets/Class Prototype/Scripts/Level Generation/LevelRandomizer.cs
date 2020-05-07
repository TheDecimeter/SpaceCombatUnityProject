using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelRandomizer : MonoBehaviour
{
    private const int North = 0, East = 1, South = 2, West = 3, Any=4;

    [Header("QuickSettings:")]
    public int FramesToKeepDoorsOpen=10;
    public int PlayerDoorDashSpeed=500;

    [Header("Where are level tiles stored:")]
    public GameObject LevelTileArray;

    [Header("Where are players stored:")]
    public GameObject PlayerArray;

    [Header("Where are item templates stored:")]
    public GameObject ItemArray;
    public string itemType = "All";
    public int itemQuantity = 4;
    
    public int MapDemensionsX { get; protected set; }
    public int MapDemensionsY { get; protected set; }

    [Header("Random Seed, -1 is random")]
    public int Seed = -1;
    
    private bool FillMap = true;
    
    private int TileQuantity = 16;

    [Header("What are the demensions of individual tiles")]
    public float xTileSize = 12.741084f;
    public float yTileSize = 8f;

    [Header("Center of tile from parent")]
    public float xOffset = 6;
    public float yOffset = 0;

    [Header("Room Closing Stats")]
    public int CloseRoomXManyFrames = 180;
    public int WarnForXManyFrames = 180;
    public int LeaveRoomsOpen = 5;

    [Header("Lower Left corner of map")]
    public int startX = 0;
    public int startY = 0;
    public int startZ = 0;

    //private List<TileInformation> _tileList = new List<TileInformation>();

    //place to store the types of items and tiles which a level uses (constant every round)
    private GameObject[] TilePallette;
    private GameObject[] ItemPallette;

    //place to store the spawners in a level (changes every round)
    private PlayerSpawner[] PlayerSpawners;
    private ItemSpawner[] ItemSpawners;

    //the various things created in a specific level (changes every round)
    public GameObject[] PlacedItems { get; protected set; }
    public TileInformation[][] Map { get; protected set; }
    private List<Object> PlacedTileList;
    //private List<DoorBehavior> LevelDoors;

    private DoorManager doorManager;

    private CloseLevel closer;
    private AsteroidGenerator daRocks;

    private int TileCounter;


    // Start is called before the first frame update
    void Start()
    {
        //foreach (Transform child in LevelTileArray.transform)
        //{
        //    _tileList.Add(child.gameObject.GetComponent<TileInformation>());
        //}


        daRocks = GetComponent<AsteroidGenerator>();
        int tmpX=3, tmpY=4;

        FindObjectOfType<UndestroyableData>().SetUpLevel(ref tmpX, ref tmpY, ref FillMap,
            ref TileQuantity, ref WarnForXManyFrames, ref CloseRoomXManyFrames,
            ref daRocks.AsteroidSpeed, ref daRocks.chuckEvery, ref daRocks.plusOrMinus);

        MapDemensionsX = tmpX;
        MapDemensionsY = tmpY;

        foreach (Transform child in PlayerArray.transform)
        {
            if (child.gameObject.GetComponent<CharacterMovement_Physics>().Environment == null)
                child.gameObject.GetComponent<CharacterMovement_Physics>().Environment = this;
            if (PlayerDoorDashSpeed != -1)
                child.gameObject.GetComponent<CharacterMovement_Physics>().doorPullSpeed = PlayerDoorDashSpeed;
        }


        Map = new TileInformation[MapDemensionsY][];
        for (int i = 0; i < MapDemensionsY; ++i)
        {
            Map[i] = new TileInformation[MapDemensionsX];
            //for(int j=0; j<MapDemensions; ++j)
            //    Map[i][j] = new TileInformation();
        }
        PlacedTileList = new List<Object>();

        int tileCount = 0;//, playerSpawnerCount = 0 ;
        foreach (Transform child in LevelTileArray.transform)
        {
            tileCount++;
            //playerSpawnerCount+=child.GetComponent<TileInformation>().PlayerSpawners.Length;
        }

        ////Debug.Log("count=" + playerSpawnerCount);
        TilePallette = new GameObject[tileCount];
        //PlayerSpawners = new PlayerSpawner[playerSpawnerCount];
        tileCount = 0;
        //playerSpawnerCount = 0;
        foreach (Transform child in LevelTileArray.transform)
        {
            //foreach(PlayerSpawner g in child.GetComponent<TileInformation>().PlayerSpawners)
            //    PlayerSpawners[playerSpawnerCount++] = g;



            child.gameObject.SetActive(false);
            TilePallette[tileCount] = child.gameObject;
            TileInformation tmp= TilePallette[tileCount].GetComponent<TileInformation>();


            tmp.closeEast();
            tmp.closeNorth();
            tmp.closeSouth();
            tmp.closeWest();
            tileCount++;
        }


        setUpItemPallette();


        if (Seed != -1)
            Random.InitState(Seed);
        //Random.seed = Seed;

        doorManager = new DoorManager(Map, FramesToKeepDoorsOpen);
        randomizeLevel();
        
        //daRocks.Init(startX, startY, startZ, xTileSize, yTileSize, WarnForXManyFrames);

    }

    private void FixedUpdate()
    {
        int x, y;
        if (daRocks.warn(closer.warnCheck(out x, out y), x, y))
        {
            float lx = startX + x * xTileSize + xOffset - xTileSize / 2;
            float hx = startX + x * xTileSize + xOffset + xTileSize / 2;
            float ly = startY + y * yTileSize + yOffset - yTileSize / 2;
            float hy = startY + y * yTileSize + yOffset + yTileSize / 2;
            ////Debug.Log("CLOSE ROOM " + x + " " + y + " " + hx + " " + lx + " " + hy + " " + ly);

            //if the player is within the bounds of the room, DIE
            foreach (Transform child in PlayerArray.transform)
            {
                if (child.position.x > lx && child.position.x < hx &&
                    child.position.y > ly && child.position.y < hy)
                    child.GetComponent<PlayerHealth>().PlayerDeath();
            }

            //close all doors in the room
            doorManager.CloseDownRoom(x,y);
        }
    }

    public void LaunchPod()
    {
        //stop closing off rooms
        closer.stop = true;

        int x=0, y=0, count=0;
        for(int i=0; i<MapDemensionsY; ++i)
            for(int j=0; j<MapDemensionsX; ++j)
            {
                if (Map[i][j]!=null&&!Map[i][j].isClosed)
                    count++;
            }
        int stop = Random.Range(0, count);
        //Debug.Log("stop " + stop+ " count "+count);
        count = 0;
        for (int i = 0; i < MapDemensionsY; ++i)
            for (int j = 0; j < MapDemensionsX; ++j)
            {
                if (Map[i][j] != null && !Map[i][j].isClosed)
                    if (count++ == stop)
                    {
                        x = j;
                        y = i;
                        goto LeaveLoop;
                    }
            }
        LeaveLoop:

        ////Debug.Log("x" + x +" y "+y);

        GetComponent<EscapePodLauncher>().Launch(startX + x * xTileSize + xOffset, startY + y * yTileSize + yOffset, startZ);

    }

    

    //private void lockDoor(DoorBehavior door)
    //{
    //    door.setOpenable(false);
    //    LevelDoors.Remove(door);
    //}

    private void setUpItemPallette()
    {
        int count = 0;
        foreach (Transform child in ItemArray.transform)
            count++;
        ItemPallette = new GameObject[count];
        count = 0;
        foreach (Transform child in ItemArray.transform)
            ItemPallette[count++] = child.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void randomizeLevel()
    {

        TileCounter = 0;
        if (TileQuantity > MapDemensionsX * MapDemensionsY)
            TileQuantity = MapDemensionsX * MapDemensionsY;
        Shuffle(TilePallette);

        int x = Random.Range(0, MapDemensionsX),
            y = Random.Range(0, MapDemensionsY);

        
        recursiveTilePlacement(x, y);

        while(!FillMap && TileCounter < TileQuantity)
        {
            x = Random.Range(0, MapDemensionsX);
            y = Random.Range(0, MapDemensionsY);
            if (Map[y][x] != null)
            {
                recursiveTilePlacement(x-1, y);
                recursiveTilePlacement(x+1, y);
                recursiveTilePlacement(x, y-1);
                recursiveTilePlacement(x, y+1);
            }
        }


        //add level doors to the data structure so it's easier to check and see if players are
        //near them in game
        doorManager.SetDoors(PlacedTileList);
            


        //go through the placed tiles and see where all of the
        //player spawners are.
        int Spawners = 0;
        foreach (GameObject g in PlacedTileList)
            Spawners += g.GetComponent<TileInformation>().PlayerSpawners.Length;
        PlayerSpawners = new PlayerSpawner[Spawners];
        //create a list of available player spawners
        Spawners = 0;
        foreach (GameObject g in PlacedTileList)
            foreach (PlayerSpawner p in g.GetComponent<TileInformation>().PlayerSpawners)
                PlayerSpawners[Spawners++] = p;
        //Shuffle them and place players in the first 4
        //loop back around if there are less than 4
        Shuffle(PlayerSpawners);
        Spawners = 0;
        foreach (Transform child in PlayerArray.transform)
            child.position = PlayerSpawners[(Spawners++ % PlayerSpawners.Length)].gameObject.transform.position;



        //go through the placed tiles and see where all of the
        //item spawners are.
        Spawners = 0;
        foreach (GameObject g in PlacedTileList)
            Spawners += g.GetComponent<TileInformation>().ItemSpawners.Length;
        ItemSpawners = new ItemSpawner[Spawners];
        //create a list of available item spawners
        Spawners = 0;
        foreach (GameObject g in PlacedTileList)
            foreach (ItemSpawner i in g.GetComponent<TileInformation>().ItemSpawners)
                ItemSpawners[Spawners++] = i;
        //Shuffle them and place items in the first N
        Shuffle(ItemSpawners);
        Spawners = 0;
        PlacedItems = new GameObject[itemQuantity];
        for (int i = 0; i < itemQuantity; ++i)
        {
            GameObject tmp = Instantiate(ItemPallette[Random.Range(0, ItemPallette.Length)]);
            tmp.transform.position = ItemSpawners[(Spawners % ItemSpawners.Length)].gameObject.transform.position;
            PlacedItems[Spawners] = tmp;
            Spawners++;
            ////Debug.Log("placed item at " + tmp.transform.position);
        }

        //setup the level shrinking object
        
        closer=new CloseLevel(Map, Any, CloseRoomXManyFrames, WarnForXManyFrames,LeaveRoomsOpen);
    }

    private void recursiveTilePlacement(int x, int y)
    {
        if (!FillMap && TileCounter == TileQuantity)
            return;
        //make sure you're within the grid bounds
        if (!isPositionValid(x, y))
            return;
        //find a tile that matches surrounding tiles
        int tileIndex = findValidTileIndex(x, y);
        if (tileIndex == -1)
        {
            //Debug.Log("error: no valid tile found");
            return;
        }
        //add the tile
        addTile(x, y, getTilePallette(tileIndex));
        TileCounter++;
        //move to neighbors
        if (Random.Range(0, 2) == 1 || FillMap) recursiveTilePlacement(x - 1, y);
        if (Random.Range(0, 2) == 1 || FillMap) recursiveTilePlacement(x + 1, y);
        if (Random.Range(0, 2) == 1 || FillMap) recursiveTilePlacement(x, y - 1);
        if (Random.Range(0, 2) == 1 || FillMap) recursiveTilePlacement(x, y + 1);
    }

    private int findValidTileIndex(int x, int y)
    {

        int tileIndex = Random.Range(0, TilePallette.Length), tileStop = tileIndex;
        while (tileIndex < TilePallette.Length)
        {
            if (isTileCompatible(x, y,
            getTilePallette(tileIndex)
            .GetComponent<TileInformation>())) return tileIndex;
            tileIndex++;
        }
        tileIndex = 0;
        while (tileIndex < tileStop)
        {
            if (isTileCompatible(x, y,
            getTilePallette(tileIndex)
            .GetComponent<TileInformation>())) return tileIndex;
            tileIndex++;
        }

        //Debug.Log("no compatible tile found");
        return -1;
    }
    private void deactivateLevel()
    {
        while (PlacedTileList.Count > 0)
        {
            Destroy(PlacedTileList[0]);
            PlacedTileList.RemoveAt(0);
        }
        for (int i = 0; i < MapDemensionsY; ++i)
            for (int j = 0; j < MapDemensionsX; ++j)
                Map[i][j] = null;

    }

    private void addTile(int x, int y, GameObject tile)
    {
        GameObject tmp = Instantiate(tile);
        tmp.transform.position = new Vector3(startX + x * xTileSize, startY + y * yTileSize, startZ);
        tmp.GetComponent<TileInformation>().Init();
        ////Debug.Log(tmp.transform.position);
        PlacedTileList.Add(tmp);
        Map[y][x] = tmp.GetComponent<TileInformation>();
        tmp.SetActive(true);
        effectNeighbors(x, y);
    }

    private void effectNeighbors(int x, int y)
    {
        if (x > 0)
            if (Map[y][x - 1] != null)
            {
                Map[y][x - 1].openEast();
                Map[y][x].openWest();
            }
        if (x < MapDemensionsX - 1)
            if (Map[y][x + 1] != null)
            {
                Map[y][x + 1].openWest();
                Map[y][x].openEast();
            }
        if (y > 0)
            if (Map[y - 1][x] != null)
            {
                Map[y - 1][x].openNorth();
                Map[y][x].openSouth();
            }
        if (y < MapDemensionsY - 1)
            if (Map[y + 1][x] != null)
            {
                Map[y + 1][x].openSouth();
                Map[y][x].openNorth();
            }
    }


    private bool placeIfPossible(int x, int y, int dir, GameObject Tile)
    {
        switch (dir)
        {
            case North:
                if (y == 0) return false;
                addTile(x, y, Tile);
                break;
            case South:
                if (y == MapDemensionsY - 1) return false;
                addTile(x, y, Tile);
                break;
            case West:
                if (x == 0) return false;
                addTile(x, y, Tile);
                break;
            case East:
                if (x == MapDemensionsX - 1) return false;
                addTile(x, y, Tile);
                break;
        }
        return true;
    }

    private bool isPositionValid(int x, int y)
    {
        //make sure coordinates are within boundaries
        if (y == -1) return false;
        if (y == MapDemensionsY) return false;
        if (x == -1) return false;
        if (x == MapDemensionsX) return false;

        //make sure the map position isn't already occupied
        if (Map[y][x] != null)
            return false;

        return true;
    }

    private bool isTileCompatible(int x, int y, TileInformation tile)
    {
        if (x > 0)
            if (Map[y][x - 1] != null)
            {
                if (Map[y][x - 1].East != tile.West)
                    return false;
            }
        if (x < MapDemensionsX - 1)
            if (Map[y][x + 1] != null)
            {
                if (Map[y][x + 1].West != tile.East)
                    return false;
            }
        if (y > 0)
            if (Map[y - 1][x] != null)
            {
                if (Map[y - 1][x].South != tile.North)
                    return false;
            }
        if (y < MapDemensionsY - 1)
            if (Map[y + 1][x] != null)
            {
                if (Map[y + 1][x].North != tile.South)
                    return false;
            }
        return true;
    }


    public static void Shuffle<T>(T[] list)
    {
        for (int i = 0; i < list.Length - 1; i++)
            Swap(list, i, Random.Range(i, list.Length));
    }

    public static void Swap<T>(T[] list, int i, int j)
    {
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    private GameObject getTilePallette(int index)
    {
        ////Debug.Log(index);
        return TilePallette[index % TilePallette.Length];
    }


    public bool openNearestDoors(Vector3 to, float within, ref Vector3 pullToward, int preference)
    {
        return doorManager.openNearestDoors(to, within, ref pullToward, preference);
    }





}
