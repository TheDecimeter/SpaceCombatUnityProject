using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortScale1 : MonoBehaviour
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

        float h = canvas.GetComponent<RectTransform>().rect.height * canvas.GetComponent<RectTransform>().localScale.x;
        float w = canvas.GetComponent<RectTransform>().rect.width * canvas.GetComponent<RectTransform>().localScale.y;

        RectTransform tr = GetComponent<RectTransform>();
        
        tr.localPosition = new Vector3(0, 0, 1);
        tr.sizeDelta = new Vector2(w, h);
    }
}
