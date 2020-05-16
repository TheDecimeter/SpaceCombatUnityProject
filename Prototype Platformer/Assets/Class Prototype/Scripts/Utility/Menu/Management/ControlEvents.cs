using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ControlEvents : ControlFirer, IPointerClickHandler
{
    //private Vector2 newLoc;
    private GameObject newLoc;
    public bool moving=false;

    private Stack<Vector2> moveLocs = new Stack<Vector2>();
    

    public GameObject HideIfNoFocus;
    public ControlFirer MenuChild;

    private const float closeEnough = .2f, lerpAt = 16f;

    public UnityEvent L,R,S,B,inactiveClick,homeClick,activeClick;

    void Awake()
    {
        //Active = false;
        Home = false;
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    //newLoc = transform.localPosition;
    //    //newLoc = new GameObject("location anchor for "+name);
    //    //newLoc.transform.localPosition = transform.localPosition;
    //    //newLoc.transform.SetParent(transform.parent);
    //}
    void Start()
    {
        newLoc = new GameObject("location anchor for " + name);
        newLoc.transform.SetParent(transform.parent);
        newLoc.transform.localPosition = transform.localPosition;
        //Debug.LogWarning("Creating anchor for " + gameObject.name + " t.p " + transform.localPosition + " n.p " + newLoc.transform.localPosition);
        //Debug.LogWarning("Created anchor for " + gameObject.name + " t.p " + transform.localPosition + " n.p " + newLoc.transform.localPosition);
    }
    void OnEnable()
    {
        if (moving)
            FinishMoving();
    }

    public Vector2 Loc()
    {
        //Debug.LogWarning("Returning anchor for " + gameObject.name + " t.p " + transform.localPosition + " n.p " + newLoc.transform.localPosition);
        return newLoc.transform.localPosition;
    }

    IEnumerator Move()
    {
        moving = true;
        while (moveLocs.Count > 0)
        {
            Vector2 moveLoc = moveLocs.Pop();

            
            while (Vector2.Distance(transform.localPosition, moveLoc) > closeEnough)
            {
                float dist = Dist(moveLoc);
                Vector2 dir = moveLoc- (Vector2)transform.localPosition;
                dir = dir.normalized * Time.deltaTime * dist * lerpAt;

                if (dir.magnitude > Vector2.Distance(transform.localPosition, moveLoc))
                    transform.localPosition = moveLoc;
                else
                    transform.localPosition = (Vector2)transform.localPosition + dir;
                yield return null;
            }
        }
        //transform.localPosition = newLoc;
        transform.localPosition = newLoc.transform.localPosition;
        moving = false;
    }

    private float Dist(Vector2 first)
    {
        float dist = Vector2.Distance(transform.localPosition, first);
        foreach(Vector2 to in moveLocs)
        {
            dist += Vector2.Distance(first, to);
            first = to;
        }
        return dist;
    }
    

    public void MoveTo(Vector2 newLoc)
    {
        //if (!gameObject.activeInHierarchy)
        //    return;
        //this.newLoc = newLoc;
        SetNewloc(newLoc);
        moveLocs.Push(newLoc);
        if (!moving)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(Move());
            else MoveDirectlyTo(newLoc);
        }
    }
    public void MoveDirectlyTo(Vector2 newLoc)
    {
        //if (!gameObject.activeInHierarchy)
        //    return;
        SetNewloc(newLoc);
        moving = false;
        moveLocs.Clear();
        transform.localPosition = newLoc;
    }

    private void FinishMoving()
    {
        StopAllCoroutines();
        MoveDirectlyTo(newLoc.transform.localPosition);
    }

    private void SetNewloc(Vector2 newLoc)
    {
        //if (!gameObject.activeInHierarchy)
        //    return;
        this.newLoc.transform.localPosition = newLoc;
    }

    public override void FireL()
    {
        L.Invoke();
    }
    public override void FireR()
    {
        R.Invoke();
    }
    public override void FireS()
    {
        S.Invoke();
    }
    public override void FireB()
    {
        B.Invoke();
    }

    public override ControlFirer Child()
    {
        return MenuChild;
    }
    public override bool HasChild(ControlFirer c)
    {
        if (this == c)
            return true;
        if (!MenuChild)
            return false;

        return MenuChild.HasChild(c);
    }
    public override void SetHide(bool hide)
    {
        if (HideIfNoFocus)
            HideIfNoFocus.SetActive(hide);
    }

    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (moving)
        {
            return;
        }
        if (Active)
        {
            print("active click");
            activeClick.Invoke();
        }
        else
        {
            if (Home)
            {
                print("home click");
                homeClick.Invoke();
            }
            else
            {
                print("inactive click");
                inactiveClick.Invoke();
            }
        }
    }
    
}
