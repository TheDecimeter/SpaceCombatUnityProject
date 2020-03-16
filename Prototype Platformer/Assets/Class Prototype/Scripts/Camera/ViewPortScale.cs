using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortScale : MonoBehaviour
{
    public Transform horizontalParent;
    private Vector2 resolution;
    // Start is called before the first frame update
    Canvas canvas;
    int players = 4;
    private Vector2 pos, size;
    void Awake()
    {
        resolution = new Vector2(Screen.width, Screen.height);
        Transform current = transform;
        while (current)
        {
            canvas = current.GetComponent<Canvas>();
            if (canvas)
                break;
            current = current.parent;
        }
        players = GetPlayers();
        print("player count " + players);

    }

    private void setDim()
    {

        switch (players)
        {
            case 1:
                SetScale1();
                break;

            case 2:
                SetScale2(false);
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
        setDim();
    }

    private void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            setDim();

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }
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
            print("setting vert viewPortFuzzy for " + transform.parent.gameObject.name);
            float hUnit = h / -2;
            float wUnit = w / 4;

            tr.localPosition = new Vector3(wUnit, hUnit, 1);
            tr.sizeDelta = new Vector2(w / 2, h);
        }
        else
        {
            print("setting horiz viewPortFuzzy for " + transform.parent.gameObject.name);
            if (horizontalParent!=null)
            {
                print("setting parent for " + transform.parent.gameObject.name);
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
}
