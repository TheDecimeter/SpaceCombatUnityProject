using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortScale : MonoBehaviour
{
    private Vector2 resolution;
    // Start is called before the first frame update
    void Awake()
    {
        resolution = new Vector2(Screen.width, Screen.height);
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
        Canvas canvas = FindObjectOfType<Canvas>();

        float h = canvas.GetComponent<RectTransform>().rect.height;
        float w = canvas.GetComponent<RectTransform>().rect.width;

        RectTransform tr = GetComponent<RectTransform>();

        float hUnit = h / -4;
        float wUnit = w / 4;
        tr.localPosition = new Vector3(wUnit, hUnit, 1);
        tr.sizeDelta = new Vector2(w / 2, h / 2);
    }
}
