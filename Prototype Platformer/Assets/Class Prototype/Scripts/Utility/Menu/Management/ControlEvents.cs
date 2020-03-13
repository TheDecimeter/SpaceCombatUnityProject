using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ControlEvents : MonoBehaviour, IPointerClickHandler
{
    public GameObject HideIfNoFocus;
    public ControlEvents MenuChild;
    private Vector2 newLoc;
    public bool Active { get; set; }
    private bool _home;
    public bool Home
    {
        get
        {
            return _home;
        }
        set
        {
            _home = value;
            if(MenuChild)
                MenuChild.Home = value;
        }
    }

    private const float closeEnough = .2f, lerpAt = 16;

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
        return newLoc;
    }

    IEnumerator Move()
    {
        while(Vector2.Distance(transform.position, newLoc) > closeEnough)
        {
            transform.position = Vector2.Lerp(transform.position, newLoc, Time.deltaTime * lerpAt);
            yield return null;
        }

        transform.position = newLoc;
    }

    public void MoveTo(Vector2 newLoc)
    {
        this.newLoc = newLoc;
        StartCoroutine(Move());
    }
    public void MoveDirectlyTo(Vector2 newLoc)
    {
        this.newLoc = newLoc;
        transform.position = newLoc;
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
