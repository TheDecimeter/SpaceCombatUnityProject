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

    public string Aweapon()
    {
        string r = getName();
        if (r.Contains("fisticufs"))
            return r;
        if (r.Contains("Oxygen\nTank"))
            return "an " + r;
        return "a " + r;
    }

    public int Rank()
    {
        string w = getName();
        switch (w)
        {
            case "fisticufs":
                return 1;
            case "Shiv":
                return 6;
            case "Guards\nGun":
                return 6;
            case "Trash Can\nLid":
                return 3;
            case "Oxygen\nTank":
                return 10;
            case "Lethal\nInjection":
                return 9;
            default:
                return int.MaxValue;
        }
    }
}
