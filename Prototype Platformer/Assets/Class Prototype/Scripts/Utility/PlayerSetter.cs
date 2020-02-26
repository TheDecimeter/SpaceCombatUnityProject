using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetter : MonoBehaviour
{
    private static readonly string[] nameOf = { "Bibbs", "Leslie", "Giggles", "Hobbs" };

    public GameObject HUD;
    public GameObject PlayerArray;
    // Start is called before the first frame update
    void Start()
    {
        //TODO link huds based on playercount
        LinkHud();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LinkHud()
    {
        foreach (Transform child in PlayerArray.transform)
        {
            string name = "HUD_" + nameOf[child.gameObject.GetComponent<CharacterMovement_Physics>().PlayerNumber];
            LinkHudMount(child, name, HUD);
            LinkHealth(child, name, HUD);
            LinkMenu(child, name, HUD);
            ////link menu
            //if (child.gameObject.GetComponent<CharacterMovement_Physics>().inGameMenu == null)
            //    child.gameObject.GetComponent<CharacterMovement_Physics>().inGameMenu
            //        = HUD.transform.Find(name + "/HUD/inGameMenu").gameObject;
            ////make sure menu is disabled
            //HUD.transform.Find(name + "/HUD/inGameMenu").gameObject.SetActive(false);

            ////link item name
            //if (child.gameObject.GetComponent<CharacterMovement_Physics>().HUDmount == null)
            //    child.gameObject.GetComponent<CharacterMovement_Physics>().HUDmount
            //        = HUD.transform.Find(name + "/HUD/ItemInfoMountPoint").gameObject;

            ////link health
            //if (child.gameObject.GetComponent<PlayerHealth>().health == null)
            //    child.gameObject.GetComponent<PlayerHealth>().health
            //        = HUD.transform.Find(name + "/HUD/Health").gameObject.GetComponent<Text>();
            //child.gameObject.GetComponent<PlayerHealth>().health.text
            //    = " " + child.gameObject.GetComponent<PlayerHealth>().getHealth();
        }
    }

    public void SwitchLinks(GameObject newHud)
    {
        foreach (Transform child in PlayerArray.transform)
        {
            string name = "HUD_" + nameOf[child.gameObject.GetComponent<CharacterMovement_Physics>().PlayerNumber];

            LinkHudMount(child, name, newHud);
            LinkHealth(child, name, newHud);
            LinkMenu(child, name, newHud);
            
        }
    }

    private void LinkMenu(Transform player, string playerName, GameObject newHud)
    {

        GameObject oldMenu = player.gameObject.GetComponent<CharacterMovement_Physics>().inGameMenu;

        GameObject newMenu = newHud.transform.Find(playerName + "/HUD/inGameMenu").gameObject;
        player.gameObject.GetComponent<CharacterMovement_Physics>().inGameMenu = newMenu;

        if(oldMenu)
            newMenu.SetActive(oldMenu.activeInHierarchy);
    }

    private void LinkHealth(Transform player, string playerName, GameObject newHud)
    {
        player.gameObject.GetComponent<PlayerHealth>().health
                    = newHud.transform.Find(playerName + "/HUD/Health").gameObject.GetComponent<Text>();
        player.gameObject.GetComponent<PlayerHealth>().health.text
            = " " + player.gameObject.GetComponent<PlayerHealth>().getHealth();
    }

    private void LinkHudMount(Transform player, string playerName, GameObject newHud)
    {
        List<Transform> hudItems = new List<Transform>();
        GameObject oldHudMount = player.gameObject.GetComponent<CharacterMovement_Physics>().HUDmount;
        if(oldHudMount)
            foreach (Transform hudItem in oldHudMount.transform)
                hudItems.Add(hudItem);

        //link item name
        GameObject newHudMount = newHud.transform.Find(playerName + "/HUD/ItemInfoMountPoint").gameObject;
        player.gameObject.GetComponent<CharacterMovement_Physics>().HUDmount = newHudMount;

        if(oldHudMount)
            foreach (Transform hudItem in hudItems)
                LinkHudItem(hudItem, newHudMount.transform);
    }

    private void LinkHudItem(Transform item, Transform parent)
    {
        item.SetParent(parent);
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;
    }
}
