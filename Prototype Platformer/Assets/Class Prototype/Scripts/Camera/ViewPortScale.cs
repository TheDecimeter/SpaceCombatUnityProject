using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortScale : MonoBehaviour
{
    public Transform horizontalParent;
    private Transform verticalParent;
    private Vector2 resolution;
    // Start is called before the first frame update
    Canvas canvas;
    int players = 4;

    void Awake()
    {
        verticalParent = transform.parent;
        resolution = new Vector2(Screen.width, Screen.height);
        Transform current = transform;
        while (current)
        {
            canvas = current.GetComponent<Canvas>();
            if (canvas)
                break;
            current = current.parent;
        }


    }

    private void setDim(bool vertical)
    {

        switch (players)
        {
            case 1:
                SetScale1();
                break;

            case 2:
                SetScale2(vertical);
                break;

            case 3:
                SetScale3();
                break;
            default:
                SetScale4();
                break;
        }
    }


    void Start()
    {
        players = GetPlayers();
        //Debug.Log("player count " + players);
        setDim(GetVerticalSplit());
    }

    private void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            setDim(GetVerticalSplit());

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }
    }

    public void Reset(bool vertical)
    {
        setDim(vertical);
    }

    private void SetScale4()
    {

        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;

        RectTransform tr = GetComponent<RectTransform>();

        float hUnit = h / -4;
        float wUnit = w / 4;
        tr.localPosition = new Vector2(wUnit, hUnit);
        tr.sizeDelta = new Vector2(w / 2, h / 2);
    }
    private void SetScale3()
    {

        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;

        RectTransform tr = GetComponent<RectTransform>();

        float hUnit = h / -4;
        float wUnit = w / 4;
        tr.localPosition = new Vector2(wUnit, hUnit);
        tr.sizeDelta = new Vector2(w / 2, h / 2);
    }

    private void SetScale2(bool vertical)
    {

        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;

        RectTransform tr = GetComponent<RectTransform>();


        if (vertical)
        {
            if(horizontalParent!=null)
                transform.SetParent(verticalParent);

            //Debug.Log("setting vert viewPortFuzzy for " + transform.parent.gameObject.name);
            float hUnit = h / -2;
            float wUnit = w / 4;

            tr.localPosition = new Vector3(wUnit, hUnit, 1);
            tr.sizeDelta = new Vector2(w / 2, h);
        }
        else
        {
            //Debug.Log("setting horiz viewPortFuzzy for " + transform.parent.gameObject.name);
            if (horizontalParent!=null)
            {
                //Debug.Log("setting parent for " + transform.parent.gameObject.name);
                transform.SetParent(horizontalParent);
            }

            float hUnit = h / -4;
            float wUnit = w / 2;

            tr.localPosition = new Vector3(wUnit, hUnit, 1);
            tr.sizeDelta = new Vector2(w, h/2);
            
        }
    }
    private void SetScale1()
    {

        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;

        RectTransform tr = GetComponent<RectTransform>();

        float hUnit = h / -2;
        float wUnit = w / 2;
        tr.localPosition = new Vector3(wUnit, hUnit, 1);
        tr.sizeDelta = new Vector2(w, h);
    }


    private int GetPlayers()
    {
        int r = 0;
        FindObjectOfType<UndestroyableData>().GetPlayers((x) => { r = x; });
        return r;
    }
    private bool GetVerticalSplit()
    {
        bool r = false;
        FindObjectOfType<UndestroyableData>().GetVerticalScreenSplit((x) => { r = x; });
        return r;
    }
}
