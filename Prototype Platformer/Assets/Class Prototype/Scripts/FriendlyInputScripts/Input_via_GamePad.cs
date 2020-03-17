﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_GamePad : MonoBehaviour
{
    [System.Serializable]
    public class ControlEvent : UnityEvent<ControlStruct> { }

    GamepadInput _input;

    public ControlEvent controller1;
    public ControlEvent controller2;
    public ControlEvent controller3;
    public ControlEvent controller4;
    

    public int maxPlayers = 4;

    private static GamepadDevice [] devices;

#if !UNITY_ANDROID
    private List<ControlStruct> previousControls;
    //Axis 4=lefttrig, 5=righttrig, 0=leftAnalogH

    //Button A=0, B=1, X=2, Y=3, Rbumper=12


    private const int start=5, jump = 0, attackButton = 2, attackAxis1=4, attackAxis2 = 5, action = 3, door1=1, door2=12, door3 = 11, left = 0, right = 0;
    private const int AXIS = 1, BUTTON = 0;
    private float axisThreshold = 0.2f;

    private bool printOnce = true;

    public GamepadInput input
    {
        get
        {
            if (!_input)
                _input = GetComponent<GamepadInput>();
            return _input;
        }
    }
    

    // Use this for initialization
    void Start()
    {

        if (devices == null)
        {
            print("Creating/initializing device map");
            devices = new GamepadDevice[4];
        }

        previousControls = new List<ControlStruct>(maxPlayers);
        for (int i = 0; i < maxPlayers; ++i)
            previousControls.Add(new ControlStruct(ControlStruct.Controller));
    }
    
    private int DeviceIndex(GamepadDevice d)
    {
        if (IsDeviceAt(d.deviceId, d))
            return d.deviceId;
        for (int i = 0; i < 4; ++i)
            if (IsDeviceAt(i,d))
                return i;
        return -1;
    }

    private bool IsDeviceAt(int index, GamepadDevice d)
    {
        if (devices[index] == null)
            return false;
        if (devices[index] == d)
            return true;
        if (d.deviceId == devices[index].deviceId)
        {
            devices[index] = d;
            return true;
        }
        return false;
    }

    private void RemoveDeviceAt(int index)
    {
        devices[index] = null;
    }
    private void FireDevice(int index,ControlStruct playerControls)
    {
        if (index == 0) controller1.Invoke(playerControls);
        if (index == 1) controller2.Invoke(playerControls);
        if (index == 2) controller3.Invoke(playerControls);
        if (index == 3) controller4.Invoke(playerControls);
    }
    
    

    //Update is called once per frame
    void Update()
    {
        //if (!Input.GetKey(KeyCode.LeftShift))
        {
            
            if (input == null)
                print("input is null");


            //if (input.gamepads.Count == 0)
            //{
            //    print("no gamepads connected");
            //}

            int playerNum = 0;

            List<ControlStruct> Cs = new List<ControlStruct>(4);
            List<GamepadDevice> Ds = new List<GamepadDevice>(4);

            foreach (GamepadDevice gamepad in input.gamepads)
            {
                //create a structure for holding controls
                ControlStruct playerControls = new ControlStruct(ControlStruct.Controller);


                int[] buttonValues = (int[])System.Enum.GetValues(typeof(GamepadButton));


                for (int i = 0; i < buttonValues.Length; i++)
                {

                    //print("Not left shift " + (GamepadButton)buttonValues[i] + ": " + gamepad.GetButton((GamepadButton)buttonValues[i]) + "\n");
                    //print("Not left shift " + i + ": " + gamepad.GetButton((GamepadButton)buttonValues[i]) + "\n");
                    updateControl(i, playerControls, gamepad.GetButton((GamepadButton)buttonValues[i]));
                        
                    
                        
                }

                int[] axisValues = (int[])System.Enum.GetValues(typeof(GamepadAxis));
                for (int i = 0; i < axisValues.Length; i++)
                {
                    if (gamepad.GetAxis((GamepadAxis)axisValues[i]) != 0)
                    {
                        
                        //print("Not left shift " + (GamepadAxis)axisValues[i] + ": " + gamepad.GetAxis((GamepadAxis)axisValues[i]) + "\n");
                        updateControl(i, playerControls, gamepad.GetAxis((GamepadAxis)axisValues[i]));
                        
                    }
                        
                }


                //pass the controls on to the player through an event
                //keep track of gamepads by ID, rather than player number
                //so that if a controller becomes unplugged, it doesn't shift
                //all controllers down an index.

                Ds.Add(gamepad);
                Cs.Add(playerControls);

                //if (gamepad.deviceId == 0) controller1.Invoke(playerControls);
                //if (gamepad.deviceId == 1) controller2.Invoke(playerControls);
                //if (gamepad.deviceId == 2) controller3.Invoke(playerControls);
                //if (gamepad.deviceId == 3) controller4.Invoke(playerControls);
                playerNum++;

                //if (printOnce)
                    //print("gamePad " + gamepad.deviceId + " " + gamepad.displayName + " -sys: " + gamepad.systemName);
            }
            if (printOnce)
            {
                print("controllers found: " + (playerNum));
                foreach (GamepadDevice d in Ds)
                    print("  " + d.deviceId + ", " + d.systemName + ", " + d.displayName);
                printOnce = false;
            }

            bool[] changed = new bool[4];

            //Invoke known devices
            for(int i=0; i<Ds.Count; ++i)
            {
                int index = DeviceIndex(Ds[i]);
                if (index == -1)
                    continue;
                changed[index] = true;
                FireDevice(index, Cs[i]);
            }

            //Assign and invoke uknown devices
            for (int i = 0; i < Ds.Count; ++i)
            {
                int index = DeviceIndex(Ds[i]);
                if (index != -1)
                    continue;

                int freeSpot=GetFirstUnchanged(changed);
                devices[freeSpot] = Ds[i];
                FireDevice(freeSpot, Cs[i]);
            }

            //remove old devices
            PurgeUnchangedDevices(changed);
            
        }

    }

    private int GetFirstUnchanged(bool [] changed)
    {
        for(int i=0; i<changed.Length; ++i)
        {
            if(!changed[i])
            {
                changed[i] = true;
                return i;
            }
        }
        return -1;
    }
    private void PurgeUnchangedDevices(bool [] changed)
    {
        for (int i = 0; i < changed.Length; ++i)
        {
            if (!changed[i])
            {
                devices[i] = null;
            }
        }
    }

    private bool isControlUpdated(int currentControl, ControlStruct previous, bool state)
    {
        switch (currentControl)
        {
            case jump:
                if (previous.jump == state)
                    return false;
                break;
            case attackButton:
                if (previous.attack == state)
                    return false;
                break;
            case action:
                if (previous.action == state)
                    return false;
                break;
            default:
                return false;
        }
        return true;
    }
    private void updateControl(int currentControl, ControlStruct current, bool state)
    {
        if(state)
            switch (currentControl)
            {
                case jump:
                    current.jump = true;
                    break;
                case attackButton:
                    current.attack = true;
                    break;
                case action:
                    current.action = true;
                    break;
                case door1:
                    current.B = true;
                    current.door = true;
                    break;
                case door2:
                case door3:
                    current.door = true;
                    break;
                case start:
                    current.inGameMenu = true;
                    break;
            }
    }
    private bool isControlUpdated(int currentControl, ControlStruct previous, float state)
    {
        switch (currentControl)
        {
            case left:
                if (previous.moveLeft >= state - axisThreshold && previous.moveLeft <= state + axisThreshold)
                    return false;
                break;
        }
        return true;
    }
    private void updateControl(int currentControl, ControlStruct current, float state)
    {
        //print("current control " + currentControl + " state " + state);

        switch (currentControl)
        {
            case left:
                current.moveLeft = state;
                break;
            case attackAxis1:
            case attackAxis2:
                if (state > axisThreshold)
                    current.attack = true;
                break;
        }
        return;
    }

    
#endif


}