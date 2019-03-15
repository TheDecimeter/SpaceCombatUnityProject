using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileInformation : MonoBehaviour
{
    [Header("Where Items Spawn")]
    public ItemSpawner[] ItemSpawners;
    [Header("Where Players Spawn")]
    public PlayerSpawner[] PlayerSpawners;
    [Header("Doors")]
    public DoorBehavior[] NorthDoors;
    public DoorBehavior[] SouthDoors;
    public DoorBehavior[] EastDoors;
    public DoorBehavior[] WestDoors;
    public DoorBehavior[] OtherOpenableDoors;
    [Header("0 = Closed, 1 = Open")]
    public int North=1, South=1, East=1, West=1;

    public class OpenTileEvent : UnityEvent<bool> { }

    [Header("What should happen when a side is opened")]
    public UnityEvent OpenNorth;
    public UnityEvent OpenSouth;
    public UnityEvent OpenEast;
    public UnityEvent OpenWest;

    [Header("What should happen when a side is closed")]
    public UnityEvent CloseNorth;
    public UnityEvent CloseSouth;
    public UnityEvent CloseEast;
    public UnityEvent CloseWest;

    [Header("probably don't mess with this")]
    public bool isClosed=false;


    public void openNorth()
    {
        if (OpenNorth != null) OpenNorth.Invoke();
    }
    public void openSouth()
    {
        if (OpenSouth != null) OpenSouth.Invoke();
    }
    public void openEast()
    {
        if (OpenEast != null) OpenEast.Invoke();
    }
    public void openWest()
    {
        if (OpenWest != null) OpenWest.Invoke();
    }


    public void closeNorth()
    {
        if (OpenNorth != null) CloseNorth.Invoke();
    }
    public void closeSouth()
    {
        if (OpenSouth != null) CloseSouth.Invoke();
    }
    public void closeEast()
    {
        if (OpenEast != null) CloseEast.Invoke();
    }
    public void closeWest()
    {
        if (OpenWest != null) CloseWest.Invoke();
    }
}
