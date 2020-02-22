using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    private const int Closed = 0, Closing = 1, Opened = 2, Opening = 3;
    [Header("Vertical door=true, horizontal=false")]
    public bool Vertical=false;
    [Header("Which way does the door slide")]
    public bool OpenLeftorDown;

    [Header("Where a player be pulled to")]
    public Transform pullPoint;

    public int FramesToStayOpen=8;
    // Start is called before the first frame update
    public bool isOpenable = true;
    private Color LockedColor = new Color(.3f,0,0,.655f);
    private Color unLockedColor=new Color(.1691f,.4842f,1, .655f);//new Color(0, 0, 0);

    private int _state=Closed;
    private int _frameCounter=0;

    private const string element = "_TintColor";

    private static AudioManager audio;
    void Start()
    {
        //getCurrentTint();
        //unLockedColor = GetComponent<Renderer>().material.GetColor(emission);
        if (audio == null)
            audio = FindObjectOfType<AudioManager>();
        //if(isOpenable)
        //    this.transform.localScale = new Vector3(0, 0, 0);
    }

    //private void getCurrentTint()
    //{
    //    Transform current = transform;
    //    current = transform.parent.transform.parent;
    //    foreach (Transform child in current)
    //    {
    //        Renderer r = child.GetComponent<Renderer>();
    //        if (r)
    //        {
    //            unLockedColor=r.material.GetColor(element);
    //            break;
    //        }
    //    }
    //}

    private void setCurrentTint(Color color)
    {
        Transform current = transform;
        current = transform.parent.transform.parent;
        foreach (Transform child in current)
        {
            Renderer r = child.GetComponent<Renderer>();
            if (r)
            {
                r.material.SetColor(element, color);
            }
        }
    }
    

    public bool isOpened()
    {
        return _state == Opened;
    }

    public Vector3 getPullDirection(Vector3 from)
    {
        if (pullPoint == null)
        {
            if(Vertical)
                return new Vector3(transform.position.x-from.x,0,0);
            return new Vector3(0, transform.position.y - from.y, 0);
        }
        return pullPoint.position;
    }

    public void setOpenable(bool value)
    {
        isOpenable = value;
        if (!isOpenable)
        {
            close();
            //GetComponent<Renderer>().material.SetColor(element,LockedColor);
            setCurrentTint(LockedColor);

        }
        else
            setCurrentTint(unLockedColor); // GetComponent<Renderer>().material.SetColor(element, unLockedColor);
    }

    public void open()
    {
        if (_state == Opened)
            return;

        if (isOpenable)
        {
            //door opening sounds can go here
            //print("DoorOpen");
            audio.Play("DoorOpen");

            this.transform.localScale = new Vector3(0, 0, 0);
            _state = Opened;
            _frameCounter = FramesToStayOpen;
        }
    }

    public void close()
    {
        if (_state == Closed)
            return;

        //door closing sounds can go here
        //audio.Play("DoorClose");

        this.transform.localScale = new Vector3(1, 1, 1);
        _state = Closed;
    }
}
