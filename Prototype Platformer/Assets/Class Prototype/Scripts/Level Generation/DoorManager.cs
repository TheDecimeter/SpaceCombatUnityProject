using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager
{
    private const int North = 0, East = 1, South = 2, West = 3, Any = 4;

    List<DoorBehavior> GeneralDoors;
    List<DoorBehavior> NorthDoors;
    List<DoorBehavior> SouthDoors;
    List<DoorBehavior> EastDoors;
    List<DoorBehavior> WestDoors;
    int MapDemensionsY, MapDemensionsX;
    TileInformation[][] Map;

    public DoorManager(TileInformation[][] map)
    {
        Map = map;
        MapDemensionsX = map[0].Length;
        MapDemensionsY = map.Length;
    }

    public void SetDoors(List<Object> placedTiles)
    {
        GeneralDoors = new List<DoorBehavior>();
        NorthDoors = new List<DoorBehavior>();
        SouthDoors = new List<DoorBehavior>();
        EastDoors = new List<DoorBehavior>();
        WestDoors = new List<DoorBehavior>();

        foreach (GameObject g in placedTiles)
        {
            TileInformation tmp = g.GetComponent<TileInformation>();

            foreach (DoorBehavior d in tmp.NorthDoors)
                addDoor(d, NorthDoors);
            foreach (DoorBehavior d in tmp.SouthDoors)
                addDoor(d, SouthDoors);
            foreach (DoorBehavior d in tmp.EastDoors)
                addDoor(d, EastDoors);
            foreach (DoorBehavior d in tmp.WestDoors)
                addDoor(d, WestDoors);
            foreach (DoorBehavior d in tmp.OtherOpenableDoors)
                addDoor(d, GeneralDoors);
        }
    }

    private void addDoor(DoorBehavior door, List<DoorBehavior> list)
    {
        if (door.isOpenable && door.gameObject.activeInHierarchy) list.Add(door);
    }

    public void CloseDownRoom(int x, int y)
    {
        if (x < 0) return;
        if (y < 0) return;
        TileInformation tile = Map[y][x];
        //lock all doors in the tile
        foreach (DoorBehavior d in tile.NorthDoors)
            lockDoor(d, NorthDoors);
        foreach (DoorBehavior d in tile.SouthDoors)
            lockDoor(d, SouthDoors);
        foreach (DoorBehavior d in tile.EastDoors)
            lockDoor(d, EastDoors);
        foreach (DoorBehavior d in tile.WestDoors)
            lockDoor(d, WestDoors);
        foreach (DoorBehavior d in tile.OtherOpenableDoors)
            lockDoor(d, GeneralDoors);

        //lock doors neighboring the tile
        if (x - 1 >= 0 && Map[y][x - 1] != null)
            foreach (DoorBehavior d in Map[y][x - 1].EastDoors)
                lockDoor(d, EastDoors);
        if (x + 1 < MapDemensionsX && Map[y][x + 1] != null)
            foreach (DoorBehavior d in Map[y][x + 1].WestDoors)
                lockDoor(d, WestDoors);
        if (y - 1 >= 0 && Map[y - 1][x] != null)
            foreach (DoorBehavior d in Map[y - 1][x].NorthDoors)
                lockDoor(d, NorthDoors);
        if (y + 1 < MapDemensionsY && Map[y + 1][x] != null)
            foreach (DoorBehavior d in Map[y + 1][x].SouthDoors)
                lockDoor(d, SouthDoors);
    }
    public void lockDoor(DoorBehavior door, List<DoorBehavior> list)
    {
        door.setOpenable(false);
        list.Remove(door);
    }

    public bool openNearestDoors(Vector3 to, float within, ref Vector3 pullToward, int preference)
    {
        List<List<DoorBehavior>> checkDoors = new List<List<DoorBehavior>>();

        switch (preference)
        {
            case North:
                checkDoors.Add(NorthDoors);
                checkDoors.Add(SouthDoors);
                checkDoors.Add(EastDoors);
                checkDoors.Add(WestDoors);
                break;
            case South:
                checkDoors.Add(SouthDoors);
                checkDoors.Add(SouthDoors);
                checkDoors.Add(EastDoors);
                checkDoors.Add(WestDoors);
                break;
            case East:
                checkDoors.Add(EastDoors);
                checkDoors.Add(WestDoors);
                checkDoors.Add(SouthDoors);
                checkDoors.Add(SouthDoors);
                break;
            case West:
                checkDoors.Add(WestDoors);
                checkDoors.Add(EastDoors);
                checkDoors.Add(SouthDoors);
                checkDoors.Add(SouthDoors);
                break;
            default:
                checkDoors.Add(WestDoors);
                checkDoors.Add(EastDoors);
                checkDoors.Add(SouthDoors);
                checkDoors.Add(SouthDoors);
                return openNearestDoorsNoPreference(to,within,ref pullToward,checkDoors);
        }
        return openNearestDoorsWithPreference(to, within, ref pullToward, checkDoors);
    }

    private bool openNearestDoorsWithPreference(Vector3 to, float within, ref Vector3 pullToward,
        List<List<DoorBehavior>> list)
    {
        Vector3 closestPoint;
        float shortestDistance = -1;

        bool foundDoor = false;
        foreach (List<DoorBehavior> l in list)
        {
            foreach (DoorBehavior d in l)
            {
                closestPoint = d.gameObject.transform.position;
                shortestDistance = distance(closestPoint, to);
                pullToward = d.getPullDirection(to);
                goto EndLoop;
            }
        }
        EndLoop:

        if (shortestDistance == -1) return false;

        foreach (List<DoorBehavior> l in list)
        {
            foreach (DoorBehavior d in l)
            {
                if (distance(d.transform.position, to) <= within)
                {
                    d.open();
                    foundDoor = true;
                    if (distance(d.transform.position, to) < shortestDistance)
                    {
                        closestPoint = d.gameObject.transform.position;
                        pullToward = d.getPullDirection(to);
                        shortestDistance = distance(closestPoint, to);
                    }
                }
            }
            if (foundDoor) return true;
        }
        return false;
    }
    private bool openNearestDoorsNoPreference(Vector3 to, float within, ref Vector3 pullToward,
        List<List<DoorBehavior>> list)
    {
        Vector3 closestPoint;
        float shortestDistance = -1;

        foreach (List<DoorBehavior> l in list)
            foreach (DoorBehavior d in l)
            {
                closestPoint = d.gameObject.transform.position;
                shortestDistance = distance(closestPoint, to);
                pullToward = d.getPullDirection(to);
            }

        if (shortestDistance == -1) return false;

        bool foundDoor = false;
        foreach (List<DoorBehavior> l in list)
        {
            foreach (DoorBehavior d in l)
            {
                if (distance(d.transform.position, to) <= within)
                {
                    d.open();
                    foundDoor = true;
                    if (distance(d.transform.position, to) < shortestDistance)
                    {
                        closestPoint = d.gameObject.transform.position;
                        pullToward = d.getPullDirection(to);
                        shortestDistance = distance(closestPoint, to);
                    }
                }
            }
        }
        return foundDoor;
    }


    public static float distance(Vector3 from, Vector3 to)
    {
        return Mathf.Sqrt(Mathf.Pow(from.x - to.x, 2) + Mathf.Pow(from.y - to.y, 2));
    }
}
