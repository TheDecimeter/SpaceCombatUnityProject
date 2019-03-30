using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to open a door, and prompt players  on how to open them
/// </summary>
public class DoorController : MonoBehaviour
{
    public enum DoorType
    {
        Other,
        North,
        South,
        East,
        West
    }
    [Header("Where is door in tile")]
    public DoorType DoorLocation=DoorType.Other;
    [Header("Where are the que templates")]
    public GameObject GeneralOpenQue;
    public GameObject NorthOpenQue;
    public GameObject SouthOpenQue;
    public GameObject EastOpenQue;
    public GameObject WestOpenQue;
    [Header("what doors will this control")]
    public DoorBehavior[] Doors;
    [Header("who can open this door")]
    public string playerTag="Player";

    [Header("Can these offsets be overridden externally")]
    public bool allowOffsetsToBeOverridden = true;
    [Header("Fine tune the que positions")]
    public float xOffset=-1;
    public float yOffset=.05f;
    public float zOffset=-2;

    private GameObject general;
    private GameObject north;
    private GameObject east;
    private GameObject south;
    private GameObject west;

    private Dictionary<int, QueNode> displayedQues = new Dictionary<int, QueNode>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //remove que node if player is no longer colliding
    //

    public bool open()
    {
        bool ret = false;
        foreach (DoorBehavior d in Doors)
        {
            if (!d.isOpened()&&d.isOpenable) ret = true;
            d.open();
        }
        return ret;
    }

    public void OnTriggerStay(Collider other)
    {
        bool validdoors = false;
        foreach (DoorBehavior d in Doors)
            if (d.isOpenable)
            {
                validdoors = true;
                break;
            }
        if (!validdoors) return;

        if (other.CompareTag(playerTag))
        {
            print("here");
            DoorType orientation = DoorLocation;
            CharacterMovement_Physics player = other.transform.parent.transform.parent.gameObject.GetComponent<CharacterMovement_Physics>();
            int count = player.AddDoor(this, out orientation);
            
            if (count == 1) add(DoorType.Other, player.PlayerNumber);
            else add(orientation, player.PlayerNumber);
            //if count is 1, orientation doesn't matter
            //player collides
            //if dictionary does not already contain that orientation
            //add a player and orientation to the dictionary.
        }
    }

    private void add(DoorType orientation, int playernum)
    {
        //don't add a node of the same type twice
        foreach (QueNode n in displayedQues.Values)
        {
            if (n.orientation == orientation)
            {
                displayedQues[playernum] = n;
                return;
            }
        }

        GameObject que;
        switch (orientation)
        {
            default:
                que=Instantiate(GeneralOpenQue);
                break;
            case DoorType.North:
                que = Instantiate(NorthOpenQue);
                break;
            case DoorType.South:
                que = Instantiate(SouthOpenQue);
                break;
            case DoorType.East:
                que = Instantiate(EastOpenQue);
                break;
            case DoorType.West:
                que = Instantiate(WestOpenQue);
                break;
        }
        AddQue(que, orientation, playernum);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            int playernum = other.transform.parent.transform.parent.gameObject.GetComponent<CharacterMovement_Physics>().PlayerNumber;
            if (displayedQues.ContainsKey(playernum))
            {
                //GameObject tmp = displayedQues[playernum].que;

                other.transform.parent.transform.parent.gameObject.GetComponent<CharacterMovement_Physics>().RemoveDoor(this);
                RemoveQue(playernum);
            }
        }
    }

    private void AddQue(GameObject que, DoorType orientation, int playerNum)
    {
        if (displayedQues.ContainsKey(playerNum))
        {
            if (displayedQues[playerNum].orientation == orientation)
                return;
            Destroy(displayedQues[playerNum].que);
            displayedQues[playerNum] = new QueNode(que, orientation);
        }
        else displayedQues.Add(playerNum, new QueNode(que, orientation));
        que.transform.position = new Vector3(
            transform.position.x+xOffset,
            transform.position.y+yOffset,
            transform.position.z+zOffset);

    }

    private void RemoveQue(int playernum)
    {
        //if multiple players are in the hit box
        //just remove the player leaving and leave the
        //que alive.

        for(int i=0; i<4; ++i)
        {
            if(displayedQues.ContainsKey(i))
                if (i!=playernum&&displayedQues[i].orientation == displayedQues[playernum].orientation)
                {
                    displayedQues.Remove(playernum);
                    print("other player present in door");
                    return;
                }
        }
        
        //otherwise, destroy the que remove the playernum
        //and close the door.
        Destroy(displayedQues[playernum].que);
        displayedQues.Remove(playernum);

        if (displayedQues.Count == 0)
            foreach (DoorBehavior d in Doors)
                d.close();
    }

    public void queGeneral(int playerNumber, DoorType orientation)
    {
        //if (general!=null)
        //    return;
        if (displayedQues.ContainsKey(playerNumber))
        {
            Destroy(displayedQues[playerNumber].que);
            displayedQues.Remove(playerNumber);
        }

        GameObject que = Instantiate(GeneralOpenQue);
        AddQue(que, orientation, playerNumber);
    }
    public void queNorth() { }
    public void queEast() { }
    public void queSouth() { }
    public void queWest() { }

    private class QueNode
    {
        public const int General= 0, North = 1, South = 2, East = 3, West = 4;
        public GameObject que;
        public DoorType orientation;
        public QueNode(GameObject que, DoorType orientation)
        {
            this.orientation = orientation;
            this.que = que;
        }
    }
}
