using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{

    public string effect = "none";
    protected static AudioManager audio;
    public const int Ranged = 0, Stab = 1, Punch=-1;
    public GameObject itemExterior;
    public string spawnLocation="Any";
    public abstract int getType();
    public abstract string getName();
    public abstract string getAnimationFlag();
    public abstract GameObject getInUseHUD();
    public abstract GameObject getPickUpHUD();
    public abstract void use(Transform targetList, Transform user);
}
