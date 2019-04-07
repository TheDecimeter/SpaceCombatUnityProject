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
            //if (child.gameObject.GetComponent<CharacterMovement_Physics>().currentItemHUD == null)
            //    child.gameObject.GetComponent<CharacterMovement_Physics>().currentItemHUD
            //        = HUD.transform.Find(name+"/Item").gameObject.GetComponent<Text>();
            if (child.gameObject.GetComponent<PlayerHealth>().health == null)
                child.gameObject.GetComponent<PlayerHealth>().health
                    = HUD.transform.Find(name + "/Health").gameObject.GetComponent<Text>();
            child.gameObject.GetComponent<PlayerHealth>().health.text
                = " " + child.gameObject.GetComponent<PlayerHealth>().getHealth();



        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
