using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ControlEvents : ControlFirer, IPointerClickHandler
{
    public GameObject HideIfNoFocus;
    public ControlEvents MenuChild;
    private Vector2 newLoc;
    public bool Active { get; set; }
    private bool _home,moving=false;

    private Stack<Vector2> moveLocs = new Stack<Vector2>();

    public bool Home
    {
        get
        {
            return _home;
        }
        set
        {
            if (_home == value)
                return;
            _home = value;

            //if (!value)
            //{
            //    if (MenuChild)
            //        MenuChild.Home = value;
            //}

            if (!value)
            {
                if (HideIfNoFocus)
                    HideIfNoFocus.SetActive(false);
                if (MenuChild)
                {
                    MenuChild.Home = value;
                    MenuChild.FireB();
                }
            }
            else
            {
                if (MenuChild)
                    MenuChild.Home = value;
            }
        }
    }

    private const float closeEnough = .2f, lerpAt = 16f;

    public UnityEvent L,R,S,B,inactiveClick,homeClick,activeClick;

    void Awake()
    {
        Active = false;
        Home = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        newLoc = transform.position;
    }

    public Vector2 Loc()
    {
        if(moving)
            return newLoc;
        else
            return transform.position;
    }

    IEnumerator Move()
    {
        moving = true;
        while (moveLocs.Count > 0)
        {
            Vector2 moveLoc = moveLocs.Pop();

            
            while (Vector2.Distance(transform.position, moveLoc) > closeEnough)
            {
                float dist = Dist(moveLoc);
                Vector2 dir = moveLoc- (Vector2)transform.position;
                dir = dir.normalized * Time.deltaTime * dist * lerpAt;

                if (dir.magnitude > Vector2.Distance(transform.position, moveLoc))
                    transform.position = moveLoc;
                else
                    transform.position = (Vector2)transform.position + dir;
                yield return null;
            }
        }
        transform.position = newLoc;
        moving = false;
    }

    private float Dist(Vector2 first)
    {
        float dist = Vector2.Distance(transform.position, first);
        foreach(Vector2 to in moveLocs)
        {
            dist += Vector2.Distance(first, to);
            first = to;
        }
        return dist;
    }
    

    public void MoveTo(Vector2 newLoc)
    {
        this.newLoc = newLoc;
        moveLocs.Push(newLoc);
        if(!moving)
            StartCoroutine(Move());
    }

    public void FireL()
    {
        L.Invoke();
    }
    public void FireR()
    {
        R.Invoke();
    }
    public void FireS()
    {
        S.Invoke();
    }
    public void FireB()
    {
        B.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Active)
            activeClick.Invoke();
        else
        {
            if (Home)
                homeClick.Invoke();
            else
                inactiveClick.Invoke();
        }
    }
}
