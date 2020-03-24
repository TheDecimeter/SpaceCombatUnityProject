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

            //Init new Controls;
            value.MenuControls.Init();

            //set ItemButton
            for (int i = 0; i < links.PickupButton.Length; ++i)
                CarryOverButtonInfo(links.PickupButton[i], value.PickupButton[i]);
            //set DoorButton
            for (int i = 0; i < links.DoorButton.Length; ++i)
                CarryOverButtonInfo(links.DoorButton[i], value.DoorButton[i]);

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
    

    private void CarryOverButtonInfo(DynamicButton oldb, DynamicButton newb)
    {
        newb.UpdateButton(oldb.IsActive());
    }

    private void CarryOverMenuSettings(InGameMenuManager oldm, InGameMenuManager newm)
    {
        newm.SetState(oldm);
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
        //print("Transfered weapon info for " + item.gameObject + " to " + parent.gameObject);
        item.SetParent(parent,false);
        //item.parent = parent;
        //item.localPosition = Vector3.zero;
        //item.localRotation = Quaternion.identity;
    }

    public void ControllerListener1(ControlStruct newControls)
    {
        if (Link == null)
        {
            return;
        }
        else if (Link.MenuControls == null)
            print("MenuControls is null");

        //print("hud control listener 1");

        Link.MenuControls.ControllerListener1(newControls);
    }
    public void ControllerListener2(ControlStruct newControls)
    {
        if (Link == null)
        {
            return;
        }
        Link.MenuControls.ControllerListener2(newControls);
    }
    public void ControllerListener3(ControlStruct newControls)
    {
        if (Link == null)
        {
            return;
        }
        Link.MenuControls.ControllerListener3(newControls);
    }
    public void ControllerListener4(ControlStruct newControls)
    {
        if (Link == null)
        {
            return;
        }
        Link.MenuControls.ControllerListener4(newControls);
    }

    public void ReturnToTitleMenu()
    {
        FindObjectOfType<UndestroyableData>().OpenStartMenu();
        SceneLoader loader = FindObjectOfType<SceneLoader>();

        loader.sceneLoadDelay = 0;
        loader.sceneFadeDuration = 0;
        loader.RestartScene();
    }

    public void CloseMenus()
    {
        foreach (InGameMenuManager m in Link.Menu)
            m.SetMenuActive(0, false);
    }
}
