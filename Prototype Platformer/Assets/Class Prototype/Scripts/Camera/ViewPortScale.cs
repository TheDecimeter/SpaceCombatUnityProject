using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortScale : MonoBehaviour
{
    private Vector2 resolution;
    // Start is called before the first frame update
    Canvas canvas;
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
    }

    void Start()
    {
        SetScale();
    }

    private void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            SetScale();

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }
    }

    private void SetScale()
    {

        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;

        RectTransform tr = GetComponent<RectTransform>();

        float hUnit = h / -4;
        float wUnit = w / 4;
        tr.localPosition = new Vector3(wUnit, hUnit, 1);
        tr.sizeDelta = new Vector2(w / 2, h / 2);
    }
}
