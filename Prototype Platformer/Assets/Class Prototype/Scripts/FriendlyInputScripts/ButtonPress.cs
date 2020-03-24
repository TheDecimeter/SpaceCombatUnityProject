using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonPress : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private RawImage image;
    private Color clicked, notClicked;
    public PressEvent Controls;

    [System.Serializable]
    public class PressEvent : UnityEvent<bool> { }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = clicked;
        Controls.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = notClicked;
        Controls.Invoke(false);
    }

    private void OnDisable()
    {
        if (!image)
            return;
        image.color = notClicked;
        Controls.Invoke(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
        notClicked = image.color;
        clicked = new Color(notClicked.r, notClicked.g, notClicked.b, notClicked.a / 2);
    }
}
