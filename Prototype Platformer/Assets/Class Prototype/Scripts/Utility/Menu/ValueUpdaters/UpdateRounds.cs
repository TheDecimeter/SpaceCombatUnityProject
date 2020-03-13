using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateRounds : MonoBehaviour
{
    public StartMenu Menu;
    private int rounds;
    public TextMeshProUGUI Display;

    private void Awake()
    {
        Menu.savedData.AddDataReadEvent(UpdateSavedData);
    }
    private void UpdateSavedData()
    {
        rounds = Menu.savedData.GetRounds();
        UpdateDisplay();
    }

    public void ChangeBy(int diff)
    {
        rounds += diff;
        if (rounds < 1)
            rounds = Menu.MaxRounds;
       
        if (rounds > Menu.MaxRounds)
            rounds = 1;
        UpdateDisplay();
        Menu.savedData.SetRounds(rounds);
    }

    private void OnEnable()
    {
        if (!Display)
            Display = GetComponent<TextMeshProUGUI>();
        rounds = Menu.savedData.GetRounds();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.text = "" + rounds;
    }
    
}
