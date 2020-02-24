using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    public UndestroyableData SaveData;
    public Transform ObjectToRotate;
    public int PlayerNumber;
    public ControlLayouts [] MobileControlLayouts;
    
    [System.Serializable]
    public struct ControlLayouts
    {
        public GameObject Controls;
        public float Rotation;
        

    }

    private int currentRot = 0;

    private void Start()
    {
        Rotate(SaveData.GetRotation(PlayerNumber));
    }

    public void Rotate()
    {
        Rotate(currentRot + 1);
    }

    private void Rotate(int index)
    {
        MobileControlLayouts[currentRot].Controls.SetActive(false);
        currentRot=index;
        if (currentRot == MobileControlLayouts.Length)
            currentRot = 0;

        SaveData.SetRotation(PlayerNumber,currentRot);

        MobileControlLayouts[currentRot].Controls.SetActive(true);

        ObjectToRotate.rotation = Quaternion.Euler(0, 0, MobileControlLayouts[currentRot].Rotation);
    }
}
