using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowButtonWhenNeeded : DynamicButton
{
    public override bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

    public override void UpdateButton(bool active)
    {
        gameObject.SetActive(active);
    }
}
