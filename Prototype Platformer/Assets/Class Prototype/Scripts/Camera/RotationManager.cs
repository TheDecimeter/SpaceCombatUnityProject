using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    public UndestroyableData SaveData;
    //public PlayerSetter playerLinks;
    public HUD hud;
    [Header("Cameras")]
    public Transform Player1Cam;
    public Transform Player2Cam, Player3Cam, Player4Cam;

    [Header("Rotation Configurations")]
    public RotationConfigurations [] configurations;
    
    [System.Serializable]
    public struct RotationConfigurations
    {
        public HudLinks Hud;
        public float Cam1Rot, Cam2Rot, Cam3Rot, Cam4Rot;
        public bool VerticalSplit;
    }

    private int currentRot = 0;

    private void Start()
    {
        Rotate(SaveData.GetRotation());
        hud.Link.MenuControls.gameObject.SetActive(false);
    }

    public void Rotate()
    {
        Rotate(currentRot + 1);
    }

    public void SetRotate(int index)
    {
        if (index == currentRot)
            return;
        Rotate(index);
    }
    public void GetRotate(IntUpdater.Get ret)
    {
        ret(currentRot);
    }

    private void Rotate(int index)
    {
        if (index < 0)
            index = configurations.Length - 1;
        else if(index>=configurations.Length)
            index%=configurations.Length;

        HudLinks oldHud = configurations[currentRot].Hud;
        HudLinks newHud = configurations[index].Hud;

        //Set Links
        //playerLinks.SwitchLinks(newHud);
        hud.Link = newHud;
        
        oldHud.gameObject.SetActive(false);
        newHud.gameObject.SetActive(true);

        currentRot =index;

        SaveData.SetRotation(currentRot);

        Player1Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam1Rot);
        Player2Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam2Rot);
        Player3Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam3Rot);
        Player4Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam4Rot);

        SwitchWinMessages(newHud.gameObject);
    }

    private void SwitchWinMessages(GameObject newHud)
    {
        string txt = SaveData.WinText.text;
        SaveData.WinText.text = "";
        SaveData.WinText = newHud.transform.Find("WinMessage").GetComponent<TextMeshProUGUI>();
        SaveData.WinText.text = txt;
    }
}
