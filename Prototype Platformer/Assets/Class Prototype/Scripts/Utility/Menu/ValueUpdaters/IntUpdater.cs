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
    
    private int value;
    public TextMeshProUGUI Display;

    //partial min/max. If there is a more constraining min/max in the setter function, it will
    // take control.
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
        value = GetVal(GetValue);
        UpdateDisplay();
    }

    public void ChangeBy(int diff)
    {
        int wrapDir = 0;
        value += diff;
        if (value < Min)
        {
            value = Max;
            wrapDir = 1;
        }

        if (value > Max)
        {
            wrapDir = -1;
            value = Min;
        }

        print("change val " + value);
        value = ConfirmVal(wrapDir);
        print("constrained val " + value);
        UpdateDisplay();
        //Menu.savedData.SetRounds(rounds);
    }

    private int ConfirmVal(int wrapDir)
    {
        int newVal = BoundValue(value);
        //SetValue.Invoke(value);
        //int newVal = GetVal(GetValue);
        if (newVal != value)
        {
            if (wrapDir == 0)
            {
                if (newVal < value)
                {//if I have gone above a legal value, wrap down
                    newVal = Min;
                    value = Min;
                    SetValue.Invoke(value);
                    newVal = GetVal(GetValue);
                }


                else
                {//if I have gone below a legal value, wrap up
                    newVal = Max;
                    value = Max;
                    SetValue.Invoke(value);
                    newVal = GetVal(GetValue);
                }
            }
            else
            {
                if (wrapDir<0)
                {//if I have gone above a legal value, wrap down
                    newVal = Min;
                    value = Min;
                    SetValue.Invoke(value);
                    newVal = GetVal(GetValue);
                }


                else
                {//if I have gone below a legal value, wrap up
                    newVal = Max;
                    value = Max;
                    SetValue.Invoke(value);
                    newVal = GetVal(GetValue);
                }
            }

        }

        return newVal;
    }

    private void OnEnable()
    {
        if (!Display)
            Display = GetComponent<TextMeshProUGUI>();
        //rounds = Menu.savedData.GetRounds();
        value = GetVal(GetValue);
        value=BoundValue(value);
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
        Display.text = "" + value;
    }

    private int BoundValue(int value)
    {
        value = Mathf.Clamp(value, Min, Max);
        SetValue.Invoke(value);
        value = GetVal(GetValue);
        return value;
    }
}
