using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateRounds : MonoBehaviour
{
    public StartMenu Menu;
    private int rounds;
    public TextMeshProUGUI Display;

    private void Start()
    {
        rounds = Menu.savedData.GetRounds();
    }

    public void ChangeBy(int diff)
    {
        rounds += diff;
        if (rounds < 1)
            rounds = Menu.MaxRounds;
       
        if (rounds > Menu.MaxRounds)
            rounds = 1;
        UpdateDisplay();
    }

    private void OnEnable()
    {
        if (!Display)
            Display = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.text = "" + rounds;
    }

    private void OnDestroy()
    {
        Menu.savedData.SetRounds(rounds);
    }
}
