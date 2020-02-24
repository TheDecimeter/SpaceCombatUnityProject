using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    public Transform ObjectToRotate;
    public ControlLayouts [] MobileControlLayouts;
    
    [System.Serializable]
    public struct ControlLayouts
    {
        public GameObject Controls;
        public float Rotation;
    }

    private int currentRot = 0;
    

    public void Rotate()
    {
        MobileControlLayouts[currentRot].Controls.SetActive(false);
        currentRot++;
        if (currentRot == MobileControlLayouts.Length)
            currentRot = 0;

        MobileControlLayouts[currentRot].Controls.SetActive(true);

        ObjectToRotate.rotation = Quaternion.Euler(0, 0, MobileControlLayouts[currentRot].Rotation);
    }
}
