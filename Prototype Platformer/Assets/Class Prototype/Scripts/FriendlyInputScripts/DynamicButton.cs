using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicButton : MonoBehaviour
{
    public abstract void UpdateButton(bool active);
    public abstract bool IsActive();
}
