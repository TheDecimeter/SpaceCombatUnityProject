using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class IntUpdater : MonoBehaviour
{
    public delegate void Get(int input);
    public delegate void DataListener();
    [System.Serializable]
    public class GetEvent : UnityEvent<Get> { }
    [System.Serializable]
    public class DataListenerEvent : UnityEvent<UnityAction> { }
    [System.Serializable]
    public class SetEvent : UnityEvent<int> { }
    
    private int rounds;
    public TextMeshProUGUI Display;

    public int Max, Min;

    public GetEvent GetValue;
    public SetEvent SetValue;
    public DataListenerEvent FireWhenDataIsReady;

    private void Awake()
    {
        //Menu.savedData.AddDataReadEvent(UpdateSavedData);
        FireWhenDataIsReady.Invoke(UpdateSavedData);
    }
    private void UpdateSavedData()
    {
        //rounds = Menu.savedData.GetRounds();
        rounds = GetVal(GetValue);
        UpdateDisplay();
    }

    public void ChangeBy(int diff)
    {
        rounds += diff;
        if (rounds < 1)
            rounds = Max;

        if (rounds > Max)
            rounds = Min;
        UpdateDisplay();
        //Menu.savedData.SetRounds(rounds);
        SetValue.Invoke(rounds);
    }

    private void OnEnable()
    {
        if (!Display)
            Display = GetComponent<TextMeshProUGUI>();
        //rounds = Menu.savedData.GetRounds();
        rounds = GetVal(GetValue);
        UpdateDisplay();
    }

    public static int GetVal(GetEvent GetValue)
    {
        int val=0;
        GetValue.Invoke((x) => { val = x; });
        return val;
    }

    private void UpdateDisplay()
    {
        Display.text = "" + rounds;
    }
}
