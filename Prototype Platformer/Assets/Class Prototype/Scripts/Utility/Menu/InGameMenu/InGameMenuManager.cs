using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    public GameObject ControllerMenu;
    public GameObject KeyboardMenu;
    public ControlManager GlobalMenu;


    public void SetState(InGameMenuManager oldm)
    {
        ControllerMenu.SetActive(oldm.ControllerMenu.activeInHierarchy);
        KeyboardMenu.SetActive(oldm.KeyboardMenu.activeInHierarchy);

        if (oldm.GlobalMenu != GlobalMenu)
        {
            GlobalMenuActive(oldm.GlobalMenu.gameObject.activeInHierarchy);
            oldm.GlobalMenuActive(false);
        }
    }


    public void SetMenuActive(int source, bool active)
    {
        if (active)
        {
            if (source == ControlStruct.Controller || ControlStruct.IsDevice(source))
                SetControllerMenuActive();
            else if (source == ControlStruct.Keyboard)
                SetKeyboardMenuActive();
            else
                SetGlobalMenuActive();
        }
        else
        {
            DeactivateMenus();
        }

    }
    public void SetGlobalMenuActive(bool active)
    {
        SetGlobalMenuActive(active, null);
    }
    public void SetGlobalMenuActive(bool active, ControlStruct c)
    {
        if (active)
        {
            SetGlobalMenuActive();
            GlobalMenu.SetIgnoreController(c);
        }
        else
            GlobalMenuActive(false);
    }

    private void SetControllerMenuActive()
    {
        ControllerMenu.SetActive(true);
        KeyboardMenu.SetActive(false);
        GlobalMenuActive(false);
    }
    private void SetKeyboardMenuActive()
    {
        ControllerMenu.SetActive(false);
        KeyboardMenu.SetActive(true);
        GlobalMenuActive(false);
    }
    private void SetGlobalMenuActive()
    {
        ControllerMenu.SetActive(false);
        KeyboardMenu.SetActive(false);
        GlobalMenuActive(true);
    }

    public void DeactivateMenus()
    {
        ControllerMenu.SetActive(false);
        KeyboardMenu.SetActive(false);
        GlobalMenuActive(false);
    }

    private void GlobalMenuActive(bool active)
    {
        GlobalMenu.gameObject.SetActive(active);
        GlobalMenu.enabled = active;
    }

    public bool IsMenuActive()
    {
        return (ControllerMenu.activeInHierarchy || KeyboardMenu.activeInHierarchy);
    }

    public bool IsGlobalMenuActive()
    {
        if (!GlobalMenu)
            return false;
        return GlobalMenu.gameObject.activeInHierarchy;
    }

    public void ReturnToTitleMenu()
    {
        FindObjectOfType<UndestroyableData>().OpenStartMenu();
        SceneLoader loader = FindObjectOfType<SceneLoader>();

        loader.sceneLoadDelay = 0;
        loader.sceneFadeDuration = 0;
        loader.RestartScene();
    }

}
