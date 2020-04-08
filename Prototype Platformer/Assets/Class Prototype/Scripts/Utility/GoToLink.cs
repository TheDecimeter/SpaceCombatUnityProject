using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoToLink : MonoBehaviour, IPointerClickHandler
{
    public string Link;

    private void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("open url " + Link);
        Application.OpenURL(Link);
    }
}
