using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelRandomizer : MonoBehaviour
{
    private const int North = 0, East = 1, South = 2, West = 3;
    [Header("Where are level tiles stored:")]
    public GameObject LevelTileArray;
    [Header("Map Size")]
    public int MapDemensions = 3;
    [Header("Random Seed, -1 is random")]
    public int Seed = -1;
    [Header("Snake or Fill")]
    public bool FillMap = true;
    [Header("If Snake, how many tiles?")]
    public int TileQuantity = 10;
    public float xTileSize = 12.741084f, yTileSize = 10.75f;

    //private List<TileInformation> _tileList = new List<TileInformation>();
    private TileInformation[][] Map;
    private List<Object> PlacedTileList;
    private GameObject[] TilePallette;
    private GameObject[] PlayerSpawners;
    private GameObject[] ItemSpawners;

    public int startX = 0, startY = 0, startZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        //foreach (Transform child in LevelTileArray.transform)
        //{
        //    _tileList.Add(child.gameObject.GetComponent<TileInformation>());
        //}
        Map = new TileInformation[MapDemensions][];
        for (int i = 0; i < MapDemensions; ++i)
        {
            Map[i] = new TileInformation[MapDemensions];
            //for(int j=0; j<MapDemensions; ++j)
            //    Map[i][j] = new TileInformation();
        }
        PlacedTileList = new List<Object>();

        int count = 0;
        foreach (Transform child in LevelTileArray.transform)
            count++;

        print("count=" + count);
        TilePallette = new GameObject[count];
        count = 0;
        foreach (Transform child in LevelTileArray.transform)
        {
            child.gameObject.SetActive(false);
            TilePallette[count] = child.gameObject;
            TilePallette[count].GetComponent<TileInformation>().closeEast();
            TilePallette[count].GetComponent<TileInformation>().closeNorth();
            TilePallette[count].GetComponent<TileInformation>().closeSouth();
            TilePallette[count].GetComponent<TileInformation>().closeWest();
            count++;
        }

        if (Seed != -1)
            Random.seed = Seed;

        randomizeLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void randomizeLevel()
    {
        Shuffle(TilePallette);

        int x = Random.Range(0, MapDemensions),
            y = Random.Range(0, MapDemensions);

        recursiveTilePlacement(x, y);


        //for(int i=0; i<TileQuantity||FillMap;)
        //{
        //    int tileIndex= Random.Range(0,TilePallette.Length), tileStop=tileIndex;

        //    while (tileIndex<TilePallette.Length&&
        //        !isTileCompatible(x, y, TilePallette[tileIndex++].GetComponent<TileInformation>())) ;
        //    if (tileIndex == TilePallette.Length)
        //    {
        //        tileIndex = 0;
        //        while (tileIndex < tileStop &&
        //            !isTileCompatible(x, y, TilePallette[tileIndex++].GetComponent<TileInformation>())) ;
        //    }

        //    if (tileIndex == tileStop)
        //    {
        //        print("no compatible tile found");
        //        break;
        //    }

        //    addTile(x, y, TilePallette[tileIndex]);

        //}
    }

    private void recursiveTilePlacement(int x, int y)
    {
        //make sure you're within the grid bounds
        if (!isPositionValid(x, y))
            return;
        //find a tile that matches surrounding tiles
        int tileIndex = findValidTileIndex(x, y);
        if (tileIndex == -1)
        {
            print("error: no valid tile found");
            return;
        }
        //add the tile
        addTile(x, y, getTilePallette(tileIndex));

        //move to neighbors
        if (Random.Range(0, 1) == 1 || FillMap) recursiveTilePlacement(x - 1, y);
        if (Random.Range(0, 1) == 1 || FillMap) recursiveTilePlacement(x + 1, y);
        if (Random.Range(0, 1) == 1 || FillMap) recursiveTilePlacement(x, y - 1);
        if (Random.Range(0, 1) == 1 || FillMap) recursiveTilePlacement(x, y + 1);
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

            print("no compatible tile found");
            return -1;
        return tileIndex;
    }
    private void deactivateLevel()
    {
        while (PlacedTileList.Count > 0)
        {
            Destroy(PlacedTileList[0]);
            PlacedTileList.RemoveAt(0);
        }
        for (int i = 0; i < MapDemensions; ++i)
            for (int j = 0; j < MapDemensions; ++j)
                Map[i][j] = null;

    }

    private void addTile(int x, int y, GameObject tile)
    {
        GameObject tmp = Instantiate(tile);
        tmp.transform.position = new Vector3(startX + x * xTileSize, startY + y * yTileSize, startZ);
        print(tmp.transform.position);
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
        if (x < MapDemensions - 1)
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
        if (y < MapDemensions - 1)
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
                if (y == MapDemensions - 1) return false;
                addTile(x, y, Tile);
                break;
            case West:
                if (x == 0) return false;
                addTile(x, y, Tile);
                break;
            case East:
                if (x == MapDemensions - 1) return false;
                addTile(x, y, Tile);
                break;
        }
        return true;
    }

    private bool isPositionValid(int x, int y)
    {
        //make sure coordinates are within boundaries
        if (y == -1) return false;
        if (y == MapDemensions) return false;
        if (x == -1) return false;
        if (x == MapDemensions) return false;

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
        if (x < MapDemensions - 1)
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
        if (y < MapDemensions - 1)
            if (Map[y + 1][x] != null)
            {
                if (Map[y + 1][x].North != tile.South)
                    return false;
            }
        return true;
    }

    public static void Shuffle(GameObject[] list)
    {
        for (int i = 0; i < list.Length - 1; i++)
            Swap(list, i, Random.Range(i, list.Length));
    }

    public static void Swap(GameObject[] list, int i, int j)
    {
        GameObject temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    private GameObject getTilePallette(int index)
    {
        print(index);
        return TilePallette[index % TilePallette.Length];
    }
}
