using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public HudLinks Link
    {
        get
        {
            return links;
        }
        set
        {
            if (links == null)
            {
                links = value;
                return;
            }

            //set menu
            for (int i = 0; i < links.Menu.Length; ++i)
                CarryOverMenuSettings(links.Menu[i], value.Menu[i]);

            //set healths
            for (int i = 0; i < links.Menu.Length; ++i)
                CarryOverHealth(links.Health[i], value.Health[i]);

            //set item infos
            for (int i = 0; i < links.Menu.Length; ++i)
                CarryOverWeaponInfo(links.WeaponInfo[i], value.WeaponInfo[i]);

            links = value;
        }
    }

    private HudLinks links;

    

    private void CarryOverMenuSettings(GameObject oldm, GameObject newm)
    {
        newm.SetActive(oldm.activeInHierarchy);
    }
    
    private void CarryOverHealth(Text oldh, Text newh)
    {
        newh.text = oldh.text;
    }
    
    private void CarryOverWeaponInfo(GameObject oldw, GameObject neww)
    {
        foreach (Transform t in oldw.transform)
            TransferWeaponInfo(t, neww.transform);
    }
    
    private void TransferWeaponInfo(Transform item, Transform parent)
    {
        print("Transfered weapon info for " + item.gameObject + " to " + parent.gameObject);
        item.SetParent(parent,false);
        //item.parent = parent;
        //item.localPosition = Vector3.zero;
        //item.localRotation = Quaternion.identity;
    }
}
