using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControlEvents : MonoBehaviour
{
    public GameObject shade;
    private Vector2 newLoc;


    private const float closeEnough = .2f, lerpAt = 16;

    public UnityEvent L,R,S,B;

    // Start is called before the first frame update
    void Start()
    {
        newLoc = transform.position;
    }

    public Vector2 Loc()
    {
        return newLoc;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //if (move)
    //    //{
    //    //    if (Vector2.Distance(transform.position, newLoc) < closeEnough)
    //    //    {
    //    //        move = false;
    //    //        transform.position = newLoc;
    //    //    }
    //    //    else
    //    //    {
    //    //        transform.position = Vector2.Lerp(transform.position,newLoc, Time.deltaTime * lerpAt);
    //    //    }
    //    //}
    //}

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
}
