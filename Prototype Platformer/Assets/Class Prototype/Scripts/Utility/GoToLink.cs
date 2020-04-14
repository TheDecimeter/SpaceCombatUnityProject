using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GoToLink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string Link;
    TextMeshProUGUI text;
    private Color hover,normal;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text)
        {
            if(text.color.a>.5f)
                hover = new Color(text.color.r, text.color.g, text.color.b, text.color.a / 2);
            else
                hover = new Color(text.color.r, text.color.g, text.color.b, 1);
            normal = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("open url " + Link);
        Application.OpenURL(Link);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(text)
            text.color = hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(text)
            text.color = normal;
    }
}
