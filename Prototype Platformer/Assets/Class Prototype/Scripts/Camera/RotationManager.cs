using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    public UndestroyableData SaveData;
    public PlayerSetter playerLinks;
    [Header("Cameras")]
    public Transform Player1Cam;
    public Transform Player2Cam, Player3Cam, Player4Cam;

    [Header("Rotation Configurations")]
    public RotationConfigurations [] configurations;
    
    [System.Serializable]
    public struct RotationConfigurations
    {
        public GameObject Hud;
        public float Cam1Rot, Cam2Rot, Cam3Rot, Cam4Rot;
        

    }

    private int currentRot = 0;

    private void Start()
    {
        Rotate(SaveData.GetRotation());
    }

    public void Rotate()
    {
        Rotate(currentRot + 1);
    }

    private void Rotate(int index)
    {
        if(index>=configurations.Length)
            index%=configurations.Length;

        GameObject oldHud = configurations[currentRot].Hud;
        GameObject newHud = configurations[index].Hud;

        //Set Links
        playerLinks.SwitchLinks(newHud);


        oldHud.SetActive(false);
        newHud.SetActive(true);

        currentRot=index;

        SaveData.SetRotation(currentRot);

        Player1Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam1Rot);
        Player2Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam2Rot);
        Player3Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam3Rot);
        Player4Cam.rotation = Quaternion.Euler(0, 0, configurations[currentRot].Cam4Rot);

        SwitchWinMessages(newHud);
    }

    private void SwitchWinMessages(GameObject newHud)
    {
        string txt = SaveData.WinText.text;
        SaveData.WinText.text = "";
        SaveData.WinText = newHud.transform.Find("WinMessage").GetComponent<TextMeshProUGUI>();
        SaveData.WinText.text = txt;
    }
}
