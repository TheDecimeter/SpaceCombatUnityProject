using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BoolUpdater : MonoBehaviour
{
    public delegate void Get(bool input);
    public delegate void DataListener();
    [System.Serializable]
    public class GetEvent : UnityEvent<Get> { }
    [System.Serializable]
    public class DataListenerEvent : UnityEvent<UnityAction> { }
    [System.Serializable]
    public class SetEvent : UnityEvent<bool> { }
    
    private bool value;
    public GameObject check;

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
        value = GetVal(GetValue);
        UpdateDisplay();
    }

    public void Change()
    {
        value = !value;

        value = ConfirmVal();

        UpdateDisplay();
        //Menu.savedData.SetRounds(rounds);
    }

    private bool ConfirmVal()
    {
        SetValue.Invoke(value);
        bool newVal = GetVal(GetValue);
        return newVal;
    }

    private void OnEnable()
    {
        value = GetVal(GetValue);
        UpdateDisplay();
    }

    public static bool GetVal(GetEvent GetValue)
    {
        bool val=false;
        GetValue.Invoke((x) => { val = x; });
        return val;
    }

    private void UpdateDisplay()
    {
        check.SetActive(value);
    }
}
