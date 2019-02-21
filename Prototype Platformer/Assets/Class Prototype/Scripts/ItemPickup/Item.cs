using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public const int Ranged = 0, Stab = 1, Punch=-1;
    public GameObject itemExterior;
    public abstract int getType();
    public abstract string getName();
    public abstract void use(Transform targetList, Transform user);
}
