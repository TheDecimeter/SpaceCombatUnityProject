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
        foreach (Transform child in PlayerArray.transform)
        {
            string name = "HUD_"+nameOf[child.gameObject.GetComponent<CharacterMovement_Physics>().PlayerNumber];
            if (child.gameObject.GetComponent<CharacterMovement_Physics>().inGameMenu == null)
                child.gameObject.GetComponent<CharacterMovement_Physics>().inGameMenu
                    = HUD.transform.Find(name + "/HUD/inGameMenu").gameObject;

            HUD.transform.Find(name + "/HUD/inGameMenu").gameObject.SetActive(false);

            if (child.gameObject.GetComponent<PlayerHealth>().health == null)
                child.gameObject.GetComponent<PlayerHealth>().health
                    = HUD.transform.Find(name + "/HUD/Health").gameObject.GetComponent<Text>();
            child.gameObject.GetComponent<PlayerHealth>().health.text
                = " " + child.gameObject.GetComponent<PlayerHealth>().getHealth();



        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
